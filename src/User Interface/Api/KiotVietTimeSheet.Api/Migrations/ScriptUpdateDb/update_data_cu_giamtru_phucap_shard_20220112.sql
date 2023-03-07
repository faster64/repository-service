
/*******************************************
 *  CHAY DU LIEU CAP NHAT CUOI CUNG CHO GIAM TRU, PHU CAP
 
 *******************************************/
-- chuan bi  table tam , chi chay 1 lan 

-- LƯU KẾT QUẢ PHỤ CẤP
DROP TABLE IF EXISTS [backup_Allowance_ruleValueDetails]
 
CREATE TABLE [dbo].[backup_Allowance_ruleValueDetails](
	[AllowanceId] BIGINT NULL,
	[Value] FLOAT NULL,
	[ValueRatio] FLOAT  NULL,	
	[Type] int  NULL,
	[Rank] FLOAT NULL,
	[IsChecked] BIt NULL
)

CREATE NONCLUSTERED INDEX IX_backup_Allowance_ruleValueDetails_Id ON [backup_Allowance_ruleValueDetails](AllowanceId)

-- LƯU KẾT QUẢ GIẢM TRỪ

DROP TABLE IF EXISTS [backup_Deduction_ruleValueDetails]

CREATE TABLE [dbo].[backup_Deduction_ruleValueDetails](
	[DeductionId] [bigint] NULL,	
	[Value] [decimal](18, 2) NULL,
	[ValueRatio] [float] NULL,
	[ValueType] [int] NULL,
	[DeductionRuleId] [int] NULL,
	[DeductionTypeId] [int] NULL,
	[BlockTypeTimeValue] [int] NULL,	
	[BlockTypeMinuteValue] [int] NULL
)

CREATE NONCLUSTERED INDEX IX_backup_Deduction_ruleValueDetails_Id ON [backup_Deduction_ruleValueDetails](DeductionId)
	
-- tim PayRateDetail RuleType = AllowanceRule và DeductionRule 
-- Nhom theo tenantid, sap xep theo CreatedDate, ModifiedDate DESC của PayRate
-- Đồng thời loại bỏ các PayRate của nhân viên đã xóa

DROP TABLE IF EXISTS [backup_update_ruleValueDetails]

SELECT 
	prd.Id AS Id,
	RuleValue as OrginRuleValue,
	--NULL as RuleValueObject,
	0 AS IsUpdated,
	prd.RuleType ,
	row_number() over ( partition BY pr.TenantId ORDER BY pr.ModifiedDate DESC, pr.CreatedDate DESC) PartitionSort
INTO [backup_update_ruleValueDetails]
FROM PayRateDetail AS prd
INNER JOIN PayRate AS pr ON pr.id = prd.PayRateId
INNER JOIN Employee AS e ON pr.EmployeeId = e.Id
WHERE ISNULL(e.IsDeleted,0) <> 1 
	  AND prd.RuleType  IN('DeductionRule','AllowanceRule')
	  AND LEN(CAST(prd.RuleValue AS NVARCHAR(4000))) > 33  
  
-- danh index dam bao performace 
-- ID CẦN GIỮ THỨ TỰ NÊN KHÔNG DÙNG CLUSTER INDEX
CREATE UNIQUE INDEX IX_backup_update_ruleValueDetails_Id ON [backup_update_ruleValueDetails](Id )
CREATE NONCLUSTERED INDEX IX_backup_update_ruleValueDetails_IsUpdated_RuleType ON [backup_update_ruleValueDetails](IsUpdated)
	
-- xoa du lieu dam bao ket qua moi 
DELETE from [backup_Allowance_ruleValueDetails]
DELETE from [backup_Deduction_ruleValueDetails] 
-- PARSE JSON VALUE TỪ TÙNG DÒNG
-- KIỂM TRẢ RULE TYPE VÀ INSERT KẾT QUẢ VÀO TABLE TẠM TƯỞNG ỨNG 
-- NẾU DEDUCTION HOẶC ALLOWANCE ID ĐÃ TỒN TẠI THÌ BỎ QUA KHÔNG UPDATE -> ĐẢM BẢO LẤY ID UPDATE SAU CÙNG 
--SELECT * FROM [backup_update_ruleValueDetails]

