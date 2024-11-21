using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class TextArea : Control
    {
        private IFont _font = ConsoleFont.ms_Sans_Serif_1;

        private float _fontSize = 1;

        private string? _text = null;

        public override Color BackgroundColor { get; set; } = Color.White;

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

        public override bool IsSelectable { get; set; } = true;

        public string? Text
        {
            get => _text;
            set
            {
                _text = value;
                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            Bitmap bitmap = new(width, height, BackgroundColor);
            if (!string.IsNullOrWhiteSpace(_text))
            {
                Bitmap textmap = _font.Render(_text, Color.Black, Color.White, FontSize);
                bitmap.DrawBitmap(textmap, 0, 0);
            }

            return bitmap;
        }
    }
}