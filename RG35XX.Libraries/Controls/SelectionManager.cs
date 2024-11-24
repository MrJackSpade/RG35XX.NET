namespace RG35XX.Libraries.Controls
{
    public class SelectionManager
    {
        public Control? SelectedControl { get; private set; } = null;

        public void Select(Control control)
        {
            if (control.IsSelectable)
            {
                if (SelectedControl != null)
                {
                    SelectedControl.IsSelected = false;
                }

                SelectedControl = control;
                SelectedControl.IsSelected = true;
            }
        }

        public void SelectNext(Page page)
        {
            List<Control> controlTree = this.GetSelectableChildren(page).ToList();

            if (controlTree.Count == 0)
            {
                return;
            }

            if (SelectedControl == null)
            {
                this.Select(controlTree[0]);
            }
            else
            {
                int index = controlTree.IndexOf(SelectedControl);

                if (index == controlTree.Count - 1)
                {
                    this.Select(controlTree[0]);
                }
                else
                {
                    int newIndex = index + 1;

                    this.Select(controlTree[newIndex]);
                }
            }
        }

        internal void SelectPrevious(Page page)
        {
            List<Control> controlTree = this.GetSelectableChildren(page).ToList();

            if (controlTree.Count == 0)
            {
                return;
            }

            if (SelectedControl == null)
            {
                this.Select(controlTree[^1]);
            }
            else
            {
                int index = controlTree.IndexOf(SelectedControl);

                if (index == 0)
                {
                    this.Select(controlTree[^1]);
                }
                else
                {
                    int newIndex = index - 1;

                    this.Select(controlTree[newIndex]);
                }
            }
        }

        internal void Unselect(Control control)
        {
            if (SelectedControl == control)
            {
                SelectedControl.IsSelected = false;
                SelectedControl = null;
            }
        }

        private IEnumerable<Control> GetSelectableChildren(Control control)
        {
            if (control.TabThroughChildren)
            {
                foreach (Control child in control.Controls)
                {
                    if (child.IsSelectable)
                    {
                        yield return child;
                    }

                    foreach (Control c_child in this.GetSelectableChildren(child))
                    {
                        yield return c_child;
                    }
                }
            }
        }
    }
}