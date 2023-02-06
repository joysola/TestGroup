using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Mvc;

namespace TestLogWebapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private ILoggerManager _logger2;
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,ILoggerManager loggerManager)
        {
            _logger = logger;
            _logger2 = loggerManager;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger2.LogInfo("Here is info message from our values controller.");
            _logger2.LogDebug("Here is debug message from our values controller.");
            _logger2.LogWarn("Here is warn message from our values controller.");
            _logger2.LogError("Here is an error message from our values controller.");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}