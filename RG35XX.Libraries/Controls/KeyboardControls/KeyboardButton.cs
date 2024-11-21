namespace RG35XX.Libraries.Controls.KeyboardControls
{
    internal class KeyboardButton : Button
    {
        public char Character { get; set; }

        public KeyboardButton(char character)
        {
            Character = character;
            Text = character.ToString();
        }
    }
}