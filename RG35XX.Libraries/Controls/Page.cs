using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public class Page : Control
    {
        public Page()
        {
            SelectionManager = new();
        }

        public void Close()
        {
            Application?.ClosePage(this);
        }

        public override void OnKey(GamepadKey key)
        {
            if (key == GamepadKey.L1_DOWN)
            {
                SelectionManager?.SelectLast(this);

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
    }
}