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

        // 1️⃣ Get all historical sensor readings from DB
        [HttpGet]
        public ActionResult<List<SensorReading>> GetAll()
        {
            return Ok(_store.GetAll());
        }

        // 2️⃣ Get the latest live reading from in-memory cache (all devices)
        [HttpGet("latest")]
        public ActionResult<List<SensorReading>> GetLatestLiveReadings()
        {
            var readings = _cache.GetAllLatest();
            if (readings.Count == 0)
                return NotFound("No live sensor data in cache.");
            return Ok(readings);
        }

        // 3️⃣ Optional: Get latest by MAC
        [HttpGet("latest/{mac}")]
        public ActionResult<SensorReading> GetByMac(string mac)
        {
            Console.WriteLine($"🔍 Request for MAC: {mac}");

            var reading = _cache.GetLatest(mac);
            if (reading == null)
                return NotFound($"No live data for MAC: {mac}");
            return Ok(reading);
        }




        // 4️⃣ Get all unique MAC addresses from live cache
        [HttpGet("mac address")]
        public ActionResult<List<string>> GetMacAddresses()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
                return NotFound("No live data available to extract MAC addresses.");

            var macAddresses = readings
                .Where(r => !string.IsNullOrEmpty(r.Mac))
                .Select(r => r.Mac!)
                .Distinct()
                .ToList();

            return Ok(macAddresses);
        }



        // 5️⃣ Get only the latest pH level from the live cache
        [HttpGet("latest/ph")]
        public ActionResult<double?> GetLatestPhLevel()
        {
            var readings = _cache.GetAllLatest();
            if (readings == null || readings.Count == 0)
                return NotFound("No live sensor data available.");

            // Get the most recent reading based on timestamp
            var latestReading = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            if (latestReading == null || latestReading.PhLevel == null)
                return NotFound("No valid pH level data available.");

            return Ok(latestReading.PhLevel);
        }

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

        //get the last recorded temperature value 
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

        //get the last recorded humidity index 
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
    }
}
