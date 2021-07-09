Imports System.IO

Friend NotInheritable Class NutzerDaten
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Shared Property XMLData As OutlookXML

    Public Sub New()
        ' Initialisiere die Nutzerdaten
        Laden()
    End Sub

    Friend Sub Laden()

        Dim DateiInfo As FileInfo
        Dim Pfad As String

        Pfad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, DfltConfigFileName)

        DateiInfo = New FileInfo(Pfad)
        DateiInfo.Directory.Create() ' If the directory already exists, this method does nothing.

        If File.Exists(Pfad) AndAlso DeserializeXML(Pfad, True, XMLData) Then
            NLogger.Debug($"Einstellungsdatei eigelesen: {Pfad}")
        Else
            NLogger.Debug($"Einstellungsdatei generiert")
            XMLData = New OutlookXML
        End If

        ' Setze einige Felder

        With XMLData.POptionen
            .ValidFBAdr = ValidIP(.TBFBAdr)
        End With

        ' Passe Loglevel an
        SetLogLevel()

    End Sub
End Class