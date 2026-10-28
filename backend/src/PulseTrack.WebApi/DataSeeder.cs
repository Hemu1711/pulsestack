using PulseTrack.Application.Interfaces;
using PulseTrack.Domain.Entities;
using PulseTrack.Domain.Enums;

namespace PulseTrack.WebApi;

public static class DataSeeder
{
    public static async Task SeedAsync(IWorkOrderRepository workOrders, IPartsCatalogRepository parts)
    {
        var existing = await workOrders.GetAllAsync();
        if (existing.Any()) return;

        var orders = new List<WorkOrder>
        {
            CreateOrder("1FTFW1ED4MFA10492", 2021, "Ford", "F-150 SuperCrew", "Apex Logistics Fleet", "515-555-0810", PriorityLevel.FleetCritical, 4850m, "Marcus Vance", RepairStage.BodyRepair, "Front right fender and quarter panel collision repair in progress."),
            CreateOrder("4T1B11HK5JU294812", 2020, "Toyota", "Camry Hybrid", "Amanda Lewis", "515-555-0723", PriorityLevel.Standard, 2150m, "David Chen", RepairStage.Intake, "Insurance estimate approved. Awaiting teardown slot."),
            CreateOrder("WAUZZZF27NA019283", 2022, "Audi", "A4 Quattro", "Robert Sterling", "515-555-0644", PriorityLevel.Expedited, 7890m, "Elena Rostova", RepairStage.PaintRefinish, "Basecoat applied. Clearcoat baking cycle scheduled."),
            CreateOrder("3GNKBFRS5LS102941", 2021, "Chevrolet", "Traverse LT", "Metro Couriers Inc", "515-555-0551", PriorityLevel.FleetCritical, 3400m, "Marcus Vance", RepairStage.QualityControl, "Pre-delivery alignment and ADAS radar calibration check."),
            CreateOrder("1G1ZD5ST8HF182741", 2019, "Chevrolet", "Malibu", "Patricia Alvarez", "515-555-0412", PriorityLevel.Standard, 1920m, "David Chen", RepairStage.ReadyForDelivery, "Final detailing completed. Customer notified for pickup."),
            CreateOrder("JM1GL1V48M1109482", 2022, "Mazda", "CX-5 AWD", "Jordan Miller", "515-555-0391", PriorityLevel.Standard, 3100m, "Elena Rostova", RepairStage.Teardown, "Bumper cover removed. Hidden sensor bracket impact inspected.")
        };

        foreach (var o in orders)
        {
            await workOrders.AddAsync(o);
        }

        var partItems = new List<PartItem>
        {
            new("FO-10294-FNDR", "Front Right Steel Fender Panel", "Body & Sheetmetal", 8, 385.00m, "Aisle 4 - Shelf B2", "Motorcraft Direct"),
            new("TY-88412-BUMP", "Front Bumper Cover Fascia Primed", "Body & Trim", 14, 295.50m, "Aisle 2 - Shelf D1", "Toyota OEM Parts"),
            new("AU-55019-HLMP", "Matrix LED Headlight Assembly Passenger", "Lighting & Electrical", 4, 1240.00m, "Secure Bay 1", "Audi Genuine Parts"),
            new("GM-33201-RADS", "Heavy Duty Engine Cooling Radiator", "Mechanical & Cooling", 9, 310.00m, "Aisle 6 - Shelf C3", "ACDelco Supply"),
            new("MZ-90142-SENS", "Blind Spot Radar Detection Sensor", "ADAS & Electronics", 12, 420.00m, "Secure Bay 2", "Mazda Global Supply"),
            new("UN-11002-CLRC", "High-Gloss Polyurethane Clearcoat 1-Gal", "Paint & Refinish", 25, 145.00m, "Paint Mixing Lab", "PPG Automotive"),
            new("UN-22041-PRIM", "2K High-Build Epoxy Primer Gray 1-Gal", "Paint & Refinish", 18, 98.00m, "Paint Mixing Lab", "Axalta Coating")
        };

        await parts.AddRangeAsync(partItems);
    }

    private static WorkOrder CreateOrder(
        string vin, int year, string make, string model, string customer, string phone,
        PriorityLevel priority, decimal estimate, string tech, RepairStage stage, string notes)
    {
        var order = WorkOrder.Create(vin, year, make, model, customer, phone, priority, estimate, tech, notes);
        if (stage != RepairStage.Intake)
        {
            order.TransitionStage(stage, notes);
        }
        return order;
    }
}
