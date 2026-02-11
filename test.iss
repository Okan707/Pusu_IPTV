[Setup]
AppName=TestApp
AppVersion=1.0
DefaultDirName={autopf}\TestApp
OutputDir=Installer_Output
OutputBaseFilename=TestSetup

[Files]
Source: "IPTVApp\app_settings.txt"; DestDir: "{app}"
