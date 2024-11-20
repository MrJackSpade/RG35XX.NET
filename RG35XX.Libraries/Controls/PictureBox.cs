using RG35XX.Core.Drawing;

namespace RG35XX.Libraries.Controls
{
    public class PictureBox : Control
    {
        private Bitmap? _image;

        public Bitmap? Image
        {
            get => _image;
            set
            {
                _image = value;
                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                if (_image is null)
                {
                    return new Bitmap(width, height, BackgroundColor);
                }

                return _image.Resize(width, height, ResizeMode.Average);
            }
        }
    }
}