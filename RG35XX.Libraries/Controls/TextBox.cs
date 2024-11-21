using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;
using RG35XX.Libraries.Pages;

namespace RG35XX.Libraries.Controls
{
    public class TextBox : Control
    {
        private IFont _font = ConsoleFont.ms_Sans_Serif_1;

        private float _fontSize = 1;

        private int _insetBorderThickness = 2;

        private string _placeHolder = string.Empty;

        private Color _placeholderColor = Color.Grey;

        private Color _textColor = Color.Black;

        private string? _value;

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

        public int InsetBorderThickness
        {
            get => _insetBorderThickness;
            set
            {
                _insetBorderThickness = value;
                Application?.MarkDirty();
            }
        }

        public override bool IsSelectable { get; set; } = true;

        public string PlaceHolder
        {
            get => _placeHolder; set
            {
                _placeHolder = value;
                Application?.MarkDirty();
            }
        }

        public Color PlaceHolderColor
        {
            get => _placeholderColor;
            set
            {
                _placeholderColor = value;
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

        public string? Value
        {
            get => _value;
            set
            {
                _value = value;
                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                Bitmap bitmap = new(width - (_insetBorderThickness * 2), height - (_insetBorderThickness * 2), BackgroundColor);

                if (string.IsNullOrWhiteSpace(Value) && !string.IsNullOrWhiteSpace(PlaceHolder))
                {
                    Bitmap textMap = Font.Render(PlaceHolder, width, height, PlaceHolderColor, BackgroundColor, FontSize);
                    bitmap.DrawBitmap(textMap, _insetBorderThickness, _insetBorderThickness);
                }
                else if (!string.IsNullOrWhiteSpace(Value))
                {
                    Bitmap textMap = Font.Render(Value, width, height, TextColor, BackgroundColor, FontSize);
                    bitmap.DrawBitmap(textMap, _insetBorderThickness, _insetBorderThickness);
                }

                if (IsSelected)
                {
                    bitmap.DrawBorder(_insetBorderThickness, HighlightColor);
                }
                else
                {
                    bitmap.DrawBorder(_insetBorderThickness, FormColors.ControlDarkDark, FormColors.ControlLightLight);
                }

                return bitmap;
            }
        }

        public override void OnKey(GamepadKey key)
        {
            if (key == GamepadKey.A_DOWN)
            {
                OnScreenKeyboard osk = new(_value);
                Application.OpenPage(osk);
                osk.OnClosing += (s, e) =>
                {
                    _value = osk.Value;
                    Application?.MarkDirty();
                };
            }

            base.OnKey(key);
        }
    }
}