using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

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
            SyncExitInfo exInfo = new SyncExitInfo(){
                Source = target.Source.Path,
                Destination = target.Destination.Path,
                FullLog = new List<string>()
            }; 

            try {
                // Create the authentication commands
                List<string> AuthCommand = new List<string>(){
                    "net use \"" + target.Source.Path + "\" \"" + target.Source.Password + "\" /user:" + target.Source.Username + ";",
                    "net use \"" + target.Destination.Path + "\" \"" + target.Destination.Password + "\" /user:" + target.Destination.Username + ";"
                };
                exInfo.FullLog.Add("Authentication commands created");

                // Create the robocopy command with parameters
                string RoboCommand = "robocopy \"" + target.Source.Path + "\" \"" + target.Destination.Path + "\" /NP" + parameters.GetAdditionalFlags() + "; exit $LASTEXITCODE";
                exInfo.FullLog.Add("Flags fetched and added to Robocopy command");
                exInfo.FullLog.Add("Command: " + RoboCommand);

                // Set shell-based parameters
                string ShellParams = "";
                if (parameters.Shell == RunWithShell.powershell || parameters.Shell == RunWithShell.pwsh) {
                    ShellParams = " -NoProfile -ExecutionPolicy Bypass -Command ";
                }
                else {
                    ShellParams = "/C ";
                }
                exInfo.FullLog.Add("Shell parameters set");


                exInfo.Command = parameters.Shell.ToString() + ".exe" + ShellParams + String.Join(" ", RoboCommand);

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
                exInfo.FullLog.Add("Process created");
                process.Start();
                exInfo.FullLog.Add("Process started");

                while (!process.StandardOutput.EndOfStream) {
                    string line = process.StandardOutput.ReadLine().TrimStart(new char[]{' ', '\t'});
                    if (line.StartsWith("ERROR") || line.StartsWith("The system cannot")) {
                        exInfo.FullLog.Add("ERROR found: ");
                        exInfo.FullLog.Add(line);               
                    }
                    if (line != "") {
                        exInfo.FullLog.Add(line);
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
                exInfo.FullLog.Add("Process exited");

                // Get exit code from RoboExitCode by number
                // Get name of exit code
                // Set other exit parameters
                exInfo.ExitCode = (RoboExitCode)process.ExitCode;
                exInfo.ExitMessage = Regex.Replace(exInfo.ExitCode.ToString(), "(\\B[A-Z])", " $1");
                exInfo.FullLog.Add("Exit info and messages set");

                // Close the process
                process.Close();
                exInfo.FullLog.Add("Process closed");
            }
            catch (Exception e) {
                exInfo.ExitMessage = "Serious Error Occured - Check Full Logs";

                // Add caught error message to log
                exInfo.FullLog.Add(e.Message);
                exInfo.FullLog.Add(JsonConvert.SerializeObject(e));
                exInfo.ExitCode = RoboExitCode.SeriousError;
            }

            if (exInfo.ExitCode == RoboExitCode.SeriousError) {
                    throw new Exception(JsonConvert.SerializeObject(exInfo));
            }

            // Return the exit code
            return exInfo;            
        }
    }
}