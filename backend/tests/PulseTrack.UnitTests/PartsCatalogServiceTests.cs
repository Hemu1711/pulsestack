using FluentAssertions;
using Moq;
using PulseTrack.Application.DTOs;
using PulseTrack.Application.Interfaces;
using PulseTrack.Application.Services;
using PulseTrack.Domain.Entities;
using Xunit;

namespace PulseTrack.UnitTests;

public class PartsCatalogServiceTests
{
    private readonly Mock<IPartsCatalogRepository> _repoMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly PartsCatalogService _service;

    public PartsCatalogServiceTests()
    {
        _service = new PartsCatalogService(_repoMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task SearchPartsAsync_CacheHit_ReturnsFromCacheWithoutQueryingRepo()
    {
        var cachedList = new List<PartItemDto>
        {
            new() { PartNumber = "TY-88412-BUMP", Description = "Front Bumper Cover", UnitCost = 295.50m }
        };

        _cacheMock.Setup(c => c.GetAsync<IReadOnlyList<PartItemDto>>("pulsetrack:parts:search:BUMPER", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedList);

        var result = await _service.SearchPartsAsync("bumper");

        result.Should().HaveCount(1);
        result.First().PartNumber.Should().Be("TY-88412-BUMP");
        _repoMock.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchPartsAsync_CacheMiss_QueriesRepoAndStoresInCache()
    {
        var dbItems = new List<PartItem>
        {
            new("FO-10294-FNDR", "Front Right Steel Fender Panel", "Sheetmetal", 8, 385.00m, "Aisle 4", "Motorcraft")
        };

        _cacheMock.Setup(c => c.GetAsync<IReadOnlyList<PartItemDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<PartItemDto>?)null);

        _repoMock.Setup(r => r.SearchAsync("FENDER", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbItems);

        var result = await _service.SearchPartsAsync("fender");

        result.Should().HaveCount(1);
        result.First().PartNumber.Should().Be("FO-10294-FNDR");
        _cacheMock.Verify(c => c.SetAsync(
            "pulsetrack:parts:search:FENDER",
            It.IsAny<IReadOnlyList<PartItemDto>>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
