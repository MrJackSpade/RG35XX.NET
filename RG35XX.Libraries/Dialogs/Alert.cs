using RG35XX.Core.Drawing;
using RG35XX.Libraries.Controls;

namespace RG35XX.Libraries.Dialogs
{
    public class Alert : Dialog
    {
        private readonly Button _okButton;

        private readonly string _text;

        private readonly TextArea _textArea;

        public Alert(string title, string text = "", bool showOk = true) : base(title)
        {
            _text = text;

            float textAreaHeight = showOk ? 0.6f : 0.8f;

            _textArea = new TextArea
            {
                Text = _text,
                FontSize = 0.5f,
                Padding = 0.05f,
                BackgroundColor = BackgroundColor,
                TextColor = Color.Black,
                Bounds = new Bounds(0.1f, 0.1f, 0.8f, textAreaHeight),
                IsSelectable = false
            };

            this.AddControl(_textArea);

            if (showOk)
            {
                _okButton = new Button
                {
                    Text = "OK",
                    Bounds = new Bounds(0.1f, 0.7f, 0.8f, 0.2f),
                    BackgroundColor = BackgroundColor
                };

                _okButton.Click += (sender, e) => this.Ok();

                this.AddControl(_okButton);
            }
        }
    }
}