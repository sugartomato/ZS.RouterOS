''' <summary>
''' MAC地址类型
''' </summary>
Public Class _MACAddress

    Private m_MACAddress As String = String.Empty
    Public Sub New(ByVal macAddress As String)

        macAddress = macAddress.Replace("-", ":")
        macAddress = macAddress.Trim()
        If (Not System.Text.RegularExpressions.Regex.IsMatch(macAddress, "^([\da-fA-F]{2})(([:]{1}[\da-fA-F]{2}){5})$")) Then
            Throw New RosException("无效的MAC地址！")
        End If

        m_MACAddress = macAddress
    End Sub

    ''' <summary>
    ''' 输出该类型的ROS值。即:分隔的MAC地址表示
    ''' </summary>
    Public Overrides Function ToString() As String
        Return m_MACAddress
    End Function

End Class

''' <summary>
''' IP地址定义
''' </summary>
Public Class _IPAddress
    Private m_IPAddress As String = String.Empty

    Public Sub New(ByVal address As String)
        m_IPAddress = address
    End Sub

    ''' <summary>
    ''' 输出该类型的ROS值。
    ''' </summary>
    Public Overrides Function ToString() As String
        Return m_IPAddress
    End Function

End Class

''' <summary>
''' 禁用属性定义
''' </summary>
Public Class _Disabled
    Private m_Disabled As Boolean = False

    Public Sub New(ByVal disabled As Boolean)
        m_Disabled = disabled
    End Sub

    ''' <summary>
    ''' 输出该类型的ROS值。True输出为yes，False输出为no
    ''' </summary>
    Public Overrides Function ToString() As String
        If m_Disabled Then
            Return "yes"
        Else
            Return "no"
        End If
    End Function

End Class

''' <summary>
''' 注释
''' </summary>
Public Class _Comment

    Private m_Comment As String = String.Empty
    Public Sub New(ByVal comment As String)
        m_Comment = comment
    End Sub

    Public Overrides Function ToString() As String
        Return m_Comment
    End Function

End Class
