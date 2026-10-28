-- PulseTrack Fleet Maintenance & Repair Order Schema
-- Target: SQL Server 2019 / 2022

CREATE TABLE dbo.WorkOrders (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_WorkOrders PRIMARY KEY CLUSTERED,
    OrderNumber NVARCHAR(30) NOT NULL CONSTRAINT UQ_WorkOrders_OrderNumber UNIQUE,
    Vin NVARCHAR(17) NOT NULL,
    Year INT NOT NULL,
    Make NVARCHAR(50) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    CustomerPhone NVARCHAR(25) NOT NULL,
    CurrentStage INT NOT NULL, -- 1=Intake, 2=Teardown, 3=Body, 4=Paint, 5=QC, 6=Ready
    Priority INT NOT NULL,     -- 1=Standard, 2=Expedited, 3=FleetCritical
    TotalEstimate DECIMAL(18, 2) NOT NULL,
    CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_WorkOrders_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_WorkOrders_UpdatedAt DEFAULT SYSUTCDATETIME(),
    AssignedTechnician NVARCHAR(100) NOT NULL,
    ShopLocation NVARCHAR(100) NOT NULL,
    RepairNotes NVARCHAR(MAX) NOT NULL
);
GO

CREATE TABLE dbo.PartsCatalog (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_PartsCatalog PRIMARY KEY CLUSTERED,
    PartNumber NVARCHAR(50) NOT NULL CONSTRAINT UQ_PartsCatalog_PartNumber UNIQUE,
    Description NVARCHAR(255) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    InStockQuantity INT NOT NULL,
    UnitCost DECIMAL(18, 2) NOT NULL,
    BinLocation NVARCHAR(50) NOT NULL,
    OEMSupplier NVARCHAR(100) NOT NULL
);
GO
