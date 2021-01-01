Imports System.Threading
Imports System.Timers
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media.Imaging
Imports Microsoft.Office.Interop

Public Class WählclientWPF
    Inherits Window

    Private WithEvents CtrlKontaktWahl As UserCtrlKontaktwahl
    Private WithEvents CtrlDirektWahl As UserCtrlDirektwahl
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property PhonerApp As Phoner

    Public Sub New(Direktwahl As Boolean)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Initialisiere das ViewModel. Die Daten werden aus den Optionen geladen.
        DataContext = New WählClientViewModel

        ' Initiere Phoner, wenn erforderlich
        If XMLData.POptionen.CBPhoner Then
            PhonerApp = New Phoner
            If Not PhonerApp.PhonerReady Then
                NLogger.Debug(WählClientPhonerInaktiv)
                PhonerApp.Dispose()
                PhonerApp = Nothing
            End If
        End If

        ' Lade initale Daten
        SetTelefonDaten()

        If Direktwahl Then
            CtrlDirektWahl = New UserCtrlDirektwahl With {.DataContext = DataContext}
            NavigationCtrl.Content = CtrlDirektWahl
        Else
            CtrlKontaktWahl = New UserCtrlKontaktwahl With {.DataContext = DataContext}
            NavigationCtrl.Content = CtrlKontaktWahl
        End If
    End Sub

#Region "WithEvents"
    Private WithEvents FBWählClient As FritzBoxWählClient
    Private WithEvents TimerSchließen As Timers.Timer
