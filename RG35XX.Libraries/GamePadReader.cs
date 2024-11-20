using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries
{
    public class GamePadReader : IGamePadReader
    {
        private readonly IGamePadReader _gamePadReader;

        public GamePadReader()
        {
#if DEBUG
            _gamePadReader = new RG35XX.Desktop.KeyboardInput();

#else
            _gamePadReader = new RG35XX.Handheld.LinuxGamePadReader();
#endif
        }

        public void ClearBuffer()
        {
            _gamePadReader.ClearBuffer();
        }

        public void Initialize(string devicePath = "/dev/input/js0")
        {
            _gamePadReader.Initialize(devicePath);
        }

        public GamepadKey ReadInput()
        {
            return _gamePadReader.ReadInput();
        }
    }
}