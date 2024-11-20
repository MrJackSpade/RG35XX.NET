using Microsoft.Win32.SafeHandles;
using RG35XX.Core.Drawing;
using RG35XX.Core.Interfaces;
using System.Runtime.InteropServices;

namespace RG35XX.Handheld
{
    public partial class LinuxFramebuffer : IDisposable, IFrameBuffer
    {
        private const uint FBIOGET_FSCREENINFO = 0x4602;

        private const uint FBIOGET_VSCREENINFO = 0x4600;

        private const uint FBIOPUT_VSCREENINFO = 0x4601;

        private readonly SafeFileHandle _fbHandle;

        private readonly int _frameBufferSize;

        private fb_fix_screeninfo _fixInfo;

        private nint _mappedMemory;

        private fb_var_screeninfo _varInfo;

        public int BitsPerPixel => (int)_varInfo.bits_per_pixel;

        public int Height => (int)_varInfo.yres;

        public int Width => (int)_varInfo.xres;

        public LinuxFramebuffer(string fbDevice = "/dev/fb0")
        {
            // Open the framebuffer device
            _fbHandle = File.OpenHandle(fbDevice, FileMode.Open, FileAccess.ReadWrite);

            // Get fixed screen information
            _fixInfo = new fb_fix_screeninfo();
            nint fixInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_fixInfo));
            try
            {
                int result = ioctl(_fbHandle, FBIOGET_FSCREENINFO, fixInfoPtr);
                if (result != 0)
                {
                    throw new IOException($"Failed to get fixed screen info: {Marshal.GetLastWin32Error()}");
                }

                _fixInfo = Marshal.PtrToStructure<fb_fix_screeninfo>(fixInfoPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(fixInfoPtr);
            }

            // Get variable screen information
            _varInfo = new fb_var_screeninfo();
            nint varInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_varInfo));
            try
            {
                int result = ioctl(_fbHandle, FBIOGET_VSCREENINFO, varInfoPtr);
                if (result != 0)
                {
                    throw new IOException($"Failed to get variable screen info: {Marshal.GetLastWin32Error()}");
                }

                _varInfo = Marshal.PtrToStructure<fb_var_screeninfo>(varInfoPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(varInfoPtr);
            }

            // Calculate framebuffer size
            _frameBufferSize = (int)(_fixInfo.line_length * _varInfo.yres);

            // Map the framebuffer memory
            _mappedMemory = mmap(nint.Zero, (uint)_frameBufferSize,
                0x1 | 0x2, // PROT_READ | PROT_WRITE
                0x1, // MAP_SHARED
                _fbHandle, 0);

            if (_mappedMemory == new nint(-1))
            {
                throw new IOException($"Failed to map framebuffer memory: {Marshal.GetLastWin32Error()}");
            }
        }

        public void Clear()
        {
            // Clear the entire framebuffer by writing zeros
            unsafe
            {
                byte* ptr = (byte*)_mappedMemory;
                for (int i = 0; i < _frameBufferSize; i++)
                {
                    ptr[i] = 0;
                }
            }
        }

        public void Dispose()
        {
            if (_mappedMemory != nint.Zero && _mappedMemory != new nint(-1))
            {
                Console.WriteLine("Unmapping framebuffer memory...");
                munmap(_mappedMemory, (uint)_frameBufferSize);
                _mappedMemory = nint.Zero;
            }

            if (_fbHandle != null && !_fbHandle.IsInvalid)
            {
                Console.WriteLine("Closing framebuffer device...");
                _fbHandle.Dispose();
            }
        }

        public void Draw(Bitmap bitmap, int x, int y)
        {
            ArgumentNullException.ThrowIfNull(bitmap);

            // Wait for vsync before drawing
            this.WaitForVSync();

            // Reset the display offset to 0,0
            _varInfo.xoffset = 0;
            _varInfo.yoffset = 0;

            // Update the screen info
            nint varInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_varInfo));
            try
            {
                Marshal.StructureToPtr(_varInfo, varInfoPtr, false);
                int result = ioctl(_fbHandle, FBIOPUT_VSCREENINFO, varInfoPtr);
                if (result != 0)
                {
                    throw new IOException($"Failed to update variable screen info: {Marshal.GetLastWin32Error()}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(varInfoPtr);
            }

            unsafe
            {
                byte* fbPtr = (byte*)_mappedMemory;
                int bytesPerPixel = BitsPerPixel / 8;
                int stride = (int)_fixInfo.line_length;

                int bitmapWidth = bitmap.Width;
                int bitmapHeight = bitmap.Height;
                Color[] pixels = bitmap.Pixels;

                if (BitsPerPixel == 32)
                {
                    // Precompute component offsets and lengths
                    int blueOffset = (int)_varInfo.blue_offset;
                    int greenOffset = (int)_varInfo.green_offset;
                    int redOffset = (int)_varInfo.red_offset;
                    int transpOffset = (int)_varInfo.transp_offset;
                    int blueLength = (int)_varInfo.blue_length;
                    int greenLength = (int)_varInfo.green_length;
                    int redLength = (int)_varInfo.red_length;
                    int transpLength = (int)_varInfo.transp_length;

                    bool hasTransparency = transpLength > 0;

                    for (int py = 0; py < bitmapHeight; py++)
                    {
                        int fbY = y + py;
                        if (fbY < 0 || fbY >= Height)
                        {
                            continue;
                        }

                        int pixelRowStartIndex = py * bitmapWidth;
                        int fbRowOffset = fbY * stride;

                        for (int px = 0; px < bitmapWidth; px++)
                        {
                            int fbX = x + px;
                            if (fbX < 0 || fbX >= Width)
                            {
                                continue;
                            }

                            Color pixel = pixels[pixelRowStartIndex + px];
                            int fbOffset = fbRowOffset + (fbX * bytesPerPixel);

                            // Construct the 32-bit pixel value
                            uint pixelValue = 0;

                            // Note: Ensure that the color component values are within the expected range
                            pixelValue |= ((uint)pixel.B & 0xFF) >> (8 - blueLength) << blueOffset;
                            pixelValue |= ((uint)pixel.G & 0xFF) >> (8 - greenLength) << greenOffset;
                            pixelValue |= ((uint)pixel.R & 0xFF) >> (8 - redLength) << redOffset;

                            if (hasTransparency)
                            {
                                pixelValue |= ((uint)pixel.A & 0xFF) >> (8 - transpLength) << transpOffset;
                            }

                            // Write the 32-bit pixel value directly
                            *(uint*)(fbPtr + fbOffset) = pixelValue;
                        }
                    }
                }
                else if (BitsPerPixel == 16)
                {
                    for (int py = 0; py < bitmapHeight; py++)
                    {
                        int fbY = y + py;
                        if (fbY < 0 || fbY >= Height)
                        {
                            continue;
                        }

                        int pixelRowStartIndex = py * bitmapWidth;
                        int fbRowOffset = fbY * stride;

                        for (int px = 0; px < bitmapWidth; px++)
                        {
                            int fbX = x + px;
                            if (fbX < 0 || fbX >= Width)
                            {
                                continue;
                            }

                            Color pixel = pixels[pixelRowStartIndex + px];
                            int fbOffset = fbRowOffset + (fbX * bytesPerPixel);

                            // Convert to RGB565 format
                            ushort color565 = (ushort)(
                                ((pixel.R & 0xF8) << 8) |
                                ((pixel.G & 0xFC) << 3) |
                                (pixel.B >> 3)
                            );

                            fbPtr[fbOffset] = (byte)(color565 & 0xFF);
                            fbPtr[fbOffset + 1] = (byte)(color565 >> 8);
                        }
                    }
                }
                // Add handlers for other color depths if needed
            }
        }

        public void Initialize(int width, int height)
        {
        }

        [DllImport("libc", SetLastError = true)]
        private static extern int ioctl(SafeFileHandle fd, uint cmd, nint arg);

        [DllImport("libc", SetLastError = true)]
        private static extern nint mmap(nint addr, uint length, int prot, int flags, SafeFileHandle fd, int offset);

        [DllImport("libc", SetLastError = true)]
        private static extern int munmap(nint addr, uint length);

        private string GetErrorDescription(int errorCode)
        {
            return errorCode switch
            {
                1 => "Operation not permitted",
                2 => "No such file or directory",
                13 => "Permission denied",
                16 => "Device or resource busy",
                19 => "No such device",
                22 => "Invalid argument",
                _ => "Unknown error",
            };
        }

        private void WaitForVSync()
        {
            try
            {
                int dummy = 0;
                nint dummyPtr = Marshal.AllocHGlobal(sizeof(int));
                try
                {
                    Marshal.WriteInt32(dummyPtr, dummy);
                    // FBIO_WAITFORVSYNC constant is typically 0x4620
                    int result = ioctl(_fbHandle, 0x4620, dummyPtr);
                    if (result != 0)
                    {
                        Console.WriteLine($"VSync wait failed: {Marshal.GetLastWin32Error()}");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(dummyPtr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VSync wait exception: {ex.Message}");
            }
        }
    }
}