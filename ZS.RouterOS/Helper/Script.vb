Namespace Helper

    Public Class Script
        Public Sub New()
            Config.CheckConfig()
        End Sub

        Public Function Print() As List(Of String)
            Dim ListRe As List(Of String) = Config.API.Execute("/system script print", Nothing)
            Return ListRe
        End Function

        ''' <summary>
        ''' 执行指定名称的脚本
        ''' </summary>
        ''' <param name="scriptName">脚本名称</param>
        ''' <returns></returns>
        Public Function RunScript(ByVal scriptName As String, ByRef errMessage As String) As Boolean
            Dim para As New Dictionary(Of String, String)
            para.Add(".id", scriptName)
            Dim ListRe As List(Of String) = Config.API.Execute("/system script run ", para)
            If (Not ListRe Is Nothing AndAlso ListRe.Count > 0) Then
                If (ListRe(0) = "!done") Then
                    Return True
                Else
                    errMessage = "脚本【" & scriptName & "】执行失败：" & ListRe(0)
                    Return False
                End If
            Else
                errMessage = "执行脚本失败，未获取任何路由信息！"
                Return False
            End If
        End Function

        ''' <summary>
        ''' 添加脚本
        ''' </summary>
        ''' <param name="m">脚本对象</param>
        ''' <param name="errMessage">执行消息</param>
        ''' <returns></returns>
        Public Function Add(ByVal m As Script_Model, ByRef errMessage As String) As Boolean
            Dim para As New Dictionary(Of String, String)
            para.Add("name", m.Name)
            para.Add("source", m.Source)

            Dim ListRe As List(Of String) = Config.API.Execute("/system script add ", para)
            If (Not ListRe Is Nothing AndAlso ListRe.Count > 0) Then
                If (ListRe(0).StartsWith("!done")) Then
                    Return True
                Else
                    errMessage = "脚本【" & m.Name & "】添加失败：" & ListRe(0)
                    Return False
                End If
            Else
                errMessage = "添加脚本失败，未获取任何路由信息！"
                Return False
            End If
        End Function

        ''' <summary>
        ''' 移除指定名称的脚本
        ''' </summary>
        ''' <param name="scriptName">脚本名称</param>
        ''' <param name="errMessage">执行消息</param>
        ''' <returns></returns>
        Public Function Remove(ByVal scriptName As String, ByRef errMessage As String) As Boolean
            Dim para As New Dictionary(Of String, String)
            para.Add(".id", scriptName)
            Dim ListRe As List(Of String) = Config.API.Execute("/system script remove ", para)
            If (Not ListRe Is Nothing AndAlso ListRe.Count > 0) Then
                If (ListRe(0).StartsWith("!done")) Then
                    Return True
                Else
                    errMessage = "脚本【" & scriptName & "】删除失败：" & ListRe(0)
                    Return False
                End If
            Else
                errMessage = "删除脚本失败，未获取任何路由信息！"
                Return False
            End If
        End Function

    End Class

    Public Class Script_Model
        ''' <summary>
        ''' 脚本名称
        ''' </summary>
        Public Property Name As String
        ''' <summary>
        ''' 脚本内容
        ''' </summary>
        Public Property Source As String

    End Class


End Namespace


