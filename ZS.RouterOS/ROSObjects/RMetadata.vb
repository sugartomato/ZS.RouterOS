
Namespace ROSObjects

    ''' <summary>
    ''' 标记在每个ROS对象类/属性上的原数据
    ''' </summary>
    Public Class RMetadata

        Public Property BaseCommand As String

        Public Sub New(ByVal T As Type)

            Dim att As RObjAttribute = CType(T.GetCustomAttributes(True).FirstOrDefault(Function(ByVal o As Object) TypeOf (o) Is RObjAttribute), RObjAttribute)

            If (Not att Is Nothing) Then
                Me.BaseCommand = att.BaseCommand
            End If

        End Sub



    End Class


End Namespace

