Imports Csla.DataPortalClient
Imports Csla.Server
Namespace DataPortalClient
    Public Class CustomDataPortalRemotingProxy
        Implements Csla.DataPortalClient.IDataPortalProxy

        Public ReadOnly Property IsServerRemote() As Boolean Implements Csla.DataPortalClient.IDataPortalProxy.IsServerRemote
            Get
                Return GetProxy().IsServerRemote
            End Get
        End Property

        Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult Implements Csla.Server.IDataPortalServer.Create
            Return GetProxy().Create(objectType, criteria, context)
        End Function

        Public Function Delete(ByVal criteria As Object, ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult Implements Csla.Server.IDataPortalServer.Delete
            Return GetProxy().Delete(criteria, context)
        End Function

        Public Function Fetch(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult Implements Csla.Server.IDataPortalServer.Fetch
            Return GetProxy().Fetch(objectType, criteria, context)
        End Function

        Public Function Update(ByVal obj As Object, ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult Implements Csla.Server.IDataPortalServer.Update
            Return GetProxy().Update(obj, context)
        End Function


#Region " Multi Dataportal code "

        Private mCachedPortals As New Dictionary(Of DataPortalTypes, IDataPortalProxy)
        Private Function GetProxy() As IDataPortalProxy
            If Not mCachedPortals.ContainsKey(mDPType) Then
                Select Case mDPType
                    Case DataPortalTypes.SimplePortal
                        mCachedPortals.Add(mDPType, New Csla.DataPortalClient.LocalProxy)
                    Case DataPortalTypes.RemotingPortal
                        mCachedPortals.Add(mDPType, New Csla.DataPortalClient.RemotingProxy)
                    Case DataPortalTypes.WebServicesPortal
                        mCachedPortals.Add(mDPType, New Csla.DataPortalClient.WebServicesProxy)
                    Case DataPortalTypes.CompressedRemotingPortal
                        mCachedPortals.Add(DataPortalTypes.CompressedRemotingPortal, New DataPortalClient.CompressedRemotingProxy)
                    Case Else
                        Throw New NotSupportedException("The selected DataPortalType is not supported")
                End Select
            End If
            Return mCachedPortals(mDPType)
        End Function

        Private Shared mDPType As DataPortalTypes = DataPortalTypes.RemotingPortal
        Public Shared Property DataPortalType() As DataPortalTypes
            Get
                Return mDPType
            End Get
            Set(ByVal value As DataPortalTypes)
                mDPType = value
            End Set
        End Property

        Shared Sub New()

        End Sub

#End Region

    End Class
End Namespace