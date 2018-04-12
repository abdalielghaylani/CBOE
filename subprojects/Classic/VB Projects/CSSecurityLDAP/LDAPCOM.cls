VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "LDAPCOM"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private Const kLDAPAuthenticateNodeName = "ldapauthenticate"
Private Const kLDAPUserListenerNodeName = "ldapuserlistener"

' -- configuration file node names
Private Const kProcessNode = "process"

Private Const kBindNode = "bind"
Private Const kDnNode = "dn"
Private Const kErrorsNode = "errors"
Private Const kQueryNode = "query"
Private Const kUnBindNode = "unbind"

Private sValidateNode As String


Public Property Get LDAPAuthenticateNodeName() As String
    LDAPAuthenticateNodeName = kLDAPAuthenticateNodeName
End Property

Public Property Get LDAPUserListenerNodeName() As String
    LDAPUserListenerNodeName = kLDAPUserListenerNodeName
End Property


Public Property Let ValidateNode(ByVal NodeName As String)
    sValidateNode = NodeName
    
End Property

Private Property Get ValidateNode() As String
    
    ValidateNode = sValidateNode
    
End Property


Public Sub ConfigDocument(ByRef config As DOMDocument, ByVal ConfigFile As String, ByVal ErrorSource As String)
    Dim configPath As String
    Dim fso As New FileSystemObject

    configPath = fso.BuildPath(App.Path, ConfigFile)
    With config
        .async = False
        .Load configPath
        With .parseError
            If .errorCode <> 0 Then
                Set config = Nothing
                Err.Raise vbObjectError + 513, _
                    Source:=ErrorSource, _
                    Description:=vbCrLf & "An error occured while reading " & configPath & vbCrLf & _
                                    "Line: " & .Line & vbCrLf & "Reason: " & .reason
                    
                Exit Sub
            End If
        End With
    End With
End Sub

Friend Property Get LDAPNode(ByVal ConfigElement As IXMLDOMElement, ByVal NodeName As String, ByVal ErrorSource As String) As IXMLDOMNode
    
    Set LDAPNode = ConfigElement.selectSingleNode(NodeName) ' ldap config node
    If LDAPNode Is Nothing Then
        Err.Raise vbObjectError + 513, _
                    Source:=ErrorSource, _
                    Description:=vbCrLf & "Node not found: " & vbCrLf & _
                        "Expected node " & NodeName & " was not found." & _
                        vbCrLf & "Contact Administrator to make required configuration changes."
        
        Exit Property
    End If
End Property

Public Sub ValidatePersonLDAP(ByVal ConfigElement As IXMLDOMElement, ByVal LoginID As String, _
                        ByVal password As String, ByRef IsAdmin As Boolean)
                        
    Const kGenericError = -2147467259
    Const kErrorSource = "LDAPCOM::ValidatePersonLDAP"

    Dim ldResults As New LDAPResults
    
    ' check for the server
    ' get the required values
    If ConfigElement Is Nothing Then
        Dim config As DOMDocument
        Set config = New DOMDocument
        Call ConfigDocument(config, "..\LDAPConfig.xml", kErrorSource)
        Set ConfigElement = config.documentElement
    End If
    
    ldResults.addAttribute attr:="username", val:=LoginID
    RunLDAPProcess ConfigElement:=ConfigElement, _
                    ldResults:=ldResults, _
                    NodeName:=LDAPAuthenticateNodeName, _
                    password:=password
    Set ldResults = Nothing
                    
End Sub


