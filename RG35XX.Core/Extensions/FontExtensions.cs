using RG35XX.Core.Drawing;
using RG35XX.Core.Interfaces;

namespace RG35XX.Core.Extensions
{
    public static class FontExtensions
    {
        public static Bitmap GetCharacterMap(this IFont font, char index, Color foreground, Color background, float size = 1, int padding = -1)
        {
            if (index == ' ')
            {
                if (padding == -1)
                {
                    return new Bitmap((int)(font.Width * size), (int)(font.Height * size), background);
                }
                else
                {
                    return new Bitmap(padding * 2, (int)(font.Height * size), background);
                }
            }

            if (!font.Data.TryGetValue(index, out byte[][]? fullData))
            {
                return null;
            }

            int minX = font.Width;
            int maxX = 0;

            // Determine the minimal x-range where the character has foreground pixels
            for (int y = 0; y < font.Height; y++)
            {
                for (int x = 0; x < font.Width; x++)
                {
                    int byteIndex = x / 8;
                    int bitIndex = x % 8;

                    byte pixelByte = fullData[y][byteIndex];

                    if ((pixelByte & (1 << bitIndex)) != 0)
                    {
                        if (x < minX)
                        {
                            minX = x;
                        }

                        if (x > maxX)
                        {
                            maxX = x;
                        }
                    }
                }
            }

            if (padding == -1)
            {
                minX = 0;
                maxX = font.Width - 1;
            }
            else
            {
                minX = Math.Max(0, minX - padding);
                maxX = Math.Min(font.Width - 1, maxX + padding);
            }

            int newWidth = maxX - minX + 1;

            Bitmap bitmap = new(newWidth, font.Height, background);

            for (int y = 0; y < font.Height; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    int byteIndex = x / 8;
                    int bitIndex = x % 8;

                    byte pixelByte = fullData[y][byteIndex];

                    if ((pixelByte & (1 << bitIndex)) != 0)
                    {
                        bitmap.SetPixel(x - minX, y, foreground);
                    }
                    // Background pixels are already set during bitmap initialization
                }
            }

            if (size != 1)
            {
                bitmap = bitmap.Resize((int)(bitmap.Width * size), (int)(bitmap.Height * size), ResizeMode.Average);
            }

            return bitmap;
        }

        public static Bitmap Render(this IFont font, string text, Color foregroundColor, Color backgroundColor, float size = 1, int padding = 1)
        {
            int x = 0;
            int y = 0;

            List<Bitmap> charMaps = [];

            foreach (char c in text)
            {
                Bitmap charmap = font.GetCharacterMap(c, foregroundColor, backgroundColor, size, padding = 2);
                charMaps.Add(charmap);
            }

            int width = charMaps.Sum(c => c.Width);
            int height = charMaps.Max(c => c.Height);

            Bitmap bitmap = new(width, height, backgroundColor);

            foreach (Bitmap charmap in charMaps)
            {
                bitmap.DrawBitmap(charmap, x, y);
                x += charmap.Width;
            }

            return bitmap;
        }

        public static Bitmap Render(this IFont font, string text, int width, int height, Color foregroundColor, Color backgroundColor, float size = 1, int padding = 1)
        {
            Bitmap bitmap = new(width, height, backgroundColor);

            int x = 0;
            int y = 0;

            foreach (char c in text)
            {
                Bitmap charmap = font.GetCharacterMap(c, foregroundColor, backgroundColor, size, padding = 2);

                bitmap.DrawBitmap(charmap, x, y);

                x += charmap.Width;

                if (x >= width)
                {
                    y += charmap.Height;
                    x = 0;
                }

                if (y >= height)
                {
                    break;
                }
            }

            return bitmap;
        }
    }
}