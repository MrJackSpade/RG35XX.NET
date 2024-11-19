using RG35XX.Core.Interfaces;
using RG35XX.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Bitmap = RG35XX.Core.Drawing.Bitmap;
using Color = RG35XX.Core.Drawing.Color;

namespace RG35XX.Windows
{
    public class FormsFrameBuffer : IFrameBuffer
    {
        private readonly ManualResetEvent _formDrawn = new(false);

        private readonly object _rendererLock = new();

        private Bitmap? _displayed;

        private Renderer? _renderer;

        private bool _shouldExit;

        public int Height => _displayed?.Height ?? 0;

        public int Width => _displayed?.Width ?? 0;

        public void Clear()
        {
            if (_displayed == null)
            {
                throw new System.InvalidOperationException();
            }

            for (int y = 0; y < _displayed.Height; y++)
            {
                for (int x = 0; x < _displayed.Width; x++)
                {
                    _displayed.SetPixel(x, y, Color.Black);
                }
            }

            this.Dump();
        }

        public bool DiagnoseFramebuffer(string fbDevice = "/dev/fb0")
        {
            return true;
        }

        public void Draw(Bitmap bitmap, int x, int y)
        {
            if (_displayed == null)
            {
                throw new System.InvalidOperationException();
            }

            _displayed.Draw(bitmap, x, y);

            this.Dump();
        }

        public void Dump()
        {
            if (_displayed == null)
            {
                throw new InvalidOperationException();
            }

            if (_renderer == null)
            {
                throw new InvalidOperationException();
            }

            int width = _displayed.Width;
            int height = _displayed.Height;
            Color[] pixels = _displayed.Pixels; // Ensure this is your Color[] array with correct dimensions

            // Create a new bitmap with the same dimensions and pixel format
            System.Drawing.Bitmap bmp = new(width, height, PixelFormat.Format32bppArgb);

            // Lock the bitmap's bits
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                bmp.PixelFormat);

            int bytesPerPixel = 4; // For Format32bppArgb
            int srcStride = width * bytesPerPixel;
            int dstStride = bmpData.Stride;

            unsafe
            {
                byte* dstScan0 = (byte*)bmpData.Scan0;

                fixed (Color* srcScan0 = pixels)
                {
                    byte* srcRow = (byte*)srcScan0;
                    byte* dstRow = dstScan0;

                    for (int y = 0; y < height; y++)
                    {
                        // Copy one scanline at a time
                        Buffer.MemoryCopy(
                            source: srcRow,
                            destination: dstRow,
                            destinationSizeInBytes: dstStride,
                            sourceBytesToCopy: srcStride);

                        srcRow += srcStride;
                        dstRow += dstStride;
                    }
                }
            }

            // Unlock the bits
            bmp.UnlockBits(bmpData);

            lock (_rendererLock)
            {
                _renderer.Invoke(() => _renderer.SetImage(bmp));
            }
        }

        public void Initialize(int width, int height)
        {
            // Start the Windows Form on a separate thread
            Thread formThread = new(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                _renderer = new();
                _renderer.Initialize(width, height);
                _renderer.Show();

                _renderer.FormClosed += (s, e) => _shouldExit = true;
                _renderer.Shown += (s, e) => _formDrawn.Set();
                Application.Run(_renderer);
            });

            formThread.SetApartmentState(ApartmentState.STA);
            formThread.Start();

            _displayed = new(width, height);
            _formDrawn.WaitOne();
        }
    }
}