Public Sub RunLDAPProcess(ByVal ConfigElement As IXMLDOMElement, _
                            ByRef ldResults As LDAPResults, _
                            ByVal NodeName As String, _
                            Optional ByVal password As String)
    Const kGenericError = -2147467259
    Const kErrorSource = "LDAPCOM::RunLDAPProcess"
    
    
     '-- Attribute names constants
    Const kDisplayDetailsAttr = "displaydetails"
    
    Dim configNode As IXMLDOMNode
    Dim currNode As IXMLDOMNode
    Dim processNode As IXMLDOMNode
    Dim stepNode As IXMLDOMNode
    Dim nodeList As IXMLDOMNodeList
    Dim node As IXMLDOMNode
            
    ' Other variables
    Dim ldServer As New LDAPServer
    Dim ld As New CLDAPProxy
    Dim ldQuery As New LDAPQuery
    
    Dim isQueryError As Boolean
    Dim DN As String
    Dim searchScope As String
    Dim pwdRequired As Boolean
    Dim errDes As String
    Dim currErrorNode As String

    On Error GoTo LDAPErrorHandler
    
    Set configNode = LDAPNode(ConfigElement, NodeName, kErrorSource)

    ' set up ldap server
    ldServer.Initialize node:=configNode, ldapProxy:=ld
    
    ' This is the authenticator engine
    ' first create a dom document to hold the config variables
    ' add the username as the login id to the variable list
    ldResults.Initialize ldapProxy:=ld
    ' get the process node
    Set processNode = configNode.selectSingleNode(kProcessNode)
    If processNode Is Nothing Then
        Err.Raise vbObjectError + 513, _
                    Source:=kErrorSource, _
                    Description:=vbCrLf & "Process Node Missing: The authentication process workflow is missing from the configuration." & _
                    vbCrLf & "Contact Administrator to make required configuration changes."
        Exit Sub
    End If
    
    ' get the process child nodes.. bind, unbind and query
    Set nodeList = processNode.childNodes
    For Each stepNode In nodeList
        Select Case LCase(stepNode.NodeName)
            Case kBindNode
                ' each bind has own error code
                ' each bind clears the dn
                currErrorNode = ""
                DN = ""
                getBindNodeAttributes node:=stepNode, pwdRequired:=pwdRequired, bindError:=currErrorNode
                
                Set node = stepNode.selectSingleNode(kDnNode)
                If Not node Is Nothing Then
                    DN = node.Text
                    ldResults.replaceVar item:=DN
                End If
                Set node = Nothing
                'isBindError = True
                errDes = "Unable to bind to authentication service."
                ' here is where we bind
                If pwdRequired Then
                    If password = "" Then
                        Err.Raise vbObjectError + 513, _
                                    Source:=kErrorSource, _
                                    Description:=vbCrLf & "Blank Password found: A password is required for authentication."
                        Exit Sub
                    End If
                    If DN = "" Then
                        Err.Raise vbObjectError + 513, _
                                    Source:=kErrorSource, _
                                    Description:=vbCrLf & "Distinguished name not provided: A distinguished name is required for password authentication." & _
                                    vbCrLf & "Contact Administrator to make required configuration changes."
                        Exit Sub
                    End If
                    On Error GoTo LDAPErrorHandler
                    ld.Connect serverName:=ldServer.Name, Port:=ldServer.Port, DN:=DN, password:=password, useSSL:=0
                    On Error GoTo ErrorHandler
                Else
                    On Error GoTo LDAPErrorHandler
                    ld.Connect serverName:=ldServer.Name, Port:=ldServer.Port, DN:=DN, password:="", useSSL:=0
                    On Error GoTo ErrorHandler
                End If
                'isBindError = False
            Case kQueryNode
                ldQuery.Initialize node:=stepNode, ldResults:=ldResults
                currErrorNode = ldQuery.ErrorNode
                errDes = ldQuery.DefaultError
                
                isQueryError = True
                On Error GoTo LDAPErrorHandler
                ldQuery.RunQuery ldap:=ld
                
                If ldQuery.ResultsClass.Count = 0 Then
                    On Error GoTo ErrorHandler
                    GoTo LDAPErrorHandler
                End If
                isQueryError = False
                
            Case kUnBindNode
                On Error Resume Next
                DN = ""
                ld.Disconnect
                On Error GoTo ErrorHandler
            Case Else
                ' do nothing
        End Select
    Next stepNode
    Set stepNode = Nothing
    Set nodeList = Nothing
    Set configNode = Nothing
    Set processNode = Nothing
    Set ldQuery = Nothing
    Set ldServer = Nothing
    Set ld = Nothing
    
    Exit Sub
    
