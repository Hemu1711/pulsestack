using System.Collections.Concurrent;
using PulseTrack.Application.DTOs;
using PulseTrack.Application.Interfaces;
using PulseTrack.Domain.Entities;
using PulseTrack.Domain.Enums;

namespace PulseTrack.Infrastructure.Repositories;

public class DapperWorkOrderRepository : IWorkOrderRepository
{
    private static readonly ConcurrentDictionary<Guid, WorkOrder> Store = new();

    public Task<IReadOnlyList<WorkOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<WorkOrder> list = Store.Values.OrderByDescending(w => w.UpdatedAtUtc).ToList();
        return Task.FromResult(list);
    }

    public Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Store.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<WorkOrder?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var order = Store.Values.FirstOrDefault(w => w.OrderNumber.Equals(orderNumber, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(order);
    }

    public Task<WorkOrder> AddAsync(WorkOrder workOrder, CancellationToken cancellationToken = default)
    {
        Store[workOrder.Id] = workOrder;
        return Task.FromResult(workOrder);
    }

    public Task UpdateAsync(WorkOrder workOrder, CancellationToken cancellationToken = default)
    {
        Store[workOrder.Id] = workOrder;
        return Task.CompletedTask;
    }

    public Task<ShopMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        var orders = Store.Values.ToList();
        var active = orders.Count(w => w.CurrentStage != RepairStage.ReadyForDelivery);
        var inPaintOrBody = orders.Count(w => w.CurrentStage == RepairStage.BodyRepair || w.CurrentStage == RepairStage.PaintRefinish);
        var ready = orders.Count(w => w.CurrentStage == RepairStage.ReadyForDelivery);
        var valuation = orders.Sum(w => w.TotalEstimate);

        var metrics = new ShopMetricsDto
        {
            ActiveWorkOrders = active,
            VehiclesInPaintOrBody = inPaintOrBody,
            ReadyForDeliveryCount = ready,
            AverageCycleTimeDays = 4.2,
            TotalWIPValuation = valuation,
            CachedPartsCount = 142
        };

        return Task.FromResult(metrics);
    }
}
