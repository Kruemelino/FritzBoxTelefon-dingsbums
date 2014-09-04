Imports System.Drawing
Friend Class CommonFenster

#Region "Enum"
    'Enum eStartPosition
    '    BottomRight
    '    BottomLeft
    '    TopLeft
    '    TopRight
    'End Enum

    'Enum eMoveDirection
    '    Y
    '    X
    'End Enum
#End Region

#Region "DefaultFont"
    Private sDefFontName As String = "Microsoft Sans Serif"
    Property DefFontName() As String
        Get
            Return sDefFontName
        End Get
        Set(ByVal value As String)
            sDefFontName = value
        End Set
    End Property

    Private sDefFontStyle As FontStyle = FontStyle.Regular
    Property DefFontStyle() As FontStyle
        Get
            Return sDefFontStyle
        End Get
        Set(ByVal value As FontStyle)
            sDefFontStyle = value
        End Set
    End Property

    Private sDefGraphicsUnit As GraphicsUnit = GraphicsUnit.Point
    Property DefGraphicsUnit() As GraphicsUnit
        Get
            Return sDefGraphicsUnit
        End Get
        Set(ByVal value As GraphicsUnit)
            sDefGraphicsUnit = value
        End Set
    End Property

    Private sDefgdiCharSet As Byte = CType(0, Byte)
    Property DefgdiCharSet() As Byte
        Get
            Return sDefgdiCharSet
        End Get
        Set(ByVal value As Byte)
            sDefgdiCharSet = value
        End Set
    End Property
#End Region

#Region "Properties Stoppuhr"

    Private clHeader As Color = SystemColors.ControlDarkDark
    Property HeaderColor() As Color
        Get
            Return clHeader
        End Get
        Set(ByVal value As Color)
            clHeader = value
        End Set
    End Property

    Private clBody As Color = SystemColors.Control
    Property BodyColor() As Color
        Get
            Return clBody
        End Get
        Set(ByVal value As Color)
            clBody = value
        End Set
    End Property

    Private clTitle As Color = SystemColors.ControlText
    Property TitleColor() As Color
        Get
            Return clTitle
        End Get
        Set(ByVal value As Color)
            clTitle = value
        End Set
    End Property

    Private clBase As Color = SystemColors.ControlText
    Property ContentColor() As Color
        Get
            Return clBase
        End Get
        Set(ByVal value As Color)
            clBase = value
        End Set
    End Property

    Private clBorder As Color = SystemColors.WindowFrame
    Property BorderColor() As Color
        Get
            Return clBorder
        End Get
        Set(ByVal value As Color)
            clBorder = value
        End Set
    End Property

    Private clCloseBorder As Color = SystemColors.WindowFrame
    Property ButtonBorderColor() As Color
        Get
            Return clCloseBorder
        End Get
        Set(ByVal value As Color)
            clCloseBorder = value
        End Set
    End Property

    Private clCloseHover As Color = SystemColors.Highlight
    Property ButtonHoverColor() As Color
        Get
            Return clCloseHover
        End Get
        Set(ByVal value As Color)
            clCloseHover = value
        End Set
    End Property

    Private iDiffGradient As Integer = 50
    Property GradientPower() As Integer
        Get
            Return iDiffGradient
        End Get
        Set(ByVal value As Integer)
            iDiffGradient = value
        End Set
    End Property

    Private ftSUBase As Font = New Font(DefFontName, 18.0!, DefFontStyle, sDefGraphicsUnit, DefgdiCharSet)
    Property fSUContentFont() As Font
        Get

            Return ftSUBase
        End Get
        Set(ByVal value As Font)
            ftSUBase = value
        End Set
    End Property

    Private pdTextPadding As Padding = New Padding(5)
    Property TextPadding() As Padding
        Get
            Return pdTextPadding
        End Get
        Set(ByVal value As Padding)
            pdTextPadding = value
        End Set
    End Property

    Private iHeaderHeight As Integer = 9
    Property HeaderHeight() As Integer
        Get
            Return iHeaderHeight
        End Get
        Set(ByVal value As Integer)
            iHeaderHeight = value
        End Set
    End Property

#End Region

