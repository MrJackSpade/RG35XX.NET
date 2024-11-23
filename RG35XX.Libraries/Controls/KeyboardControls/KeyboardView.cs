namespace RG35XX.Libraries.Controls.KeyboardControls
{
    internal class KeyboardView
    {
        private readonly List<KeyboardRow> _rows = [];

        public IEnumerable<KeyboardButton> Buttons => _rows.SelectMany(r => r.Buttons.Values);

        public IReadOnlyList<KeyboardRow> Rows => _rows;

        public void AddRow(KeyboardRow row)
        {
            _rows.Add(row);

            this.Align();
        }

        public void Align()
        {
            float height = 1 / (float)_rows.Count;

            for (int i = 0; i < _rows.Count; i++)
            {
                KeyboardRow row = _rows[i];

                row.Align();

                float y = i * height;

                foreach (Button b in row.Buttons.Values)
                {
                    Bounds bounds = b.Bounds;
                    b.Bounds = new(bounds.X, y, bounds.Width, height);
                }
            }
        }

        public Button GetKey(char key)
        {
            foreach (KeyboardRow row in _rows)
            {
                if (row.Buttons.TryGetValue(key, out KeyboardButton button))
                {
                    return button;
                }
            }

            throw new KeyNotFoundException($"Key '{key}' not found in keyboard.");
        }

        public KeyboardRow GetRow(char key)
        {
            return _rows.First(r => r.Buttons.ContainsKey(key));
        }

        public void SetSelectable(bool value)
        {
            foreach (KeyboardButton button in Buttons)
            {
                button.IsSelectable = value;
            }
        }
    }
}