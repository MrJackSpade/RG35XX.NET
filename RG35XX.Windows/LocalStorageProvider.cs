using RG35XX.Core.Interfaces;

namespace RG35XX.Windows
{
    public class LocalStorageProvider : IStorageProvider
    {
        public string MMC => Path.Combine(ROOT, "MMC");

        public string ROOT => Directory.GetCurrentDirectory();

        public string SD => Path.Combine(ROOT, "SDCARD");

        public void Initialize()
        {
            if (!Directory.Exists(MMC))
            {
                Directory.CreateDirectory(MMC);
            }
        }
    }
}