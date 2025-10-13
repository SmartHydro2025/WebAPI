using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.LiveCache;

namespace SmartHydro_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIEventController : ControllerBase
    {
        private readonly AIEventCache _cache;

        public AIEventController(AIEventCache cache)
        {
            _cache = cache;
        }


        /// Gets the latest AI event message related to temperature.
        [HttpGet("latest/temperature")]
        public ActionResult<string> GetLatestTemperatureEvent()
        {
            var events = _cache.GetAllLatest();

            if (events == null || events.Count == 0)
            {
                return NotFound("No AI event data is available.");
            }

            // Find the first event that matches the "Temperature" sensor.
            foreach (var ai in events)
            {
                if (ai.Sensor.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(ai.Message);
                }
            }
            return NotFound("No temperature events were found.");
        }

        // Gets the latest AI event message related to humidity.
        [HttpGet("latest/humidity")]
        public ActionResult<string> GetLatestHumidityEvent()
        {
            var events = _cache.GetAllLatest();

            if (events == null || events.Count == 0)
            {
                return NotFound("No AI event data is available.");
            }

            foreach (var ai in events)
            {
                if (ai.Sensor.Equals("Humidity", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(ai.Message);
                }
            }
            return NotFound("No temperature events were found.");
        }

        // Gets the latest AI event message related to pH.
        [HttpGet("latest/ph")]
        public ActionResult<string> GetLatestPHEvent()
        {
            var events = _cache.GetAllLatest();

            if (events == null || events.Count == 0)
            {
                return NotFound("No AI event data is available.");
            }

            foreach (var ai in events)
            {
                if (ai.Sensor.Equals("pH", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(ai.Message);
                }
            }
            return NotFound("No pH events were found.");
        }

        // Gets the latest AI event message related to EC.
        [HttpGet("latest/ec")]
        public ActionResult<string> GetLatestECEvent()
        {
            var events = _cache.GetAllLatest();

            if (events == null || events.Count == 0)
            {
                return NotFound("No AI event data is available.");
            }

            foreach (var ai in events)
            {
                if (ai.Sensor.Equals("EC", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(ai.Message);
                }
            }
            return NotFound("No EC events were found.");
        }
    }
}
