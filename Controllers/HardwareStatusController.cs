using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Interface;
using SmartHydro_API.LiveCache;
using System.Security.Cryptography;

namespace SmartHydro_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HardwareStatusController : ControllerBase
    {
        private readonly LiveHardwareStatusCache _cache;

        public HardwareStatusController(LiveHardwareStatusCache cache)
        {
            _cache = cache;
        }

        // Returns the current status of the grow light.
        [HttpGet("status/{mac}/growlight")]
        
        public ActionResult<string> GetGrowLightStatus(string mac)
        {
            var readings = _cache.GetLatest(mac);

            if (readings == null )
            {
                return NotFound("No hardware status data available.");
            }


            if (readings.GrowLightStatus == "0")
            {
                return Ok("The grow light is on");
            }
            else if (readings.GrowLightStatus == "1")
            {
                return Ok("The grow light is off");
            }
            return NotFound("No grow light status found");
        }


        // Returns the current status of the nutrient pump
        [HttpGet("status/{mac}/nutrientpump")]
        public ActionResult<string> GetNutrientPumpStatus(string mac)
        {
            var readings = _cache.GetLatest(mac);

            if (readings == null)
            {
                return NotFound("No hardware status data available.");
            }

            // check to see what the status is
            if (readings.NutrientPumpStatus == "0")
            {
                return Ok("The nutrient pump is on");
            }
            else if (readings.NutrientPumpStatus == "1")
            {
                return Ok("The nutrient pump is off");
            }
            return NotFound("No nutrient pump status found");

        }

        // Returns the current status of the water pump
        [HttpGet("status/{mac}/waterpump")]
        public ActionResult<string> GetWaterPumpStatus(string mac)
        {
            var readings = _cache.GetLatest(mac);

            if (readings == null)
            {
                return NotFound("No hardware status data available.");
            }

            // check to see what the status is
            if (readings.WaterPumpStatus == "0")
            {
                return Ok("The water pump is on");
            }
            else if (readings.WaterPumpStatus == "1")
            {
                return Ok("The water pump is off");
            }
            return NotFound("No water pump status found");
        }

        // Returns the current status of the circilation pump
        [HttpGet("status/{mac}/circulationpump")]
        public ActionResult<string> GetCirculationPumpStatus(string mac)
        {
            var readings = _cache.GetLatest(mac);

            if (readings == null)
            {
                return NotFound("No hardware status data available.");
            }

            // check to see what the status is
            if (readings.CirculationPumpStatus == "0")
            {
                return Ok("The circulation pump is on");
            }
            else if (readings.CirculationPumpStatus == "1")
            {
                return Ok("The circulation pump is off");
            }
            return NotFound("No circulation pump status found");
        }

        // Returns the current status of the fan
        [HttpGet("status/{mac}/circulationfan")]
        public ActionResult<string> GetCirculationFanStatus(string mac)
        {
            var readings = _cache.GetLatest(mac);

            if (readings == null)
            {
                return NotFound("No hardware status data available.");
            }

            // check to see what the status is
            if (readings.CirculationFanStatus == "0")
            {
                return Ok("The circulation fan is on");
            }
            else if (readings.CirculationFanStatus == "1")
            {
                return Ok("The circulation fan is off");
            }
            return NotFound("No circulation fan status found");
        }

        // Returns the current status of the extractor fan
        [HttpGet("status/{mac}/extractorfan")]
        public ActionResult<string> GetExtractorFanStatus(string mac)
        {
            var readings = _cache.GetLatest(mac);


            if (readings == null)
            {
                return NotFound("No hardware status data available.");
            }
               // check to see what the status is
            if (readings.ExtractorFanStatus == "0")
            {
                return Ok("The extractor fan is on");
            }
            else if (readings.ExtractorFanStatus == "1")
            {
                return Ok("The extractor fan is off");
            }
            return NotFound("No extractor fan status found");
        }


    }
}
