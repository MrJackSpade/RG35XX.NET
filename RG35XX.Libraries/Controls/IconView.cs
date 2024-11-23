using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public class IconView : Control
    {
        private int _insetBorderThickness = 2;

        private int _itemPadding = 10;

        private Color _scrollBarColor = FormColors.ScrollBar;

        private float _scrollBarWidth = 0.03f;

        private int _windowPadding = 4;

        private int firstVisibleItemIndex = 0;

        public override Color BackgroundColor { get; set; } = Color.White;

        public int InsetBorderThickness
        {
            get => _insetBorderThickness;
            set
            {
                _insetBorderThickness = value;
                Application?.MarkDirty();
            }
        }

        public override bool IsSelectable { get; set; } = true;

        public int ItemPadding
        {
            get => _itemPadding;
            set
            {
                _itemPadding = value;
                Application?.MarkDirty();
            }
        }

        public ItemSelectionMode ItemSelectionMode { get; set; } = ItemSelectionMode.Single;

        public Size ItemSize { get; set; } = new Size(0.2f, 0.25f);

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

        public override bool TabThroughChildren { get; set; } = false;

        public int WindowPadding
        {
            get => _windowPadding;
            set
            {
                _windowPadding = value;
                Application?.MarkDirty();
            }
        }

        public override Bitmap Draw(int width, int height)
        {
            int scrollBarWidth = 0;

            int itemsPerRow = (int)(1 / ItemSize.Width);
            int itemsPerColumn = (int)(1 / ItemSize.Height);
            int itemsPerPage = itemsPerRow * itemsPerColumn;

            if (_controls.Count > itemsPerPage)
            {
                scrollBarWidth = (int)(width * _scrollBarWidth);
            }

            int clientWidth = width - scrollBarWidth - (_insetBorderThickness * 2) - (_windowPadding * 2);
            int clientHeight = height - (_insetBorderThickness * 2) - (_windowPadding * 2);

            int itemWidth = (int)(clientWidth * ItemSize.Width) - (_itemPadding * 2);
            int itemHeight = (int)(clientHeight * ItemSize.Height) - (_itemPadding * 2);

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

                while (selectedIndex >= firstVisibleItemIndex + itemsPerPage)
                {
                    firstVisibleItemIndex += itemsPerRow;
                }

                int endItemIndex = Math.Min(_controls.Count, firstVisibleItemIndex + itemsPerPage);

                for (int index = firstVisibleItemIndex; index < endItemIndex; index++)
                {
                    Control item = _controls[index];

                    int itemIndexInPage = index - firstVisibleItemIndex;

                    int rowInPage = itemIndexInPage / itemsPerRow;
                    int columnInPage = itemIndexInPage % itemsPerRow;

                    int x = columnInPage * itemWidth;
                    int y = rowInPage * itemHeight;

                    Bitmap itemBitmap = item.Draw(itemWidth,
                                                  itemHeight);

                    int xOffset = x + _insetBorderThickness + _windowPadding + (_itemPadding * columnInPage * 2);
                    int yOffset = y + _insetBorderThickness + _windowPadding + (_itemPadding * rowInPage * 2);

                    if (item == SelectedItem)
                    {
                        bitmap.DrawRectangle(xOffset,
                                                   yOffset,
                                                   itemWidth + (_itemPadding * 2),
                                                   itemHeight + (_itemPadding * 2),
                                                   HighlightColor,
                                                   FillStyle.Fill);
                    }

                    bitmap.DrawTransparentBitmap(xOffset + _itemPadding, yOffset + _itemPadding, itemBitmap);
                }

                if (_insetBorderThickness > 0)
                {
                    bitmap.DrawBorder(_insetBorderThickness, FormColors.ControlDarkDark, FormColors.ControlLightLight);
                }

                if (scrollBarWidth > 0)
                {
                    int scrollBarLeft = width - scrollBarWidth;

                    bitmap.DrawRectangle(scrollBarLeft, 0, scrollBarWidth, height, _scrollBarColor, FillStyle.Fill);

                    int totalItems = _controls.Count;
                    int scrollHandleHeight = (int)(height * (float)itemsPerPage / totalItems);
                    int scrollHandleY = (int)(height * (float)firstVisibleItemIndex / totalItems);

                    Bitmap scrollHandle = new(scrollBarWidth, scrollHandleHeight, FormColors.ControlLight);
                    scrollHandle.DrawBorder(1, FormColors.ControlLightLight, FormColors.ControlDarkDark);

                    bitmap.DrawBitmap(scrollHandle, scrollBarLeft, scrollHandleY);
                }

                return bitmap;
            }
        }

        public override void OnKey(GamepadKey key)
        {
            int itemsPerRow = (int)(1 / ItemSize.Width);

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
                if (SelectedIndex < 0)
                {
                    SelectedIndex = 0;
                    Application?.MarkDirty();
                }
                else if (SelectedIndex - itemsPerRow >= 0)
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
                if (SelectedIndex < 0)
                {
                    SelectedIndex = 0;
                    Application?.MarkDirty();
                }
                else if (SelectedIndex + itemsPerRow < _controls.Count)
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