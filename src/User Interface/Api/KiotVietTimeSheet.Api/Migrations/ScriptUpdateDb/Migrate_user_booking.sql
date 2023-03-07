

-- migrate data user => employee
/* Hint
 * Replace KvBooking -with booking db
 * Replace KvTimeSheet with timesheet db
 */
 
USE KvTimeSheet

--[script debug] select *  FROM KvTimeSheet.dbo.Employee  WHERE CreatedBy = -1
--[script debug] backup data phụ cấp  dùng cho  rollback 
--[script debug] SELECT * FROM KvTimeSheet.dbo.Employee 132
--[script debug] SELECT * FROM KvBooking.dbo.[User] 32

DECLARE @testRetailer INT
SET @testRetailer = 592386

DROP TABLE IF EXISTS backup_employee_fromUser

SELECT 
		us.Id			AS EmployeeId,
		us.Id AS Code,
		us.GivenName AS Name,
		us.DOB,
		us.RetailerId	AS TenantId,
		ISNULL(us.BranchDefaultId,b.Id)	AS BranchId,
		us.isDeleted,
		us.MobilePhone,
		us.Email,
		us.Address,
		us.LocationName,
		us.WardName,
		us.Note,		
		us.IsActive,	
		us.CreatedDate,
		us.CreatedBy,
		us.Id			AS UserId,
		0 AS IsUpdated			
INTO backup_employee_fromUser
FROM KvBooking.dbo.[User] us
INNER JOIN KvBooking.dbo.[Branch] AS b ON B.Id = ( SELECT TOP 1 Id FROM  KvBooking.dbo.[Branch] br
																 WHERE br.RetailerId = us.RetailerId AND br.IsActive = 1
																 ORDER BY ID ASC)	
--WHERE us.RetailerId = @testRetailer															 

CREATE CLUSTERED INDEX IX_backup_employee_fromUser_Id ON backup_employee_fromUser(EmployeeId)
CREATE NONCLUSTERED INDEX IX_backup_employee_fromUser_UserId ON backup_employee_fromUser(UserId)


-- migrate nhan vien theo chi nhanh
DROP TABLE IF EXISTS backup_employee_branch_fromUser

SELECT BranchId, TenantId,EmployeeId
INTO backup_employee_branch_fromUser
FROM 
   (SELECT  u.RetailerId AS TenantId,
			P.BranchId		AS BranchId,
			u.Id			AS EmployeeId
	FROM	KvBooking.dbo.[User]			AS u
			INNER JOIN		KvBooking.dbo.[Permission] AS p 
										 ON u.RetailerId = p.RetailerId AND u.Id = p.UserId AND u.IsAdmin <> 1
    --WHERE u.RetailerId = @testRetailer
    
	UNION ALL
	SELECT  u.RetailerId AS TenantId,
			b.Id			AS BranchId,
			u.Id			AS BookingEmployeeId
	FROM	KvBooking.dbo.[User]	AS u
		INNER JOIN	KvBooking.dbo.[Branch] AS b
						ON u.RetailerId = b.RetailerId AND u.IsAdmin = 1) A
    --WHERE TenantId = @testRetailer
    
--[script debug]  SELECT * FROM backup_employee_fromUser
--[script debug]  SELECT * FROM backup_employee_branch_fromUser
--[script debug] SELECT * FROM Employee WHERE TenantId = 592386  ORDER BY CreatedDate desc


-- [script debug] select * FROM employee WHERE id IN (SELECT EmployeeId FROM KvTimeSheet.dbo.backup_employee_fromUser)

 --select* FROM employee as e 
 --WHERE id IN (SELECT EmployeeId FROM KvTimeSheet.dbo.backup_employee_fromUser)
 --ORDER BY TenantId
 
USE KvTimeSheet

DROP TABLE IF EXISTS employee_existed_fromUser

SELECT Id 
INTO employee_existed_fromUser
FROM Employee WITH(NOLOCK)
WHERE Id IN (SELECT EmployeeId FROM backup_employee_fromUser WITH(NOLOCK))

