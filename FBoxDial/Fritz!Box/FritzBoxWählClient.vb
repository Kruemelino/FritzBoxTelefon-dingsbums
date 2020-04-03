Imports System.Collections
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop

Public Class FritzBoxWählClient
    Implements IDisposable

#Region "Properties"
    Private Shared Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Private ReadOnly Property PFBLinkTelData As String = FritzBoxDefault.PFBLinkBasis & "/data.lua"
    Private ReadOnly Property PFBLinkDialSetDialPort(ByVal sSID As String, ByVal DialPort As String) As String
        Get
            Return String.Format("&xhr=1&clicktodial=on&port={0}{1}&back_to_page=%2Ffon_num%2Fdial_fonbook.lua&btn_apply=&lang=de&page=telDial", DialPort, sSID)
        End Get
    End Property
    Private ReadOnly Property PFBLinkDial(ByVal sSID As String, ByVal DialCode As String, ByVal HangUp As Boolean) As String
        Get
            Return String.Format("{0}/fon_num/foncalls_list.lua?{1}{2}", FritzBoxDefault.PFBLinkBasis, sSID, If(HangUp, "&hangup=", "&dial=" & DialCode))
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

    Private ListFormWählbox As List(Of FormWählclient)

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

        SOAPDial = False

        Using fbSOAP As New FritzBoxServices
            ' DialPort setzen, wenn erforderlich
            DialPortEingestellt = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialGetConfig").Item("NewX_AVM-DE_PhoneName").ToString.AreEqual(Telefon.UPnPDialport)
            If Not DialPortEingestellt Then
                ' Das Telefon der Fritz!Box Wählhilfe muss geändert werden
                StatusMeldung = PWählClientDialStatus("SOAPDial", PWählClientStatusDialPort, Telefon.UPnPDialport)
                RaiseEvent SetStatus(StatusMeldung)

                InPutData.Add("NewX_AVM-DE_PhoneName", Telefon.UPnPDialport)
                OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialSetConfig", InPutData)

                If OutPutData.Contains("Error") Then
                    DialPortEingestellt = False
                    StatusMeldung = PWählClientDialStatus("SOAPDial", PWählClientDialFehler, OutPutData("Error").ToString.Replace("CHR(60)", "<").Replace("CHR(62)", ">"))
                    NLogger.Error(StatusMeldung)
                    RaiseEvent SetStatus(StatusMeldung)
                Else
                    ' Überprüfe, ob der Dialport tatsächlich geändert wurde:
                    DialPortEingestellt = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialGetConfig").Item("NewX_AVM-DE_PhoneName").ToString.AreEqual(Telefon.UPnPDialport)
                    If Not DialPortEingestellt Then
                        StatusMeldung = PWählClientDialStatus("SOAPDial", PWählClientStatusSOAPDialPortFehler, Telefon.UPnPDialport)
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
                SOAPDial = OutPutData.Contains("Error")

                If Not SOAPDial Then
                    StatusMeldung = PWählClientDialStatus("SOAPDial", PWählClientDialFehler, OutPutData("Error").ToString.Replace("CHR(60)", "<").Replace("CHR(62)", ">"))
                    NLogger.Error(StatusMeldung)
                    RaiseEvent SetStatus(StatusMeldung)
                End If
            End If
        End Using
    End Function
#End Region

