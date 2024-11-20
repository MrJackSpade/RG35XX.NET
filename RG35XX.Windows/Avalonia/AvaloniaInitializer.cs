using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace RG35XX.Windows.Avalonia
{
    public static class AvaloniaInitializer
    {
        public static void InitializeAvalonia()
        {
            // Configure and initialize Avalonia
            AppBuilder.Configure<AvaloniaApp>()
                      .UsePlatformDetect()
                      .SetupWithoutStarting();
        }
    }
}