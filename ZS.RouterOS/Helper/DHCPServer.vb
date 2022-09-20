Namespace Helper

    <Obsolete("使用HDCP命名空间下的方法代替此类")>
    Public Class DHCPServer


#Region "DHCP-绑定"

        ''' <summary>
        ''' DHCP绑定模型
        ''' </summary>
        Public Class Leases_Model

            Public Sub New()
                Me.Server = "all"
                Me.LeaseTime = String.Empty
                Me.Disabled = New _Disabled(False)
                Me.AlwaysBroadcast = New _Disabled(True)
            End Sub

            ''' <summary>
            ''' 使用指定参数构造一个新的DHCP绑定对象
            ''' </summary>
            ''' <param name="_address">IP地址</param>
            ''' <param name="_macAddress">MAC地址</param>
            ''' <param name="_server">DHCP服务器名称</param>
            ''' <param name="_comment">备注</param>
            Public Sub New(ByVal _address As String,
                           ByVal _macAddress As String,
                           ByVal _server As String,
                           ByVal _comment As String)
                Me.Server = _server
                Me.MacAddress = New _MACAddress(_macAddress)
                Me.Address = New _IPAddress(_address)
                Me.Comment = _comment
            End Sub

            ''' <summary>IP地址</summary>
            Public Property Address As _IPAddress
            ''' <summary>MAC抵制</summary>
            Public Property MacAddress As _MACAddress
            ''' <summary>DHCP服务器。注意服务器名称必须要大小写完全匹配。默认为all</summary>
            Public Property Server As String
            ''' <summary>
            ''' 租约时间。格式为HH:mm:ss。默认为空，即按照Server的租约时间处理。
            ''' </summary>
            Public Property LeaseTime As String
            ''' <summary>
            ''' 是否禁用。默认为False
            ''' </summary>
            Public Property Disabled As _Disabled
            ''' <summary>
            ''' 是否始终广播。默认为True
            ''' </summary>
            Public Property AlwaysBroadcast As _Disabled
            ''' <summary>备注</summary>
            Public Property Comment As String

            ''' <summary>
            ''' 将dhcp对象转化为添加DHCP的脚本程序
            ''' </summary>
            ''' <returns></returns>
            Public Function ToAddScriptString()
                Dim script As String = String.Empty

                script += vbCrLf & "#RemoveScript"
                ' 删除DHCP绑定
                script &= vbCrLf & Leases_ScriptText_RemoveByMACAddress(Me.MacAddress.ToString())

                ' 删除ARP绑定
                script &= vbCrLf & ARP.ARP_ScriptText_RemoveByMacAddress(Me.MacAddress.ToString())


                script &= vbCrLf & "#" & Comment
                script &= vbCrLf & "#" & "=".PadLeft(20, "=")
                ' 添加DHCP绑定
                script &= vbCrLf & "#AddScript - DHCP"
                script &= vbCrLf & String.Format(
                    "/ip dhcp-server lease add address={0} mac-address={1} disabled=no always-broadcast=yes server={2}", Me.Address.ToString(), Me.MacAddress.ToString(), Me.Server)

                If Not Me.Comment Is Nothing AndAlso Me.Comment.Length > 0 Then
                    script &= " comment=""" & Me.Comment.ToHexString() & """"
                End If

                ' 添加ARP绑定
                script &= vbCrLf & "#AddScript - ARP"
                script &= vbCrLf & String.Format("/ip arp add address={0} mac-address={1} interface=LAN", Me.Address.ToString(), Me.MacAddress.ToString())

                script &= vbCrLf & "#" & "=".PadLeft(20, "=")

                Return script

            End Function

        End Class

        ''' <summary>
        ''' 显示当前的静态DHCP列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Leases_Print() As List(Of String)
            Dim ListRe As List(Of String) = Config.API.Execute("/ip dhcp-server lease print", Nothing)
            Return ListRe
        End Function

        ''' <summary>
        ''' 添加1条DHCP绑定
        ''' </summary>
        ''' <param name="obj">绑定对象</param>
        ''' <param name="errMessage">执行消息</param>
        ''' <returns></returns>
        Public Function Leases_Add(ByVal obj As Leases_Model, ByRef errMessage As String) As Boolean
            If (obj Is Nothing) Then
                errMessage = "未指定的DHCPLeases对象"
                Return False
            End If

            ' 检查必要的设置

            Dim att As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            att.Add("address", obj.Address.ToString())
            att.Add("mac-address", obj.MacAddress.ToString())
            att.Add("server", obj.Server)
            att.Add("always-broadcast", obj.AlwaysBroadcast.ToString())
            att.Add("disabled", obj.Disabled.ToString())
            att.Add("comment", obj.Comment)
            Dim ListRe As List(Of String) = Config.API.Execute("/ip dhcp-server lease add", att)

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
        ''' 移除所有动态分配的地址
        ''' </summary>
        ''' <returns></returns>
        Public Function Leases_RemoveAllDynamic(ByRef exMessage As String) As Boolean
            Dim _scriptContent As String = ":foreach i in=[/ip dhcp-server lease find dynamic=yes] do=[/ip dhcp-server lease remove $i]"
            Dim _scriptName As String = System.Guid.NewGuid().ToString("N").ToUpper()
            Dim _scriptHelper As Script = New Script()
            Dim _scriptModel As Script_Model = New Script_Model()

            _scriptModel.Name = _scriptName
            _scriptModel.Source = _scriptContent

            ' 创建脚本
            If (Not _scriptHelper.Add(_scriptModel, exMessage)) Then
                Return False
            End If

            ' 执行脚本
            If (Not _scriptHelper.RunScript(_scriptName, exMessage)) Then
                Return False
            End If

            ' 删除脚本
            If (Not _scriptHelper.Remove(_scriptName, exMessage)) Then


            End If

            Return True
        End Function


