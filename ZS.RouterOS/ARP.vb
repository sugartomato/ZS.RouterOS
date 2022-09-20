''' <summary>
''' ARP对象
''' </summary>
Public Class ARP

    ''' <summary>
    ''' 构造
    ''' </summary>
    Public Sub New()
        Me.Disabled = New _Disabled(False)
    End Sub

    ''' <summary>
    ''' 使用指定参数构造一个新的ARP绑定对象
    ''' </summary>
    ''' <param name="_address">IP地址</param>
    ''' <param name="_macAddress">物理地址</param>
    ''' <param name="_Interface">网卡接口名称</param>
    ''' <param name="_comment">备注</param>
    ''' <param name="_disabled">是否禁用</param>
    Public Sub New(ByVal _address As String,
                           ByVal _macAddress As String,
                           ByVal _Interface As String,
                           ByVal _comment As String,
                           ByVal _disabled As Boolean)

        Me.Address = New _IPAddress(_address)
        Me.MacAddress = New _MACAddress(_macAddress)
        Me.InterfaceName = _Interface
        Me.Comment = _comment
        Me.Disabled = New _Disabled(_disabled)
    End Sub

    ''' <summary>设备IP地址</summary>
    Public Property Address As _IPAddress
    ''' <summary>设备物理地址</summary>
    Public Property MacAddress As _MACAddress
    ''' <summary>网卡接口名称</summary>
    Public Property InterfaceName As String
    ''' <summary>是否禁用</summary>
    Public Property Disabled As _Disabled
    ''' <summary>备注</summary>
    Public Property Comment As String



#Region "静态方法"

    ''' <summary>
    ''' 获取添加ARP的脚本
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    Public Shared Function ARP_ScriptText_Add(ByVal model As ARP) As String
        If (model Is Nothing) Then
            Return String.Empty
        End If

        Dim s As String = vbNewLine
        s &= "/ip arp add"
        s &= " address=" & model.Address.ToString()
        s &= " mac-address=" & model.MacAddress.ToString()
        s &= " interface=" & model.InterfaceName
        s &= " comment=" & model.Comment
        s &= " disabled=" & model.Disabled.ToString()

        Return s
    End Function

    ''' <summary>
    ''' 根据IP地址生成删除脚本
    ''' </summary>
    ''' <param name="ip"></param>
    ''' <returns></returns>
    Public Shared Function ScriptText_RemoveByIPAddress(ByVal ip As String) As String
        Dim objIP As New _IPAddress(ip)
        Return vbNewLine & "/ip arp  {:foreach i in=[find address=" & objIP.ToString() & "] do={remove $i}}"

    End Function

    ''' <summary>
    ''' 根据MAC地址生成删除脚本
    ''' </summary>
    ''' <param name="mac"></param>
    ''' <returns></returns>
    Public Shared Function ScriptText_RemoveByMacAddress(ByVal mac As String) As String
        Dim objMac As New _MACAddress(mac)
        Return vbNewLine & "/ip arp  {:foreach i in=[find mac-address=" & objMac.ToString() & "] do={remove $i}}"
    End Function


#End Region



End Class
