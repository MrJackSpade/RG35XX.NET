﻿using RG35XX.Core.Interfaces;

namespace RG35XX.Handheld
{
    public class LinuxStorageProvider : IStorageProvider
    {
        public string MMC => "/mnt/mmc";

        public string ROOT => "/";

        public string SD => "/mnt/sdcard";

        public void Initialize()
        {
        }
    }
}