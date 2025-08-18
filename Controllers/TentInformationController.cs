using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Database;
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
        private readonly SmartHydroDbContext _dbContext;

        public TentInformationController(LiveTentInformationCache cache, MqttService mqttService, ILogger<TentControlController> logger, SmartHydroDbContext dbContext            )
        {
            _cache = cache;
            _mqttService = mqttService;
            _logger = logger;
            _dbContext = dbContext;
        }

        //adds a tent with a mac address, location of the tent and tent name
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
            _cache.Update(tent);
            var payload = JsonSerializer.Serialize(tent);
            await _mqttService.HandleTentInformationAsync(tent);

            _logger.LogInformation("Published tent for MAC {Mac}: Location '{location}' and Name '{name}'", mac, location, name);
            return Ok(new { message = $"Tent was created successfully.", tent });
        }

        //pulls a single tent details by mac address
        [HttpGet("tent/{mac}")]
        public ActionResult<TentInformation> GetTentDetails(string mac)
        {
            var tentDetails = _dbContext.TentInformation.FirstOrDefault(r => r.Mac == mac);

            if (tentDetails == null)
            {
                return NotFound("No tent data available.");
            }

            return Ok(tentDetails);
        }

        //returns a list of all tents logged in db
        [HttpGet("alltents")]
        public ActionResult<List<TentInformation>> GetAllTents()
        {
            var tentDetails = _dbContext.TentInformation.ToList();

            if (tentDetails == null)
            {
                return NotFound("No tent data available.");
            }

            return Ok(tentDetails);
        }
    }
}
