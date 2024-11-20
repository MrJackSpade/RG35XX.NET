using Avalonia.Threading;

namespace RG35XX.Desktop.Avalonia
{
    public static class AvaloniaEventLoop
    {
        private static bool _started = false;

        public static void Start()
        {
            if (_started)
            {
                return;
            }

            _started = true;

            // Start the Avalonia event loop on a new thread
            Thread t = new(() => Dispatcher.UIThread.MainLoop(CancellationToken.None));

            t.Start();
        }
    }
}