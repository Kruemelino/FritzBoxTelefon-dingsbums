'Imports Microsoft.Office.Interop
'Friend Class ApiWindow
'    Friend Property HWnd() As IntPtr
'End Class

'Friend Class KontaktNotiz

'    Private Property ApiWindowList As New List(Of ApiWindow)

'    ''' <summary>
'    ''' Fügt einen Notizzeile in den Body eines Kontaktes
'    ''' </summary>
'    ''' <param name="olKontakt">Kontakt, in den die Notizzeile geschrieben werden soll.</param>
'    Friend Sub AddNote(olKontakt As Outlook.ContactItem)
'        Dim oInsp As Outlook.Inspector
'        Dim Handle As IntPtr
'        Dim ReturnValue As Long
'        Dim oDoc As Word.Document
'        Dim oTable As Word.Table = Nothing
'        Dim HeaderRow As Word.Row = Nothing
'        Dim CallRow As Word.Row = Nothing
'        Dim NoteRow As Word.Row = Nothing
'        Dim startLocation As Object

'        oInsp = olKontakt.GetInspector
'        Handle = GetBodyHandle(oInsp)

'        If Not Handle = IntPtr.Zero Then
'            oDoc = CType(oInsp.WordEditor, Word.Document)
'            CreateTable(oDoc, oTable, HeaderRow, CallRow, NoteRow, True)

'            With CallRow
'                .Cells(1).Range.Text = DataProvider.P_Def_AnrMonDirection_Default
'                .Cells(2).Range.Text = C_OLI.BenutzerInitialien
'            End With

'            If NoteRow IsNot Nothing Then
'                startLocation = NoteRow.Range.Start
'                oDoc.Range(startLocation, startLocation).Select()
'            End If
'            oDoc = Nothing

'            ' Fokus setzen WICHTIG!
'            ReturnValue = OutlookSecurity.SetFocus(Handle)
'            ' Aufräumen

'            NAR(oDoc)
'            NAR(oTable)
'            NAR(HeaderRow)
'            NAR(CallRow)
'            NAR(NoteRow)
'        End If
'    End Sub


'    ''' <summary>
'    ''' Ermittelt den Handle des Body-Elementes eines Kontaktinspectors
'    ''' </summary>
'    ''' <param name="oInsp">Inspector eines Kontaktes.</param>
'    ''' <returns>Pointer auf das Body-Element.</returns>
'    Private Function GetBodyHandle(oInsp As Outlook.Inspector) As IntPtr
'        Dim HandleNames() As String = {"AfxWndW",
'                                       "AfxWndW",
'                                       DataProvider.P_Def_ErrorMinusOne_String,
'                                       "AfxWndA",
'                                       "_WwB"}

'        GetBodyHandle = OutlookSecurity.FindWindowEX(GetBodyHandle, IntPtr.Zero, "rctrl_renwnd32", oInsp.Caption)

'        For Each HandleName As String In HandleNames
'            If HandleName = DataProvider.P_Def_ErrorMinusOne_String Then
'                GetBodyHandle = GetChildWindows(GetBodyHandle).Item(0).HWnd
'            Else
'                GetBodyHandle = OutlookSecurity.FindWindowEX(GetBodyHandle, IntPtr.Zero, HandleName, vbNullString)
'            End If
'            If GetBodyHandle = IntPtr.Zero Then
'                Exit For
'            End If
'        Next
'    End Function

'    ''' <summary>
'    ''' Erstellt die Notiztabelle, bzw. fügt Notizzeilen an.
'    ''' </summary>
'    ''' <param name="oDoc">Das Worddokument, in den die Notiztabelle, bzw. Notizzeile eingefügt werden soll.</param>
'    ''' <param name="oTable">Die Notiztabelle an sich.</param>
'    ''' <param name="HeaderRow">Die Kopfzeile der Notiztabelle.</param>
'    ''' <param name="CallRow">Die Kopfzeile des einzelnen Anrufes.</param>
'    ''' <param name="NoteRow">BEreich in den die Notizen eingetragen werden.</param>
'    ''' <param name="NeueZeile">Flag, die angibt ob eine neue Zeile hinzugefügt werden soll.</param>
'    Friend Sub CreateTable(ByRef oDoc As Word.Document, ByRef oTable As Word.Table, ByRef HeaderRow As Word.Row, ByRef CallRow As Word.Row, ByRef NoteRow As Word.Row,  NeueZeile As Boolean)

