using RG35XX.Core.Drawing;

namespace RG35XX.Libraries.Controls
{
    public class Panel : Control
    {
        public int _borderWidth = 2;

        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            Bitmap b = new(width, height, BackgroundColor);

            if (BorderWidth > 0)
            {
                b.DrawBorder(_borderWidth, FormColors.ControlLightLight, FormColors.ControlDarkDark);
            }

            return b;
        }
    }
}