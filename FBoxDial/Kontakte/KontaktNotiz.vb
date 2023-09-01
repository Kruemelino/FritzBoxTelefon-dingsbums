Imports Microsoft.Office.Interop
Imports Microsoft.Win32

Friend Module KontaktNotiz
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property ApiWindowList As New List(Of ApiWindow)

    ''' <summary>
    ''' Fügt einen Notizzeile in den Body eines Kontaktes
    ''' </summary>
    ''' <param name="olKontakt">Kontakt, in den die Notizzeile geschrieben werden soll.</param>
    Friend Sub AddNote(olKontakt As Outlook.ContactItem)
        Dim oInsp As Outlook.Inspector
        Dim Handle As IntPtr
        Dim ReturnValue As Long
        Dim oDoc As Word.Document
        Dim oTable As Word.Table = Nothing
        Dim HeaderRow As Word.Row = Nothing
        Dim CallRow As Word.Row = Nothing
        Dim NoteRow As Word.Row = Nothing
        Dim startLocation As Object

        oInsp = olKontakt.GetInspector
        Handle = GetBodyHandle(oInsp)

        If Not Handle = IntPtr.Zero Then
            oDoc = CType(oInsp.WordEditor, Word.Document)
            CreateTable(oDoc, oTable, HeaderRow, CallRow, NoteRow, True)

            With CallRow
                .Cells(1).Range.Text = "Default"
                .Cells(2).Range.Text = "Init"
            End With

            If NoteRow IsNot Nothing Then
                startLocation = NoteRow.Range.Start
                oDoc.Range(startLocation, startLocation).Select()
            End If
            oDoc = Nothing

            ' Fokus setzen WICHTIG!
            ReturnValue = UnSaveMethods.SetFocus(Handle)
            ' Aufräumen

            ReleaseComObject(oDoc)
            ReleaseComObject(oTable)
            ReleaseComObject(HeaderRow)
            ReleaseComObject(CallRow)
            ReleaseComObject(NoteRow)
        End If
    End Sub

    ''' <summary>
    ''' Ermittelt den Handle des Body-Elementes eines Kontaktinspectors
    ''' </summary>
    ''' <param name="oInsp">Inspector eines Kontaktes.</param>
    ''' <returns>Pointer auf das Body-Element.</returns>
    Private Function GetBodyHandle(oInsp As Outlook.Inspector) As IntPtr

        Dim BodyHandle As IntPtr

        Dim HandleNames() As String = {"AfxWndW",
                                       "AfxWndW",
                                       "-1",
                                       "AfxWndA",
                                       "_WwB"}

        BodyHandle = UnSaveMethods.FindWindowEX(BodyHandle, IntPtr.Zero, "rctrl_renwnd32", oInsp.Caption)

        For Each HandleName As String In HandleNames

            If HandleName = "-1" Then
                BodyHandle = GetChildWindows(BodyHandle).Item(0).HWnd
            Else
                BodyHandle = UnSaveMethods.FindWindowEX(BodyHandle, IntPtr.Zero, HandleName, vbNullString)
            End If

            If BodyHandle = IntPtr.Zero Then
                Exit For
            End If

        Next

        Return BodyHandle
    End Function

    ''' <summary>
    ''' Erstellt die Notiztabelle, bzw. fügt Notizzeilen an.
    ''' </summary>
    ''' <param name="oDoc">Das Worddokument, in den die Notiztabelle, bzw. Notizzeile eingefügt werden soll.</param>
    ''' <param name="oTable">Die Notiztabelle an sich.</param>
    ''' <param name="HeaderRow">Die Kopfzeile der Notiztabelle.</param>
    ''' <param name="CallRow">Die Kopfzeile des einzelnen Anrufes.</param>
    ''' <param name="NoteRow">BEreich in den die Notizen eingetragen werden.</param>
    ''' <param name="NeueZeile">Flag, die angibt ob eine neue Zeile hinzugefügt werden soll.</param>
    Private Sub CreateTable(ByRef oDoc As Word.Document, ByRef oTable As Word.Table, ByRef HeaderRow As Word.Row, ByRef CallRow As Word.Row, ByRef NoteRow As Word.Row, NeueZeile As Boolean)

        Dim nRow As Integer = 1
        Dim nCol As Integer = 6

        Dim oTableLineStyle As Word.WdLineStyle = Word.WdLineStyle.wdLineStyleSingle
        Dim oTableLineWidth_1 As Word.WdLineWidth = Word.WdLineWidth.wdLineWidth025pt
        Dim oTableLineWidth_2 As Word.WdLineWidth = Word.WdLineWidth.wdLineWidth150pt
        Dim oTableLineColor As Word.WdColor = Word.WdColor.wdColorBlack
        Dim oTableFontColorIndex As Word.WdColorIndex = Word.WdColorIndex.wdBlack
        Dim Sel4BM As Object

        With oDoc.Bookmarks
            For i = 1 To .Count
                If .Item(i).Name = "FBDB_Note_Table" Then
                    oTable = .Item(i).Range.Tables(1)
                    Exit For
                End If
            Next
        End With

        If oTable Is Nothing Then
            ' Testweise Bestehender Inhalt bleibt bestehen
            oDoc.Paragraphs.Add()
            oTable = oDoc.Tables.Add(oDoc.Paragraphs.Add.Range, nRow, nCol)
            Sel4BM = oTable
            oDoc.Bookmarks.Add("FBDB_Note_Table", Sel4BM)
            With oTable
                With .Borders
                    .OutsideLineStyle = oTableLineStyle
                    .OutsideLineWidth = oTableLineWidth_1
                    .OutsideColor = oTableLineColor
                    .InsideLineStyle = oTableLineStyle
                    .InsideLineWidth = oTableLineWidth_1
                    .InsideColor = oTableLineColor
                End With
                HeaderRow = .Rows(1)
                With HeaderRow
                    .Cells(1).Width = 30
                    .Cells(2).Width = 40
                    .Cells(3).Width = 140
                    .Cells(4).Width = 140
                    .Cells(5).Width = 140
                    .Cells(6).Width = 140
                End With

                CallRow = .Rows.Add()
                NoteRow = .Rows.Add()
            End With

            With HeaderRow
                .Range.Font.Bold = vbTrue
                .Cells(1).Range.Text = "Typ"
                .Cells(2).Range.Text = "Initialen"
                .Cells(3).Range.Text = "Telefonnummer"
                .Cells(4).Range.Text = "Begin"
                .Cells(5).Range.Text = "Ende"
                .Cells(6).Range.Text = "Dauer"
                .Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter

                For Each cCell As Word.Cell In .Cells
                    cCell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                Next
            End With

        Else
            HeaderRow = oTable.Rows(1)
            If NeueZeile Then
                CallRow = oTable.Rows.Add(oTable.Rows.Item(2))
                NoteRow = oTable.Rows.Add(oTable.Rows.Item(3))
            Else
                CallRow = oTable.Rows(HeaderRow.Index + 1)
                NoteRow = oTable.Rows(HeaderRow.Index + 2)
            End If
        End If

        With CallRow
            For i = 3 To nCol
                .Cells(i).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight
            Next
        End With

        With NoteRow
            .Cells.Merge()

            With .Cells(1).Range
                .ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
                .Font.ColorIndex = oTableFontColorIndex
            End With

            With .Borders(Word.WdBorderType.wdBorderBottom)
                .LineStyle = oTableLineStyle
                .LineWidth = oTableLineWidth_2
                .Color = oTableLineColor
            End With

            With .Range()
                .ParagraphFormat.SpaceBefore = 6
                .ParagraphFormat.SpaceAfter = 6
            End With
        End With
    End Sub

    ''' <summary>
    ''' Füllt die Notizzeile mit Informationen
    ''' </summary>
    ''' <param name="Telfonat">Alle Informationen zu dem Telefonat.</param>
    Friend Sub FillNote(Telfonat As Telefonat)

        With Telfonat

            Dim oInsp As Outlook.Inspector = .OlKontakt.GetInspector
            Dim oPage As Outlook.Pages
            Dim oDoc As Word.Document = CType(oInsp.WordEditor, Word.Document)
            Dim oTable As Word.Table = Nothing

            Dim HeaderRow As Word.Row = Nothing
            Dim CallRow As Word.Row = Nothing
            Dim NoteRow As Word.Row = Nothing

            CreateTable(oDoc, oTable, HeaderRow, CallRow, NoteRow, (.AnrMonStatus = Telefonat.AnrufStatus.RING Or .AnrMonStatus = Telefonat.AnrufStatus.CALL) And Not XMLData.POptionen.CBAnrMonZeigeKontakt)
            If CallRow IsNot Nothing Then

                Select Case .AnrMonStatus
                    Case Telefonat.AnrufStatus.RING, Telefonat.AnrufStatus.CALL

                        CallRow.Cells(1).Range.Text = If(.AnrMonStatus = Telefonat.AnrufStatus.RING, "[<-]", "[->]")
                        CallRow.Cells(2).Range.Text = BenutzerInitialien()
                        CallRow.Cells(3).Range.Text = .GegenstelleTelNr.Formatiert
                        CallRow.Cells(4).Range.Text = .ZeitBeginn.ToShortDateString
                        CallRow.Cells(5).Range.Text = String.Empty
                        CallRow.Cells(6).Range.Text = String.Empty

                    Case Telefonat.AnrufStatus.CONNECT

                        CallRow.Cells(4).Range.Text = .ZeitVerbunden.ToShortDateString
                        UnSaveMethods.SetFocus(GetBodyHandle(oInsp)).ToString()

                    Case Telefonat.AnrufStatus.DISCONNECT

                        CallRow.Cells(5).Range.Text = .ZeitEnde.ToShortDateString
                        CallRow.Cells(6).Range.Text = .Dauer.ToString
                        UnSaveMethods.SetFocus(GetBodyHandle(oInsp)).ToString()

                End Select

            End If

            If Not XMLData.POptionen.CBAnrMonZeigeKontakt Then
                oPage = CType(oInsp.ModifiedFormPages, Outlook.Pages)
                oPage.Add("General")
                oInsp.HideFormPage("General")
                .OlKontakt.Save()
            End If
        End With
    End Sub

    Private Function BenutzerInitialien() As String
        Dim Regkey As RegistryKey = Nothing
        Dim UserInitials As String = "Initialien"

        Using key As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Office\Common\UserInfo")
            Try
                '64 Bit prüfen!
                UserInitials = Regkey.GetValue("UserInitials", "Initialien").ToString

            Catch ex As Exception
                NLogger.Error("Fehler beim Zugriff auf die Registry (BenutzerInitialien): " & ex.Message)
            End Try

        End Using

        Return UserInitials
    End Function

End Module
