using System.Diagnostics;

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