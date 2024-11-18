using RG35XX.Core.Interfaces;

namespace RG35XX.Windows
{
    public class LocalStorageProvider : IStorageProvider
    {
        public string MMC => "MMC";

        public string SD => "SDCARD";

        public void Initialize()
        {
            if (!Directory.Exists(MMC))
            {
                Directory.CreateDirectory(MMC);
            }
        }
    }
}