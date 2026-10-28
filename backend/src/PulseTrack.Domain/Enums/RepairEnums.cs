namespace PulseTrack.Domain.Enums;

public enum RepairStage
{
    Intake = 1,
    Teardown = 2,
    BodyRepair = 3,
    PaintRefinish = 4,
    QualityControl = 5,
    ReadyForDelivery = 6
}

public enum PriorityLevel
{
    Standard = 1,
    Expedited = 2,
    FleetCritical = 3
}
