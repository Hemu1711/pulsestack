using Microsoft.AspNetCore.Mvc;
using PulseTrack.Application.DTOs;
using PulseTrack.Application.Services;

namespace PulseTrack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkOrdersController : ControllerBase
{
    private readonly WorkOrderService _service;

    public WorkOrdersController(WorkOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _service.GetAllAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _service.GetByIdAsync(id, cancellationToken);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}/stage")]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransitionStage(Guid id, [FromBody] TransitionStageRequest request, CancellationToken cancellationToken)
    {
        var updated = await _service.TransitionStageAsync(id, request.NextStage, request.Notes, cancellationToken);
        return Ok(updated);
    }
}
