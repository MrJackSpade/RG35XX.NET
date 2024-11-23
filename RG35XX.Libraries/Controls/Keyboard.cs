using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.GamePads;
using RG35XX.Libraries.Controls.KeyboardControls;

namespace RG35XX.Libraries.Controls
{
    public class Keyboard : Control
    {
        private readonly KeyboardView[] _keyboardViews = new KeyboardView[3];

        private int _selectedKeyIndex = 0;

        private int _selectedViewIndex = 0;

        public override bool IsSelectable { get; set; } = true;

        public override bool TabThroughChildren { get; set; } = false;

        private KeyboardButton SelectedKey => _keyboardViews[_selectedViewIndex].Buttons.ElementAt(_selectedKeyIndex);

        private KeyboardView SelectedView => _keyboardViews[_selectedViewIndex];

        public event EventHandler<KeyboardButton> KeyDown;

        public Keyboard()
        {
            BackgroundColor = Color.White;

            KeyboardView lowercaseView = this.BuildView([
                "1234567890",
                "qwertyuiop",
                "asdfghjkl",
                "↑zxcvbnm←",
                ", ."
            ]);

            KeyboardView uppercaseView = this.BuildView([
                "1234567890",
                "QWERTYUIOP",
                "ASDFGHJKL",
                "$ZXCVBNM←",
                ", ."
            ]);

            KeyboardView symbolView = this.BuildView([
                "1234567890",
                "!@#$%^&*()",
                "[]{}\\|;:'\"",
                "↓<>/?`~←",
                ", ."
            ]);

            symbolView.GetKey('↓').Text = "Lower";
            lowercaseView.GetKey('↑').Text = "Upper";
            uppercaseView.GetKey('$').Text = "Symbol";
            symbolView.GetKey('←').Text = "Back";
            lowercaseView.GetKey('←').Text = "Back";
            uppercaseView.GetKey('←').Text = "Back";

            _keyboardViews[0] = lowercaseView;
            _keyboardViews[1] = uppercaseView;
            _keyboardViews[2] = symbolView;

            symbolView.SetSelectable(false);
            uppercaseView.SetSelectable(false);

            for (int i = 0; i < _keyboardViews.Length; i++)
            {
                _keyboardViews[0].Align();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            Bitmap keyView = new(width, height, BackgroundColor);

            int index = 0;

            foreach (KeyboardButton button in _keyboardViews[_selectedViewIndex].Buttons)
            {
                int buttonWidth = (int)(button.Bounds.Width * width);
                int buttonHeight = (int)(button.Bounds.Height * height);
                int buttonX = (int)(button.Bounds.X * width);
                int buttonY = (int)(button.Bounds.Y * height);

                Bitmap buttonMap = button.Draw(buttonWidth, buttonHeight);

                if (index == _selectedKeyIndex)
                {
                    buttonMap.DrawBorder(2, HighlightColor);
                }

                keyView.DrawBitmap(buttonMap, buttonX, buttonY);

                index++;
            }

            return keyView;
        }

        public override void OnKey(GamepadKey key)
        {
            base.OnKey(key);

            if (key == GamepadKey.UP)
            {
                if (this.GetOffset(-1, out int foundOffset))
                {
                    _selectedKeyIndex = foundOffset;
                    Application?.MarkDirty();
                }
            }
            else if (key == GamepadKey.DOWN)
            {
                if (this.GetOffset(1, out int foundOffset))
                {
                    _selectedKeyIndex = foundOffset;
                    Application?.MarkDirty();
                }
            }
            else if (key == GamepadKey.LEFT)
            {
                _selectedKeyIndex--;
                Application?.MarkDirty();
            }
            else if (key == GamepadKey.RIGHT)
            {
                _selectedKeyIndex++;
                Application?.MarkDirty();
            }
            else if (key == GamepadKey.A_DOWN)
            {
                this.SendKey(SelectedKey);
            }
            else if (key == GamepadKey.B_DOWN)
            {
                this.SendKey(new KeyboardButton('←')
                {
                    Text = "Back"
                });
            }
        }

        private KeyboardView BuildView(string[] rows)
        {
            KeyboardView view = new();

            foreach (string row in rows)
            {
                KeyboardRow buttonRow = new(row);

                view.AddRow(buttonRow);
            }

            return view;
        }

        private bool GetOffset(int direction, out int foundOffset)
        {
            foundOffset = -1;

            KeyboardButton currentButton = SelectedKey;

            KeyboardView currentView = SelectedView;

            KeyboardRow currentRow = SelectedView.GetRow(currentButton.Character);

            int rowIndex = currentView.Rows.IndexOf(currentRow);

            if (rowIndex < 0)
            {
                return false;
            }

            int newIndex = rowIndex + direction;

            if (newIndex < 0 || newIndex >= currentView.Rows.Count)
            {
                return false;
            }

            KeyboardRow aboveRow = currentView.Rows[newIndex];

            float currentCenter = currentButton.Bounds.X + (currentButton.Bounds.Width / 2);

            KeyboardButton closestButton = aboveRow.Buttons.Values.OrderBy(x => Math.Abs(x.Bounds.X + (x.Bounds.Width / 2) - currentCenter)).First();

            foundOffset = currentView.Buttons.IndexOf(closestButton);

            return foundOffset >= 0;
        }

        private void SendKey(KeyboardButton button)
        {
            int lastSelectedView = _selectedViewIndex;

            string? text = button.Text;

            switch (text)
            {
                case "Upper":
                    _selectedViewIndex = 1;
                    break;

                case "Lower":
                    _selectedViewIndex = 0;
                    break;

                case "Symbol":
                    _selectedViewIndex = 2;
                    break;

                default:
                    KeyDown?.Invoke(this, button);
                    break;
            }

            if (lastSelectedView != _selectedViewIndex)
            {
                for (int i = 0; i < _keyboardViews.Length; i++)
                {
                    _keyboardViews[i].SetSelectable(i == _selectedViewIndex);
                }
            }

            Application?.MarkDirty();
        }
    }
}