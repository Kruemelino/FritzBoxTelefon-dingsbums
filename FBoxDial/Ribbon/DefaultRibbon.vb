Friend Structure DefaultRibbonWerte

#Region "Ribbon Label"
    ''' <summary>
    ''' "Fritz!Box Telefon-dingsbums"
    ''' </summary>
    Public Shared ReadOnly Property PLabelTab() As String = Localize.resCommon.strDefLongName

    ''' <summary>
    ''' Wählen
    ''' </summary>  
    Public Shared ReadOnly Property PLabelbtnDialExpl() As String = "Wählen"

    ''' <summary>
    ''' Wählen
    ''' </summary>  
    Public Shared ReadOnly Property PLabelbtnDialInsp() As String = PLabelbtnDialExpl

    ''' <summary>
    ''' Wahlwiederholung
    ''' </summary>
    Public Shared ReadOnly Property PLabelCallList() As String = "Wahlwiederholung"

    ''' <summary>
    ''' Rückruf
    ''' </summary>
    Public Shared ReadOnly Property PLabelRingList() As String = "Rückruf"

    ''' <summary>
    ''' Direktwahl
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnDirektwahl() As String = "Direktwahl"

    ''' <summary>
    ''' Anrufmonitor
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnAnrMonIO() As String = "Anrufmonitor"

    ''' <summary>
    ''' Anzeigen
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnAnrMonShow() As String = "Anzeigen"

    ''' <summary>
    ''' Anrufmonitor neustarten
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnAnrMonRestart() As String = "Anrufmonitor neustarten"

    ''' <summary>
    ''' Fritz!Box Telefonbücher
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property PLabelbtnFBTelBch() As String
        Get
            Return String.Format("{0} Telefonbücher", FritzBoxDefault.PDfltFritzBoxName)
        End Get
    End Property

    ''' <summary>
    ''' VIP-Liste
    ''' </summary>
    Public Shared ReadOnly Property PLabelVIPList() As String = "VIP-Liste"

    ''' <summary>
    ''' Journalimport
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnAnrMonJI() As String = "Journalimport"

    ''' <summary>
    ''' Einstellungen
    ''' </summary>
    Public Shared ReadOnly Property PLabelEinstellungen() As String = "Einstellungen"

    ''' <summary>
    ''' Liste löschen...
    ''' </summary>
    Public Shared ReadOnly Property PLabeldynListDel() As String = "Liste löschen..."

    ''' <summary>
    ''' Anrufen (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property PLabelcbtnDial() As String = $"Anrufen ({Localize.resCommon.strDefLongName})"

    ''' <summary>
    ''' Anrufen (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property PLabelrbtnDial() As String = PLabelcbtnDial

    ''' <summary>
    ''' VIP (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property PLabelctbtnVIP() As String
        Get
            Return String.Format("{0} ({1})", PLabeltbtnVIP, Localize.resCommon.strDefLongName)
        End Get
    End Property

    ''' <summary>
    ''' Upload (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property PLabelcbtnUpload() As String
        Get
            Return String.Format("{0} ({1})", PLabelbtnUpload, Localize.resCommon.strDefLongName)
        End Get
    End Property

    ''' <summary>
    ''' Rückwärtssuche
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnRWS As String = "Rückwärtssuche"

    ''' <summary>
    ''' Notiz
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnNote() As String = "Notiz"

    ''' <summary>
    ''' VIP
    ''' </summary>
    Public Shared ReadOnly Property PLabeltbtnVIP() As String = "VIP"

    ''' <summary>
    ''' Upload
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnUpload() As String = "Upload"

    ''' <summary>
    ''' Upload
    ''' </summary>
    Public Shared ReadOnly Property PLabelcdMUpload() As String = PLabelbtnUpload

    ''' <summary>
    ''' Upload
    ''' </summary>
    Public Shared ReadOnly Property PLabelMUpload() As String = PLabelbtnUpload

    ''' <summary>
    ''' Liste löschen...
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ClearEntry() As String = "Liste löschen..."

    ''' <summary>
    ''' Rückwärtssuche
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_RWS() As String = "Rückwärtssuche"

    ''' <summary>          
    ''' Kontakt erstellen
    ''' </summary>
    Public Shared ReadOnly Property PLabelbtnAddContact() As String = "Kontakt erstellen"

    ''' <summary>
    ''' Kontakt anzeigen
    ''' </summary>
    Public Shared ReadOnly Property PLabelAnzeigenbtnAddContact() As String = "Kontakt anzeigen"

    ''' <summary>          
    ''' Kontakt erstellen
    ''' </summary>
    Public Shared ReadOnly Property PLabelcbtnAddContact() As String = PLabelbtnAddContact

    ''' <summary>
    ''' Kontakt anzeigen
    ''' </summary>
    Public Shared ReadOnly Property PLabelAnzeigencbtnAddContact() As String = PLabelAnzeigenbtnAddContact
#End Region

#Region "ScreenTipp"
    ''' <summary>
    ''' Öffnet den Wahldialog um das ausgewählte Element anzurufen.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnDialExpl() As String = "Öffnet den Wahldialog um das ausgewählte Element anzurufen"
    ''' <summary>
    ''' Öffnet den Wahldialog um das ausgewählte Element anzurufen.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnDialInsp() As String = PScreenTippbtnDialExpl
    ''' <summary>
    ''' Öffnet den Wahldialog um das ausgewählte Element anzurufen.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTipprbtnDial() As String = PScreenTippbtnDialExpl

    ''' <summary>
    ''' Öffnet den Wahldialog für die Wahlwiederholung
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippCallList() As String = "Öffnet den Wahldialog für die Wahlwiederholung"

    ''' <summary>
    ''' Öffnet den Wahldialog für den Rückruf
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippRingList() As String = "Öffnet den Wahldialog für den Rückruf"

    ''' <summary>
    ''' Startet den Anrufmonitor.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnAnrMonIO() As String = "Startet den Anrufmonitor"

    ''' <summary>
    ''' Öffnet den Wahldialog für die Direktwahl
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnDirektwahl() As String = "Öffnet den Wahldialog für die Direktwahl"

    ''' <summary>
    ''' Zeigt den letzten Anruf an
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnAnrMonShow() As String = "Zeigt den letzten Anruf an"

    ''' <summary>
    ''' Startet den Anrufmonitor neu
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnAnrMonRestart() As String = "Startet den Anrufmonitor neu"

    ''' <summary>
    ''' Öffnet den Wahldialog um einen VIP anzurufen
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippVIPList() As String = "Öffnet den Wahldialog um einen VIP anzurufen"

    ''' <summary>
    ''' Füge diesen Kontakt der VIP-Liste hinzu.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippAddtbtnVIP() As String = "Füge diesen Kontakt der VIP-Liste hinzu."

    ''' <summary>
    ''' Entfernt diesen Kontakt von der VIP-Liste.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippRemovetbtnVIP() As String = "Entfernt diesen Kontakt von der VIP-Liste."

    ''' <summary>
    ''' Importiert die Anrufliste der Fritz!Box als Journaleinträge
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnAnrMonJI() As String
        Get
            Return "Importiert die Anrufliste der " & FritzBoxDefault.PDfltFritzBoxName & " als Journaleinträge"
        End Get
    End Property

    ''' <summary>
    ''' Öffnet die Fritz!Box Telefon-dingsbums Einstellungen
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippEinstellungen() As String = $"Öffnet den {Localize.resCommon.strDefLongName} Einstellungsdialog"

    ''' <summary>
    ''' Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnRWS() As String = "Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche"

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Journaleintrag
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnAddContact() As String = "Erstellt einen Kontakt aus einem Journaleintrag"

    ''' <summary>
    ''' Zeigt den Kontakt zu diesem Journaleintrag an
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippAnzeigenbtnAddContact() As String = "Zeigt den Kontakt zu diesem Journaleintrag an"

    ''' <summary>
    ''' Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTipp_CMB_Kontakt_Anzeigen_Error_ToolTipp() As String = "Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag."

    ''' <summary>
    ''' Einen Notizeintrag hinzufügen
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnNote() As String = "Einen Notizeintrag hinzufügen"


    ''' <summary>
    ''' Lädt diesen Kontakt auf die Fritz!Box hoch.
    ''' </summary>
    Public Shared ReadOnly Property PScreenTippbtnUpload() As String
        Get
            Return String.Format("Lädt diesen Kontakt auf die {0} hoch.", FritzBoxDefault.PDfltFritzBoxName)
        End Get
    End Property

    Public Shared ReadOnly Property PScreenTippbtnFBTelBch() As String
        Get
            Return String.Format("Öffne die Verwaltung der {0} Telefonbücher", FritzBoxDefault.PDfltFritzBoxName)
        End Get
    End Property
#End Region

#Region "ImageMso"
    Public Shared ReadOnly Property PImageMsobtnDialExpl() As String = "AutoDial"
    Public Shared ReadOnly Property PImageMsobtnDialInsp() As String = PImageMsobtnDialExpl
    Public Shared ReadOnly Property PImageMsorbtnDial() As String = PImageMsobtnDialExpl
    Public Shared ReadOnly Property PImageMsobtnDirektwahl() As String = "SlidesPerPage9Slides"
    Public Shared ReadOnly Property PImageMsodynMWwdListe() As String = "RecurrenceEdit"
    Public Shared ReadOnly Property PImageMsodynMAnrListe() As String = "DirectRepliesTo"
    Public Shared ReadOnly Property PImageMsodynMVIPListe() As String = "Pushpin"
    Public Shared ReadOnly Property PImageMsotbtnVIP() As String = PImageMsodynMVIPListe
    Public Shared ReadOnly Property PImageMsodynListDel() As String = "ToolDelete"
    Public Shared ReadOnly Property PImageMsobtnAnrMonRestart() As String = "RecurrenceEdit"
    Public Shared ReadOnly Property PImageMsobtnAnrMonShow() As String = "ClipArtInsert"
    Public Shared ReadOnly Property PImageMsobtnAnrMonJI() As String = "NewJournalEntry"
    Public Shared ReadOnly Property PImageMsobtnUpload() As String = "DistributionListAddNewMember"
    Public Shared ReadOnly Property PImageMsoMUpload() As String = "DistributionListAddNewMember"
    Public Shared ReadOnly Property PImageMsobtnRWS() As String = "CheckNames"
    Public Shared ReadOnly Property PImageMsobtnAddContact() As String = "RecordsSaveAsOutlookContact"
    Public Shared ReadOnly Property PImageMsocbtnAddContact() As String = PImageMsobtnAddContact
    Public Shared ReadOnly Property PImageMsobtnNote() As String = "ShowNotesPage"
    Public Shared ReadOnly Property PImageMsoCallList() As String = PDfltStringEmpty
    Public Shared ReadOnly Property PImageMsoRingList() As String = PDfltStringEmpty
    Public Shared ReadOnly Property PImageMsoVIPList() As String = PDfltStringEmpty
    Public Shared ReadOnly Property PImageMsobtnFBTelBch As String = "AddressBook"
#End Region


End Structure
