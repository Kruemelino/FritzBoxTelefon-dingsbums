Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Threading
Imports Microsoft.Office.Interop.Outlook
Imports Microsoft.Win32
Friend Class OptionenService
    Implements IOptionenService

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private _OutlookStoreRootFolder As IEnumerable(Of MAPIFolder)
    Private Function GetOutlookStoreRootFolder() As IEnumerable(Of MAPIFolder) Implements IOptionenService.GetOutlookStoreRootFolder
        ' Ermittle die Wurzelordner jedes Stores
        If _OutlookStoreRootFolder Is Nothing Then
            _OutlookStoreRootFolder = From S In Globals.ThisAddIn.Application.Session.Stores() Select CType(S, Store).GetRootFolder

            NLogger.Debug($"Outlook RootFolder erstmalig eingelesen: {_OutlookStoreRootFolder.Count}")
        End If

        Return _OutlookStoreRootFolder
    End Function

#Region "Design"
    Private Sub UpdateTheme() Implements IOptionenService.UpdateTheme
        OfficeColors.UpdateTheme()
    End Sub

    Private Sub ShowDesignTest() Implements IOptionenService.ShowDesignTest
        AddWindow(Of EasyWPFThemeLib.MainWindow)()
    End Sub

    Private Sub ToogleDesign() Implements IOptionenService.ToogleDesign
        ToogleTheme()
    End Sub
#End Region

#Region "Grunddaten"
    Private Function LadeFBoxUser() As ObservableCollectionEx(Of FBoxAPI.User) Implements IOptionenService.LadeFBoxUser

        Dim UserList As New ObservableCollectionEx(Of FBoxAPI.User)
        Dim XMLString As String = String.Empty
        Dim FritzBoxUsers As New FBoxAPI.UserList

        If Globals.ThisAddIn.FBoxTR064?.Ready Then
            If Globals.ThisAddIn.FBoxTR064.LANConfigSecurity.GetUserList(XMLString) AndAlso DeserializeXML(XMLString, False, FritzBoxUsers) Then
                UserList.AddRange(FritzBoxUsers.UserListe)

                NLogger.Trace($"Userliste ermittelt: {XMLString}")
            Else
                NLogger.Trace("Userliste nicht ermittelt")
            End If
        Else
            NLogger.Trace("Userliste nicht ermittelt, da TR064 nicht verfügbar.")
        End If

        Return UserList
    End Function
#End Region

#Region "Import Telefoniedaten"
    Private Property FritzBoxDaten As Telefonie

    Private Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie)) Implements IOptionenService.Beendet
    Private Event Status As EventHandler(Of String) Implements IOptionenService.Status
    Private Event FBoxAPIStatus As EventHandler(Of String)

    Private Sub StartImport() Implements IOptionenService.StartImport

        ' Neue Telefonie erstellen
        FritzBoxDaten = New Telefonie

        ' Ereignishandler hinzufügen
        AddHandler FritzBoxDaten.Beendet, AddressOf FritzBoxDatenImportBeendet
        AddHandler FritzBoxDaten.Status, AddressOf SetStatus

        NLogger.Debug($"Einlesen der Telefoniedaten gestartet")
        ' Starte das Einlesen
        Dispatcher.CurrentDispatcher.BeginInvoke(Sub() If Ping(XMLData.POptionen.ValidFBAdr) Then FritzBoxDaten.GetFritzBoxDaten())

    End Sub

    Private Sub SetStatus(sender As Object, e As String)
        RaiseEvent Status(Me, e)
    End Sub

    Private Sub FritzBoxDatenImportBeendet()

        ' Signalisiere, das beenden des Einlesens
        RaiseEvent Beendet(Me, New NotifyEventArgs(Of Telefonie)(FritzBoxDaten))

        ' Ereignishandler entfernen
        RemoveHandler FritzBoxDaten.Beendet, AddressOf FritzBoxDatenImportBeendet
        RemoveHandler FritzBoxDaten.Status, AddressOf SetStatus

        NLogger.Debug($"Einlesen der Telefoniedaten beendet")
    End Sub

#End Region

