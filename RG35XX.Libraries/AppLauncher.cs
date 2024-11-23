using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RG35XX.Libraries
{
    /// <summary>
    /// Only supported on handheld devices. Launches the specified command and exits the current process.
    /// This is how the device can switch between applications without returning control to dmenu
    /// </summary>
    public class AppLauncher
    {
        public void LaunchAndExit(string command)
        {
            //If we're running windows, throw a not supported exception
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This method is not supported on Windows.");
            }

            string nextFile = "/tmp/.next";

            //Write the command to the next file
            System.IO.File.WriteAllText(nextFile, command);

            //Make the file executable
            System.Diagnostics.Process.Start("chmod", $"+x {nextFile}");

            //Exit the current process
            Environment.Exit(0);
        }
    }
}