#End Region

    ''' <summary>
    ''' Sammelt alle Kontaktdaten des Outlook-Kontaktes als <see cref="Outlook.ContactItem"/> zusammen.
    ''' </summary>
    ''' <param name="oContact">Outlook Kontakt, der eingeblendet werden soll.</param>
    Friend Sub SetOutlookKontakt(ByVal oContact As Outlook.ContactItem)

        With CType(DataContext, WählClientViewModel)
            ' Outlook Kontakt im ViewModel setzen
            .OKontakt = oContact

            ' Telefonnummern des Kontaktes setzen 
            .DialNumberList.AddRange(GetKontaktTelNrList(oContact))

            ' Kopfdaten setzen
            .Name = WählClientFormText($"{oContact.FullName}{If(oContact.CompanyName.IsNotStringEmpty, $" ({oContact.CompanyName})", DfltStringEmpty)}")

            ' Kontaktbild anzeigen
            Dim BildPfad As String

            BildPfad = KontaktBild(oContact)

            If BildPfad.IsNotStringEmpty Then
                ' Kontaktbild laden
                .Kontaktbild = New BitmapImage
                With .Kontaktbild
                    .BeginInit()
                    .CacheOption = BitmapCacheOption.OnLoad
                    .UriSource = New Uri(BildPfad)
                    .EndInit()
                End With
                'Lösche das Kontaktbild 
                DelKontaktBild(BildPfad)
            End If
        End With
    End Sub

    ''' <summary>
    ''' Sammelt alle Kontaktdaten des Outlook-ExchangeNutzers als <see cref="Outlook.ExchangeUser"/> zusammen.
    ''' </summary>
    ''' <param name="oExchangeUser">Outlook-ExchangeNutzers, der eingeblendet werden soll.</param>
    Friend Sub SetOutlookKontakt(ByVal oExchangeUser As Outlook.ExchangeUser)
        With CType(DataContext, WählClientViewModel)
            ' Outlook ExchangeNutzer im ViewModel setzen
            .OExchangeNutzer = oExchangeUser

            ' Telefonnummern des Kontaktes setzen 
            .DialNumberList.AddRange(GetKontaktTelNrList(oExchangeUser))

            ' Kopfdaten setzen
            .Name = WählClientFormText($"{oExchangeUser.Name}{If(oExchangeUser.CompanyName.IsNotStringEmpty, $" ({oExchangeUser.CompanyName})", DfltStringEmpty)}")

        End With
    End Sub

    Friend Sub SetTelefonnummer(ByVal TelNr As Telefonnummer)

        With CType(DataContext, WählClientViewModel)
            ' Telefonnummer setzen 
            .DialNumberList.Add(TelNr)

            ' Kopfdaten setzen
            .Name = WählClientFormText(TelNr.Formatiert)

        End With
    End Sub

    Friend Sub SetDirektwahl()
        With CType(DataContext, WählClientViewModel)
            ' Kopfdaten setzen
            .Name = WählClientFormText("Direktwahl")

            ' Wahlwiederhohlung in Combobox schreiben
            If XMLData.PTelefonie.CALLListe IsNot Nothing AndAlso XMLData.PTelefonie.CALLListe.Any Then
                .DialDirektWahlList.AddRange(XMLData.PTelefonie.GetTelNrList(XMLData.PTelefonie.CALLListe))
            End If
        End With
    End Sub

    Private Sub SetTelefonDaten()
        With CType(DataContext, WählClientViewModel)

            ' Standard Status Wert festlegen
            .Status = DfltStringEmpty

            ' Abbruch Button deaktivieren/ausblenden
            BAbbruch.Visibility = Visibility.Hidden

            ' Optionen aktivieren
            GBoxOptionen.IsEnabled = True

            ' Rufnummernunterdrückung gemäß Optionen setzen
            .CLIR = XMLData.POptionen.CBCLIR

            NLogger.Debug(WählClientStatusLadeGeräte)

            ' Schreibe alle geeigneten Telefone rein (kein Fax, keine IP-Telefonie, keine AB)
            If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then

                ' Nur FON, DECT, S0 und Phoner, wenn Phoner aktiv
                .DialDeviceList.AddRange(XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) Not TG.IsFax And (TG.TelTyp = DfltWerteTelefonie.TelTypen.FON Or TG.TelTyp = DfltWerteTelefonie.TelTypen.DECT Or TG.TelTyp = DfltWerteTelefonie.TelTypen.S0)).ToList)

                ' Ausgewähltes Standardgerät
                .TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.StdTelefon)

                ' Wenn kein Standard-Gerät in den Einstellungen festgelegt wurde, dann nimm das zuletzt genutzte Telefon
                If .TelGerät Is Nothing Then
                    NLogger.Debug(WählClientStatusLetztesGerät)
                    .TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.ZuletztGenutzt)
                End If

                ' Wenn kein Standard-Gerät in den Einstellungen festgelegt wurde, dann nimm das erste in der Liste
                If .TelGerät Is Nothing And .DialDeviceList.Count.IsNotZero Then
                    NLogger.Debug(WählClientStatus1Gerät)
                    .TelGerät = .DialDeviceList.First
                End If

            Else
                NLogger.Debug(WählClientStatusFehlerGerät)
            End If
        End With
    End Sub

#Region "Form Events"
    Private Sub BOptionen_MouseEnter(sender As Object, e As MouseEventArgs)
        OptionPopup.StaysOpen = True
    End Sub

    Private Sub BOptionen_MouseLeave(sender As Object, e As MouseEventArgs)
        OptionPopup.StaysOpen = False
    End Sub

    Private Sub BContact_Click(sender As Object, e As RoutedEventArgs)
        With CType(DataContext, WählClientViewModel)
            .OKontakt?.Display()
            .OExchangeNutzer?.Details()
        End With
    End Sub
    Private Sub BAbbruch_Click(sender As Object, e As RoutedEventArgs) Handles BAbbruch.Click
        Using tmpTelNr As New Telefonnummer
            DialTelNr(tmpTelNr, True)
        End Using
    End Sub




