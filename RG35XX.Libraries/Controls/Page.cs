using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public class Page : Control
    {
        public Page()
        {
            SelectionManager = new();
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

            selectedControl?.OnKey(key);
        }
    }
}