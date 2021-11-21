Imports System.Threading.Tasks
Imports Microsoft.Office.Interop

Public Class FritzBoxWählClient
    Implements IDisposable

#Region "Properties"
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Property WPFWindow As WählclientWPF
#End Region

#Region "Wählen per TR-064"
    ''' <summary>
    ''' Initialisiert den Wählvorgang der Fritz!Box Wählhilfe.
    ''' </summary>
    ''' <param name="DialCode">Die zu Wählende Nummer</param>
    ''' <param name="Telefon">Das ausgehende Telefon</param>
    ''' <param name="Auflegen">Angabe, ob der Verbindungsaufbau abgebrochen werden soll.</param>
    ''' <returns></returns>
    Friend Function TR064Dial(DialCode As String, Telefon As Telefoniegerät, Auflegen As Boolean) As Boolean

        Dim PhoneName As String = DfltStringEmpty

        Using TR064 As New FBoxAPI.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)

            With Telefon
                If Not TR064.X_voip.DialGetConfig(PhoneName) Then
                    ' Es kann sein, dass die Fritz!Box Wählhilfe aktiviert ist, aber kein Telefon ausgewählt ist.
                    ' In diesem Fall: Versuche das gewünschte Telefon zu setzen
                    If TR064.X_voip.DialSetConfig(.TR064Dialport) Then
                        ' Das einstellen hat geklappt. Fahre normal Fort
                    End If
                End If

                ' Ermittle das aktuell eingestellte Telefon.
                If TR064.X_voip.DialGetConfig(PhoneName) Then

                    ' Vorprüfung, ist der hinterlegte Dialport korrekt
                    ' Es kann vorkommen, dass der Dialport leer ist. Das bedeutet, dass er nicht korrekt eingelesen wurde. 
                    ' Es muss in dem Fall auf den Rückfallwert zurückgegriffen werden.
                    If .TR064Dialport.IsStringNothingOrEmpty Then
                        .TR064Dialport = .GetDialPortFallback

                        ' Log schreiben
                        NLogger.Info($"Der TR064Dialport für Telefon { .Name} wurde auf den Rückfallwert geändert: { .TR064Dialport}")
                    End If

                    ' Prüfe, ob das korrekte Telefon ausgewählt wurde.
                    If PhoneName.AreNotEqual(.TR064Dialport) Then

                        ' Das Telefon der Fritz!Box Wählhilfe muss geändert werden
                        NLogger.Debug($"Der Phoneport wird von '{PhoneName}' auf '{ .TR064Dialport}' geändert.")

                        ' Stelle das Telefon um.
                        If TR064.X_voip.DialSetConfig(.TR064Dialport) Then

                            ' Prüfe, ob das Telefon tatsächlich umgestellt wurde
                            If TR064.X_voip.DialGetConfig(PhoneName) Then
                                If PhoneName.AreEqual(.TR064Dialport) Then
                                    ' Der Phoneport wurde erfolgreich umgestellt
                                    NLogger.Debug($"Der Phoneport wurde erfolgreich auf '{PhoneName}' geändert.")
                                Else
                                    ' Der Phoneport wurde nicht umgestellt
                                    NLogger.Error($"Der Phoneport konnte nicht von '{PhoneName}' auf '{ .TR064Dialport}' geändert werden.")
                                    Return False
                                End If
                            Else
                                ' Genereller Fehler
                                NLogger.Error($"Der aktuelle Phoneport konnte nach der Umstellung auf { .TR064Dialport} nicht ausgelesen werden.")
                                Return False
                            End If

                        Else
                            ' Genereller Fehler
                            NLogger.Error($"Der Phoneport konnte nicht von '{PhoneName}' auf '{ .TR064Dialport}' umgestellt werden.")
                            Return False
                        End If
                    End If
                    ' Hier kommt man nur hin, wenn es zu keinem Fehler gekommen ist.
                    NLogger.Debug($"Übermittle das Wählkomando an die Fritz!box: Auflegen: '{Auflegen}', '{DialCode}', '{PhoneName}'")
                    ' Das Telefon der Fritz!Box Wählhilfe muss nicht geändert werden
                    ' Senden des Wählkomandos und Rückmeldung, ob das Wählen erfolgreich war
                    Return If(Auflegen, TR064.X_voip.DialHangup, TR064.X_voip.DialNumber(DialCode))
                Else
                    ' Genereller Fehler
                    NLogger.Error($"Der aktuelle Phoneport konnte nicht ausgelesen werden.")
                    Return False
                End If
            End With
        End Using

    End Function

#End Region

    '''' <summary>
    '''' Startet den Wählvorgang
    '''' </summary>
    Friend Async Function DialTelNr(TelNr As Telefonnummer, Telefon As Telefoniegerät, CLIR As Boolean, Abbruch As Boolean) As Task(Of Boolean)
        Dim DialCode As String = DfltStringEmpty
        Dim Erfolreich As Boolean = False

        If Abbruch Then
            NLogger.Debug("Anruf wird abgebrochen...")

            DialCode = DfltStringEmpty

        Else

            ' Status setzen
            NLogger.Debug("Anruf wird vorbereitet...")
            ' Entferne 1x # am Ende
            DialCode = TelNr.Unformatiert.RegExRemove("#{1}$")
            ' Füge VAZ und LKZ hinzu, wenn gewünscht
            If XMLData.POptionen.CBForceDialLKZ Then
                DialCode = DialCode.RegExReplace("^0(?=[1-9])", DfltWerteTelefonie.PDfltVAZ & TelNr.Landeskennzahl)
            End If

            ' Rufnummerunterdrückung
            DialCode = $"{If(CLIR, "*31#", DfltStringEmpty)}{XMLData.POptionen.TBPräfix}{DialCode}#"

            NLogger.Debug($"Dialcode: {DialCode}")

        End If

        If Telefon.IsSoftPhone Then

            If Telefon.IsPhoner Then
                ' Initiere Phoner, wenn erforderlich
                If XMLData.POptionen.CBPhoner Then

                    Using PhonerApp = New Phoner

                        If PhonerApp.PhonerReady Then
                            ' Telefonat an Phoner übergeben
                            NLogger.Info($"Wählclient an Phoner: {DialCode} über {Telefon.Name}")
                            Erfolreich = PhonerApp.Dial(DialCode, Abbruch)
                        Else
                            NLogger.Debug(String.Format(Localize.LocWählclient.strErrorSoftphoneNotReady, "Phoner"))
                            Erfolreich = False
                        End If

                    End Using
                End If
            End If

            If Telefon.IsMicroSIP Then
                ' Initiere MicroSIP, wenn erforderlich
                If XMLData.POptionen.CBMicroSIP Then

                    Using MicroSIPApp = New MicroSIP

                        If MicroSIPApp.MicroSIPReady Then
                            ' Telefonat an Phoner übergeben
                            NLogger.Info($"Wählclient an MicroSIP: {DialCode} über {Telefon.Name}")
                            Erfolreich = MicroSIPApp.Dial(DialCode, Abbruch)
                        Else
                            NLogger.Debug(String.Format(Localize.LocWählclient.strErrorSoftphoneNotReady, "MicroSIP"))
                            Erfolreich = False
                        End If

                    End Using
                End If
            End If

        Else
            ' Telefonat über TR064Dial an Fritz!Box weiterreichen
            NLogger.Info($"Wählclient TR064Dial: '{DialCode}', Dialport: '{Telefon.TR064Dialport}'")

            Erfolreich = Await Task.Run(Function()
                                            If Ping(XMLData.POptionen.ValidFBAdr) Then
                                                Return TR064Dial(DialCode, Telefon, Abbruch)
                                            Else Return False
                                            End If

                                        End Function)

        End If

        ' Ergebnis auswerten 
        If Erfolreich Then

            ' Einstellungen (Welcher Anschluss, CLIR...) speichern
            XMLData.POptionen.CBCLIR = CLIR
            ' Standard-Gerät speichern

            If Not Telefon.ZuletztGenutzt Then
                ' Entferne das Flag bei allen anderen Geräten
                ' (eigentlich reicht es, das Flag bei dem einen Gerät zu entfernen. Sicher ist sicher.
                XMLData.PTelefonie.Telefoniegeräte.ForEach(Sub(TE) TE.ZuletztGenutzt = False)
                ' Flag setzen
                Telefon.ZuletztGenutzt = True
            End If

            ' Timer zum automatischen Schließen des Fensters starten
            If Not Abbruch And XMLData.POptionen.CBCloseWClient Then WPFWindow.StarteAusblendTimer()

        End If

        Return Erfolreich
    End Function

