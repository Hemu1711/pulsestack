using System.Collections.Concurrent;
using PulseTrack.Application.Interfaces;
using PulseTrack.Domain.Entities;

namespace PulseTrack.Infrastructure.Repositories;

public class DapperPartsCatalogRepository : IPartsCatalogRepository
{
    private static readonly ConcurrentDictionary<string, PartItem> Catalog = new(StringComparer.OrdinalIgnoreCase);

    public Task<IReadOnlyList<PartItem>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            IReadOnlyList<PartItem> top = Catalog.Values.Take(15).ToList();
            return Task.FromResult(top);
        }

        var term = query.Trim().ToUpperInvariant();
        IReadOnlyList<PartItem> matches = Catalog.Values
            .Where(p => p.PartNumber.Contains(term) ||
                        p.Description.ToUpperInvariant().Contains(term) ||
                        p.Category.ToUpperInvariant().Contains(term))
            .Take(20)
            .ToList();

        return Task.FromResult(matches);
    }

    public Task<PartItem?> GetByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default)
    {
        Catalog.TryGetValue(partNumber.Trim(), out var item);
        return Task.FromResult(item);
    }

    public Task AddRangeAsync(IEnumerable<PartItem> items, CancellationToken cancellationToken = default)
    {
        foreach (var item in items)
        {
            Catalog[item.PartNumber] = item;
        }
        return Task.CompletedTask;
    }

    public Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Catalog.Count);
    }
}
