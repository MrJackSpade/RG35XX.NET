using RG35XX.Core.Drawing;
using RG35XX.Core.Interfaces;

namespace RG35XX.Core.Extensions
{
    public static partial class FontExtensions
    {
        public static Bitmap GetCharacterMap(this IFont font, char index, Color foreground, Color background, float size = 1, int padding = -1)
        {
            if (index == ' ')
            {
                if (padding == -1 || font.FixedWidth)
                {
                    return new Bitmap((int)(font.Width * size), (int)(font.Height * size), background);
                }
                else
                {
                    Bitmap bullshit = GetCharacterMap(font, 'A', foreground, background, size, 0);

                    return new Bitmap(bullshit.Width, (int)(font.Height * size), background);
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

            if (padding == -1 || font.FixedWidth)
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

                if (charmap != null)
                {
                    charMaps.Add(charmap);
                }
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
            int x = 0;
            int y = 0;

            List<CharMapping> charMaps = [];

            foreach (char c in text)
            {
                if(c == '\r')
                {
                    continue;
                }

                Bitmap charmap = font.GetCharacterMap(c, foregroundColor, backgroundColor, size, padding = 2)
                                 ?? font.GetCharacterMap('?', foregroundColor, backgroundColor, size, padding = 2)
                                 ?? font.GetCharacterMap(' ', foregroundColor, backgroundColor, size, padding = 2);

                if (charmap != null)
                {
                    charMaps.Add(new CharMapping()
                    {
                        Bitmap = charmap,
                        Character = c
                    });
                }
            }

            int renderWidth = Math.Min(width, charMaps.Sum(c => c.Width));

            int renderHeight = charMaps.Max(x => x.Height);

            for(int i = 0; i < charMaps.Count; i++)
            {
                CharMapping charmap = charMaps[i];

                bool isLast = i == charMaps.Count - 1;

                x += charmap.Width;

                if (!isLast && x >= renderWidth || charmap.Character == '\n')
                {
                    x = 0;
                    renderHeight += charMaps.Max(x => x.Height);
                }
            }

            x = 0;
            y = 0;

            Bitmap bitmap = new(renderWidth, renderHeight, backgroundColor);

            foreach (CharMapping charmap in charMaps)
            {
                if (charmap.Character != '\n')
                {
                    bitmap.DrawBitmap(charmap.Bitmap, x, y);
                }

                x += charmap.Width;

                if (x >= renderWidth || charmap.Character == '\n')
                {
                    x = 0;
                    y += charMaps.Max(x => x.Height);
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