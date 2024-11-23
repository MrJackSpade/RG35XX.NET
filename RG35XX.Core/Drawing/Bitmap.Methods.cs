namespace RG35XX.Core.Drawing
{
    public partial class Bitmap
    {
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

        public void DrawBitmap(int x, int y, Bitmap bitmap)
        {
            this.DrawBitmap(bitmap, x, y);
        }

        public void DrawBitmap(Bitmap source, int offsetX, int offsetY)
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

        public void DrawBorder(int thickness, Color upperLeft, Color lowerRight)
        {
            // Draw top and bottom borders
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < thickness; y++)
                {
                    this.SetPixel(x, y, upperLeft); // Top border
                    this.SetPixel(x, Height - y - 1, lowerRight); // Bottom border
                }
            }

            // Draw left and right borders
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < thickness; x++)
                {
                    this.SetPixel(x, y, upperLeft); // Left border
                    this.SetPixel(Width - x - 1, y, lowerRight); // Right border
                }
            }
        }

        public void DrawBorder(int thickness, Color color)
        {
            this.DrawBorder(thickness, color, color);
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

        public void DrawGradientRectangle(int x, int y, int width, int height, Color startColor, Color endColor, GradientDirection direction)
        {
            if (direction == GradientDirection.LeftToRight)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    // Calculate the interpolation factor (f) between 0 and 1
                    float f = (width > 1) ? (float)dx / (width - 1) : 0;

                    // Interpolate between startColor and endColor
                    int R = (int)((startColor.R * (1 - f)) + (endColor.R * f));
                    int G = (int)((startColor.G * (1 - f)) + (endColor.G * f));
                    int B = (int)((startColor.B * (1 - f)) + (endColor.B * f));
                    Color color = Color.FromRgb(R, G, B);

                    // Draw a vertical line with the interpolated color
                    for (int dy = 0; dy < height; dy++)
                    {
                        this.SetPixel(x + dx, y + dy, color);
                    }
                }
            }
            else if (direction == GradientDirection.TopToBottom)
            {
                for (int dy = 0; dy < height; dy++)
                {
                    // Calculate the interpolation factor (f) between 0 and 1
                    float f = (height > 1) ? (float)dy / (height - 1) : 0;

                    // Interpolate between startColor and endColor
                    int R = (int)((startColor.R * (1 - f)) + (endColor.R * f));
                    int G = (int)((startColor.G * (1 - f)) + (endColor.G * f));
                    int B = (int)((startColor.B * (1 - f)) + (endColor.B * f));
                    Color color = Color.FromRgb(R, G, B);

                    // Draw a horizontal line with the interpolated color
                    for (int dx = 0; dx < width; dx++)
                    {
                        this.SetPixel(x + dx, y + dy, color);
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

        public void DrawTransparentBitmap(Alignment alignment, Bitmap image)
        {
            int x = 0;
            int y = 0;

            switch (alignment)
            {
                case Alignment.TopLeft:
                    x = 0;
                    y = 0;
                    break;
                case Alignment.TopCenter:
                    x = (this.Width - image.Width) / 2;
                    y = 0;
                    break;
                case Alignment.TopRight:
                    x = this.Width - image.Width;
                    y = 0;
                    break;
                case Alignment.MiddleLeft:
                    x = 0;
                    y = (this.Height - image.Height) / 2;
                    break;
                case Alignment.MiddleCenter:
                    x = (this.Width - image.Width) / 2;
                    y = (this.Height - image.Height) / 2;
                    break;
                case Alignment.MiddleRight:
                    x = this.Width - image.Width;
                    y = (this.Height - image.Height) / 2;
                    break;
                case Alignment.BottomLeft:
                    x = 0;
                    y = this.Height - image.Height;
                    break;
                case Alignment.BottomCenter:
                    x = (this.Width - image.Width) / 2;
                    y = this.Height - image.Height;
                    break;
                case Alignment.BottomRight:
                    x = this.Width - image.Width;
                    y = this.Height - image.Height;
                    break;
            }

            this.DrawTransparentBitmap(x, y, image);
        }

        public void DrawTransparentBitmap(int x, int y, Bitmap bitmap)
        {
            for (int cy = 0; cy < bitmap.Height; cy++)
            {
                for (int cx = 0; cx < bitmap.Width; cx++)
                {
                    Color sourceColor = bitmap.GetPixel(cx, cy);

                    // Skip fully transparent pixels
                    if (sourceColor.A == 0)
                    {
                        continue;
                    }

                    // If pixel is fully opaque, just set it directly
                    if (sourceColor.A == 255)
                    {
                        this.SetPixel(x + cx, y + cy, sourceColor);
                        continue;
                    }

                    // Get the background color
                    Color destColor = this.GetPixel(x + cx, y + cy);

                    // Calculate alpha values
                    float sourceAlpha = sourceColor.A / 255f;
                    float destAlpha = destColor.A / 255f;

                    // Blend the colors using alpha compositing formula
                    int r = (int)((sourceColor.R * sourceAlpha) + (destColor.R * (1 - sourceAlpha)));
                    int g = (int)((sourceColor.G * sourceAlpha) + (destColor.G * (1 - sourceAlpha)));
                    int b = (int)((sourceColor.B * sourceAlpha) + (destColor.B * (1 - sourceAlpha)));
                    int a = (int)(255 * (sourceAlpha + (destAlpha * (1 - sourceAlpha))));

                    // Create new blended color and set the pixel
                    Color blendedColor = Color.FromArgb(a, r, g, b);
                    this.SetPixel(x + cx, y + cy, blendedColor);
                }
            }
        }

        public Bitmap Resize(int newWidth, int newHeight, ResizeMode resizeMode)
        {
            Color[] newPixels = new Color[newWidth * newHeight];

            if (resizeMode == ResizeMode.NearestNeighbor)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    int origY = y * Height / newHeight;
                    for (int x = 0; x < newWidth; x++)
                    {
                        int origX = x * Width / newWidth;
                        newPixels[(y * newWidth) + x] = Pixels[(origY * Width) + origX];
                    }
                }
            }
            else if (resizeMode == ResizeMode.Average)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    float y0 = y * (float)Height / newHeight;
                    float y1 = (y + 1) * (float)Height / newHeight;

                    int origYStart = (int)Math.Floor(y0);
                    int origYEnd = (int)Math.Ceiling(y1);

                    for (int x = 0; x < newWidth; x++)
                    {
                        float x0 = x * (float)Width / newWidth;
                        float x1 = (x + 1) * (float)Width / newWidth;

                        int origXStart = (int)Math.Floor(x0);
                        int origXEnd = (int)Math.Ceiling(x1);

                        int rSum = 0, gSum = 0, bSum = 0, aSum = 0;
                        int count = 0;

                        for (int origY = origYStart; origY < origYEnd && origY < Height; origY++)
                        {
                            for (int origX = origXStart; origX < origXEnd && origX < Width; origX++)
                            {
                                Color c = Pixels[(origY * Width) + origX];
                                rSum += c.R;
                                gSum += c.G;
                                bSum += c.B;
                                aSum += c.A;
                                count++;
                            }
                        }

                        if (count > 0)
                        {
                            int avgA = aSum / count;
                            int avgR = rSum / count;
                            int avgG = gSum / count;
                            int avgB = bSum / count;

                            Color avgColor = Color.FromArgb(avgA, avgR, avgG, avgB);
                            newPixels[(y * newWidth) + x] = avgColor;
                        }
                        else
                        {
                            newPixels[(y * newWidth) + x] = Color.Transparent;
                        }
                    }
                }
            }

            return new Bitmap(newWidth, newHeight, newPixels);
        }

        public Bitmap Scale(int newWidth, int newHeight, ResizeMode resizeMode = ResizeMode.Average, ScaleMode scaleMode = ScaleMode.PreserveAspectRatio)
        {
            int finalWidth = newWidth;
            int finalHeight = newHeight;

            if (scaleMode == ScaleMode.PreserveAspectRatio)
            {
                float aspectRatio = (float)Width / Height;

                if ((float)newWidth / newHeight > aspectRatio)
                {
                    // Adjust width to preserve aspect ratio
                    finalWidth = (int)(newHeight * aspectRatio);
                }
                else
                {
                    // Adjust height to preserve aspect ratio
                    finalHeight = (int)(newWidth / aspectRatio);
                }
            }

            return this.Resize(finalWidth, finalHeight, resizeMode);
        }

        internal Bitmap Crop(int offset_x, int offset_y, object width, object height)
        {
            throw new NotImplementedException();
        }
    }
}