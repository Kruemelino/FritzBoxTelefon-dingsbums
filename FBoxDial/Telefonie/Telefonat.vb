Imports System.Xml.Serialization
Imports Microsoft.Office.Interop
<Serializable()> Public Class Telefonat
    Implements IEquatable(Of Telefonat)
    Implements IDisposable

    Private Shared Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

#Region "Eigenschaften"
    <XmlAttribute> Public Property ID As Integer
    <XmlIgnore> Public Property EigeneTelNr As Telefonnummer
    <XmlElement> Public Property OutEigeneTelNr As String
    <XmlElement> Public Property GegenstelleTelNr As Telefonnummer
    <XmlIgnore> Public Property TelGerät As Telefoniegerät
    <XmlIgnore> Public Property RINGGeräte As List(Of Telefoniegerät)
    <XmlElement> Public Property NebenstellenNummer As Integer
    <XmlElement> Public Property AnschlussID As String

    <XmlElement> Public Property ZeitBeginn As Date
    <XmlElement> Public Property ZeitVerbunden As Date
    <XmlElement> Public Property ZeitEnde As Date
    <XmlElement> Public Property Dauer As Integer
    <XmlElement> Public Property RingTime As Double
    <XmlElement> Public Property AnrufRichtung As Integer

    <XmlIgnore> Public Property Aktiv As Boolean
    <XmlIgnore> Public Property Beendet As Boolean
    <XmlAttribute> Public Property NrUnterdrückt As Boolean
    <XmlAttribute> Public Property Angenommen As Boolean
    '<XmlAttribute> Public Property Verpasst As Boolean
    <XmlIgnore> Public Property AnrMonAusblenden As Boolean

    <XmlElement> Public Property OutlookKontaktID As String
    <XmlElement> Public Property OutlookStoreID As String
    <XmlElement> Public Property VCard As String

    <XmlElement> Public Property FBTelBookKontakt As FritzBoxXMLKontakt
    <XmlElement> Public Property Anrufer As String
    <XmlElement> Public Property Firma As String
    <XmlIgnore> Public Property OlKontakt() As Outlook.ContactItem
    <XmlIgnore> Public Property AnrMonPopUp As Popup

    ''' <summary>
    '''         0        ; 1  ;2;    3     ;  4   ; 5  ; 6
    ''' 23.06.18 13:20:24;RING;1;0123456789;987654;SIP4;
    ''' </summary>
    <XmlIgnore> Friend WriteOnly Property SetAnrMonRING As String()
        Set(FBStatus As String())
            AnrufRichtung = AnrufRichtungen.Eingehend

            For i = LBound(FBStatus) To UBound(FBStatus)
                Select Case i
                    Case 0 ' Uhrzeit des Telefonates - Startzeit
                        ZeitBeginn = CDate(FBStatus(i))
                    Case 2 ' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
                        ID = CInt(FBStatus(i))
                    Case 3 ' Eingehende (anrufende) Telefonnummer
                        GegenstelleTelNr = New Telefonnummer With {.SetNummer = FBStatus(i)}

                        NrUnterdrückt = GegenstelleTelNr.Unbekannt
                    Case 4 ' Eigene (angerufene) Telefonnummer, MSN
                        Dim j As Integer = i ' Vermeide Fehler: BC42324 Using the iteration variable in a lambda expression may have unexpected results
                        EigeneTelNr = XMLData.PTelefonie.Telefonnummern.Find(Function(Tel) Tel.Equals(New Telefonnummer With {.SetNummer = FBStatus(j)}))
                        ' Wert für Serialisierung in separater Eigenschaft ablegen
                        OutEigeneTelNr = EigeneTelNr.Unformatiert
                    Case 5 ' Anschluss, SIP...
                        AnschlussID = FBStatus(i)
                End Select
            Next
            AnrMonRING()
        End Set
    End Property

    ''' <summary>
    '''         0        ; 1  ;2;3;  4   ;    5     ; 6  ; 7
    ''' 23.06.18 13:20:24;CALL;3;4;987654;0123456789;SIP0;
    ''' </summary>
    <XmlIgnore> Friend WriteOnly Property SetAnrMonCALL As String()
        Set(FBStatus As String())
            AnrufRichtung = AnrufRichtungen.Ausgehend

            For i = LBound(FBStatus) To UBound(FBStatus)
                Select Case i
                    Case 0 ' Uhrzeit des Telefonates - Startzeit
                        ZeitBeginn = CDate(FBStatus(i))
                    Case 2 ' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
                        ID = CInt(FBStatus(i))
                    Case 3 ' Nebenstellennummer, eindeutige Zuordnung des Telefons
                        NebenstellenNummer = CInt(FBStatus(i))
                    Case 4 ' Eingehende (anrufende) Telefonnummer
                        Dim j As Integer = i ' Vermeide Fehler: BC42324 Using the iteration variable in a lambda expression may have unexpected results
                        EigeneTelNr = XMLData.PTelefonie.Telefonnummern.Find(Function(Tel) Tel.Equals(FBStatus(j)))
                        ' Wert für Serialisierung in separater Eigenschaft ablegen
                        If EigeneTelNr IsNot Nothing Then
                            OutEigeneTelNr = EigeneTelNr.Unformatiert
                        End If
                    Case 5 ' Gewählte (ausgehende) Telefonnummer
                        GegenstelleTelNr = New Telefonnummer With {.SetNummer = FBStatus(i)}
                    Case 6
                        AnschlussID = FBStatus(i)
                End Select
            Next

            AnrMonCALL()
        End Set
    End Property

    ''' <summary>
    '''         0        ;   1   ;2;3 ;    4     ; 5 
    ''' 23.06.18 13:20:44;CONNECT;1;40;0123456789;
    ''' </summary>
    <XmlIgnore> Friend WriteOnly Property SetAnrMonCONNECT As String()
        Set(FBStatus As String())
            For i = LBound(FBStatus) To UBound(FBStatus)
                Select Case i
                    Case 0 ' Uhrzeit des Telefonates - Startzeit
                        ZeitVerbunden = CDate(FBStatus(i))
                    Case 2 ' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
                        ID = CInt(FBStatus(i))
                    Case 3 ' Nebenstellennummer, eindeutige Zuordnung des Telefons
                        NebenstellenNummer = CInt(FBStatus(i))
                    Case 4 ' Gewählte Telefonnummer (CALL) bzw. eingehende Telefonnummer (RING)
                        GegenstelleTelNr = New Telefonnummer With {.SetNummer = FBStatus(i)}
                End Select
            Next

            AnrMonCONNECT()
        End Set
    End Property

    ''' <summary>
    '''         0        ;   1      ;2;3; 4
    ''' 23.06.18 13:20:52;DISCONNECT;1;9;
    ''' </summary>
    <XmlIgnore> Friend WriteOnly Property SetAnrMonDISCONNECT As String()
        Set(FBStatus As String())
            For i = LBound(FBStatus) To UBound(FBStatus)
                Select Case i
                    Case 0 ' Uhrzeit des Telefonates - Startzeit
                        ZeitEnde = CDate(FBStatus(i))
                    Case 2 ' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
                        ID = CInt(FBStatus(i))
                    Case 3 ' Dauer des Telefonates
                        Dauer = CInt(FBStatus(i))
                End Select
            Next

            AnrMonDISCONNECT()
        End Set
    End Property
