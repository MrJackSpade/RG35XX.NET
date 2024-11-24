using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RG35XX.Core.Drawing
{
    public partial class Bitmap
    {
        public int Height { get; private set; }

        public Color[] Pixels { get; private set; }

        public int Width { get; private set; }

        public Bitmap(Image image)
        {
            this.FillPixelArray(image);
        }

        public Bitmap(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            // Load the stream into an image
            using Image image = Image.Load(stream);

            this.FillPixelArray(image);
        }

        public Bitmap(string embeddedResourcePath, Assembly? assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            AssemblyName assemblyName = assembly.GetName();

            string resourceName = assemblyName.Name + "." + embeddedResourcePath.Replace("/", ".").Replace("\\", ".");

            // Load the embedded resource as a stream
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new ArgumentException("Resource not found", nameof(embeddedResourcePath));
            }

            // Load the stream into an image
            using Image image = Image.Load(stream);

            this.FillPixelArray(image);
        }

        public Bitmap(string embeddedResourcePath) : this(embeddedResourcePath, Assembly.GetEntryAssembly())
        {
        }

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

        [MemberNotNull(nameof(Pixels))]
        private void FillPixelArray(Image image)
        {
            Image<Rgba32> rgbaImage = image.CloneAs<Rgba32>();

            // Initialize the bitmap
            Width = rgbaImage.Width;
            Height = rgbaImage.Height;
            Pixels = new Color[Width * Height];

            // Iterate through the pixels
            // This is not performant and should be changed
            for (int y = 0; y < rgbaImage.Height; y++)
            {
                Memory<Rgba32> pixelRowSpan = rgbaImage.DangerousGetPixelRowMemory(y);

                for (int x = 0; x < rgbaImage.Width; x++)
                {
                    Rgba32 pixel = pixelRowSpan.Span.ToArray()[x];

                    this.SetPixel(x, y, new Color(pixel.R, pixel.G, pixel.B, pixel.A));
                }
            }
        }
    }
}