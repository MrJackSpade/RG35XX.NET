using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public enum ItemSelectionMode
    {
        None,

        Single,

        Multiple
    }

    public class ListBox : Control
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

        public override Bitmap Draw(int width, int height)
        {
            int itemHeight = (int)(height * ItemHeight);

            int itemsPerPage = (int)(1 / ItemHeight);

            int scrollBarWidth = 0;

            if (_controls.Count > itemsPerPage)
            {
                scrollBarWidth = (int)(width * _scrollBarWidth);
            }

            int clientWidth = width - scrollBarWidth;

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

                int y = 0;

                for (int i = firstVisibleItemIndex; i < _controls.Count && i < firstVisibleItemIndex + itemsPerPage; i++)
                {
                    Control item = _controls[i];

                    Bitmap itemBitmap = item.Draw(clientWidth - (_borderThickness * 2), itemHeight - (_borderThickness * 2));

                    if (item == SelectedItem)
                    {
                        itemBitmap.DrawBorder(_borderThickness, HighlightColor);
                    }

                    bitmap.DrawBitmap(itemBitmap, _borderThickness, y + _borderThickness);

                    y += itemHeight;
                }

                if (scrollBarWidth > 0)
                {
                    bitmap.DrawRectangle(clientWidth, 0, scrollBarWidth, height, _scrollBarColor, FillStyle.Fill);

                    int scrollHandleHeight = (int)(height * (float)itemsPerPage / _controls.Count);

                    Bitmap scrollHandle = new(scrollBarWidth, scrollHandleHeight, BackgroundColor);

                    int scrollHandleY = (int)(height * (float)firstVisibleItemIndex / _controls.Count);

                    scrollHandle.DrawBorder(1, FormColors.ControlLightLight, FormColors.ControlDarkDark);

                    bitmap.DrawBitmap(scrollHandle, clientWidth, scrollHandleY);
                }

                return bitmap;
            }
        }

        public override void OnKey(GamepadKey key)
        {
            if (key == GamepadKey.UP)
            {
                SelectedIndex = Math.Max(0, SelectedIndex - 1);
                Application?.MarkDirty();
            }
            else if (key == GamepadKey.DOWN)
            {
                SelectedIndex = Math.Min(_controls.Count - 1, SelectedIndex + 1);
                Application?.MarkDirty();
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