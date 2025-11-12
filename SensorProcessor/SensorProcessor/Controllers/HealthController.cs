using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorProcessor.Data;

namespace SensorProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    private readonly SensorDbContext _context;

    public HealthController(ILogger<HealthController> logger, SensorDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            var dataCount = await _context.SensorData.CountAsync();

            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                service = "Sensor Processor API",
                database = canConnect ? "Connected" : "Disconnected",
                dataCount = dataCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }
}