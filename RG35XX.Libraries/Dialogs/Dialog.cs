using RG35XX.Libraries.Controls;

namespace RG35XX.Libraries.Dialogs
{
    public enum DialogResult
    {
        OK,

        Cancel
    }

    public class Dialog : Page
    {
        internal TaskCompletionSource<DialogResult> _tcs = new();

        private readonly Control _window;

        public override bool HasTransparency { get; set; } = true;

        internal Task<DialogResult> Task => _tcs.Task;

        public Dialog(string title)
        {
            _window = new Window
            {
                Title = title,
                Bounds = new Bounds(.2f, .3f, 0.6f, 0.4f)
            };

            base.AddControl(_window);
        }

        public Dialog()
        {
            _window = new Window
            {
                Bounds = new Bounds(.2f, .2f, 0.8f, 0.8f)
            };

            base.AddControl(_window);
        }

        public override bool AddControl(Control control)
        {
            return _window.AddControl(control);
        }

        public override bool RemoveControl(Control control)
        {
            return _window.RemoveControl(control);
        }

        protected void Cancel()
        {
            _tcs.SetResult(DialogResult.Cancel);
        }

        protected void Ok()
        {
            _tcs.SetResult(DialogResult.OK);
        }
    }
}