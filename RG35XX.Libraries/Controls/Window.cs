using RG35XX.Core.Drawing;
using RG35XX.Core.Extensions;
using RG35XX.Core.Fonts;
using RG35XX.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG35XX.Libraries.Controls
{
    public class Window : Control
    {
        private IFont _font = ConsoleFont.MS_Sans_Serif_Bold;

        private float _fontSize = 0.6f;

        private string _title = string.Empty;

        private float _titleHeight = 0.05f;

        public IFont Font
        {
            get => _font;
            set
            {
                _font = value;
                Application?.MarkDirty();
            }
        }

        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                Application?.MarkDirty();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Application?.MarkDirty();
            }
        }

        public float TitleHeight
        {
            get => _titleHeight;
            set
            {
                _titleHeight = value;
                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                bitmap.DrawGradientRectangle(0, 0, width, (int)(height * _titleHeight), FormColors.TitleBarStart, FormColors.TitleBarEnd, GradientDirection.LeftToRight);

                if (!string.IsNullOrEmpty(_title))
                {
                    Bitmap title = Font.Render(_title, Color.White, Color.Transparent, _fontSize);
                    bitmap.DrawTransparentBitmap(0, 0, title);
                }

                int clientHeight = height - (int)(height * _titleHeight);

                Bitmap client = base.Draw(width, clientHeight);

                bitmap.DrawTransparentBitmap(0, (int)(height * _titleHeight), client);

                return bitmap;
            }
        }
    }
}