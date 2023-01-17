# Frends.HIT.RoboSharp (.NET 4.7.1)
## Frends.HIT.RoboSharp.SyncFolders
Frends task to synchronize folders between two Windows machines over SMB. Requires .NET 4.7.1 on a Windows machine.
### Parameters
| Property Group | Property | Type | Description | Example |
| --- | --- | --- | --- | --- |
| Source.PathSettings | Path | string | Path to the source folder without trailing slash | "\\someserver\someshare$\folder" |
| Source.PathSettings | Username | string | Username for the source folder | "DOMAIN\USERNAME" |
| Source.PathSettings | Password | string | Password for the source folder | "PASSWORD" |
| Destination.PathSettings | Path | string | Path to the destination folder without trailing slash | "\\someserver\someshare$\folder" |
| Destination.PathSettings | Username | string | Username for the destination folder | "DOMAIN\USERNAME" |
| Destination.PathSettings | Password | string | Password for the destination folder | "PASSWORD" |
| SyncParameters | Shell | Choice | The type of shell to use for the synchronization | Powershell |
| SyncParameters | ExcludeFiles | string[] | List of files to exclude from the synchronization | "file1.txt" "file2.txt" |
| SyncParameters | ExcludeFolders | string[] | List of folders to exclude from the synchronization | "folder1" "folder2" |
| SyncParmeters | RetryCount | int | Number of times to retry the synchronization if it fails | 3 |
| SyncParameters | RetryWaitTime | int | Time to wait between retries in seconds | 30 |
| SyncParameters | RobocopyOptions | RoboOption[] | Options to use for Robocopy command | RoboOption.MIR, RoboOption.NFL, RoboOption.NDL, RoboOption.NP |

### Return Values
| Property | Type | Description | Example |
| --- | --- | --- | --- |
| ExitCode | int | Exit code of program (0-9) | 0 |
| ExitMessage | string | Exit message of program | "The operation completed successfully." |
| Source | string | Path to the source folder | "\\someserver\someshare$\folder" |
| Destination | string | Path to the destination folder | "\\someserver\someshare$\folder" |
| Files | SyncItemInfo | Informtion about amount of transferred files | - |
| Folders | SyncItemInfo | Informtion about amount of transferred folders | - |
| Bytes | SyncItemInfo | Informtion about amount of transferred bytes | - |
| Started | DateTime | Time when the synchronization started | Tuesday, 17 January 2023 15:20:21 |
| Ended | DateTime | Time when the synchronization ended | Tuesday, 17 January 2023 15:21:21 |

### RoboOptions
Options for customizing robocopy behaviour. See [Robocopy documentation](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/robocopy) for more information.

| Option | Description |
| --- | --- |
| MIR | Mirror a whole directory tree (equivalent to /E plus /PURGE) |
| S | Copy Subdirectories (recursive) |
| E | Copy subdirectories, including Empty ones |
| SEC | Include security options |
| COPYALL | Copy all file information/metadata |
| A | Copy only files tagged with the Archive attribute |
| M | Copy only files tagged with the Archive attribute, then remove attribute on source files after sync |
| CREATE | Create destination directory structure and empty files only |
| MOV | Move files (Delete source files after sync but leve directories intact) |
| MOVE | Move files and folders (Delete source files and folders after sync) |
| Z | Copy files in restartable mode |
| ZB | Try copying files in restartable mode, if fails failover to backup mode |
| J | Copy in unbuffered mode (good for larger files) |
| XX | Exclude extra files at destination |
| TBD | Wait for share names to be defined (Retry error 67) |
| NFL | Do not log file names |
| NDL | Do not log directory names |


### SyncItemInfo
| Property | Type | Description |
| --- | --- | --- |
| Total | int | Total amount of items (sum of below) |
| Copied | int | Copied items (not present at destination) |
| Skipped | int | Skipped  items (already present) |
| Mismatch | int | Mismatched items (different at destination) |
| Failed | int | Failed items (failed to copy) |
| Extras | int | Extra items (present at destination) |
