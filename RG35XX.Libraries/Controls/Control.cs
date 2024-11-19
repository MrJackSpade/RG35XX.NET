using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;

namespace RG35XX.Libraries.Controls
{
    public class Control : IDisposable
    {
        protected readonly object _lock = new();

        private readonly List<Control> _controls = [];

        public Color BackgroundColor { get; set; } = FormColors.BackgroundColor;

        public virtual Bounds Bounds { get; set; } = new(0, 0, 1, 1);

        public IReadOnlyList<Control> Controls => _controls;

        public Color HighlightColor { get; set; } = FormColors.HighlightColor;

        public virtual bool IsSelectable { get; set; } = false;

        public virtual bool IsSelected { get; internal set; } = false;

        public Control? Parent { get; protected set; }

        public IEnumerable<Control> RecursiveChildren
        {
            get
            {
                foreach (Control control in _controls)
                {
                    yield return control;

                    foreach (Control child in control.RecursiveChildren)
                    {
                        yield return child;
                    }
                }
            }
        }

        public virtual void OnKey(GamepadKey key)
        {
        }

        internal SelectionManager? SelectionManager { get; set; }

        public void AddControl(Control control)
        {
            lock (_lock)
            {
                control.SelectionManager = SelectionManager;
                control.Parent = this;
                _controls.Add(control);
            }
        }

        public virtual void Dispose()
        {
        }

        public virtual Bitmap Draw(int width, int height)
        {
            lock (_lock)
            {
                ///This is super inefficient, and should be optimized
                Bitmap bitmap = new(width, height, BackgroundColor);

                foreach (Control control in _controls)
                {
                    int controlX = (int)(control.Bounds.X * width);
                    int controlY = (int)(control.Bounds.Y * height);
                    int controlWidth = (int)(control.Bounds.Width * width);
                    int controlHeight = (int)(control.Bounds.Height * height);

                    Bitmap controlBitmap = control.Draw(controlWidth, controlHeight);

                    bitmap.Draw(controlBitmap, controlX, controlY);
                }

                return bitmap;
            }
        }

        public void RemoveControl(Control control)
        {
            lock (_lock)
            {
                _controls.Remove(control);
            }
        }

        public void Select()
        {
            SelectionManager?.Select(this);
        }

        public void Unselect()
        {
            SelectionManager?.Unselect(this);
        }
    }
}