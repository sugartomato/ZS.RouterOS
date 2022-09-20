
Namespace Helper

    Public Class Hotspot

        Public Sub New()
            Config.CheckConfig()
        End Sub

#Region "IP绑定"


        ''' <summary>
        ''' 显示当前的IP绑定列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IPBindings_Print() As List(Of String)
            Dim ListRe As List(Of String) = Config.API.Execute("/ip hotspot ip-binding print", Nothing)
            Return ListRe
        End Function


        ''' <summary>
        ''' 添加一个热点绑定
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="errMessage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IPBindings_Add(ByVal obj As IPBindings_Model, ByRef errMessage As String) As Boolean
            If (obj Is Nothing) Then
                Return False
            End If

            Dim att As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            If (Not obj.Address Is Nothing) Then att.Add("address", obj.Address.ToString())
            If (Not obj.MACAddress Is Nothing) Then att.Add("mac-address", obj.MACAddress.ToString())
            If (Not obj.ToAddress Is Nothing) Then att.Add("to-address", obj.ToAddress.ToString())

            att.Add("server", obj.Server)
            att.Add("type", GetTypeString(obj.Type))
            att.Add("disabled", obj.Disabled.ToString())

            Dim ListRe As List(Of String) = Config.API.Execute("/ip hotspot ip-binding add", att)

            If (Not ListRe Is Nothing AndAlso ListRe.Count > 0) Then
                ' 执行成功的消息示例： "!done=ret=*F558.tag=sss"
                If (ListRe(0).StartsWith("!done=ret=")) Then
                    Return True
                Else
                    If (ListRe(0).StartsWith("!trap")) Then
                        If (ListRe(0).IndexOf("=message=")) > 0 Then
                            errMessage = ListRe(0).Split(New String() {"=message="}, StringSplitOptions.None)(1)
                        Else
                            errMessage = ListRe(0)
                        End If
                    Else
                        errMessage = ListRe(0)
                    End If
                End If
            Else
                errMessage = "未知的错误。未获取任何执行结果的消息！"
            End If

            Return False
        End Function


        ''' <summary>
        ''' 删除指定服务下的IP绑定。如果不指定服务名称，则是删除所有服务的绑定数据。
        ''' </summary>
        ''' <param name="server">热点服务名称</param>
        ''' <returns></returns>
        Public Function IPBindings_RemoveByServer(ByVal server As String, ByRef exMessage As String) As Boolean
            Dim scriptName As String = System.Guid.NewGuid().ToString("N").ToUpper()
            Dim scriptHelper As Script = New Script()
            Dim scriptModel As Script_Model = New Script_Model()
            Dim _server = String.Empty
            If (Not String.IsNullOrEmpty(server)) Then
                _server = " server=" & server
            End If
            scriptModel.Name = scriptName
            scriptModel.Source = ":foreach i in=[/ip hotspot ip-binding find" & _server & "] do=[/ip hotspot ip-binding remove $i]"

            ' 创建脚本
            If (Not scriptHelper.Add(scriptModel, exMessage)) Then
                Return False
            End If

            ' 执行脚本
            If (Not scriptHelper.RunScript(scriptName, exMessage)) Then
                Return False
            End If

            ' 删除脚本
            If (Not scriptHelper.Remove(scriptName, exMessage)) Then


            End If

            Return True
        End Function

        ''' <summary>
        ''' 检查指定的IP地址是否存在于IP绑定列表中。
        ''' </summary>
        ''' <param name="ip"></param>
        ''' <returns></returns>
        Public Function IPBindings_IsIpExists(ByVal ip As _IPAddress, ByRef errMessage As String) As Boolean

            If (ip Is Nothing) Then
                Throw New ArgumentNullException("ip", "未设定该值！")
            End If

            Dim att As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            'att.Add("address", ip.ToString())
            att.Add("server", "SKZX")

            Dim ListRe As List(Of String) = Config.API.Execute(" /ip hotspot ip-binding find ", Nothing)

            If (Not ListRe Is Nothing AndAlso ListRe.Count > 0) Then
                ' 执行成功的消息示例： "!done=ret=*F558.tag=sss"
                If (ListRe(0).StartsWith("!done=ret=")) Then
                    Return True
                Else
                    If (ListRe(0).StartsWith("!trap")) Then
                        If (ListRe(0).IndexOf("=message=")) > 0 Then
                            errMessage = ListRe(0).Split(New String() {"=message="}, StringSplitOptions.None)(1)
                        Else
                            errMessage = ListRe(0)
                        End If
                    Else
                        errMessage = ListRe(0)
                    End If
                End If
            Else
                errMessage = "未知的错误。未获取任何执行结果的消息！"
            End If

            Return False

        End Function

        Private Function GetTypeString(ByVal t As IPBindings_Model.BindingType) As String
            Select Case t
                Case IPBindings_Model.BindingType.Blocked
                    Return "blocked"
                Case IPBindings_Model.BindingType.ByPassed
                    Return "bypassed"
                Case IPBindings_Model.BindingType.Regular
                    Return "regular"
                Case Else
                    Return "regular"
            End Select
        End Function

        ''' <summary>
        ''' IP绑定模型
        ''' </summary>
        Public Class IPBindings_Model

            Public Sub New()
                Me.Disabled = New _Disabled(False)
            End Sub

            ''' <summary>MAC地址</summary>
            Public Property MACAddress As _MACAddress
            ''' <summary>IP地址范围起始IP</summary>
            Public Property Address As _IPAddress
            ''' <summary>IP地址范围截至IP</summary>
            Public Property ToAddress As _IPAddress
            ''' <summary>热点服务名称。名称是区分大小写的，所以必须与ROS中的名称完全一致</summary>
            Public Property Server As String
            ''' <summary>网络访问授权类型</summary>
            Public Property Type As BindingType
            ''' <summary>是否禁用。默认为否</summary>
            Public Property Disabled As _Disabled

            ''' <summary>访问限制类型</summary>
            Public Enum BindingType
                Regular
                ''' <summary>阻止访问</summary>
                Blocked
                ''' <summary>授权访问</summary>
                ByPassed
            End Enum

        End Class


    End Class


#End Region





End Namespace
