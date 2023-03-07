GO
USE [$(DatabaseName)];


GO
PRINT N'Altering [dbo].[Employee]...';


GO
ALTER TABLE [dbo].[Employee]
    ADD [IdentityKeyClocking] NVARCHAR (100) NULL,
        [AccountSecretKey]    NVARCHAR (100) NULL;


GO
PRINT N'Creating [dbo].[ConfirmClocking]...';


GO
CREATE TABLE [dbo].[ConfirmClocking] (
    [Id]                  BIGINT          NOT NULL,
    [TenantId]            INT             NOT NULL,
    [GpsInfoId]           BIGINT          NOT NULL,
    [EmployeeId]          BIGINT          NOT NULL,
    [CheckTime]           DATETIME2 (3)   NOT NULL,
    [Type]                TINYINT         NOT NULL,
    [Status]              TINYINT         NOT NULL,
    [Note]                NVARCHAR (2000) NULL,
    [CreatedBy]           BIGINT          NOT NULL,
    [CreatedDate]         DATETIME2 (3)   NOT NULL,
    [ModifiedBy]          BIGINT          NULL,
    [ModifiedDate]        DATETIME2 (3)   NULL,
    [IsDeleted]           BIT             NOT NULL,
    [DeletedBy]           BIGINT          NULL,
    [DeletedDate]         DATETIME2 (3)   NULL,
    [IdentityKeyClocking] NVARCHAR (100)  NULL,
    [Extra]               NVARCHAR (4000) NULL,
    CONSTRAINT [PK_ConfirmClocking] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[ConfirmClocking].[IX_ConfirmClocking_EmployeeId]...';


GO
CREATE NONCLUSTERED INDEX [IX_ConfirmClocking_EmployeeId]
    ON [dbo].[ConfirmClocking]([EmployeeId] ASC);


GO
PRINT N'Creating [dbo].[ConfirmClocking].[NonClusteredIndex-ConfirmClocking-GpsInfoId]...';


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-ConfirmClocking-GpsInfoId]
    ON [dbo].[ConfirmClocking]([GpsInfoId] ASC);


GO
PRINT N'Creating [dbo].[ConfirmClocking].[NonClusteredIndex-ConfirmClocking-TenantId]...';


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-ConfirmClocking-TenantId]
    ON [dbo].[ConfirmClocking]([TenantId] ASC);


GO
PRINT N'Creating [dbo].[ConfirmClockingHistory]...';


GO
CREATE TABLE [dbo].[ConfirmClockingHistory] (
    [Id]                BIGINT          NOT NULL,
    [TenantId]          INT             NOT NULL,
    [ConfirmClockingId] BIGINT          NOT NULL,
    [ConfirmBy]         BIGINT          NOT NULL,
    [ConfirmDate]       DATETIME2 (3)   NOT NULL,
    [StatusOld]         TINYINT         NOT NULL,
    [StatusNew]         TINYINT         NOT NULL,
    [Note]              NVARCHAR (2000) NULL,
    [CreatedBy]         BIGINT          NOT NULL,
    [CreatedDate]       DATETIME2 (3)   NOT NULL,
    [ModifiedBy]        BIGINT          NULL,
    [ModifiedDate]      DATETIME2 (3)   NULL,
    [IsDeleted]         BIT             NOT NULL,
    [DeletedBy]         BIGINT          NULL,
    [DeletedDate]       DATETIME2 (3)   NULL,
    CONSTRAINT [PK_ConfirmClockingHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[ConfirmClockingHistory].[NonClusteredIndex-ConfirmClockingHistory-ConfirmClockingId]...';


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-ConfirmClockingHistory-ConfirmClockingId]
    ON [dbo].[ConfirmClockingHistory]([ConfirmClockingId] ASC);


GO
PRINT N'Creating [dbo].[ConfirmClockingHistory].[NonClusteredIndex-ConfirmClockingHistory-TenantId]...';


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-ConfirmClockingHistory-TenantId]
    ON [dbo].[ConfirmClockingHistory]([TenantId] ASC);


GO
PRINT N'Creating [dbo].[GpsInfo]...';


