using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Interfaces;
using Bitmap = RG35XX.Core.Drawing.Bitmap;

namespace RG35XX.Libraries
{
    public class ConsoleRenderer(IFont font)
    {
        private readonly IFont _font = font;

        private readonly FrameBuffer _frameBuffer = new();

        private CharData[,] _buffer = new CharData[0, 0];

        private int _cursorX;

        private int _cursorY;

        private int _height;

        private int _width;

        public void Initialize(int width, int height)
        {
            _frameBuffer.Initialize(width, height);
            _width = width / _font.Width;
            _height = height / _font.Height;
            _buffer = new CharData[_height, _width];
            _cursorX = 0;
            _cursorY = 0;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _buffer[y, x] = new CharData() { Char = 0, BackgroundColor = Color.Black, ForegroundColor = Color.White };
                }
            }
        }

        public void Write(string text)
        {
            this.Write(text, Color.White, Color.Black);
        }

        public void Write(string text, Color foreground, Color background)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(text);

            foreach (byte b in text)
            {
                if (b == '\n')
                {
                    _cursorX = 0;
                    _cursorY++;

                    if (_cursorY >= _height)
                    {
                        this.ScrollBufferUp();
                        _cursorY = _height - 1;
                    }
                }
                else
                {
                    _buffer[_cursorY, _cursorX] = new CharData()
                    {
                        Char = b,
                        BackgroundColor = background,
                        ForegroundColor = foreground
                    };

                    _cursorX++;

                    if (_cursorX >= _width)
                    {
                        _cursorX = 0;
                        _cursorY++;
                        if (_cursorY >= _height)
                        {
                            this.ScrollBufferUp();
                            _cursorY = _height - 1;
                        }
                    }
                }
            }

            this.Dump();
        }

        public void WriteLine(string text)
        {
            this.WriteLine(text, Color.White, Color.Black);
        }

        public void WriteLine(string text, Color foreground, Color background)
        {
            this.Write(text + '\n', foreground, background);
        }

        private void Dump()
        {
            Bitmap toDraw = new(_width * _font.Width, _height * _font.Height);

            Bitmap blankChar = new(_font.Width, _font.Height, Color.Black);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    CharData charData = _buffer[y, x];

                    Bitmap? charMap = _font.GetCharacterMap(charData.Char, charData.ForegroundColor, charData.BackgroundColor);

                    if (charMap != null)
                    {
                        toDraw.Draw(charMap, x * _font.Width, y * _font.Height);
                    }
                    else
                    {
                        toDraw.Draw(blankChar, x * _font.Width, y * _font.Height);
                    }
                }
            }

            _frameBuffer.Draw(toDraw, 0, 0);
        }

        private void ScrollBufferUp()
        {
            // Move each line up by one
            for (int y = 1; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _buffer[y - 1, x] = _buffer[y, x];
                }
            }
            // Clear the last line
            for (int x = 0; x < _width; x++)
            {
                _buffer[_height - 1, x] = new CharData() { Char = (byte)' ', BackgroundColor = Color.Black, ForegroundColor = Color.White };
            }
        }
    }
}