using System.Diagnostics;
using Color = RG35XX.Core.Drawing.Color;

namespace RG35XX.Core.Drawing
{
    [DebuggerDisplay("{Char} {ForegroundColor} {BackgroundColor}")]
    public struct CharData
    {
        public Color BackgroundColor;

        public char Char;

        public Color ForegroundColor;
    }
}