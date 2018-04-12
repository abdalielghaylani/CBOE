VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "LDAPServer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private Const kLdapVersionAttr = "ldapversion"
Private Const kServerNameAttr = "name"
Private Const kServerPortAttr = "port"
Private Const kServerNode = "server"
Private Const kDefaultServerName = ""
Private Const kDefaultServerPort = -1    ' this is setting for default port


Private sName As String
Private lPort As Long
Private lVersion As Long
Private xmlServerNode As IXMLDOMNode
Private ldap As CLDAPProxy


Friend Property Get Name() As String
    Name = sName
End Property

Private Property Let Name(val As String)
    sName = val
End Property

Friend Property Get Port() As Long
    Port = lPort
End Property

Private Property Let Port(val As Long)
    lPort = val
End Property

Friend Property Get ProtocolVersion() As Long
    ProtocolVersion = lVersion
End Property

Private Property Let ProtocolVersion(val As Long)
    lVersion = val
End Property

Private Property Get ServerNode() As IXMLDOMNode
    Set ServerNode = xmlServerNode
End Property

Friend Sub Initialize(ByRef node As IXMLDOMNode, ByRef ldapProxy As CLDAPProxy)
    Set ldap = ldapProxy
    Set xmlServerNode = node.selectSingleNode(kServerNode)
    If Not xmlServerNode Is Nothing Then
        setServerAttributes node:=xmlServerNode
    End If
End Sub

Private Sub setServerAttributes(ByRef node As IXMLDOMNode)
                                
    Dim serverNodeMap As IXMLDOMNamedNodeMap
    Dim attr As IXMLDOMAttribute

    Set serverNodeMap = ServerNode.Attributes
    For Each attr In serverNodeMap
        Select Case LCase(attr.Name)
            Case kLdapVersionAttr
                Select Case attr.Text
                    Case "3"
                        'Version = LDAPv3 ' this is the default
                    Case Else
                        ProtocolVersion = LDAPv2
                End Select
                ldap.LDAPProtocol = ProtocolVersion
            Case kServerNameAttr
                Name = attr.Text
                
            Case kServerPortAttr
                Port = CLng(attr.Text)
            Case Else
                ' nothing
        End Select
    Next
   
    Set serverNodeMap = Nothing
End Sub

Private Sub Class_Initialize()
    Name = kDefaultServerName
    Port = kDefaultServerPort
    ProtocolVersion = LDAPv3
End Sub

Private Sub Class_Terminate()
    Set xmlServerNode = Nothing
End Sub
