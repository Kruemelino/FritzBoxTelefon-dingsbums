Imports System.IO
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Friend Module JSONSerialize

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property JSONTraceWriter As New JSONTraceWriter

#Region "JSON"
#Region "JSON Deserialisieren"

    ''' <summary>
    ''' JSON Serialisierungseinstellungen. Es werden die Fehlerbehandlung, die Converter (Boolean) und das Logging definiert.
    ''' </summary>
    Private ReadOnly Property JSS As New JsonSerializerSettings With {.[Error] = AddressOf OnJSONDeserializeError,
                                                                      .Converters = {New JSONBooleanConverter},
                                                                      .TraceWriter = JSONTraceWriter}
    ''' <summary>
    ''' Deserialisiert einen JSON-Datei vom Datenträger in eine Klasse vom Typ <typeparamref name="T"/>.
    ''' </summary>
    ''' <typeparam name="T">Zieldatentyp</typeparam>
    ''' <param name="FilePath">Pfad zur Datei</param>
    ''' <param name="LogOff">Angabe, ob das Logging deaktiviert werden soll.</param>>
    Friend Async Function JSONDeserializeFromFileAsync(Of T)(FilePath As String, Optional LogOff As Boolean = False) As Task(Of T)
        Return Await Task.Run(Function()
                                  ' Schalte das JSON Logging aus, falls dies nicht erwünscht ist.
                                  JSONTraceWriter.JSONLoggerOff = LogOff

                                  Using FS As New FileStream(FilePath, FileMode.Open)
                                      Using SR As New StreamReader(FS)
                                          Using JR As New JsonTextReader(SR)
                                              Dim JS As JsonSerializer = JsonSerializer.Create(JSS)

                                              Return JS.Deserialize(Of T)(JR)
                                          End Using
                                      End Using
                                  End Using

                                  ' Schalte das JSON Logging in den Ursprungszustand
                                  JSONTraceWriter.JSONLoggerOff = False
                              End Function)
    End Function

    ''' <summary>
    ''' Deserialisiert einen Datenstrom in eine Klasse vom Typ <typeparamref name="T"/>.
    ''' </summary>
    ''' <typeparam name="T">Zieldatentyp</typeparam>
    ''' <param name="S">Datenstrom</param>
    ''' <param name="LogOff">Angabe, ob das Logging deaktiviert werden soll.</param>>
    Friend Async Function JSONDeserializeFromStreamAsync(Of T)(S As Stream, Optional LogOff As Boolean = False) As Task(Of T)
        Return Await Task.Run(Function()
                                  ' Schalte das JSON Logging aus, falls dies nicht erwünscht ist.
                                  JSONTraceWriter.JSONLoggerOff = LogOff

                                  Using SR As New StreamReader(S)
                                      Using JR As New JsonTextReader(SR)
                                          Dim JS As JsonSerializer = JsonSerializer.Create(JSS)

                                          Return JS.Deserialize(Of T)(JR)

                                      End Using
                                  End Using

                                  ' Schalte das JSON Logging in den Ursprungszustand
                                  JSONTraceWriter.JSONLoggerOff = False
                              End Function)
    End Function

#End Region

#Region "JSON Serialisieren"
    ''' <summary>
    ''' Serialisiert ein Datenobjekt vom Type <typeparamref name="T"/> in eine Datei auf dem Dateisystem. 
    ''' </summary>
    ''' <typeparam name="T">JSON Serialisierbarer Typ</typeparam>
    ''' <param name="FilePath">Pfad zur Datei, in dem die Daten serialisiert gespeichert werden sollen.</param>
    ''' <param name="Data">Die zu serialisierenden Daten</param>
    ''' <param name="LogOff">Angabe, ob das Logging deaktiviert werden soll.</param>
    Friend Async Sub JSONSerializeToFileAsync(Of T)(FilePath As String, Data As T, Optional LogOff As Boolean = False)
        Await Task.Run(Sub()
                           ' Schalte das JSON Logging aus, falls dies nicht erwünscht ist.
                           JSONTraceWriter.JSONLoggerOff = LogOff

                           Using FS As New FileStream(FilePath, FileMode.Create)
                               Using SW As New StreamWriter(FS)
                                   Using JW As New JsonTextWriter(SW)
                                       Dim JS As JsonSerializer = JsonSerializer.Create(JSS)
                                       JS.Serialize(JW, Data, GetType(T))
                                   End Using
                               End Using
                           End Using

                           ' Schalte das JSON Logging in den Ursprungszustand
                           JSONTraceWriter.JSONLoggerOff = False
                       End Sub)
    End Sub
#End Region

#Region "JSON OnError"
    ''' <summary>
    ''' Fehlerbehandlung während des Serialisierens.
    ''' <see href="https://www.newtonsoft.com/json/help/html/SerializationErrorHandling.htm"/>
    ''' </summary>
    Private Sub OnJSONDeserializeError(sender As Object, e As Newtonsoft.Json.Serialization.ErrorEventArgs)
        With e.ErrorContext
            ' Log Message
            NLogger.Error(.Error.Message)

            .Handled = True
        End With

    End Sub
#End Region

#End Region

End Module
