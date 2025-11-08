using Microsoft.AspNetCore.Mvc;

namespace SmartHydro_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {

        private readonly IWebHostEnvironment _env;

        public ImageController(IWebHostEnvironment env)
        {
            _env = env;
        }



        [HttpGet]
        public IActionResult Image(string name)
        {

            try
            {
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
