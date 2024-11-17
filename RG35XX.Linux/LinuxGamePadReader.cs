using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;
using System.Runtime.InteropServices;

namespace RG35XX.Linux
{

    public partial class LinuxGamePadReader : IGamePadReader
    {

        private static readonly byte[] eventBuffer = new byte[8];

        private static FileStream stream;

        public static void Cleanup()
        {
            stream?.Dispose();
        }

        public void Initialize(string devicePath = "/dev/input/js0")
        {
            try
            {
                stream = new FileStream(devicePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to open gamepad device: {e.Message}");
            }
        }

        public GamepadKey ReadInput()
        {
            if (stream == null)
            {
                return GamepadKey.None;
            }

            try
            {
                if (stream.Read(eventBuffer, 0, 8) != 8)
                {
                    return GamepadKey.None;
                }

                JoystickEvent evt = new();
                GCHandle handle = GCHandle.Alloc(eventBuffer, GCHandleType.Pinned);
                try
                {
                    evt = (JoystickEvent)Marshal.PtrToStructure(
                        handle.AddrOfPinnedObject(),
                        typeof(JoystickEvent));
                }
                finally
                {
                    handle.Free();
                }

                JoystickInput input = new()
                {
                    Type = evt.Type,
                    Number = evt.Number,
                    Value = (ushort)evt.Value
                };

                int value = input.Number;

                value <<= 16;

                value |= input.Value;

                return (GamepadKey)value;
            }
            catch
            {
                return GamepadKey.None;
            }
        }
    }
}