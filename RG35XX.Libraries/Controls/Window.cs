using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class Window : Control
    {
        private int _borderThickness = 2;

        private IFont _font = ConsoleFont.MS_Sans_Serif_Bold;

        private float _fontSize = 0.6f;

        private string _title = string.Empty;

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

        public override bool IsSelectable { get; set; } = false;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            int widthMinusBorder = width - (2 * _borderThickness);

            int heightMinusBorder = height - (2 * _borderThickness);

            int titleHeight = (int)(_font.Height * _fontSize);

            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                if (BorderThickness > 0)
                {
                    bitmap.DrawBorder(_borderThickness, FormColors.ControlLightLight, FormColors.ControlDarkDark);
                }

                bitmap.DrawGradientRectangle(_borderThickness, _borderThickness, widthMinusBorder, titleHeight, FormColors.TitleBarStart, FormColors.TitleBarEnd, GradientDirection.LeftToRight);

                if (!string.IsNullOrEmpty(_title))
                {
                    Bitmap title = Font.Render(_title, Color.White, Color.Transparent, _fontSize);
                    bitmap.DrawTransparentBitmap(_borderThickness, _borderThickness, title);
                }

                int clientHeight = (int)(heightMinusBorder - titleHeight);

                Bitmap client = base.Draw(widthMinusBorder, clientHeight);

                bitmap.DrawTransparentBitmap(_borderThickness, _borderThickness + titleHeight, client);

                return bitmap;
            }
        }
    }
}