using System.Runtime.InteropServices;

namespace RG35XX.Linux
{
    public partial class LinuxFramebuffer
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct fb_fix_screeninfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] id;

            public nint smem_start;

            public uint smem_len;

            public uint type;

            public uint type_aux;

            public uint visual;

            public ushort xpanstep;

            public ushort ypanstep;

            public ushort ywrapstep;

            public uint line_length;

            public nint mmio_start;

            public uint mmio_len;

            public uint accel;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public ushort[] reserved;
        }
    }
}