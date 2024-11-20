using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class Label : Control
    {
        private IFont _font = ConsoleFont.ms_Sans_Serif_1;

        private float _fontSize = 1;

        private string? _text;

        private Color _textColor = Color.Black;

        public IFont Font
        {
            get => _font;
            set
            {
                _font = value;
                Application?.MarkDirty();
            }
        }

        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                Application?.MarkDirty();
            }
        }

        public string? Text
        {
            get => _text;
            set
            {
                _text = value;
                Application?.MarkDirty();
            }
        }

        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                if (Text is not null)
                {
                    Bitmap textMap = Font.Render(Text, width, height, TextColor, BackgroundColor, FontSize);
                    bitmap.DrawBitmap(textMap, 0, 0);
                }

                return bitmap;
            }
        }
    }
}