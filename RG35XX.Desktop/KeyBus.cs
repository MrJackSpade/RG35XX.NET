using Avalonia.Input;
using RG35XX.Core.GamePads;
using System.Collections.Concurrent;

namespace RG35XX.Desktop
{
    public static class KeyBus
    {
        // Replace ConcurrentQueue with BlockingCollection
        private static readonly BlockingCollection<GamepadKey> _keys = new();

        public static void ClearBuffer()
        {
            while (_keys.Count > 0)
            {
                _keys.Take();
            }
        }

        public static void OnKeyDown(KeyEventArgs e)
        {
            GamepadKey key = GamepadKey.None;

            switch (e.Key)
            {
                case Key.A:
                    key = GamepadKey.A_DOWN;
                    break;

                case Key.Enter:
                    key = GamepadKey.START_DOWN;
                    break;

                case Key.B:
                    key = GamepadKey.B_DOWN;
                    break;

                case Key.Y:
                    key = GamepadKey.Y_DOWN;
                    break;

                case Key.X:
                    key = GamepadKey.X_DOWN;
                    break;

                case Key.Up:
                    key = GamepadKey.UP;
                    break;

                case Key.Down:
                    key = GamepadKey.DOWN;
                    break;

                case Key.Left:
                    key = GamepadKey.LEFT;
                    break;

                case Key.Right:
                    key = GamepadKey.RIGHT;
                    break;

                case Key.Escape:
                    key = GamepadKey.MENU_DOWN;
                    break;

                case Key.OemComma:
                    key = GamepadKey.L1_DOWN;
                    break;

                case Key.OemPeriod:
                    key = GamepadKey.R1_DOWN;
                    break;
            }

            if (key != GamepadKey.None)
            {
                _keys.Add(key); // Use Add instead of Enqueue
            }
        }

        public static void OnKeyUp(KeyEventArgs e)
        {
            GamepadKey key = GamepadKey.None;

            switch (e.Key)
            {
                case Key.A:
                    key = GamepadKey.A_UP;
                    break;

                case Key.Enter:
                    key = GamepadKey.START_UP;
                    break;

                case Key.B:
                    key = GamepadKey.B_UP;
                    break;

                case Key.Y:
                    key = GamepadKey.Y_UP;
                    break;

                case Key.X:
                    key = GamepadKey.X_UP;
                    break;

                case Key.Up:
                    key = GamepadKey.UP_DOWN_UP;
                    break;

                case Key.Down:
                    key = GamepadKey.UP_DOWN_UP;
                    break;

                case Key.Left:
                    key = GamepadKey.LEFT_RIGHT_UP;
                    break;

                case Key.Right:
                    key = GamepadKey.LEFT_RIGHT_UP;
                    break;

                case Key.Escape:
                    key = GamepadKey.MENU_UP;
                    break;

                case Key.OemComma:
                    key = GamepadKey.L1_UP;
                    break;

                case Key.OemPeriod:
                    key = GamepadKey.R1_UP;
                    break;
            }

            if (key != GamepadKey.None)
            {
                _keys.Add(key); // Use Add instead of Enqueue
            }
        }

        public static GamepadKey ReadInput()
        {
            // This will block until a key is available
            return _keys.Take();
        }
    }
}