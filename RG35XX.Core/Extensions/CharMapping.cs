using RG35XX.Core.Drawing;

namespace RG35XX.Core.Extensions
{
    public static partial class FontExtensions
    {
        class CharMapping
        {
            public char Character;
            public Bitmap Bitmap;
            public int Width => Bitmap.Width;
            public int Height => Bitmap.Height;
        }
    }
}