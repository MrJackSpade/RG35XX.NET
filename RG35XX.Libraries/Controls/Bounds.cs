namespace RG35XX.Libraries.Controls
{
    public struct Bounds
    {
        public float Height;

        public float Width;

        public float X;

        public float Y;

        public Bounds(float x, float y, float width, float height) : this()
        {
            if (x is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            if (width is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}