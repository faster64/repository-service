﻿CREATE PROCEDURE [dbo].[pr_Booking_Insert_TrialData_Deduction]
(
    @tenantId	INT,			-- ID gian hàng
	@userId		BIGINT,			-- ID tài khoản admin
	@deductionId BIGINT OUTPUT
)
AS
BEGIN
	SET @deductionId = NEXT VALUE FOR DeductionSeq

	INSERT INTO Deduction (Id, [Name], CreatedBy, CreatedDate, TenantId, Code, IsDeleted, [Value], ValueType, DeductionRuleId, DeductionTypeId, BlockTypeTimeValue, BlockTypeMinuteValue)
	VALUES        (@deductionId, N'Đi trễ', @userId, GETDATE(), @tenantId, 'GT000001', 0, 10000, 1, 1, 1, 1, 10)
END