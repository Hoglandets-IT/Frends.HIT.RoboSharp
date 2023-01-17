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

### SyncItemInfo
| Property | Type | Description |
| --- | --- | --- |
| Total | int | Total amount of items (sum of below) |
| Copied | int | Copied items (not present at destination) |
| Skipped | int | Skipped  items (already present) |
| Mismatch | int | Mismatched items (different at destination) |
| Failed | int | Failed items (failed to copy) |
| Extras | int | Extra items (present at destination) |
