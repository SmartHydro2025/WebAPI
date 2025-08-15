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

        [HttpGet("status/growlight")]
        public ActionResult<string> GetGrowLightStatus()
        {
            var readings = _cache.GetAllLatest();


            if (readings == null || readings.Count == 0)
            {
                return NotFound("No hardware status data available.");
            }
            var latestStatus = readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

            // check to see what the status is

            if (latestStatus?.GrowLightStatus == false)
            {
                return ("The grow light is on");
            }
            else if (latestStatus?.GrowLightStatus == true)
            {
                return ("The grow light is off");
            }
            return NotFound("No grow light status found");
        }

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
            if (latestStatus?.NutrientPumpStatus == false)
            {
                return ("The nutrient pump is on");
            }
            else if (latestStatus?.NutrientPumpStatus ==  true)
            {
                return ("The nutrient pump is off");
            }
            return NotFound("No nutrient pump status found");

        }


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
            if (latestStatus?.WaterPumpStatus == false)
            {
                return ("The water pump is on");
            }
            else if (latestStatus?.WaterPumpStatus == true)
            {
                return ("The water pump is off");
            }
            return NotFound("No water pump status found");
        }


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
            if (latestStatus?.CirculationPumpStatus == false)
            {
                return ("The circulation pump is on");
            }
            else if (latestStatus?.CirculationPumpStatus == true)
            {
                return ("The circulation pump is off");
            }
            return NotFound("No circulation pump status found");
        }

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
            if (latestStatus?.CirculationFanStatus == false)
            {
                return ("The circulation fan is on");
            }
            else if (latestStatus?.CirculationFanStatus == true)
            {
                return ("The circulation fan is off");
            }
            return NotFound("No circulation fan status found");
        }

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
            if (latestStatus?.ExtractorFanStatus == false)
            {
                return ("The extractor fan is on");
            }
            else if (latestStatus?.ExtractorFanStatus == true)
            {
                return ("The extractor fan is off");
            }
            return NotFound("No extractor fan status found");
        }


    }
}
