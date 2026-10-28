# PulseTrack: Enterprise Fleet Maintenance & Collision Operations Platform

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Dapper](https://img.shields.io/badge/Dapper-Micro--ORM-orange.svg)](https://github.com/DapperLib/Dapper)
[![Redis](https://img.shields.io/badge/Redis-Distributed%20Cache-red.svg)](https://redis.io/)
[![WCAG 2.1 AA](https://img.shields.io/badge/WCAG%202.1-Level%20AA-success.svg)](https://www.w3.org/WAI/WCAG21/quickref/)

High-throughput shop floor management and parts inventory optimization platform engineered for multi-location automotive collision repair centers, processing thousands of daily repair order status transitions with sub-10ms query latency.

---

## Architectural Highlights

- **Dapper Micro-ORM:** Sub-10ms read pipelines for vehicle lookups and parts search.
- **Multi-Tier Caching:** Redis distributed cache abstraction with TTL invalidation on stage progression.
- **SQL Server Index Tuning:** Converted costly Clustered Index Scans (88% cost) into Covering Non-Clustered Index Seeks (12% cost).
- **Accessible Kanban Board:** Keyboard stage movement with live ARIA announcements, W3C ARIA 1.2 Combobox parts search.

---

## Project Structure

```
02-pulsetrack-fleet-ops/
├── backend/
│   ├── PulseTrack.sln
│   ├── src/
│   │   ├── PulseTrack.Domain/          # WorkOrder, PartItem, RepairStage
│   │   ├── PulseTrack.Application/     # Services, DTOs, ICacheService
│   │   ├── PulseTrack.Infrastructure/  # Dapper repositories, Redis Cache
│   │   └── PulseTrack.WebApi/          # Controllers, Swagger, Seeder
│   └── tests/
│       └── PulseTrack.UnitTests/       # 4 xUnit tests (100% Passed)
├── frontend/
│   ├── index.html                      # Accessible Kanban & Combobox UI
│   ├── css/                            # Dark mode tokens & Kanban layout
│   └── js/                             # Interactive stage transitions & search
└── database/
    ├── 01_schema.sql                   # WorkOrders & PartsCatalog tables
    └── 02_indexing_performance.sql     # Covering index tuning scripts
```

---

## Running the Project

```powershell
# Backend
cd 02-pulsetrack-fleet-ops/backend
dotnet run --project src/PulseTrack.WebApi

# Tests
dotnet test

# Frontend
npx serve 02-pulsetrack-fleet-ops/frontend -l 3001
```