#End Region

#Region "Structures"
    Friend Structure AnrufRichtungen
        Const Eingehend As Integer = 0
        Const Ausgehend As Integer = 1
    End Structure
#End Region
    Sub New()
        'Stop
    End Sub
    Private Sub AnrMonRING()
        Angenommen = False
        Beendet = False

        ' Ermitteln der Gerätenammen der Telefone, die auf diese eigene Nummer reagieren
        RINGGeräte = XMLData.PTelefonie.Telefoniegeräte.FindAll(Function(Tel) Tel.StrEinTelNr.Contains(EigeneTelNr.Unformatiert))

        ' Anrufmonitor einblenden, wenn Bedingungen erfüllt 
        If EigeneTelNr.Überwacht Then PopUpAnrMon()

        ' Anrufername aus Kontakten und Rückwärtssuche ermitteln
        StarteKontaktsuche()

        ' RING-Liste initialisieren, falls erforderlich
        If XMLData.PTelefonie.RINGListe Is Nothing Then
            XMLData.PTelefonie.RINGListe = New XRingListe With {.Einträge = New List(Of Telefonat)}
        End If

        ' Telefonat in erste Positon der RING-Liste speichern
        XMLData.PTelefonie.RINGListe.Einträge.Insert(Me)

        ' Anrufmonitor aktualisieren
        If AnrMonPopUp IsNot Nothing Then PopUpAnrMon()

    End Sub
    Private Sub AnrMonCALL()
        Angenommen = False
        Beendet = False

        ' Anrufername aus Kontakten und Rückwärtssuche ermitteln
        StarteKontaktsuche()

        ' Telefoniegerät ermitteln
        TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.AnrMonID.AreEqual(NebenstellenNummer))

        ' CALL-Liste initialisieren, falls erforderlich
        If XMLData.PTelefonie.CALLListe Is Nothing Then
            XMLData.PTelefonie.CALLListe = New XCallListe With {.Einträge = New List(Of Telefonat)}
        End If

        ' Telefonat in erste Positon der CALL-Liste speichern
        XMLData.PTelefonie.CALLListe.Einträge.Insert(Me)

    End Sub
    Private Sub AnrMonCONNECT()
        Angenommen = True

        ' Telefoniegerät ermitteln
        TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.AnrMonID.AreEqual(NebenstellenNummer))
    End Sub
    Private Sub AnrMonDISCONNECT()
        Beendet = True

        If XMLData.POptionen.PCBJournal Then
            ErstelleJournalEintrag()
        End If
    End Sub

    ''' <summary>
    ''' Routine zum Initialisieren der Einblendung des Anrfomitors
    ''' </summary>
    Friend Sub PopUpAnrMon()

        If Not VollBildAnwendungAktiv() Then
            If AnrMonPopUp Is Nothing Then
                ' Blende einen neuen Anrufmonitor ein
                AnrMonPopUp = New Popup
                AnrMonPopUp.AnrMonEinblenden(Me)
            Else
                ' Aktualisiere den Anrufmonitor
                AnrMonPopUp.UpdateAnrMon(Me)
            End If
        End If
    End Sub

    Friend Async Sub StarteKontaktsuche()

        ' Kontaktsuche in den Outlook-Kontakten
        Using KSucher As New KontaktSucher
            OlKontakt = Await KSucher.KontaktSuche(GegenstelleTelNr, PDfltStringEmpty)
        End Using

        If OlKontakt IsNot Nothing Then
            With OlKontakt
                ' Anrufernamen ermitteln
                Anrufer = .FullName
                ' Firma aus Kontaktdaten ermitteln
                Firma = .CompanyName
                ' KontaktID und StoreID speichern
                OutlookKontaktID = .EntryID
                OutlookStoreID = .StoreID
            End With
        End If

        ' Kontaktsuche in den Fritz!Box Telefonbüchern
        If XMLData.POptionen.PCBKontaktSucheFritzBox Then
            If OlKontakt Is Nothing Then
                If ThisAddIn.PPhoneBookXML IsNot Nothing Then
                    FBTelBookKontakt = ThisAddIn.PPhoneBookXML.GetKontaktByTelNr(GegenstelleTelNr)
                End If
            End If

            If FBTelBookKontakt IsNot Nothing Then
                If XMLData.POptionen.PCBKErstellen Then
                    OlKontakt = ErstelleKontakt(OutlookKontaktID, OutlookStoreID, FBTelBookKontakt, GegenstelleTelNr, True)

                    With OlKontakt
                        Anrufer = .FullName
                        Firma = .CompanyName
                    End With
                Else
                    Anrufer = FBTelBookKontakt.Person.RealName
                End If
            End If
        End If

        ' Kontaktsuche über die Rückwärtssuche
        If FBTelBookKontakt Is Nothing And OlKontakt Is Nothing Then

            ' Eine Rückwärtssuche braucht nur dann gemacht werden, wennd die Länge der Telefonnummer aussreichend ist.
            ' Ggf. muss der Wert angepasst werden.
            If GegenstelleTelNr.Unformatiert.Length.IsLargerOrEqual(4) Then

                If XMLData.POptionen.PCBRWS Then
                    Using RWSSucher As New Rückwärtssuche
                        VCard = Await RWSSucher.StartRWS(GegenstelleTelNr, XMLData.POptionen.PCBRWSIndex)
                    End Using

                    If VCard.IsNotStringEmpty Then
                        If XMLData.POptionen.PCBKErstellen Then
                            OlKontakt = ErstelleKontakt(OutlookKontaktID, OutlookStoreID, VCard, GegenstelleTelNr, True)
                            With OlKontakt
                                Anrufer = .FullName
                                Firma = .CompanyName
                            End With
                        Else
                            With MixERP.Net.VCards.Deserializer.GetVCard(VCard)
                                Anrufer = .FormattedName
                                Firma = .Organization
                            End With
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Funktion, welche das öffnen des hinterlegten Kontaktes anstößt
    ''' </summary>
    Friend Sub ZeigeKontakt()
        ' Es gibt mehrere Varianten zu beachten.
        ' 1. Ein Kontakte ist in der Eigenschaft hinterlegt.
        ' 2. Es ist kein Kontakt hinterlegt, jedoch KontaktID und StoreID
        ' 3. Es ist eine vCard hinterlegt
        ' 4. Es ist nur eine Nummer hinterlegt
        ' 5. Es ist nichts hinterlegt.

        If OlKontakt Is Nothing AndAlso OutlookKontaktID.IsNotStringNothingOrEmpty And OutlookStoreID.IsNotStringNothingOrEmpty Then
            ' Verknüpfe den Kontakt
            OlKontakt = GetOutlookKontakt(OutlookKontaktID, OutlookStoreID)
        End If

        If OlKontakt Is Nothing Then
            If VCard.IsNotStringNothingOrEmpty Then
                ' wenn nicht, dann neuen Kontakt mit TelNr öffnen
                OlKontakt = ErstelleKontakt(GegenstelleTelNr, False)
            Else
                'vCard gefunden
                OlKontakt = ErstelleKontakt(PDfltStringEmpty, PDfltStringEmpty, VCard, GegenstelleTelNr, False)
            End If
        End If

        If OlKontakt IsNot Nothing Then OlKontakt.Display()

    End Sub

    Friend Sub Rückruf()
        Dim WählClient As New FritzBoxWählClient
        WählClient.WählboxStart(Me)
    End Sub

    Friend Sub ErstelleJournalEintrag()

        Dim OutlookApp As Outlook.Application = ThisAddIn.POutookApplication
        Dim olJournal As Outlook.JournalItem = Nothing

        If OutlookApp IsNot Nothing Then
            ' Journalimport nur dann, wenn Nummer überwacht wird
            If XMLData.POptionen.PCBJournal And EigeneTelNr.Überwacht Then
                Try
                    olJournal = CType(OutlookApp.CreateItem(Outlook.OlItemType.olJournalItem), Outlook.JournalItem)
                Catch ex As Exception
                    NLogger.Error(ex)
                End Try

                If olJournal IsNot Nothing Then
                    Dim tmpSubject As String

                    If Angenommen Then
                        If AnrufRichtung = Telefonat.AnrufRichtungen.Ausgehend Then
                            tmpSubject = PDfltJournalTextAusgehend
                        Else
                            tmpSubject = PDfltJournalTextEingehend
                        End If
                    Else 'Verpasst
                        If AnrufRichtung = Telefonat.AnrufRichtungen.Ausgehend Then
                            tmpSubject = PDfltJournalTextNichtErfolgreich
                        Else
                            tmpSubject = PDfltJournalTextVerpasst
                        End If
                    End If

                    With olJournal

                        .Subject = String.Format("{0} {1}{2}", tmpSubject, Anrufer, If(NrUnterdrückt, PDfltStringEmpty, If(Anrufer.IsStringNothingOrEmpty, GegenstelleTelNr.Formatiert, String.Format(" ({0})", GegenstelleTelNr.Formatiert))))
                        .Duration = Dauer.GetLarger(31) \ 60
                        .Body = PDfltJournalBody(If(NrUnterdrückt, PDfltStringUnbekannt, GegenstelleTelNr.Formatiert), Angenommen, VCard)
                        .Start = ZeitBeginn
                        .Companies = Firma

                        ' Bei verpassten Anrufen ist TelGerät ggf. leer
                        .Categories = String.Format("{0};{1}", If(TelGerät Is Nothing, "Verpasst", TelGerät.Name), String.Join("; ", PDfltJournalDefCategories.ToArray))

                        ' Testweise: Speichern der EntryID und StoreID in Benutzerdefinierten Feldern
                        If OlKontakt IsNot Nothing Then

                            Dim colArgs(1) As Object
                            colArgs(0) = OlKontakt.EntryID
                            colArgs(1) = OlKontakt.StoreID

                            For i As Integer = 0 To 1
                                .PropertyAccessor.SetProperty(DASLTagJournal(i).ToString, colArgs(i))
                            Next

                            ' Funktioniert aus irgendeinem dummen Grund nicht. Die EntryID wird nicht übertragen.
                            '.PropertyAccessor.SetProperties(DASLTagJournal, colArgs)
                        End If

                        .Close(Outlook.OlInspectorClose.olSave)
                    End With

                    olJournal.ReleaseComObject
                    ' Merke die Zeit
                    If XMLData.POptionen.PLetzterJournalEintrag < Now Then XMLData.POptionen.PLetzterJournalEintrag = Now
                    ' Merke die ID
                    XMLData.POptionen.PLetzterJournalEintragID = Math.Max(XMLData.POptionen.PLetzterJournalEintragID, ID)
                End If
            End If
        Else
            NLogger.Info(PDfltJournalFehler)
        End If
    End Sub

