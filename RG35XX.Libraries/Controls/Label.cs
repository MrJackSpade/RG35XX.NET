using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class Label : Control
    {
        public IFont Font { get; set; } = ConsoleFont.Px437_IBM_VGA_8x16;

        public string Text { get; set; } = string.Empty;

        public Color TextColor { get; set; } = Color.Black;

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                int x = 0;
                int y = 0;

                foreach (char c in Text)
                {
                    Bitmap charmap = Font.GetCharacterMap(c, TextColor, BackgroundColor);

                    bitmap.Draw(charmap, x, y);

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
}