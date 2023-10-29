Imports System.IO

Friend NotInheritable Class NutzerDaten
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Shared Property XMLData As OutlookXML
    Friend Shared Property TelefonieListen As TelListen

    ''' <summary>
    ''' Basispfad in das Roaming-Verzeichnis
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property BasisPfad As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName)
    Private Const PräfixListen As String = "Listen"
    Public Sub New()
        ' Initialisiere die Nutzerdaten
        Laden()
    End Sub

    Private Sub Laden()

        Dim DateiInfo As FileInfo

        ' Einstellungsdatei
        Dim ConfigPfad As String = Path.Combine(BasisPfad, $"{My.Resources.strDefShortName}.xml")
        Dim ListenPfad As String = Path.Combine(BasisPfad, $"{My.Resources.strDefShortName}-{PräfixListen}.xml")

        ' Datei für Anruflisten und VIP

        DateiInfo = New FileInfo(ConfigPfad)
        DateiInfo.Directory.Create() ' If the directory already exists, this method does nothing.

        ' Lade die Einstellungsdatei
        If File.Exists(ConfigPfad) AndAlso DeserializeXML(ConfigPfad, True, XMLData) Then
            NLogger.Debug($"Einstellungsdatei eingelesen: {ConfigPfad}")
        Else
            NLogger.Debug($"Einstellungsdatei generiert")
            XMLData = New OutlookXML
        End If

        ' Lade die  Datei für Anruflisten und VIP
        If File.Exists(ListenPfad) AndAlso DeserializeXML(ListenPfad, True, TelefonieListen) Then
            NLogger.Debug($"Listen eingelesen: {ListenPfad}")
        Else
            NLogger.Debug($"Listen generiert")
            TelefonieListen = New TelListen
        End If

        ' Setze einige Felder

        With XMLData.POptionen
            ' Ermittle eine gülte IP-Adresse
            .ValidFBAdr = ValidIP(.TBFBAdr)

            ' Passe Loglevel an
            SetLogLevel(.CBoxMinLogLevel)
        End With
    End Sub

    Friend Sub Speichern()
        ' Speichere Einstellungsdatei
        XmlSerializeToFile(XMLData, Path.Combine(BasisPfad, $"{My.Resources.strDefShortName}.xml"))
        ' Speichere Listen
        XmlSerializeToFile(TelefonieListen, Path.Combine(BasisPfad, $"{My.Resources.strDefShortName}-{PräfixListen}.xml"))
    End Sub
End Class