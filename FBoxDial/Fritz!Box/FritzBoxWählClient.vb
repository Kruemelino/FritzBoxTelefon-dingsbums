﻿Imports System.Collections
Imports Microsoft.Office.Interop

Public Class FritzBoxWählClient
    Implements IDisposable

#Region "Properties"
    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property PFBLinkTelData As String = FritzBoxDefault.FBLinkBasis & "/data.lua"
    Private ReadOnly Property PFBLinkDialSetDialPort(ByVal sSID As String, ByVal DialPort As String) As String
        Get
            Return String.Format("&xhr=1&clicktodial=on&port={0}{1}&back_to_page=%2Ffon_num%2Fdial_fonbook.lua&btn_apply=&lang=de&page=telDial", DialPort, sSID)
        End Get
    End Property
    Private ReadOnly Property PFBLinkDial(ByVal sSID As String, ByVal DialCode As String, ByVal HangUp As Boolean) As String
        Get
            Return String.Format("{0}/fon_num/foncalls_list.lua?{1}{2}", FritzBoxDefault.FBLinkBasis, sSID, If(HangUp, "&hangup=", "&dial=" & DialCode))
        End Get
    End Property
#End Region

#Region "Event"
    ''' <summary>
    ''' Event zum setzen des Status
    ''' </summary>
    ''' <param name="Status">Text, welcher Angezeigt werden soll</param>
    Friend Event SetStatus(ByVal Status As String)
#End Region

    Private ListWählboxWPF As List(Of WählclientWPF)
