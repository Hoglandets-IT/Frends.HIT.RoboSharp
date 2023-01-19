using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Frends.HIT.RoboSharp {
    /// <summary>
    /// Main class containing functions
    /// </summary>
    [DisplayName("RoboSharp")]
    public class Main {

        /// <summary>
        /// Synchronize folders over SMB/2/3 using Robocopy
        /// </summary>
        /// <param name="target">Path Settings</param>
        /// <param name="parameters">Sync Settings</param>
        /// <returns>SyncExitInfo Object</returns>
        [DisplayName("Sync SMB Folders")]
        public static SyncExitInfo SyncFolders([PropertyTab]TargetSettings target, [PropertyTab]SyncParameters parameters) {
            // Create the authentication commands
            List<string> AuthCommand = new List<string>(){
                "net use \"" + target.Source.Path + "\" \"" + target.Source.Password + "\" /user:" + target.Source.Username + ";",
                "net use \"" + target.Destination.Path + "\" \"" + target.Destination.Password + "\" /user:" + target.Destination.Username + ";"
            };

            // Create the robocopy command with parameters
            string RoboCommand = "robocopy \"" + target.Source.Path + "\" \"" + target.Destination.Path + "\" /NP" + parameters.GetAdditionalFlags();

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
            List<string> FullLog = new List<string>();

            while (!process.StandardOutput.EndOfStream) {
                string line = process.StandardOutput.ReadLine().TrimStart(new char[]{' ', '\t'});
                if (line.StartsWith("ERROR")) {
                    throw new Exception(line);
                }
                if (line != "") {
                    FullLog.Add(line);
                    foreach (var retn in Helpers.GetReturnInfo)
                    {
                        if (retn.GetStatistics && line.StartsWith(retn.Match + " :") && !line.Contains("*.*")) {
                            exInfo.GetType().GetProperty(retn.Match).SetValue(
                                exInfo, 
                                Helpers.GetStatistics(line, retn.GetStatisticsBytes)
                            );
                        }
                        else if (!retn.GetStatistics && line.StartsWith(retn.Match + " :")) {
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
            // Set other exit parameters
            exInfo.ExitCode = (RoboExitCode)process.ExitCode;
            exInfo.ExitMessage = Regex.Replace(exInfo.ExitCode.ToString(), "(\\B[A-Z])", " $1");
            exInfo.Source = target.Source.Path;
            exInfo.Destination = target.Destination.Path;
            exInfo.Command = String.Join(" ", RoboCommand);
            exInfo.FullLog = FullLog;
            // Close the process
            process.Close();

            // Return the exit code
            return exInfo;
        }
    }
}