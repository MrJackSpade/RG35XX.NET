using System.Runtime.InteropServices;

namespace RG35XX.Libraries
{
    /// <summary>
    /// Only supported on handheld devices. Launches the specified command and exits the current process.
    /// This is how the device can switch between applications without returning control to dmenu
    /// </summary>
    public class AppLauncher
    {
        public static void PatchDmenuLn()
        {
            string contents = File.ReadAllText("/mnt/vendor/ctrl/dmenu_ln");

            if (contents.Contains("#PATCHED NEXT EXECUTION"))
            {
                return;
            }

            List<string> lines = File.ReadAllLines("/mnt/vendor/ctrl/dmenu_ln").ToList();

            List<string> outLines = [];

            bool patching = false;

            foreach (string line in lines)
            {
                if (line.StartsWith("function app_scheduling()"))
                {
                    patching = true;

                    outLines.Add("#PATCHED NEXT EXECUTION");
                    outLines.Add("function app_scheduling()");
                    outLines.Add("{");
                    outLines.Add("    # Clean up any stale .next files first");
                    outLines.Add("    rm -f /tmp/.next*");
                    outLines.Add("");
                    outLines.Add("    if $CMD > /dev/null 2>&1; then");
                    outLines.Add("        while true; do");
                    outLines.Add("            # Find the next file to execute");
                    outLines.Add("            nextfile=$(ls /tmp/.next /tmp/.next-* 2>/dev/null | sort -n -t- -k2 | head -n1)");
                    outLines.Add("");
                    outLines.Add("            # Exit loop if no more .next files");
                    outLines.Add("            if [ ! -f \"$nextfile\" ]; then");
                    outLines.Add("                break");
                    outLines.Add("            fi");
                    outLines.Add("");
                    outLines.Add("            # Execute the next file");
                    outLines.Add("            if ! sh $nextfile > /dev/null 2>&1; then");
                    outLines.Add("                echo \"[allenapp] exe app fail ...\"");
                    outLines.Add("            fi");
                    outLines.Add("            rm -f \"$nextfile\"");
                    outLines.Add("        done");
                    outLines.Add("    else");
                    outLines.Add("        sleep 30");
                    outLines.Add("    fi");
                    outLines.Add("}");
                }

                if (!patching)
                {
                    outLines.Add(line);
                }

                if (patching)
                {
                    if (line.StartsWith('}'))
                    {
                        patching = false;
                    }
                }
            }
        }

        public void LaunchAndExit(string command)
        {
#if DEBUG
    return;
#endif
            PatchDmenuLn();

            int index = 1;

            string nextFile = $"/tmp/.next-{index}";

            while (File.Exists(nextFile))
            {
                nextFile = $"/tmp/.next-{index}";
            }

            //Write the command to the next file
            System.IO.File.WriteAllText(nextFile, command);

            //Make the file executable
            System.Diagnostics.Process.Start("chmod", $"+x {nextFile}");

            //Exit the current process
            Environment.Exit(0);
        }
    }
}