using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;
using RG35XX.Desktop;

namespace RG35XX.Windows
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