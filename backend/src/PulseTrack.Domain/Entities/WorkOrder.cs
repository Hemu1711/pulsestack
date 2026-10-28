using PulseTrack.Domain.Enums;

namespace PulseTrack.Domain.Entities;

public class WorkOrder
{
    public Guid Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public string Vin { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string Make { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public RepairStage CurrentStage { get; private set; }
    public PriorityLevel Priority { get; private set; }
    public decimal TotalEstimate { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public string AssignedTechnician { get; private set; } = string.Empty;
    public string ShopLocation { get; private set; } = "Shop #412 - Des Moines North";
    public string RepairNotes { get; private set; } = string.Empty;

    public WorkOrder() { }

    public static WorkOrder Create(
        string vin,
        int year,
        string make,
        string model,
        string customerName,
        string customerPhone,
        PriorityLevel priority,
        decimal totalEstimate,
        string assignedTechnician,
        string repairNotes)
    {
        var randomSuffix = Random.Shared.Next(1000, 9999);
        var orderNum = $"WO-{DateTime.UtcNow:yyMM}-{randomSuffix}";

        return new WorkOrder
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNum,
            Vin = vin.Trim().ToUpperInvariant(),
            Year = year,
            Make = make.Trim(),
            Model = model.Trim(),
            CustomerName = customerName.Trim(),
            CustomerPhone = customerPhone.Trim(),
            CurrentStage = RepairStage.Intake,
            Priority = priority,
            TotalEstimate = totalEstimate,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            AssignedTechnician = assignedTechnician.Trim(),
            RepairNotes = repairNotes.Trim()
        };
    }

    public void TransitionStage(RepairStage nextStage, string notes)
    {
        CurrentStage = nextStage;
        UpdatedAtUtc = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(notes))
        {
            RepairNotes = $"{RepairNotes}\n[{DateTime.UtcNow:MM/dd HH:mm}] {notes}".Trim();
        }
    }
}
