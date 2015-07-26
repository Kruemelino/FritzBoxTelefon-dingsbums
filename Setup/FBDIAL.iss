#include ReadReg(HKEY_LOCAL_MACHINE,'Software\Sherlock Software\InnoTools\Downloader','ScriptPath','')

#define MyAppName "Fritz!Box Telefon-dingsbums"
#define MyAppVersion "3.9.5"
#define MyAppPublisher "Kruemelino"
#define MyAppURL "https://github.com/Kruemelino/FritzBoxTelefon-dingsbums"
#define MyAppDescription "Das Fritz!Box Telefon-dingsbums ist ein Outlook-Addin, welches ein direktes Wählen der Kontakte aus dem Computer ermöglicht. Zusätzlich bietet es nützliche Funktionen, wie einen Anrufmonitor oder eine Rückwärtssuche."
#define MyGUID "411894A1-05D5-4F89-B336-4A4175D5E537" 
#define MyAppNameKurz "FritzBoxDial"
#define MyAppTime GetDateTimeString('yymmdd', '', '') 
#define MyAppType "" 
[Setup]
AppId={{411894A1-05D5-4F89-B336-4A4175D5E537}}
AppName={#MyAppName} {#MyAppType}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={code:DefDirRoot}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=FBDBSetup_{#MyAppVersion}_{#MyAppTime}
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=none
SignTool=WinSDK /t $qhttp://timestamp.verisign.com/scripts/timstamp.dll$q /du $q{#MyAppURL}$q /d $q{#MyAppDescription}$q $f
SignedUninstaller=yes
[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Registry]
;Office 2003

Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FritzBoxDial";                  Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: dword;  ValueName: "CommandLineSafe";   ValueData: "1" 
Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FritzBoxDial";                  Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string; ValueName: "Description";       ValueData: "{#MyAppDescription}"
Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FritzBoxDial";                  Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string; ValueName: "FriendlyName";      ValueData: "{#MyAppName}"
Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FritzBoxDial";                  Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: dword;  ValueName: "LoadBehavior";      ValueData: "3"
Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FritzBoxDial";                  Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string; ValueName: "Manifest";          ValueData: "{app}\{#MyAppNameKurz}.dll.manifest"

Root: HKLM; Subkey: "Software\Classes\{#MyAppNameKurz}\CLSID";                                Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string;                                 ValueData: {code:CurrectGUID}
Root: HKLM; Subkey: "Software\Classes\CLSID\{code:CurrectGUID}";                              Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string;                                 ValueData: {#MyAppDescription}
Root: HKLM; Subkey: "Software\Classes\CLSID\{code:CurrectGUID}\InprocServer32";               Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string;                                 ValueData: "{cf}\Microsoft Shared\VSTO\8.0\AddinLoader.dll"
Root: HKLM; Subkey: "Software\Classes\CLSID\{code:CurrectGUID}\InprocServer32";               Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string; ValueName: "ManifestLocation";  ValueData: "{app}\"
Root: HKLM; Subkey: "Software\Classes\CLSID\{code:CurrectGUID}\InprocServer32";               Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string; ValueName: "ManifestName";      ValueData: "{#MyAppNameKurz}.dll.manifest"
Root: HKLM; Subkey: "Software\Classes\CLSID\{code:CurrectGUID}\InprocServer32";               Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string; ValueName: "ThreadingModel";    ValueData: "Both"
Root: HKLM; Subkey: "Software\Classes\CLSID\{code:CurrectGUID}\ProgID";                       Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string;                                 ValueData: "{#MyAppNameKurz}"
Root: HKLM; Subkey: "Software\Classes\CLSID\{code:CurrectGUID}\Programmable";                 Check: OutlookVersion(2003);  Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Classes\CLSID\{code:CurrectGUID}\VersionIndependentProgID";     Check: OutlookVersion(2003);  Flags: uninsdeletekey; ValueType: string;                                 ValueData: "{#MyAppNameKurz}"

;Office 2007
Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FBDB.FritzBoxDial";             Check: OutlookVersion(2007);  Flags: uninsdeletekey; ValueType: string; ValueName: "Description";       ValueData: "{#MyAppDescription}"
Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FBDB.FritzBoxDial";             Check: OutlookVersion(2007);  Flags: uninsdeletekey; ValueType: string; ValueName: "FriendlyName";      ValueData: "{#MyAppName}"
Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FBDB.FritzBoxDial";             Check: OutlookVersion(2007);  Flags: uninsdeletekey; ValueType: dword;  ValueName: "LoadBehavior";      ValueData: "3"
Root: HKCU; Subkey: "Software\Microsoft\Office\Outlook\Addins\FBDB.FritzBoxDial";             Check: OutlookVersion(2007);  Flags: uninsdeletekey; ValueType: string; ValueName: "Manifest";          ValueData: "file:///{app}/Fritz!Box Telefon-Dingsbums.vsto|vstolocal"
;Office 2010 & 2013 x64
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: "Outlookx64 and (OutlookVersion(2010) or OutlookVersion(2013))";       Flags: uninsdeletekey; ValueType: string; ValueName: "Description";   ValueData: "{#MyAppDescription}"
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: "Outlookx64 and (OutlookVersion(2010) or OutlookVersion(2013))";       Flags: uninsdeletekey; ValueType: string; ValueName: "FriendlyName";  ValueData: "{#MyAppName}"
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: "Outlookx64 and (OutlookVersion(2010) or OutlookVersion(2013))";       Flags: uninsdeletekey; ValueType: dword;  ValueName: "LoadBehavior";  ValueData: "3"
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: "Outlookx64 and (OutlookVersion(2010) or OutlookVersion(2013))";       Flags: uninsdeletekey; ValueType: string; ValueName: "Manifest";      ValueData: "file:///{app}/Fritz!Box Telefon-Dingsbums.vsto|vstolocal"
;Office 2010 & 2013 x32
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: "not Outlookx64 and (OutlookVersion(2010) or OutlookVersion(2013))";  Flags: uninsdeletekey; ValueType: string; ValueName: "Description";    ValueData: "{#MyAppDescription}"
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: "not Outlookx64 and (OutlookVersion(2010) or OutlookVersion(2013))";  Flags: uninsdeletekey; ValueType: string; ValueName: "FriendlyName";   ValueData: "{#MyAppName}"
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: "not Outlookx64 and (OutlookVersion(2010) or OutlookVersion(2013))";  Flags: uninsdeletekey; ValueType: dword;  ValueName: "LoadBehavior";   ValueData: "3"
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: "not Outlookx64 and (OutlookVersion(2010) or OutlookVersion(2013))";  Flags: uninsdeletekey; ValueType: string; ValueName: "Manifest";       ValueData: "file:///{app}/Fritz!Box Telefon-Dingsbums.vsto|vstolocal"

[Files]
;Office 2003
#if FileExists("2003\FritzBoxDial.dll") & FileExists("2003\FritzBoxDial.dll.manifest") & FileExists("2003\Funktionen.dll") & FileExists("2003\PopupFenster.dll") 
  Source: "2003\FritzBoxDial.dll";                                    Check: OutlookVersion(2003); DestDir: "{app}"; Flags: ignoreversion
  Source: "2003\FritzBoxDial.dll.manifest";                           Check: OutlookVersion(2003); DestDir: "{app}"; Flags: ignoreversion
  Source: "2003\Funktionen.dll";                                      Check: OutlookVersion(2003); DestDir: "{app}"; Flags: ignoreversion
  Source: "2003\PopupFenster.dll";                                    Check: OutlookVersion(2003); DestDir: "{app}"; Flags: ignoreversion
#endif
#if FileExists("2003\setcaspol.exe")
  Source: "2003\setcaspol.exe";                                       Check: OutlookVersion(2003); DestDir: "{app}"; Flags: ignoreversion
#endif
;Office 2003 - Debuginformationen
#if FileExists("2003\FritzBoxDial.pdb") & FileExists("2003\Funktionen.pdb") & FileExists("2003\PopupFenster.pdb") 
  Source: "2003\FritzBoxDial.pdb";                                    Check: OutlookVersion(2003); DestDir: "{app}"; Flags: ignoreversion
  Source: "2003\Funktionen.pdb";                                      Check: OutlookVersion(2003); DestDir: "{app}"; Flags: ignoreversion
  Source: "2003\PopupFenster.pdb";                                    Check: OutlookVersion(2003); DestDir: "{app}"; Flags: ignoreversion
#endif

;Office 2007
#if FileExists("2007\Fritz!Box Telefon-Dingsbums.dll") & FileExists("2007\Fritz!Box Telefon-Dingsbums.dll.manifest") & FileExists("2007\Fritz!Box Telefon-Dingsbums.vsto") & FileExists("2007\Funktionen.dll") & FileExists("2007\PopupFenster.dll")
  Source: "2007\Fritz!Box Telefon-Dingsbums.dll";                     Check: OutlookVersion(2007); DestDir: "{app}"; Flags: ignoreversion
  Source: "2007\Fritz!Box Telefon-Dingsbums.dll.manifest";            Check: OutlookVersion(2007); DestDir: "{app}"; Flags: ignoreversion
  Source: "2007\Fritz!Box Telefon-Dingsbums.vsto";                    Check: OutlookVersion(2007); DestDir: "{app}"; Flags: ignoreversion
  Source: "2007\Funktionen.dll";                                      Check: OutlookVersion(2007); DestDir: "{app}"; Flags: ignoreversion
  Source: "2007\PopupFenster.dll";                                    Check: OutlookVersion(2007); DestDir: "{app}"; Flags: ignoreversion
#endif

;Office 2007 - Debuginformationen
#if FileExists("2007\\Fritz!Box Telefon-Dingsbums.pdb")
  Source: "2007\Fritz!Box Telefon-Dingsbums.pdb";                     Check: OutlookVersion(2007); DestDir: "{app}"; Flags: ignoreversion
  Source: "2007\Funktionen.pdb";                                      Check: OutlookVersion(2007); DestDir: "{app}"; Flags: ignoreversion
  Source: "2007\PopupFenster.pdb";                                    Check: OutlookVersion(2007); DestDir: "{app}"; Flags: ignoreversion
#endif

;Office 2010
#if FileExists("2010\Fritz!Box Telefon-Dingsbums.dll") & FileExists("2010\Fritz!Box Telefon-Dingsbums.dll.manifest") & FileExists("2010\Fritz!Box Telefon-Dingsbums.vsto") & FileExists("2010\Funktionen.dll") & FileExists("2010\PopupFenster.dll") & FileExists("2010\FritzBoxUPnP.dll") & FileExists("2010\Newtonsoft.Json.dll")
  Source: "2010\Fritz!Box Telefon-Dingsbums.dll";                     Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\Fritz!Box Telefon-Dingsbums.dll.manifest";            Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\Fritz!Box Telefon-Dingsbums.vsto";                    Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\Funktionen.dll";                                      Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\PopupFenster.dll";                                    Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\FritzBoxUPnP.dll";                                    Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\Newtonsoft.Json.dll";                                 Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
#endif

;Office 2010 - Debuginformationen
#if FileExists("2010\\Fritz!Box Telefon-Dingsbums.pdb")
  Source: "2010\Fritz!Box Telefon-Dingsbums.pdb";                     Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\Funktionen.pdb";                                      Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\PopupFenster.pdb";                                    Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
  Source: "2010\FritzBoxUPnP.pdb";                                    Check: OutlookVersion(2010); DestDir: "{app}"; Flags: ignoreversion
#endif

;Office 2013
#if FileExists("2013\Fritz!Box Telefon-Dingsbums.dll") & FileExists("2013\Fritz!Box Telefon-Dingsbums.dll.manifest") & FileExists("2013\Fritz!Box Telefon-Dingsbums.vsto") & FileExists("2013\Funktionen.dll") & FileExists("2013\PopupFenster.dll")
  Source: "2013\Fritz!Box Telefon-Dingsbums.dll";                     Check: OutlookVersion(2013); DestDir: "{app}"; Flags: ignoreversion
  Source: "2013\Fritz!Box Telefon-Dingsbums.dll.manifest";            Check: OutlookVersion(2013); DestDir: "{app}"; Flags: ignoreversion
  Source: "2013\Fritz!Box Telefon-Dingsbums.vsto";                    Check: OutlookVersion(2013); DestDir: "{app}"; Flags: ignoreversion
  Source: "2013\Funktionen.dll";                                      Check: OutlookVersion(2013); DestDir: "{app}"; Flags: ignoreversion
  Source: "2013\PopupFenster.dll";                                    Check: OutlookVersion(2013); DestDir: "{app}"; Flags: ignoreversion
#endif

;Office 2013 - Debuginformationen
#if FileExists("2013\\Fritz!Box Telefon-Dingsbums.pdb")
  Source: "2013\Fritz!Box Telefon-Dingsbums.pdb";                     Check: OutlookVersion(2013); DestDir: "{app}"; Flags: ignoreversion
  Source: "2013\Funktionen.pdb";                                      Check: OutlookVersion(2013); DestDir: "{app}"; Flags: ignoreversion
  Source: "2013\PopupFenster.pdb";                                    Check: OutlookVersion(2013); DestDir: "{app}"; Flags: ignoreversion
#endif
   
;Office 2007 & 2010 & 2013 COMMON
#if FileExists("2007\Fritz!Box Telefon-Dingsbums.dll") | FileExists("2010\Fritz!Box Telefon-Dingsbums.dll") | FileExists("2013\Fritz!Box Telefon-Dingsbums.dll")
  #if FileExists("Common\Microsoft.Office.Tools.Common.v4.0.Utilities.dll") & FileExists("Common\Microsoft.Office.Tools.Outlook.v4.0.Utilities.dll")
    Source: "Common\Microsoft.Office.Tools.Common.v4.0.Utilities.dll";  Check: (not OutlookVersion(2003)); DestDir: "{app}"; Flags: ignoreversion
    Source: "Common\Microsoft.Office.Tools.Outlook.v4.0.Utilities.dll"; Check: (not OutlookVersion(2003)); DestDir: "{app}"; Flags: ignoreversion
  #else
    #error Microsoft.Office.Tools.Common.v4.0.Utilities.dll und/oder Microsoft.Office.Tools.Outlook.v4.0.Utilities.dll fehlen im Ordner "Common"
  #endif
#else

#endif

[Run]
;Office 2003
Filename: {code:CaspolPath}; Parameters: "-pp off"; WorkingDir: {win}\Microsoft.NET\Framework\v2.0.50727\; Flags: waituntilidle runhidden; Check: OutlookVersion(2003); StatusMsg: "Registriere Addin für Office 2003: Start..."
  Filename: {code:CaspolPath}; Parameters: "-machine -addgroup 1 -strong -file ""{app}\FritzBoxDial.dll"" -noname -noversion  FullTrust -n ""{#MyAppNameKurz}"" -d ""{#MyAppNameKurz} mit FullTrust"""; WorkingDir: {win}\Microsoft.NET\Framework\v2.0.50727\; Flags: waituntilidle runhidden ; Check: OutlookVersion(2003); StatusMsg: "Registriere Addin für Office 2003: FritzBoxDial.dll..."
  Filename: {code:CaspolPath}; Parameters: "-machine -addgroup 1 -strong -file ""{app}\PopupFenster.dll"" -noname -noversion  FullTrust -n ""{#MyAppNameKurz}"" -d ""{#MyAppNameKurz} (PopupFenster.dll) mit FullTrust"""; WorkingDir: {win}\Microsoft.NET\Framework\v2.0.50727\; Flags: waituntilidle runhidden; Check: OutlookVersion(2003); StatusMsg: "Registriere Addin für Office 2003: PopupFenster.dll..."
  Filename: {code:CaspolPath}; Parameters: "-machine -addgroup 1 -strong -file ""{app}\Funktionen.dll"" -noname -noversion  FullTrust -n ""{#MyAppNameKurz}"" -d ""{#MyAppNameKurz} (Funktionen.dll) mit FullTrust"""; WorkingDir: {win}\Microsoft.NET\Framework\v2.0.50727\; Flags: waituntilidle runhidden; Check: OutlookVersion(2003); StatusMsg: "Registriere Addin für Office 2003: Funktionen.dll..."
#if FileExists("2003\FritzBoxDial.pdb") & FileExists("2003\Funktionen.pdb") & FileExists("2003\PopupFenster.pdb") 
	Filename: {code:CaspolPath}; Parameters: "-machine -addgroup 1 -strong -file ""{app}\FritzBoxDial.pdb"" -noname -noversion  FullTrust -n ""{#MyAppNameKurz}"" -d ""{#MyAppNameKurz} (FritzBoxDial.pdb) mit FullTrust"""; WorkingDir: {win}\Microsoft.NET\Framework\v2.0.50727\; Flags: waituntilidle runhidden; Check: OutlookVersion(2003); StatusMsg: "Registriere Addin für Office 2003: FritzBoxDial.pdb..."
  Filename: {code:CaspolPath}; Parameters: "-machine -addgroup 1 -strong -file ""{app}\PopupFenster.pdb"" -noname -noversion  FullTrust -n ""{#MyAppNameKurz}"" -d ""{#MyAppNameKurz} (PopUpAnrMon.pdb) mit FullTrust"""; WorkingDir: {win}\Microsoft.NET\Framework\v2.0.50727\; Flags: waituntilidle runhidden; Check: OutlookVersion(2003); StatusMsg: "Registriere Addin für Office 2003: PopupFenster.pdb..."
  Filename: {code:CaspolPath}; Parameters: "-machine -addgroup 1 -strong -file ""{app}\Funktionen.pdb"" -noname -noversion  FullTrust -n ""{#MyAppNameKurz}"" -d ""{#MyAppNameKurz} (Funktionen.pdb) mit FullTrust"""; WorkingDir: {win}\Microsoft.NET\Framework\v2.0.50727\; Flags: waituntilidle runhidden; Check: OutlookVersion(2003); StatusMsg: "Registriere Addin für Office 2003: Funktionen.pdb..."
#endif
Filename: {code:CaspolPath}; Parameters: "-pp on"; WorkingDir: {win}\Microsoft.NET\Framework\v2.0.50727\; Flags: waituntilidle runhidden; Check: OutlookVersion(2003); StatusMsg: "Registriere Addin für Office 2003: Ende!"

;Office 2007 & 2010 & 2013
Filename: {code:VSTOInstallerPath}; Parameters: "/i ""{app}\Fritz!Box Telefon-Dingsbums.vsto"" /s "; WorkingDir: {app} ; Check: (not OutlookVersion(2003)); StatusMsg: "Installiere Fritz!Box Telefon-Dingsbums.vsto..."

[UninstallRun]
;Office 2003
Filename: {code:CaspolPath}; Parameters: "-pp off"; WorkingDir: {app}; Flags: runhidden; Check: OutlookVersion(2003); StatusMsg: "Deregistriere Addin für Office 2003: Start..."
Filename: {code:CaspolPath}; Parameters: "-rg ""FritzBoxDial"""; WorkingDir: {app}; Flags: runhidden; Check: OutlookVersion(2003); StatusMsg: "Deregistriere Addin für Office 2003: Entferne Berechtigung..."
Filename: {code:CaspolPath}; Parameters: "-pp on"; WorkingDir: {app} Flags: runhidden; Check: OutlookVersion(2003); StatusMsg: "Deregistriere Addin für Office 2003: Ende!"
 
;Office 2007 & 2010 & 2013
Filename: {code:VSTOInstallerPath}; Parameters: "/u ""{app}\Fritz!Box Telefon-Dingsbums.vsto"" /s"; WorkingDir: {app};  Check: (not OutlookVersion(2003)); StatusMsg: "Deinstalliere Fritz!Box Telefon-Dingsbums.vsto..."

[Icons]
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Code]
var Versionspfad:String;
var Version:String;

var inst_dotnetfx40client:boolean;
var inst_dotnetfx35:boolean;
var inst_dotnetfx35SP1:boolean;
var inst_VSTO2005SE_Redistributable:boolean;
var inst_VSTO2010_Redistributable:boolean;
var inst_o2003pia:boolean;

const	dotnetfx40client_url = 'http://download.microsoft.com/download/7/B/6/7B629E05-399A-4A92-B5BC-484C74B5124B/dotNetFx40_Client_setup.exe';
const dotnetfx35_url = 'http://download.microsoft.com/download/7/0/3/703455ee-a747-4cc8-bd3e-98a615c3aedb/dotNetFx35setup.exe';
const dotnetfx35sp1_url = 'http://download.microsoft.com/download/0/6/1/061f001c-8752-4600-a198-53214c69b51f/dotnetfx35setup.exe';
const VSTO2005SE_Redistributable_url = 'http://download.microsoft.com/download/1/6/b/16ba60f5-d478-4d22-a695-203003494477/vstor.exe';
const VSTO2010_Redistributable_url = 'http://go.microsoft.com/fwlink/?LinkId=158918';
const o2003pia_url = 'http://download.microsoft.com/download/8/3/a/83a40b5a-5050-4940-bcc4-7943e1e59590/O2003PIA.EXE';

procedure InitializeWizard();
begin
  ITD_Init;
  ITD_DownloadAfter(wpReady);  
end;

function OutlookVersion (Get:Integer): boolean;
  begin
    if StrToInt(Version) = Get then
    begin
      Result := true
      exit
    end
    else Result:= false;
end;

function CurrectGUID(dummy: String): String;
begin
  Result := '{' + '{#myGUID}' +  '}'
end;
 
function PrepareToInstall(var NeedsRestart: Boolean): String;
var
ResultCode : Integer;
begin
  if inst_dotnetfx40client then
	begin
    ShellExec('open', ExpandConstant('{tmp}\dotNetFx40_Client_setup.exe'), '/q /passive /norestart', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
  end

  if  inst_dotnetfx35 then
  begin
    ShellExec('open', ExpandConstant('{tmp}\dotNetFx35setup.exe'), '/qb /norestart', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
  end
	if  inst_dotnetfx35 then
  begin
    ShellExec('open', ExpandConstant('{tmp}\dotNetFx35setupSP1.exe'), '/qb /norestart', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
  end

  if  inst_VSTO2005SE_Redistributable then
  begin
    ShellExec('open', ExpandConstant('{tmp}\vstor.exe'), '/q /norestart', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
  end

  if  inst_VSTO2010_Redistributable then
  begin
    ShellExec('open', ExpandConstant('{tmp}\vstor_redist.exe'), '/q /norestart', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
  end

  if  inst_o2003pia then
  begin
    MsgBox('Die Primary Interop Assemblies (PIA) für Microsoft Office 2003 können nicht automatisch installiert werden. Bitte Installieren Sie diese nach der Installation von Hand.', mbInformation, MB_OK);
		ShellExec('open', ExpandConstant('{tmp}\O2003PIA.EXE'), '', '', SW_SHOWNORMAL, ewNoWait, ResultCode);
  end
end;

function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, release, serviceCount: cardinal;
    check45, success: boolean;
begin
    // .NET 4.5 installs as update to .NET 4.0 Full
    if version = 'v4.5' then begin
        version := 'v4\Full';
        check45 := true;
    end else
        check45 := false;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0/4.5 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 uses additional value Release
    if check45 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= 378389);
    end;

    Result := success and (install = 1) and (serviceCount >= service);
end;

function GetOutlookVersion(): String;
  var Versionsnr,n :Integer;
  begin
  Versionsnr := 0;
  if RegQueryStringValue(HKLM,'SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE','', Versionspfad) then
    begin
    GetVersionNumbersString(Versionspfad,Version);
    n:= Pos('.',Version)-1;
    Versionsnr := StrToInt(Copy(Version,0,n));
    CASE Versionsnr OF
      11: Version := '2003';
      12: Version := '2007';
      14: Version := '2010';
      15: Version := '2013';
    END; // CASE

    Result:= Version;
    // Prüfen welche Version vorliegt
  end;
end;

function Outlookx64: boolean;
  var x86, RegOutlook: String;
  begin

    CASE StrToInt(GetOutlookVersion) OF
      2010: RegOutlook := 'SOFTWARE\Microsoft\Office\14.0\Outlook';
      2013: RegOutlook := 'SOFTWARE\Microsoft\Office\15.0\Outlook';
    END; // CASE

    if RegQueryStringValue(HKLM,RegOutlook,'Bitness', x86) then
    begin
      if x86 = 'x64' then
      begin    
        Result := true
        exit
      end
    end
    else result:= false;
end;

function CaspolPath(dummy: String): String;
var
  Pfad, strNET, key: String;
begin
    strNET := 'v2.0.50727';
    
    if IsDotNetDetected(strNet, 0) then
    begin
      key := 'SOFTWARE\Microsoft\.NETFramework'
      
      if RegQueryStringValue(HKLM, key, 'InstallRoot', Pfad) then
      begin
        Pfad := Pfad + strNET + '\CasPol.exe'
      
        if FileExists(Pfad) then
        begin
          Result := Pfad
          exit
        end
        else
        begin
          MsgBox('Die Datei CasPol.exe wurde nicht gefunden.', mbError, MB_OK);
          result := 'false'
        end 
      end
      else
      begin
        MsgBox('Der Registryeintrag InstallRoot wurde nicht gefunden.', mbError, MB_OK);
        Result := 'false'
      end
    end
    else
    begin
      MsgBox('.NET v2.0.50727 wurde nicht gefunden.', mbError, MB_OK);
      result := 'false'
    end
end;

function VSTOInstallerPath(dummy: String): String;
var
  Pfad, key: String;
begin
    if IsWin64 then	
    begin
		  key := 'SOFTWARE\Wow6432Node\Microsoft\VSTO Runtime Setup\v4'
    end
    else
			key := 'SOFTWARE\Microsoft\VSTO Runtime Setup\v4'
    begin
    end
		
    if RegQueryStringValue(HKLM, key, 'InstallerPath', Pfad) then
      begin
      
				if FileExists(Pfad) then
        begin
          Result := Pfad
          exit
        end
        else
        begin
          MsgBox('Die Datei VSTOInstaller.exe wurde nicht gefunden.', mbError, MB_OK);
          Result := 'false' 
        end 
      end
    else
			begin
        MsgBox('Der Registryeintrag InstallerPath wurde nicht gefunden.', mbError, MB_OK);
        Result := 'false'
    end
		
end;

function IsRegularUser(): Boolean;
  begin
  Result := not (IsAdminLoggedOn or IsPowerUserLoggedOn);
end;

function DefDirRoot(Param: String): String;
  begin
  if IsRegularUser then
    Result := ExpandConstant('{localappdata}')
  else
  Result := ExpandConstant('{pf}')
end;

// found at: https://stackoverflow.com/questions/2000296/innosetup-how-to-automatically-uninstall-previous-installed-version
/////////////////////////////////////////////////////////////////////
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
  begin
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  end
  
  if (IsWin64) and (sUnInstallString = '') then
  begin
    sUnInstPath := ExpandConstant('Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
    if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    begin
      RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
    end
  end 
  Result := sUnInstallString;
end;
 
/////////////////////////////////////////////////////////////////////
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;

/////////////////////////////////////////////////////////////////////
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

/////////////////////////////////////////////////////////////////////
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;

function InitializeSetup(): Boolean;
  var
    strNET, strNET2, strERR:String;
    tmpInt: Integer;
    VSTORFeature: Cardinal;
  begin
    Version:= GetOutlookVersion;
    tmpInt:= StrToInt(Version);
    Result := true;

    CASE tmpInt OF
      2003: 
      begin 
        if CaspolPath('') = 'false' then
        begin
          Result := false
        end
        // Prüfe auf Microsoft Visual Studio 2005 Tools for Office Second Edition Runtime (VSTO 2005 SE) (x86)
        if RegQueryDWordValue(HKLM,'SOFTWARE\Microsoft\VSTO Runtime Setup\v2.0.50727','Install', VSTORFeature) then
        begin 
          if VSTORFeature = 0 then
          begin    
            Result := false
          end
        else
          begin
            Result := false
          end 
        end 
      end; 
      2007, 2010, 2013: 
      begin 

        // Prüfe auf VSTO 2010
        if Outlookx64 then 
        begin 
          if RegQueryDWordValue(HKLM,'SOFTWARE\Wow6432Node\Microsoft\VSTO Runtime Setup\v4R','VSTORFeature_CLR40', VSTORFeature) then
          begin 
            if VSTORFeature = 0 then
            begin    
              Result := false
            end 
          end
          else
          begin
            Result := false
          end  
        end
        else
        begin 
          if RegQueryDWordValue(HKLM,'SOFTWARE\Microsoft\VSTO Runtime Setup\v4R','VSTORFeature_CLR40', VSTORFeature) then
          begin
            if VSTORFeature = 0 then
            begin    
              Result := false
            end 
          end
          else
          begin
            Result := false
          end 
        end
                  
        if not Result then 
        begin
          Result:=false;
          if (tmpInt = 2003) then
            begin
              inst_VSTO2005SE_Redistributable := true;
            end
          else
            begin
              inst_VSTO2010_Redistributable := true;
            end
        end 
      end;  
    END;

    if tmpInt = 2003 then
    begin
		  strNET := 'v3.5';
      strNET2 := '.NET Framework 3.5';
			if not IsDotNetDetected(strNet, 0) then
			begin 
        inst_dotnetfx35 := true;
				inst_dotnetfx35SP1 := true;					
			end	
			else if not IsDotNetDetected(strNet, 1) then
			begin
				inst_dotnetfx35SP1 := true;
			end
			//PIA
      Result := RegKeyExists(HKLM, 'SOFTWARE\Classes\Installer\Features\9040941900063D11C8EF10054038389C');
      if not Result then
      begin
        inst_o2003pia := true;
      end;
    end
		else
		begin
			strNET := 'v4\Client';
      strNET2 := '.NET Framework 4.0 Client Profile';
			if not IsDotNetDetected(strNet, 0) then
			begin
			  inst_dotnetfx40client := true;
			end 
		end			 

    if Not Result then
    begin
      strERR := 'Folgende Komponenten werden von {#MyAppName} benötigt, wurden aber auf Ihrem Rechner nicht gefunden:'#13#10' '#13#10'';
      if inst_dotnetfx35 or inst_dotnetfx40client then
      begin
        strERR := strERR+ 'Microsoft ' + strNET2 + ''#13#10''
      end
			if inst_dotnetfx35SP1 then
      begin
        strERR := strERR+ 'Microsoft .NET Framework 3.5 Service Pack 1'#13#10''
      end
      if inst_VSTO2005SE_Redistributable then
      begin
        strERR := strERR + 'Microsoft Visual Studio 2005 Tools for Office Second Edition Runtime (VSTO 2005 SE)'#13#10''
      end
      if inst_VSTO2010_Redistributable then
      begin
        strERR := strERR + 'Microsoft Visual Studio 2010-Tools für Office (VSTO 2010)'#13#10''
      end
      if inst_o2003pia then
      begin
        strERR := strERR + 'Primary Interop Assemblies (PIA) für Microsoft Office 2003'#13#10''
      end
      strERR := strERR + #13#10 + 'Sollen die fehlenden Komponenten heruntergeladen und installiert werden?'


      if MsgBox(strERR, mbConfirmation, MB_YESNO) = IDYES then
        begin
        if inst_dotnetfx35 or inst_dotnetfx40client then
        begin
          if strNET = 'v3.5' then
            begin
            ITD_AddFileSize(dotnetfx35_url, ExpandConstant('{tmp}\dotNetFx35setup.exe'),2869264);
            end 
          else if strNET = 'v4\Client' then 
            begin
            ITD_AddFileSize(dotnetfx40client_url, ExpandConstant('{tmp}\dotNetFx40_Client_setup.exe'),43000680);
          end
        end
        if inst_dotnetfx35SP1 then
				begin
					ITD_AddFileSize(dotnetfx35sp1_url, ExpandConstant('{tmp}\dotNetFx35setupSP1.exe'),2961408)
				end
        if inst_VSTO2005SE_Redistributable then
        begin
          ITD_AddFileSize(VSTO2005SE_Redistributable_url, ExpandConstant('{tmp}\vstor.exe'),1333432);
        end
        if inst_VSTO2010_Redistributable then
        begin
          ITD_AddFileSize(VSTO2010_Redistributable_url, ExpandConstant('{tmp}\vstor_redist.exe'),40029664);
        end
        if inst_o2003pia then
        begin
          ITD_AddFileSize(o2003pia_url, ExpandConstant('{tmp}\O2003PIA.EXE'),4329472);
        end
        Result := true
      end
    end
end;


//EOF