
Namespace ROSObjects

    <AttributeUsage(AttributeTargets.All)>
    Public Class RObjAttribute
        Inherits Attribute


        Public Sub New()

        End Sub

        ''' <summary>该对象的ROS基本命令行。如：/ip address。该属性应只作用于Class</summary>
        Public Property BaseCommand As String

        ''' <summary>该属性在ROS中的名称</summary>
        Public Property ROSName As String

    End Class


End Namespace

