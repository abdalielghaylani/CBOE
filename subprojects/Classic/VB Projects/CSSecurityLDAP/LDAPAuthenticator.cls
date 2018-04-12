VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "LDAPAuthenticator"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Public Function Authenticate(ByVal ConfigXmlPath As String, ByVal ADDomain As String, ByVal UserName As String, ByVal password As String) As Integer
    Dim config As New DOMDocument
    Dim oLDAPCOM As New LDAPCOM
    Dim nodes As IXMLDOMNodeList
    Dim node As IXMLDOMNode
    Dim fullName As String
    
    Const kExemptUsersNode = "exemptuser"
    
    
    On Error GoTo ErrorHandler
    
    ConfigDocument config, ConfigXmlPath, ""
    
    ' check for ldap exempt users
    ' then check if current login is exempt
    Set nodes = config.selectNodes("//" & oLDAPCOM.LDAPAuthenticateNodeName & "/" & kExemptUsersNode)
    
    For Each node In nodes
        If LCase(node.Text) = LCase(UserName) Then
            Authenticate = -1   '===> Exit point
            Exit Function
        End If
    Next
    Set nodes = Nothing
    
    If ADDomain <> "" Then fullName = ADDomain & "\" & UserName
    
    ' call validatenode to set the node that will be used in the validation
    oLDAPCOM.ValidateNode = oLDAPCOM.LDAPAuthenticateNodeName
    oLDAPCOM.ValidatePersonLDAP config.documentElement, fullName, password, False
    Set oLDAPCOM = Nothing
    Authenticate = 1            '====> Exit point
    Exit Function
ErrorHandler:
    ' Raise error
    Set oLDAPCOM = Nothing
    Set nodes = Nothing
    
    Dim errDesc As String
    Dim errNum As Long

    errDesc = Err.Description
    errNum = Err.Number

    If errNum < 0 Then
        Err.Raise errNum, kErrorSource, errDesc
    Else
        Err.Raise vbObjectError + errNum, kErrorSource, errDesc
    End If
    
End Function



Public Function GetUserInfo(ByVal ConfigXmlPath As String, ByVal UserName As String, ByVal svcAccount As String, ByVal password As String) As LDAPResults
    Dim config As New DOMDocument
    Dim oLDAPCOM As New LDAPCOM
    Dim nodes As IXMLDOMNodeList
    Dim node As IXMLDOMNode
    Dim tnode As IXMLDOMNode
    Dim tAttr As IXMLDOMAttribute
    Dim ldResults As New LDAPResults
    Dim mapToAttr As IXMLDOMNode
    Dim displayNameAttr As IXMLDOMNode
     
    Const kGetUserInfoNode = "getUserInfo"
    Const kErrorSource = "CSSecurityLDAP:GetUserInfo"
    Const kMapToAttrName = "mapTo"
    Const kDisplayNameAttrName = "displayName"
    
    On Error GoTo ErrorHandler
    
    ' Read the configuration file
    ConfigDocument config, ConfigXmlPath, ""
    
    ' Add the user name to the results
    ldResults.addAttribute "username", UserName
    ' Optionally add the Service Account to the results
    If svcAccount <> "" Then ldResults.addAttribute "svcaccount", svcAccount
    
    ' Run the LDAP process defined in the configuration file
    oLDAPCOM.RunLDAPProcess config.documentElement, ldResults, kGetUserInfoNode, password
    Set oLDAPCOM = Nothing
    
    
    ' Transfer the mapto and display name attributes from the config file to the results file
    Set nodes = config.documentElement.selectNodes("//" & kGetUserInfoNode & "/process/query/attr")
    For Each node In nodes
        Set mapToAttr = node.Attributes.getNamedItem(kMapToAttrName)
        Set displayNameAttr = node.Attributes.getNamedItem(kDisplayNameAttrName)
        If Not (mapToAttr Is Nothing) Then
            Set tnode = ldResults.XML.selectSingleNode("/results/" & node.Text)
            Set tAttr = tnode.ownerDocument.createAttribute(kMapToAttrName)
            tAttr.value = mapToAttr.Text
            tnode.Attributes.setNamedItem tAttr
            Set tAttr = tnode.ownerDocument.createAttribute(kDisplayNameAttrName)
            tAttr.value = displayNameAttr.Text
            tnode.Attributes.setNamedItem tAttr
        End If
    Next
     
    ' Return the results object.
    Set GetUserInfo = ldResults
    Exit Function '====> Exit point
ErrorHandler:
    ' Raise error
    Set oLDAPCOM = Nothing
    Set nodes = Nothing
    
    Dim errDesc As String
    Dim errNum As Long

    errDesc = Err.Description
    errNum = Err.Number

    If errNum < 0 Then
        Err.Raise errNum, kErrorSource, errDesc
    Else
        Err.Raise vbObjectError + errNum, kErrorSource, errDesc
    End If

End Function


Public Function CheckUserExists(ByVal ConfigXmlPath As String, ByVal UserName As String, ByVal password As String) As Integer
    Dim config As New DOMDocument
    Dim oLDAPCOM As New LDAPCOM
    Dim nodes As IXMLDOMNodeList
    Dim node As IXMLDOMNode
    Dim ldResults As New LDAPResults
    
    Const kCheckUserNode = "checkUserExists"
    
    Const kErrorSource = "CSSecurityLDAP:CheckUser"
    On Error GoTo ErrorHandler
    
    ConfigDocument config, ConfigXmlPath, ""
    
    ldResults.addAttribute "username", UserName
    oLDAPCOM.RunLDAPProcess config.documentElement, ldResults, kCheckUserNode, password
    Set oLDAPCOM = Nothing
    CheckUserExists = 1            '====> Exit point
    Exit Function
ErrorHandler:
    ' Raise error
    Set oLDAPCOM = Nothing
    Set nodes = Nothing
    
    Dim errDesc As String
    Dim errNum As Long

    errDesc = Err.Description
    errNum = Err.Number

    If errNum < 0 Then
        Err.Raise errNum, kErrorSource, errDesc
    Else
        Err.Raise vbObjectError + errNum, kErrorSource, errDesc
    End If

End Function

Sub ConfigDocument(ByRef config, ByVal ConfigFile, ByVal ErrorSource)
    Dim configPath
    Dim fso

    Set fso = CreateObject("Scripting.FileSystemObject")

    configPath = fso.BuildPath("", ConfigFile)
    With config
        .async = False
        .Load configPath
        With .parseError
            If .errorCode <> 0 Then
                Set config = Nothing
                Err.Raise vbObjectError + 513, Err.Source, "An error occured while reading " & configPath & vbCrLf & _
                "Line: " & .Line & vbCrLf & "Reason: " & .reason
            End If
        End With
    End With
End Sub

