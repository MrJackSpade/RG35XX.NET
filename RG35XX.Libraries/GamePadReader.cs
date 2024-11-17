﻿using RG35XX.Core.GamePads;
using RG35XX.Core.Interfaces;

namespace RG35XX.Libraries
{
    public class GamePadReader : IGamePadReader
    {
        private readonly IGamePadReader _gamePadReader;
        public GamePadReader()
        {
#if DEBUG
            _gamePadReader = new RG35XX.Windows.KeyboardInput();

#else
            _gamePadReader = new RG35XX.Linux.LinuxGamePadReader();
#endif
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
