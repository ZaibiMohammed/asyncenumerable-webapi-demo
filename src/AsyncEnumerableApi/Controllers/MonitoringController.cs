using AsyncEnumerableApi.Infrastructure.Monitoring;
using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerableApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonitoringController : ControllerBase
{
    private readonly StreamingMetricsCollector _metricsCollector;

    public MonitoringController(StreamingMetricsCollector metricsCollector)
    {
        _metricsCollector = metricsCollector;
    }

    [HttpGet("metrics")]
    public ActionResult<IEnumerable<StreamingMetrics>> GetMetrics()
    {
        return Ok(_metricsCollector.GetMetrics());
    }
}