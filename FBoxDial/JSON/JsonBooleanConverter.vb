Imports Newtonsoft.Json
Public Class JsonBooleanConverter
    Inherits JsonConverter(Of Boolean)

    Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Boolean, hasExistingValue As Boolean, serializer As JsonSerializer) As Boolean
        If reader.ValueType Is GetType(String) Then
            Dim StrVal As String = reader.Value.ToString

            If StrVal.IsNotStringNothingOrEmpty Then
                Dim StrBool As Boolean
                If Boolean.TryParse(StrVal, StrBool) Then
                    Return True
                Else
                    Return Convert.ToBoolean(Convert.ToInt32(StrVal))
                End If

            Else
                Return False

            End If
        Else
            Return Convert.ToBoolean(reader.Value)
        End If
    End Function

    Public Overrides Sub WriteJson(writer As JsonWriter, value As Boolean, serializer As JsonSerializer)
        serializer.Serialize(writer, value)
    End Sub
End Class

