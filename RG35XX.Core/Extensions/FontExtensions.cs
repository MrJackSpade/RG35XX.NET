using RG35XX.Core.Drawing;
using RG35XX.Core.Interfaces;

namespace RG35XX.Core.Extensions
{
    public static class FontExtensions
    {
        public static Bitmap GetCharacterMap(this IFont font, char index, Color foreground, Color background)
        {
            if(!font.Data.TryGetValue(index, out byte[][]? fullData))
            {
                return null;
            } 

            if(font.Width > 8)
            {
                throw new NotImplementedException("Font width > 8 is not implemented");
            }

            Bitmap bitmap = new(font.Width, font.Height);

            for (int y = 0; y < font.Height; y++)
            {
                for (int x = 0; x < font.Width; x++)
                {
                    //First byte is the first 8 bits, if theres multiple bytes
                    //we need to write new code to handle that
                    byte pixel = fullData[y][0];

                    if ((pixel & (1 << x)) != 0)
                    {
                        bitmap.SetPixel(x, y, foreground);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, background);
                    }
                }
            }

            return bitmap;
        }
    }
}