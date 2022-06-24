Imports System.Net.Http
Imports System.Reflection
Imports System.Text.RegularExpressions

''' <summary>
''' The HTTP WWW-Authenticate response header defines the HTTP authentication methods ("challenges") that might be used to gain access to a specific resource.<br/>
''' <see href="link">https://datatracker.ietf.org/doc/html/rfc7616</see><br/>
''' <see href="link">https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/WWW-Authenticate</see> 
''' </summary>
Friend Class WWWAuthenticatorHeader

    ''' <summary>
    ''' The Authentication scheme. Some of the more common types are (case-insensitive): Basic, Digest, Negotiate and AWS4-HMAC-SHA256.
    ''' </summary>
    Public Property Scheme As String

    Public Property Parameter As String

    Public Property Realm As String

    Public Property Domain As String

    Public Property Nonce As String

    Public Property Opaque As String

    Public Property Stale As Boolean = False

    Public Property Algorithm As String = "MD5"

    Public Property QoP As String = "auth"

    Friend Property Userhash As Boolean = False

    Friend ReadOnly Property IsSessionAuth As Boolean
        Get
            Return Algorithm.EndsWith("-sess")
        End Get
    End Property

    Friend ReadOnly Property IsIntegrityProtection As Boolean
        Get
            Return QoP.EndsWith("-int")
        End Get
    End Property

    Public ReadOnly Property AlgorithmName As String = Algorithm.Replace("-sess", String.Empty)

    Friend ReadOnly Property GetClientResponseHeader(UserName As String, Response As String, ClientNonce As String, NonceCount As Integer, Uri As String) As String
        Get
            Return String.Join(", ", New List(Of String) From {$"username=""{UserName}""",
                                                               $"realm=""{Realm}""",
                                                               $"nonce=""{Nonce}""",
                                                               $"uri=""{Uri}""",
                                                               $"algorithm={Algorithm}",
                                                               $"qop={QoP}",
                                                               $"nc={NonceCount:00000000}",
                                                               $"cnonce=""{ClientNonce}""",
                                                               $"response=""{Response}""",
                                                               $"opaque=""{Opaque}""",
                                                               $"userhash={Userhash.ToString.ToLower}"})
        End Get
    End Property

    Sub New(Header As Headers.AuthenticationHeaderValue)
        With Header
            Scheme = .Scheme
            Parameter = .Parameter

            ' Schleife durch alle Properties dieser Klasse
            For Each PI As PropertyInfo In [GetType].GetProperties
                If PI.CanWrite Then
                    Dim tmpstr As String = GetChallengeValueFromHeader(PI.Name, .Parameter)
                    Select Case PI.Name
                        Case NameOf(Scheme), NameOf(Parameter)
                            ' Ignore
                        Case NameOf(Stale), NameOf(Userhash) ' Boolean

                            If tmpstr.IsStringNothingOrEmpty Then
                                PI.SetValue(Me, False)
                            Else
                                PI.SetValue(Me, CBool(tmpstr))
                            End If

                        Case Else

                            If tmpstr.IsNotStringNothingOrEmpty Then PI.SetValue(Me, GetChallengeValueFromHeader(PI.Name, .Parameter))

                    End Select
                End If
            Next
        End With
    End Sub

    Private Function GetChallengeValueFromHeader(challengeName As String, AuthenticateHeaderParameter As String) As String

        Dim Match As Match = Regex.Match(AuthenticateHeaderParameter, $"{challengeName}=([^,]*)", RegexOptions.IgnoreCase)
        With Match.Groups(1)
            ' Entferne die Anfürhungsstriche am Anfang und Ende
            Return Match.Groups(1).Value.RegExRemove("^""|""$")
        End With
    End Function

End Class
