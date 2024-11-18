using RG35XX.Core.Drawing;
using RG35XX.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace RG35XX.Libraries
{
    public class FrameBuffer : IFrameBuffer
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
        private IFrameBuffer _frameBuffer;

        public int Height => _frameBuffer.Height;

        public int Width => _frameBuffer.Width;

        public FrameBuffer()
        {
#if DEBUG
            _frameBuffer = new RG35XX.Windows.FormsFrameBuffer();
#else
            _frameBuffer = new RG35XX.Linux.LinuxFramebuffer();
#endif
        }

        public void Clear()
        {
            _frameBuffer.Clear();
        }

        public void Draw(Bitmap bitmap, int x, int y)
        {
            _frameBuffer.Draw(bitmap, x, y);
        }

        public void Initialize(int width, int height)
        {
            _frameBuffer.Initialize(width, height);
        }
    }
}