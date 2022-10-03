Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Data

Public Class BoolToVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim boolValue As Boolean = CBool(value)
        If boolValue Then Return Visibility.Visible
        Return Visibility.Collapsed
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class BoolToInvertedBoolConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is Boolean Then
            Return Not CBool(value)
        Else
            Return False
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack() of BoolToInvertedBoolConverter is not implemented")
    End Function
End Class

Public Class ColorToBrushConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is Media.Color Then
            Return New Media.SolidColorBrush(CType(value, Media.Color))
        Else
            Return Nothing
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack() of ColorToBrushConverter is not implemented")
    End Function
End Class

Public Class StringToColorConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert

        If TypeOf value Is String Then
            Dim tmpStr As String = CStr(value)
            If tmpStr.IsNotStringNothingOrEmpty Then
                Return CType(Media.ColorConverter.ConvertFromString(tmpStr), Media.Color)
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack

        If TypeOf value Is Media.Color Then
            Return CType(value, Media.Color).ToString
        Else
            Return String.Empty
        End If

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

    Private Function GetEnumDescription(enumObj As [Enum]) As String
        Dim fieldInfo As FieldInfo = enumObj.[GetType]().GetField(enumObj.ToString())
        Dim attribArray As Object() = fieldInfo.GetCustomAttributes(False)

        If attribArray.Length = 0 Then
            Return enumObj.ToString()
        Else
            Dim attrib As DescriptionAttribute = TryCast(attribArray(0), DescriptionAttribute)
            Return attrib.Description
        End If
    End Function

    Private Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim description As String = String.Empty
        If value IsNot String.Empty Then description = GetEnumDescription(CType(value, [Enum]))

        Return description
    End Function

    Private Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return String.Empty
    End Function

End Class

Public Class DateToVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim DateValue As Date = CDate(value)
        If DateValue = Date.MinValue Then Return Visibility.Collapsed
        Return Visibility.Visible
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class TelNrToFontWeightConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert

        If values.All(Function(T) T IsNot Nothing) AndAlso values.First.GetType Is values.Last.GetType Then
            With values.Cast(Of Telefonnummer)
                Return If(.First.Equals(.Last), (New FontWeightConverter).ConvertFrom(parameter), FontWeights.Normal)
            End With
        End If
        Return FontWeights.Normal
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class EnumToVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing OrElse parameter Is Nothing OrElse Not (TypeOf value Is [Enum]) Then Return Visibility.Hidden

        Dim EnumState As String = value.ToString()
        Dim parameterString As String = parameter.ToString()

        For Each state As String In parameterString.Split(","c)
            If EnumState.Equals(state) Then Return Visibility.Visible
        Next

        Return Visibility.Hidden
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class EmptyComboConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing Then Return String.Empty
        Return value
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack

        If TypeOf value Is Controls.ComboBoxItem Then Return Nothing

        Return value

    End Function
End Class