#Region "Indizierung"
    Private Function ZähleKontakte(olFolders As List(Of MAPIFolder)) As Integer Implements IOptionenService.ZähleOutlookKontakte
        Dim retval As Integer = 0
        For Each olFolder In olFolders
            retval += olFolder.Items.Count
        Next
        Return retval
    End Function

    Private Function Indexer(olOrdner As MAPIFolder, IndexModus As Boolean, ct As CancellationToken, progress As IProgress(Of String)) As Integer

        Dim VerarbeiteteKontakte As Integer = 0

        ' Schleife durch jedes Element dieses Ordners. 
        For Each Item In olOrdner.Items

            Select Case True
                ' Unterscheidung je nach Datentyp
                Case TypeOf Item Is ContactItem

                    Dim aktKontakt As ContactItem = CType(Item, ContactItem)

                    If IndexModus Then
                        IndiziereKontakt(aktKontakt)
                    Else
                        DeIndiziereKontakt(aktKontakt)
                    End If

                    ' Erhöhe Wert für Progressbar und schreibe einen Status
                    progress?.Report($"Kontakt '{aktKontakt.FullName}' abgeschlossen ...")

                    aktKontakt = Nothing
                    'ReleaseComObject(aktKontakt)

                Case TypeOf Item Is AddressList ' Adressliste
                    With CType(Item, AddressList)
                        progress?.Report($"Adressliste '{ .Name}' übergangen ...")
                    End With

                Case TypeOf Item Is DistListItem ' Verteilerliste
                    With CType(Item, DistListItem)
                        progress?.Report($"Verteilerliste '{ .DLName}' übergangen ...")
                    End With

                Case Else
                    progress?.Report($"Unbekanntes Objekt übergangen ...")

            End Select

            ReleaseComObject(Item)
            ' Frage Cancelation ab
            If ct.IsCancellationRequested Then Exit For

            VerarbeiteteKontakte += 1
        Next

        NLogger.Info($"{If(IndexModus, "Indizierung", "Deindizierung")} des Ordners {olOrdner.Name} ist abgeschlossen ({VerarbeiteteKontakte} Kontakte verarbeitet).")

        ReleaseComObject(olOrdner)

        Return VerarbeiteteKontakte
    End Function

    Private Async Function Indexer(OrdnerListe As List(Of MAPIFolder), IndexModus As Boolean, ct As CancellationToken, progress As IProgress(Of String)) As Task(Of Integer) Implements IOptionenService.Indexer

        Dim IndexTasks As New List(Of Task(Of Integer))

        ' Verarbeite alle Ordner die der Kontaktsuche entsprechen
        For Each Ordner In OrdnerListe
            NLogger.Debug($"{If(IndexModus, "Indiziere", "Deindiziere")} Ordner {Ordner.Name}")
            ' Starte das Indizieren
            IndexTasks.Add(Task.Run(Function()
                                        Return Indexer(Ordner, IndexModus, ct, progress)
                                    End Function, ct))

            ' Frage Cancelation ab
            If ct.IsCancellationRequested Then Exit For
        Next

        ' Warte den Abschluss der Indizierung ab
        Return (Await Task.WhenAll(IndexTasks)).Sum

    End Function

#End Region

#Region "Test Rückwärtssuche"
    Private Event BeendetRWS As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetRWS

    Private Async Sub StartRWSTest(TelNr As String) Implements IOptionenService.StartRWSTest

        ' Ereignishandler hinzufügen
        AddHandler Rückwärtssuche.Beendet, AddressOf RWSTestBeendet
        AddHandler Rückwärtssuche.Status, AddressOf SetStatus

        NLogger.Debug($"Test der Rückwärtssuche für '{TelNr}' gestartet")

        Await StartRWS(New Telefonnummer With {.SetNummer = TelNr}, False)

    End Sub

    Private Sub RWSTestBeendet(sender As Object, e As NotifyEventArgs(Of Boolean))
        RaiseEvent BeendetRWS(Me, New NotifyEventArgs(Of Boolean)(e.Value))

        ' Ereignishandler hinzufügen
        RemoveHandler Rückwärtssuche.Beendet, AddressOf RWSTestBeendet
        RemoveHandler Rückwärtssuche.Status, AddressOf SetStatus

        NLogger.Debug($"Test der Rückwärtssuche beendet")
    End Sub
#End Region

#Region "Tellows"
    Private Async Function GetTellowsAccountData(XAuthToken As String) As Task(Of TellowsPartnerInfo) Implements IOptionenService.GetTellowsAccountData
        Using tellows = New Tellows(XAuthToken)
            Return Await tellows.GetTellowsAccountInfo()
        End Using
    End Function

    Private Async Function GetTellowsLiveAPIData(TelNr As String, XAuthToken As String) As Task(Of TellowsResponse) Implements IOptionenService.GetTellowsLiveAPIData
        If TelNr.IsNotStringNothingOrEmpty Then
            Using Tel As New Telefonnummer With {.SetNummer = TelNr}
                If Tel.TellowsNummer.IsNotStringNothingOrEmpty Then
                    Using tellows = New Tellows(XAuthToken)
                        Return Await tellows.GetTellowsLiveAPIData(Tel)
                    End Using
                Else
                    Return Nothing
                End If
            End Using
        Else
            Return Nothing
        End If
    End Function
