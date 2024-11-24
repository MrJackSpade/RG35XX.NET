using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class TextArea : Control
    {
        private IFont _font = ConsoleFont.ms_Sans_Serif_1;

        private float _fontSize = 0.5f;

        private string? _text = null;

        private float _padding = 0.01f;

        public float Padding
        {
            get => _padding;
            set
            {
                _padding = value;
                Application?.MarkDirty();
            }
        }

        public override Color BackgroundColor { get; set; } = Color.White;

        private Color _textColor = Color.Black;

        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Application?.MarkDirty();
            }
        }

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

            int paddingx = (int)(width * _padding);
            int paddingy = (int)(height * _padding);

            if (!string.IsNullOrWhiteSpace(_text))
            {
                Bitmap textmap = _font.Render(_text, width - paddingx * 2, height - paddingy * 2, _textColor, BackgroundColor, FontSize);
                bitmap.DrawBitmap(textmap, paddingx, paddingy);
            }

            if(IsSelected)
            {
                bitmap.DrawBorder(2, HighlightColor);
            }

            return bitmap;
        }
    }
}