namespace RG35XX.Core.Drawing
{
    public class Bitmap
    {
        public int Height { get; }

        public Color[] Pixels { get; }

        public int Width { get; }

        public Bitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new Color[width * height];
        }

        public Bitmap(int width, int height, Color[] pixels)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public Bitmap(int width, int height, Color color)
        {
            Width = width;
            Height = height;
            Pixels = new Color[width * height];

            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] = color;
            }
        }

        public Bitmap Crop(int x, int y, int width, int height)
        {
            // Ensure dimensions are positive
            width = Math.Max(0, width);
            height = Math.Max(0, height);

            // Create new bitmap for cropped result
            Bitmap result = new(width, height);

            // For each pixel in crop region
            for (int cy = 0; cy < height; cy++)
            {
                for (int cx = 0; cx < width; cx++)
                {
                    // Calculate source coordinates
                    int sourceX = x + cx;
                    int sourceY = y + cy;

                    // Only copy if within bounds of source bitmap
                    if (sourceX >= 0 && sourceX < Width && sourceY >= 0 && sourceY < Height)
                    {
                        result.SetPixel(cx, cy, this.GetPixel(sourceX, sourceY));
                    }
                }
            }

            return result;
        }

        public void Draw(Bitmap source, int offsetX, int offsetY)
        {
            // For each pixel in source bitmap
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    // Calculate destination coordinates
                    int destX = x + offsetX;
                    int destY = y + offsetY;

                    // Only draw if within bounds of destination bitmap
                    if (destX >= 0 && destX < Width && destY >= 0 && destY < Height)
                    {
                        this.SetPixel(destX, destY, source.GetPixel(x, y));
                    }
                }
            }
        }

        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new IndexOutOfRangeException();
            }

            return Pixels[(y * Width) + x];
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return;
            }

            Pixels[(y * Width) + x] = color;
        }

        internal Bitmap Crop(int offset_x, int offset_y, object width, object height)
        {
            throw new NotImplementedException();
        }
    }
}