LDAPErrorHandler:
    Dim ErrorNode As IXMLDOMNode
    Dim details As String
    Dim displayDetails As Boolean
    displayDetails = False

    'details = "Login Id: " & LoginID & vbCrLf & _
    details = "Login Id: " & vbCrLf & _
                "Server: " & ldServer.Name & vbCrLf & _
               "Port: " & ldServer.Port & vbCrLf & _
               "DN: " & ldQuery.DN & vbCrLf
    'If isBindError Then
    '    errDes = "Unable to bind to authentication service."
    If isQueryError Then
        'errDes = "Failed to find items specified in the filter."
        details = details & vbCrLf & "Filter: " & ldQuery.Filter
    End If
    
    'mConnection.WriteLogStream kErrorSource & " : " & LoginID & " : ErrDes : " & errDes
    
    ' get custom error message if they are there
    Set ErrorNode = configNode.selectSingleNode(kErrorsNode)
    If Not ErrorNode Is Nothing Then
        Dim map As IXMLDOMNamedNodeMap
        ' determine if need to diplay details
        Set map = ErrorNode.Attributes
        If Not map Is Nothing Then
            If LCase(map.item(0).NodeName) = LCase(kDisplayDetailsAttr) Then
                displayDetails = CBool(map.item(0).Text)
            End If
        End If
        'If isBindError And bindErrorCode <> "" Then
        '    Set errorNode = errorNode.selectSingleNode(bindErrorCode)
        'ElseIf isQueryError And queryErrorCode <> "" Then
        '    Set errorNode = errorNode.selectSingleNode(ldQuery.ErrorCode)
        If (currErrorNode <> "") Then
            Set ErrorNode = ErrorNode.selectSingleNode(currErrorNode)
            If Not ErrorNode Is Nothing Then
                errDes = ErrorNode.Text
                If isQueryError Then ldQuery.ResultsClass.replaceVar item:=errDes
            End If
            If displayDetails Then errDes = errDes & vbCrLf & details
            Set ErrorNode = Nothing
        Else
            Set ErrorNode = Nothing
        End If
        
    End If
    If Err.Description <> "" Then
        Err.Description = Err.Description & vbCrLf & errDes
    Else
        Err.Description = errDes
    End If
ErrorHandler:
    ' Clean up object variables
    Set stepNode = Nothing
    Set nodeList = Nothing
    Set configNode = Nothing
    Set processNode = Nothing
    Set ldResults = Nothing
    Set ldQuery = Nothing
    Set ldServer = Nothing
    Set ld = Nothing
    ' Raise error
    
    If Err.Number < 0 Then
        Err.Raise Err.Number, kErrorSource, Err.Description
    Else
        Err.Raise vbObjectError + Err.Number, kErrorSource, Err.Description
    End If
End Sub


Private Sub getBindNodeAttributes(ByRef node As IXMLDOMNode, _
                                   ByRef pwdRequired As Boolean, _
                                   ByRef bindError As String)
    Const kBindErrorMsgAttr = "error"
    Const kPwdRequiredAttr = "pwdrequired"
    
    Dim bindNodeMap As IXMLDOMNamedNodeMap
    Dim attr As IXMLDOMAttribute
    
    pwdRequired = True
    bindError = ""
    
    Set bindNodeMap = node.Attributes
    For Each attr In bindNodeMap
        Select Case LCase(attr.Name)
            Case kPwdRequiredAttr
                pwdRequired = CBool(attr.Text)
            Case kBindErrorMsgAttr
                bindError = attr.Text
            Case Else
                ' nothing
        End Select
    Next
    Set bindNodeMap = Nothing
End Sub

