using Microsoft.AspNetCore.Mvc;
using PulseTrack.Application.DTOs;
using PulseTrack.Application.Services;

namespace PulseTrack.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PartsController : ControllerBase
{
    private readonly PartsCatalogService _service;

    public PartsController(PartsCatalogService service)
    {
        _service = service;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<PartItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? q, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = await _service.SearchPartsAsync(q ?? string.Empty, cancellationToken);
        stopwatch.Stop();

        Response.Headers.Append("X-Lookup-Latency-Ms", stopwatch.ElapsedMilliseconds.ToString());
        return Ok(results);
    }

    [HttpGet("{partNumber}")]
    [ProducesResponseType(typeof(PartItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPartNumber(string partNumber, CancellationToken cancellationToken)
    {
        var part = await _service.GetByPartNumberAsync(partNumber, cancellationToken);
        return part == null ? NotFound() : Ok(part);
    }
}
