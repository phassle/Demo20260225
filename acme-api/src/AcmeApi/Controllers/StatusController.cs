using AcmeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcmeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private static readonly DateTime StartTime = DateTime.UtcNow;
    private readonly ILogger<StatusController> _logger;
    private readonly IWebHostEnvironment _environment;

    public StatusController(ILogger<StatusController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Getting application status");

        var version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0";
        var uptime = (DateTime.UtcNow - StartTime).ToString(@"d\.hh\:mm\:ss");

        var dto = new StatusDto(
            version,
            uptime,
            _environment.EnvironmentName,
            ProductStore.Products.Count,
            OrderStore.Orders.Count
        );

        return Ok(dto);
    }
}
