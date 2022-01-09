Imports System.Windows
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input
''' <summary>
''' In Anlehnung an Dirk Bahle: <code>https://www.codeproject.com/Articles/1224943/Advanced-WPF-TreeView-in-Csharp-VB-Net-Part-of-n</code>
''' </summary>
Public Module VirtualToggleButton
#Region "attached properties"
#Region "ChangedCommand"
    ''' <summary>
    ''' Field of attached ICommand property
    ''' </summary>
    Public ReadOnly ChangedCommandProperty As DependencyProperty = DependencyProperty.RegisterAttached("ChangedCommand", GetType(ICommand), GetType(VirtualToggleButton), New PropertyMetadata(Nothing))

    ''' <summary>
    ''' Setter method of the attached ChangedCommand <seealso cref="ICommand"/> property
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="value"></param>
    Public Sub SetChangedCommand(source As DependencyObject, value As ICommand)
        source.SetValue(ChangedCommandProperty, value)
    End Sub

    ''' <summary>
    ''' Getter method of the attached ChangedCommand <seealso cref="ICommand"/> property
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    Public Function GetChangedCommand(source As DependencyObject) As ICommand
        Return TryCast(source.GetValue(ChangedCommandProperty), ICommand)
    End Function
#End Region

#Region "IsChecked"
    ''' <summary>
    ''' IsChecked Attached Dependency Property
    ''' </summary>
    Public ReadOnly IsCheckedProperty As DependencyProperty = DependencyProperty.RegisterAttached("IsChecked", GetType(Boolean?), GetType(VirtualToggleButton), New FrameworkPropertyMetadata(CType(False, Boolean?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault Or FrameworkPropertyMetadataOptions.Journal, New PropertyChangedCallback(AddressOf OnIsCheckedChanged)))

    ''' <summary>
    ''' Gets the IsChecked property.  This dependency property  indicates whether the toggle button is checked.
    ''' </summary>
    Function GetIsChecked(d As DependencyObject) As Boolean?
        Return CType(d.GetValue(IsCheckedProperty), Boolean?)
    End Function

    ''' <summary>
    ''' Sets the IsChecked property.  This dependency property indicates whether the toggle button is checked.
    ''' </summary>
    Sub SetIsChecked(d As DependencyObject, value As Boolean?)
        d.SetValue(IsCheckedProperty, value)
    End Sub

    ''' <summary>
    ''' Handles changes to the IsChecked property.
    ''' </summary>
    Private Sub OnIsCheckedChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim pseudobutton As UIElement = TryCast(d, UIElement)

        If pseudobutton IsNot Nothing Then
            Dim newValue As Boolean? = CType(e.NewValue, Boolean?)

            If newValue = True Then
                RaiseCheckedEvent(pseudobutton)
            ElseIf newValue = False Then
                RaiseUncheckedEvent(pseudobutton)
            Else
                RaiseIndeterminateEvent(pseudobutton)
            End If
        End If
    End Sub
#End Region

