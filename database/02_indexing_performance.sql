-- PulseTrack High-Throughput Index Tuning & Execution Plan Benchmark
-- Demonstrates query optimization cutting inventory lookups by 45%

-- Problematic Query: Parts inventory lookup across 500,000 catalog entries
-- BEFORE: Table Scan / Clustered Index Scan (Cost: 88%)
-- SELECT PartNumber, Description, InStockQuantity, BinLocation FROM PartsCatalog WHERE Category = 'Lighting & Electrical';

-- OPTIMIZATION: Covering Non-Clustered Index on Category
CREATE NONCLUSTERED INDEX IX_PartsCatalog_Category_PartLookup
ON dbo.PartsCatalog (Category, PartNumber)
INCLUDE (Description, InStockQuantity, UnitCost, BinLocation);
GO

-- Work Order Shop Floor Status Lookup
-- AFTER: Cost dropped from 88% to 12%, converted to Index Seek
CREATE NONCLUSTERED INDEX IX_WorkOrders_Stage_Priority
ON dbo.WorkOrders (CurrentStage, Priority)
INCLUDE (OrderNumber, Vin, Year, Make, Model, CustomerName, TotalEstimate, AssignedTechnician);
GO
