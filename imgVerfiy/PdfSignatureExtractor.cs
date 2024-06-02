using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace imgVerfiy
{
    public class PdfSignatureExtractor
    {
        public static Bitmap ExtractSignature(Stream pdfStream, Rectangle signatureRegion, int pageNumber = 0)
        {
            using (var pdfDocument = PdfDocument.Load(pdfStream))
            {
                if (pageNumber > pdfDocument.PageCount)
                    throw new ArgumentException("Invalid page number.");

                // Render the PDF page to an image
                using (var pageImage = pdfDocument.Render(pageNumber, 300, 300, PdfRenderFlags.CorrectFromDpi))
                {
                    // Create a new Bitmap
                    Bitmap bitmapImage = new Bitmap(pageImage.Width, pageImage.Height);

                    // Draw the PdfiumViewer image onto the Bitmap
                    using (Graphics graphics = Graphics.FromImage(bitmapImage))
                    {
                        graphics.DrawImage(pageImage, 0, 0);
                    }

                    // Crop the image to the signature region
                    Bitmap croppedImage = CropImage(bitmapImage, signatureRegion);
                    return croppedImage;
                }
            }
        }

        private static Bitmap CropImage(Bitmap image, Rectangle cropArea)
        {
            Bitmap croppedImage = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(image, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
                            cropArea,
                            GraphicsUnit.Pixel);
            }
            croppedImage.Save("pdf.png");
            return croppedImage;
        }
    }
}
