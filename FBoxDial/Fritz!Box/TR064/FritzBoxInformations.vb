Imports System.Collections

Public Module FritzBoxInformations

#Region "Lokale Variablen"
    Friend ErrorHashTable As Hashtable
#End Region

#Region "Properties"
    Friend ReadOnly Property TR064ContentType As String = "text/xml; charset=""utf-8"""
    Friend ReadOnly Property TR064UserAgent As String = "AVM UPnP/1.0 Client 1.0"

#End Region

#Region "Fritz!Box UPnP/TR-064 Files"

    ''' <summary>
    ''' Gibt die SCPDURL der bekannten Services zurück.
    ''' </summary>
    Public Structure Tr064Files
        ''' <summary>
        ''' deviceconfigSCPD
        ''' </summary>
        Shared deviceconfigSCPD As String = "/deviceconfigSCPD.xml"

        ''' <summary>
        ''' deviceinfoSCPD
        ''' </summary>
        Shared deviceinfoSCPD As String = "/deviceinfoSCPD.xml"

        ''' <summary>
        ''' lanconfigsecuritySCPD
        ''' </summary>
        Shared lanconfigsecuritySCPD As String = "/lanconfigsecuritySCPD.xml"

        ''' <summary>
        ''' lanhostconfigmgmSCPD
        ''' </summary>
        Shared lanhostconfigmgmSCPD As String = "/lanhostconfigmgmSCPD.xml"

        ''' <summary>
        ''' layer3forwardingSCPD
        ''' </summary>
        Shared layer3forwardingSCPD As String = "/layer3forwardingSCPD.xml"

        ''' <summary>
        ''' mgmsrvSCPD
        ''' </summary>
        Shared mgmsrvSCPD As String = "/mgmsrvSCPD.xml"

        ''' <summary>
        ''' timeSCPD
        ''' </summary>
        Shared timeSCPD As String = "/timeSCPD.xml"

        ''' <summary>
        ''' timeSCPDuserifSCPD
        ''' </summary>
        Shared userifSCPD As String = "/userifSCPD.xml"

        ''' <summary>
        ''' wancommonifconfigSCPD
        ''' </summary>
        Shared wancommonifconfigSCPD As String = "/wancommonifconfigSCPD.xml"

        ''' <summary>
        ''' wandslifconfigSCPD
        ''' </summary>
        Shared wandslifconfigSCPD As String = "/wandslifconfigSCPD.xml"

        ''' <summary>
        ''' wandsllinkconfigSCPD
        ''' </summary>
        Shared wandsllinkconfigSCPD As String = "/wandsllinkconfigSCPD.xml"

        ''' <summary>
        ''' wanpppconnSCPD
        ''' </summary>
        Shared wanpppconnSCPD As String = "/wanpppconnSCPD.xml"

        ''' <summary>
        ''' wanipconnSCPD
        ''' </summary>
        Shared wanipconnSCPD As String = "/wanipconnSCPD.xml"

        ''' <summary>
        ''' wlanconfigSCPD
        ''' </summary>
        Shared wlanconfigSCPD As String = "/wlanconfigSCPD.xml"

        ''' <summary>
        ''' hostsSCPD
        ''' </summary>
        Shared hostsSCPD As String = "/hostsSCPD.xml"

        ''' <summary>
        ''' lanifconfigSCPD
        ''' </summary>
        Shared lanifconfigSCPD As String = "/lanifconfigSCPD.xml"

        ''' <summary>
        ''' wanethlinkconfigSCPD
        ''' </summary>
        Shared wanethlinkconfigSCPD As String = "/wanethlinkconfigSCPD.xml"

        ''' <summary>
        ''' x_upnpSCPD
        ''' </summary>
        Shared x_upnpSCPD As String = "/x_upnpSCPD.xml"

        ''' <summary>
        ''' x_webdavSCPD
        ''' </summary>
        Shared x_contactSCPD As String = "/x_contactSCPD.xml"

        ''' <summary>
        ''' x_myfritzSCPD
        ''' </summary>
        Shared x_myfritzSCPD As String = "/x_myfritzSCPD.xml"

        ''' <summary>
        ''' x_storageSCPD
        ''' </summary>
        Shared x_storageSCPD As String = "/x_storageSCPD.xml"

        ''' <summary>
        ''' x_remoteSCPD
        ''' </summary>
        Shared x_remoteSCPD As String = "/x_remoteSCPD.xml"

        ''' <summary>
        ''' x_tamSCPD
        ''' </summary>
        Shared x_tamSCPD As String = "/x_tamSCPD.xml"

        ''' <summary>
        ''' x_voipSCPD
        ''' </summary>
        Shared x_voipSCPD As String = "/x_voipSCPD.xml"

        ''' <summary>
        ''' x_webdavSCPD
        ''' </summary>
        Shared x_webdavSCPD As String = "/x_webdavSCPD.xml"

        ''' <summary>
        ''' X_appsetup
        ''' </summary>
        Shared X_appsetup As String = "/x_appsetupSCPD.xml"

        ''' <summary>
        ''' X_homeautoSCPD
        ''' </summary>
        Shared X_homeautoSCPD As String = "/x_homeautoSCPD.xml"

        ''' <summary>
        ''' X_homeplugSCPD
        ''' </summary>
        Shared X_homeplugSCPD As String = "/x_homeplugSCPD.xml"

        ''' <summary>
        ''' igddesc
        ''' </summary>
        Shared igddesc As String = "/igddesc.xml"

        ''' <summary>
        ''' any
        ''' </summary>
        Shared any As String = "/any.xml"

        ''' <summary>
        ''' x_webdavSCPD
        ''' </summary>
        Shared igdicfgSCPD As String = "/igdicfgSCPD.xml"

        ''' <summary>
        ''' x_webdavSCPD
        ''' </summary>
        Shared igddslSCPD As String = "/igddslSCPD.xml"

        ''' <summary>
        ''' igdconnSCPD
        ''' </summary>
        Shared igdconnSCPD As String = "/igdconnSCPD.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared tr64desc As String = "/tr64desc.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared x_speedtestSCPD As String = "/x_speedtestSCPD.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared x_dectSCPD As String = "/x_dectSCPD.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared x_filelinksSCPD As String = "/x_filelinksSCPD.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared x_authSCPD As String = "/x_authSCPD.xml"
    End Structure

#End Region
End Module