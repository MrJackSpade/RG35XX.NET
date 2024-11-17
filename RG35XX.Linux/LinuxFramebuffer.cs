using Microsoft.Win32.SafeHandles;
using RG35XX.Core.Drawing;
using RG35XX.Core.Interfaces;
using System.Runtime.InteropServices;

namespace RG35XX.Linux
{
    public partial class LinuxFramebuffer : IDisposable, IFrameBuffer
    {

        private const uint FBIOGET_FSCREENINFO = 0x4602;

        private const uint FBIOGET_VSCREENINFO = 0x4600;

        private const uint FBIOPUT_VSCREENINFO = 0x4601;

        private SafeFileHandle _fbHandle;

        private fb_fix_screeninfo _fixInfo;

        private int _frameBufferSize;

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

        public bool DiagnoseFramebuffer(string fbDevice = "/dev/fb0")
        {
            Console.WriteLine($"Starting framebuffer diagnosis for {fbDevice}");

            // Check if device exists
            if (!File.Exists(fbDevice))
            {
                Console.WriteLine($"Error: {fbDevice} does not exist");
                return false;
            }

            Console.WriteLine("Checking device permissions...");
            try
            {
                FileInfo info = new(fbDevice);
                Console.WriteLine($"Device permissions: {info.UnixFileMode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting device info: {ex.Message}");
                return false;
            }

            Console.WriteLine("Attempting to open device...");
            try
            {
                _fbHandle = File.OpenHandle(fbDevice, FileMode.Open, FileAccess.ReadWrite);
                Console.WriteLine("Successfully opened device");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open device: {ex.Message}");
                return false;
            }

            // Get fixed screen information
            Console.WriteLine("Requesting fixed screen information...");
            _fixInfo = new fb_fix_screeninfo();
            nint fixInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_fixInfo));
            try
            {
                int result = ioctl(_fbHandle, FBIOGET_FSCREENINFO, fixInfoPtr);
                if (result != 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to get fixed screen info: {error} ({this.GetErrorDescription(error)})");
                    return false;
                }

                _fixInfo = Marshal.PtrToStructure<fb_fix_screeninfo>(fixInfoPtr);
                Console.WriteLine($"Fixed info received: line_length={_fixInfo.line_length}, mmio_len={_fixInfo.mmio_len}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during fixed info request: {ex.Message}");
                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(fixInfoPtr);
            }

            // Get variable screen information
            Console.WriteLine("Requesting variable screen information...");
            _varInfo = new fb_var_screeninfo();
            nint varInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_varInfo));
            try
            {
                int result = ioctl(_fbHandle, FBIOGET_VSCREENINFO, varInfoPtr);
                if (result != 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Failed to get variable screen info: {error} ({this.GetErrorDescription(error)})");
                    return false;
                }

                _varInfo = Marshal.PtrToStructure<fb_var_screeninfo>(varInfoPtr);
                Console.WriteLine($"Variable info received: resolution={_varInfo.xres}x{_varInfo.yres}, bpp={_varInfo.bits_per_pixel}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during variable info request: {ex.Message}");
                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(varInfoPtr);
            }

            // Calculate and verify framebuffer size
            _frameBufferSize = (int)(_fixInfo.line_length * _varInfo.yres);
            Console.WriteLine($"Calculated framebuffer size: {_frameBufferSize} bytes");

            // Attempt memory mapping
            Console.WriteLine("Attempting to map framebuffer memory...");
            _mappedMemory = mmap(nint.Zero, (uint)_frameBufferSize,
                0x1 | 0x2, // PROT_READ | PROT_WRITE
                0x1, // MAP_SHARED
                _fbHandle, 0);

            if (_mappedMemory == new nint(-1))
            {
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine($"Failed to map memory: {error} ({this.GetErrorDescription(error)})");
                return false;
            }

            Console.WriteLine($"Successfully mapped framebuffer memory at {_mappedMemory}");

            // Try writing a test pattern
            Console.WriteLine("Attempting to write test pattern...");
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)_mappedMemory;
                    // Write a small white rectangle in the top-left corner
                    for (int y = 0; y < 10; y++)
                    {
                        for (int x = 0; x < 10; x++)
                        {
                            int offset = (y * (int)_fixInfo.line_length) + (x * ((int)_varInfo.bits_per_pixel / 8));
                            for (int b = 0; b < _varInfo.bits_per_pixel / 8; b++)
                            {
                                ptr[offset + b] = 255;
                            }
                        }
                    }
                }

                Console.WriteLine("Test pattern written successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write test pattern: {ex.Message}");
                return false;
            }

            return true;
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

                for (int py = 0; py < bitmap.Height; py++)
                {
                    int fbY = y + py;

                    if (fbY < 0)
                    {
                        continue;
                    }

                    if (fbY >= Height)
                    {
                        break;
                    }

                    for (int px = 0; px < bitmap.Width; px++)
                    {
                        int fbX = x + px;

                        if (fbX < 0)
                        {
                            continue;
                        }

                        if (fbX >= Width)
                        {
                            break;
                        }

                        Color pixel = bitmap.GetPixel(px, py);

                        // Calculate position in framebuffer
                        int fbOffset = (fbY * stride) + (fbX * bytesPerPixel);

                        // Write pixel data based on framebuffer format
                        if (BitsPerPixel == 32)
                        {
                            fbPtr[fbOffset + ((int)_varInfo.blue_offset / 8)] = pixel.B;
                            fbPtr[fbOffset + ((int)_varInfo.green_offset / 8)] = pixel.G;
                            fbPtr[fbOffset + ((int)_varInfo.red_offset / 8)] = pixel.R;
                            if (_varInfo.transp_length > 0)
                            {
                                fbPtr[fbOffset + ((int)_varInfo.transp_offset / 8)] = pixel.A;
                            }
                        }
                        else if (BitsPerPixel == 16)
                        {
                            // Convert to RGB565 format
                            ushort color565 = (ushort)(
                                ((pixel.R & 0xF8) << 8) |
                                ((pixel.G & 0xFC) << 3) |
                                (pixel.B >> 3)
                            );

                            fbPtr[fbOffset] = (byte)(color565 & 0xFF);
                            fbPtr[fbOffset + 1] = (byte)(color565 >> 8);
                        }
                        // Add more color depth handlers as needed
                    }
                }
            }
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

        public void Initialize(int width, int height)
        {

        }
    }
}