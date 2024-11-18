using RG35XX.Core.GamePads;

namespace RG35XX.Core.Interfaces
{
    public interface IGamePadReader
    {
        void ClearBuffer();

        void Initialize(string devicePath = "/dev/input/js0");

        GamepadKey ReadInput();
    }
}