#End Region

    ''' <summary>
    ''' Startet den Wählvorgang
    ''' </summary>
    ''' <param name="TelNr"></param>
    ''' <param name="AufbauAbbrechen"></param>
    Private Sub DialTelNr(TelNr As Telefonnummer, AufbauAbbrechen As Boolean)

        With CType(DataContext, WählClientViewModel)
            ' Abbruch Button aktivieren/einblenden
            BAbbruch.Visibility = Visibility.Visible
            ' Optionen deaktivieren
            GBoxOptionen.IsEnabled = False

            Dim DialCode As String = DfltStringEmpty
            Dim Erfolreich As Boolean = False

            If AufbauAbbrechen Then
                NLogger.Debug(WählClientStatusAbbruch)

                DialCode = DfltStringEmpty

                ' Timmer abbrechen, falls er läuft
                If TimerSchließen IsNot Nothing Then TimerSchließen.Stop()
                ' Ein erneutes Wählen ermöglichen
                'DGNummern.UnselectAll()
            Else
                ' Status setzen
                .Status = WählClientBitteWarten
                NLogger.Debug(WählClientStatusVorbereitung)
                ' Entferne 1x # am Ende
                DialCode = TelNr.Unformatiert.RegExRemove("#{1}$")
                ' Füge VAZ und LKZ hinzu, wenn gewünscht
                If XMLData.POptionen.CBForceDialLKZ Then DialCode = DialCode.RegExReplace("^0(?=[1-9])", DfltWerteTelefonie.PDfltVAZ & TelNr.Landeskennzahl)

                ' Rufnummerunterdrückung
                DialCode = $"{If(.CLIR, "*31#", DfltStringEmpty)}{XMLData.POptionen.TBAmt}{DialCode}#"

                NLogger.Debug(WählClientStatusWählClient(DialCode))
            End If

            If .TelGerät.IsPhoner Then
                ' Telefonat an Phoner übergeben
                NLogger.Info("Wählclient an Phoner: {0} über {1}", DialCode, .TelGerät.Name)
                Erfolreich = PhonerApp.DialPhoner(DialCode, AufbauAbbrechen)
            Else
                ' Telefonat üper SOAP an Fritz!Box weiterreichen
                NLogger.Info("Wählclient SOAPDial: {0} über {1}", DialCode, .TelGerät.Name)
                Erfolreich = FBWählClient.SOAPDial(DialCode, .TelGerät, AufbauAbbrechen)
            End If

            ' Ergebnis auswerten 
            If Erfolreich Then
                If AufbauAbbrechen Then
                    .Status = WählClientDialHangUp
                Else
                    .Status = WählClientJetztAbheben
                    ' Abbruch-Button aktivieren, wenn Anruf abgebrochen
                    BAbbruch.IsEnabled = True
                End If

                ' Einstellungen (Welcher Anschluss, CLIR...) speichern
                XMLData.POptionen.CBCLIR = .CLIR
                ' Standard-Gerät speichern

                If Not .TelGerät.ZuletztGenutzt Then
                    ' Entferne das Flag bei allen anderen Geräten
                    ' (eigentlich reicht es, das Flag bei dem einen Gerät zu entfernen. Sicher ist sicher.
                    XMLData.PTelefonie.Telefoniegeräte.ForEach(Sub(TE) TE.ZuletztGenutzt = False)
                    ' Flag setzen
                    .TelGerät.ZuletztGenutzt = True
                End If
                ' Timer zum automatischen Schließen des Fensters starten
                If XMLData.POptionen.CBCloseWClient Then TimerSchließen = SetTimer(XMLData.POptionen.TBWClientEnblDauer * 1000)
            Else
                .Status = WählClientDialFehler
            End If

        End With
    End Sub
#Region "Timer"
    Private Sub TimerSchließen_Elapsed(sender As Object, e As ElapsedEventArgs) Handles TimerSchließen.Elapsed
        TimerSchließen = KillTimer(TimerSchließen)
        Close()
    End Sub

    Private Sub KontaktWahl_Selected(sender As Object, e As RoutedEventArgs) Handles CtrlKontaktWahl.Dial, CtrlDirektWahl.Dial
        DialTelNr(CType(DataContext, WählClientViewModel).TelNr, False)
    End Sub
#End Region
End Class
