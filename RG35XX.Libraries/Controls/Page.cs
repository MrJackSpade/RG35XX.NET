using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public class Page : Control
    {
        public virtual bool HasTransparency { get; set; } = false;

        public event EventHandler? OnClosing;

        public Page()
        {
            SelectionManager = new();
        }

        public void SelectNext()
        {
            SelectionManager?.SelectNext(this);
        }

        public void SelectPrevious()
        {
            SelectionManager?.SelectPrevious(this);
        }

        public void Close()
        {
            Application?.ClosePage(this);
        }

        public virtual void OnClose()
        {
            OnClosing?.Invoke(this, EventArgs.Empty);
        }

        public override void OnKey(GamepadKey key)
        {
            if (key == GamepadKey.L1_DOWN)
            {
                SelectionManager?.SelectPrevious(this);

                return;
            }

            if (key == GamepadKey.R1_DOWN)
            {
                SelectionManager?.SelectNext(this);

                return;
            }

            Control? selectedControl = SelectionManager?.SelectedControl;

            if (selectedControl is null)
            {
                SelectionManager?.SelectNext(this);
                return;
            }

            Stack<Control> controls = new();

            do
            {
                controls.Push(selectedControl);
                selectedControl = selectedControl.Parent;
            } while (selectedControl != null && selectedControl != this);

            while (controls.Count > 0)
            {
                selectedControl = controls.Pop();

                selectedControl.OnKey(key);
            }
        }

        public virtual void OnOpen()
        {
        }
    }
}