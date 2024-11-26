using System.Diagnostics;

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
                    outLines.Add("    local logfile=\"/tmp/app_scheduling.log\"");
                    outLines.Add("");
                    outLines.Add("    echo \"$(date): Starting app_scheduling()\" >> $logfile");
                    outLines.Add("");
                    outLines.Add("    # Clean up any stale .next files first");
                    outLines.Add("    echo \"$(date): Cleaning up stale .next files\" >> $logfile");
                    outLines.Add("    rm -f /tmp/.next*");
                    outLines.Add("");
                    outLines.Add("    if $CMD > /dev/null 2>&1; then");
                    outLines.Add("        echo \"$(date): CMD executed successfully\" >> $logfile");
                    outLines.Add("        while true; do");
                    outLines.Add("            echo \"$(date): Starting new iteration of while loop\" >> $logfile");
                    outLines.Add("");
                    outLines.Add("            # Find the next file to execute");
                    outLines.Add("            echo \"$(date): Searching for next files...\" >> $logfile");
                    outLines.Add("            ls -la /tmp/.next* >> $logfile 2>&1");
                    outLines.Add("");
                    outLines.Add("            nextfile=$(ls /tmp/.next /tmp/.next-* 2>/dev/null | sort -n -t- -k2 | head -n1)");
                    outLines.Add("            echo \"$(date): Found nextfile: '$nextfile'\" >> $logfile");
                    outLines.Add("");
                    outLines.Add("            # Exit loop if no more .next files");
                    outLines.Add("            if [ ! -f \"$nextfile\" ]; then");
                    outLines.Add("                echo \"$(date): No valid nextfile found, breaking loop\" >> $logfile");
                    outLines.Add("                break");
                    outLines.Add("            fi");
                    outLines.Add("");
                    outLines.Add("            echo \"$(date): Contents of nextfile '$nextfile':\" >> $logfile");
                    outLines.Add("            cat \"$nextfile\" >> $logfile 2>&1");
                    outLines.Add("");
                    outLines.Add("            # Execute the next file");
                    outLines.Add("            echo \"$(date): Attempting to execute $nextfile\" >> $logfile");
                    outLines.Add("            if ! sh $nextfile > /tmp/nextfile.out 2>&1; then");
                    outLines.Add("                echo \"$(date): [allenapp] exe app fail ...\" >> $logfile");
                    outLines.Add("                echo \"$(date): Execution output:\" >> $logfile");
                    outLines.Add("                cat /tmp/nextfile.out >> $logfile");
                    outLines.Add("            else");
                    outLines.Add("                echo \"$(date): Execution successful\" >> $logfile");
                    outLines.Add("            fi");
                    outLines.Add("");
                    outLines.Add("            echo \"$(date): Removing $nextfile\" >> $logfile");
                    outLines.Add("            rm -f \"$nextfile\"");
                    outLines.Add("");
                    outLines.Add("            echo \"$(date): Remaining .next files:\" >> $logfile");
                    outLines.Add("            ls -la /tmp/.next* >> $logfile 2>&1");
                    outLines.Add("");
                    outLines.Add("            echo \"$(date): End of iteration\" >> $logfile");
                    outLines.Add("            echo \"----------------------------------------\" >> $logfile");
                    outLines.Add("        done");
                    outLines.Add("    else");
                    outLines.Add("        echo \"$(date): CMD failed, sleeping for 30 seconds\" >> $logfile");
                    outLines.Add("        sleep 30");
                    outLines.Add("    fi");
                    outLines.Add("");
                    outLines.Add("    echo \"$(date): Exiting app_scheduling()\" >> $logfile");
                    outLines.Add("    echo \"========================================\" >> $logfile");
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

            File.WriteAllLines("/mnt/vendor/ctrl/dmenu_ln", outLines);
        }

        public bool IsDmenuLnPatched()
        {
            string contents = File.ReadAllText("/mnt/vendor/ctrl/dmenu_ln");

            return contents.Contains("#PATCHED NEXT EXECUTION");
        }

        public void LaunchAndExit(string command)
        {
#if DEBUG
            return;
#endif
#pragma warning disable CS0162 // Unreachable code detected
            PatchDmenuLn();
#pragma warning restore CS0162 // Unreachable code detected

            int index = 1;

            string nextFile = $"/tmp/.next-{index}";

            while (File.Exists(nextFile))
            {
                nextFile = $"/tmp/.next-{index}";
            }

            //Write the command to the next file
            System.IO.File.WriteAllText(nextFile, command);

            //Make the file executable
            Process p = System.Diagnostics.Process.Start("chmod", $"+x {nextFile}");

            p.WaitForExit();

            //Exit the current process
            Environment.Exit(0);
        }
    }
}