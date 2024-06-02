/*using SkiaSharp;
using System;
using System.IO;

namespace ImageComparisonApi.Services
{
    public class ImageComparisonService
    {
        public double CalculateSimilarityPercentage(Stream image1Stream, Stream image2Stream)
        {
            // Load images from streams
            var image1 = LoadImageFromStream(image1Stream);
            var image2 = LoadImageFromStream(image2Stream);

            // Resize images to a common size
            ResizeImages(ref image1, ref image2);

            // Convert images to grayscale
            var grayImage1 = ConvertToGrayscale(image1);
            var grayImage2 = ConvertToGrayscale(image2);

            // Calculate the difference
            int width = grayImage1.Width;
            int height = grayImage1.Height;
            long diffPixels = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel1 = grayImage1.GetPixel(x, y);
                    var pixel2 = grayImage2.GetPixel(x, y);
                    var diff = (byte)Math.Abs(pixel1.Red - pixel2.Red);

                    if (diff > 50) // threshold
                    {
                        diffPixels++;
                    }
                }
            }

            // Calculate percentage difference
            double totalPixels = width * height;
            double differencePercentage = (double)diffPixels / totalPixels;

            // Return similarity percentage
            return (1.0 - differencePercentage) * 100.0;
        }

        private void ResizeImages(ref SKBitmap image1, ref SKBitmap image2)
        {
            // Get the maximum dimensions of the two images
            int maxWidth = Math.Max(image1.Width, image2.Width);
            int maxHeight = Math.Max(image1.Height, image2.Height);

            // Resize both images to have the same dimensions
            ResizeImage(ref image1, maxWidth, maxHeight);
            ResizeImage(ref image2, maxWidth, maxHeight);
        }

        private void ResizeImage(ref SKBitmap image, int width, int height)
        {
            var resizedImage = new SKBitmap(width, height);

            using (var canvas = new SKCanvas(resizedImage))
            {
                canvas.Clear();
                canvas.DrawBitmap(image, SKRect.Create(width, height), null);
            }

            image.Dispose();
            image = resizedImage;
        }

        private SKBitmap LoadImageFromStream(Stream stream)
        {
            return SKBitmap.Decode(stream);
        }

        private SKBitmap ConvertToGrayscale(SKBitmap bitmap)
        {
            var grayBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

            using (var canvas = new SKCanvas(grayBitmap))
            {
                var paint = new SKPaint
                {
                    ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
                    {
                        0.3f, 0.3f, 0.3f, 0, 0,
                        0.59f, 0.59f, 0.59f, 0, 0,
                        0.11f, 0.11f, 0.11f, 0, 0,
                        0, 0, 0, 1, 0

                    })
                };

                canvas.DrawBitmap(bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height), paint);
            }
            using (var image = SKImage.FromBitmap(grayBitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            {
                // save the data to a stream
                using (var stream = File.OpenWrite(Path.Combine("PRateek", "1.png")))
                {
                    data.SaveTo(stream);
                }
            }
            return grayBitmap;
        }
    }
}


*/

using SkiaSharp;
using System;
using System.IO;

namespace ImageComparisonApi.Services
{
    public class ImageComparisonService
    {
        public double CalculateSimilarityPercentage(Stream image1Stream, Stream image2Stream)
        {
            // Load images from streams
            var image1 = LoadImageFromStream(image1Stream);
            var image2 = LoadImageFromStream(image2Stream);

            PreprocessImage( image1);
            PreprocessImage( image2);

            // Resize images to a common size
            ResizeImages(ref image1, ref image2);

            // Convert images to grayscale
            var grayImage1 = ConvertToGrayscale(image1);
            var grayImage2 = ConvertToGrayscale(image2);

            // Calculate the difference
            int diffPixels = CalculateDifference(grayImage1, grayImage2);

            // Calculate percentage difference
            double totalPixels = grayImage1.Width * grayImage1.Height;
            double differencePercentage = (double)diffPixels / totalPixels;

            // Return similarity percentage
            return (1.0 - differencePercentage) * 100.0;
        }

        private int CalculateDifference(SKBitmap image1, SKBitmap image2)
        {
            int diffPixels = 0;
            int width = Math.Min(image1.Width, image2.Width);
            int height = Math.Min(image1.Height, image2.Height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel1 = image1.GetPixel(x, y);
                    var pixel2 = image2.GetPixel(x, y);
                    var diff = Math.Abs(pixel1.Red - pixel2.Red);

                    if (diff > 50) // Adjust threshold as needed
                    {
                        diffPixels++;
                    }
                }
            }

            return diffPixels;
        }

        private SKBitmap ConvertToGrayscale(SKBitmap bitmap)
        {
            var grayBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

            using (var canvas = new SKCanvas(grayBitmap))
            {
                var paint = new SKPaint
                {
                    ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
                    {
                        0.3f, 0.3f, 0.3f, 0, 0,
                        0.59f, 0.59f, 0.59f, 0, 0,
                        0.11f, 0.11f, 0.11f, 0, 0,
                        0, 0, 0, 1, 0
                    })
                };

                canvas.DrawBitmap(bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height), paint);
            }

            return grayBitmap;
        }

        private SKBitmap LoadImageFromStream(Stream stream)
        {
            return SKBitmap.Decode(stream);
        }

        private void ResizeImages(ref SKBitmap image1, ref SKBitmap image2)
        {
            // Get the maximum dimensions of the two images
            int maxWidth = Math.Max(image1.Width, image2.Width);
            int maxHeight = Math.Max(image1.Height, image2.Height);

            // Resize both images to have the same dimensions
            ResizeImage(ref image1, maxWidth, maxHeight);
            ResizeImage(ref image2, maxWidth, maxHeight);
        }

        private void ResizeImage(ref SKBitmap image, int width, int height)
        {
            var resizedImage = new SKBitmap(width, height);

            using (var canvas = new SKCanvas(resizedImage))
            {
                canvas.Clear();
                canvas.DrawBitmap(image, SKRect.Create(width, height), null);
            }

            image.Dispose();
            image = resizedImage;
        }

        // Additional Methods for Accuracy Enhancement

        private SKBitmap PreprocessImage(SKBitmap bitmap)
        {
            // Example: Apply Gaussian blur to reduce noise
            using (var blurredBitmap = new SKBitmap(bitmap.Width, bitmap.Height))
            using (var canvas = new SKCanvas(blurredBitmap))
            {
                var paint = new SKPaint { ImageFilter = SKImageFilter.CreateBlur(1.5f, 1.5f) };
                canvas.DrawBitmap(bitmap, 0, 0, paint);
                return blurredBitmap.Copy();
            }
        }

      
    }
}



