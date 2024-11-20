using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Desktop
{
    public class KeyboardInput : IGamePadReader
    {
        public void ClearBuffer()
        {
            KeyBus.ClearBuffer();
        }

        public void Initialize(string devicePath = "/dev/input/js0")
        {
        }

        public GamepadKey ReadInput()
        {
            return KeyBus.ReadInput();
        }
    }
}