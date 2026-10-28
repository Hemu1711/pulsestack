using FluentAssertions;
using Moq;
using PulseTrack.Application.DTOs;
using PulseTrack.Application.Interfaces;
using PulseTrack.Application.Services;
using PulseTrack.Domain.Entities;
using PulseTrack.Domain.Enums;
using Xunit;

namespace PulseTrack.UnitTests;

public class WorkOrderServiceTests
{
    private readonly Mock<IWorkOrderRepository> _repoMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly WorkOrderService _service;

    public WorkOrderServiceTests()
    {
        _service = new WorkOrderService(_repoMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsAndInvalidatesCache()
    {
        var request = new CreateWorkOrderRequest
        {
            Vin = "1HGCR2F83HA019284",
            Year = 2021,
            Make = "Honda",
            Model = "Accord Touring",
            CustomerName = "David Kim",
            CustomerPhone = "515-555-0912",
            Priority = PriorityLevel.Expedited,
            TotalEstimate = 3250.00m,
            AssignedTechnician = "Elena Rostova",
            RepairNotes = "Rear quarter panel dent repair and blended clearcoat."
        };

        _repoMock.Setup(r => r.AddAsync(It.IsAny<WorkOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkOrder w, CancellationToken _) => w);

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        result.Vin.Should().Be("1HGCR2F83HA019284");
        result.CurrentStage.Should().Be(RepairStage.Intake);
        result.Priority.Should().Be(PriorityLevel.Expedited);

        _cacheMock.Verify(c => c.RemoveAsync("pulsetrack:workorders:all", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TransitionStageAsync_ExistingOrder_UpdatesStageAndInvalidatesCache()
    {
        var order = WorkOrder.Create("1HGCR2F83HA019284", 2021, "Honda", "Accord", "David Kim", "515-555-0912", PriorityLevel.Standard, 1500m, "Elena", "Initial notes");
        _repoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var result = await _service.TransitionStageAsync(order.Id, RepairStage.BodyRepair, "Frame straightened and laser aligned.");

        result.CurrentStage.Should().Be(RepairStage.BodyRepair);
        result.RepairNotes.Should().Contain("Frame straightened");
        _repoMock.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync("pulsetrack:workorders:all", It.IsAny<CancellationToken>()), Times.Once);
    }
}
