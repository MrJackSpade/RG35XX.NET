using RG35XX.Core.Interfaces;

namespace RG35XX.Linux
{
    public class LinuxStorageProvider : IStorageProvider
    {
        public string MMC => "/mnt/mmc";
        public string SD => "/mnt/sdcard";

        public void Initialize()
        {
        }
    }
}
