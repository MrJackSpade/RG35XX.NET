using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RG35XX.Core.Drawing
{
    [DebuggerDisplay("{DisplayName}")]
    [StructLayout(LayoutKind.Explicit)]
    public struct Color(byte r, byte g, byte b, byte a = 255)
    {
        [FieldOffset(0)]
        public byte B = b;

        [FieldOffset(1)]
        public byte G = g;

        [FieldOffset(2)]
        public byte R = r;

        [FieldOffset(3)]
        public byte A = a;

        public static Color Black => new(0, 0, 0);

        public static Color Blue => new(0, 0, 255);

        public static Color Cyan => new(0, 255, 255);

        public static Color Green => new(0, 255, 0);

        public static Color Magenta => new(255, 0, 255);

        public static Color Red => new(255, 0, 0);

        public static Color White => new(255, 255, 255);

        public static Color Yellow => new(255, 255, 0);

        public readonly string DisplayName
        {
            get
            {
                if (this == Color.Black)
                {
                    return "Black";
                }

                if (this == Color.Blue)
                {
                    return "Blue";
                }

                if (this == Color.Cyan)
                {
                    return "Cyan";
                }

                if (this == Color.Green)
                {
                    return "Green";
                }

                if (this == Color.Magenta)
                {
                    return "Magenta";
                }

                if (this == Color.Red)
                {
                    return "Red";
                }

                if (this == Color.White)
                {
                    return "White";
                }

                if (this == Color.Yellow)
                {
                    return "Yellow";
                }

                return $"R:{R} G:{G} B:{B} A:{A}";
            }
        }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new(r, g, b, a);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public readonly Color BlendWith(Color background)
        {
            if (A == 255)
            {
                return this;
            }

            if (A == 0)
            {
                return background;
            }

            float alpha = A / 255f;
            return new Color(
                (byte)((R * alpha) + (background.R * (1 - alpha))),
                (byte)((G * alpha) + (background.G * (1 - alpha))),
                (byte)((B * alpha) + (background.B * (1 - alpha))),
                255
            );
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is Color color)
            {
                return color.R == R && color.G == G && color.B == B && color.A == A;
            }

            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }
    }
}