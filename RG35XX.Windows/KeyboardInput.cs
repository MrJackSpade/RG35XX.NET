using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Windows
{
    public class KeyboardInput : IGamePadReader
    {
        public void Initialize(string devicePath = "/dev/input/js0")
        {
        }

        public GamepadKey ReadInput()
        {
            ConsoleKey consoleKey = Console.ReadKey().Key;

            do
            {
                switch (consoleKey)
                {
                    case ConsoleKey.LeftArrow:
                        return GamepadKey.LEFT;

                    case ConsoleKey.RightArrow:
                        return GamepadKey.RIGHT;

                    case ConsoleKey.UpArrow:
                        return GamepadKey.UP;

                    case ConsoleKey.DownArrow:
                        return GamepadKey.DOWN;

                    case ConsoleKey.Enter:
                        return GamepadKey.START_DOWN;

                    case ConsoleKey.Escape:
                        return GamepadKey.SELECT_DOWN;

                    case ConsoleKey.A:
                        return GamepadKey.A_DOWN;

                    case ConsoleKey.B:
                        return GamepadKey.B_DOWN;

                    case ConsoleKey.Y:
                        return GamepadKey.Y_DOWN;

                    case ConsoleKey.X:
                        return GamepadKey.X_DOWN;

                    case ConsoleKey.OemPeriod:
                        return GamepadKey.R1_DOWN;

                    case ConsoleKey.OemComma:
                        return GamepadKey.L1_DOWN;
                }
            } while (true);
        }
    }
}