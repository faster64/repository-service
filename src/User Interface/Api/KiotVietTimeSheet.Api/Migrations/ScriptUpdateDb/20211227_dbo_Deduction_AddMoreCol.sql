ALTER TABLE dbo.Deduction
ADD [Value]              DECIMAL(18, 2) NULL, 
    ValueRatio           FLOAT(53) NULL, 
    ValueType            INT NULL, 
    DeductionRuleId      INT NOT NULL
                             DEFAULT 0, 
    DeductionTypeId      INT NOT NULL
                             DEFAULT 1, 
    BlockTypeTimeValue   INT NOT NULL
                             DEFAULT 1, 
    BlockTypeMinuteValue INT NOT NULL
                             DEFAULT 1;