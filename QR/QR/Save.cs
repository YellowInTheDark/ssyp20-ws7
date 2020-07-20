using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace QR
{
    class Save
    {
        public static void RequestImageSave(int[,] matrix)
        {
            Console.Write("Do you want to save QR as .png? (y/N): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                SaveImage(matrix);
                Console.WriteLine("Successful saved. Check your desktop.");
            }
        }

        private static void SaveImage(int[,] matrix)
        {
            string filePath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
              "QR.png");
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(0);
            Bitmap bmp = new Bitmap(width + 8, height + 8);
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                gfx.FillRectangle(brush, 0, 0, width + 8, height + 8);
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (matrix[i, j] == 1) bmp.SetPixel(j + 4, i + 4, Color.Black);
                    else bmp.SetPixel(j + 4, i + 4, Color.White);
                }
            }
            ResizeImage(bmp, 1024, 1024).Save(filePath);

        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }
}
