using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Interface;
using SmartHydro_API.LiveCache;
using System.Security.Cryptography;

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

        // Gets the latest live reading from all devices
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
        [HttpGet("allMacAddress")]
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



        // Gets just the latest pH value 
        [HttpGet("latest/{mac}/ph")]
        public ActionResult<double?> GetLatestPhLevel(string mac)
        {

            var latestReading = _cache.GetLatest(mac);

            if (latestReading == null)
            {
                return NotFound($"No live data for MAC: {mac}");
            }

            if (latestReading.PhLevel == null)
            {
                return NotFound($"No valid temperature data available for MAC: {mac}");
            }



            return Ok(latestReading.PhLevel);
        }


        // Gets just the latest ec value 
        [HttpGet("latest/{mac}/ec")]
        public ActionResult<double?> GetLatestECLevel(string mac)
        {
            var latestReading = _cache.GetLatest(mac);

            if (latestReading == null)
            {
                return NotFound($"No live data for MAC: {mac}");
            }

            if (latestReading.EcLevel == null)
            {
                return NotFound($"No valid temperature data available for MAC: {mac}");
            }


            return Ok(latestReading.EcLevel);
        }

        // Gets just the latest temp value 
        [HttpGet("latest/{mac}/temp")]
        public ActionResult<double?> GetLatestTempValue(string mac)
        {
            var latestReading = _cache.GetLatest(mac);

            if (latestReading == null)
            {
                return NotFound($"No live data for MAC: {mac}");
            }

            if (latestReading.PhLevel == null)
            {
                return NotFound($"No valid temperature data available for MAC: {mac}");
            }


            return Ok(latestReading.Temperature);
        }

        // Gets just the latest humidity 
        [HttpGet("latest/{mac}/humidity")]
        public ActionResult<double?> GetLatestHumidityIndex(string mac)
        {
            var latestReading = _cache.GetLatest(mac);

            if (latestReading == null)
            {
                return NotFound($"No live data for MAC: {mac}");
            }

            if (latestReading.PhLevel == null)
            {
                return NotFound($"No valid temperature data available for MAC: {mac}");
            }

            return Ok(latestReading.Humidity);
        }

        // Gets just the latest light value from the cache. 
        [HttpGet("latest/{mac}/light")]
        public ActionResult<double?> GetLatestLightLevel(string mac)
        {
            var latestReading = _cache.GetLatest(mac);

            if (latestReading == null)
            {
                return NotFound($"No live data for MAC: {mac}");
            }

            if (latestReading.PhLevel == null)
            {
                return NotFound($"No valid temperature data available for MAC: {mac}");
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
