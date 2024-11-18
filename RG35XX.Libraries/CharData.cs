using System.Diagnostics;
using Color = RG35XX.Core.Drawing.Color;

namespace RG35XX.Libraries
{
    [DebuggerDisplay("{Char} {ForegroundColor} {BackgroundColor}")]
    internal struct CharData
    {
        public Color BackgroundColor;

        public char Char;

        public Color ForegroundColor;
    }
}