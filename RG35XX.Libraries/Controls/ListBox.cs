using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public class ListBox : Control
    {
        private int firstVisibleItemIndex = 0;

        public override bool IsSelectable { get; set; } = true;

        public float ItemHeight { get; set; } = 0.25f;

        public int SelectedIndex { get; set; } = -1;

        public Control? SelectedItem => SelectedIndex >= 0 && SelectedIndex < _controls.Count ? _controls[SelectedIndex] : null;

        public override Bitmap Draw(int width, int height)
        {
            int itemHeight = (int)(height * ItemHeight);
            int itemsPerPage = (int)(1 / ItemHeight);

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

                    Bitmap itemBitmap = item.Draw(width - 4, itemHeight - 4);

                    if (item == SelectedItem)
                    {
                        itemBitmap.DrawBorder(2, HighlightColor);
                    }

                    bitmap.DrawBitmap(itemBitmap, 2, y + 2);

                    y += itemHeight;
                }

                return bitmap;
            }
        }

        public override void OnKey(GamepadKey key)
        {
            if (key == GamepadKey.UP)
            {
                SelectedIndex = Math.Max(0, SelectedIndex - 1);
                Renderer?.MarkDirty();
            }
            else if (key == GamepadKey.DOWN)
            {
                SelectedIndex = Math.Min(_controls.Count - 1, SelectedIndex + 1);
                Renderer?.MarkDirty();
            }
            else if (key == GamepadKey.A_DOWN)
            {
                if (SelectedItem is not null)
                {
                    SelectedItem.IsSelected = !SelectedItem.IsSelected;
                    Renderer?.MarkDirty();
                }
            }

            base.OnKey(key);
        }
    }
}