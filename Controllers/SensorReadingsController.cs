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
            var reading = _cache.GetLatest(mac);
            if (reading == null)
                return NotFound($"No live data for MAC: {mac}");
            return Ok(reading);
        }
    }
}