GO
CREATE TABLE [dbo].[GpsInfo] (
    [Id]           BIGINT          NOT NULL,
    [TenantId]     INT             NOT NULL,
    [BranchId]     INT             NOT NULL,
    [Coordinate]   NVARCHAR (2000) NULL,
    [Address]      NVARCHAR (100)  NULL,
    [LocationName] NVARCHAR (100)  NULL,
    [WardName]     NVARCHAR (100)  NULL,
    [Province]     NVARCHAR (100)  NULL,
    [District]     NVARCHAR (100)  NULL,
    [Status]       TINYINT         NOT NULL,
    [QrKey]        NVARCHAR (100)  NOT NULL,
    [CreatedBy]    BIGINT          NOT NULL,
    [CreatedDate]  DATETIME2 (3)   NOT NULL,
    [ModifiedBy]   BIGINT          NULL,
    [ModifiedDate] DATETIME2 (3)   NULL,
    [IsDeleted]    BIT             NOT NULL,
    [DeletedBy]    BIGINT          NULL,
    [DeletedDate]  DATETIME2 (3)   NULL,
    [RadiusLimit]  INT             NOT NULL,
    CONSTRAINT [PK_GpsInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[GpsInfo].[NonClusteredIndex-GpsInfo-TenantId]...';


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-GpsInfo-TenantId]
    ON [dbo].[GpsInfo]([TenantId] ASC);


GO
PRINT N'Creating unnamed constraint on [dbo].[ConfirmClocking]...';


GO
ALTER TABLE [dbo].[ConfirmClocking]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating unnamed constraint on [dbo].[ConfirmClockingHistory]...';


GO
ALTER TABLE [dbo].[ConfirmClockingHistory]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating unnamed constraint on [dbo].[GpsInfo]...';


GO
ALTER TABLE [dbo].[GpsInfo]
    ADD DEFAULT ((0)) FOR [RadiusLimit];


GO
PRINT N'Creating unnamed constraint on [dbo].[GpsInfo]...';


GO
ALTER TABLE [dbo].[GpsInfo]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating [dbo].[ConfirmClockingHistorySeq]...';


GO
CREATE SEQUENCE [dbo].[ConfirmClockingHistorySeq]
    AS BIGINT
    START WITH 1
    INCREMENT BY 100;


GO
PRINT N'Creating [dbo].[ConfirmClockingSeq]...';


GO
CREATE SEQUENCE [dbo].[ConfirmClockingSeq]
    AS BIGINT
    START WITH 1
    INCREMENT BY 100;


GO
PRINT N'Creating [dbo].[GpsInfoSeq]...';


GO
CREATE SEQUENCE [dbo].[GpsInfoSeq]
    AS BIGINT
    START WITH 1
    INCREMENT BY 100;


GO
PRINT N'Creating [dbo].[FK_ConfirmClocking_Employee_EmployeeId]...';


GO
ALTER TABLE [dbo].[ConfirmClocking] WITH NOCHECK
    ADD CONSTRAINT [FK_ConfirmClocking_Employee_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([Id]) ON DELETE CASCADE NOT FOR REPLICATION;


GO
PRINT N'Creating [dbo].[FK_ConfirmClocking_GpsInfo_GpsInfoId]...';


GO
ALTER TABLE [dbo].[ConfirmClocking] WITH NOCHECK
    ADD CONSTRAINT [FK_ConfirmClocking_GpsInfo_GpsInfoId] FOREIGN KEY ([GpsInfoId]) REFERENCES [dbo].[GpsInfo] ([Id]) ON DELETE CASCADE NOT FOR REPLICATION;


GO
PRINT N'Creating [dbo].[FK_ConfirmClockingHistory_ConfirmClocking_ConfirmClockingId]...';


GO
ALTER TABLE [dbo].[ConfirmClockingHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_ConfirmClockingHistory_ConfirmClocking_ConfirmClockingId] FOREIGN KEY ([ConfirmClockingId]) REFERENCES [dbo].[ConfirmClocking] ([Id]) ON DELETE CASCADE NOT FOR REPLICATION;


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[ConfirmClocking] WITH CHECK CHECK CONSTRAINT [FK_ConfirmClocking_Employee_EmployeeId];

ALTER TABLE [dbo].[ConfirmClocking] WITH CHECK CHECK CONSTRAINT [FK_ConfirmClocking_GpsInfo_GpsInfoId];

ALTER TABLE [dbo].[ConfirmClockingHistory] WITH CHECK CHECK CONSTRAINT [FK_ConfirmClockingHistory_ConfirmClocking_ConfirmClockingId];


GO
PRINT N'Update complete.';


GO