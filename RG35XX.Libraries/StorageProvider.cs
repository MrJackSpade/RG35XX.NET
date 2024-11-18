using RG35XX.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG35XX.Libraries
{
    public class StorageProvider : IStorageProvider
    {
        private readonly IStorageProvider _storageProvider;

        public string MMC => _storageProvider.MMC;

        public string SD => _storageProvider.SD;

        public StorageProvider()
        {
#if DEBUG
            _storageProvider = new RG35XX.Windows.LocalStorageProvider();
#else
            _storageProvider = new RG35XX.Linux.LinuxStorageProvider();
#endif
        }

        public void Initialize()
        {
            _storageProvider.Initialize();
        }
    }
}