UpdateMore_Allowance:
	
	DECLARE @handleId BIGINT
	SET @handleId=  (SELECT top 1 Id FROM [backup_update_ruleValueDetails] WHERE IsUpdated = 0)
	
	DECLARE @handle_allowance_String NVARCHAR(4000)
	DECLARE @handle_type NVARCHAR(20)
	SET @handle_allowance_String= (SELECT top 1 OrginRuleValue FROM [backup_update_ruleValueDetails] WHERE Id = @handleId)
	SET @handle_type= (SELECT top 1 RuleType FROM [backup_update_ruleValueDetails] WHERE Id = @handleId)
	
	DECLARE @parse_JsonValue NVARCHAR(4000)
	SET @parse_JsonValue = ( SELECT [value] from OpenJson(@handle_allowance_String))
	 
	IF(@handle_type  = N'AllowanceRule')
		BEGIN
			INSERT dbo.[backup_Allowance_ruleValueDetails](
				AllowanceId,
				[Value],
				[ValueRatio],	
				[Type],
				[Rank],
				[IsChecked]
			)					
			SELECT [AllowanceId],[Value],[ValueRatio],[Type],[Rank],[IsChecked]
					
			from (
				SELECT [AllowanceId],[Value],[ValueRatio],[Type],[Rank],[IsChecked]
	
				FROM OpenJson(@parse_JsonValue)
				WITH (
				[AllowanceId] BIGINT '$.AllowanceId',
				[Value] FLOAT '$.Value',
				[ValueRatio] FLOAT '$.ValueRatio',
				[Type] int '$.Type',
				[Rank] FLOAT '$.Rank',
				[IsChecked] BIt '$.IsChecked'
			)
			) backup_llowance
			WHERE NOT EXISTS (SELECT * FROM [backup_Allowance_ruleValueDetails] AS a WHERE a.AllowanceId =backup_llowance.AllowanceId)
		END		
	ELSE  IF(@handle_type  = N'DeductionRule')
		BEGIN	
			
			INSERT dbo.[backup_Deduction_ruleValueDetails](
				DeductionId,
				[Value],
				[ValueRatio],
				[ValueType],
				[DeductionRuleId],
				[DeductionTypeId],
				[BlockTypeTimeValue],
				[BlockTypeMinuteValue]
			)
			SELECT DeductionId,[Value],[ValueRatio],[ValueType],[DeductionRuleId],[DeductionTypeId],[BlockTypeTimeValue],[BlockTypeMinuteValue]
			from (
				SELECT DeductionId,[Value],[ValueRatio],[ValueType],[DeductionRuleId],[DeductionTypeId],[BlockTypeTimeValue],[BlockTypeMinuteValue]	
				FROM OpenJson(@parse_JsonValue)
				WITH (
					[DeductionId] bigint '$.DeductionId',	
					[Value] decimal(18, 2) '$.Value',
					[ValueRatio] float '$.ValueRatio',
					[ValueType] int '$.Type',
					[DeductionRuleId] int '$.DeductionRuleId',
					[DeductionTypeId] int '$.DeductionTypeId',
					[BlockTypeTimeValue] int '$.BlockTypeTimeValue',	
					[BlockTypeMinuteValue] int '$.BlockTypeMinuteValue'
				)
			)backup_Deduction
			
			WHERE NOT EXISTS (SELECT * FROM [backup_Deduction_ruleValueDetails] AS a WHERE a.[DeductionId] =backup_Deduction.[DeductionId])
		END

		-- cap nhat trang thai 			  
		UPDATE [backup_update_ruleValueDetails] SET IsUpdated = 1 WHERE Id = @handleId	
		
IF @@ROWCOUNT != 0
	GOTO UpdateMore_Allowance;

-- cập nhật giá trị về giá trị gốc	
--backup data dùng cho  rollback 

DROP TABLE IF EXISTS backup_updated_Deduction

SELECT d.Id,d.[Value],d.[ValueRatio],d.[ValueType],d.[DeductionRuleId],d.[DeductionTypeId],d.[BlockTypeTimeValue],d.[BlockTypeMinuteValue]
INTO backup_updated_Deduction
FROM backup_Deduction_ruleValueDetails r
	JOIN Deduction AS d ON d.Id = r.DeductionId
	AND ISNULL(d.[Value],0) = 0 AND ISNULL(d.ValueRatio,0)=0
	
	
UPDATE d
SET
	[Value] = isnull (r.[Value],0),
	[ValueRatio]= isnull (r.[ValueRatio],0),
	[ValueType]= case when isnull (r.[Value],0) > 0 then 1 else 2 end ,
	[DeductionRuleId]= r.[DeductionRuleId],
	[DeductionTypeId]= r.[DeductionTypeId],
	[BlockTypeTimeValue]= r.[BlockTypeTimeValue],
	[BlockTypeMinuteValue]= r.[BlockTypeMinuteValue] 
	
FROM backup_Deduction_ruleValueDetails r
	JOIN Deduction AS d ON d.Id = r.DeductionId
	AND ISNULL(d.[Value],0) = 0 AND ISNULL(d.ValueRatio,0)=0
	
--backup data phụ cấp  dùng cho  rollback 
DROP TABLE IF EXISTS backup_updated_Allowance

SELECT a.Id,a.[Value],a.[ValueRatio],a.[Type],a.[Rank],a.[IsChecked]
INTO backup_updated_Allowance
FROM backup_Allowance_ruleValueDetails r
	JOIN Allowance AS a ON a.Id = r.AllowanceId
	AND a.[Value] = 0 AND a.ValueRatio = 0
-- update data phụ cấp 
	
UPDATE a
SET
	[Value] =isnull (r.[Value],0),
	[ValueRatio]= isnull (r.[ValueRatio],0),	
	[Type]=r.[Type],
	[Rank]=isnull (r.[Rank],0),
	[IsChecked]=isnull (r.[IsChecked],0)
FROM backup_Allowance_ruleValueDetails r
	JOIN Allowance AS a ON a.Id = r.AllowanceId
	AND a.[Value] = 0 AND a.ValueRatio = 0

 
 