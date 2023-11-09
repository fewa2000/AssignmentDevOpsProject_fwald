using SkiaSharp;
using System;
using System.IO;

namespace ImageEditor
{
    public class ImageHelper
    {
        public static Stream AddTextToImage(Stream imageStream, params (string text, (float x, float y) position, float fontSize, string colorHex)[] texts)
        {
            try
            {
                // Reset the stream position
                imageStream.Position = 0;

                // Decode the image stream into a bitmap
                using var originalBitmap = SKBitmap.Decode(imageStream);
                using var imageSurface = SKSurface.Create(new SKImageInfo(originalBitmap.Width, originalBitmap.Height));
                var canvas = imageSurface.Canvas;

                // Draw the original image on the canvas
                canvas.DrawBitmap(originalBitmap, new SKPoint(0, 0));

                foreach (var (text, (x, y), fontSize, colorHex) in texts)
                {
                    // Create paint for text
                    using var paint = new SKPaint
                    {
                        Color = SKColor.Parse(colorHex),
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill,
                        TextAlign = SKTextAlign.Left,
                        TextSize = fontSize
                    };

                    // Draw the text
                    canvas.DrawText(text, x, y, paint);
                }

                // Convert the canvas into an image and then a stream
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

        internal static Stream AddTextToImage(MemoryStream imageStream, (byte[] weatherData, (int, int), int, string) value)
        {
            throw new NotImplementedException();
        }
    }
}