'        Dim nRow As Integer = 1
'        Dim nCol As Integer = 6

'        Dim oTableLineStyle As Word.WdLineStyle = Word.WdLineStyle.wdLineStyleSingle
'        Dim oTableLineWidth_1 As Word.WdLineWidth = Word.WdLineWidth.wdLineWidth025pt
'        Dim oTableLineWidth_2 As Word.WdLineWidth = Word.WdLineWidth.wdLineWidth150pt
'        Dim oTableLineColor As Word.WdColor = Word.WdColor.wdColorBlack
'        Dim oTableFontColorIndex As Word.WdColorIndex = Word.WdColorIndex.wdBlack
'        Dim Sel4BM As Object

'        With oDoc.Bookmarks
'            For i = 1 To .Count
'                If .Item(i).Name = DataProvider.P_Def_Note_Table Then
'                    oTable = .Item(i).Range.Tables(1)
'                    Exit For
'                End If
'            Next
'        End With
'        If oTable Is Nothing Then
'            ' Testweise Bestehender Inhalt bleibt bestehen
'            oDoc.Paragraphs.Add()
'            oTable = oDoc.Tables.Add(oDoc.Paragraphs.Add.Range, nRow, nCol)
'            Sel4BM = oTable
'            oDoc.Bookmarks.Add(DataProvider.P_Def_Note_Table, Sel4BM)
'            With oTable
'                With .Borders
'                    .OutsideLineStyle = oTableLineStyle
'                    .OutsideLineWidth = oTableLineWidth_1
'                    .OutsideColor = oTableLineColor
'                    .InsideLineStyle = oTableLineStyle
'                    .InsideLineWidth = oTableLineWidth_1
'                    .InsideColor = oTableLineColor
'                End With
'                HeaderRow = .Rows(1)
'                With HeaderRow
'                    .Cells(1).Width = 30
'                    .Cells(2).Width = 40
'                    .Cells(3).Width = 140
'                    .Cells(4).Width = 140
'                    .Cells(5).Width = 140
'                    .Cells(6).Width = 140
'                End With

'                CallRow = .Rows.Add()
'                NoteRow = .Rows.Add()
'            End With

'            With HeaderRow
'                .Range.Font.Bold = vbTrue
'                .Cells(1).Range.Text = "Typ"
'                .Cells(2).Range.Text = "Initialen"
'                .Cells(3).Range.Text = "Telefonnummer"
'                .Cells(4).Range.Text = "Begin"
'                .Cells(5).Range.Text = "Ende"
'                .Cells(6).Range.Text = "Dauer"
'                .Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter

'                For Each cCell As Word.Cell In .Cells
'                    cCell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
'                Next
'            End With

'        Else
'            HeaderRow = oTable.Rows(1)
'            If NeueZeile Then
'                CallRow = oTable.Rows.Add(oTable.Rows.Item(2))
'                NoteRow = oTable.Rows.Add(oTable.Rows.Item(3))
'            Else
'                CallRow = oTable.Rows(HeaderRow.Index + 1)
'                NoteRow = oTable.Rows(HeaderRow.Index + 2)
'            End If
'        End If

'        With CallRow
'            For i = 3 To nCol
'                .Cells(i).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight
'            Next
'        End With

'        With NoteRow
'            .Cells.Merge()

'            With .Cells(1).Range
'                .ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
'                .Font.ColorIndex = oTableFontColorIndex
'            End With

'            With .Borders(Word.WdBorderType.wdBorderBottom)
'                .LineStyle = oTableLineStyle
'                .LineWidth = oTableLineWidth_2
'                .Color = oTableLineColor
'            End With

'            With .Range()
'                .ParagraphFormat.SpaceBefore = 6
'                .ParagraphFormat.SpaceAfter = 6
'            End With
'        End With
'    End Sub

'    ''' <summary>
'    ''' Füllt die Notizzeile mit Informationen
'    ''' </summary>
'    ''' <param name="AnrMonTyp">Gibt, an ob es sich um einen RING, CALL, CONNECT oder DISCONNECT handelt.</param>
'    ''' <param name="Telfonat">Alle Informationen zu dem Telefonat.</param>
'    ''' <param name="ContactShown">Gibt an ob der Kontakt angezeigt wird.</param>
'    Friend Sub FillNote(AnrMonTyp As Anrufmonitor.AnrMonEvent,  Telfonat As C_Telefonat,  ContactShown As Boolean)

