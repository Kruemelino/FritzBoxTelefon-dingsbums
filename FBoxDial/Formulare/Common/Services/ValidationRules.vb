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
        Dim Input As Integer = 0
        Try
            If CStr(value).Length.IsNotZero Then Input = Integer.Parse(CStr(value))
        Catch ex As Exception
            NLogger.Warn(ex)
            Return New ValidationResult(False, String.Format(resCommon.strValidationIntChr, ex.Message))
        End Try

        ' Es gibt nur eine untere Grenze
        If Min.IsLarger(Max) AndAlso Input.IsLess(Min) Then
            NLogger.Warn($"Der eingegebene Wert ({Input}) ist kleiner als der Mindestwert von {Min}.")
            Return New ValidationResult(False, String.Format(resCommon.strValidationIntLess, Min))
        End If

        ' Es gibt nur eine obere Grenze
        If Min.AreEqual(Max) AndAlso Input.IsLarger(Max) Then
            NLogger.Warn($"Der eingegebene Wert ({Input}) ist größer als der Maximalwert von {Max}.")
            Return New ValidationResult(False, String.Format(resCommon.strValidationIntLarger, Max))
        End If

        ' Es wird ein Bereich festgelegt
        If Min.IsLess(Max) AndAlso Not Input.IsInRange(Min, Max) Then
            NLogger.Warn($"Der eingegebene Wert ({Input}) muss im Bereich zwischen {Min} und {Max} liegen.")
            Return New ValidationResult(False, String.Format(resCommon.strValidationIntRange, Min, Max))
        End If

        Return ValidationResult.ValidResult
    End Function
End Class

Public Class StrValidationRule
    Inherits ValidationRule

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public Property RegExPattern As String

    Public Overrides Function Validate(value As Object, cultureInfo As Globalization.CultureInfo) As ValidationResult

        Dim Input As String = CStr(value)

        If Not Input.IsRegExMatch(RegExPattern) Then
            NLogger.Warn($"Die eingegebene Zeichenfolge ({Input}) entspricht nicht den erwarteten Format '{RegExPattern}'.")
            Return New ValidationResult(False, $"Die eingegebene Zeichenfolge entspricht nicht den erwarteten Format '{RegExPattern}'.")
        End If

        Return ValidationResult.ValidResult
    End Function
End Class