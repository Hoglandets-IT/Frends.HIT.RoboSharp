using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace Frends.HIT.RoboSharp {
    public enum RoboExitCode : ushort {
        NoFilesCopied = 0,
        AllFilesCopied = 1,
        AdditionalFilesInDestinationDirectoryNoFilesCopied = 2,
        SomeFilesCopiedAdditionalFilesPresent = 3,
        SomeFilesCopiedSomeFilesMismatched = 5,
        AdditionalFilesAndMismatchedFilesExistNoFilesCopied = 6,
        FilesCopiedMismatchAndAdditionalFilesPresent = 7,
        SeveralFilesDidNotCopy = 8
    }

    public enum RunWithShell {
        [Display(Name = "Powershell")]
        powershell,

        [Display(Name = "Pwsh")]
        pwsh,

        [Display(Name = "Cmd")]
        cmd,
    }

    public enum RoboOption {
        [Display(Name = "Mirror Directory Tree (/MIR)")]
        MIR,

        [Display(Name = "Copy Subdirectories (/E)")]
        E,

        [Display(Name = "Wait for share names to be defined (/TBD)")]
        TBD,

        [Display(Name = "Do not log file names (/NFL)")]
        NFL,

        [Display(Name = "Do not log directory names (/NDL)")]
        NDL,

        [Display(Name = "Do not show progress bar (/NP)")]
        NP,
    }
    public class PathSettings {
        /// <summary>
        /// The path, without a trailing slash
        /// </summary>
        [Display(Name = "Path")]
        [DefaultValue(@"\\SOMESERVER\SomeShare$\SomeFolder")]
        [DisplayFormat(DataFormatString = "Text")]
        [Required]
        public string Path { get; set; }

        /// <summary>
        /// The username to use for the path (e.g. INTERN\f-user)
        /// </summary>
        [Display(Name = @"DOMAIN\Username")]
        [DefaultValue(@"DOMAIN\Username")]
        [DisplayFormat(DataFormatString = "Text")]
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The password for the user
        /// </summary>
        [Display(Name = "Password")]
        [DisplayFormat(DataFormatString = "Expression")]
        [Required]
        public string Password { get; set; }
    }

    public class SyncParameters
    {
        /// <summary>
        /// The shell to use for the sync
        /// </summary>
        [Display(Name = "Shell")]
        [DefaultValue(RunWithShell.powershell)]
        public RunWithShell Shell { get; set; }

        /// <summary>
        /// A list of files to exclude
        /// </summary>
        [Display(Name = @"Excluded Files")]
        [DefaultValue(new string[] { "Thumbs.db", "desktop.ini" })]
        public string[] ExcludeFiles { get; set; }
        
        /// <summary>
        /// A list of folders to exclude
        /// </summary>
        [Display(Name = @"Excluded Folders")]
        [DefaultValue(null)]
        public string[] ExcludeFolders { get; set; }

        /// <summary>
        /// Number of retries on failed copies
        /// </summary>
        [Display(Name = "Retry Count")]
        [DefaultValue("10")]
        public string RetryCount { get; set; }

        /// <summary>
        /// Number of seconds to wait between retries
        /// </summary>
        [Display(Name = "Retry Wait Time")]
        [DefaultValue("30")]
        public string RetryWaitTime { get; set; }

        /// <summary>
        /// A list of options to enable for the sync
        /// </summary>
        [Display(Name = "RoboCopy Options")]
        [DefaultValue(new RoboOption[] { 
            RoboOption.MIR, 
            RoboOption.NFL,
            RoboOption.NDL,
            RoboOption.NP
        })]
        public RoboOption[] Options { get; set; }
    }

    public class InfoMatch {
        public string Match { get; set; }
        public bool GetStatistics { get; set; }
        public bool GetStatisticsBytes { get; set; }
    }

    public class SyncItemInfo {
        public int Total { get; set; }
        public int Copied { get; set; }
        public int Skipped { get; set; }
        public int Mismatch { get; set; }
        public int Failed { get; set; }
        public int Extras { get; set; }
    }

    public class SyncExitInfo {
        public RoboExitCode ExitCode { get; set; }
        public string ExitMessage { get; set; }

        public string Source { get; set; }
        public string Destination { get; set; }

        
        public string Options { get; set; }
        public SyncItemInfo Dirs { get; set; } = null;    
        public SyncItemInfo Files { get; set; } = null;    
        public SyncItemInfo Bytes { get; set; } = null;

        public string Started { get; set; }
        public string Ended { get; set; }
    }
    

    public class Helpers {
        public static InfoMatch[] GetReturnInfo = new InfoMatch[]{
                new InfoMatch{
                    Match = "Dirs",
                    GetStatistics = true,
                    GetStatisticsBytes = false
                },
                new InfoMatch{
                    Match = "Files",
                    GetStatistics = true,
                    GetStatisticsBytes = false
                },
                new InfoMatch{
                    Match = "Bytes",
                    GetStatistics = true,
                    GetStatisticsBytes = true
                },
                new InfoMatch{
                    Match = "Started",
                    GetStatistics = false,
                    GetStatisticsBytes = false
                },
                new InfoMatch{
                    Match = "Ended",
                    GetStatistics = false,
                    GetStatisticsBytes = false
                },
                new InfoMatch{
                    Match = "Options",
                    GetStatistics = false,
                    GetStatisticsBytes = false
                }
            };
        public static SyncItemInfo GetStatistics(string row, bool Bytes = false) {
            string[] desc = row.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (Bytes) {
                desc[1] = desc[1].Replace(" m", "m");
                desc[1] = desc[1].Replace(" g", "g");
                desc[1] = desc[1].Replace(" k", "k");
                desc[1] = desc[1].Replace(" b", "b");
            }

            string[] parts = desc[1].Split(new char[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0 && !Bytes) {
                return new SyncItemInfo() {
                    Total = int.Parse(parts[0]),
                    Copied = int.Parse(parts[1]),
                    Skipped = int.Parse(parts[2]),
                    Mismatch = int.Parse(parts[3]),
                    Failed = int.Parse(parts[4]),
                    Extras = int.Parse(parts[5]),
                };
            }
            else if (parts.Length > 0 && Bytes) {
                return new SyncItemInfo() {
                    Total = int.Parse(parts[0].TrimEnd('m', 'g', 'k', 'b').Split('.')[0]),
                    Copied = int.Parse(parts[1].TrimEnd('m', 'g', 'k', 'b').Split('.')[0]),
                    Skipped = int.Parse(parts[2].TrimEnd('m', 'g', 'k', 'b').Split('.')[0]),
                    Mismatch = int.Parse(parts[3].TrimEnd('m', 'g', 'k', 'b').Split('.')[0]),
                    Failed = int.Parse(parts[4].TrimEnd('m', 'g', 'k', 'b').Split('.')[0]),
                    Extras = int.Parse(parts[5].TrimEnd('m', 'g', 'k', 'b').Split('.')[0]),
                };
            }
            return new SyncItemInfo();
        }
    }
    
}