#End Region

#Region "IP Telefone"
    Private Function GetSIPClients() As FBoxAPI.SIPClientList Implements IOptionenService.GetSIPClients

        If Globals.ThisAddIn.FBoxTR064?.Ready Then
            Dim SIPList As FBoxAPI.SIPClientList = Nothing
            If Globals.ThisAddIn.FBoxTR064.X_voip.GetClients(SIPList) Then
                Return SIPList

            End If
        End If
        ' Gib eine leere Liste zurück
        Return New FBoxAPI.SIPClientList
    End Function
#End Region

#Region "Wählclient"

    ''' <summary>
    ''' Registriert ein cmd-Command für die Verknüpfung mit tel:// und callto:// Links<br/>
    ''' <see href="link">https://stackoverflow.com/a/69163202</see>
    ''' </summary>
    Private Function RegisterApp() As Boolean Implements IOptionenService.RegisterApp
        Using key As RegistryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\Classes\callto")
            ' [HKEY_CURRENT_USER\SOFTWARE\Classes\callto]
            ' @="URL:callto"
            ' "URL Protocol"=""
            ' "Owner Name"="FritzOutlookV5"

            key.SetValue("", "URL:callto", RegistryValueKind.String)
            key.SetValue("URL Protocol", String.Empty, RegistryValueKind.String)
            key.SetValue("Owner Name", My.Resources.strDefShortName, RegistryValueKind.String)
        End Using

        Using key As RegistryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\Classes\tel")
            ' [HKEY_CURRENT_USER\SOFTWARE\Classes\tel]
            ' @="URL:tel"
            ' "URL Protocol"=""
            ' "Owner Name"="FritzOutlookV5"

            key.SetValue("", "URL:tel", RegistryValueKind.String)
            key.SetValue("URL Protocol", String.Empty, RegistryValueKind.String)
            key.SetValue("Owner Name", My.Resources.strDefShortName, RegistryValueKind.String)
        End Using

        Using key As RegistryKey = Registry.CurrentUser.CreateSubKey($"SOFTWARE\Classes\{My.Resources.strDefShortName}.callto\Shell\Open\Command")

            ' [HKEY_CURRENT_USER\SOFTWARE\Classes\FritzOutlookV5.callto]

            ' [HKEY_CURRENT_USER\SOFTWARE\Classes\FritzOutlookV5.callto\Shell]

            ' [HKEY_CURRENT_USER\SOFTWARE\Classes\FritzOutlookV5.callto\Shell\Open]

            ' [HKEY_CURRENT_USER\SOFTWARE\Classes\FritzOutlookV5.callto\Shell\Open\Command]
            ' @="cmd.exe /C echo %1 > "%%AppData%%\Fritz!Box Telefon-Dingsbums\TelProt.txt""

            ' Das muss am Ende im (Standard) stehen: cmd.exe /C echo %1 > "%%AppData%%\Fritz!Box Telefon-Dingsbums\TelProt.txt"
            key.SetValue("", $"cmd.exe /C echo %1 > ""%%AppData%%\{My.Resources.strDefLongName}\{My.Resources.strLinkProtFileName}""", RegistryValueKind.String)
        End Using

        Using key As RegistryKey = Registry.CurrentUser.CreateSubKey($"SOFTWARE\{My.Resources.strDefShortName}\Capabilities")
            ' [HKEY_CURRENT_USER\SOFTWARE\FritzOutlookV5]

            ' [HKEY_CURRENT_USER\SOFTWARE\FritzOutlookV5\Capabilities]
            ' "ApplicationDescription"="Fritz!Box Telefon-dingsbums"
            ' "ApplicationName"="FritzOutlookV5"

            key.SetValue("ApplicationDescription", My.Resources.strDefLongName, RegistryValueKind.String)
            key.SetValue("ApplicationName", My.Resources.strDefShortName, RegistryValueKind.String)

            Using key.CreateSubKey("URLAssociations")
                ' [HKEY_CURRENT_USER\SOFTWARE\FritzOutlookV5\Capabilities\URLAssociations]
                ' "callto"="FritzOutlookV5.callto"
                ' "tel"="FritzOutlookV5.callto"

                key.SetValue("callto", $"{My.Resources.strDefShortName}.callto", RegistryValueKind.String)
                key.SetValue("tel", $"{My.Resources.strDefShortName}.callto", RegistryValueKind.String)
            End Using

        End Using

        Using key As RegistryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\RegisteredApplications")
            ' [HKEY_CURRENT_USER\SOFTWARE\RegisteredApplications]
            ' "FritzOutlookV5"="Software\\FritzOutlookV5\\Capabilities"

            key.SetValue("FritzOutlookV5", $"Software\\{My.Resources.strDefShortName}\\Capabilities", RegistryValueKind.String)
        End Using

        Return True
    End Function