--[script debug] SELECT * FROM employee_existed_fromUser

UpdateMore:
	DECLARE @NextId BIGINT
	SET @NextId=  (SELECT top 1 EmployeeId FROM backup_employee_fromUser WHERE IsUpdated = 0)
	DECLARE @NextTenantId BIGINT
	SET @NextTenantId=  (SELECT top 1 TenantId FROM backup_employee_fromUser WHERE EmployeeId = @NextId)
	
	DECLARE @NextCode NVARCHAR(50)
	
	 DECLARE @Identity BIGINT = 1;
		DECLARE @MaxCode NVARCHAR(64);
		
		SELECT TOP 1 @MaxCode = Code
		FROM Employee 
		WHERE TenantId = @NextTenantId
		AND  Code LIKE N'NV[0-9]%' 
		AND SUBSTRING(Code, LEN(N'NV') + 1, LEN(Code)) NOT LIKE N'%[^0-9]%'  
		AND LEN(SUBSTRING(Code, LEN('NV') + 1, LEN(Code))) <= 10
		ORDER BY Code DESC
		
		IF @MaxCode IS NULL
		BEGIN		
		    SET @Identity = 1
		END
		ELSE
		BEGIN		
			DECLARE @CurrentIdentity NVARCHAR(64) 
			SELECT @CurrentIdentity = REPLACE(@MaxCode, 'NV','')
			SELECT @Identity = CAST( @CurrentIdentity AS BIGINT) + 1; 			
		END	
	SET @NextCode = (SELECT 'NV'+ RIGHT(Replicate('0', 6) + CAST(@Identity AS VARCHAR), 6))
	
	
	INSERT KvTimeSheet.dbo.Employee 
		(
			Id,
			UserId,
			Code,
			Name,
			DOB,
			Gender,--
			TenantId,
			BranchId,
			IsDeleted,
			MobilePhone,
			Email,
			Address,
			LocationName,
			WardName,
			Note,
			IsActive,
			CreatedDate,
			CreatedBy
		)
	SELECT
			e.EmployeeId,
			e.UserId,
			@NextCode,
			e.Name,
			e.DOB,
			NULL,
			e.TenantId,
			e.BranchId,
			e.IsDeleted,
			e.MobilePhone,
			e.Email,
			e.Address,
			e.LocationName,
			e.WardName,
			e.Note,
			e.IsActive,
			GETDATE(),
			e.CreatedBy
		
	FROM backup_employee_fromUser e
	WHERE  e.EmployeeId NOT IN (SELECT id FROM employee_existed_fromUser)
	AND EmployeeId = @NextId	
	
	INSERT KvTimeSheet.dbo.EmployeeBranch 
	(
		Id,
		TenantId,
		BranchId,
		EmployeeId
	)
	SELECT
		(NEXT VALUE FOR [EmployeeBranchSeq]),
		eb.TenantId,
		eb.BranchId,
		eb.EmployeeId
	FROM backup_employee_branch_fromUser eb
	WHERE  eb.EmployeeId NOT IN (SELECT id FROM employee_existed_fromUser)
	AND EmployeeId = @NextId	
	
	UPDATE backup_employee_fromUser SET  IsUpdated = 1 WHERE EmployeeId = @NextId	
	
IF @@ROWCOUNT != 0
		GOTO UpdateMore;	
	

-- DROP  table employee_existed_fromUser
-- DROP table backup_employee_fromUser
-- DROP table backup_employee_branch_fromUser

SELECT * FROM backup_employee_fromUser
--SELECT e.UserId, *
--  FROM Employee AS e WHERE e.Id IN (SELECT Id FROM employee_existed_fromUser)
--AND e.TenantId = 200000143

--SELECT UserId, * FROM Employee AS e WHERE e.Code = 'NV000008' AND e.TenantId = 200000143


SELECT TOP 2 * FROM Employee AS eb
ORDER BY eb.Id DESC