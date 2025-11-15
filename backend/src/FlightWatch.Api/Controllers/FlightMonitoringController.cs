<<<<<<< HEAD
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.FlightMonitoring;
using FlightWatch.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightWatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightMonitoringController(
    IFlightMonitoringService monitoringService,
    ILogger<FlightMonitoringController> logger) : ControllerBase
{
    private readonly IFlightMonitoringService _monitoringService = monitoringService;
    private readonly ILogger<FlightMonitoringController> _logger = logger;

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            service = "FlightMonitoring",
            status = "healthy",
            timestamp = DateTime.UtcNow
        });
    }
}

=======
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.FlightMonitoring;
using FlightWatch.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightWatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightMonitoringController(
    IFlightMonitoringService monitoringService,
    ILogger<FlightMonitoringController> logger) : ControllerBase
{
    private readonly IFlightMonitoringService _monitoringService = monitoringService;
    private readonly ILogger<FlightMonitoringController> _logger = logger;

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            service = "FlightMonitoring",
            status = "healthy",
            timestamp = DateTime.UtcNow
        });
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
