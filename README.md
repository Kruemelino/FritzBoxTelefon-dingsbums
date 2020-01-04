!Achtung! Dieses Programm wird in dieser Form nicht mehr weiterentwickelt. Es gibt bereits einen neuen Branch zu einer neuen Programmversion.

Fritz!Box Telefon-dingsbums
===========================

!Achtung! Die Unterstützung für Office 2003 und 2007 ist bis auf weiteres eingestellt.

1.1  Voraussetzungen für alle Versionen

- Fritz!Box
- Der interne Anrufmonitor der Fritz!Box muss eingeschaltet sein  (Telefoncode zum Einschalten: #96\*5\*).
- Für ein korrektes Wählen ist es wichtig, dass der Haken „Wählhilfe verwenden“ in der Fritz!Box gesetzt wurde (Telefonbuch/Wählhilfe).
- Die Zwei-Wege-Authentifizierung der Fritz!Box muss deaktiviert werden. Ansonsten ist das Wählen mit den ausgewählten Telefon nicht möglich.
- Bei Anmeldung mit Username und Benutzername an der Fritz!Box, muss der entsprechende Fritz!User BoxAdmin sein.

Die Installation lädt alle weiteren Komponenten (.NET Framework, VSTO) automatisch runter und versucht sie zu installieren. Wenn es Probleme damit gibt, dann installieren Sie die Komponenten per Hand vor der Installation des Addins.

1.2	Systemvoraussetzungen für Outlook 365, Outlook 2019, Outlook 2016, Outlook 2013, Outlook 2010

- Microsoft Outlook 365, Outlook 2019 oder Outlook 2016 oder Outlook 2013 oder Outlook 2010 
- Microsoft .NET Framework 4.7.2: https://www.microsoft.com/net/download/dotnet-framework-runtime
- Microsoft Visual Studio 2010 Tools for Office Runtime Redistributable (VSTOR 2010): http://go.microsoft.com/fwlink/?LinkId=158918

Das Programm wurde erfolgreich in den folgenden Konstellationen getestet:

- Windows 10 x64 & Office 2010 x64, Windows 10 x64 & Office 2016 x32
- Windows 8.1 x64 & Office 2010 x86, Windows 8.1 x64 & Office 2013 x64
- Windows 7 x64 & Office 2010 x64, Windows 7 x86 & Office 2010 x86
- Outlook 365 (kein direkter Support möglich)

1.3	Kontaktmöglichkeiten:
Trotz sorgfältiger Überprüfung können Fehler nicht ausgeschlossen werden.
Sofern jemand noch Fehler findet, bitte eine detaillierte Fehlerbeschreibung erstellen und diese hier bei github als Issue hinzufügen. Ansonsten kann es passieren, dass der Fehler ewig im Programm verbleibt, da er niemand anderem aufgefallen ist.

1.	IPPF (Anmeldung erforderlich): http://www.ip-phone-forum.de/showthread.php?t=237086 
2.	E-Mail:  kruemelino@gert-michael.de

1.4	Updateinformationen
Bevor eine neue Version aufgespielt werden kann, muss die alte vorher entfernt werden. Dies übernimmt der Installer (ab Version 3.7). Es ist dennoch ratsam zu prüfen, ob die alte Version tatsächlich entfernt wurde. 
Bei einem Update von einer älteren Version muss über die Systemsteuerung das alte Addin zuerst deinstalliert werden. Erst danach kann eine neue Version aufgespielt werden.

1.5	Lizenzinformationen
-	Der Anrufmonitor basiert auf dem "An Office 2003-like popup notifier" von Nicolas Wälti welches unter The Code Project Open License (CPOL) liziensiert ist.
Nähere Informationen zu diesem Open Source Projekt finden Sie auf der Projektseite:
http://www.codeproject.com/KB/cpp/PopupNotifier.aspx 
