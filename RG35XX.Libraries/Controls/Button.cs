using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class Button : Control
    {
        private Color _borderHighlight = FormColors.ControlLightLight;

        private Color _borderShadow = FormColors.ControlDarkDark;

        private int _borderThickness = 2;

        private IFont _font = ConsoleFont.ms_Sans_Serif_1;

        private float _fontSize = 0.5f;

        private int _padding = 1;

        private string? _text;

        private Color _textColor = Color.Black;

        public Color BorderHighlight
        {
            get => _borderHighlight;
            set
            {
                _borderHighlight = value;
                Application?.MarkDirty();
            }
        }

        public Color BorderShadow
        {
            get => _borderShadow;
            set
            {
                _borderShadow = value;
                Application?.MarkDirty();
            }
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
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

        public int Padding
        {
            get => _padding;
            set
            {
                _padding = value;
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

        public event EventHandler? Click;

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                if (IsSelected)
                {
                    bitmap.DrawBorder(_borderThickness, HighlightColor);
                }
                else
                {
                    bitmap.DrawBorder(_borderThickness, BorderHighlight, BorderShadow);
                }

                if (Text is not null)
                {
                    int spacing = _padding + _borderThickness;

                    Bitmap textMap = Font.Render(Text, width - (spacing * 2), height - (spacing * 2), TextColor, BackgroundColor, FontSize);

                    bitmap.DrawBitmap(textMap, (width / 2) - (textMap.Width / 2), (height / 2) - (textMap.Height / 2));
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