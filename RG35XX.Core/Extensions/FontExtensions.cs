using RG35XX.Core.Drawing;
using RG35XX.Core.Interfaces;

namespace RG35XX.Core.Extensions
{
    public static class FontExtensions
    {
        public static Bitmap GetCharacterMap(this IFont font, byte index, Color foreground, Color background)
        {
            if (index < 0 || index >= font.Data.Length)
            {
                return null;
            }

            byte[] data = font.Data[index];

            Bitmap bitmap = new(font.Width, font.Height);

            for (int y = 0; y < font.Height; y++)
            {
                for (int x = 0; x < font.Width; x++)
                {
                    byte pixel = data[y];

                    if ((pixel & (1 << x)) != 0)
                    {
                        bitmap.SetPixel(x, y, foreground);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, background);
                    }
                }
            }

            return bitmap;
        }
    }
}