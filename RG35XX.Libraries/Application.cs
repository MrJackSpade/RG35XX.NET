using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;
using RG35XX.Libraries.Controls;

namespace RG35XX.Libraries
{
    public class Application
    {
        private readonly IFrameBuffer _frameBuffer;

        private readonly IGamePadReader _gamePadReader;

        private readonly Thread _gamepadThread;

        private readonly object _lock = new();

        private readonly Stack<Page> _pages = new();

        private readonly Thread _renderingThread;

        private readonly TaskCompletionSource<bool> _tcs;

        private bool _running;

        public Application(int width, int height)
        {
            _tcs = new TaskCompletionSource<bool>();
            _frameBuffer = new FrameBuffer();
            _frameBuffer.Initialize(width, height);
            _gamePadReader = new GamePadReader();
            _gamePadReader.Initialize();

            _renderingThread = new Thread(this.Render);
            _gamepadThread = new Thread(this.ReadGamePad);
        }

        public void ClosePage()
        {
            lock (_lock)
            {
                if (_pages.Count > 0)
                {
                    Page page = _pages.Pop();
                    page.Dispose();
                }

                if (_pages.Count == 0)
                {
                    _running = false;
                    _tcs.SetResult(true);
                }
            }
        }

        public Task Execute()
        {
            _running = true;

            _renderingThread.Start();

            _gamepadThread.Start();

            return _tcs.Task;
        }

        public void OpenPage(Page page)
        {
            lock (_lock)
            {
                _pages.Push(page);
            }
        }

        private void ReadGamePad(object? obj)
        {
            do
            {
                GamepadKey key = _gamePadReader.WaitForInput();

                lock (_lock)
                {
                    if (_pages.Count > 0)
                    {
                        Page page = _pages.Peek();
                        page.OnKey(key);
                    }
                }
            } while (_running);
        }

        private void Render()
        {
            while (_running)
            {
                lock (_lock)
                {
                    Page page = _pages.Peek();
                    Bitmap bitmap = page.Draw(_frameBuffer.Width, _frameBuffer.Height);
                    _frameBuffer.Draw(bitmap, 0, 0);
                }
            }
        }
    }
}