#Region "Wähldialog"
    ''' <summary>
    ''' wird durch die Kontaktsuche ausgeführt
    ''' </summary>
    ''' <param name="olKontakt">Des anzurufende <see cref="Outlook.ContactItem"/></param>
    Friend Overloads Sub WählboxStart(olKontakt As Outlook.ContactItem)
        If olKontakt IsNot Nothing Then Wählbox(olKontakt)
    End Sub

    ''' <summary>
    ''' wird durch das Symbol 'Wählen' in der 'FritzBox'-Symbolleiste ausgeführt
    ''' </summary>
    ''' <param name="olAuswahl">Die aktuelle Auswahl eines Outlook-Elementes</param>
    Friend Overloads Sub WählboxStart(olAuswahl As Outlook.Selection)

        ' Ist überhaupt etwas ausgewählt?
        If olAuswahl.Count.AreEqual(1) Then

            Select Case True
                Case TypeOf olAuswahl.Item(1) Is Outlook.ContactItem   ' ist aktuelles Fenster ein Kontakt?
                    Wählbox(CType(olAuswahl.Item(1), Outlook.ContactItem))

                Case TypeOf olAuswahl.Item(1) Is Outlook.JournalItem   ' ist aktuelles Fenster ein Journal?
                    ' Es wurde ein Journaleintrag gewählt!
                    WählboxStart(CType(olAuswahl.Item(1), Outlook.JournalItem))

                Case TypeOf olAuswahl.Item(1) Is Outlook.MailItem      ' ist aktuelles Fenster ein Mail?
                    ' Es wurde eine Mail ausgewählt
                    ' Den zur Email-Adresse gehörigen Kontakt suchen
                    WählboxStart(CType(olAuswahl.Item(1), Outlook.MailItem))

                Case Else
                    ' Nix tun
                    MsgBox(Localize.LocWählclient.strErrorAuswahl, MsgBoxStyle.Exclamation, "WählboxStart")

            End Select
        Else
            MsgBox(Localize.LocWählclient.strErrorAuswahl, MsgBoxStyle.Exclamation, "WählboxStart")
        End If

    End Sub

    ''' <summary>
    ''' Startet die Direktwahl.
    ''' </summary>
    Friend Overloads Sub WählboxStart()
        Wählbox()
    End Sub

    ''' <summary>
    ''' wird durch das Symbol 'Wählen' in der 'FritzBox'-Symbolleiste in Inspektoren ausgeführt
    ''' </summary>
    ''' <param name="olInsp">Der aktuelle Inspektor</param>
    Friend Overloads Sub WählboxStart(olInsp As Outlook.Inspector)

        Select Case True
            Case TypeOf olInsp.CurrentItem Is Outlook.ContactItem   ' ist aktuelles Fenster ein Kontakt?
                Wählbox(CType(olInsp.CurrentItem, Outlook.ContactItem))
            Case TypeOf olInsp.CurrentItem Is Outlook.JournalItem   ' ist aktuelles Fenster ein Journal?
                ' Es wurde ein Journaleintrag gewählt!
                WählboxStart(CType(olInsp.CurrentItem, Outlook.JournalItem))
            Case TypeOf olInsp.CurrentItem Is Outlook.MailItem      ' ist aktuelles Fenster ein Mail?
                ' Es wurde eine Mail ausgewählt
                ' Den zur Email-Adresse gehörigen Kontakt suchen
                WählboxStart(CType(olInsp.CurrentItem, Outlook.MailItem))
            Case Else
                ' Nix tun
        End Select

    End Sub

    ''' <summary>
    ''' Wählen aus einer IM Contactcard
    ''' </summary>
    Friend Overloads Sub WählboxStart(ContactCard As Microsoft.Office.Core.IMsoContactCard)

        ' Es gibt zwei Möglichkeiten:
        ' A: Ein klassischer Kontakt ist hinterlegt
        ' B: Ein Exchange-User existiert.

        ' A: Führe zunächst die Suche nach Outlook-Kontakten durch
        Dim aktKontakt As Outlook.ContactItem = KontaktSuche(ContactCard)

        If aktKontakt IsNot Nothing Then
            ' Wenn ein Kontakt gefunden wurde so wähle diesen an.
            Wählbox(aktKontakt)
        Else
            ' Es wurde kein Kontakt gefunden. 
            ReleaseComObject(aktKontakt)

            ' B: Suche den ExchangeNutzer
            Dim aktExchangeNutzer As Outlook.ExchangeUser = KontaktSucheExchangeUser(ContactCard)
            If aktExchangeNutzer IsNot Nothing Then
                ' Wenn ein ExchangeUser gefunden wurde so wähle diesen an.
                Wählbox(aktExchangeNutzer)
            Else
                MsgBox(String.Format(Localize.LocWählclient.strErrorMail, ContactCard.Address), MsgBoxStyle.Information, "WählboxStart")
            End If
        End If

        ReleaseComObject(ContactCard)
    End Sub

    ''' <summary>
    ''' Wählen aus einer E-Mail
    ''' </summary>
    ''' <param name="aktMail">Die E-Mail, deren Absender angerufen werden soll</param>
    Friend Overloads Sub WählboxStart(aktMail As Outlook.MailItem)

        Dim SMTPAdresse As EMailType = GetSenderSMTPAddress(aktMail)

        ' Es gibt zwei Möglichkeiten:
        ' A: Ein klassischer Kontakt ist hinterlegt
        ' B: Ein Exchange-User existiert. 

        If SMTPAdresse.Addresse.IsNotStringEmpty Then
            ' A: Führe zunächst die Absendersuche nach Outlook-Kontakten durch
            Dim aktKontakt As Outlook.ContactItem = KontaktSuche(SMTPAdresse)

            If aktKontakt IsNot Nothing Then
                ' Wenn ein Kontakt gefunden wurde so wähle diesen an.
                Wählbox(aktKontakt)
            Else
                ' Es wurde kein Kontakt gefunden. 
                ReleaseComObject(aktKontakt)

                ' B: Suche den ExchangeNutzer
                Dim aktExchangeNutzer As Outlook.ExchangeUser = KontaktSucheExchangeUser(SMTPAdresse)
                If aktExchangeNutzer IsNot Nothing Then
                    ' Wenn ein ExchangeUser gefunden wurde so wähle diesen an.
                    Wählbox(aktExchangeNutzer)
                Else
                    MsgBox(String.Format(Localize.LocWählclient.strErrorMail, SMTPAdresse.Addresse), MsgBoxStyle.Information, "WählboxStart")
                End If

            End If
        End If

        ReleaseComObject(aktMail)
    End Sub

    ''' <summary>
    ''' Wählen aus einem Journaleintrag
    ''' </summary>
    ''' <param name="olJournal">Der Journaleintrag, deren verknüpfter Kontakt angerufen werden soll</param>
    Friend Overloads Sub WählboxStart(olJournal As Outlook.JournalItem)

        With olJournal
            If Not .Body.Contains(Localize.LocAnrMon.strNrUnterdrückt) And .Categories.Contains(Localize.LocAnrMon.strJournalCatDefault) Then
                Dim aktKontakt As Outlook.ContactItem
                Dim vCard As String
                Dim TelNr As Telefonnummer

                ' Telefonnummer aus dem Body ermitteln
                TelNr = New Telefonnummer With {.SetNummer = olJournal.Body.GetSubString(Localize.LocAnrMon.strJournalBodyStart, Dflt1NeueZeile)}

                ' Entweder erst eingebetteten Kontakt suchen, oder nach vCard suchen.
                aktKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object()))

                If aktKontakt Is Nothing Then
                    ' vCard aus dem .Body herausfiltern
                    vCard = $"{DfltBegin_vCard}{ .Body.GetSubString(DfltBegin_vCard, DfltEnd_vCard)}{DfltEnd_vCard}"

                    'Wenn keine vCard im Body gefunden
                    If vCard.AreNotEqual($"{DfltBegin_vCard}{DfltStrErrorMinusOne}{DfltEnd_vCard}") Then
                        'vCard gefunden
                        aktKontakt = ErstelleKontakt(vCard, TelNr, False)
                    End If
                End If

                If aktKontakt IsNot Nothing Then
                    Wählbox(aktKontakt)
                Else
                    Wählbox(TelNr)
                End If

            End If
        End With
    End Sub

    ''' <summary>
    ''' Wählen aus der Wahlwiederholungs- oder Rückrufliste bzw. Wählvorgang aus einem Telefonat-Objekt.
    ''' </summary>
    ''' <param name="DialTelefonat">Telefonat</param>
    Friend Overloads Sub WählboxStart(DialTelefonat As Telefonat)

        With DialTelefonat
            ' Kontakt aus Telefonat ermitteln
            If .OlKontakt Is Nothing AndAlso (.OutlookKontaktID.IsNotStringEmpty And .OutlookStoreID.IsNotStringEmpty) Then
                ' Es gibt eine KontaktID und StoreID: Ermittle den Kontakt
                .OlKontakt = GetOutlookKontakt(.OutlookKontaktID, .OutlookStoreID)
            End If

            If .OlKontakt IsNot Nothing Then
                Wählbox(.OlKontakt, .GegenstelleTelNr)
            Else
                Wählbox(.GegenstelleTelNr)
            End If

        End With
    End Sub

    Friend Overloads Sub WählboxStart(DialVIP As VIPEntry)

        With DialVIP
            ' Kontakt aus telefinat ermitteln
            If .OlContact Is Nothing AndAlso (.StoreID.IsNotStringEmpty And .EntryID.IsNotStringEmpty) Then
                ' Es gibt eine KontaktID und StoreID: Ermittle den Kontakt
                .OlContact = GetOutlookKontakt(.EntryID, .StoreID)
            End If

            Wählbox(.OlContact)
        End With
    End Sub

    Friend Overloads Sub WählboxStart(Kontakt As FBoxAPI.Contact)

        WPFWindow = New WählclientWPF
        With WPFWindow
            .DataContext = New WählClientViewModel With {.Wählclient = Me,
                                                         .Instance = WPFWindow.Dispatcher,
                                                         .IsContactDial = True,
                                                         .SetOutlookFBoxXMLKontakt = Kontakt}

            .Show()
        End With
    End Sub

    ''' <summary>
    ''' Startet das Wählen auf Basis eines Outlook Kontaktes
    ''' </summary>
    ''' <param name="oContact">Der Outlook-Kontakt, welcher angerufen werden soll</param>
    Private Sub Wählbox(oContact As Outlook.ContactItem)
        If oContact IsNot Nothing Then

            WPFWindow = New WählclientWPF

            With WPFWindow
                .DataContext = New WählClientViewModel With {.Wählclient = Me,
                                                             .Instance = WPFWindow.Dispatcher,
                                                             .IsContactDial = True,
                                                             .SetOutlookKontakt = oContact}

                .Show()
            End With

        Else
            NLogger.Error("Der Outlook-Kontakt ist nicht vorhanden.")
        End If
    End Sub

    ''' <summary>
    ''' Startet das Wählen auf Basis eines Outlook Kontaktes. Die zuletzt angerufene Telefonnummer wird markiert.
    ''' </summary>
    ''' <param name="oContact">Der Outlook-Kontakt, welcher angerufen werden soll</param>
    Private Sub Wählbox(oContact As Outlook.ContactItem, TelNr As Telefonnummer)
        If oContact IsNot Nothing Then

            WPFWindow = New WählclientWPF

            With WPFWindow
                .DataContext = New WählClientViewModel With {.Wählclient = Me,
                                                             .Instance = WPFWindow.Dispatcher,
                                                             .IsContactDial = True,
                                                             .ZuletztGewählteTelNr = TelNr,
                                                             .SetOutlookKontakt = oContact}

                .Show()
            End With

        Else
            NLogger.Error("Der Outlook-Kontakt ist nicht vorhanden.")
        End If
    End Sub

    ''' <summary>
    ''' Startet das Wählen auf Basis eines Outlook Exchange Users
    ''' </summary>
    ''' <param name="oExchangeNutzer">Der Exchange User, welcher angerufen werden soll</param>
    Private Sub Wählbox(oExchangeNutzer As Outlook.ExchangeUser)

        If oExchangeNutzer IsNot Nothing Then
            WPFWindow = New WählclientWPF

            With WPFWindow
                .DataContext = New WählClientViewModel With {.Wählclient = Me,
                                                             .Instance = WPFWindow.Dispatcher,
                                                             .IsContactDial = True,
                                                             .SetOutlookExchangeUser = oExchangeNutzer}

                .Show()
            End With


        Else
            NLogger.Error("Der Outlook-oExchangeUser ist nicht vorhanden.")
        End If
    End Sub

    ''' <summary>
    ''' Startet das Wählen auf Basis einer Telefonnummer 
    ''' </summary>
    ''' <param name="TelNr">Die Telefonnummer, welche angerufen werden soll</param>
    Private Sub Wählbox(TelNr As Telefonnummer)

        If TelNr IsNot Nothing Then
            WPFWindow = New WählclientWPF

            With WPFWindow
                .DataContext = New WählClientViewModel With {.Wählclient = Me,
                                                             .Instance = WPFWindow.Dispatcher,
                                                             .IsContactDial = False,
                                                             .SetTelNr = TelNr}

                .Show()
            End With

        Else
            NLogger.Error("Die Telefonnummer ist nicht vorhanden.")
        End If
    End Sub

    ''' <summary>
    ''' Startet das Wählen als Direktwahl 
    ''' </summary>
    Private Sub Wählbox()

        WPFWindow = New WählclientWPF

        With WPFWindow
            .DataContext = New WählClientViewModel With {.Wählclient = Me,
                                                         .Instance = WPFWindow.Dispatcher,
                                                         .IsContactDial = False}

            .Show()
        End With

    End Sub
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub


    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
