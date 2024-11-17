using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG35XX.Core.Interfaces
{
    public interface IFont
    {
        byte[][] Data { get; }

        int Height { get; }

        int Width { get; }
    }
}