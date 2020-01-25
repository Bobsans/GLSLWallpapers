#define MyAppName "GLSLWallpapers"
#define MyAppVersion "v0.2"
#define MyAppPublisher "Dim-Tim, Inc."
#define MyAppURL "https://github.com/Bobsans/GLSLWallpaper"
#define MyAppExeName "GLSLWallpapers.exe"

[Setup]
AppId={{631961D4-5118-43A8-8DE0-7638945821A3}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=..\_release
OutputBaseFilename={#MyAppName} {#MyAppVersion} setup
SetupIconFile=..\Properties\Resources\icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ChangesAssociations=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[CustomMessages]
english.AdditionalTasks=Additional tasks:
russian.AdditionalTasks=Дополнительные задачи:
english.AssociateFiles=Associate files ".glslwallpaper" with application
russian.AssociateFiles=Ассоциировать файлы ".glslwallpaper" с приложением
english.WallpaperFileType=GLSL wallpaper file
russian.WallpaperFileType=Файл обоев GLSL

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "associate"; Description: "{cm:AssociateFiles}"; GroupDescription: "{cm:AdditionalTasks}"

[Files]
Source: "..\_release\app\GLSLWallpapers.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_release\app\INIFileParser.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_release\app\OpenTK.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_release\app\Sciter.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_release\app\SciterSharpWindows.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_release\app\data\*"; DestDir: "{app}\data"; Flags: ignoreversion recursesubdirs createallsubdirs

[Dirs]
Name: "{app}"; Permissions: everyone-full
Name: "{app}\data"; Permissions: everyone-full

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
Root: HKCR; Subkey: ".glslwallpaper"; ValueType: string; ValueName: ""; ValueData: "{#MyAppName}"; Flags: uninsdeletevalue; Tasks: associate
Root: HKCR; Subkey: "{#MyAppName}"; ValueType: string; ValueName: ""; ValueData: "{cm:WallpaperFileType}"; Flags: uninsdeletekey; Tasks: associate
Root: HKCR; Subkey: "{#MyAppName}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},1"; Tasks: associate
Root: HKCR; Subkey: "{#MyAppName}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""; Tasks: associate

[UninstallDelete]
Type: files; Name: "{app}\latest.log"
