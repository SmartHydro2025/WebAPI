using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Interface;
using SmartHydro_API.LiveCache;

namespace SmartHydro_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly ISensorReadingStore _store;
        private readonly LiveSensorCache _cache;

        public SensorReadingsController(ISensorReadingStore store, LiveSensorCache cache)
        {
            _store = store;
            _cache = cache;
        }

        // Gets all historical sensor readings from the database.
        [HttpGet]
        public ActionResult<List<SensorReading>> GetAll()
        {
            return Ok(_store.GetAll());
        }

        // Gets the latest live reading from all devices in the cache.
        [HttpGet("latest")]
        public ActionResult<List<SensorReading>> GetLatestLiveReadings()
        {
            var readings = _cache.GetAllLatest();
            if (readings.Count == 0)
            {
                return NotFound("No live sensor data in cache.");
            }
            return Ok(readings);
        }

        // Gets the latest live reading for a specific device.
        [HttpGet("latest/{mac}")]
        public ActionResult<SensorReading> GetByMac(string mac)
        {
            Console.WriteLine($" Request for MAC: {mac}");

            var reading = _cache.GetLatest(mac);
            if (reading == null)
            {
                return NotFound($"No live data for MAC: {mac}");
            }
            return Ok(reading);
        }




        // Gets a list of all unique MAC addresses currently in the live cache
        [HttpGet("mac address")]
        public ActionResult<List<string>> GetMacAddresses()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No live data available to extract MAC addresses.");
            }

            var macAddresses = readings
                .Where(r => !string.IsNullOrEmpty(r.Mac))
                .Select(r => r.Mac!)
                .Distinct()
                .ToList();

            return Ok(macAddresses);
        }



        // Gets just the latest pH value from the cache.
        [HttpGet("latest/ph")]
        public ActionResult<double?> GetLatestPhLevel()
        {
            var readings = _cache.GetAllLatest();
            if (readings == null || readings.Count == 0)
            {
                return NotFound("No live sensor data available.");
            }

            // Get the most recent reading based on timestamp
            var latestReading = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            if (latestReading == null || latestReading.PhLevel == null)
            {
                return NotFound("No valid pH level data available.");
            }

            return Ok(latestReading.PhLevel);
        }


        // Gets just the latest ec value from the cache.
        [HttpGet("latest/ec")]
        public ActionResult<double?> GetLatestECLevel()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No live sensor data available.");
            }


            // Get the most recent reading based on timestamp
            var latestReading = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            if (latestReading == null || latestReading.EcLevel == null)
            {
                return NotFound("No valid EC level data available.");
            }

            return Ok(latestReading.PhLevel);
        }

        // Gets just the latest temp value from the cache.
        [HttpGet("latest/temp")]
        public ActionResult<double?> GetLatestTempValue()
        {
            var allReadings = _cache.GetAllLatest();

            if (allReadings == null || allReadings.Count == 0)
            {
                return NotFound("No live sensor data available.");
            }


            //pull the most recent reading based on timestamp
            var lastReading = allReadings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            if (lastReading == null || lastReading.Temperature == null)
            {
                return NotFound("No temperature data available.");
            }

            return Ok(lastReading.Temperature);
        }

        // Gets just the latest humidity value from the cache. 
        [HttpGet("latest/humidity")]
        public ActionResult<double?> GetLatestHumidityIndex()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No live sensor data available.");
            }


            //pull the most recent reading based on timestamp
            var latestReading = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            if (latestReading == null || latestReading.Humidity == null)
            {
                return NotFound("No humidity data available.");
            }

            return Ok(latestReading.Humidity);
        }

        // Gets just the latest light value from the cache. 
        [HttpGet("latest/light")]
        public ActionResult<double?> GetLatestLightLevel()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No live sensor data available.");
            }


            //get the most recent reading based on timestamp
            var latestReading = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            if (latestReading == null || latestReading.EcLevel == null)
            {
                return NotFound("No valid light level data available.");
            }

            return Ok(latestReading.LightLevel);
        }



        //Testing Purpose
        [HttpPost]
        public IActionResult Post([FromBody] SensorReading reading)
        {
            if (reading == null)
            {
                return BadRequest("Sensor reading data is null.");
            }

            // Set the timestamp to the current time when received.
            reading.Timestamp = DateTime.UtcNow;

            // 1. Update the live cache for immediate access.
            _cache.Update(reading);

            // 2. Save the reading to the database for historical record.
            _store.Update(reading);

            // Return a success response.
            return Ok(new { message = "Sensor reading received and stored successfully." });
        }

    }
}
