using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class Icon : Control
    {
        private int _borderThickness = 2;

        private IFont _font = ConsoleFont.ms_Sans_Serif_1;

        private float _fontSize = 0.5f;

        private Bitmap? _image = null;

        private int _padding = 1;

        private string? _text;

        private Color _textColor = Color.Black;

        public override Color BackgroundColor { get; set; } = Color.Transparent;

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

        public Bitmap? Image
        {
            get => _image;
            set
            {
                _image = value;
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

        public event EventHandler<Exception> OnImageLoadFailed;

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                if (IsSelected)
                {
                    bitmap.DrawBorder(this._borderThickness, HighlightColor);
                }

                int textHeight = 0;

                if (Text is not null)
                {
                    int spacing = this._padding + this._borderThickness;

                    Bitmap textMap = Font.Render(Text, width - spacing * 2, height - spacing * 2, TextColor, BackgroundColor, FontSize);

                    textHeight = textMap.Height;

                    bitmap.DrawTransparentBitmap((width / 2) - (textMap.Width / 2), height - textHeight - (this._padding + this._borderThickness), textMap);
                }

                int iconWidth = width - (this._padding + this._borderThickness) * 2;
                int iconHeight = height - (this._padding + this._borderThickness) * 2 - textHeight;

                if (Image is not null)
                {
                    Bitmap icon = Image.Scale(iconWidth, iconHeight);

                    bitmap.DrawTransparentBitmap((width / 2) - (icon.Width / 2), 0, icon);
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

        public async Task TryLoadImageAsync(string url)
        {
            try
            {
                Stream imageStream = await new HttpClient().GetStreamAsync(url);

                Image = new Bitmap(imageStream);
            }
            catch (Exception ex)
            {
                OnImageLoadFailed?.Invoke(this, ex);
            }
        }
    }
}