#Region "Wählen per WebCLient"
    Friend Async Function WebCientDial(ByVal sDialCode As String, ByVal Telefon As Telefoniegerät, ByVal Auflegen As Boolean) As Threading.Tasks.Task(Of Boolean)

        Dim fbAntwort As String
        Dim DialPortEingestellt As Boolean

        Dim SessionID As String

        SessionID = GetSessionID

        ' WebCientDial = PWählClientDialError1
        Using fbQuery As New FritzBoxQuery
            ' DialPort setzen, wenn erforderlich
            fbAntwort = Await fbQuery.FritzBoxQuery(SessionID, "DialPort=telcfg:settings/DialPort")

            DialPortEingestellt = fbAntwort.Contains(CStr(Telefon.Dialport))
            If Not DialPortEingestellt Then
                ' Das Telefon der Fritz!Box Wählhilfe muss geändert werden
                RaiseEvent SetStatus(PWählClientDialStatus("WebCientDial", PWählClientStatusDialPort, CStr(Telefon.Dialport)))

                ' per HTTP-POST Dialport ändern
                fbAntwort = Await HTTPPost(PFBLinkTelData, PFBLinkDialSetDialPort(SessionID, CStr(Telefon.Dialport)), XMLData.POptionen.PEncodingFritzBox)
                ' {"data":{"btn_apply":"twofactor","twofactor":"button,dtmf;3170"}}
                If fbAntwort.Contains("twofactor") Then
                    DialPortEingestellt = False
                    MsgBox(PWarnung2FA, MsgBoxStyle.Critical, "WebCientDial")
                Else
                    ' Überprüfe, ob der Dialport tatsächlich geändert wurde:
                    fbAntwort = Await fbQuery.FritzBoxQuery(SessionID, "DialPort=telcfg:settings/DialPort")
                    DialPortEingestellt = fbAntwort.Contains(CStr(Telefon.Dialport))
                End If
            End If

            ' Wählen, wenn der Dialport passt
            If DialPortEingestellt Then
                ' Senden des Wählkomandos
                ' Tipp von Pikachu: Umwandlung von # und *, da ansonsten die Telefoncodes verschluckt werden. 
                ' Alternativ ein URLEncode (Uri.EscapeDataString(Link).Replace("%20", "+")), 
                ' was aber in der Funktion httpGET zu einem Fehler bei dem Erstellen der neuen URI führt.

                ' Senden des Wählkomandos
                fbAntwort = Await HTTPGet(PFBLinkDial(SessionID, sDialCode.Replace("#", "%23").Replace("*", "%2A"), Auflegen), XMLData.POptionen.PEncodingFritzBox)

                ' Die Rückgabe ist der JSON - Wert "dialing"
                ' Bei der Wahl von Telefonnummern ist es ein {"dialing": "0123456789#"}
                ' Bei der Wahl von Telefoncodes ist es ein {"dialing": "#96*0*"}
                ' Bei der Wahl Des Hangup ist es ein {"dialing": false} ohne die umschließenden Anführungszeichen" 
                ' NEU {"dialing":true,"err":0}
                ' NEU {"dialing":false,"err":0}
                If fbAntwort = "{""dialing"":true,""err"":0}" Or (fbAntwort.Contains("""dialing""") And fbAntwort.Contains(If(Auflegen, "false", sDialCode))) Then
                    Return True
                Else
                    NLogger.Error("{0}: {1} {2}", "WebCientDial", "Fehler", fbAntwort.Replace(vbLf, ""))
                    Return False
                End If
            Else
                Return False
            End If
        End Using
    End Function
#End Region

