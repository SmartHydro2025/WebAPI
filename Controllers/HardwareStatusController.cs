using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Interface;
using SmartHydro_API.LiveCache;

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
        [HttpGet("status/growlight")]
        [HttpGet("status/growlight")]
        public ActionResult<string> GetGrowLightStatus()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No hardware status data available.");
            }

            // Get the most recent status update from any device.
            var latestStatus = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            // check to see what the status is

            if ((bool)(latestStatus?.GrowLightStatus.Equals("0")))
            {
                return ("The grow light is on");
            }
            else if ((bool)(latestStatus?.GrowLightStatus.Equals("1")))
            {
                return ("The grow light is off");
            }
            return NotFound("No grow light status found");
        }


        // Returns the current status of the nutrient pump
        [HttpGet("status/nutrientpump")]
        public ActionResult<string> GetNutrientPumpStatus()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No hardware status data available.");
            }
            var latestStatus = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            // check to see what the status is
            if ((bool)(latestStatus?.NutrientPumpStatus.Equals("0")))
            {
                return ("The nutrient pump is on");
            }
            else if ((bool)(latestStatus?.NutrientPumpStatus.Equals("1")))
            {
                return ("The nutrient pump is off");
            }
            return NotFound("No nutrient pump status found");

        }

        // Returns the current status of the water pump
        [HttpGet("status/waterpump")]
        public ActionResult<string> GetWaterPumpStatus()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No hardware status data available.");
            }
            var latestStatus = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            // check to see what the status is
            if ((bool)(latestStatus?.WaterPumpStatus.Equals("0")))
            {
                return ("The water pump is on");
            }
            else if ((bool)(latestStatus?.WaterPumpStatus.Equals("1")))
            {
                return ("The water pump is off");
            }
            return NotFound("No water pump status found");
        }

        // Returns the current status of the circilation pump
        [HttpGet("status/circulationpump")]
        public ActionResult<string> GetCirculationPumpStatus()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No hardware status data available.");
            }
            var latestStatus = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            // check to see what the status is
            if ((bool)(latestStatus?.CirculationPumpStatus.Equals("0")))
            {
                return ("The circulation pump is on");
            }
            else if ((bool)(latestStatus?.CirculationPumpStatus.Equals("1")))
            {
                return ("The circulation pump is off");
            }
            return NotFound("No circulation pump status found");
        }

        // Returns the current status of the fan
        [HttpGet("status/circulationfan")]
        public ActionResult<string> GetCirculationFanStatus()
        {
            var readings = _cache.GetAllLatest();

            if (readings == null || readings.Count == 0)
            {
                return NotFound("No hardware status data available.");
            }
            var latestStatus = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            // check to see what the status is
            if ((bool)(latestStatus?.CirculationFanStatus.Equals("0")))
            {
                return ("The circulation fan is on");
            }
            else if ((bool)(latestStatus?.CirculationFanStatus.Equals("1")))
            {
                return ("The circulation fan is off");
            }
            return NotFound("No circulation fan status found");
        }

        // Returns the current status of the extractor fan
        [HttpGet("status/extractorfan")]
        public ActionResult<string> GetExtractorFanStatus()
        {
            var readings = _cache.GetAllLatest();


            if (readings == null || readings.Count == 0)
            {
                return NotFound("No hardware status data available.");
            }
            var latestStatus = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            // check to see what the status is
            if ((bool)(latestStatus?.ExtractorFanStatus.Equals("0")))
            {
                return ("The extractor fan is on");
            }
            else if ((bool)(latestStatus?.ExtractorFanStatus.Equals("1")))
            {
                return ("The extractor fan is off");
            }
            return NotFound("No extractor fan status found");
        }


    }
}
