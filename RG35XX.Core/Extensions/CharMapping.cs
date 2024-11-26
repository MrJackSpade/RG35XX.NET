using RG35XX.Core.Drawing;
using System.Diagnostics;

namespace RG35XX.Core.Extensions
{
    public static partial class FontExtensions
    {
        [DebuggerDisplay("{Character}")]
        private class CharMapping
        {
            public Bitmap Bitmap;

            public char Character;

            public int Height => Bitmap.Height;

            public int Width => Bitmap.Width;
        }
    }
}