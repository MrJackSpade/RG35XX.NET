using RG35XX.Core.Drawing;

namespace RG35XX.Libraries.Controls
{
    public enum Orientation
    {
        Horizontal,

        Vertical
    }

    public class ProgressBar : Control
    {
        private Color _backgroundColor = Color.Grey;

        private Color _foregroundColor = Color.Blue;

        private int _maximum = 100;

        private int _minimum = 0;

        private Orientation _orientation = Orientation.Horizontal;

        private int _value = 0;

        public override Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                Application?.MarkDirty();
            }
        }

        public Color ForegroundColor
        {
            get => _foregroundColor;
            set
            {
                _foregroundColor = value;
                Application?.MarkDirty();
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_value > _maximum)
                {
                    _value = _maximum;
                }

                Application?.MarkDirty();
            }
        }

        public int Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_value < _minimum)
                {
                    _value = _minimum;
                }

                Application?.MarkDirty();
            }
        }

        public Orientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                Application?.MarkDirty();
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                if (value < _minimum)
                {
                    _value = _minimum;
                }
                else if (value > _maximum)
                {
                    _value = _maximum;
                }
                else
                {
                    _value = value;
                }

                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                float progress = (_value - _minimum) / (float)(_maximum - _minimum);

                if (Orientation == Orientation.Horizontal)
                {
                    int barWidth = (int)(width * progress);
                    bitmap.DrawRectangle(0, 0, barWidth, height, ForegroundColor, FillStyle.Fill);
                }
                else
                {
                    int barHeight = (int)(height * progress);
                    bitmap.DrawRectangle(0, height - barHeight, width, barHeight, ForegroundColor, FillStyle.Fill);
                }

                return bitmap;
            }
        }
    }
}