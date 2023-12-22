using SkiaSharp;
using System;
using System.IO;

namespace AssignmentDevOpsProject_fwald.Services
{
    public static class ImageProcessor
    {
        public static Stream AddWeatherDataToImage(Stream baseImageStream, string weatherData, (float x, float y) position, float fontSize = 24, string colorHex = "#FFFFFF")
        {
            try
            {
                baseImageStream.Position = 0;

                using var originalBitmap = SKBitmap.Decode(baseImageStream);
                using var imageSurface = SKSurface.Create(new SKImageInfo(originalBitmap.Width, originalBitmap.Height));
                var canvas = imageSurface.Canvas;
                canvas.DrawBitmap(originalBitmap, new SKPoint(0, 0));

                using var paint = new SKPaint
                {
                    Color = SKColor.Parse(colorHex),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    TextAlign = SKTextAlign.Left,
                    TextSize = fontSize
                };
                canvas.DrawText(weatherData, position.x, position.y, paint);

                using var image = imageSurface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                var outputStream = new MemoryStream();
                data.SaveTo(outputStream);
                outputStream.Position = 0;

                return outputStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
