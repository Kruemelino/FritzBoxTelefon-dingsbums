Friend Structure ExTAM

    ''' <summary>
    ''' 
    ''' </summary>
    Friend Property Enable As Boolean

    ''' <summary>
    ''' Friendly Name for TAM
    ''' </summary>
    Friend Property Name As String

    ''' <summary>
    ''' 1 running <br/> 0 Not running 
    ''' </summary>
    Friend Property TAMRunning As Boolean

    ''' <summary>
    ''' <list type="bullet">
    ''' <item>0 no USB memory stick</item>
    ''' <item>1 TAM already using USB memory stick</item>
    ''' <item>2 USB memory stick available but folder avm_tam missing</item>
    ''' </list>
    ''' </summary>
    Friend Property Stick As UShort

    '''  <summary>
    ''' <list type="bullet">
    ''' <item>Bit 0: busy</item>
    ''' <item>Bit 1: no space left</item>
    ''' <item>Bit 15: Display in WebUI</item>
    ''' </list>
    ''' </summary>
    Friend Property Status As UShort

    Friend Property Capacity As ULong

    ''' <summary>
    ''' play_announcement, record_message, timeprofile 
    ''' </summary>
    Friend Property Mode As String

    ''' <summary>
    ''' 0…255 <br/>
    ''' 0 immediately, 255 automatic
    ''' </summary>
    Friend Property RingSeconds As UShort

    ''' <summary>
    ''' Empty string represents all numbers. <br/>
    ''' Comma separated list represents specific phone numbers
    ''' </summary>
    Friend Property PhoneNumbers As String()
End Structure