#Region "Wählen per SOAP"
    ''' <summary>
    ''' Initialisiert den Wählvorgang der Fritz!Box Wählhilfe.
    ''' </summary>
    ''' <param name="sDialCode">Die zu Wählende Nummer</param>
    ''' <param name="Telefon">Das ausgehende Telefon</param>
    ''' <param name="Auflegen">Angabe, ob der Verbindungsaufbau abgebrochen werden soll.</param>
    ''' <returns></returns>
    Friend Function SOAPDial(ByVal sDialCode As String, ByVal Telefon As Telefoniegerät, ByVal Auflegen As Boolean) As Boolean
        Dim DialPortEingestellt As Boolean
        Dim InPutData As New Hashtable
        Dim OutPutData As Hashtable

        Dim StatusMeldung As String

        Using fbSOAP As New FritzBoxSOAP
            ' DialPort setzen, wenn erforderlich

            OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialGetConfig")
            DialPortEingestellt = OutPutData.Item("NewX_AVM-DE_PhoneName").ToString.AreEqual(Telefon.UPnPDialport)
            If Not DialPortEingestellt Then
                ' Das Telefon der Fritz!Box Wählhilfe muss geändert werden
                StatusMeldung = WählClientDialStatus("SOAPDial", WählClientStatusDialPort, Telefon.UPnPDialport)
                RaiseEvent SetStatus(StatusMeldung)

                InPutData.Add("NewX_AVM-DE_PhoneName", Telefon.UPnPDialport)
                OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialSetConfig", InPutData)

                If OutPutData.Contains("Error") Then
                    DialPortEingestellt = False
                    StatusMeldung = WählClientDialStatus("SOAPDial", WählClientDialFehler, OutPutData("Error").ToString.Replace("CHR(60)", "<").Replace("CHR(62)", ">"))
                    NLogger.Error(StatusMeldung)
                    RaiseEvent SetStatus(StatusMeldung)
                Else
                    ' Überprüfe, ob der Dialport tatsächlich geändert wurde:
                    OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialGetConfig")
                    DialPortEingestellt = OutPutData.Item("NewX_AVM-DE_PhoneName").ToString.AreEqual(Telefon.UPnPDialport)
                    If Not DialPortEingestellt Then
                        StatusMeldung = WählClientDialStatus("SOAPDial", WählClientStatusSOAPDialPortFehler, Telefon.UPnPDialport)
                        NLogger.Error(StatusMeldung)
                        RaiseEvent SetStatus(StatusMeldung)
                    End If
                End If
            End If

            ' Wählen, wenn der Dialport passt
            If DialPortEingestellt Then
                ' Senden des Wählkomandos
                InPutData.Clear()
                If Auflegen Then
                    OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialHangup")
                Else
                    InPutData.Add("NewX_AVM-DE_PhoneNumber", sDialCode)
                    OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialNumber", InPutData)
                End If

                ' Rückmeldung, ob das Wählen erfolgreich war

                If OutPutData.Contains("Error") Then
                    StatusMeldung = WählClientDialStatus("SOAPDial", WählClientDialFehler, OutPutData("Error").ToString.Replace("CHR(60)", "<").Replace("CHR(62)", ">"))
                    NLogger.Error(StatusMeldung)
                    RaiseEvent SetStatus(StatusMeldung)
                    Return False
                Else
                    Return True
                End If
            Else
                Return False
            End If
        End Using
    End Function
#End Region

    '#Region "Wählen per WebCLient"
    '    Friend Function WebCientDial(ByVal sDialCode As String, ByVal Telefon As Telefoniegerät, ByVal Auflegen As Boolean) As Boolean

    '        Dim fbAntwort As String
    '        Dim DialPortEingestellt As Boolean

    '        Dim SessionID As String

    '        SessionID = GetSessionID

    '        ' WebCientDial = PWählClientDialError1
    '        Using fbQuery As New FritzBoxQuery
    '            ' DialPort setzen, wenn erforderlich
    '            fbAntwort = fbQuery.FritzBoxQuery(SessionID, "DialPort=telcfg:settings/DialPort")

    '            DialPortEingestellt = fbAntwort.Contains(CStr(Telefon.Dialport))
    '            If Not DialPortEingestellt Then
    '                ' Das Telefon der Fritz!Box Wählhilfe muss geändert werden
    '                RaiseEvent SetStatus(PWählClientDialStatus("WebCientDial", PWählClientStatusDialPort, CStr(Telefon.Dialport)))

    '                ' per HTTP-POST Dialport ändern
    '                fbAntwort = HTTPPost(PFBLinkTelData, PFBLinkDialSetDialPort(SessionID, CStr(Telefon.Dialport)), XMLData.POptionen.PEncodingFritzBox)
    '                ' {"data":{"btn_apply":"twofactor","twofactor":"button,dtmf;3170"}}
    '                If fbAntwort.Contains("twofactor") Then
    '                    DialPortEingestellt = False
    '                    MsgBox(PWarnung2FA, MsgBoxStyle.Critical, "WebCientDial")
    '                Else
    '                    ' Überprüfe, ob der Dialport tatsächlich geändert wurde:
    '                    fbAntwort = fbQuery.FritzBoxQuery(SessionID, "DialPort=telcfg:settings/DialPort")
    '                    DialPortEingestellt = fbAntwort.Contains(CStr(Telefon.Dialport))
    '                End If
    '            End If

    '            ' Wählen, wenn der Dialport passt
    '            If DialPortEingestellt Then
    '                ' Senden des Wählkomandos
    '                ' Tipp von Pikachu: Umwandlung von # und *, da ansonsten die Telefoncodes verschluckt werden. 
    '                ' Alternativ ein URLEncode (Uri.EscapeDataString(Link).Replace("%20", "+")), 
    '                ' was aber in der Funktion httpGET zu einem Fehler bei dem Erstellen der neuen URI führt.

    '                ' Senden des Wählkomandos
    '                fbAntwort = HTTPGet(PFBLinkDial(SessionID, sDialCode.Replace("#", "%23").Replace("*", "%2A"), Auflegen), XMLData.POptionen.PEncodingFritzBox)

    '                ' Die Rückgabe ist der JSON - Wert "dialing"
    '                ' Bei der Wahl von Telefonnummern ist es ein {"dialing": "0123456789#"}
    '                ' Bei der Wahl von Telefoncodes ist es ein {"dialing": "#96*0*"}
    '                ' Bei der Wahl Des Hangup ist es ein {"dialing": false} ohne die umschließenden Anführungszeichen" 
    '                ' NEU {"dialing":true,"err":0}
    '                ' NEU {"dialing":false,"err":0}
    '                If fbAntwort = "{""dialing"":true,""err"":0}" Or (fbAntwort.Contains("""dialing""") And fbAntwort.Contains(If(Auflegen, "false", sDialCode))) Then
    '                    Return True
    '                Else
    '                    NLogger.Error("{0}: {1} {2}", "WebCientDial", "Fehler", fbAntwort.Replace(vbLf, ""))
    '                    Return False
    '                End If
    '            Else
    '                Return False
    '            End If
    '        End Using
    '    End Function
    '#End Region

#Region "Dialog Wähldialog"
    ''' <summary>
    ''' wird durch das Symbol 'Wählen' in der 'FritzBox'-Symbolleiste ausgeführt
    ''' </summary>
    ''' <param name="olAuswahl">Die aktuelle Auswahl eines Outlook-Elementes</param>
    Friend Overloads Sub WählboxStart(ByVal olAuswahl As Outlook.Selection)

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
                    MsgBox(WählClientAuswahlFalsch, MsgBoxStyle.Exclamation, "WählboxStart")
            End Select
        Else
            MsgBox(WählClientAuswahlFalsch, MsgBoxStyle.Exclamation, "WählboxStart")
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
    Friend Overloads Sub WählboxStart(ByVal olInsp As Outlook.Inspector)

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
    Friend Overloads Sub WählboxStart(ByVal ContactCard As Microsoft.Office.Core.IMsoContactCard)

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
            aktKontakt.ReleaseComObject

            ' B: Suche den ExchangeNutzer
            Dim aktExchangeNutzer As Outlook.ExchangeUser = KontaktSucheExchangeUser(ContactCard)
            If aktExchangeNutzer IsNot Nothing Then
                ' Wenn ein ExchangeUser gefunden wurde so wähle diesen an.
                Wählbox(aktExchangeNutzer)
            Else
                MsgBox(WählClientEMailunbekannt(ContactCard.Address), MsgBoxStyle.Information, "WählboxStart")
            End If
        End If

        ContactCard.ReleaseComObject
    End Sub

    ''' <summary>
    ''' Wählen aus einer E-Mail
    ''' </summary>
    ''' <param name="aktMail">Die E-Mail, deren Absender angerufen werden soll</param>
    Friend Overloads Sub WählboxStart(ByVal aktMail As Outlook.MailItem)

        Dim SMTPAdresse As String = GetSenderSMTPAddress(aktMail)

        ' Es gibt zwei Möglichkeiten:
        ' A: Ein klassischer Kontakt ist hinterlegt
        ' B: Ein Exchange-User existiert. 

        If SMTPAdresse.IsNotStringEmpty Then
            ' A: Führe zunächst die Absendersuche nach Outlook-Kontakten durch
            Dim aktKontakt As Outlook.ContactItem = KontaktSuche(SMTPAdresse)

            If aktKontakt IsNot Nothing Then
                ' Wenn ein Kontakt gefunden wurde so wähle diesen an.
                Wählbox(aktKontakt)
            Else
                ' Es wurde kein Kontakt gefunden. 
                aktKontakt.ReleaseComObject

                ' B: Suche den ExchangeNutzer
                Dim aktExchangeNutzer As Outlook.ExchangeUser = KontaktSucheExchangeUser(SMTPAdresse)
                If aktExchangeNutzer IsNot Nothing Then
                    ' Wenn ein ExchangeUser gefunden wurde so wähle diesen an.
                    Wählbox(aktExchangeNutzer)
                Else
                    MsgBox(WählClientEMailunbekannt(SMTPAdresse), MsgBoxStyle.Information, "WählboxStart")
                End If

            End If
        End If

        aktMail.ReleaseComObject
    End Sub

    Friend Overloads Sub WählboxStart(ByVal olJournal As Outlook.JournalItem)

        With olJournal
            If Not .Body.Contains(DfltStringUnbekannt) And .Categories.Contains(DfltJournalKategorie) Then
                Dim aktKontakt As Outlook.ContactItem
                Dim vCard As String
                Dim TelNr As Telefonnummer

                ' Telefonnummer aus dem Body ermitteln
                TelNr = New Telefonnummer With {.SetNummer = olJournal.Body.GetSubString(PfltJournalBodyStart, Dflt1NeueZeile)}

                ' Entweder erst eingebetteten Kontakt suchen, oder nach vCard suchen.
                aktKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object()))

                If aktKontakt Is Nothing Then
                    ' vCard aus dem .Body herausfiltern
                    vCard = DfltBegin_vCard & .Body.GetSubString(DfltBegin_vCard, DfltEnd_vCard) & DfltEnd_vCard

                    'Wenn keine vCard im Body gefunden
                    If vCard.AreNotEqual(DfltBegin_vCard & DfltStrErrorMinusOne & DfltEnd_vCard) Then
                        'vCard gefunden
                        aktKontakt = ErstelleKontakt(DfltStringEmpty, DfltStringEmpty, vCard, TelNr, False)
                    End If
                End If

                If aktKontakt Is Nothing Then
                    Wählbox(aktKontakt)
                Else
                    Wählbox(TelNr)
                End If

            End If
        End With
    End Sub

    Friend Overloads Sub WählboxStart(ByVal DialTelefonat As Telefonat)

        With DialTelefonat
            ' Kontakt aus Telefonat ermitteln
            If .OlKontakt Is Nothing AndAlso (.OutlookKontaktID.IsNotStringEmpty And .OutlookStoreID.IsNotStringEmpty) Then
                ' Es gibt eine KontaktID und StoreID: Ermittle den Kontakt
                .OlKontakt = GetOutlookKontakt(.OutlookKontaktID, .OutlookStoreID)
            End If

            If .OlKontakt IsNot Nothing Then
                Wählbox(.OlKontakt)
            Else
                Wählbox(.GegenstelleTelNr)
            End If

        End With
    End Sub

    Friend Overloads Sub WählboxStart(ByVal DialVIP As VIPEntry)

        With DialVIP
            ' Kontakt aus telefinat ermitteln
            If .OlContact Is Nothing AndAlso (.StoreID.IsNotStringEmpty And .EntryID.IsNotStringEmpty) Then
                ' Es gibt eine KontaktID und StoreID: Ermittle den Kontakt
                .OlContact = GetOutlookKontakt(.EntryID, .StoreID)
            End If

            Wählbox(.OlContact)
        End With
    End Sub

    ''' <summary>
    ''' Startet das Wählen auf Basis eines Outlook Kontaktes
    ''' </summary>
    ''' <param name="oContact">Der Outlook-Kontakt, welcher angerufen werden soll</param>
    Private Sub Wählbox(ByVal oContact As Outlook.ContactItem)

        If oContact IsNot Nothing Then
            ' Es soll nur ein Formular anzeigbar sein.
            If ListWählboxWPF Is Nothing Then ListWählboxWPF = New List(Of WählclientWPF)

            Dim fWählClient As WählclientWPF

            If ListWählboxWPF.Count.IsZero Then
                fWählClient = New WählclientWPF

                ListWählboxWPF.Add(fWählClient)
                With fWählClient
                    .SetOutlookKontakt(oContact)
                    .Show()
                End With

            End If
        Else
            NLogger.Error("Der Outlook-Kontakt ist nicht vorhanden.")
        End If
    End Sub

    Private Sub Wählbox(ByVal oExchangeNutzer As Outlook.ExchangeUser)

        If oExchangeNutzer IsNot Nothing Then
            ' Es soll nur ein Formular anzeigbar sein.
            If ListWählboxWPF Is Nothing Then ListWählboxWPF = New List(Of WählclientWPF)

            Dim fWählClient As WählclientWPF

            If ListWählboxWPF.Count.IsZero Then
                fWählClient = New WählclientWPF

                ListWählboxWPF.Add(fWählClient)
                With fWählClient
                    .SetOutlookKontakt(oExchangeNutzer)
                    .Show()
                End With

            End If
        Else
            NLogger.Error("Der Outlook-oExchangeUser ist nicht vorhanden.")
        End If
    End Sub

    ''' <summary>
    ''' Startet das Wählen auf Basis einer Telefonnummer 
    ''' </summary>
    ''' <param name="TelNr">Die Telefonnummer, welche angerufen werden soll</param>
    Private Sub Wählbox(ByVal TelNr As Telefonnummer)

        If TelNr IsNot Nothing Then

            ' Es soll nur ein Formular anzeigbar sein.
            If ListWählboxWPF Is Nothing Then ListWählboxWPF = New List(Of WählclientWPF)

            Dim fWählClient As WählclientWPF

            If ListWählboxWPF.Count.IsZero Then
                fWählClient = New WählclientWPF

                ListWählboxWPF.Add(fWählClient)
                With fWählClient
                    .SetTelefonnummer(TelNr)
                    .Show()
                End With
            End If
        Else
            NLogger.Error("Die Telefonnummer ist nicht vorhanden.")
        End If
    End Sub

    ''' <summary>
    ''' Startet das Wählen als Direktwahl 
    ''' </summary>
    Private Sub Wählbox()
        ' Es soll nur ein Formular anzeigbar sein.
        If ListWählboxWPF Is Nothing Then ListWählboxWPF = New List(Of WählclientWPF)

        Dim fWählClient As WählclientWPF

        If ListWählboxWPF.Count.IsZero Then
            fWählClient = New WählclientWPF

            ListWählboxWPF.Add(fWählClient)
            With fWählClient
                .SetDirektwahl()
                .Show()
            End With
        End If
    End Sub
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
