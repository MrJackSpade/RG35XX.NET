namespace RG35XX.Libraries.Controls.KeyboardControls
{
    internal class KeyboardRow
    {
        public Dictionary<char, KeyboardButton> Buttons { get; set; } = [];

        public float Offset { get; set; }

        public KeyboardRow()
        {
        }

        public KeyboardRow(string buttons)
        {
            foreach (char c in buttons)
            {
                Buttons.Add(c, new KeyboardButton(c)
                {
                    FontSize = 0.5f
                });
            }

            this.Align();
        }

        public void Align()
        {
            List<KeyboardButton> buttons = Buttons.Values.ToList();

            float x = Offset;
            float width = (1 - Offset) / buttons.Count;

            foreach (KeyboardButton b in buttons)
            {
                Bounds bounds = b.Bounds;
                b.Bounds = new(x, bounds.Y, width, bounds.Height);
                x += width;
            }
        }
    }
}