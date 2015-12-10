Fritz!Box Telefon-dingsbums
=========================

Fritz!Box Telefon-dingsbums

1.1  Voraussetzungen für alle Versionen

- Fritz!Box
- Der interne Anrufmonitor der Fritz!Box muss eingeschaltet sein  (Telefoncode zum Einschalten: #96\*5\*).
- Für ein korrektes Wählen ist es wichtig, dass der Haken „Wählhilfe verwenden“ in der Fritz!Box gesetzt wurde (Telefonbuch/Wählhilfe).
- Bei Anmeldung mit Username und Benutzername an der Fritz!Box, muss der entsprechende Fritz!User BoxAdmin sein.


Alle weiteren Komponenten (.NET Framework, VSTO, PIA), die das Addin benötigen, hängen von der jeweiligen Office Version ab. Die Installation lädt die Kompo-nenten automatisch runter und versucht sie zu installieren. Wenn es Probleme damit gibt, dann Installieren Sie die Komponenten per Hand vor der Installation des Addins.

1.2	Systemvoraussetzungen für Outlook 365, Outlook 2016, Outlook 2013, Outlook 2010, Outlook 2007

- Microsoft Outlook 365 oder Outlook 2013 oder Outlook 2010 oder Outlook 2007
- Microsoft .NET Framework 4.0 Client Profile: http://www.microsoft.com/de-de/download/details.aspx?id=24872 
- Microsoft Visual Studio 2010 Tools for Office Runtime Redistributable (VSTOR 2010): http://go.microsoft.com/fwlink/?LinkId=158918

Das Programm wurde erfolgreich in den folgenden Konstellationen getestet:

- Windows 10 x64 & Office 2010 x64, Windows 10 x64 & Office 2016 x32
- Windows 8.1 x64 & Office 2010 x86, Windows 8.1 x64 & Office 2013 x64
- Windows 7 x64 & Office 2010 x64, Windows 7 x86 & Office 2010 x86
- Windows XP SP3 (Entwicklungsrechner)
- Outlook 365 (kein direkter Support möglich)

1.3	Systemvoraussetzungen für Outlook 2003

- Microsoft Outlook 2003
- Microsoft .Net Framework 3.5: http://www.microsoft.com/downloads/details.aspx?displaylang=de&FamilyID=333325fd-ae52-4e35-b531-508d977d32a6
- Eventuell: Microsoft .Net Framework 3.5 SP1: http://www.microsoft.com/downloads/de-de/details.aspx?FamilyID=AB99342F-5D1A-413D-8319-81DA479AB0D7
- Redistributable Primary Interop Assemblies für Office 2003: http://www.microsoft.com/downloads/details.aspx?familyid=3c9a983a-ac14-4125-8ba0-d36d67e0f4ad&displaylang=en
- Microsoft Visual Studio 2005 Tools for Office Second Edition Runtime (VSTO 2005 SE) (x86): http://www.microsoft.com/DOWNLOADS/details.aspx?displaylang=de&FamilyID=f5539a90-dc41-4792-8ef8-f4de62ff1e81

Das Programm wurde erfolgreich in den folgenden Konstellationen getestet:

Windows XP SP3 (Entwicklungsrechner VS2008)

1.4	Kontaktmöglichkeiten:
Trotz sorgfältiger Überprüfung können Fehler nicht ausgeschlossen werden.
Sofern jemand noch Fehler findet, bitte eine detaillierte Fehlerbeschreibung erstellen und diese hier bei github als Issue hinzufügen. Ansonsten kann es passieren, dass der Fehler ewig im Programm verbleibt, da er niemand anderem aufgefallen ist.

1.	IPPF (Anmeldung erforderlich): http://www.ip-phone-forum.de/showthread.php?t=237086 
2.	E-Mail:  kruemelino@gert-michael.de

1.5	Updateinformationen
Bevor eine neue Version aufgespielt werden kann, muss die alte vorher entfernt werden. Dies übernimmt der Installer (ab Version 3.7). Es ist dennoch ratsam zu prüfen, ob die alte Version tatsächlich entfernt wurde. 
Bei einem Update von einer älteren Version muss über die Systemsteuerung das alte Addin zuerst deinstalliert werden. Erst danach kann eine neue Version aufgespielt werden.

1.6	Lizenzinformationen
-	Der Anrufmonitor basiert auf dem "An Office 2003-like popup notifier" von Nicolas Wälti welches unter The Code Project Open License (CPOL) liziensiert ist.
Nähere Informationen zu diesem Open Source Projekt finden Sie auf der Projektseite:
http://www.codeproject.com/KB/cpp/PopupNotifier.aspx 
