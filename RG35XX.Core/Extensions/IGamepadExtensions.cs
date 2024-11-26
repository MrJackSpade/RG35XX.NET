using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Core.Extensions
{
    public static class IGamepadExtensions
    {
        public static GamepadKey WaitForInput(this IGamePadReader reader)
        {
            GamepadKey result = GamepadKey.None;

            while (result == GamepadKey.None)
            {
                result = reader.ReadInput();
            }

            return result;
        }

        public static GamepadKey WaitForInput(this IGamePadReader reader, GamepadKey[] keys, int delayMs = 100)
        {
            GamepadKey result = GamepadKey.None;

            while (!keys.Contains(result))
            {
                Thread.Sleep(delayMs);
                result = reader.ReadInput();
            }

            return result;
        }
    }
}