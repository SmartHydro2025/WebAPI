using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Interface;
using SmartHydro_API.LiveCache;
using System.Security.Cryptography;
using SmartHydro_API.Models;

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
        [HttpGet("allDBReadings")]
        public ActionResult<List<SensorReading>> GetAll()
        {
            return Ok(_store.GetAll());
        }

        // Gets the latest live reading from all devices
        [HttpGet("allLatestReadings")]
        public ActionResult<List<SensorReading>> GetLatestLiveReadings()
        {
            var readings = _cache.GetAllLatest();
            if (readings.Count == 0)
            {
                return Ok("No live sensor data in cache.");
            }
            return Ok(readings);
        }

        // Gets the latest live reading for a specific device.
        [HttpGet("latest/{mac}")]
        public ActionResult<SensorReading> GetByMac(string mac)
        {
            Console.WriteLine($" Request for MAC: {mac}");

            try
            {
                var reading = _cache.GetLatest(mac);

                if (reading == null)
                {
                    reading = _store.GetByMac(mac);
                  
                }

                if (reading == null)
                {
                    return Ok($"No data found for MAC: {mac}");

                }


                return Ok(reading);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest for MAC {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred while retrieving data.");
            }
        }


        // Gets just the latest pH value 
        [HttpGet("latest/{mac}/ph")]
        public ActionResult<double?> GetLatestPhLevel(string mac)
        {
            try
            {

                var latestReading = _cache.GetLatest(mac);

                //checks database
                if (latestReading == null)
                {
                    latestReading = _store.GetByMac(mac);
                    

                }

                //if cache and databse is null
                if (latestReading == null)
                {

                    return Ok($"No data found for MAC: {mac}");

                }

                if (latestReading.PhLevel == null)
                {
                    return Ok(0.0);
                }

                return Ok(latestReading.PhLevel);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest for MAC {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred while retrieving data.");
            }
        }


        // Gets just the latest ec value 
        [HttpGet("latest/{mac}/ec")]
        public ActionResult<double?> GetLatestECLevel(string mac)
        {
            try
            {
                var latestReading = _cache.GetLatest(mac);

                //checks database
                if (latestReading == null)
                {
                    latestReading = _store.GetByMac(mac);


                }

                //if cache and databse is null
                if (latestReading == null)
                {

                    return Ok($"No data found for MAC: {mac}");

                }

                if (latestReading.EcLevel == null)
                {
                    return Ok(0.0);
                }


                return Ok(latestReading.EcLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest for MAC {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred while retrieving data.");
            }
        }

        // Gets just the latest temp value 
        [HttpGet("latest/{mac}/temp")]
        public ActionResult<double?> GetLatestTempValue(string mac)
        {
            try
            {
                var latestReading = _cache.GetLatest(mac);

                //checks database
                if (latestReading == null)
                {
                    latestReading = _store.GetByMac(mac);


                }

                //if cache and databse is null
                if (latestReading == null)
                {

                    return Ok($"No data found for MAC: {mac}");

                }

                if (latestReading.Temperature == null)
                {
                    return Ok(0);
                }


                return Ok(latestReading.Temperature);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest for MAC {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred while retrieving data.");
            }

        }

        // Gets just the latest humidity 
        [HttpGet("latest/{mac}/humidity")]
        public ActionResult<double?> GetLatestHumidityIndex(string mac)
        {
            try
            {
                var latestReading = _cache.GetLatest(mac);

                //checks database
                if (latestReading == null)
                {
                    latestReading = _store.GetByMac(mac);


                }

                //if cache and databse is null
                if (latestReading == null)
                {

                    return Ok($"No data found for MAC: {mac}");

                }

                if (latestReading.Humidity == null)
                {
                    return Ok(0.0);
                }

                return Ok(latestReading.Humidity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest for MAC {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred while retrieving data.");
            }
        }

        // Gets just the latest light value from the cache. 
        [HttpGet("latest/{mac}/light")]
        public ActionResult<double?> GetLatestLightLevel(string mac)
        {
            try
            {
                var latestReading = _cache.GetLatest(mac);

                //checks database
                if (latestReading == null)
                {
                    latestReading = _store.GetByMac(mac);


                }

                //if cache and databse is null
                if (latestReading == null)
                {

                    return Ok($"No data found for MAC: {mac}");

                }

                if (latestReading.LightLevel == null)
                {
                    return Ok(0.0);
                }

                return Ok(latestReading.LightLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest for MAC {mac}: {ex.Message}");
                return StatusCode(500, "An internal error occurred while retrieving data.");
            }
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
