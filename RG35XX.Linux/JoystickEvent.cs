using System.Runtime.InteropServices;

namespace RG35XX.Linux
{

    public partial class LinuxGamePadReader
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct JoystickEvent
        {
            public uint Time;     // Event timestamp in milliseconds

            public short Value;   // Value

            public byte Type;     // Event type

            public byte Number;   // Axis/button number
        }
    }
}