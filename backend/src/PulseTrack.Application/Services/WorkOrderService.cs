using PulseTrack.Application.DTOs;
using PulseTrack.Application.Interfaces;
using PulseTrack.Domain.Entities;
using PulseTrack.Domain.Enums;

namespace PulseTrack.Application.Services;

public class WorkOrderService
{
    private readonly IWorkOrderRepository _repository;
    private readonly ICacheService _cache;
    private const string WorkOrdersCacheKey = "pulsetrack:workorders:all";

    public WorkOrderService(IWorkOrderRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IReadOnlyList<WorkOrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetAsync<IReadOnlyList<WorkOrderDto>>(WorkOrdersCacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(MapToDto).ToList();

        await _cache.SetAsync(WorkOrdersCacheKey, dtos, TimeSpan.FromSeconds(30), cancellationToken);
        return dtos;
    }

    public async Task<WorkOrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<WorkOrderDto> CreateAsync(CreateWorkOrderRequest request, CancellationToken cancellationToken = default)
    {
        var entity = WorkOrder.Create(
            request.Vin,
            request.Year,
            request.Make,
            request.Model,
            request.CustomerName,
            request.CustomerPhone,
            request.Priority,
            request.TotalEstimate,
            request.AssignedTechnician,
            request.RepairNotes);

        var saved = await _repository.AddAsync(entity, cancellationToken);
        await _cache.RemoveAsync(WorkOrdersCacheKey, cancellationToken);

        return MapToDto(saved);
    }

    public async Task<WorkOrderDto> TransitionStageAsync(Guid id, RepairStage nextStage, string notes, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Work order {id} was not found.");
        }

        entity.TransitionStage(nextStage, notes);
        await _repository.UpdateAsync(entity, cancellationToken);
        await _cache.RemoveAsync(WorkOrdersCacheKey, cancellationToken);

        return MapToDto(entity);
    }

    public async Task<ShopMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetMetricsAsync(cancellationToken);
    }

    private static WorkOrderDto MapToDto(WorkOrder e) => new()
    {
        Id = e.Id,
        OrderNumber = e.OrderNumber,
        Vin = e.Vin,
        Year = e.Year,
        Make = e.Make,
        Model = e.Model,
        CustomerName = e.CustomerName,
        CustomerPhone = e.CustomerPhone,
        CurrentStage = e.CurrentStage,
        Priority = e.Priority,
        TotalEstimate = e.TotalEstimate,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
        AssignedTechnician = e.AssignedTechnician,
        ShopLocation = e.ShopLocation,
        RepairNotes = e.RepairNotes
    };
}
