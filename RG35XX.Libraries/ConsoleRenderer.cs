﻿using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Interfaces;
using Bitmap = RG35XX.Core.Drawing.Bitmap;

namespace RG35XX.Libraries
{
    public class ConsoleRenderer
    {
        private readonly IFont _font;

        private CharData[,] _buffer = new CharData[0, 0];

        private int _cursorX;

        private int _cursorY;

        public bool AutoFlush { get; set; } = true;

        public FrameBuffer? FrameBuffer { get; }

        public int Height { get; private set; }

        public bool Initialized { get; private set; }

        public int Width { get; private set; }

        public ConsoleRenderer(IFont font)
        {
            _font = font;
            FrameBuffer = new FrameBuffer();
        }

        public ConsoleRenderer(IFont font, FrameBuffer? frameBuffer)
        {
            _font = font;
            FrameBuffer = frameBuffer;
        }

        public void Clear(bool flush = true)
        {
            this.EnsureInitialized();

            _buffer = new CharData[Height, Width];
            _cursorX = 0;
            _cursorY = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _buffer[y, x] = new CharData() { Char = '\0', BackgroundColor = Color.Black, ForegroundColor = Color.White };
                }
            }

            if (flush)
            {
                this.Flush();
            }
        }

        public void ClearLine(bool flush = true)
        {
            this.EnsureInitialized();

            _cursorX = 0;

            for (int x = 0; x < Width; x++)
            {
                _buffer[_cursorY, x] = new CharData() { Char = ' ', BackgroundColor = Color.Black, ForegroundColor = Color.White };
            }

            if (flush)
            {
                this.Flush();
            }
        }

        public void Flush()
        {
            this.EnsureInitialized();

            Bitmap render = this.Render();

            FrameBuffer?.Draw(render, 0, 0);
        }

        public void Initialize(int width, int height)
        {
            Initialized = true;
            FrameBuffer?.Initialize(width, height);
            Width = width / _font.Width;
            Height = height / _font.Height;
            this.Clear(false);
        }

        public Bitmap Render()
        {
            this.EnsureInitialized();

            Bitmap toDraw = new(Width * _font.Width, Height * _font.Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    CharData charData = _buffer[y, x];

                    Bitmap? charMap = null;

                    if (charData.Char != '\0')
                    {
                        charMap = _font.GetCharacterMap(charData.Char, charData.ForegroundColor, charData.BackgroundColor) ??
                                  _font.GetCharacterMap('?', charData.ForegroundColor, charData.BackgroundColor);
                    }

                    if (charMap != null)
                    {
                        toDraw.DrawBitmap(charMap, x * _font.Width, y * _font.Height);
                    }
                    else
                    {
                        Bitmap blankChar = new(_font.Width, _font.Height, charData.BackgroundColor);
                        toDraw.DrawBitmap(blankChar, x * _font.Width, y * _font.Height);
                    }
                }
            }

            return toDraw;
        }

        public void SetCursorPosition(int x, int y)
        {
            this.EnsureInitialized();

            _cursorX = x;
            _cursorY = y;
        }

        public void Write(char c)
        {
            this.EnsureInitialized();

            this.Write(c, Color.White, Color.Black);
        }

        public void Write(char c, Color foreground)
        {
            this.EnsureInitialized();

            this.Write(c, foreground, Color.Black);
        }

        public void Write(char c, Color foreground, Color background)
        {
            this.EnsureInitialized();

            if (c == '\r')
            {
                return;
            }

            if (_cursorX >= Width)
            {
                _cursorX = 0;
                _cursorY++;
            }

            if (_cursorY >= Height)
            {
                this.ScrollBufferUp();
                _cursorY = Height - 1;
            }

            if (c == '\n')
            {
                _cursorX = 0;
                _cursorY++;
            }
            else
            {
                _buffer[_cursorY, _cursorX] = new CharData()
                {
                    Char = c,
                    BackgroundColor = background,
                    ForegroundColor = foreground
                };

                _cursorX++;
            }

            if (AutoFlush)
            {
                this.Flush();
            }
        }

        public void Write(string text)
        {
            this.EnsureInitialized();

            this.Write(text, Color.White, Color.Black);
        }

        public void Write(string text, Color foreground)
        {
            this.EnsureInitialized();

            this.Write(text, foreground, Color.Black);
        }

        public void Write(string text, Color foreground, Color background)
        {
            this.EnsureInitialized();

            foreach (char c in text)
            {
                if (c == '\r')
                {
                    continue;
                }

                if (_cursorX >= Width)
                {
                    _cursorX = 0;
                    _cursorY++;
                }

                if (_cursorY >= Height)
                {
                    this.ScrollBufferUp();
                    _cursorY = Height - 1;
                }

                if (c == '\n')
                {
                    _cursorX = 0;
                    _cursorY++;
                }
                else
                {
                    _buffer[_cursorY, _cursorX] = new CharData()
                    {
                        Char = c,
                        BackgroundColor = background,
                        ForegroundColor = foreground
                    };

                    _cursorX++;
                }
            }

            if (AutoFlush)
            {
                this.Flush();
            }
        }

        public void WriteLine(string text = "")
        {
            this.EnsureInitialized();

            this.WriteLine(text, Color.White, Color.Black);
        }

        public void WriteLine(string? text, Color foreground)
        {
            this.EnsureInitialized();

            this.Write(text + '\n', foreground, Color.Black);
        }

        public void WriteLine(string? text, Color foreground, Color background)
        {
            this.EnsureInitialized();

            this.Write(text + '\n', foreground, background);
        }

        private void EnsureInitialized()
        {
            if (!Initialized)
            {
                throw new System.InvalidOperationException("ConsoleRenderer has not been initialized.");
            }
        }

        private void ScrollBufferUp()
        {
            this.EnsureInitialized();

            // Move each line up by one
            for (int y = 1; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _buffer[y - 1, x] = _buffer[y, x];
                }
            }
            // Clear the last line
            for (int x = 0; x < Width; x++)
            {
                _buffer[Height - 1, x] = new CharData() { Char = ' ', BackgroundColor = Color.Black, ForegroundColor = Color.White };
            }
        }
    }
}