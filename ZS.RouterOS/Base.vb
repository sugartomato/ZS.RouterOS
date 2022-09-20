Partial Public MustInherit Class Base

    ''' <summary>会话数据</summary>
    Protected m_Session As ZS.RouterOS.Session = Nothing

    Public Sub New(ByVal session As ZS.RouterOS.Session)
        If (CheckSession(session)) Then
            m_Session = session
        End If
    End Sub

    Private Function CheckSession(ByVal session As ZS.RouterOS.Session)
        If (session Is Nothing) Then
            Throw New ApplicationException("未指定Session对象！")
        End If

        Return True
    End Function


End Class
