using PulseTrack.Application.DTOs;
using PulseTrack.Application.Interfaces;
using PulseTrack.Domain.Entities;

namespace PulseTrack.Application.Services;

public class PartsCatalogService
{
    private readonly IPartsCatalogRepository _repository;
    private readonly ICacheService _cache;

    public PartsCatalogService(IPartsCatalogRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IReadOnlyList<PartItemDto>> SearchPartsAsync(string query, CancellationToken cancellationToken = default)
    {
        var cleanTerm = query.Trim().ToUpperInvariant();
        var cacheKey = $"pulsetrack:parts:search:{cleanTerm}";

        var cached = await _cache.GetAsync<IReadOnlyList<PartItemDto>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var results = await _repository.SearchAsync(cleanTerm, cancellationToken);
        var dtos = results.Select(MapToDto).ToList();

        // High-speed caching for catalog lookups (10 minute TTL)
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10), cancellationToken);
        return dtos;
    }

    public async Task<PartItemDto?> GetByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default)
    {
        var clean = partNumber.Trim().ToUpperInvariant();
        var cacheKey = $"pulsetrack:part:{clean}";

        var cached = await _cache.GetAsync<PartItemDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var entity = await _repository.GetByPartNumberAsync(clean, cancellationToken);
        if (entity == null) return null;

        var dto = MapToDto(entity);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30), cancellationToken);
        return dto;
    }

    private static PartItemDto MapToDto(PartItem p) => new()
    {
        Id = p.Id,
        PartNumber = p.PartNumber,
        Description = p.Description,
        Category = p.Category,
        InStockQuantity = p.InStockQuantity,
        UnitCost = p.UnitCost,
        BinLocation = p.BinLocation,
        OEMSupplier = p.OEMSupplier
    };
}
