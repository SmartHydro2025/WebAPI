using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.LiveCache;
using System;
using System.ComponentModel;
using System.Text.Json;

namespace SmartHydro_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TentInformationController : Controller
    {
        private readonly LiveTentInformationCache _cache;
        private readonly MqttService _mqttService;
        private readonly ILogger<TentControlController> _logger;

        public TentInformationController(LiveTentInformationCache cache, MqttService mqttService, ILogger<TentControlController> logger)
        {
            _cache = cache;
            _mqttService = mqttService;
            _logger = logger;
        }

        [HttpPost("tent/add")]
        public async Task<IActionResult> AddTent(string mac, string location, string name)
        {
            //create tent object
            var tent = new TentInformation
            {
                Mac = mac,
                tentLocation = location,
                tentName = name
            };

            //hopefully pass the details to mqtt
            var payload = JsonSerializer.Serialize(tent);
            await _mqttService.PublishAsync("tentInformation", payload);

            _logger.LogInformation("Published tent for MAC {Mac}: Location '{location}' and Name '{name}'", mac, location, name);
            return Ok(new { message = $"Tent was created successfully.", tent });
        }

        [HttpGet("tent/{mac}")]
        public ActionResult<string> GetTentDetails(string mac)
        {
            var tentDetails = _cache.GetAll(); //pull all tents details


            if (tentDetails == null || tentDetails.Count == 0)
            {
                return NotFound("No tent data available.");
            }
            var macSorted = tentDetails.OrderBy(r => r.Mac).FirstOrDefault();

            //check which tent details match asked for tent in the list of objects pulled
            foreach (var tent in tentDetails)
            {
                if (macSorted?.Mac == mac) //if matching mac is found
                {
                    return (tent.ToString()); //test if the segments will be picked up by the android side
                }
            }
            return NotFound($"No tent with mac address [{mac}] found"); //if not found return that mac wasnt in the list
        }

        //possible option to update name or location of tent? **
    }
}
