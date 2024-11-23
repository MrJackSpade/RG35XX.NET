using RG35XX.Core.Drawing;

namespace RG35XX.Libraries.Controls
{
    public class PictureBox : Control
    {
        private Bitmap? _image;

        private ScaleMode _scaleMode = ScaleMode.PreserveAspectRatio;

        public Bitmap? Image
        {
            get => _image;
            set
            {
                _image = value;
                Application?.MarkDirty();
            }
        }

        public ScaleMode ScaleMode
        {
            get => _scaleMode;
            set
            {
                _scaleMode = value;
                Application?.MarkDirty();
            }
        }

        public event EventHandler<Exception>? OnImageLoadFailed;

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                if (_image is null)
                {
                    return new Bitmap(width, height, BackgroundColor);
                }

                return _image.Scale(width, height, ResizeMode.Average, _scaleMode);
            }
        }

        public async Task TryLoadImageAsync(string url)
        {
            try
            {
                Stream imageStream = await new HttpClient().GetStreamAsync(url);

                Image = new Bitmap(imageStream);
            }
            catch (Exception ex)
            {
                OnImageLoadFailed?.Invoke(this, ex);
            }
        }
    }
}