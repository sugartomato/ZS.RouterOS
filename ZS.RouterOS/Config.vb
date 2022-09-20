
Friend Class Config

    ''' <summary>
    ''' 会话信息
    ''' </summary>
    ''' <returns></returns>
    Friend Shared Property Session As Session


    Friend Shared ReadOnly Property API As API
        Get
            If (Session Is Nothing) Then
                Throw New RosException("请先设置会话信息！")
            End If
            Return New API(Session)
        End Get
    End Property

    Friend Shared Sub CheckConfig()

        If (Session Is Nothing) Then
            Throw New RosException("未指定的会话信息！")
        End If

    End Sub

End Class
