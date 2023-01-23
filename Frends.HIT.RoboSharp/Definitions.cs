using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.HIT.RoboSharp {
    /// <summary>
    /// Exit codes for the Robocopy process
    /// </summary>
    public enum RoboExitCode : ushort {
        /// <summary>
        /// No errors occurred, and no copying was done.
        /// The source and destination directory trees are completely synchronized. 
        /// </summary>
        NoFilesCopied = 0,

        /// <summary>
        /// One or more files were copied successfully and everything is now synchronized.
        /// </summary>
        AllFilesCopied = 1,

        /// <summary>
        /// Some Extra files or directories were detected at the destintion. No files were copied
        /// </summary>
        AdditionalFilesInDestinationDirectoryNoFilesCopied = 2,

        /// <summary>
        /// Some files were copied. Additional files were present. No failure was encountered.
        /// </summary>
        SomeFilesCopiedAdditionalFilesPresent = 3,

        /// <summary>
        /// Some Mismatched files or directories were detected.
        /// Examine the output log. Housekeeping might be required.
        /// </summary>
        SomeMismatchedFilesDetected = 4,

        /// <summary>
        /// Some files were copied. Some files were mismatched. No failure was encountered.
        /// </summary>
        SomeFilesCopiedSomeFilesMismatched = 5,

        /// <summary>
        /// Additional files and mismatched files exist. No files were copied and no failures were encountered.
        /// This means that the files already exist in the destination directory
        /// </summary>
        AdditionalFilesAndMismatchedFilesExistNoFilesCopied = 6,

        /// <summary>
        /// Files were copied, a file mismatch was present, and additional files were present.
        /// </summary>
        FilesCopiedMismatchAndAdditionalFilesPresent = 7,

        /// <summary>
        /// Some files or directories could not be copied
        /// (copy errors occurred and the retry limit was exceeded).
        /// Check these errors further.
        /// </summary>
        SeveralFilesDidNotCopy = 8,

        /// <summary>
        /// Serious error. Robocopy did not copy any files.
        /// Either a usage error or an error due to insufficient access privileges
        /// on the source or destination directories.
        /// </summary>
        SeriousError = 16
    }

    /// <summary>
    /// The shell to use for the sync command
    /// </summary>
    public enum RunWithShell {
        /// <summary>
        /// Use Powershell (~5)
        /// </summary>
        [Display(Name = "Powershell")]
        powershell,

        /// <summary>
        /// Use pwsh (Powershell ~7)
        /// </summary>
        [Display(Name = "Pwsh")]
        pwsh,

        /// <summary>
        /// Use cmd.exe
        /// </summary>
        [Display(Name = "Cmd")]
        cmd,
    }

    /// <summary>
    /// Configuration for SMB paths
    /// </summary>
    public class PathSettings {
        /// <summary>
        /// The path, without a trailing slash
        /// </summary>
        /// <example>\\SOMESERVER\SomeShare$\SomeFolder</example>
        [DisplayFormat(DataFormatString = "Text")]
        [Required]
        public string Path { get; set; }

        /// <summary>
        /// The username to use for the path (e.g. INTERN\f-user)
        /// </summary>
        /// <example>DOMAIN\f-user</example>
        [DisplayFormat(DataFormatString = "Text")]
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The password for the user
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Configuration object for transfer settings
    /// </summary>
    public class TargetSettings
    {
        /// <summary>
        /// The source folder to copy files from
        /// </summary>
        public PathSettings Source { get; set; }

        /// <summary>
        /// The destination folder to copy files to
        /// </summary>
        public PathSettings Destination { get; set; }
    }

    /// <summary>
    /// Parameters for RoboCopy command
    /// </summary>
    public class SyncParameters
    {
        /// <summary>
        /// The shell to use for the sync
        /// </summary>
        [DefaultValue(RunWithShell.powershell)]
        public RunWithShell Shell { get; set; }

        /// <summary>
        /// A list of files to exclude
        /// </summary>
        [DefaultValue(null)]
        public string[] ExcludeFiles { get; set; }
        
        /// <summary>
        /// A list of folders to exclude
        /// </summary>
        [DefaultValue(null)]
        public string[] ExcludeFolders { get; set; }

        /// <summary>
        /// Mirror Directory Tree
        /// Mirror the entire directory tree (recursive)
        /// Will delete, create and update in destination dir
        /// Same as /PURGE and /E
        /// </summary>
        [DefaultValue(true)]
        public bool MIR { get; set; }

        /// <summary>
        /// Copy Subdirectories
        /// </summary>
        [DefaultValue(false)]
        public bool S { get; set; }
        
        /// <summary>
        /// Copy Subdirectories including empty folders
        /// </summary>
        [DefaultValue(false)]
        public bool E { get; set; }

        /// <summary>
        /// Copy files with security options
        /// </summary>
        [DefaultValue(false)]
        public bool SEC { get; set; }


        /// <summary>
        /// Include all file information and metadata
        /// </summary>
        [DefaultValue(false)]
        public bool COPYALL { get; set; }

        /// <summary>
        /// Copy only files with the archive attribute set
        /// </summary>
        [DefaultValue(false)]
        public bool A { get; set; }

        /// <summary>
        /// Copy only files with the archive attribute set but remove the attribute on source files
        /// </summary>
        [DefaultValue(false)]
        public bool M { get; set; }

        /// <summary>
        /// Only create directory tree and zero-length files
        /// </summary>
        [DefaultValue(false)]
        public bool CREATE { get; set; }
        
        /// <summary>
        /// Deletes all source files after copying but leaves dir tree intact
        /// </summary>
        [DefaultValue(false)]
        public bool MOV { get; set; }

        /// <summary>
        /// Deletes the source files and folders after synchronization
        /// </summary>
        [DefaultValue(false)]
        public bool MOVE { get; set; }

        /// <summary>
        /// Copies files in restartable mode (survive network glitches)
        /// </summary>
        [DefaultValue(false)]
        public bool Z { get; set; }
        
        /// <summary>
        /// Try using restartable mode, if not possible fallback to backup mode
        /// </summary>
        [DefaultValue(true)]
        public bool ZB { get; set; }

        /// <summary>
        /// Copes files in backup mode   
        /// </summary>
        [DefaultValue(false)]
        public bool B { get; set; }

        /// <summary>
        /// Copies the files directly without the buffer, useful for large files
        /// </summary>
        [DefaultValue(false)]
        public bool J { get; set; }

        /// <summary>
        /// Don't delete additional files present at destination
        /// </summary>
        [DefaultValue(false)]
        public bool XX { get; set; }

        /// <summary>
        /// Wait for share names to be defined (Retry error 67)
        /// </summary>
        [DefaultValue(false)]
        public bool TBD { get; set; }

        /// <summary>
        /// Skip logging filenames
        /// </summary>
        [DefaultValue(true)]
        public bool NFL { get; set; }

        /// <summary>
        /// Skip logging directory names
        /// </summary>
        [DefaultValue(true)]
        public bool NDL { get; set; }

        /// <summary>
        /// Number of retries on failed copies
        /// </summary>
        [DefaultValue(10)]
        public int R { get; set; }

        /// <summary>
        /// Number of seconds to wait between retries
        /// </summary>
        [DefaultValue(30)]
        public int W { get; set; }

        /// <summary>
        /// Returns true if any additional flags are set
        /// </summary>
        public bool HasAdditionalFlags()
        {
            return S || E || SEC || COPYALL || A || M || CREATE || MOV || MOVE || Z || ZB || B || J || XX || TBD || NFL || NDL;
        }

        /// <summary>
        /// Returns an instance with default values
        /// </summary>
        public static SyncParameters GetDefaultInstance()
        {
            return new SyncParameters()
            {
                Shell = RunWithShell.powershell,
                ExcludeFiles = null,
                ExcludeFolders = null,
                MIR = true,
                S = false,
                E = false,
                SEC = false,
                COPYALL = false,
                A = false,
                M = false,
                CREATE = false,
                MOV = false,
                MOVE = false,
                Z = false,
                ZB = true,
                B = false,
                J = false,
                XX = false,
                TBD = false,
                NFL = true,
                NDL = true,
                R = 10,
                W = 30
            };
        }

        /// <summary>
        /// Returns the additional flags as a string
        /// </summary>
        public string GetAdditionalFlags()
        {
            var flags = new List<string>(){
                ExcludeFiles.Length > 0 ? "/XF \"" + String.Join(" ", ExcludeFiles) + "\"" : null, 
                ExcludeFolders.Length > 0 ? "/XD \"" + String.Join(" ", ExcludeFolders) + "\"" : null, 
                MIR ? "/MIR" : null,
                S ? "/S" : null,
                E ? "/E" : null,
                SEC ? "/SEC" : null,
                COPYALL ? "/COPYALL" : null,
                A ? "/A" : null,
                M ? "/M" : null,
                CREATE ? "/CREATE" : null,
                MOV ? "/MOV" : null,
                MOVE ? "/MOVE" : null,
                Z ? "/Z" : null,
                ZB ? "/ZB" : null,
                B ? "/B" : null,
                J ? "/J" : null,
                XX ? "/XX" : null,
                TBD ? "/TBD" : null,
                NFL ? "/NFL" : null,
                NDL ? "/NDL" : null,
                R > 0 ? "/R:" + R : null,
                W > 0 ? "/W:" + W : null
            };

            return " " + string.Join(" ", flags.FindAll(f => !string.IsNullOrEmpty(f)));
        }
    }

    /// <summary>
    /// Match object used for decoding RoboCopy output
    /// </summary>
    public class InfoMatch {
        /// <summary>
        /// Match this at the beginning of the line
        /// </summary>
        public string Match { get; set; }

        /// <summary>
        /// Denotes if the row is a 6-fielded statistics row
        /// </summary>
        public bool GetStatistics { get; set; }

        /// <summary>
        /// If true, the row is a 6-fld statistics row with floats insted of ints
        /// </summary>
        public bool GetStatisticsBytes { get; set; }
    }

    /// <summary>
    /// Information about the synced files, folders and other stats
    /// </summary>
    public class SyncItemInfo {

        /// <summary>
        /// Number/Size of files handled, total (sum of other fields)
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Number/Size of copied items
        /// </summary>
        public int Copied { get; set; }

        /// <summary>
        /// Number/Size of skipped items
        /// </summary>
        public int Skipped { get; set; }

        /// <summary>
        /// Number/Size of mismatched items
        /// </summary>
        public int Mismatch { get; set; }

        /// <summary>
        /// Number/Size of failed items
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        /// Number/Size of extra items at destination
        /// </summary>
        public int Extras { get; set; }
    }

    /// <summary>
    /// Exit information from RoboCopy
    /// </summary>
    public class SyncExitInfo {
        /// <summary>
        /// The program exit code (e.g. 0 for No Files Transferred)
        /// </summary>
        public RoboExitCode ExitCode { get; set; }

        /// <summary>
        /// The exit message corresponding to the code (e.g. "No files transferred")
        /// </summary>
        public string ExitMessage { get; set; }

        /// <summary>
        /// The source path that was copied
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The destination path
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Options used for the sync
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// Directory statistics
        /// </summary>
        public SyncItemInfo Dirs { get; set; } = null;    

        /// <summary>
        /// File statistics
        /// </summary>
        public SyncItemInfo Files { get; set; } = null;    

        /// <summary>
        /// Total data copied statistics
        /// </summary>
        public SyncItemInfo Bytes { get; set; } = null;

        /// <summary>
        /// When the sync was started
        /// </summary>
        public string Started { get; set; }

        /// <summary>
        /// When the sync was finished
        /// </summary>
        /// <value></value>
        public string Ended { get; set; }

        /// <summary>
        /// The full log from RoboCopy
        /// </summary>
        public List<string> FullLog { get; set; }

        /// <summary>
        /// The entire command used to perform the sync
        /// </summary>
        /// <value></value>
        public string Command { get; set; }
    }
    
    /// <summary>
    /// Helpers for various operations
    /// </summary>
    public class Helpers {

        /// <summary>
        /// Used for decoding robocopy output
        /// </summary>
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
        
        /// <summary>
        /// Get statistics from output row if available
        /// </summary>
        /// <param name="row">The output row</param>
        /// <param name="Bytes">Whether the row is the "Bytes"-statistic</param>
        /// <returns>SyncItemInfo</returns>
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
