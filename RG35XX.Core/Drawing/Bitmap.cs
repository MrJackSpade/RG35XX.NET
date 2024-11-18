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
            // Determine the area to copy
            int copyWidth = Math.Min(Width - offsetX, source.Width);
            int copyHeight = Math.Min(Height - offsetY, source.Height);

            // Return if there's nothing to copy
            if (copyWidth <= 0 || copyHeight <= 0)
            {
                return;
            }

            // For each row in the area to copy
            for (int y = 0; y < copyHeight; y++)
            {
                // Calculate source and destination indices
                int sourceIndex = y * source.Width;
                int destIndex = ((y + offsetY) * Width) + offsetX;

                // Copy a block of pixels from source to destination
                Array.Copy(source.Pixels, sourceIndex, Pixels, destIndex, copyWidth);
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