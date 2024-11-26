using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;
using RG35XX.Libraries.Controls;

namespace RG35XX.Libraries.Dialogs
{
    public class Confirm : Dialog
    {
        private readonly Button _cancelButton;

        private readonly Button _okButton;

        private readonly string _text;

        private readonly TextArea _textArea;

        public Confirm(string title, string text = "") : base(title)
        {
            _text = text;

            _textArea = new TextArea
            {
                Text = _text,
                FontSize = 0.5f,
                Padding = 0.05f,
                BackgroundColor = BackgroundColor,
                TextColor = Color.Black,
                Bounds = new Bounds(0.1f, 0.1f, 0.8f, 0.6f),
                IsSelectable = false
            };

            _okButton = new Button
            {
                Text = "OK",
                Bounds = new Bounds(0.55f, 0.7f, 0.4f, 0.2f),
                BackgroundColor = BackgroundColor
            };

            _cancelButton = new Button
            {
                Text = "Cancel",
                Bounds = new Bounds(0.05f, 0.7f, 0.4f, 0.2f),
                BackgroundColor = BackgroundColor
            };

            _okButton.Click += (sender, e) => this.Ok();
            _cancelButton.Click += (sender, e) => this.Cancel();

            this.AddControl(_textArea);
            this.AddControl(_okButton);
            this.AddControl(_cancelButton);
        }

        public override void OnKey(GamepadKey key)
        {
            if (key.IsCancel())
            {
                this.Cancel();
                return;
            }

            if (key is GamepadKey.LEFT or GamepadKey.RIGHT or GamepadKey.UP or GamepadKey.DOWN)
            {
                SelectionManager?.SelectNext(this);
                return;
            }

            base.OnKey(key);
        }
    }
}