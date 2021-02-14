Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Data
''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class BoolToVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim boolValue = CBool(value)
        If boolValue Then Return Visibility.Visible
        Return Visibility.Collapsed
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class NullToVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value IsNot Nothing Then Return Visibility.Visible
        Return Visibility.Collapsed
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class IntegerToBoolConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Return CInt(value).Equals(1)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If CBool(value) Then
            Return 1
        Else
            Return 0
        End If
    End Function
End Class

Public Class EnumDescriptionConverter
    Implements IValueConverter

    Private Function GetEnumDescription(ByVal enumObj As [Enum]) As String
        Dim fieldInfo As FieldInfo = enumObj.[GetType]().GetField(enumObj.ToString())
        Dim attribArray As Object() = fieldInfo.GetCustomAttributes(False)

        If attribArray.Length = 0 Then
            Return enumObj.ToString()
        Else
            Dim attrib As DescriptionAttribute = TryCast(attribArray(0), DescriptionAttribute)
            Return attrib.Description
        End If
    End Function

    Private Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim myEnum As [Enum] = CType(value, [Enum])
        Dim description As String = GetEnumDescription(myEnum)
        Return description
    End Function

    Private Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return String.Empty
    End Function

End Class
