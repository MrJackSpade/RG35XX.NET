using System.Runtime.InteropServices;

namespace RG35XX.Handheld
{
    public partial class LinuxFramebuffer
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct fb_var_screeninfo
        {
            public uint xres;

            public uint yres;

            public uint xres_virtual;

            public uint yres_virtual;

            public uint xoffset;

            public uint yoffset;

            public uint bits_per_pixel;

            public uint grayscale;

            public uint red_offset;

            public uint red_length;

            public uint red_msb_right;

            public uint green_offset;

            public uint green_length;

            public uint green_msb_right;

            public uint blue_offset;

            public uint blue_length;

            public uint blue_msb_right;

            public uint transp_offset;

            public uint transp_length;

            public uint transp_msb_right;

            public uint nonstd;

            public uint activate;

            public uint height;

            public uint width;

            public uint accel_flags;

            public uint pixclock;

            public uint left_margin;

            public uint right_margin;

            public uint upper_margin;

            public uint lower_margin;

            public uint hsync_len;

            public uint vsync_len;

            public uint sync;

            public uint vmode;

            public uint rotate;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] reserved;
        }
    }
}