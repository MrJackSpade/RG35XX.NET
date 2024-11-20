﻿using RG35XX.Core.Drawing;
using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries.Controls
{
    public class Control : IDisposable
    {
        protected readonly List<Control> _controls = [];

        protected readonly object _lock = new();

        private bool _isSelected;

        private Control? _parent;

        public Color BackgroundColor { get; set; } = FormColors.BackgroundColor;

        public virtual Bounds Bounds { get; set; } = new(0, 0, 1, 1);

        public IReadOnlyList<Control> Controls => _controls;

        public Color HighlightColor { get; set; } = FormColors.HighlightColor;

        public virtual bool IsSelectable { get; set; } = false;

        public virtual bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                Renderer?.MarkDirty();
            }
        }

        public Control? Parent
        {
            get => _parent;
            set
            {
                _parent?.RemoveControl(this);
                _parent = value;
            }
        }

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

        internal IRenderer? Renderer { get; set; }

        internal SelectionManager? SelectionManager { get; set; }

        public bool AddControl(Control control)
        {
            lock (_lock)
            {
                if (!_controls.Contains(control))
                {
                    control.SelectionManager = SelectionManager;
                    control.Parent = this;
                    _controls.Add(control);
                    control.SetRenderer(Renderer);
                    return true;
                }

                return false;
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

                    bitmap.DrawBitmap(controlBitmap, controlX, controlY);
                }

                return bitmap;
            }
        }

        public virtual void OnKey(GamepadKey key)
        {
        }

        public bool RemoveControl(Control control)
        {
            lock (_lock)
            {
                if (_controls.Remove(control))
                {
                    control.Parent = null;
                    return true;
                }

                return false;
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

        internal void SetRenderer(IRenderer? renderer)
        {
            Renderer = renderer;

            foreach (Control control in _controls)
            {
                control.SetRenderer(renderer);
            }

            Renderer?.MarkDirty();
        }
    }
}