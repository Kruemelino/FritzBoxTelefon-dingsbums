Imports System.Collections

Namespace SOAP
    Friend Interface IService
        Property NLogger As Logger
        Property TR064Start As Func(Of String, String, Hashtable, Hashtable)
        Property PushStatus As Action(Of LogLevel, String)
    End Interface
End Namespace


