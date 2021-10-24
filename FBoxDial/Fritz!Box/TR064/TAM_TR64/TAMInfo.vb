Namespace TR064
    Public Class TAMInfo
        Inherits NotifyBase

        Private _Enable As Boolean
        Public Property Enable As Boolean
            Get
                Return _Enable
            End Get
            Set
                SetProperty(_Enable, Value)
            End Set
        End Property

        Private _Name As String
        ''' <summary>
        ''' Publicly Name for TAM
        ''' </summary>
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set
                SetProperty(_Name, Value)
            End Set
        End Property

        Private _TAMRunning As Boolean
        ''' <summary>
        ''' 1 running <br/> 0 Not running 
        ''' </summary>
        Public Property TAMRunning As Boolean
            Get
                Return _TAMRunning
            End Get
            Set
                SetProperty(_TAMRunning, Value)
            End Set
        End Property

        Private _Stick As UShort
        ''' <summary>
        ''' <list type="bullet">
        ''' <item>0 no USB memory stick</item>
        ''' <item>1 TAM already using USB memory stick</item>
        ''' <item>2 USB memory stick available but folder avm_tam missing</item>
        ''' </list>
        ''' </summary>
        Public Property Stick As UShort
            Get
                Return _Stick
            End Get
            Set
                SetProperty(_Stick, Value)
            End Set
        End Property

        Private _Status As UShort
        '''  <summary>
        ''' <list type="bullet">
        ''' <item>Bit 0: busy</item>
        ''' <item>Bit 1: no space left</item>
        ''' <item>Bit 15: Display in WebUI</item>
        ''' </list>
        ''' </summary>
        Public Property Status As UShort
            Get
                Return _Status
            End Get
            Set
                SetProperty(_Status, Value)
            End Set
        End Property

        Private _Capacity As ULong
        Public Property Capacity As ULong
            Get
                Return _Capacity
            End Get
            Set
                SetProperty(_Capacity, Value)
            End Set
        End Property

        Private _Mode As String
        ''' <summary>
        ''' play_announcement, record_message, timeprofile 
        ''' </summary>
        Public Property Mode As String
            Get
                Return _Mode
            End Get
            Set
                SetProperty(_Mode, Value)
            End Set
        End Property

        Private _RingSeconds As UShort
        ''' <summary>
        ''' 0…255 <br/>
        ''' 0 immediately, 255 automatic
        ''' </summary>
        Public Property RingSeconds As UShort
            Get
                Return _RingSeconds
            End Get
            Set
                SetProperty(_RingSeconds, Value)
            End Set
        End Property

        Private _PhoneNumbers As String()
        ''' <summary>
        ''' Empty string represents all numbers. <br/>
        ''' Comma separated list represents specific phone numbers
        ''' </summary>
        Public Property PhoneNumbers As String()
            Get
                Return _PhoneNumbers
            End Get
            Set
                SetProperty(_PhoneNumbers, Value)
            End Set
        End Property
    End Class

End Namespace