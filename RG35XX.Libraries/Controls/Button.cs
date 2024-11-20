using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class Button : Control
    {
        private IFont _font = ConsoleFont.Px437_IBM_VGA_8x16;

        private float _fontSize = 1;

        private string? _text;

        private Color _textColor = Color.Black;

        public IFont Font
        {
            get => _font;
            set
            {
                _font = value;
                Renderer?.MarkDirty();
            }
        }

        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                Renderer?.MarkDirty();
            }
        }

        public override bool IsSelectable { get; set; } = true;

        public string? Text
        {
            get => _text;
            set
            {
                _text = value;
                Renderer?.MarkDirty();
            }
        }

        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Renderer?.MarkDirty();
            }
        }

        public event EventHandler? Click;

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                if (IsSelected)
                {
                    bitmap.DrawBorder(2, HighlightColor);
                }
                else
                {
                    bitmap.DrawBorder(2, TextColor);
                }

                if (Text is not null)
                {
                    Bitmap textMap = Font.Render(Text, width - 6, height - 6, TextColor, BackgroundColor, FontSize);

                    bitmap.DrawBitmap(textMap, 3, 3);
                }

                return bitmap;
            }
        }

        public override void OnKey(GamepadKey key)
        {
            if (key == GamepadKey.A_DOWN)
            {
                this.OnClick();
            }
        }

        protected virtual void OnClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}