#Region "Dialog Wähldialog"
    ''' <summary>
    ''' wird durch das Symbol 'Wählen' in der 'FritzBox'-Symbolleiste ausgeführt
    ''' </summary>
    ''' <param name="olAuswahl">Die aktuelle Auswahl eines Outlook-Elementes</param>
    Friend Overloads Sub WählboxStart(ByVal olAuswahl As Outlook.Selection, ByVal DirektWahl As Boolean)

        If DirektWahl Then
            Wählbox(Nothing, Nothing, True)
        Else
            ' Ist überhaupt etwas ausgewählt?
            If olAuswahl.Count.AreEqual(1) Then

                Select Case True
                    Case TypeOf olAuswahl.Item(1) Is Outlook.ContactItem   ' ist aktuelles Fenster ein Kontakt?
                        Wählbox(CType(olAuswahl.Item(1), Outlook.ContactItem), Nothing, False)
                    Case TypeOf olAuswahl.Item(1) Is Outlook.JournalItem   ' ist aktuelles Fenster ein Journal?
                        ' Es wurde ein Journaleintrag gewählt!
                        WählboxStart(CType(olAuswahl.Item(1), Outlook.JournalItem))
                    Case TypeOf olAuswahl.Item(1) Is Outlook.MailItem      ' ist aktuelles Fenster ein Mail?
                        ' Es wurde eine Mail ausgewählt
                        ' Den zur Email-Adresse gehörigen Kontakt suchen
                        WählboxStart(CType(olAuswahl.Item(1), Outlook.MailItem))
                    Case Else
                        ' Nix tun
                        MsgBox(PWählClientAuswahlFalsch, MsgBoxStyle.Exclamation, "WählboxStart")
                End Select
            Else
                MsgBox(PWählClientAuswahlFalsch, MsgBoxStyle.Exclamation, "WählboxStart")
            End If
        End If
    End Sub

    Friend Overloads Sub WählboxStart(ByVal olInsp As Outlook.Inspector)

        Select Case True
            Case TypeOf olInsp.CurrentItem Is Outlook.ContactItem   ' ist aktuelles Fenster ein Kontakt?
                Wählbox(CType(olInsp.CurrentItem, Outlook.ContactItem), Nothing, False)
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
        Dim aktKontakt As Outlook.ContactItem
        Dim EMail As String = GetSmtpAddress(ContactCard)

        If EMail.IsNotStringEmpty Then

            aktKontakt = KontaktSuche(EMail)

            If aktKontakt IsNot Nothing Then
                Wählbox(aktKontakt, Nothing, False)
            Else
                MsgBox(PWählClientEMailunbekannt(EMail), MsgBoxStyle.Information, "WählboxStart")
            End If
        End If

        ContactCard.ReleaseComObject
    End Sub

    Friend Overloads Sub WählboxStart(ByVal aktMail As Outlook.MailItem)
        Dim aktKontakt As Outlook.ContactItem
        If aktMail.SenderEmailAddress.IsNotStringEmpty Then

            aktKontakt = KontaktSuche(aktMail.SenderEmailAddress)

            If aktKontakt IsNot Nothing Then
                Wählbox(aktKontakt, Nothing, False)
            Else
                MsgBox(PWählClientEMailunbekannt(aktMail.SenderEmailAddress), MsgBoxStyle.Information, "WählboxStart")
            End If

        End If

        aktMail.ReleaseComObject
    End Sub

    Friend Overloads Sub WählboxStart(ByVal olJournal As Outlook.JournalItem)

        With olJournal
            If Not .Body.Contains(PDfltStringUnbekannt) And .Categories.Contains(PDfltJournalKategorie) Then
                Dim aktKontakt As Outlook.ContactItem
                Dim vCard As String
                Dim TelNr As Telefonnummer

                ' Telefonnummer aus dem Body ermitteln
                TelNr = New Telefonnummer With {.SetNummer = olJournal.Body.GetSubString(PDfltJournalBodyStart, PDflt1NeueZeile)}

                ' Entweder erst eingebetteten Kontakt suchen, oder nach vCard suchen.
                aktKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object()))

                If aktKontakt Is Nothing Then
                    ' vCard aus dem .Body herausfiltern
                    vCard = PDfltBegin_vCard & .Body.GetSubString(PDfltBegin_vCard, PDfltEnd_vCard) & PDfltEnd_vCard

                    'Wenn keine vCard im Body gefunden
                    If vCard.AreNotEqual(PDfltBegin_vCard & PDfltStrErrorMinusOne & PDfltEnd_vCard) Then
                        'vCard gefunden
                        aktKontakt = ErstelleKontakt(PDfltStringEmpty, PDfltStringEmpty, vCard, TelNr, False)
                    End If
                End If

                Wählbox(aktKontakt, TelNr, False)

            End If
        End With
    End Sub

    Friend Overloads Sub WählboxStart(ByVal DialTelefonat As Telefonat)

        With DialTelefonat
            ' Kontakt aus telefinat ermitteln
            If .OlKontakt Is Nothing Then
                ' gibt es eine KontaktID und StoreID
                If .OutlookStoreID.IsNotStringEmpty And .OutlookKontaktID.IsNotStringEmpty Then
                    .OlKontakt = GetOutlookKontakt(.OutlookKontaktID, .OutlookStoreID)
                End If
            End If

            Wählbox(.OlKontakt, .GegenstelleTelNr, False)

        End With
    End Sub

    Friend Overloads Sub WählboxStart(ByVal DialVIP As VIPEntry)

        With DialVIP
            ' Kontakt aus telefinat ermitteln
            If .OlContact Is Nothing Then
                ' gibt es eine KontaktID und StoreID
                If .StoreID.IsNotStringEmpty And .EntryID.IsNotStringEmpty Then
                    .OlContact = GetOutlookKontakt(.EntryID, .StoreID)
                End If
            End If

            Wählbox(.OlContact, Nothing, False)
        End With
    End Sub

    Private Sub Wählbox(ByVal oContact As Outlook.ContactItem, ByVal TelNr As Telefonnummer, ByVal DirektWahl As Boolean)

        ' Es soll nur ein Formular anzeigbar sein.
        If ListFormWählbox Is Nothing Then ListFormWählbox = New List(Of FormWählclient)

        Dim fWählClient As FormWählclient

        If ListFormWählbox.Count.IsZero Then
            fWählClient = New FormWählclient(Me)
            ListFormWählbox.Add(fWählClient)

            With fWählClient
                If DirektWahl Then
                    .SetDirektwahl()
                Else
                    If oContact IsNot Nothing Then
                        .SetOutlookKontakt(oContact)
                    Else
                        .SetTelefonnummer(TelNr)
                    End If
                End If
                .Show()
                .BringToFront()
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
