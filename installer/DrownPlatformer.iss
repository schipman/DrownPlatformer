; Inno Setup script. Build with installer/build-installer.ps1 (publishes first).
#define AppName "Drown Platformer"
#ifndef AppVersion
  #define AppVersion "0.1.0"
#endif

[Setup]
AppId={{2D5B7E10-9C44-4F6A-8B31-7A0E5D3C9F12}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Scott Chipman
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=..\dist
OutputBaseFilename=DrownPlatformer-Setup-{#AppVersion}
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
LicenseFile=..\LICENSE
WizardStyle=modern
UninstallDisplayIcon={app}\DrownPlatformer.exe

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; Flags: unchecked

[Files]
; self-contained publish: the .NET runtime is inside, nothing else to install
Source: "..\dist\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\DrownPlatformer.exe"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\DrownPlatformer.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\DrownPlatformer.exe"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent
