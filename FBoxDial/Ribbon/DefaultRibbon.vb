Public NotInheritable Class DefaultRibbon
    ''' <summary>
    ''' Wählen
    ''' </summary>  
    Public Shared ReadOnly Property P_CMB_Dial() As String = "Wählen"

    ''' <summary>
    ''' Wahlwiederholung
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_WWDH() As String = "Wahlwiederholung"

    ''' <summary>
    ''' Direktwahl
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Direktwahl() As String = "Direktwahl"

    ''' <summary>
    ''' Anrufmonitor
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMon() As String = "Anrufmonitor"

    ''' <summary>
    ''' Anzeigen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonAnzeigen() As String = "Anzeigen"

    ''' <summary>
    ''' Anrufmonitor neustarten
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonNeuStart() As String = "Anrufmonitor neustarten"

    ''' <summary>
    ''' Rückruf
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_CallBack() As String = "Rückruf"

    ''' <summary>
    ''' VIP-Liste
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP() As String = "VIP-Liste"

    ''' <summary>
    ''' Journalimport
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Journal() As String = "Journalimport"

    ''' <summary>
    ''' Einstellungen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Setup() As String = "Einstellungen"

    ''' <summary>
    ''' Liste löschen...
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ClearList() As String = "Liste löschen..."

    ''' <summary>
    ''' Liste löschen...
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ClearEntry() As String = "Eintrag löschen..."

    ''' <summary>
    ''' Öffnet den Wahldialog um das ausgewählte Element anzurufen.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Dial_ToolTipp() As String = "Öffnet den Wahldialog um das ausgewählte Element anzurufen"

    ''' <summary>
    ''' Öffnet den Wahldialog für die Wahlwiederholung
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_WWDH_ToolTipp() As String = "Öffnet den Wahldialog für die Wahlwiederholung"

    ''' <summary>
    ''' Startet den Anrufmonitor.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMon_ToolTipp() As String = "Startet den Anrufmonitor"

    ''' <summary>
    ''' Öffnet den Wahldialog für die Direktwahl
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Direktwahl_ToolTipp() As String = "Öffnet den Wahldialog für die Direktwahl"

    ''' <summary>
    ''' Zeigt den letzten Anruf an
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonAnzeigen_ToolTipp() As String = "Zeigt den letzten Anruf an"

    ''' <summary>
    ''' Startet den Anrufmonitor neu
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonNeuStart_ToolTipp() As String = "Startet den Anrufmonitor neu"

    ''' <summary>
    ''' Öffnet den Wahldialog für den Rückruf
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_CallBack_ToolTipp() As String = "Öffnet den Wahldialog für den Rückruf"

    ''' <summary>
    ''' Öffnet den Wahldialog um einen VIP anzurufen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_ToolTipp() As String = "Öffnet den Wahldialog um einen VIP anzurufen"

    ''' <summary>
    ''' Die VIP-Liste ist mit 10 Einträgen bereits voll.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_O11_Voll_ToolTipp() As String = "Die VIP-Liste ist mit 10 Einträgen bereits voll."

    ''' <summary>
    ''' Füge diesen Kontakt der VIP-Liste hinzu.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_Hinzufügen_ToolTipp() As String = "Füge diesen Kontakt der VIP-Liste hinzu."

    ''' <summary>
    ''' Entfernt diesen Kontakt von der VIP-Liste.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_Entfernen_ToolTipp() As String = "Entfernt diesen Kontakt von der VIP-Liste."

    ''' <summary>
    ''' Importiert die Anrufliste der Fritz!Box als Journaleinträge
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Journal_ToolTipp() As String
        Get
            Return "Importiert die Anrufliste der " & FritzBoxDefault.PDfltFritzBoxName & " als Journaleinträge"
        End Get
    End Property

    ''' <summary>
    ''' Öffnet die Fritz!Box Telefon-dingsbums Einstellungen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Setup_ToolTipp() As String
        Get
            Return "Öffnet den " & PDfltAddin_LangName & " Einstellungsdialog"
        End Get
    End Property

    ''' <summary>
    ''' VIP
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_VIP() As String = "VIP"

    ''' <summary>
    ''' Upload
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Upload() As String = "Upload"

    ''' <summary>
    ''' Anrufen (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ContextMenueItemCall() As String
        Get
            Return "Anrufen (" & PDfltAddin_LangName & ")"
        End Get
    End Property

    ''' <summary>
    ''' VIP (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ContextMenueItemVIP() As String
        Get
            Return P_CMB_Insp_VIP & " (" & PDfltAddin_LangName & ")"
        End Get
    End Property

    ''' <summary>
    ''' Upload (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ContextMenueItemUpload() As String
        Get
            Return P_CMB_Insp_Upload & " (" & PDfltAddin_LangName & ")"
        End Get
    End Property

    ''' <summary>
    ''' Rückwärtssuche
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_RWS() As String = "Rückwärtssuche"

    ''' <summary>
    ''' DasÖrtliche
    ''' </summary>
    Public Shared ReadOnly Property P_RWS_Name As String = "Rückwärtssuche"

    ''' <summary>
    ''' Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_RWS_ToolTipp() As String = "Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche"

    ''' <summary>
    ''' Notiz
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Note() As String = "Notiz"

    ''' <summary>
    ''' Einen Notizeintrag hinzufügen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Note_ToolTipp() As String = "Einen Notizeintrag hinzufügen"

    ''' <summary>
    ''' Fritz!Box Telefonbuch
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Expl_Adrbk() As String = FritzBoxDefault.PDfltFritzBoxName & " Telefonbuch"

    ''' <summary>
    ''' Einen Notizeintrag hinzufügen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_UploadKontakt_ToolTipp() As String
        Get
            Return "Lädt diesen Kontakt auf die " & FritzBoxDefault.PDfltFritzBoxName & " hoch."
        End Get
    End Property

    ''' <summary>          
    ''' Kontakt erstellen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Erstellen() As String = "Kontakt erstellen"

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Journaleintrag
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Erstellen_ToolTipp() As String = "Erstellt einen Kontakt aus einem Journaleintrag"

    ''' <summary>
    ''' Kontakt anzeigen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen() As String = "Kontakt anzeigen"

    ''' <summary>
    ''' Zeigt den Kontakt zu diesem Journaleintrag an
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen_ToolTipp() As String = "Zeigt den Kontakt zu diesem Journaleintrag an"

    ''' <summary>
    ''' Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen_Error_ToolTipp() As String = "Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag."

End Class
