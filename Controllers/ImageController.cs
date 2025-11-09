using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.Interface;
using SmartHydro_API.LiveCache;

namespace SmartHydro_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly LiveSensorCache _cache;
        private readonly ISensorReadingStore _store;

        public ImageController(IWebHostEnvironment env, LiveSensorCache cache, ISensorReadingStore store)
        {
            _env = env;
            _cache = cache;
            _store = store;
        }



        [HttpGet("{mac}")]
        public IActionResult Image(string mac)
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


                var imagePath = Path.Combine(_env.ContentRootPath, "images", "plant.jpg");

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound();
                }

                var imageBytes = System.IO.File.ReadAllBytes(imagePath);

                //Stack Overflow. 2016
                return File(imageBytes, "image/jpeg");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

    }
}


/*
 REFERENCES
==================
Stack Overflow. 2016. Can an ASP.NET MVC controller return an Image? [Online]. Available at:  https://stackoverflow.com/questions/186062/can-an-asp-net-mvc-controller-return-an-image
 */
