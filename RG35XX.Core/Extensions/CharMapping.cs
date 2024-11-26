using RG35XX.Core.Drawing;
using System.Diagnostics;

namespace RG35XX.Core.Extensions
{
    public static partial class FontExtensions
    {
        [DebuggerDisplay("{Character}")]
        class CharMapping
        {
            public char Character;

            public Bitmap Bitmap;
            public int Width => Bitmap.Width;
            public int Height => Bitmap.Height;
        }
    }
}