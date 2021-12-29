Imports System.IO

Public Class Utf8StringWriter
    Inherits StringWriter

    ''' <summary>
    ''' Use UTF8 encoding but write no BOM to the wire
    ''' </summary>
    Public Overrides ReadOnly Property Encoding As Encoding
        Get
            Return New UTF8Encoding(False)
        End Get
    End Property
End Class

