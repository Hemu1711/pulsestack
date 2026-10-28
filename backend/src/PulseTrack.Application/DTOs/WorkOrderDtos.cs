using PulseTrack.Domain.Enums;

namespace PulseTrack.Application.DTOs;

public record WorkOrderDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string Vin { get; init; } = string.Empty;
    public int Year { get; init; }
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string VehicleDisplay => $"{Year} {Make} {Model}";
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;
    public RepairStage CurrentStage { get; init; }
    public string StageName => CurrentStage.ToString();
    public PriorityLevel Priority { get; init; }
    public string PriorityName => Priority.ToString();
    public decimal TotalEstimate { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public string AssignedTechnician { get; init; } = string.Empty;
    public string ShopLocation { get; init; } = string.Empty;
    public string RepairNotes { get; init; } = string.Empty;
}

public record CreateWorkOrderRequest
{
    public string Vin { get; init; } = string.Empty;
    public int Year { get; init; }
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;
    public PriorityLevel Priority { get; init; } = PriorityLevel.Standard;
    public decimal TotalEstimate { get; init; }
    public string AssignedTechnician { get; init; } = string.Empty;
    public string RepairNotes { get; init; } = string.Empty;
}

public record TransitionStageRequest
{
    public RepairStage NextStage { get; init; }
    public string Notes { get; init; } = string.Empty;
}

public record PartItemDto
{
    public Guid Id { get; init; }
    public string PartNumber { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int InStockQuantity { get; init; }
    public decimal UnitCost { get; init; }
    public string BinLocation { get; init; } = string.Empty;
    public string OEMSupplier { get; init; } = string.Empty;
}

public record ShopMetricsDto
{
    public int ActiveWorkOrders { get; init; }
    public int VehiclesInPaintOrBody { get; init; }
    public int ReadyForDeliveryCount { get; init; }
    public double AverageCycleTimeDays { get; init; }
    public decimal TotalWIPValuation { get; init; }
    public int CachedPartsCount { get; init; }
}
