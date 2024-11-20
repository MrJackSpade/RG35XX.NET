﻿using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public class IconView : Control
    {
        private int _borderThickness = 2;

        private Color _scrollBarColor = FormColors.ScrollBar;

        private float _scrollBarWidth = 0.05f;

        private int firstVisibleItemIndex = 0;

        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                Application?.MarkDirty();
            }
        }

        public override bool IsSelectable { get; set; } = true;

        public float ItemHeight { get; set; } = 0.25f;

        public ItemSelectionMode ItemSelectionMode { get; set; } = ItemSelectionMode.Single;

        public float ItemWidth { get; set; } = 0.25f;

        public Color ScrollBarColor
        {
            get => _scrollBarColor;
            set
            {
                _scrollBarColor = value;
                Application?.MarkDirty();
            }
        }

        public float ScrollBarWidth
        {
            get => _scrollBarWidth;
            set
            {
                _scrollBarWidth = value;
                Application?.MarkDirty();
            }
        }

        public int SelectedIndex { get; set; } = -1;

        public Control? SelectedItem => SelectedIndex >= 0 && SelectedIndex < _controls.Count ? _controls[SelectedIndex] : null;

        public override Bitmap Draw(int width, int height)
        {
            int scrollBarWidth = 0;

            if (_controls.Count > 0)
            {
                scrollBarWidth = (int)(width * _scrollBarWidth);
            }

            int clientWidth = width - scrollBarWidth;

            int itemWidth = (int)(clientWidth * ItemWidth);
            int itemHeight = (int)(height * ItemHeight);

            int itemsPerRow = (int)(1 / ItemWidth);
            int itemsPerColumn = (int)(1 / ItemHeight);
            int itemsPerPage = itemsPerRow * itemsPerColumn;

            lock (_lock)
            {
                Bitmap bitmap = new(width, height, BackgroundColor);

                if (IsSelected && SelectedIndex == -1)
                {
                    bitmap.DrawBorder(2, HighlightColor);
                }

                int selectedIndex = _controls.IndexOf(SelectedItem);

                // Adjust firstVisibleItemIndex to implement sliding window
                if (selectedIndex < 0)
                {
                    firstVisibleItemIndex = 0;
                }
                else if (selectedIndex < firstVisibleItemIndex)
                {
                    firstVisibleItemIndex = selectedIndex;
                }
                else if (selectedIndex >= firstVisibleItemIndex + itemsPerPage)
                {
                    firstVisibleItemIndex = selectedIndex - itemsPerPage + 1;
                }

                int x = 0;
                int y = 0;
                int itemsInCurrentRow = 0;

                for (int i = firstVisibleItemIndex; i < _controls.Count && i < firstVisibleItemIndex + itemsPerPage; i++)
                {
                    Control item = _controls[i];

                    Bitmap itemBitmap = item.Draw(itemWidth - (_borderThickness * 2), itemHeight - (_borderThickness * 2));

                    if (item == SelectedItem)
                    {
                        itemBitmap.DrawBorder(_borderThickness, HighlightColor);
                    }

                    bitmap.DrawBitmap(itemBitmap, x + _borderThickness, y + _borderThickness);

                    x += itemWidth;
                    itemsInCurrentRow++;

                    if (itemsInCurrentRow >= itemsPerRow)
                    {
                        x = 0;
                        y += itemHeight;
                        itemsInCurrentRow = 0;
                    }
                }

                if (scrollBarWidth > 0)
                {
                    bitmap.DrawRectangle(clientWidth, 0, scrollBarWidth, height, _scrollBarColor, FillStyle.Fill);

                    int totalItems = _controls.Count;
                    int scrollHandleHeight = (int)(height * (float)itemsPerPage / totalItems);
                    int scrollHandleY = (int)(height * (float)firstVisibleItemIndex / totalItems);

                    Bitmap scrollHandle = new(scrollBarWidth, scrollHandleHeight, BackgroundColor);
                    scrollHandle.DrawBorder(1, FormColors.ControlLightLight, FormColors.ControlDarkDark);

                    bitmap.DrawBitmap(scrollHandle, clientWidth, scrollHandleY);
                }

                return bitmap;
            }
        }

        public override void OnKey(GamepadKey key)
        {
            int itemsPerRow = (int)(1 / ItemWidth);

            if (key == GamepadKey.LEFT)
            {
                if (SelectedIndex > 0)
                {
                    SelectedIndex--;
                    Application?.MarkDirty();
                }
            }
            else if (key == GamepadKey.RIGHT)
            {
                if (SelectedIndex < _controls.Count - 1)
                {
                    SelectedIndex++;
                    Application?.MarkDirty();
                }
            }
            else if (key == GamepadKey.UP)
            {
                if (SelectedIndex - itemsPerRow >= 0)
                {
                    SelectedIndex -= itemsPerRow;
                    Application?.MarkDirty();
                }
                else if (SelectedIndex >= 0)
                {
                    SelectedIndex = 0;
                    Application?.MarkDirty();
                }
            }
            else if (key == GamepadKey.DOWN)
            {
                if (SelectedIndex + itemsPerRow < _controls.Count)
                {
                    SelectedIndex += itemsPerRow;
                    Application?.MarkDirty();
                }
                else if (SelectedIndex < _controls.Count)
                {
                    SelectedIndex = _controls.Count - 1;
                    Application?.MarkDirty();
                }
            }
            else if (key == GamepadKey.A_DOWN)
            {
                if (ItemSelectionMode == ItemSelectionMode.Single)
                {
                    if (SelectedItem is not null)
                    {
                        SelectedItem.IsSelected = !SelectedItem.IsSelected;
                        Application?.MarkDirty();
                    }
                }
                else if (ItemSelectionMode == ItemSelectionMode.Multiple)
                {
                    throw new NotImplementedException();
                }
            }

            SelectedItem?.OnKey(key);
            base.OnKey(key);
        }
    }
}