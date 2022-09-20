''' <summary>
''' 脚本生成
''' </summary>
''' <remarks></remarks>
Public Class ScriptHelper

    ''' <summary>
    ''' DHCP脚本
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DHCP

        ''' <summary>
        ''' 根据MAC地址创建HDCP Lease的删除脚本
        ''' </summary>
        ''' <param name="mac">MAC地址</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Lease_RemoveByMacAddress(ByVal mac As String) As String

            mac = mac.Replace("-", ":")
            Return vbNewLine & "/ip dhcp-server lease {:foreach i in=[find mac-address=" & mac & "] do={remove $i}}"

        End Function


    End Class

    ''' <summary>
    ''' ARP脚本
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ARP


        ''' <summary>
        ''' 根据MAC地址创建arp的删除脚本
        ''' </summary>
        ''' <param name="mac">MAC地址</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RemoveByMacAddress(ByVal mac As String) As String

            mac = mac.Replace("-", ":")
            Return vbNewLine & "/ip arp  {:foreach i in=[find mac-address=" & mac & "] do={remove $i}}"

        End Function

    End Class

    ''' <summary>
    ''' 热点
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Hotspot

        ''' <summary>
        ''' 根据MAC地址创建/ip hotspot ip-binding的删除脚本
        ''' </summary>
        ''' <param name="mac">MAC地址</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IPBinding_RemoveByMacAddress(ByVal mac As String) As String

            mac = mac.Replace("-", ":")
            Return vbNewLine & "/ip hotspot ip-binding {:foreach i in=[find mac-address=" & mac & "] do={remove $i}}"

        End Function

    End Class


End Class
