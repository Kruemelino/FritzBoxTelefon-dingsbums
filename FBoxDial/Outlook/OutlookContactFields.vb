''' <summary>
''' Alle relevanten E-Mailfelder  aus Outlook Kontakten.
''' </summary>
Friend Structure OutlookContactEMailFields
    ' E-Mail-Felder
    Public Shared ReadOnly Property EMail1Address As String = """http://schemas.microsoft.com/mapi/id/{00062004-0000-0000-C000-000000000046}/8084001f"""
    Public Shared ReadOnly Property EMail2Address As String = """http://schemas.microsoft.com/mapi/id/{00062004-0000-0000-C000-000000000046}/8094001f"""
    Public Shared ReadOnly Property EMail3Address As String = """http://schemas.microsoft.com/mapi/id/{00062004-0000-0000-C000-000000000046}/80a4001f"""

End Structure

''' <summary>
''' Alle relevanten Namensfelder aus Outlook Kontakten.
''' </summary>
Friend Structure OutlookContactNameFields
    ' Namen Felder
    Public Shared ReadOnly Property CompanyName As String = """urn:schemas:contacts:o"""
    Public Shared ReadOnly Property FullName As String = """http://schemas.microsoft.com/mapi/id/{00062004-0000-0000-C000-000000000046}/8005001f"""
    Public Shared ReadOnly Property FirstName As String = """urn:schemas:contacts:givenName"""
    Public Shared ReadOnly Property LastName As String = """urn:schemas:contacts:sn"""
    Public Shared ReadOnly Property MiddleName As String = """urn:schemas:contacts:middlename"""
    Public Shared ReadOnly Property NickName As String = """urn:schemas:contacts:nickname"""
End Structure

''' <summary>
''' Alle relevanten Telefonnummernfelder aus Outlook Kontakten.
''' </summary>
Friend Structure OutlookContactNumberFields
    ' Telefonnummernfelder
    Public Shared ReadOnly Property AssistantTelephoneNumber As String = """urn:schemas:contacts:secretaryphone"""
    Public Shared ReadOnly Property BusinessTelephoneNumber As String = """urn:schemas:contacts:officetelephonenumber"""
    Public Shared ReadOnly Property Business2TelephoneNumber As String = """urn:schemas:contacts:office2telephonenumber"""
    Public Shared ReadOnly Property CallbackTelephoneNumber As String = """urn:schemas:contacts:callbackphone"""
    Public Shared ReadOnly Property CarTelephoneNumber As String = """urn:schemas:contacts:othermobile"""
    Public Shared ReadOnly Property CompanyMainTelephoneNumber As String = """urn:schemas:contacts:organizationmainphone"""
    Public Shared ReadOnly Property HomeTelephoneNumber As String = """urn:schemas:contacts:homePhone"""
    Public Shared ReadOnly Property Home2TelephoneNumber As String = """urn:schemas:contacts:homephone2"""
    Public Shared ReadOnly Property ISDNNumber As String = """urn:schemas:contacts:internationalisdnnumber"""
    Public Shared ReadOnly Property MobileTelephoneNumber As String = """http://schemas.microsoft.com/mapi/proptag/0x3a1c001f"""
    Public Shared ReadOnly Property OtherTelephoneNumber As String = """urn:schemas:contacts:otherTelephone"""
    Public Shared ReadOnly Property PagerNumber As String = """urn:schemas:contacts:pager"""
    Public Shared ReadOnly Property PrimaryTelephoneNumber As String = """http://schemas.microsoft.com/mapi/proptag/0x3a1a001f"""
    Public Shared ReadOnly Property RadioTelephoneNumber As String = """http://schemas.microsoft.com/mapi/proptag/0x3a1d001f"""
    Public Shared ReadOnly Property BusinessFaxNumber As String = """urn:schemas:contacts:facsimiletelephonenumber"""
    Public Shared ReadOnly Property HomeFaxNumber As String = """urn:schemas:contacts:homefax"""
    Public Shared ReadOnly Property OtherFaxNumber As String = """urn:schemas:contacts:otherfax"""
    Public Shared ReadOnly Property Telex As String = """urn:schemas:contacts:telexnumber""" ' Eigentlich TelexNumber
    Public Shared ReadOnly Property TTYTDDTelephoneNumber As String = """urn:schemas:contacts:ttytddphone"""

End Structure

