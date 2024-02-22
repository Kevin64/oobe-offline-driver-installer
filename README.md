# OOBE Offline Driver Installer (OODI)

OODI is a simple program made for installing drivers automatically on a computer. The program is designed to be inserted as a routine inside the Windows device installation process, specifically at the `oobeSystem` step (see https://learn.microsoft.com/en-us/windows-hardware/manufacture/desktop/oobesystem?view=windows-11 for more details). However it isn't mandatory to use that way and can be executed as a normal file inside a working Windows 10 x64 or 11 installation.

## Setting up
Modifying the contents of the `Definitions` section inside the `config.json` file allows you to set some settings as default, like setting the log file location, driver folder path, whether it cleans or not the files, etc.

```json
"Definitions": {
    "LogLocation": "C:\\AppLog\\",
    "AddDrivers": true,
    "InstallDrivers": true,
    "CleanGarbage": true,
    "ChangeResolution": true,
    "ResolutionWidth": 1366,
    "ResolutionHeight": 768,
    "DriverPath": "C:\\Drivers\\",
    "RebootAfterFinished": false,
    "PauseAfterFinished": false
}
```
