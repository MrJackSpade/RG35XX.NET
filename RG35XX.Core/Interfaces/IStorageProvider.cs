using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG35XX.Core.Interfaces
{
    public interface IStorageProvider
    {
        void Initialize();

        public string MMC { get; }

        public string SD { get; }
    }
}
