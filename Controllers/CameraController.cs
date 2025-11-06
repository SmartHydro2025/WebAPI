using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartHydro_API.LiveCache;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //method to accept base64 encoded image data from the esp32 cam
        [HttpPost("base64/{mac}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UploadBase64Image(string mac, [FromBody] CameraImageUpload upload)
        {
            if (string.IsNullOrWhiteSpace(upload.Base64Image))
            {
                return BadRequest("No Base64 image string was provided.");
            }

            try
            {
                //decode base64 string back into a raw byte array
                var imageBytes = Convert.FromBase64String(upload.Base64Image);

                //esp32 cam is set to output jpeg format
                const string contentType = "image/jpeg";

                _logger.LogInformation("Receiving Base64 image from MAC: {mac}, Size: {length} bytes",
                    mac, imageBytes.Length);

                //create an image object using the decoded bytes
                var cameraImage = new CameraImage(imageBytes, contentType, mac);

                //update the cache with the new image
                _imageCache.Update(cameraImage);

                return Ok(new { message = $"Base64 image from {mac} received and cached successfully." });
            }
            catch (FormatException)
            {
                return BadRequest("Invalid Base64 string format. Ensure the data is a valid Base64 string.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Base64 image upload for MAC: {mac}", mac);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing image.");
            }
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
