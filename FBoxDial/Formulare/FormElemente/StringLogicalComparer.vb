''' <summary>
''' https://github.com/dotnetprojects/DotNetSiemensPLCToolBoxLibrary/blob/master/LibNoDaveConnectionLibrary/General/StringLogicalComparer.cs
''' </summary>
<Obsolete> Friend Class StringLogicalComparer
    Implements IComparer(Of String)

    Public Function Compare(s1 As String, s2 As String) As Integer Implements IComparer(Of String).Compare
        If (s1 Is Nothing) AndAlso (s2 Is Nothing) Then
            Return 0
        ElseIf s1 Is Nothing Then
            Return -1
        ElseIf s2 Is Nothing Then
            Return 1
        End If

        If (s1.Equals(String.Empty) AndAlso (s2.Equals(String.Empty))) Then
            Return 0
        ElseIf s1.Equals(String.Empty) Then
            Return -1
        ElseIf s2.Equals(String.Empty) Then
            Return -1
        End If

        Dim sp1 As Boolean = Char.IsLetterOrDigit(s1, 0)
        Dim sp2 As Boolean = Char.IsLetterOrDigit(s2, 0)
        If sp1 AndAlso Not sp2 Then Return 1
        If Not sp1 AndAlso sp2 Then Return -1
        Dim i1 As Integer = 0, i2 As Integer = 0
        Dim r As Integer
        While True
            Dim c1 As Boolean = Char.IsDigit(s1, i1)
            Dim c2 As Boolean = Char.IsDigit(s2, i2)
            If Not c1 AndAlso Not c2 Then
                Dim letter1 As Boolean = Char.IsLetter(s1, i1)
                Dim letter2 As Boolean = Char.IsLetter(s2, i2)
                If (letter1 AndAlso letter2) OrElse (Not letter1 AndAlso Not letter2) Then
                    If letter1 AndAlso letter2 Then
                        r = Char.ToLower(s1(i1)).CompareTo(Char.ToLower(s2(i2)))
                    Else
                        r = s1(i1).CompareTo(s2(i2))
                    End If

                    If r <> 0 Then Return r
                ElseIf Not letter1 AndAlso letter2 Then
                    Return -1
                ElseIf letter1 AndAlso Not letter2 Then
                    Return 1
                End If
            ElseIf c1 AndAlso c2 Then
                r = CompareNum(s1, i1, s2, i2)
                If r <> 0 Then Return r
            ElseIf c1 Then
                Return -1
            ElseIf c2 Then
                Return 1
            End If

            i1 += 1
            i2 += 1
            If (i1 >= s1.Length) AndAlso (i2 >= s2.Length) Then
                Return 0
            ElseIf i1 >= s1.Length Then
                Return -1
            ElseIf i2 >= s2.Length Then
                Return -1
            End If
        End While
        Return 0
    End Function

    Private Shared Function CompareNum(s1 As String, ByRef i1 As Integer, s2 As String, ByRef i2 As Integer) As Integer
        Dim nzStart1 As Integer = i1, nzStart2 As Integer = i2
        Dim end1 As Integer = i1, end2 As Integer = i2
        Dim j1, j2 As Integer
        ScanNumEnd(s1, i1, end1, nzStart1)
        ScanNumEnd(s2, i2, end2, nzStart2)
        Dim start1 As Integer = i1
        i1 = end1 - 1
        Dim start2 As Integer = i2
        i2 = end2 - 1
        Dim nzLength1 As Integer = end1 - nzStart1
        Dim nzLength2 As Integer = end2 - nzStart2
        If nzLength1 < nzLength2 Then
            Return -1
        ElseIf nzLength1 > nzLength2 Then
            Return 1
        End If

        While j1 <= i1
            Dim r As Integer = s1(j1).CompareTo(s2(j2))
            If r <> 0 Then Return r
            j1 += 1
            j2 += 1
        End While

        Dim length1 As Integer = end1 - start1
        Dim length2 As Integer = end2 - start2
        If length1 = length2 Then Return 0
        If length1 > length2 Then Return -1
        Return 1
    End Function

    Private Shared Sub ScanNumEnd(s As String, start As Integer, ByRef [end] As Integer, ByRef nzStart As Integer)
        nzStart = start
        [end] = start
        Dim countZeros As Boolean = True
        While Char.IsDigit(s, [end])
            If countZeros AndAlso s([end]).Equals("0"c) Then
                nzStart += 1
            Else
                countZeros = False
            End If

            [end] += 1
            If [end] >= s.Length Then Exit While
        End While
    End Sub
End Class
