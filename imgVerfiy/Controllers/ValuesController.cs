using ImageComparisonApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace imgVerfiy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ImageComparisonService _imageComparisonService;

        public ValuesController(ImageComparisonService imageComparisonService)
        {
            this._imageComparisonService = imageComparisonService;
        }

        [HttpPost("compare")]
        public async Task<IActionResult> CompareImages(IFormFile image1, IFormFile image2)
        {
            if (image1 == null || image2 == null)
            {
                return BadRequest("Both images are required.");
            }

            using var image1Stream = image1.OpenReadStream();
            using var image2Stream = image2.OpenReadStream();

            double similarityPercentage = _imageComparisonService.CalculateSimilarityPercentage(image1Stream, image2Stream);

            return Ok(new { SimilarityPercentage = similarityPercentage });
        }



        [HttpPost("comparepdf")]
        public async Task<IActionResult> CompareSignatures(IFormFile pdfFile, IFormFile signatureImage)
        {

            if (pdfFile == null || signatureImage == null)
            {
                return BadRequest("Both PDF file and signature image are required.");
            }

            int pageNumber = 0;
            int x = 590;
            int y = 150;
            int width = 150;
            int height = 55;

            // Define the signature region (adjust based on your PDF layout)
            Rectangle signatureRegion = new Rectangle(x, y, width, height);

            using (var pdfStream = pdfFile.OpenReadStream())
            using (var signatureStream = signatureImage.OpenReadStream())
            {
                // Extract the signature from the PDF
                Bitmap extractedSignature;
                try
                {
                    extractedSignature = PdfSignatureExtractor.ExtractSignature(pdfStream, signatureRegion, pageNumber);
                    MemoryStream memoryStream = new MemoryStream();
                    extractedSignature.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                }
                catch (Exception ex)
                {
                    return BadRequest($"Error extracting signature from PDF: {ex.Message}");
                }

                // Load the uploaded signature image
                Bitmap signatureImageBitmap;
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await signatureStream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        signatureImageBitmap = new Bitmap(memoryStream);
                        signatureImageBitmap.Save("sign.png");
                    }
                }
                
                catch (Exception ex)
                {
                    return BadRequest($"Error loading signature image: {ex.Message}");
                }

                // Compare the signatures
                // SimilarityScore matchingPercentage;

                var filePath = $"pdf.png";
                Stream image1Stream = null;
                if (System.IO.File.Exists(filePath))
                {
                    //image1Stream = await System.IO.File.ReadAllBytesAsync(filePath);
                    image1Stream = System.IO.File.OpenRead(filePath);
                }
                    using var image2Stream = signatureImage.OpenReadStream();

                double similarityPercentage = _imageComparisonService.CalculateSimilarityPercentage(image1Stream, image2Stream);

                return Ok(new { SimilarityPercentage = similarityPercentage });
            }
        }
    }
}
