using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Frends.HIT.RoboSharp {
    /// <summary>
    /// Main class containing functions
    /// </summary>
    public class Main {

        /// <summary>
        /// Synchronize folders over SMB/2/3 using Robocopy
        /// </summary>
        /// <param name="source">Source path and authentication</param>
        /// <param name="destination">Destintion path and authentiction</param>
        /// <param name="parameters">Robocopy Parameters</param>
        /// <returns>SyncExitInfo Object</returns>
        
        public static SyncExitInfo SyncFolders(PathSettings source, PathSettings destination, [PropertyTab]SyncParameters parameters) {
            // Create the authentication commands
            List<string> AuthCommand = new List<string>(){
                "net use \"" + source.Path + "\" \"" + source.Password + "\" /user:" + source.Username + ";",
                "net use \"" + destination.Path + "\" \"" + destination.Password + "\" /user:" + destination.Username + ";"
            };

            // Assemble the robocopy command parameters
            List<string> RoboCommand = new List<string>(){
                "robocopy \"" + source.Path + "\" \"" + destination.Path + "\" /NP"
            };

            // Add the retry count
            if (parameters.RetryCount != null) {
                RoboCommand.Add("/R:" + parameters.RetryCount);
                if (parameters.RetryWaitTime != null) {
                    RoboCommand.Add("/W:" + parameters.RetryWaitTime);
                }
            }

            // Add additional options
            if (parameters.Options != null) {
                foreach (RoboOption option in parameters.Options) {
                    RoboCommand.Add("/" + option.ToString());
                }
            }

            // Add the exclude files
            if (parameters.ExcludeFiles != null) {
                RoboCommand.Add("/XF \"" + String.Join(" ", parameters.ExcludeFiles) + "\"");
            }

            // Add the exclude folders
            if (parameters.ExcludeFolders != null) {
                RoboCommand.Add("/XD \"" + String.Join(" ", parameters.ExcludeFolders) + "\"");
            }

            // Set shell-based parameters
            string ShellParams = "";
            if (parameters.Shell == RunWithShell.powershell || parameters.Shell == RunWithShell.pwsh) {
                ShellParams = "-NoProfile -ExecutionPolicy Bypass -Command ";
            }
            else {
                ShellParams = "/C ";
            }

            // Create a process
            System.Diagnostics.Process process = new System.Diagnostics.Process{
                StartInfo = new System.Diagnostics.ProcessStartInfo{
                    FileName = parameters.Shell.ToString() + ".exe",
                    Arguments = ShellParams + String.Join(" ", AuthCommand) + String.Join(" ", RoboCommand),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            SyncExitInfo exInfo = new SyncExitInfo(); 

            while (!process.StandardOutput.EndOfStream) {
                string line = process.StandardOutput.ReadLine().TrimStart(new char[]{' ', '\t'});
                if (line.StartsWith("ERROR")) {
                    throw new Exception(line);
                }
                if (line != "") {
                    Console.WriteLine(line);
                    foreach (var retn in Helpers.GetReturnInfo)
                    {
                        if (retn.GetStatistics && line.Substring(0, retn.Match.Length+2) == retn.Match+" :" && !line.Contains("*.*")) {
                            Console.WriteLine("Match: " + retn.Match);
                            exInfo.GetType().GetProperty(retn.Match).SetValue(
                                exInfo, 
                                Helpers.GetStatistics(line, retn.GetStatisticsBytes)
                            );
                        }
                        else if (!retn.GetStatistics && line.Substring(0, retn.Match.Length+2) == retn.Match+" :") {
                            Console.WriteLine("Match 2: " + retn.Match);
                            exInfo.GetType().GetProperty(retn.Match).SetValue(
                                exInfo, 
                                line.Substring(retn.Match.Length+2)
                            );
                        }
                    }
                }
            }

            // Get exit code from RoboExitCode by number
            // Get name of exit code
            exInfo.ExitCode = (RoboExitCode)process.ExitCode;
            exInfo.ExitMessage = Regex.Replace(exInfo.ExitCode.ToString(), "(\\B[A-Z])", " $1");

            // Close the process
            process.Close();

            // Return the exit code
            return exInfo;
        }
    }
}