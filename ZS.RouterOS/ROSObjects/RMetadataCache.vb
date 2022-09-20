
Namespace ROSObjects


    ''' <summary>
    ''' 原数据缓存
    ''' </summary>
    Public Class RMetadataCache

        Private Shared m_Cache As Dictionary(Of Type, RMetadata) = New Dictionary(Of Type, RMetadata)

        ''' <summary>
        ''' 获取指定类型的ROS对象的原数据
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Public Shared Function GetMetadata(Of T)() As RMetadata

            Dim key As Type = GetType(T)
            Dim result As RMetadata = Nothing

            If (Not m_Cache.TryGetValue(key, result)) Then
                result = New RMetadata(key)
                m_Cache.Add(key, result)
            End If

            Return result
        End Function


    End Class


End Namespace