#Region "Properties Anrufmonitor"

    Private clLinkHover As Color = SystemColors.Highlight 'SystemColors.HotTrack
    Property LinkHoverColor() As Color
        Get
            Return clLinkHover
        End Get
        Set(ByVal value As Color)
            clLinkHover = value

        End Set
    End Property

    Private bCloseButtonVisible As Boolean = True
    Property CloseButton() As Boolean
        Get
            Return bCloseButtonVisible
        End Get
        Set(ByVal value As Boolean)
            bCloseButtonVisible = value
        End Set
    End Property

    Private bOptionsButtonVisible As Boolean = True
    Property OptionsButton() As Boolean
        Get
            Return bOptionsButtonVisible
        End Get
        Set(ByVal value As Boolean)
            bOptionsButtonVisible = value
        End Set
    End Property

    Private ftBase As Font = New Font(DefFontName, 15.75!, DefFontStyle, sDefGraphicsUnit, DefgdiCharSet) 'SystemFonts.DialogFont
    Property ContentFont() As Font
        Get
            Return ftBase
        End Get
        Set(ByVal value As Font)
            ftBase = value
        End Set
    End Property

    Private ftTitle As Font = New Font(DefFontName, 8.25!, DefFontStyle, sDefGraphicsUnit, DefgdiCharSet) 'SystemFonts.CaptionFont
    Property TitleFont() As Font
        Get
            Return ftTitle
        End Get
        Set(ByVal value As Font)
            ftTitle = value
        End Set
    End Property

    Private iEffektMoveGeschwindigkeit As Integer = 5
    Property EffektMoveGeschwindigkeit() As Integer
        Get
            Return iEffektMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            iEffektMoveGeschwindigkeit = value
        End Set
    End Property

    'Private pStartpunkt As eStartPosition
    'Property Startpunkt() As eStartPosition
    '    Get
    '        Return pStartpunkt
    '    End Get
    '    Set(ByVal value As eStartPosition)
    '        pStartpunkt = value
    '    End Set
    'End Property

    'Private _MoveDirection As eMoveDirection
    'Property MoveDirecktion() As eMoveDirection
    '    Get
    '        Return _MoveDirection
    '    End Get
    '    Set(ByVal value As eMoveDirection)
    '        _MoveDirection = value
    '    End Set
    'End Property

    Private ftTelNr As Font = New Font(DefFontName, 11.25!, DefFontStyle, sDefGraphicsUnit, DefgdiCharSet) 'SystemFonts.CaptionFont
    Property TelNrFont() As Font
        Get
            Return ftTelNr
        End Get
        Set(ByVal value As Font)
            ftTelNr = value

        End Set
    End Property

    Private ptImagePosition As Point = New Point(12, 32) 'New Point(12, 21)
    Property ImagePosition() As Point
        Get
            Return ptImagePosition
        End Get
        Set(ByVal value As Point)
            ptImagePosition = value

        End Set
    End Property

    Private szImageSize As Size = New Size(48, 48) 'New Size(0, 0)
    Property ImageSize() As Size
        Get
            If szImageSize.Width = 0 Then
                If Image IsNot Nothing Then
                    Return Image.Size
                Else
                    Return New Size(32, 32)
                End If
            Else
                Return szImageSize
            End If
        End Get
        Set(ByVal value As Size)
            szImageSize = value
        End Set
    End Property

    Private imImage As Image = Nothing
    Property Image() As Image
        Get
            Return imImage
        End Get
        Set(ByVal value As Image)
            imImage = value
        End Set
    End Property

    Private sAnrName As String
    Property AnrName() As String
        Get
            Return sAnrName
        End Get
        Set(ByVal value As String)
            sAnrName = value
        End Set
    End Property

    Private sUhrzeit As String
    Property Uhrzeit() As String
        Get
            Return sUhrzeit
        End Get
        Set(ByVal value As String)
            sUhrzeit = value
        End Set
    End Property

    Private sTelNr As String
    Property TelNr() As String
        Get
            Return sTelNr
        End Get
        Set(ByVal value As String)
            sTelNr = value
        End Set
    End Property

    Private sTelName As String
    Property TelName() As String
        Get
            Return sTelName
        End Get
        Set(ByVal value As String)
            sTelName = value
        End Set
    End Property

    Private sFirma As String
    Property Firma() As String
        Get
            Return sFirma
        End Get
        Set(ByVal value As String)
            sFirma = value
        End Set
    End Property

#End Region

    Private Function AddValueMax255(ByVal Input As Integer, ByVal Add As Integer) As Integer
        If Input + Add < 256 Then
            Return Input + Add
        Else
            Return 255
        End If
    End Function

    Private Function DedValueMin0(ByVal Input As Integer, ByVal Ded As Integer) As Integer
        If Input - Ded > 0 Then
            Return Input - Ded
        Else
            Return 0
        End If
    End Function

    Friend Function GetDarkerColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(255, DedValueMin0(CInt(Color.R), GradientPower), DedValueMin0(CInt(Color.G), GradientPower), DedValueMin0(CInt(Color.B), GradientPower))
        Return clNew
    End Function

    Friend Function GetLighterColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(255, AddValueMax255(CInt(Color.R), GradientPower), AddValueMax255(CInt(Color.G), GradientPower), AddValueMax255(CInt(Color.B), GradientPower))
        Return clNew
    End Function

    'Private Function GetLighterTransparentColor(ByVal Color As Color) As Color
    '    Dim clNew As Color
    '    clNew = Drawing.Color.FromArgb(0, AddValueMax255(CInt(Color.R), GradientPower), AddValueMax255(CInt(Color.G), GradientPower), AddValueMax255(CInt(Color.B), GradientPower))
    '    Return clNew
    'End Function

End Class
