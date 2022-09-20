''' <summary>
''' API操作
''' </summary>
''' <remarks></remarks>
Public Class API
    Inherits Base

    Protected m_NetStream As IO.Stream = Nothing
    Protected m_TcpClient As System.Net.Sockets.TcpClient = Nothing

    Public Sub New(ByVal session As Session)
        ' 检查会话状态
        MyBase.New(session)
        Try
            m_TcpClient = New Net.Sockets.TcpClient()
            m_TcpClient.Connect(System.Net.Dns.GetHostAddresses(m_Session.HostNameOrIPAddress), m_Session.Port)
            m_NetStream = m_TcpClient.GetStream()

        Catch ex As Exception
            Throw
        End Try

        '执行登陆
        If (Not Login()) Then
            Throw New RosException("登陆失败！检查账号或者密码是否正确！")
        End If

    End Sub

    Public Sub Close()
        m_NetStream.Close()
        m_TcpClient.Close()
    End Sub


    ''' <summary>
    ''' 执行命令
    ''' </summary>
    ''' <param name="command">命令主体，如 /ip address print</param>
    ''' <param name="attribute">命令属性集合。例如针对 /ip address add 命令需要设置的address,network,interface等</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Execute(ByVal command As String, ByVal attribute As Dictionary(Of String, String))

        If (command.Contains("=")) Then
            Throw New ApplicationException("检测到命令中包含参数设置。请将参数以Key-Value的形式用arrtibute参数传递。")
        End If

        Dim formattedCMD As String = command.Trim().Replace(" ", "/")
        Dim tag As String = ".tag=sss"
        Me.Send(formattedCMD)
        If (Not attribute Is Nothing AndAlso attribute.Count > 0) Then
            Dim keys As Dictionary(Of String, String).KeyCollection = attribute.Keys
            For Each m As String In keys
                Me.Send("=" & m & "=" & attribute(m))
            Next
        End If

        Me.Send(tag, True)


        Dim listGet As List(Of String) = Me.Read()
        Dim listRe As List(Of String) = New List(Of String)

        ' 对返回的字符串做基本处理
        If (Not listGet Is Nothing) AndAlso (listGet.Count > 0) Then
            Dim tmp As String = String.Empty
            For m As Int32 = 0 To listGet.Count - 1
                tmp = listGet(m).Replace(tag, String.Empty)
                listRe.Add(tmp)
            Next
        End If

        Return listRe
    End Function

    Public Function Execute(ByVal command As String)

        Return Execute(command, Nothing)

    End Function

    Private Function Login() As Boolean
        Send("/login", True)

        Dim hash As String = Read()(0).Split(New String() {"=ret="}, StringSplitOptions.None)(1)
        Send("/login")
        Send("=name=" & m_Session.UserName)
        Send("=response=00" & EncodePassword(m_Session.Password, hash), True)

        Dim res As List(Of String) = Read()
        If (res(0) = "!done") Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub Send(ByVal command As String, Optional ByVal EndSentence As Boolean = False)

        Dim _bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray())
        Dim _size As Byte() = EncodeLength(_bytes.Length)

        m_NetStream.Write(_size, 0, _size.Length)
        m_NetStream.Write(_bytes, 0, _bytes.Length)
        If EndSentence Then m_NetStream.WriteByte(0)

    End Sub


    Private Function Read() As List(Of String)
        Dim _outString As New List(Of String)
        Dim _tmpString = ""
        Dim _tmpByteArray(4) As Byte
        Dim _count As Long

        While True
            _tmpByteArray(3) = m_NetStream.ReadByte()
            Select Case _tmpByteArray(3)
                Case 0
                    _outString.Add(_tmpString)
                    If (_tmpString.Substring(0, 5) = "!done") Then
                        Exit While
                    Else
                        _tmpString = ""
                        Continue While
                    End If
                Case Is < &H80
                    _count = _tmpByteArray(3)
                Case Is < &HC0
                    _count = BitConverter.ToInt64(New Byte() {m_NetStream.ReadByte(), _tmpByteArray(3), 0, 0}, 0) ^ &H8000
                Case Is < &HE0
                    _tmpByteArray(2) = m_NetStream.ReadByte()
                    _count = BitConverter.ToInt64(New Byte() {m_NetStream.ReadByte(), _tmpByteArray(2), _tmpByteArray(3), 0}, 0) ^ &HC00000
                Case Is < &HF0
                    _tmpByteArray(2) = m_NetStream.ReadByte()
                    _tmpByteArray(1) = m_NetStream.ReadByte()
                    _count = BitConverter.ToInt64(New Byte() {m_NetStream.ReadByte(), _tmpByteArray(1), _tmpByteArray(2), _tmpByteArray(3)}, 0) ^ &HE0000000
                Case &HF0
                    _tmpByteArray(3) = m_NetStream.ReadByte()
                    _tmpByteArray(2) = m_NetStream.ReadByte()
                    _tmpByteArray(1) = m_NetStream.ReadByte()
                    _tmpByteArray(0) = m_NetStream.ReadByte()
                    _count = BitConverter.ToInt64(_tmpByteArray, 0)
                Case Else
                    Exit While
            End Select

            For i As Int32 = 0 To _count - 1
                _tmpString += ChrW(m_NetStream.ReadByte())
            Next
        End While

        Return _outString
    End Function

    Private Function EncodeLength(ByVal len As Int32) As Byte()

        Dim tmp As Byte() = Nothing
        If len < &H80 Then
            tmp = BitConverter.GetBytes(len)
            Return New Byte() {tmp(0)}
        ElseIf len < &H4000 Then
            tmp = BitConverter.GetBytes(len Or &H8000)
            Return New Byte() {tmp(1), tmp(0)}
        ElseIf len < &H200000 Then
            tmp = BitConverter.GetBytes(len Or &HC00000)
            Return New Byte() {tmp(2), tmp(1), tmp(0)}
        ElseIf len < &H10000000 Then
            tmp = BitConverter.GetBytes(len Or &HE0000000)
            Return New Byte() {tmp(3), tmp(2), tmp(1), tmp(0)}
        Else
            tmp = BitConverter.GetBytes(len)
            Return New Byte() {&HF0, tmp(3), tmp(2), tmp(1), tmp(0)}
        End If


    End Function

    ''' <summary>
    ''' 编码密码
    ''' </summary>
    ''' <param name="password"></param>
    ''' <param name="hash"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function EncodePassword(ByVal password As String, ByVal hash As String)

        Dim hash_byte As Byte() = New Byte(hash.Length / 2 - 1) {}

        For i As Int32 = 0 To hash.Length - 2 Step 2
            hash_byte(i / 2) = Byte.Parse(hash.Substring(i, 2), System.Globalization.NumberStyles.HexNumber)
        Next

        Dim helso As Byte() = New Byte(password.Length + hash_byte.Length) {}
        helso(0) = 0
        System.Text.Encoding.ASCII.GetBytes(password.ToCharArray()).CopyTo(helso, 1)
        hash_byte.CopyTo(helso, 1 + password.Length)

        Dim hotovo As Byte()
        Dim md5 As System.Security.Cryptography.MD5
        md5 = New System.Security.Cryptography.MD5CryptoServiceProvider()
        hotovo = md5.ComputeHash(helso)

        Dim navrat As String = String.Empty
        For Each h As Byte In hotovo
            navrat += h.ToString("x2")
        Next

        Return navrat

    End Function


End Class
