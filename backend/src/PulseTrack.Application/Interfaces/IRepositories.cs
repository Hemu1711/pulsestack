using PulseTrack.Application.DTOs;
using PulseTrack.Domain.Entities;
using PulseTrack.Domain.Enums;

namespace PulseTrack.Application.Interfaces;

public interface IWorkOrderRepository
{
    Task<IReadOnlyList<WorkOrder>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkOrder?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<WorkOrder> AddAsync(WorkOrder workOrder, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkOrder workOrder, CancellationToken cancellationToken = default);
    Task<ShopMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default);
}

public interface IPartsCatalogRepository
{
    Task<IReadOnlyList<PartItem>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<PartItem?> GetByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<PartItem> items, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
