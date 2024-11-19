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

        public void DrawCircle(int centerX, int centerY, int radius, Color color, FillStyle fillStyle)
        {
            if (fillStyle == FillStyle.Fill)
            {
                // Filled circle
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        if ((x * x) + (y * y) <= radius * radius)
                        {
                            this.SetPixel(centerX + x, centerY + y, color);
                        }
                    }
                }
            }
            else // Stroke
            {
                // Outline circle using Midpoint Circle Algorithm
                int x = radius;
                int y = 0;
                int decisionOver2 = 1 - x;

                while (y <= x)
                {
                    this.SetPixel(x + centerX, y + centerY, color);
                    this.SetPixel(y + centerX, x + centerY, color);
                    this.SetPixel(-x + centerX, y + centerY, color);
                    this.SetPixel(-y + centerX, x + centerY, color);
                    this.SetPixel(-x + centerX, -y + centerY, color);
                    this.SetPixel(-y + centerX, -x + centerY, color);
                    this.SetPixel(x + centerX, -y + centerY, color);
                    this.SetPixel(y + centerX, -x + centerY, color);
                    y++;
                    if (decisionOver2 <= 0)
                    {
                        decisionOver2 += (2 * y) + 1;
                    }
                    else
                    {
                        x--;
                        decisionOver2 += (2 * (y - x)) + 1;
                    }
                }
            }
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Color color, int thickness = 1)
        {
            // Use Bresenham's line algorithm
            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                if (thickness == 1)
                {
                    this.SetPixel(x0, y0, color);
                }
                else
                {
                    // Draw a filled circle at the current point to simulate thickness
                    this.DrawCircle(x0, y0, thickness / 2, color, FillStyle.Fill);
                }

                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 >= dy)
                {
                    err += dy;
                    x0 += sx;
                }

                if (e2 <= dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color, FillStyle fillStyle)
        {
            if (fillStyle == FillStyle.Fill)
            {
                // Filled rectangle
                for (int dy = 0; dy < height; dy++)
                {
                    for (int dx = 0; dx < width; dx++)
                    {
                        this.SetPixel(x + dx, y + dy, color);
                    }
                }
            }
            else // Stroke
            {
                // Top and bottom edges
                for (int dx = 0; dx < width; dx++)
                {
                    this.SetPixel(x + dx, y, color); // Top edge
                    this.SetPixel(x + dx, y + height - 1, color); // Bottom edge
                }

                // Left and right edges
                for (int dy = 0; dy < height; dy++)
                {
                    this.SetPixel(x, y + dy, color); // Left edge
                    this.SetPixel(x + width - 1, y + dy, color); // Right edge
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