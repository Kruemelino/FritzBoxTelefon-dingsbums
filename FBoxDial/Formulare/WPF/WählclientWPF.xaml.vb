Imports System.Threading
Imports System.Timers
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Markup

Public Class WählclientWPF
    Inherits Window

    Private WithEvents CtrlKontaktWahl As UserCtrlKontaktwahl
    Private WithEvents CtrlDirektWahl As UserCtrlDirektwahl
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "WithEvents"
    Private WithEvents TimerSchließen As Timers.Timer
#End Region

    Public Sub New(ViewModel As WählClientViewModel)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Startup Position festlegen
        WindowStartupLocation = WindowStartupLocation.CenterScreen

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Initialisiere das ViewModel. Die Daten werden aus den Optionen geladen.
        DataContext = ViewModel

        ' Lade initale Daten
        SetTelefonDaten()

        If ViewModel.IsDirektWahl Then
            NLogger.Debug("Direktwahl geladen")
            CtrlDirektWahl = New UserCtrlDirektwahl With {.DataContext = DataContext}
            NavigationCtrl.Content = CtrlDirektWahl
        Else
            NLogger.Debug("Kontaktwahl geladen")
            CtrlKontaktWahl = New UserCtrlKontaktwahl With {.DataContext = DataContext}
            NavigationCtrl.Content = CtrlKontaktWahl
        End If

        Show()
    End Sub

    Private Sub SetTelefonDaten()
        With CType(DataContext, WählClientViewModel)

            ' Standard Status Wert festlegen
            .Status = DfltStringEmpty

            ' Abbruch Button deaktivieren/ausblenden
            .IsCancelEnabled = False

            ' Optionen aktivieren
            GBoxOptionen.IsEnabled = True

            ' Abfrageflag für Mobilnummern setzen
            .CheckMobil = XMLData.POptionen.CBCheckMobil

            ' Rufnummernunterdrückung gemäß Optionen setzen
            .CLIR = XMLData.POptionen.CBCLIR

            NLogger.Debug(WählClientStatusLadeGeräte)

            ' Schreibe alle geeigneten Telefone rein (kein Fax, keine IP-Telefonie, keine AB)
            If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then

                ' Nur FON, DECT, S0 und Phoner, MicroSIP
                .DialDeviceList.AddRange(XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) TG.IsDialable).ToList)

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

#Region "WPF Events"
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
        DialTelNr(True)
    End Sub

    Private Sub KontaktWahl_Selected(sender As Object, e As RoutedEventArgs) Handles CtrlKontaktWahl.Dial, CtrlDirektWahl.Dial
        DialTelNr(False)
    End Sub

#End Region

    ''' <summary>
    ''' Startet den Wählvorgang
    ''' </summary>
    ''' <param name="AufbauAbbrechen"></param>
    Private Sub DialTelNr(AufbauAbbrechen As Boolean)

        Dim Fortsetzen As Boolean = True

        With CType(DataContext, WählClientViewModel)
            ' Optionen deaktivieren
            GBoxOptionen.IsEnabled = False

            Dim DialCode As String = DfltStringEmpty
            Dim Erfolreich As Boolean = False

            If AufbauAbbrechen Then
                NLogger.Debug(WählClientStatusAbbruch)

                DialCode = DfltStringEmpty

                ' Timmer abbrechen, falls er läuft
                If TimerSchließen IsNot Nothing Then TimerSchließen.Stop()

            Else
                ' Wenn es sich um eine Mobilnummer handelt, kann der Nutzer auswählen, ob er zunächst gefragt wird.
                If .CheckMobil AndAlso .TelNr.IstMobilnummer Then
                    Fortsetzen = MessageBox.Show(WählClientFrageMobil(.TelNr.Formatiert), Title, MessageBoxButton.YesNo) = MessageBoxResult.Yes
                End If

                If Fortsetzen Then

                    ' Status setzen
                    .Status = WählClientBitteWarten
                    NLogger.Debug(WählClientStatusVorbereitung)
                    ' Entferne 1x # am Ende
                    DialCode = .TelNr.Unformatiert.RegExRemove("#{1}$")
                    ' Füge VAZ und LKZ hinzu, wenn gewünscht
                    If XMLData.POptionen.CBForceDialLKZ Then DialCode = DialCode.RegExReplace("^0(?=[1-9])", DfltWerteTelefonie.PDfltVAZ & .TelNr.Landeskennzahl)

                    ' Rufnummerunterdrückung
                    DialCode = $"{If(.CLIR, "*31#", DfltStringEmpty)}{XMLData.POptionen.TBAmt}{DialCode}#"

                    NLogger.Debug(WählClientStatusWählClient(DialCode))
                End If
            End If

            If Fortsetzen Then

                If .TelGerät.IsSoftPhone Then

                    If .TelGerät.IsPhoner Then
                        ' Initiere Phoner, wenn erforderlich
                        If XMLData.POptionen.CBPhoner Then

                            Using PhonerApp = New Phoner

                                If PhonerApp.PhonerReady Then
                                    ' Telefonat an Phoner übergeben
                                    NLogger.Info("Wählclient an Phoner: {0} über {1}", DialCode, .TelGerät.Name)
                                    Erfolreich = PhonerApp.Dial(DialCode, AufbauAbbrechen)
                                Else
                                    NLogger.Debug(WählClientSoftPhoneInaktiv("Phoner"))
                                    Erfolreich = False
                                End If

                            End Using
                        End If
                    End If

                    If .TelGerät.IsMicroSIP Then
                        ' Initiere MicroSIP, wenn erforderlich
                        If XMLData.POptionen.CBMicroSIP Then

                            Using MicroSIPApp = New MicroSIP

                                If MicroSIPApp.MicroSIPReady Then
                                    ' Telefonat an Phoner übergeben
                                    NLogger.Info("Wählclient an MicroSIP: {0} über {1}", DialCode, .TelGerät.Name)
                                    Erfolreich = CBool((MicroSIPApp?.Dial(DialCode, AufbauAbbrechen)))
                                Else
                                    NLogger.Debug(WählClientSoftPhoneInaktiv("MicroSIP"))
                                    Erfolreich = False
                                End If

                            End Using
                        End If
                    End If

                Else
                    ' Telefonat über TR064Dial an Fritz!Box weiterreichen
                    If .Wählclient IsNot Nothing Then
                        NLogger.Info("Wählclient TR064Dial: {0} über {1}", DialCode, .TelGerät.Name)
                        Erfolreich = .Wählclient.TR064Dial(DialCode, .TelGerät, AufbauAbbrechen)
                    Else
                        NLogger.Error("Wählclient ist Nothing")
                        Erfolreich = False
                        .Status = WählClientDialFehler
                    End If

                End If

                ' Ergebnis auswerten 
                If Erfolreich Then
                    If AufbauAbbrechen Then
                        .Status = WählClientDialHangUp
                        .IsCancelEnabled = False
                    Else
                        .Status = WählClientJetztAbheben
                        ' Abbruch-Button aktivieren, wenn Anruf abgebrochen
                        .IsCancelEnabled = True
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

            End If
        End With
    End Sub

#Region "Timer"
    Private Sub TimerSchließen_Elapsed(sender As Object, e As ElapsedEventArgs) Handles TimerSchließen.Elapsed
        TimerSchließen = KillTimer(TimerSchließen)
        Close()
    End Sub
#End Region
End Class
