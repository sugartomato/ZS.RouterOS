''' <summary>
''' 会话信息
''' </summary>
Public Class Session

    ''' <summary>
    ''' 登陆账号
    ''' </summary>
    Public Property UserName As String

    ''' <summary>
    ''' 登陆密码
    ''' </summary>
    Public Property Password As String

    ''' <summary>
    ''' 主机域名或者地址
    ''' </summary>
    Public Property HostNameOrIPAddress As String


    Private _propPort As Int32 = 8728
    ''' <summary>
    ''' 端口。默认端口为8728
    ''' </summary>
    Public Property Port As Int32
        Get
            Return _propPort
        End Get
        Set(value As Int32)
            _propPort = value
        End Set
    End Property

    ''' <summary>
    ''' 路由版本号
    ''' </summary>
    Public Property Version As String

End Class
