Imports Csla.Server
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.IO.Compression
Imports ICSharpCode.SharpZipLib

Namespace Compression
    Public Class CompressionManager
        Private Shared bSerializer As New BinaryFormatter
        Public Shared Function Compress(ByVal obj As Object) As Byte()
            Dim ms3 As New MemoryStream
            Dim cmp3 As New BZip2.BZip2OutputStream(ms3)
            ' GZipStream also works but gives a much lower compression ratio
            'Dim cmp3 As New GZipStream(ms3, CompressionMode.Compress, True)
            bSerializer.Serialize(cmp3, obj)
            cmp3.Flush()
            cmp3.Close()
            Return ms3.ToArray()
        End Function
        Public Shared Function Decompress(ByVal bytes() As Byte) As Object
            Dim ms4 As New MemoryStream(bytes)
            Dim cmp4 As New BZip2.BZip2InputStream(ms4)
            ' GZipStream also works but gives a much lower compression ratio
            'Dim cmp4 As New GZipStream(ms4, CompressionMode.Decompress, True)
            Dim o As Object = bSerializer.Deserialize(cmp4)
            Return o
        End Function

        Public Shared Function GetCompressedResult(ByVal result As DataPortalResult) As DataPortalResult
            Dim obj As Object = Compress(result.ReturnObject)
            Dim result1 As New DataPortalResult(obj)
            SetContext(result1, result.GlobalContext)
            Return result1
        End Function



        Private Shared Sub SetContext(ByVal result As DataPortalResult, ByVal context As System.Collections.Specialized.HybridDictionary)
            Dim fInfo As Reflection.FieldInfo = result.GetType().GetField("mGlobalContext", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.NonPublic)
            If fInfo IsNot Nothing Then
                fInfo.SetValue(result, context)
            End If
        End Sub

        Public Shared Function GetUncompressedResult(ByVal result As Csla.Server.DataPortalResult) As Csla.Server.DataPortalResult
            If TypeOf result.ReturnObject Is Byte() Then
                Dim obj As Object = Decompress(DirectCast(result.ReturnObject, Byte()))
                Dim result1 As New DataPortalResult(obj)
                SetContext(result1, result.GlobalContext)
                Return result1
            Else
                Return result
            End If
        End Function

    End Class

End Namespace