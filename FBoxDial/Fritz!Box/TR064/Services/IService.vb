Imports System.Collections

Namespace TR064
    Friend Interface IService
        Property NLogger As Logger
        Property TR064Start As Func(Of String, String, Hashtable, Hashtable)
        Property PushStatus As Action(Of LogLevel, String)
    End Interface
End Namespace


