using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using System.Threading.Tasks;

namespace MultimediaEditorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditorController : ControllerBase
    {
        [HttpPost("process")]
        public async Task<IActionResult> ProcessImage(
            [FromForm] IFormFile file, 
            [FromForm] string filter = "grayscale", 
            [FromForm] int brightness = 0)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No image uploaded.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var image = await Image.LoadAsync(memoryStream);

            image.Mutate(x =>
            {
                if (filter == "grayscale") x.Grayscale();
                if (filter == "invert") x.Invert();
                if (filter == "gaussian") x.GaussianBlur(3.0f);
                
                if (brightness != 0)
                {
                    float factor = 1.0f + (brightness / 100.0f);
                    x.Brightness(factor);
                }
            });

            var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, new JpegEncoder());
            outputStream.Position = 0;

            return File(outputStream, "image/jpeg");
        }
    }
}