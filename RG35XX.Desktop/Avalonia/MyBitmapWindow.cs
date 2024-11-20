using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using RG35XX.Core.Drawing;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using CoreBitmap = RG35XX.Core.Drawing.Bitmap;

namespace RG35XX.Desktop.Avalonia
{
    public class MyBitmapWindow : Window
    {
        private readonly Image _imageControl;

        public MyBitmapWindow(int width, int height)
        {
            // Set window properties
            Width = width;
            Height = height;

            // Create an Image control
            _imageControl = new Image();

            // Set the window's content to the Image control
            Content = _imageControl;

            KeyDown += this.OnKeyDown;

            KeyUp += this.OnKeyUp;

            Closed += (sender, e) => System.Environment.Exit(0);
        }

        public void DisplayCustomBitmap(CoreBitmap customBitmap)
        {
            // Convert customBitmap to Avalonia Bitmap
            Bitmap avaloniaBitmap = this.ConvertToAvaloniaBitmap(customBitmap);

            // Set the Image control's source
            _imageControl.Source = avaloniaBitmap;
        }

        private Bitmap ConvertToAvaloniaBitmap(CoreBitmap customBitmap)
        {
            // Create a WriteableBitmap with the same dimensions
            PixelSize pixelSize = new(customBitmap.Width, customBitmap.Height);
            Vector dpi = new(96, 96); // Use appropriate DPI if necessary
            WriteableBitmap writeableBitmap = new(pixelSize, dpi, PixelFormat.Bgra8888);

            using (ILockedFramebuffer fb = writeableBitmap.Lock())
            {
                unsafe
                {
                    // Get pointer to the writeableBitmap's pixel buffer
                    byte* destPtr = (byte*)fb.Address;

                    // Assuming that each pixel is 4 bytes (32 bits)
                    int bytesPerPixel = 4;
                    int totalPixels = customBitmap.Width * customBitmap.Height;
                    int totalBytes = totalPixels * bytesPerPixel;

                    // Copy pixels from customBitmap.Pixels to writeableBitmap
                    fixed (Color* sourcePtr = customBitmap.Pixels)
                    {
                        // Since the memory layouts match, we can copy the pixel data directly
                        Buffer.MemoryCopy(sourcePtr, destPtr, totalBytes, totalBytes);
                    }
                }
            }

            return writeableBitmap;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            KeyBus.OnKeyDown(e);
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            KeyBus.OnKeyUp(e);
        }
    }
}