'        'FillNote = vbNull
'        With Telfonat

'            Dim oInsp As Outlook.Inspector = .olContact.GetInspector
'            Dim oPage As Outlook.Pages
'            Dim oDoc As Word.Document = CType(oInsp.WordEditor, Word.Document)
'            Dim oTable As Word.Table = Nothing

'            Dim HeaderRow As Word.Row = Nothing
'            Dim CallRow As Word.Row = Nothing
'            Dim NoteRow As Word.Row = Nothing

'            CreateTable(oDoc, oTable, HeaderRow, CallRow, NoteRow, C_hf.IIf((AnrMonTyp = Anrufmonitor.AnrMonEvent.AnrMonRING Or AnrMonTyp = Anrufmonitor.AnrMonEvent.AnrMonCALL) And Not ContactShown, True, False))
'            If CallRow IsNot Nothing Then
'                With CallRow
'                    Select Case AnrMonTyp
'                        Case Anrufmonitor.AnrMonEvent.AnrMonRING, Anrufmonitor.AnrMonEvent.AnrMonCALL
'                            .Cells(1).Range.Text = C_hf.IIf(AnrMonTyp = Anrufmonitor.AnrMonEvent.AnrMonRING, DataProvider.P_Def_AnrMonDirection_Ring, DataProvider.P_Def_AnrMonDirection_Call)
'                            .Cells(2).Range.Text = C_OLI.BenutzerInitialien
'                            .Cells(3).Range.Text = Telfonat.TelNr
'                            .Cells(4).Range.Text = CStr(Telfonat.Zeit)
'                            .Cells(5).Range.Text = DataProvider.P_Def_LeerString
'                            .Cells(6).Range.Text = DataProvider.P_Def_LeerString
'                        Case Anrufmonitor.AnrMonEvent.AnrMonCONNECT
'                            .Cells(4).Range.Text = CStr(Telfonat.Zeit)
'                            OutlookSecurity.SetFocus(GetBodyHandle(oInsp)).ToString()
'                        Case Anrufmonitor.AnrMonEvent.AnrMonDISCONNECT
'                            .Cells(5).Range.Text = Telfonat.Zeit.AddMinutes(Telfonat.Dauer).ToString()
'                            .Cells(6).Range.Text = C_hf.GetTimeInterval(Telfonat.Dauer * 60)
'                            OutlookSecurity.SetFocus(GetBodyHandle(oInsp)).ToString()
'                    End Select
'                End With
'            End If

'            If Not ContactShown Then
'                oPage = CType(oInsp.ModifiedFormPages, Outlook.Pages)
'                oPage.Add("General")
'                oInsp.HideFormPage("General")
'                .olContact.Save()
'            End If
'        End With
'    End Sub

'    ''' <summary>
'    ''' Gibt alle Handles der Childwindows zurück.
'    ''' </summary>
'    ''' <param name="hwnd">Ausgangshandle</param>
'    ''' <returns>Liste der Handles.</returns>
'    Private Function GetChildWindows(hwnd As IntPtr) As List(Of ApiWindow)
'        ' Clear the window list
'        Dim ReturnValue As Int32
'        ListChildren = New List(Of ApiWindow)
'        ' Start the enumeration process.
'        ReturnValue = OutlookSecurity.EnumChildWindows(hwnd, AddressOf EnumChildWindowProc, IntPtr.Zero)
'        ' Return the children list when the process is completed.
'        Return ListChildren
'    End Function

'    ''' <summary>
'    ''' Attempt to match the child class, if one was specified, otherwiseenumerate all the child windows.
'    ''' </summary>
'    ''' <param name="hwnd"></param>
'    ''' <param name="lParam"></param>
'    Private Sub EnumChildWindowProc(hwnd As IntPtr,  lParam As Int32)
'        ListChildren.Add(GetWindowIdentification(hwnd))
'    End Sub

'    ''' <summary>
'    ''' Build the ApiWindow object to hold information about the Window object.
'    ''' Gibt hier das Handle zurück.
'    ''' </summary>
'    ''' <param name="hwnd"></param>
'    Private Function GetWindowIdentification(hwnd As IntPtr) As ApiWindow
'        Dim window As New ApiWindow()
'        window.HWnd = CType(hwnd, IntPtr)
'        Return window
'    End Function

'End Class
