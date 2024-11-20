using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using RG35XX.Core.Interfaces;
using RG35XX.Desktop.Avalonia;
using System.Runtime.InteropServices;
using Bitmap = RG35XX.Core.Drawing.Bitmap;
using Color = RG35XX.Core.Drawing.Color;

namespace RG35XX.Desktop
{
    public class FormsFrameBuffer : IFrameBuffer
    {
        private readonly ManualResetEvent _formDrawn = new(false);

        private readonly object _rendererLock = new();

        private readonly bool _shouldExit;

        private Bitmap? _displayed;

        private MyBitmapWindow _renderer;

        private Dispatcher? _uiDispatcher;

        private Thread? _uiThread;

        public int Height => _displayed?.Height ?? 0;

        public int Width => _displayed?.Width ?? 0;

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<AvaloniaApp>()
                                 .UsePlatformDetect();
        }

        public void AppMain(Application app, string[] args)
        {
            _renderer = new MyBitmapWindow(_displayed.Width, _displayed.Height);

            _renderer.Opened += (sender, e) =>
            {
                lock (_rendererLock)
                {
                    _formDrawn.Set();
                }
            };

            _renderer.Show();

            // Start the Avalonia event loop (blocks until app exit)
            app.Run(_renderer);
        }

        public void Clear()
        {
            if (_displayed == null)
            {
                throw new InvalidOperationException();
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

        public void Draw(Bitmap bitmap, int x, int y)
        {
            if (_displayed == null)
            {
                throw new InvalidOperationException();
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

            _uiDispatcher.InvokeAsync(() => _renderer.DisplayCustomBitmap(_displayed));
        }

        public void Initialize(int width, int height)
        {
            _displayed = new(width, height);
            this.ShowWindow();
            _formDrawn.WaitOne();
        }

        public void ShowWindow()
        {
            ManualResetEvent dispatcherReady = new(false);

            // Start the Avalonia UI on a new thread
            _uiThread = new Thread(() =>
            {
                // Initialize Avalonia within the thread
                BuildAvaloniaApp()
                    .AfterSetup(_ =>
                    {
                        // Capture the Dispatcher
                        _uiDispatcher = Dispatcher.UIThread;
                        dispatcherReady.Set();
                    })
                    .Start(this.AppMain, null);
            });

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _uiThread.SetApartmentState(ApartmentState.STA);
                _uiThread.IsBackground = true;
            }

            _uiThread.Start();

            // Wait until the Dispatcher is ready
            dispatcherReady.WaitOne();
        }
    }
}