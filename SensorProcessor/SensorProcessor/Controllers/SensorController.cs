using Microsoft.AspNetCore.Mvc;
using SensorProcessor.DTOs;
using SensorProcessor.Filters;
using SensorProcessor.Models;
using SensorProcessor.Services;

namespace SensorProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(ValidationFilter))]
public class SensorController : ControllerBase
{
    private readonly ISensorService _sensorService;
    private readonly ILogger<SensorController> _logger;

    public SensorController(ISensorService sensorService, ILogger<SensorController> logger)
    {
        _sensorService = sensorService;
        _logger = logger;
    }

    [HttpPost("data")]
    public async Task<ActionResult<SensorDataResponseDto>> AddSensorData([FromBody] SensorDataDto dataDto)
    {
        try
        {
            var result = await _sensorService.AddSensorDataAsync(dataDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving sensor data");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("data")]
    public async Task<ActionResult<IEnumerable<SensorDataResponseDto>>> GetSensorData(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        try
        {
            if (start >= end)
                return BadRequest(new { error = "Start date must be before end date" });

            if ((end - start).TotalDays > 30)
                return BadRequest(new { error = "Time range cannot exceed 30 days" });

            var data = await _sensorService.GetSensorDataAsync(start, end);
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sensor data");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("sensors/summary")]
    public async Task<ActionResult<IEnumerable<SensorSummary>>> GetSensorsSummary(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        try
        {
            if (start >= end)
                return BadRequest(new { error = "Start date must be before end date" });

            var summary = await _sensorService.GetSensorsSummaryAsync(start, end);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating sensors summary");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("upload-xml")]
    public async Task<ActionResult<XmlUploadResponseDto>> UploadXml(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            if (!file.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "File must be XML format" });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { error = "File size cannot exceed 5MB" });

            using var stream = file.OpenReadStream();
            var result = await _sensorService.ProcessXmlDataAsync(stream);

            return result.IsValid
                ? Ok(result)
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing XML file");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}