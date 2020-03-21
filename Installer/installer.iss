#include ReadReg(HKEY_LOCAL_MACHINE,'Software\Sherlock Software\InnoTools\Downloader','ScriptPath','')
#define MyAppName "Fritz!Box Telefon-dingsbums"
#define MyAppVersion "5.0.0"
#define MyAppPublisher "Kruemelino"
#define MyAppURL "https://github.com/Kruemelino/FritzBoxTelefon-dingsbums"
#define MyAppDescription "Das Fritz!Box Telefon-dingsbums ist ein Addin für Outlook (2010-2019), welches ein direktes Wählen der Kontakte aus dem Computer ermöglicht. Zusätzlich bietet es nützliche Funktionen, wie einen Anrufmonitor oder eine Rückwärtssuche."
#define MyGUID "051D5E77-4942-477E-8071-12F262FDE4F3" 
#define MyAppNameKurz "FritzBoxDial"
#define MyAppTime GetDateTimeString('yymmdd', '', '') 
#define MyAppType ""

[Setup]
AppId = {{051D5E77-4942-477E-8071-12F262FDE4F3}}
AppName = {#MyAppName} {#MyAppType}
AppVersion = {#MyAppVersion}
AppPublisher = {#MyAppPublisher}
AppPublisherURL = {#MyAppURL}
AppSupportURL = {#MyAppURL}
AppUpdatesURL = {#MyAppURL}
DefaultDirName = {code:DefDirRoot}\{#MyAppName}
DefaultGroupName = {#MyAppName}
DisableProgramGroupPage = yes
OutputBaseFilename = FBDBSetup_{#MyAppVersion}_{#MyAppTime}
Compression = lzma2
SolidCompression = yes
PrivilegesRequired = none
SignTool = WinSDK /t $qhttp://timestamp.verisign.com/scripts/timstamp.dll$q /du $q{#MyAppURL}$q /d $q{#MyAppDescription}$q $f
SignedUninstaller = yes
WizardStyle = modern

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Registry]
; Office 64bit
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: Outlookx64;       Flags: uninsdeletekey; ValueType: string; ValueName: "Description";   ValueData: "{#MyAppDescription}"
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: Outlookx64;       Flags: uninsdeletekey; ValueType: string; ValueName: "FriendlyName";  ValueData: "{#MyAppName}"
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: Outlookx64;       Flags: uninsdeletekey; ValueType: dword;  ValueName: "LoadBehavior";  ValueData: "3"
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: Outlookx64;       Flags: uninsdeletekey; ValueType: string; ValueName: "Manifest";      ValueData: "file:///{app}/Fritz!Box Telefon-Dingsbums.vsto|vstolocal"

; Office 32bit
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: not Outlookx64;   Flags: uninsdeletekey; ValueType: string; ValueName: "Description";    ValueData: "{#MyAppDescription}"
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: not Outlookx64;   Flags: uninsdeletekey; ValueType: string; ValueName: "FriendlyName";   ValueData: "{#MyAppName}"
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: not Outlookx64;   Flags: uninsdeletekey; ValueType: dword;  ValueName: "LoadBehavior";   ValueData: "3"
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\Fritz!Box Telefon-Dingsbums"; Check: not Outlookx64;   Flags: uninsdeletekey; ValueType: string; ValueName: "Manifest";       ValueData: "file:///{app}/Fritz!Box Telefon-Dingsbums.vsto|vstolocal"

[Files]
#if FileExists("..\FBoxDial\bin\Debug2010\Fritz!Box Telefon-Dingsbums.dll")
     Source: "..\FBoxDial\bin\Debug2010\Fritz!Box Telefon-Dingsbums.dll";                     Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion
     Source: "..\FBoxDial\bin\Debug2010\Fritz!Box Telefon-Dingsbums.dll.config";              Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion
     Source: "..\FBoxDial\bin\Debug2010\Fritz!Box Telefon-Dingsbums.dll.manifest";            Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion
     Source: "..\FBoxDial\bin\Debug2010\Fritz!Box Telefon-Dingsbums.vsto";                    Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion
     Source: "..\FBoxDial\bin\Debug2010\Microsoft.Office.Tools.Common.v4.0.Utilities.dll";    Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion
     Source: "..\FBoxDial\bin\Debug2010\Microsoft.Office.Tools.Outlook.v4.0.Utilities.dll";   Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion 
     Source: "..\FBoxDial\bin\Debug2010\MixERP.Net.VCards.dll";                               Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion
     Source: "..\FBoxDial\bin\Debug2010\Newtonsoft.Json.dll";                                 Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion
     Source: "..\FBoxDial\bin\Debug2010\NLog.dll";                                            Check: OutlookVersion2010;     DestDir: "{app}"; Flags: ignoreversion
#endif

#if FileExists("..\FBoxDial\bin\Debug2013\Fritz!Box Telefon-Dingsbums.dll")
    Source: "..\FBoxDial\bin\Debug2013\Fritz!Box Telefon-Dingsbums.dll";                      Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion
    Source: "..\FBoxDial\bin\Debug2013\Fritz!Box Telefon-Dingsbums.dll.config";               Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion
    Source: "..\FBoxDial\bin\Debug2013\Fritz!Box Telefon-Dingsbums.dll.manifest";             Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion
    Source: "..\FBoxDial\bin\Debug2013\Fritz!Box Telefon-Dingsbums.vsto";                     Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion
    Source: "..\FBoxDial\bin\Debug2013\Microsoft.Office.Tools.Common.v4.0.Utilities.dll";     Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion
    Source: "..\FBoxDial\bin\Debug2013\Microsoft.Office.Tools.Outlook.v4.0.Utilities.dll";    Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion 
    Source: "..\FBoxDial\bin\Debug2013\MixERP.Net.VCards.dll";                                Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion
    Source: "..\FBoxDial\bin\Debug2013\Newtonsoft.Json.dll";                                  Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion
    Source: "..\FBoxDial\bin\Debug2013\NLog.dll";                                             Check: OutlookVersion2013Plus; DestDir: "{app}"; Flags: ignoreversion
#endif

[Icons]
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Code]
var Versionspfad:String;
var Version:String;

var inst_dotnetfx:boolean;
var inst_VSTO2010_Redistributable:boolean;

const dotnetfx_url = 'https://go.microsoft.com/fwlink/?linkid=2088631';
const VSTO2010_Redistributable_url = 'https://go.microsoft.com/fwlink/?LinkId=158918';

// function OutlookVersion (Get:Integer): boolean;
//     begin
//         Result := StrToInt(Version) = Get
// end;

function OutlookVersion2010: boolean;
    begin
        Result := StrToInt(Version) = 2010
end;

function OutlookVersion2013Plus: boolean;
    begin
        Result := StrToInt(Version) >= 2013
end;

function CurrectGUID(dummy: String): String;
begin
    Result := '{' + '{#myGUID}' +  '}'
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
    var
    ResultCode : Integer;
   
    begin
        if inst_dotnetfx then
            Result := '';
            
        begin
            ShellExec('open', ExpandConstant('{tmp}\ndp48-x86-x64-allos-enu.exe'), '/q /passive /norestart', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
        end;

        if inst_VSTO2010_Redistributable then
            begin
            ShellExec('open', ExpandConstant('{tmp}\vstor_redist.exe'), '/q /norestart', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
        end;
end;

function GetHKLM: Integer;
// Check IsWin64 before using a 64-bit-only feature to
// avoid an exception when running on 32-bit Windows.
begin
  if IsWin64 then
    Result := HKLM64
  else
    Result := HKLM32;
end;

// http://kynosarges.org/DotNetVersion.html
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1'          .NET Framework 1.1
//    'v2.0'          .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6'          .NET Framework 4.6
//    'v4.6.1'        .NET Framework 4.6.1
//    'v4.6.2'        .NET Framework 4.6.2
//    'v4.7'          .NET Framework 4.7
//    'v4.7.1'        .NET Framework 4.7.1
//    'v4.7.2'        .NET Framework 4.7.2
//    'v4.8'          .NET Framework 4.8
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
function IsDotNetDetected(version: string; service: cardinal): boolean;
var
    key, versionKey: string;
    install, release, serviceCount, versionRelease: cardinal;
    success: boolean;
begin
    versionKey := version;
    versionRelease := 0;

    // .NET 1.1 and 2.0 embed release number in version key
    if version = 'v1.1' then begin
        versionKey := 'v1.1.4322';
    end else if version = 'v2.0' then begin
        versionKey := 'v2.0.50727';
    end

    // .NET 4.5 and newer install as update to .NET 4.0 Full
    else if Pos('v4.', version) = 1 then begin
        versionKey := 'v4\Full';
        case version of
          'v4.5':   versionRelease := 378389;
          'v4.5.1': versionRelease := 378675; // 378758 on Windows 8 and older
          'v4.5.2': versionRelease := 379893;
          'v4.6':   versionRelease := 393295; // 393297 on Windows 8.1 and older
          'v4.6.1': versionRelease := 394254; // 394271 before Win10 November Update
          'v4.6.2': versionRelease := 394802; // 394806 before Win10 Anniversary Update
          'v4.7':   versionRelease := 460798; // 460805 before Win10 Creators Update
          'v4.7.1': versionRelease := 461308; // 461310 before Win10 Fall Creators Update
          'v4.7.2': versionRelease := 461808; // 461814 before Win10 April 2018 Update
          'v4.8':   versionRelease := 528040; // 528049 before Win10 May 2019 Update
        end;
    end;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + versionKey;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0 and newer use value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 and newer use additional value Release
    if versionRelease > 0 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= versionRelease);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;

function GetOutlookVersion(): String;
    var Versionsnr,n :Integer;
    begin
        Versionsnr := 0;
        if RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE', '', Versionspfad) then
        begin
            GetVersionNumbersString(Versionspfad, Version);
            n := Pos('.', Version) -1;
            Versionsnr := StrToInt(Copy(Version, 0, n));
            CASE Versionsnr OF
                14: Version := '2010';
                15: Version := '2013';
                16: Version := '2016';
                17: Version := '2019'
            END; // CASE

    Result := Version;    
    end
end;

function Outlookx64: boolean;
    var x86, RegOutlook: String;
    begin

    // Bei Office 365 ist es der Registrypfad: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\ClickToRun\Configuration
    RegOutlook := 'SOFTWARE\Microsoft\Office\ClickToRun\Configuration';
    // Alternative: Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\ClickToRun\REGISTRY\MACHINE\Software\Microsoft\Office\16.0\Outlook
    // RegOutlook := 'SOFTWARE\Microsoft\Office\ClickToRun\REGISTRY\MACHINE\Software\Microsoft\Office\16.0\Outlook';
    // Prüfe, ob es eine ClickToRun-Version (365) ist.
    
    
    if RegQueryStringValue(GetHKLM, 'SOFTWARE\Microsoft\Office\ClickToRun\Configuration', 'Platform', x86) then
    //if RegQueryStringValue(HKLM, RegOutlook, 'ClientCulture', x86) then
        begin
          Result := x86 = 'x64';
        end
    else         
        begin
           CASE StrToInt(GetOutlookVersion) OF
               2010: RegOutlook := 'SOFTWARE\Microsoft\Office\14.0\Outlook';
               2013: RegOutlook := 'SOFTWARE\Microsoft\Office\15.0\Outlook';
               2016: RegOutlook := 'SOFTWARE\Microsoft\Office\16.0\Outlook';
               2019: RegOutlook := 'SOFTWARE\Microsoft\Office\17.0\Outlook';
           END; // CASE
     
           if RegQueryStringValue(GetHKLM, RegOutlook, 'Bitness', x86) then 
               Result := x86 = 'x64'                      
           else 
               result := false;
           
        end;
end;

function IsRegularUser(): Boolean;
  begin
  Result := not (IsAdmin or IsPowerUserLoggedOn);
end;

function DefDirRoot(Param: String): String;
    begin
    if IsRegularUser then Result := ExpandConstant('{localappdata}')
    else Result := ExpandConstant('{pf}')
end;

function InitializeSetup(): Boolean;
    var
        strNET, strNET2, strERR:String;
        VSTORFeature: Cardinal;
    begin
        Version:= GetOutlookVersion;
        Result := true;

    // Minimale erforderliche .NET Version
    strNET := 'v4.8';
    strNET2 := '.NET Framework 4.8';

    // Prüfe, ob mindestens Office 2010 installiert ist
    if StrToInt(Version) >= 2010 then
        begin
          // Prüfe, ob VSTO installiert ist
          if IsWin64 then // Handelt es sich um eine 64 bit-Version
          // if Outlookx64 then // Handelt es sich um eine 64 bit-Version von Outlook (alt scheint nicht mehr relevant zusein.)
              begin // Eine 64-bit Version wurde gefunden
                  if RegQueryDWordValue(GetHKLM,'SOFTWARE\Wow6432Node\Microsoft\VSTO Runtime Setup\v4R','VSTORFeature_CLR40', VSTORFeature) then
                      Result := VSTORFeature = 1             
                  else            
                      Result := false              
                  end
              else // Eine 32-bit Version wurde gefunden
                  begin
                      if RegQueryDWordValue(GetHKLM,'SOFTWARE\Microsoft\VSTO Runtime Setup\v4R','VSTORFeature_CLR40', VSTORFeature) then
                          Result := not VSTORFeature = 1
                      else                
                          Result := false                 
              end;
          
          if not Result then 
              begin
                  Result := false
                  inst_VSTO2010_Redistributable := true            
              end; 
      
          // Prüfe, ob .NET 4.8 installiert ist    
          if not IsDotNetDetected(strNet, 0) then
              begin
                  Result := false
                  inst_dotnetfx := true
          end;
          
          if Not Result then
              begin
                  strERR := 'Folgende Komponenten werden von {#MyAppName} benötigt, wurden aber auf Ihrem Rechner nicht gefunden:'#13#10' '#13#10'';
              
                  // .NET 4.8
                  if inst_dotnetfx then strERR := strERR+ 'Microsoft ' + strNET2 + ''#13#10'';                 
                  // VSTO
                  if inst_VSTO2010_Redistributable then strERR := strERR + 'Microsoft Visual Studio 2010-Tools für Office (VSTO 2010)'#13#10'';
   
                  strERR := strERR + #13#10 + 'Sollen die fehlenden Komponenten heruntergeladen und installiert werden?'

                  if MsgBox(strERR, mbConfirmation, MB_YESNO) = IDYES then
                      begin
                          if inst_dotnetfx then
                              ITD_AddFileSize(dotnetfx_url, ExpandConstant('{tmp}\ndp48-x86-x64-allos-enu.exe'),43000680);                        

                          if inst_VSTO2010_Redistributable then
                              ITD_AddFileSize(VSTO2010_Redistributable_url, ExpandConstant('{tmp}\vstor_redist.exe'),40029664);
                          Result := true
                      end
              end



        end 
    else
        begin
        // Outlook ist nicht installiert
        strERR := 'Microsoft Outlook wurde auf Ihrem Rechner nicht gefunden. {#MyAppName} kann nicht installiert werden.';
        Result := false
    end;
end; 		 