#Region "RibbonXML"
    Friend Overloads Function CreateDynMenuButton(ByVal xDoc As Xml.XmlDocument, ByVal ID As Integer, ByVal Tag As String) As Xml.XmlElement
        Dim XButton As Xml.XmlElement
        Dim XAttribute As Xml.XmlAttribute

        XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

        XAttribute = xDoc.CreateAttribute("id")
        XAttribute.Value = String.Format("{0}_{1}", Tag, ID)
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("label")
        XAttribute.Value = If(Anrufer.IsNotStringNothingOrEmpty, Anrufer, GegenstelleTelNr.Formatiert).XMLMaskiereZeichen
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("onAction")
        XAttribute.Value = "BtnOnAction"
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("tag")
        XAttribute.Value = Tag.XMLMaskiereZeichen
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("supertip")
        XAttribute.Value = String.Format("Zeit: {1}{0}Telefonnummer: {2}", PDflt1NeueZeile, ZeitBeginn.ToString, GegenstelleTelNr.Formatiert)
        XButton.Attributes.Append(XAttribute)

        If Not Angenommen Then
            XAttribute = xDoc.CreateAttribute("imageMso")
            XAttribute.Value = "HighImportance"
            XButton.Attributes.Append(XAttribute)
        End If

        Return XButton
    End Function
#End Region

#Region "Equals, CompareTo"
    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals(TryCast(obj, Telefonat))
    End Function

    Public Overloads Function Equals(other As Telefonat) As Boolean Implements IEquatable(Of Telefonat).Equals
        Return other IsNot Nothing AndAlso
               EigeneTelNr Is other.EigeneTelNr AndAlso
               GegenstelleTelNr Is other.GegenstelleTelNr AndAlso
               ZeitBeginn = ZeitBeginn AndAlso
               ZeitEnde.CompareTo(other.ZeitEnde).IsZero
    End Function
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
                EigeneTelNr.Dispose()
                GegenstelleTelNr.Dispose()
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            OlKontakt = Nothing
            ' TODO: große Felder auf Null setzen.
            RINGGeräte.Clear()
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    Protected Overrides Sub Finalize()
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class


