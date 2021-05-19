using System.Drawing;
using System.Drawing.Imaging;

namespace Wator.Rendering
{
    public class BitmapRenderer
    {
        public void Render(string file, int[,] field)
        {
            var width = field.GetLength(1);
            var height = field.GetLength(0);
            var bitmap = new Bitmap(width, height);

            for (var row = 0; row < height; row++)
            for (var col = 0; col < width; col++)
            {
                var cell = field[row, col];
                bitmap.SetPixel(col, row, DecideColor(cell));
            }

            WriteToFile(file, bitmap);
        }

        private static void WriteToFile(string file, Bitmap bitmap)
        {
            bitmap.Save(file, ImageFormat.Png);
        }

        private Color DecideColor(int cell)
        {
            return cell switch
            {
                > 0 => Color.LawnGreen,
                < 0 => Color.Crimson,
                _ => Color.DodgerBlue
            };
        }
    }
}