using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public class CheckBox : Control
    {
        public Color ForegroundColor { get; set; } = Color.White;

        public bool IsChecked { get; set; } = false;

        public override bool IsSelectable { get; set; } = true;

        public override Bitmap Draw(int width, int height)
        {
            width = Math.Min(width, height);
            height = Math.Min(width, height);

            lock (_lock)
            {
                Bitmap bitmap = new(width, height, ForegroundColor);

                if (IsSelected)
                {
                    bitmap.DrawBorder(2, HighlightColor);
                }

                if (IsChecked)
                {
                    bitmap.DrawLine(0, 0, width, height, Color.Black);
                    bitmap.DrawLine(width, 0, 0, height, Color.Black);
                }

                return bitmap;
            }
        }

        public override void OnKey(GamepadKey key)
        {
            if (key is GamepadKey.A_DOWN or GamepadKey.START_DOWN)
            {
                IsChecked = !IsChecked;
                Application?.MarkDirty();
            }

            base.OnKey(key);
        }
    }
}