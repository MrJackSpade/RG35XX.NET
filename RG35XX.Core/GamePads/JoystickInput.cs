namespace RG35XX.Core.GamePads
{
    public struct JoystickInput
    {
        public byte Type;    // Event type (1 for button, 2 for axis)
        public byte Number;  // Button/axis number
        public ushort Value;  // Value (-32767 to 32767 for axis, 0 or 1 for button)
    }
}
