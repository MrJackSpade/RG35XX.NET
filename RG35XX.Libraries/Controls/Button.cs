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
            width = Math.Min(width, height);
            height = Math.Min(width, height);

            lock (_lock)
            {
                Bitmap bitmap;

                if (IsSelected)
                {
                    bitmap = new(width, height, HighlightColor);
                }
                else
                {
                    bitmap = new Bitmap(width, height, TextColor);
                }

                bitmap.DrawRectangle(2, 2, width - 4, height - 4, BackgroundColor, FillStyle.Fill);

                if (Text is not null)
                {
                    Bitmap textMap = Font.Render(Text, width - 6, height - 6, TextColor, BackgroundColor);

                    bitmap.Draw(textMap, 3, 3);
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