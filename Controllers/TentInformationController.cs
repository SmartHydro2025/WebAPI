using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Database;
using SmartHydro_API.LiveCache;
using System;
using System.ComponentModel;
using System.Text.Json;
using SmartHydro_API.Models;

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

        // This controller basically saves the only to the database as it it saves the information given when adding the tent
        public TentInformationController(LiveTentInformationCache cache, MqttService mqttService, ILogger<TentControlController> logger, SmartHydroDbContext dbContext            )
        {
            _cache = cache;
            _mqttService = mqttService;
            _logger = logger;
            _dbContext = dbContext;
        }

        //adds a tent with a mac address, location of the tent and tent name
        [HttpPost("tent/add")]
        public async Task<IActionResult> AddTent(
            [FromQuery] string mac,
            [FromQuery] string location,
            [FromQuery] string name,
            [FromQuery] string networkName
            )
        {
            try
            {
                var tent = new TentInformation
                {
                    Mac = mac,
                    tentName = name,
                    tentLocation = location,
                    networkName = networkName
                };

                _dbContext.TentInformation.Add(tent);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTentDetails), new { mac = tent.Mac }, tent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new tent for MAC {Mac}", mac);
                return StatusCode(500, "An internal error occurred while adding the tent.");
            }
        }

        // Deletes a tent
        [HttpDelete("tent/delete")]
        public async Task<IActionResult> DeleteTent(string mac)
        {

            var tent = _dbContext.TentInformation.FirstOrDefault(r => r.Mac == mac);
            if (tent == null)
            {
                return NotFound();
            }
            _dbContext.TentInformation.Remove(tent);
            await _dbContext.SaveChangesAsync();

            return Ok("Tent has been successfully deleted.");
        }

        //Gets a single tent  by mac address
        [HttpGet("tent/{mac}")]
        public ActionResult<TentInformation> GetTentDetails(string mac)
        {
            try
            {
                var tentDetails = _dbContext.TentInformation.FirstOrDefault(r => r.Mac == mac);

                if (tentDetails == null)
                {
                    return NotFound($"No tent data available for MAC: {mac}");
                }

                return Ok(tentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving details for tent with MAC {Mac}", mac);
                return StatusCode(500, "An internal error occurred while retrieving tent details.");
            }
        }

        //returns a list of all tents logged in db
        [HttpGet("alltents")]
        public ActionResult<List<TentInformation>> GetAllTents()
        {
            try
            {
                var tentDetails = _dbContext.TentInformation.ToList();

                if (tentDetails == null)
                {
                    return Ok("No tent data available.");
                }

                return Ok(tentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all tents");
                return StatusCode(500, "An internal error occurred while retrieving all tents.");
            }
        }
    }
}
