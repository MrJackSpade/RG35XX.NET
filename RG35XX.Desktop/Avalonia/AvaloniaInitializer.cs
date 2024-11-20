using Avalonia;

namespace RG35XX.Desktop.Avalonia
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