#Region "脚本文件"

        ''' <summary>
        ''' 根据IP地址删除DHCP绑定的脚本。包含静态绑定和动态绑定。
        ''' </summary>
        ''' <param name="ip"></param>
        ''' <returns></returns>
        Public Shared Function Leases_ScriptText_RemoveByIPAddress(ByVal ip As String) As String
            Dim i As _IPAddress = New _IPAddress(ip)
            Dim s As String = String.Empty
            s = vbNewLine & "/ip dhcp-server lease {:foreach i in=[find address=" & i.ToString() & "] do={remove $i}}"
            s += vbNewLine & "/ip dhcp-server lease {:foreach i in=[find active-address=" & i.ToString() & "] do={remove $i}}"
            Return s
        End Function

        ''' <summary>
        ''' 根据MAC地址删除DHCP绑定的脚本。包含静态的和动态绑定的。
        ''' </summary>
        ''' <param name="mac">MAC地址</param>
        ''' <returns></returns>
        Public Shared Function Leases_ScriptText_RemoveByMACAddress(ByVal mac As String) As String
            Dim m As _MACAddress = New _MACAddress(mac)
            Dim s As String = String.Empty
            s = vbNewLine & "/ip dhcp-server lease {:foreach i in=[find mac-address=" & m.ToString() & "] do={remove $i}}"
            s += vbNewLine & "/ip dhcp-server lease {:foreach i in=[find active-mac-address=" & m.ToString() & "] do={remove $i}}"
            Return s
        End Function

        ''' <summary>
        ''' 根据DHCP绑定对象创建绑定添加的脚本
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        Public Shared Function Leases_ScriptText_Add(ByVal model As Leases_Model) As String
            If (model Is Nothing) Then
                Return String.Empty
            End If

            Dim s As String = vbNewLine
            s &= "/ip dhcp-server lease add"
            s &= " address=" & model.Address.ToString()
            s &= " mac-address=" & model.MacAddress.ToString()
            s &= " disabled=" & model.Disabled.ToString()
            s &= " server=" & model.Server
            s &= " always-broadcast=" & model.AlwaysBroadcast.ToString()
            s &= " comment=" & model.Comment

            Return s
        End Function

#End Region




#End Region

    End Class


End Namespace
