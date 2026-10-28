using Microsoft.AspNetCore.Mvc;
using PulseTrack.Application.DTOs;
using PulseTrack.Application.Services;

namespace PulseTrack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MetricsController : ControllerBase
{
    private readonly WorkOrderService _service;

    public MetricsController(WorkOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ShopMetricsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMetrics(CancellationToken cancellationToken)
    {
        var metrics = await _service.GetMetricsAsync(cancellationToken);
        return Ok(metrics);
    }
}
