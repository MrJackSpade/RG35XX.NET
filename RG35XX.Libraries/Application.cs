using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;
using RG35XX.Libraries.Controls;
using RG35XX.Libraries.Dialogs;

namespace RG35XX.Libraries
{
    public class Application
    {
        private readonly IFrameBuffer _frameBuffer;

        private readonly IGamePadReader _gamePadReader;

        private readonly Thread _gamepadThread;

        private readonly object _lock = new();

        private readonly List<Page> _pages = [];

        private readonly AutoResetEvent _rendererWait = new(false);

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

        public Application(IFrameBuffer frameBuffer)
        {
            _tcs = new TaskCompletionSource<bool>();
            _frameBuffer = frameBuffer;
            _gamePadReader = new GamePadReader();
            _gamePadReader.Initialize();

            _renderingThread = new Thread(this.Render);
            _gamepadThread = new Thread(this.ReadGamePad);
        }

        public void ClosePage()
        {
            this.EnsureRunning();

            lock (_lock)
            {
                if (_pages.Count > 0)
                {
                    Page page = _pages.Pop();
                    page.OnClose();
                    page.Dispose();
                    this.MarkDirty();
                }

                if (_pages.Count == 0)
                {
                    _running = false;
                    _tcs.SetResult(true);
                }
            }
        }

        public void Dispose()
        {
            foreach (Page page in _pages)
            {
                page.Dispose();
            }
        }

        public void EnsureRunning()
        {
            if (!_running)
            {
                throw new InvalidOperationException("Application is not executing");
            }
        }

        public void Execute()
        {
            _running = true;

            _renderingThread.Start();

            _gamepadThread.Start();
        }

        public void MarkDirty()
        {
            _rendererWait.Set();
        }

        public void OpenPage(Page page)
        {
            this.EnsureRunning();

            lock (_lock)
            {
                page.SetApplication(this);
                _pages.Push(page);
                page.OnOpen();
                this.MarkDirty();
            }
        }

        public async Task<DialogResult> ShowDialog(Dialog dialog)
        {
            this.EnsureRunning();

            this.OpenPage(dialog);

            DialogResult result = await dialog.Task;

            this.ClosePage(dialog);

            return result;
        }

        public async Task<bool> WaitForClose()
        {
            return await _tcs.Task;
        }

        internal void ClosePage(Page page)
        {
            this.EnsureRunning();

            lock (_lock)
            {
                if (_pages.Count == 0 || !_pages.Peek().Equals(page))
                {
                    return;
                }

                this.ClosePage();
            }
        }

        private void ReadGamePad(object? obj)
        {
            this.EnsureRunning();

            do
            {
                GamepadKey key = _gamePadReader.WaitForInput();

                if (_pages.Count > 0)
                {
                    Page page = _pages.Peek();

                    page.OnKey(key);
                }
            } while (_running);
        }

        private void Render()
        {
            this.EnsureRunning();

            while (_running)
            {
                _rendererWait.WaitOne();

                Stack<Page> toRender = new();

                Page page = _pages.Peek();

                int peekIndex = 1;

                do
                {
                    toRender.Push(page);

                    if (peekIndex >= _pages.Count)
                    {
                        break;
                    }

                    page = _pages.Peek(peekIndex++);
                } while (page.HasTransparency);

                Bitmap bitmap = new(_frameBuffer.Width, _frameBuffer.Height);

                while (toRender.Count > 0)
                {
                    page = toRender.Pop();
                    bitmap.DrawBitmap(page.Draw(_frameBuffer.Width, _frameBuffer.Height), 0, 0);
                }

                _frameBuffer.Draw(bitmap, 0, 0);
            }
        }
    }
}