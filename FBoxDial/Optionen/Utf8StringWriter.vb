Imports System.IO

Public Class Utf8StringWriter
    Inherits StringWriter

    ' Use UTF8 encoding but write no BOM to the wire
    Public Overrides ReadOnly Property Encoding As Encoding
        Get
            Return New UTF8Encoding(False)
        End Get ' in real code I'll cache this encoding.
    End Property
End Class
