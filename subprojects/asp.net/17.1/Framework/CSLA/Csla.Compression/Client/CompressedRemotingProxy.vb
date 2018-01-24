Imports System.Threading
Imports System.Reflection
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http
Imports System.Configuration
Imports Csla.Server
Imports Csla
Imports Csla.Compression
Namespace DataPortalClient

    ''' <summary>
    ''' Implements a data portal proxy to relay data portal
    ''' calls to a remote application server by using the
    ''' .NET Remoting technology.
    ''' </summary>
    Public Class CompressedRemotingProxy

        Implements Csla.DataPortalClient.IDataPortalProxy

#Region " Configure Remoting "

        ''' <summary>
        ''' Configure .NET Remoting to use a binary
        ''' serialization technology even when using
        ''' the HTTP channel. Also ensures that the
        ''' user's Windows credentials are passed to
        ''' the server appropriately.
        ''' </summary>
        Shared Sub New()

            ' create and register a custom HTTP channel
            ' that uses the binary formatter
            Dim properties As New Hashtable
            properties("name") = "HttpBinary"

            If ApplicationContext.AuthenticationType = "Windows" OrElse AlwaysImpersonate Then
                ' make sure we pass the user's Windows credentials
                ' to the server
                properties("useDefaultCredentials") = True
            End If

            Dim formatter As New BinaryClientFormatterSinkProvider

            Dim channel As New HttpChannel(properties, formatter, Nothing)

            ChannelServices.RegisterChannel(channel, EncryptChannel)

        End Sub

        Private Shared ReadOnly Property AlwaysImpersonate() As Boolean
            Get
                Dim result As Boolean = _
                  (ConfigurationManager.AppSettings("CslaAlwaysImpersonate") = "true")
                Return result
            End Get
        End Property

        Private Shared ReadOnly Property EncryptChannel() As Boolean
            Get
                Dim encrypt As Boolean = _
                  (ConfigurationManager.AppSettings("CslaEncryptRemoting") = "true")
                Return encrypt
            End Get
        End Property

#End Region

        Private mPortal As ICompressedDataPortalServer

        Private ReadOnly Property Portal() As ICompressedDataPortalServer
            Get
                If mPortal Is Nothing Then
                    Dim url As String = ConfigurationManager.AppSettings("CompressedDataPortalUrl")
                    If url Is Nothing Then
                        url = ApplicationContext.DataPortalUrl.ToString
                    End If
                    mPortal = CType( _
                      Activator.GetObject(GetType(Server.CompressedRemotingPortal), _
                      url), _
                      ICompressedDataPortalServer)
                End If
                Return mPortal
            End Get
        End Property

        ''' <summary>
        ''' Called by <see cref="DataPortal" /> to create a
        ''' new business object.
        ''' </summary>
        ''' <param name="objectType">Type of business object to create.</param>
        ''' <param name="criteria">Criteria object describing business object.</param>
        ''' <param name="context">
        ''' <see cref="Csla.Server.DataPortalContext" /> object passed to the server.
        ''' </param>
        Public Function Create( _
          ByVal objectType As System.Type, ByVal criteria As Object, _
          ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult _
          Implements Csla.Server.IDataPortalServer.Create

            Return DirectCast(CompressionManager.Decompress(Portal.Create(objectType, criteria, context)), DataPortalResult)

        End Function

        ''' <summary>
        ''' Called by <see cref="DataPortal" /> to load an
        ''' existing business object.
        ''' </summary>
        ''' <param name="objectType">Type of business object to retrieve.</param>
        ''' <param name="criteria">Criteria object describing business object.</param>
        ''' <param name="context">
        ''' <see cref="Csla.Server.DataPortalContext" /> object passed to the server.
        ''' </param>
        Public Function Fetch( _
          ByVal objectType As Type, _
          ByVal criteria As Object, _
          ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult _
          Implements Csla.Server.IDataPortalServer.Fetch

            Return DirectCast(CompressionManager.Decompress(Portal.Fetch(objectType, criteria, context)), DataPortalResult)

        End Function

        ''' <summary>
        ''' Called by <see cref="DataPortal" /> to update a
        ''' business object.
        ''' </summary>
        ''' <param name="obj">The business object to update.</param>
        ''' <param name="context">
        ''' <see cref="Csla.Server.DataPortalContext" /> object passed to the server.
        ''' </param>
        Public Function Update( _
          ByVal obj As Object, _
          ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult _
          Implements Csla.Server.IDataPortalServer.Update

            Return DirectCast(CompressionManager.Decompress(Portal.Update(CompressionManager.Compress(obj), context)), DataPortalResult)

        End Function

        ''' <summary>
        ''' Called by <see cref="DataPortal" /> to delete a
        ''' business object.
        ''' </summary>
        ''' <param name="criteria">Criteria object describing business object.</param>
        ''' <param name="context">
        ''' <see cref="Csla.Server.DataPortalContext" /> object passed to the server.
        ''' </param>
        Public Function Delete( _
          ByVal criteria As Object, _
          ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult _
          Implements Csla.Server.IDataPortalServer.Delete

            Return DirectCast(CompressionManager.Decompress(Portal.Delete(criteria, context)), DataPortalResult)

        End Function

        ''' <summary>
        ''' Get a value indicating whether this proxy will invoke
        ''' a remote data portal server, or run the "server-side"
        ''' data portal in the caller's process and AppDomain.
        ''' </summary>
        Public ReadOnly Property IsServerRemote() As Boolean _
          Implements Csla.DataPortalClient.IDataPortalProxy.IsServerRemote
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace
