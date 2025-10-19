using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.LiveCache;
using System.ComponentModel.DataAnnotations;

namespace SmartHydro_API.Controllers
{
    public class CameraController : Controller
    {
        private readonly LiveImageCache _imageCache; //logs image data
        private readonly ILogger<CameraController> _logger;

        public CameraController(LiveImageCache imageCache, ILogger<CameraController> logger)
        {
            _imageCache = imageCache;
            _logger = logger;
        }

        //method to add image to cache (which goes to mqtt)
        [HttpPost("{mac}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImage(string mac, [Required] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file was uploaded.");
            }

            _logger.LogInformation("Receiving image from MAC: {mac}, Size: {length} bytes, Type: {type}",
                mac, image.Length, image.ContentType);

            //read the image data into a byte array
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            //create an image object
            var cameraImage = new CameraImage(imageBytes, image.ContentType, mac);

            //update the cache with the new image
            _imageCache.Update(mac, cameraImage);

            return Ok(new { message = $"Image from {mac} received and cached successfully." });
        }

        //method to get latest image of a tent
        [HttpGet("{mac}/latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetLatestImage(string mac)
        {
            var latestImage = _imageCache.GetLatest(mac); //pull what image is last stored in cache

            if (latestImage == null)
            {
                return NotFound($"No image found for MAC address: {mac}");
            }

            //return the image file with the correct content type like image/jpeg
            return File(latestImage.ImageBytes, latestImage.ContentType);
        }
    }
}
