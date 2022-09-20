Imports System
Imports System.Reflection

Namespace ROSObjects.IP

    <RObj(BaseCommand:="/ip address")>
    Public Class Address

        <RObj(ROSName:="ip")>
        Public Property Address As _IPAddress


    End Class


End Namespace

