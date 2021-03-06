Imports System.Windows
Imports System.Windows.Controls

''' <summary>
''' <code>http://www.nullskull.com/a/1401/creating-a-wpf-custom-control.aspx</code>
''' </summary>
<TemplatePart(Name:=TimePicker.ElementHourTextBox, Type:=GetType(TextBox))>
<TemplatePart(Name:=TimePicker.ElementMinuteTextBox, Type:=GetType(TextBox))>
<TemplatePart(Name:=TimePicker.ElementSecondTextBox, Type:=GetType(TextBox))>
<TemplatePart(Name:=TimePicker.ElementIncrementButton, Type:=GetType(Button))>
<TemplatePart(Name:=TimePicker.ElementDecrementButton, Type:=GetType(Button))>
Public Class TimePicker
    Inherits Control
#Region "Constants"

    Friend Const ElementHourTextBox As String = "PART_HourTextBox"
    Friend Const ElementMinuteTextBox As String = "PART_MinuteTextBox"
    Friend Const ElementSecondTextBox As String = "PART_SecondTextBox"
    Friend Const ElementIncrementButton As String = "PART_IncrementButton"
    Friend Const ElementDecrementButton As String = "PART_DecrementButton"

#End Region

#Region "Data"

    Private Shared ReadOnly MinValidTime As TimeSpan = New TimeSpan(0, 0, 0)
    Private Shared ReadOnly MaxValidTime As TimeSpan = New TimeSpan(23, 59, 59)
    Private hourTextBox As TextBox
    Private minuteTextBox As TextBox
    Private secondTextBox As TextBox
    Private incrementButton As Button
    Private decrementButton As Button
    Private selectedTextBox As TextBox

#End Region

#Region "Ctor"

    Shared Sub New()
        Call DefaultStyleKeyProperty.OverrideMetadata(GetType(TimePicker), New FrameworkPropertyMetadata(GetType(TimePicker)))
    End Sub

    Public Sub New()
        SelectedTime = Date.Now.TimeOfDay
    End Sub

#End Region

#Region "Public Properties"

#Region "SelectedTime"

    Public Property SelectedTime As TimeSpan
        Get
            Return CType(GetValue(SelectedTimeProperty), TimeSpan)
        End Get
        Set
            SetValue(SelectedTimeProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly SelectedTimeProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedTime), GetType(TimeSpan), GetType(TimePicker), New FrameworkPropertyMetadata(MinValidTime, New PropertyChangedCallback(AddressOf OnSelectedTimeChanged), New CoerceValueCallback(AddressOf CoerceSelectedTime)))

#End Region

#Region "MinTime"

    Public Property MinTime As TimeSpan
        Get
            Return CType(GetValue(MinTimeProperty), TimeSpan)
        End Get
        Set
            SetValue(MinTimeProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly MinTimeProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinTime), GetType(TimeSpan), GetType(TimePicker), New FrameworkPropertyMetadata(MinValidTime, New PropertyChangedCallback(AddressOf OnMinTimeChanged)), New ValidateValueCallback(AddressOf IsValidTime))

#End Region

#Region "MaxTime"

    Public Property MaxTime As TimeSpan
        Get
            Return CType(GetValue(MaxTimeProperty), TimeSpan)
        End Get
        Set
            SetValue(MaxTimeProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly MaxTimeProperty As DependencyProperty = DependencyProperty.Register("MaxTime", GetType(TimeSpan), GetType(TimePicker), New FrameworkPropertyMetadata(MaxValidTime, New PropertyChangedCallback(AddressOf OnMaxTimeChanged), New CoerceValueCallback(AddressOf CoerceMaxTime)), New ValidateValueCallback(AddressOf IsValidTime))

#End Region

#Region "SelectedTimeChangedEvent"

    Public Custom Event SelectedTimeChanged As RoutedPropertyChangedEventHandler(Of TimeSpan)
        AddHandler(value As RoutedPropertyChangedEventHandler(Of TimeSpan))
            [AddHandler](SelectedTimeChangedEvent, value)
        End AddHandler
        RemoveHandler(value As RoutedPropertyChangedEventHandler(Of TimeSpan))
            [RemoveHandler](SelectedTimeChangedEvent, value)
        End RemoveHandler
        RaiseEvent(sender As Object, e As RoutedPropertyChangedEventArgs(Of TimeSpan))
        End RaiseEvent
    End Event

    Public Shared ReadOnly SelectedTimeChangedEvent As RoutedEvent = EventManager.RegisterRoutedEvent("SelectedTimeChanged", RoutingStrategy.Bubble, GetType(RoutedPropertyChangedEventHandler(Of TimeSpan)), GetType(TimePicker))

#End Region

#End Region

#Region "Public Methods"

    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()
        hourTextBox = TryCast(GetTemplateChild(ElementHourTextBox), TextBox)

        If hourTextBox IsNot Nothing Then
            hourTextBox.IsReadOnly = True
            AddHandler hourTextBox.GotFocus, AddressOf SelectTextBox
        End If

        minuteTextBox = TryCast(GetTemplateChild(ElementMinuteTextBox), TextBox)

        If minuteTextBox IsNot Nothing Then
            minuteTextBox.IsReadOnly = True
            AddHandler minuteTextBox.GotFocus, AddressOf SelectTextBox
        End If

        secondTextBox = TryCast(GetTemplateChild(ElementSecondTextBox), TextBox)

        If secondTextBox IsNot Nothing Then
            secondTextBox.IsReadOnly = True
            AddHandler secondTextBox.GotFocus, AddressOf SelectTextBox
        End If

        incrementButton = TryCast(GetTemplateChild(ElementIncrementButton), Button)

        If incrementButton IsNot Nothing Then
            AddHandler incrementButton.Click, AddressOf IncrementTime
        End If

        decrementButton = TryCast(GetTemplateChild(ElementDecrementButton), Button)

        If decrementButton IsNot Nothing Then
            AddHandler decrementButton.Click, AddressOf DecrementTime
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Shared Function CoerceSelectedTime(d As DependencyObject, Value As Object) As TimeSpan
        Dim timePicker = CType(d, TimePicker)
        Dim minimum = timePicker.MinTime
        Dim retTimeSpan As TimeSpan = CType(Value, TimeSpan)

        If retTimeSpan < minimum Then Return minimum

        Dim maximum = timePicker.MaxTime

        If retTimeSpan > maximum Then Return maximum

        Return retTimeSpan
    End Function

    Private Shared Function CoerceMaxTime(d As DependencyObject, Value As Object) As TimeSpan
        Dim timePicker = CType(d, TimePicker)
        Dim minimum = timePicker.MinTime
        Dim retTimeSpan As TimeSpan = CType(Value, TimeSpan)

        If retTimeSpan < minimum Then Return minimum

        Return retTimeSpan
    End Function

    Private Shared Function IsValidTime(Value As Object) As Boolean
        Dim time As TimeSpan = CType(Value, TimeSpan)
        Return MinValidTime <= time AndAlso time <= MaxValidTime
    End Function

    Protected Overridable Sub OnSelectedTimeChanged(oldSelectedTime As TimeSpan, newSelectedTime As TimeSpan)
        Dim e As New RoutedPropertyChangedEventArgs(Of TimeSpan)(oldSelectedTime, newSelectedTime) With {.RoutedEvent = SelectedTimeChangedEvent}
        [RaiseEvent](e)
    End Sub

    Private Shared Sub OnSelectedTimeChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim element = CType(d, TimePicker)
        element.OnSelectedTimeChanged(CType(e.OldValue, TimeSpan), CType(e.NewValue, TimeSpan))
    End Sub

    Protected Overridable Sub OnMinTimeChanged(oldMinTime As TimeSpan, newMinTime As TimeSpan)
    End Sub

    Private Shared Sub OnMinTimeChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim element = CType(d, TimePicker)
        element.CoerceValue(MaxTimeProperty)
        element.CoerceValue(SelectedTimeProperty)
        element.OnMinTimeChanged(CType(e.OldValue, TimeSpan), CType(e.NewValue, TimeSpan))
    End Sub

    Protected Overridable Sub OnMaxTimeChanged(oldMaxTime As TimeSpan, newMaxTime As TimeSpan)
    End Sub

    Private Shared Sub OnMaxTimeChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim element = CType(d, TimePicker)
        element.CoerceValue(SelectedTimeProperty)
        element.OnMaxTimeChanged(CType(e.OldValue, TimeSpan), CType(e.NewValue, TimeSpan))
    End Sub

    Private Sub SelectTextBox(sender As Object, e As RoutedEventArgs)
        selectedTextBox = TryCast(sender, TextBox)
    End Sub

    Private Sub IncrementTime(sender As Object, e As RoutedEventArgs)
        IncrementDecrementTime(1)
    End Sub

    Private Sub DecrementTime(sender As Object, e As RoutedEventArgs)
        IncrementDecrementTime(-1)
    End Sub

    Private Sub IncrementDecrementTime([step] As Integer)
        If selectedTextBox Is Nothing Then
            selectedTextBox = hourTextBox
        End If

        Dim time As TimeSpan

        If selectedTextBox Is hourTextBox Then
            time = SelectedTime.Add(New TimeSpan([step], 0, 0))
        ElseIf selectedTextBox Is minuteTextBox Then
            time = SelectedTime.Add(New TimeSpan(0, [step], 0))
        Else
            time = SelectedTime.Add(New TimeSpan(0, 0, [step]))
        End If

        SelectedTime = time
    End Sub

#End Region
End Class

