# Fritz!Box Telefon-dingsbums V5

Dieses Projekt ist ein Addin f�r Microsoft Outlook. 
Das Projekt ist in den ersten Versionen bereits 2006 entstanden und wurde �ber die Jahre von einer Makrosammlung hin zu einem Office Addin �berf�hrt.

Dieses Addin ist in meiner Freizeit entstanden. Ich erwarte keine Gegenleistung. Ein Danke ist ausreichend.
### Zielsetzung
Ziel ist es, die Produkte der Firma AVM (Fritz!Box) aus Berlin besser mit Outlook zu verkn�pfen. Hierbei soll nur auf die Standardfunktionen der Fritz!Box zur�ckgegriffen werden,
d.h. keine Modifikationen an der Fritz!Box m�ssen durchgef�hrt werden.

### Funktionsumfang
#### Unterst�tze Funktionen (was geht)
* Starten von Telefonaten
* Signalisierung eingehender Anrufer
* Anzeige verf�gbarer Informationen zum Gespr�chspartner
* Ermittlung der Kontaktdaten erfolgt verschiedenen Quellen
  * Outlook Addressb�chern
  * Telefonb�cher der Fritz!Box
  * R�ckw�rtssuche
* Protokollierung aller Telefonate
* Signalisierung der aktuellen Gespr�chsdauer (Stoppuhr)
* Datentransfer zwischen Outlook und den Fritz!Box Telefonb�chern (aktuell noch im Aufbau)
* Nutzung der Softwaretelefonen ([Phoner](https://phoner.de) und [MicroSIP](https://www.microsip.org)) (PhonerLite wird nicht unterst�tzt)
* R�ckruf- und Wahlwiederholungsliste
* VIP-Liste f�r h�ufig anzurufende Kontakte
#### Ausschl�sse (was nicht geht)
* Nutzung von angeschlossenen IP-Telefonen. Hier fehlt einfach die Unterst�tzung der Fritz!Box. Einzige Ausnahmen sind die Programme [Phoner](https://phoner.de) und [MicroSIP](https://www.microsip.org/), 
  da diese eine eigene Schnittstelle haben. 

### Vorraussetzungen
F�r die Nutzung des Addins wird ben�tigt: 
1. eine (halbwegs) aktuelle AVM Fritz!Box mit Telefoniefunktion.
   1. Damit dieses Addin korrekt arbeiten kann, muss ein Nutzer mit Administratorrechten darauf zugreifen.
   2. Der Fritz!Box CallMonitor muss aktiviert sein: Hierzu den Telefoncode `#96*5*` per Telefon w�hlen.
   3. Zur Kommunikation mit der Fritz!Box wird u. a. die TR064-Schnittstelle verwendet. Daher darf diese nicht deaktiviert werden.
2. Microsoft Outlook (2010 bis 2019 inkl. 365)
3. Gegebenenfalls vorab installieren:
   1. [Microsoft .NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
   2. [Microsoft Visual Studio 2010 Tools for Office Runtime Redistributable (VSTO 2010)](https://www.microsoft.com/de-DE/download/details.aspx?id=48217)

### Links
Forum f�r Hilfestellungen ([IP Phone Forum](https://www.ip-phone-forum.de/)): [Fritz!Box Telefon-dingsbums (Ein MS Outlook Addin)](https://www.ip-phone-forum.de/threads/fritz-box-telefon-dingsbums-ein-ms-outlook-addin.237086/)

### Quellen
Urspr�ngliche Thread von 2006 im [IP Phone Forum](https://www.ip-phone-forum.de/): [Outlook W�hlhilfe (Makro) ohne Box zu modifizieren](https://www.ip-phone-forum.de/threads/outlook-w%C3%A4hlhilfe-makro-ohne-box-zu-modifizieren.102096/)

Das Addin ist nicht ohne Hilfestellung anderer Entwickler und von denen zur Verf�gung gestellten Programmcode entstanden. Im folgenden werde ich Quellen auflisten, bei denen ich Codefragmente, L�sungen und Ideen entnommen habe.
Es liegt mir fern fremden Code als meinen zu verkaufen. (Falls ich was vergessen habe, so werde ich es nat�rlich gerne erg�nzen.)
*	TCP-Client f�r den Anrufmonitor: ErfinderDesRades [VersuchsChat mit leistungsf�higem Server](https://www.vb-paradise.de/index.php/Thread/61948-VersuchsChat-mit-leistungsf%C3%A4higem-Server)
*   WPF TreeView: Dirk Bahle [TreeLib](https://github.com/Dirkster99/TreeLib), [Advanced WPF TreeView in C#/VB.Net Part 6 of n](https://www.codeproject.com/Articles/1224943/Advanced-WPF-TreeView-in-Csharp-VB-Net-Part-of-n)
*   WPF Telefonbuch: Tosker [ContactBook-Tutorial](https://github.com/Tosker/ContactBook-Tutorial), [WPF Contact Book - Part 1 [Getting Started]](https://www.youtube.com/watch?v=bmw68zxjwG4)
*   WPF Navigation in Einstellungen: Rachel Lim [Navigation with MVVM](https://rachel53461.wordpress.com/2011/12/18/navigation-with-mvvm-2/)
*	WPF RelayCommand: Nofear23m [Command's in WPF](https://www.vb-paradise.de/index.php/Thread/128963-Command-s-in-WPF/?postID=1116045#post1116045)
*   WPF Enums: Brian Lagunas [BindingEnumsInWpf](https://github.com/brianlagunas/BindingEnumsInWpf), [A Better Way to Data Bind Enums in WPF](https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/),
    [Localize Enum Descriptions in WPF](https://brianlagunas.com/localize-enum-descriptions-in-wpf/)
*   Passwortverschl�sselung: [Rijndael Encryption in VB.NET](http://www.freevbcode.com/ShowCode.asp?ID=4520)

### Datenschutz
Es werden keine zugesendeten Informationen an mich oder Dritte weitergeben.
Angaben �ber das pers�nliche Telefonieverhalten werden weder ausgewertet noch an Dritte weitergegeben.
Das Programm �bermittelt keinerlei Daten an Dritte, jedoch mit folgenden Ausnahmen:
* Bei der R�ckw�rssuche wird die zu Telefonnummer an die ausgew�hlte Suchmaschine �bergeben. Die Datenschutzhinweise der Suchmaschinen sind zu beachten!
* Bei der Nutzung der Software-Telefone [Phoner](https://phoner.de) und [MicroSIP](https://www.microsip.org/) werden die zu w�hlenden Nummern an diese Programme �bergeben. Die Datenschutzhinweise der Software-Telefone sind zu beachten!

### Markenrecht
Dieses Outlook-Addin wird vom Autor privat in der Freizeit als Hobby gepflegt. Mit der Bereitstellung des Outlook-Addins werden keine gewerblichen Interessen verfolgt. Es wird aus rein ideellen Gr�nden zum Gemeinwohl aller Nutzer einer Fritz!Box betrieben. 
Die Erstellung dieser Software erfolgt nicht im Auftrag oder mit Wissen der Firmen AVM GmbH bzw. Microsoft Corporation. Diese Software wurde unabh�ngig erstellt. Der Autor pflegt im Zusammenhang mit dieser Software keine Beziehungen zur Firma AVM GmbH oder Microsoft Corporation.