#Region "IsThreeState"
    ''' <summary>
    ''' IsThreeState Attached Dependency Property
    ''' </summary>
    Public ReadOnly IsThreeStateProperty As DependencyProperty = DependencyProperty.RegisterAttached("IsThreeState", GetType(Boolean), GetType(VirtualToggleButton), New FrameworkPropertyMetadata(False))

    ''' <summary>
    ''' Gets the IsThreeState property.  This dependency property 
    ''' indicates whether the control supports two or three states.  
    ''' IsChecked can be set to null as a third state when IsThreeState is true.
    ''' </summary>
    Function GetIsThreeState(d As DependencyObject) As Boolean
        Return CBool(d.GetValue(IsThreeStateProperty))
    End Function

    ''' <summary>
    ''' Sets the IsThreeState property.  This dependency property 
    ''' indicates whether the control supports two or three states. 
    ''' IsChecked can be set to null as a third state when IsThreeState is true.
    ''' </summary>
    Sub SetIsThreeState(d As DependencyObject, value As Boolean)
        d.SetValue(IsThreeStateProperty, value)
    End Sub
#End Region

#Region "IsVirtualToggleButton"
    ''' <summary>
    ''' IsVirtualToggleButton Attached Dependency Property
    ''' </summary>
    Public ReadOnly IsVirtualToggleButtonProperty As DependencyProperty = DependencyProperty.RegisterAttached("IsVirtualToggleButton", GetType(Boolean), GetType(VirtualToggleButton), New FrameworkPropertyMetadata(False, New PropertyChangedCallback(AddressOf OnIsVirtualToggleButtonChanged)))

    ''' <summary>
    ''' Gets the IsVirtualToggleButton property.  This dependency property 
    ''' indicates whether the object to which the property is attached is treated as a VirtualToggleButton.  
    ''' If true, the object will respond to keyboard and mouse input the same way a ToggleButton would.
    ''' </summary>
    Function GetIsVirtualToggleButton(d As DependencyObject) As Boolean
        Return CBool(d.GetValue(IsVirtualToggleButtonProperty))
    End Function

    ''' <summary>
    ''' Sets the IsVirtualToggleButton property.  This dependency property 
    ''' indicates whether the object to which the property is attached is treated as a VirtualToggleButton.  
    ''' If true, the object will respond to keyboard and mouse input the same way a ToggleButton would.
    ''' </summary>
    Sub SetIsVirtualToggleButton(d As DependencyObject, value As Boolean)
        d.SetValue(IsVirtualToggleButtonProperty, value)
    End Sub

    ''' <summary>
    ''' Handles changes to the IsVirtualToggleButton property.
    ''' </summary>
    Private Sub OnIsVirtualToggleButtonChanged(ByVal d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim element As IInputElement = TryCast(d, IInputElement)

        If element IsNot Nothing Then
            If CBool(e.NewValue) Then
                AddHandler element.MouseLeftButtonDown, AddressOf OnMouseLeftButtonDown
                AddHandler element.KeyDown, AddressOf OnKeyDown
            Else
                AddHandler element.MouseLeftButtonDown, AddressOf OnMouseLeftButtonDown
                AddHandler element.KeyDown, AddressOf OnKeyDown
            End If
        End If
    End Sub
#End Region

#End Region

#Region "routed events"

#Region "Checked"
    ''' <summary>
    ''' A static helper method to raise the Checked event on a target element.
    ''' </summary>
    ''' <param name="target">UIElement or ContentElement on which to raise the event</param>
    Friend Function RaiseCheckedEvent(target As UIElement) As RoutedEventArgs

        If target IsNot Nothing Then
            Dim args As New RoutedEventArgs With {.RoutedEvent = ToggleButton.CheckedEvent}
            [RaiseEvent](target, args)
            Return args
        Else
            Return Nothing
        End If

    End Function
#End Region

#Region "Unchecked"
    ''' <summary>
    ''' A static helper method to raise the Unchecked event on a target element.
    ''' </summary>
    ''' <param name="target">UIElement or ContentElement on which to raise the event</param>
    Friend Function RaiseUncheckedEvent(target As UIElement) As RoutedEventArgs

        If target IsNot Nothing Then
            Dim args As New RoutedEventArgs With {.RoutedEvent = ToggleButton.UncheckedEvent}
            [RaiseEvent](target, args)
            Return args
        Else
            Return Nothing
        End If

    End Function
#End Region

#Region "Indeterminate"
    ''' <summary>
    ''' A static helper method to raise the Indeterminate event on a target element.
    ''' </summary>
    ''' <param name="target">UIElement or ContentElement on which to raise the event</param>
    Friend Function RaiseIndeterminateEvent(target As UIElement) As RoutedEventArgs

        If target IsNot Nothing Then
            Dim args As New RoutedEventArgs With {.RoutedEvent = ToggleButton.IndeterminateEvent}
            [RaiseEvent](target, args)
            Return args
        Else
            Return Nothing
        End If

    End Function
#End Region

#End Region

#Region "private methods"
    Private Sub OnMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        UpdateIsChecked(TryCast(sender, DependencyObject))
    End Sub

    Private Sub OnKeyDown(sender As Object, e As KeyEventArgs)
        If Object.Equals(e.OriginalSource, sender) Then
            If e.Key = Key.Space Then
                '' ignore alt+space which invokes the system menu
                If (Keyboard.Modifiers And ModifierKeys.Alt) = ModifierKeys.Alt Then Return
                UpdateIsChecked(TryCast(sender, DependencyObject))
                e.Handled = True
            ElseIf e.Key = Key.Enter AndAlso CBool((TryCast(sender, DependencyObject)).GetValue(KeyboardNavigation.AcceptsReturnProperty)) Then
                UpdateIsChecked(TryCast(sender, DependencyObject))
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub UpdateIsChecked(d As DependencyObject)
        Dim isChecked As Boolean? = GetIsChecked(d)
        If isChecked = True Then
            SetIsChecked(d, If(GetIsThreeState(d), Nothing, CType(False, Boolean?)))
        Else
            SetIsChecked(d, isChecked.HasValue)
        End If


        Dim element = TryCast(d, FrameworkElement)
        Dim changedCommand As ICommand
        changedCommand = GetChangedCommand(d)  '' Call additional command To process this change

        '' There may Not be a command bound to this after all
        If changedCommand Is Nothing Or element Is Nothing Then
            Return
        End If

        '' Check whether this attached behaviour Is bound to a RoutedCommand
        If TypeOf changedCommand Is RoutedCommand Then

            '' Execute the routed command
            Dim routCommand As RoutedCommand
            routCommand = TryCast(changedCommand, RoutedCommand)
            routCommand.Execute(element.DataContext, element)

        Else
            '' Execute the Command as bound delegate
            changedCommand.Execute(element.DataContext)
        End If
    End Sub

    Private Sub [RaiseEvent](target As DependencyObject, args As RoutedEventArgs)
        If TypeOf target Is UIElement Then
            Dim element = TryCast(target, UIElement)
            element.[RaiseEvent](args)
        ElseIf TypeOf target Is ContentElement Then
            Dim element = TryCast(target, ContentElement)
            element.[RaiseEvent](args)
        End If
    End Sub
#End Region
End Module

