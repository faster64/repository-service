/*
 * SCRIPT TAO 
 */

-- 1 . tim PayRate co template = 0  lay duoc ma nv ,id ,SalaryPeriod, BranchId. EmployeeName
-- 2 . tim PayRateDetail tu id tim duoc buoc 1 -> lay duoc rulevalue, danh sach rule type
-- 3 . tao PayRateTemplate voi ten la 'Mẫu lương - ' + tên nhân viên, SalaryPeriod , BranchId
-- 4 . tạo PayRateTemplateDetail cho NV voi PayRateDetail tim duoc 

--0 
SET NOCOUNT ON

DECLARE @Total INT
--tao danh sach data 
DROP TABLE IF EXISTS tbl_handleData    
-- STEP 1  

SELECT pr.Id AS PayRateId,pr.EmployeeId,pr.TenantId,pr.CreatedBy,SalaryPeriod,e.Name AS EmployeeName,e.BranchId,pr.PayRateTemplateId, 0 AS IsUpdated
into tbl_handleData
FROM PayRate AS pr WITH (NOLOCK)
INNER JOIN Employee AS e WITH (NOLOCK) ON pr.EmployeeId = e.Id
WHERE (pr.PayRateTemplateId = 0 OR  pr.PayRateTemplateId IS NULL) 

CREATE CLUSTERED INDEX IX_handleData_PayRateId ON tbl_handleData(PayRateId )
CREATE NONCLUSTERED INDEX IX_handleData_IsUpdated ON tbl_handleData(IsUpdated )
-- print info 
SET @Total = (select COUNT(*) FROM tbl_handleData)
PRINT 'Total Handle PayRate : ' + CAST(@Total AS VARCHAR)
-- STEP 2 
DROP TABLE IF EXISTS tbl_HandleData_PayRateTemplateDetail 

DECLARE @TotalDetail INT

SELECT prd.RuleType,prd.RuleValue,prd.TenantId,prd.PayRateId,hd.EmployeeId,hd.EmployeeName,hd.BranchId,hd.SalaryPeriod,hd.CreatedBy
INTO tbl_HandleData_PayRateTemplateDetail
FROM PayRateDetail AS prd WITH(NOLOCK)  
INNER JOIN tbl_handleData as hd ON prd.PayRateId = hd.PayRateId
ORDER BY prd.TenantId,prd.PayRateId

CREATE CLUSTERED INDEX IX_handleData_PayRateTemplateDetail ON tbl_HandleData_PayRateTemplateDetail(PayRateId )
-- print info 
SET @TotalDetail = (select COUNT(*) FROM tbl_HandleData_PayRateTemplateDetail)
PRINT 'Total Handle PayRateTemplateDetail : ' + CAST(@TotalDetail AS VARCHAR)

-- STEP 3 
		
	UpdateMore:
		--get PayRateId can xu ly	
		DECLARE @PayRateIdNext BIGINT
	    SET @PayRateIdNext=  (SELECT top 1 PayrateId FROM tbl_handleData WHERE IsUpdated = 0)
		
		DECLARE @PayRateTemplateId BIGINT
		SET @PayRateTemplateId=  (NEXT VALUE FOR [PayRateTemplateSeq])
				
		    -- insert PayRateTemplate
			INSERT [dbo].[PayRateTemplate]
				   ([Id]
				   ,[TenantId]
				   ,[Name]
				   ,[SalaryPeriod]
				   ,[BranchId]
				   ,[CreatedDate]
				   ,[CreatedBy])
			 SELECT
				   @PayRateTemplateId
				   ,hd.TenantId
				   ,N'Mẫu lương - '+ hd.EmployeeName
				   ,hd.SalaryPeriod
				   ,hd.[BranchId]
				   ,GETDATE()
				   ,hd.[CreatedBy]
			 FROM tbl_handleData hd WHERE hd.PayRateId = @PayRateIdNext		 
			        
			-- insert PayRateTemplateDetail 
			
			INSERT INTO [dbo].[PayRateTemplateDetail]
			   ([TenantId]
			   ,[PayRateTemplateId]
			   ,[RuleType]
			   ,[RuleValue]
			   ,[CreatedBy]
			   ,[CreatedDate],
			   ModifiedBy)
			 SELECT
				prtd.TenantId
				,@PayRateTemplateId
				,prtd.RuleType
				,prtd.RuleValue
				,prtd.CreatedBy
				,GETDATE(),
				-1* prtd.CreatedBy
  			FROM tbl_HandleData_PayRateTemplateDetail prtd	
			WHERE prtd.PayRateId = @PayRateIdNext
			
			-- update status
			UPDATE tbl_handleData SET PayRateTemplateId = @PayRateTemplateId, IsUpdated = 1 WHERE PayRateId = @PayRateIdNext
		    UPDATE PayRate SET PayRateTemplateId = @PayRateTemplateId WHERE Id = @PayRateIdNext
	
		
	IF @@ROWCOUNT != 0
		GOTO UpdateMore;	
	