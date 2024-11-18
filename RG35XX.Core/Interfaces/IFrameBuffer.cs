using RG35XX.Core.Drawing;

namespace RG35XX.Core.Interfaces
{
    public interface IFrameBuffer
    {
        int Height { get; }

        int Width { get; }

        void Clear();

        void Draw(Bitmap bitmap, int x, int y);

        void Initialize(int width, int height);
    }
}