#End Region

#Region "Test Kontaktsuche"
    Private Event BeendetKontaktsuche As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetKontaktsuche

    Private Async Sub StartKontaktsucheTest(TelNr As String) Implements IOptionenService.StartKontaktsucheTest
        ' Ereignishandler hinzufügen
        AddHandler KontaktSucher.Beendet, AddressOf KontaktsucheTestBeendet
        AddHandler KontaktSucher.Status, AddressOf SetStatus

        ' Führe eine Kontaktsuche durch
        Dim oc As ContactItem = Await KontaktSucheTelNr(New Telefonnummer With {.SetNummer = TelNr})
        ' Blende den Kontakt ein
        If oc IsNot Nothing Then oc.Display()
    End Sub

    Private Sub KontaktsucheTestBeendet(sender As Object, e As NotifyEventArgs(Of Boolean))
        RaiseEvent BeendetKontaktsuche(Me, New NotifyEventArgs(Of Boolean)(e.Value))

        ' Ereignishandler hinzufügen
        RemoveHandler KontaktSucher.Beendet, AddressOf KontaktsucheTestBeendet
        RemoveHandler KontaktSucher.Status, AddressOf SetStatus

        NLogger.Debug($"Test der Kontaktsuche beendet")
    End Sub
#End Region

#Region "Test Anrufmonitor"
    Public ReadOnly Property TelefoniedatenEingelesen As Boolean Implements IOptionenService.TelefoniedatenEingelesen
        Get
            Return XMLData.PTelefonie.Telefonnummern.Count.IsNotZero 'AndAlso XMLData.PTelefonie.Telefoniegeräte.Count.IsNotZero
        End Get
    End Property

    Private Async Sub StartAnrMonTest(TelNr As String,
                                      CONNECT As Boolean,
                                      rnd As Boolean,
                                      rndOutlook As Boolean,
                                      rndFBox As Boolean,
                                      rndTellows As Boolean,
                                      clir As Boolean,
                                      AnrMonGeräteID As Integer) Implements IOptionenService.StartAnrMonTest

        Dim RndGen As New Random()

        If TelNr.IsStringNothingOrEmpty Then

            If rnd Then
                ' Generiere eine zufällige Telefonnummer aus Deutschland
                Dim LKZ As Landeskennzahl = Globals.ThisAddIn.PVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(L) L.Landeskennzahl = "49")
                Dim OKZ As Integer = RndGen.Next(0, LKZ.Ortsnetzkennzahlen.Count)
                Dim Nr As Integer = RndGen.Next(9999, 9999999)

                TelNr = $"0{LKZ.Ortsnetzkennzahlen.Item(OKZ).Ortsnetzkennzahl}{Nr}"
            End If

            ' Telefonnummer aus Outlookkontakten
            If rndOutlook Then
                ' Ermittle einen Kontakt aus den durchsuchten Ordnern
                ' Ermittle Ordner
                Dim OrdnerListe As List(Of OutlookOrdner) = XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche)

                ' Füge den Standardkontaktordner hinzu, falls keine anderen Ordner definiert wurden.
                If Not OrdnerListe.Any Then
                    OrdnerListe.Add(New OutlookOrdner(GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche))
                End If

                ' Erstelle eine Liste aller Kontakte, welche durchsucht werden sollen
                Dim OLC As New List(Of ContactItem)
                OrdnerListe.ForEach(Sub(F) OLC.AddRange(F.MAPIFolder.Items.Cast(Of ContactItem)))

                ' Ermittle einen zufälligen Kontakt
                Dim C As ContactItem = OLC.Item(RndGen.Next(0, OLC.Count))

                ' Ermittle eine zufällige Telefonnummer des Kontaktes
                Dim NL = C.GetKontaktTelNrList
                If NL.Any Then
                    TelNr = NL.Item(RndGen.Next(0, NL.Count)).Unformatiert
                Else
                    TelNr = String.Empty
                End If

                OLC.ForEach(Sub(Co) ReleaseComObject(Co))
            End If

            ' Telefonnummer aus Fritz!Box Telefonbüchern
            If rndFBox Then
                If Globals.ThisAddIn.PhoneBookXML Is Nothing OrElse Globals.ThisAddIn.PhoneBookXML.First.Phonebook Is Nothing Then
                    ' Wenn die Telefonbücher noch nicht heruntergeladen wurden, oder nur die Namen bekannt sind (Header-Daten),
                    ' Dann lade die Telefonbücher herunter
                    NLogger.Debug($"Die Telefonbücher sind für die Kontaktsuche nicht bereit. Beginne sie herunterzuladen...")
                    Globals.ThisAddIn.PhoneBookXML = Await Telefonbücher.LadeTelefonbücher()
                End If

                Dim FBC As New List(Of FBoxAPI.Contact)
                Globals.ThisAddIn.PhoneBookXML.ToList.ForEach(Sub(F) FBC.AddRange(F.Phonebook.Contacts.Where(Function(T) Not T.IstTelefon)))

                ' Ermittle einen zufälligen Kontakt
                Dim C As FBoxAPI.Contact = FBC.Item(RndGen.Next(0, FBC.Count))

                Dim NL = C.GetKontaktTelNrList
                TelNr = NL.Item(RndGen.Next(0, NL.Count)).Unformatiert
            End If

            ' Telefonnummer aus Tellows
            If rndTellows Then
                If Globals.ThisAddIn.TellowsScoreList Is Nothing Then
                    Using tellows As New Tellows
                        Globals.ThisAddIn.TellowsScoreList = Await tellows.LadeScoreList
                    End Using
                End If

                If Globals.ThisAddIn.TellowsScoreList IsNot Nothing Then
                    Using t As New Telefonnummer With {.SetNummer = Globals.ThisAddIn.TellowsScoreList.Item(RndGen.Next(0, Globals.ThisAddIn.TellowsScoreList.Count)).Number}
                        TelNr = t.Unformatiert
                    End Using
                End If
            End If

            ' Unterdrückte Telefonnummer
            If clir Then TelNr = ""
        Else
            ' Ermittle die unformatierte Nummer der Nutzereingabe
            Using t As New Telefonnummer With {.SetNummer = TelNr}
                TelNr = t.Unformatiert
            End Using
        End If

        Dim tmpStr As String

        If XMLData.PTelefonie.Telefonnummern.Count.IsZero Then
            tmpStr = RndGen.Next(9999, 99999).ToString
        Else
            tmpStr = XMLData.PTelefonie.Telefonnummern(RndGen.Next(0, XMLData.PTelefonie.Telefonnummern.Count)).Einwahl
        End If

        NLogger.Debug($"Starte Test des Anrufmonitors mit {TelNr}.")

        ' Starte ein eingehendes Telefonat
        Dim AktivesTelefonat = New Telefonat With {.SetAnrMonRING = {Now.ToString("G"), "RING", "99", TelNr, tmpStr, "SIP4"}}

        If CONNECT Then
            ' Wenn -1 übergeben wird, dann wähle zufällig ein Gerät aus
            If AnrMonGeräteID.AreEqual(-1) AndAlso XMLData.PTelefonie.Telefoniegeräte.Count.IsNotZero Then
                AnrMonGeräteID = XMLData.PTelefonie.Telefoniegeräte(RndGen.Next(0, XMLData.PTelefonie.Telefoniegeräte.Count)).AnrMonID
            End If

            ' 23.06.18 13:20:44;CONNECT;1;40;0123456789;
            AktivesTelefonat.SetAnrMonCONNECT = {Now.ToString("G"), "CONNECT", "99", $"{AnrMonGeräteID}", TelNr}
        End If

        ' 23.06.18 13:20:52;DISCONNECT;1;9;
        ' TODO: Verzögertes Starten. Ansonstne Timingproblem mit Kontaktsuche
        AktivesTelefonat.SetAnrMonDISCONNECT = {Now.ToString("G"), "DISCONNECT", "99", "60"}

        NLogger.Debug($"Test des Anrufmonitors mit {TelNr} beendet.")

        RndGen = Nothing
    End Sub


#End Region

#Region "Test 2FA"
    Private Sub Start2FATest() Implements IOptionenService.Start2FATest

        Globals.ThisAddIn.FBoxTR064.X_voip.DialSetConfig("DECT: Gert")

    End Sub
#End Region
End Class
