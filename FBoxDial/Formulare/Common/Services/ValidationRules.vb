Imports System.Windows.Controls
Imports FBoxDial.Localize

Public Class IntValidationRule
    Inherits ValidationRule

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Public Property Min As Integer
    Public Property Max As Integer

    ''' <summary>
    ''' Die Grenzen müssen wie folgt festgelegt werden:<br/>
    ''' Für eine untere Grenze: Max &lt; Min<br/>
    ''' Für eine obere Grenze: Max = Min<br/>
    ''' Für einen Bereich: Min &lt; Max
    ''' </summary>
    Public Overrides Function Validate(value As Object, cultureInfo As Globalization.CultureInfo) As ValidationResult
        Dim num1 As Integer = 0
        Try
            If (CStr(value).Length.IsNotZero) Then
                num1 = Integer.Parse(CStr(value))
            End If
        Catch ex As Exception
            NLogger.Warn(ex)
            Return New ValidationResult(False, String.Format(resCommon.strValidationIntChr, ex.Message))
        End Try

        ' Es gibt nur eine untere Grenze
        If Min.IsLarger(Max) AndAlso num1.IsLess(Min) Then
            NLogger.Warn($"Der eingegebene Wert ({num1}) ist kleiner als der Mindestwert von {Min}")
            Return New ValidationResult(False, String.Format(resCommon.strValidationIntLess, Min))
        End If

        ' Es gibt nur eine obere Grenze
        If Min.AreEqual(Max) AndAlso num1.IsLarger(Max) Then
            NLogger.Warn($"Der eingegebene Wert ({num1}) ist größer als der Maximalwert von {Max}")
            Return New ValidationResult(False, String.Format(resCommon.strValidationIntLarger, Max))
        End If

        ' Es wird ein Bereich festgelegt
        If Min.IsLess(Max) AndAlso Not num1.IsInRange(Min, Max) Then
            NLogger.Warn($"Der eingegebene Wert ({num1}) muss im Bereich zwischen {Min} und {Max} liegen.")
            Return New ValidationResult(False, String.Format(resCommon.strValidationIntRange, Min, Max))
        End If

        Return ValidationResult.ValidResult
    End Function
End Class
