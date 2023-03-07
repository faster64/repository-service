CREATE FUNCTION [dbo].[GenerateCodePayslip] (@tenantId INT) 
RETURNS varchar(255) AS 
BEGIN 
	DECLARE @ReturnValues AS varchar(255), @Values AS varchar(255)
	DECLARE @MaxValue AS INT
	SELECT TOP 1 @MaxValue = CAST(RIGHT(Code, 6) AS INT) + 1, @Values = RIGHT(Code, 6) FROM Payslip WHERE TenantId = @tenantId ORDER BY Id DESC
	
	-- if null set default value
	SET @Values = ISNULL(@Values, '000000');
	SET @MaxValue = ISNULL(@MaxValue, 1);
	SET @ReturnValues = CAST(@MaxValue AS varchar(255))
	WHILE LEN(@ReturnValues) <= LEN(@Values)
	BEGIN 
		SET @ReturnValues = '0' + @ReturnValues 
	END

	SET @ReturnValues = 'PL' + @ReturnValues 
	RETURN @ReturnValues
END