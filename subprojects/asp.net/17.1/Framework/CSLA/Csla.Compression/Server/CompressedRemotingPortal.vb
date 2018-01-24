Imports Csla
Imports Csla.Server
Imports Csla.Compression
Namespace Server
    Public Class CompressedRemotingPortal
        Inherits MarshalByRefObject
        Implements ICompressedDataPortalServer
        Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As DataPortalContext) As Byte() Implements ICompressedDataPortalServer.Create
            Dim dp As New Csla.Server.DataPortal
            Return CompressionManager.Compress(dp.Create(objectType, criteria, context))
        End Function

        Public Function Delete(ByVal criteria As Object, ByVal context As DataPortalContext) As Byte() Implements ICompressedDataPortalServer.Delete
            Dim dp As New Csla.Server.DataPortal
            Return CompressionManager.Compress(dp.Delete(criteria, context))
        End Function

        Public Function Fetch(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As DataPortalContext) As Byte() Implements ICompressedDataPortalServer.Fetch
            Dim dp As New Csla.Server.DataPortal
            Return CompressionManager.Compress(dp.Fetch(objectType, criteria, context))
        End Function

        Public Function Update(ByVal obj As Byte(), ByVal context As DataPortalContext) As Byte() Implements ICompressedDataPortalServer.Update
            Dim dp As New Csla.Server.DataPortal
            Return CompressionManager.Compress(dp.Update(CompressionManager.Decompress(obj), context))
        End Function

    End Class

End Namespace

