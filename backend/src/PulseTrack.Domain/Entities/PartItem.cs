namespace PulseTrack.Domain.Entities;

public class PartItem
{
    public Guid Id { get; private set; }
    public string PartNumber { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public int InStockQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public string BinLocation { get; private set; } = string.Empty;
    public string OEMSupplier { get; private set; } = string.Empty;

    public PartItem() { }

    public PartItem(string partNumber, string description, string category, int inStockQuantity, decimal unitCost, string binLocation, string supplier)
    {
        Id = Guid.NewGuid();
        PartNumber = partNumber.Trim().ToUpperInvariant();
        Description = description.Trim();
        Category = category.Trim();
        InStockQuantity = inStockQuantity;
        UnitCost = unitCost;
        BinLocation = binLocation.Trim();
        OEMSupplier = supplier.Trim();
    }
}
