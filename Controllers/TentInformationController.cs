﻿using Microsoft.AspNetCore.Mvc;
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
            [FromQuery] string networkName,
            [FromQuery] bool favourite
            )
        {
            var tent = new TentInformation
            {
                Mac = mac,
                tentName = name,
                tentLocation = location,
                networkName = networkName,
                favourite = favourite
            };

            _dbContext.TentInformation.Add(tent);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTentDetails), new { mac = tent.Mac }, tent);
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
