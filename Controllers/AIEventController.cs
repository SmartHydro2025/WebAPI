using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Database;
using SmartHydro_API.LiveCache;
using System;

namespace SmartHydro_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIEventController : ControllerBase
    {
        private readonly AIEventCache _cache;
        private readonly SmartHydroDbContext _dbContext;

        public AIEventController(AIEventCache cache, SmartHydroDbContext dbContext)
        {
            _cache = cache;
        }

        [HttpGet("{mac}/latestAI/temperature")]
        public ActionResult<string> GetLatestTemperatureEvent(string mac)
        {
            try
            {
                var aiEvent = _cache.GetLatest(mac);

                if (aiEvent == null)
                {
                    return NotFound($"No AI event data is available for MAC: {mac}");
                }

                if (aiEvent.Sensor.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(aiEvent.Message);
                }

                //(D, 2025; Vedpathak, 2024)
                var dbEvent = _dbContext.AiEvents
                                    .Where(e => e.Mac == mac && e.Sensor.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
                                    .OrderByDescending(e => e.ID) // Assumes higher ID = newer
                                    .FirstOrDefault();

                if (dbEvent == null)
                {
                    return NotFound($"No Temperature event data is available for MAC: {mac}");
                }

                return Ok(dbEvent.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest Temperature event for {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        [HttpGet("{mac}/latestAI/humidity")]
        public ActionResult<string> GetLatestHumidityEvent(string mac)
        {
            try
            {
                var aiEvent = _cache.GetLatest(mac);

                if (aiEvent == null)
                {
                    return NotFound($"No AI event data is available for MAC: {mac}");
                }

                if (aiEvent.Sensor.Equals("Humidity", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(aiEvent.Message);
                }

                //(D, 2025; Vedpathak, 2024)
                var dbEvent = _dbContext.AiEvents
                                    .Where(e => e.Mac == mac && e.Sensor.Equals("Humidity", StringComparison.OrdinalIgnoreCase))
                                    .OrderByDescending(e => e.ID)
                                    .FirstOrDefault();

                if (dbEvent == null)
                {
                    return NotFound($"No Humidity event data is available for MAC: {mac}");
                }

                return Ok(dbEvent.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest Humidity event for {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        [HttpGet("{mac}/latestAI/ph")]
        public ActionResult<string> GetLatestPHEvent(string mac)
        {
            try
            {
                var aiEvent = _cache.GetLatest(mac);

                if (aiEvent == null)
                {
                    return NotFound($"No AI event data is available for MAC: {mac}");
                }

                if (aiEvent.Sensor.Equals("pH", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(aiEvent.Message);
                }

                //(D, 2025; Vedpathak, 2024)
                var dbEvent = _dbContext.AiEvents
                                    .Where(e => e.Mac == mac && e.Sensor.Equals("pH", StringComparison.OrdinalIgnoreCase))
                                    .OrderByDescending(e => e.ID)
                                    .FirstOrDefault();

                if (dbEvent == null)
                {
                    return NotFound($"No pH event data is available for MAC: {mac}");
                }

                return Ok(dbEvent.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest pH event for {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        [HttpGet("{mac}/latestAI/ec")]
        public ActionResult<string> GetLatestECEvent(string mac)
        {
            try
            {
                var aiEvent = _cache.GetLatest(mac);

                if (aiEvent == null)
                {
                    return NotFound($"No AI event data is available for MAC: {mac}");
                }

                if (aiEvent.Sensor.Equals("EC", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(aiEvent.Message);
                }

                //(D, 2025; Vedpathak, 2024)
                var dbEvent = _dbContext.AiEvents
                                    .Where(e => e.Mac == mac && e.Sensor.Equals("EC", StringComparison.OrdinalIgnoreCase))
                                    .OrderByDescending(e => e.ID)
                                    .FirstOrDefault();

                if (dbEvent == null)
                {
                    return NotFound($"No EC event data is available for MAC: {mac}");
                }

                return Ok(dbEvent.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest EC event for {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred.");
            }
        }
    }
}


/*
REFERENCES
====================
D, S. 2025. LINQ in C# Tutorial for Beginners: 101 C# LINQ Operations. [Online]. Available at:    https://www.c-sharpcorner.com/article/linq-in-c-sharp-tutorial-for-beginners-101-c-sharp-linq-operations/
Vedpathak, Y. 2024. Querying with LINQ. [Online]. Available at:  https://www.c-sharpcorner.com/blogs/querying-with-linq 
 */
