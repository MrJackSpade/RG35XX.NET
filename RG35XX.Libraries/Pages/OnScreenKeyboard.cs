using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;
using RG35XX.Libraries.Controls;
using RG35XX.Libraries.Controls.KeyboardControls;

namespace RG35XX.Libraries.Pages
{
    public partial class OnScreenKeyboard : Page
    {
        private readonly Keyboard _keyboard;

        private readonly TextArea _textArea;

        public string? Value => _textArea.Text;

        public OnScreenKeyboard(string? value)
        {
            _textArea = new()
            {
                Bounds = new(0, 0, 1, 0.5f),
                Text = value
            };

            _keyboard = new()
            {
                Bounds = new(0, 0.5f, 1, 0.5f),
            };

            _keyboard.KeyDown += this.Keyboard_KeyDown;

            this.AddControl(_textArea);
            this.AddControl(_keyboard);
        }

        public override Bitmap Draw(int width, int height)
        {
            int textWidth = (int)(_textArea.Bounds.Width * width);
            int textHeight = (int)(_textArea.Bounds.Height * height);

            Bitmap textMap = _textArea.Draw(textWidth, textHeight);

            Bitmap bitmap = new(width, height, BackgroundColor);

            bitmap.DrawBitmap(textMap, 0, 0);

            Bitmap keyboardMap = _keyboard.Draw(width, height - textHeight);

            bitmap.DrawBitmap(keyboardMap, 0, textHeight);

            return bitmap;
        }

        public override void OnKey(GamepadKey key)
        {
            if (key == GamepadKey.MENU_DOWN)
            {
                this.Close();
            }

            base.OnKey(key);
        }

        public override void OnOpen()
        {
            base.OnOpen();

            SelectionManager?.Select(_keyboard);
        }

        private void Keyboard_KeyDown(object? sender, KeyboardButton e)
        {
            switch (e.Text)
            {
                case "Back":
                    if (!string.IsNullOrEmpty(_textArea.Text))
                    {
                        _textArea.Text = _textArea.Text?[..^1];
                    }

                    return;

                default:
                    _textArea.Text += e.Character;
                    return;
            }
        }
    }
}