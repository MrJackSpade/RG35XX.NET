using RG35XX.Core.Drawing;
using RG35XX.Core.Interfaces;
using RG35XX.Windows.Forms;
using System.Windows.Forms;

namespace RG35XX.Windows
{
    public class FormsFrameBuffer : IFrameBuffer
    {
        private readonly ManualResetEvent _formDrawn = new(false);

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

            for (int by = 0; by < bitmap.Height; by++)
            {
                for (int bx = 0; bx < bitmap.Width; bx++)
                {
                    Color pixel = bitmap.GetPixel(bx, by);

                    _displayed.SetPixel(x + bx, y + by, pixel);
                }
            }

            this.Dump();
        }

        public void Dump()
        {
            if (_displayed == null)
            {
                throw new System.InvalidOperationException();
            }

            if (_renderer == null)
            {
                throw new InvalidOperationException();
            }

            System.Drawing.Bitmap bmp = new(_displayed.Width, _displayed.Height);

            for (int y = 0; y < _displayed.Height; y++)
            {
                for (int x = 0; x < _displayed.Width; x++)
                {
                    Color pixel = _displayed.GetPixel(x, y);

                    bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                }
            }

            bmp.Save("framebuffer.bmp");

            _renderer.SetImage(bmp);
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