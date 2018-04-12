Attribute VB_Name = "UtilsInventory"
Option Explicit

Public msServerName As String
Public mbUseSSL As Boolean
Public msOraServiceName As String
Public msInvSchemaName As String
Public msUserID As String
Public msPassword As String
Public msListSeparator As String

Private Const AUTHENTICATE_USER_URL = "/cheminv/api/AuthenticateUser.asp"
Private Const CREATE_PLATE_URL = "/cheminv/api/createplatexml.asp"
Private Const CREATE_SUBSTANCE_URL = "/cheminv/api/createsubstanceXML.asp"
Private Const INVLOADER_SQL_URL = "/cheminv/api/invloadersql.asp"
Private Const INVLOADER_PICKLIST_URL = "/cheminv/api/invloaderpicklist.asp"
Private Const PROCESS_ACTION_BATCH_URL = "/cheminv/api/ProcessActionBatch3.asp"
Private Const GET_PRIMARY_KEY_URL = "/cheminv/api/GetPrimaryKeyIDs.asp"
Private Const LOOKUPVALUE_URL = "/cheminv/api/LookUpValue.asp"
Private Const GETLOCATIONFROMID_URL = "/cheminv/api/GetLocationFromID.asp"
Private Const GETLOCATIONFROMBARCODE_URL = "/cheminv/api/GetLocationFromBarcode_Invloader.asp"
Private Const ISVALIDCOMPOUNDID_URL = "/cheminv/api/IsValidCompoundID.asp"
Private Const GETBATCHINFO_URL = "/cheminv/api/GetBatchInfo.asp"
Private Const GETAPPLICATIONVARIABLES_URL = "/cheminv/api/GetApplicationVariables.asp"
Declare Sub Sleep Lib "kernel32.dll" (ByVal dwMilliseconds As Long)

Public Enum EnumStatementIds
    ' explicit numbering for easier matching with InvLoaderSQL.asp page
    eStmtNumberOfCompoundWells = 0
    eStmtPlateLocations = 1
    eStmtPlateFormats = 2
    eStmtPlateFormatFull = 3
    eStmtPlateTypes = 4
    eStmtLibraries = 5
    eStmtBarcodeDescs = 6
    eStmtBarcodePrefixes = 7
    eStmtPlateDimensions = 8
    eStmtWellFormats = 9
    eStmtExistingPlates = 10
    eStmtRegID = 11
    eStmtContainerLocations = 13
    eStmtContainerType = 14
    eStmtContainerStatus = 15
    eStmtContainerUnits = 16
    eStmtContainerOwner = 17
    
End Enum

Public Function CastDBField(obj As Variant) As DBField
    Set CastDBField = obj
End Function


' kfd2, designed to throw an error
Public Function GetPicklistRS(PicklistName As String)
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim rs As ADODB.Recordset
    Dim sURL As String
    Dim sContent As String
    
    Set oHTTP = New MSXML2.XMLHTTP40
    sURL = Protocol(mbUseSSL) & msServerName & INVLOADER_PICKLIST_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    sContent = "PicklistName=" & PicklistName
    oHTTP.open "POST", sURL, False
    oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
    oHTTP.send sContent
    If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
        Err.Raise vbError + 1, , INVLOADER_PICKLIST_URL & " returned an error: " & oHTTP.responseText
    End If
    Set rs = New ADODB.Recordset
    rs.CursorLocation = adUseClient
    rs.open oHTTP.responseXML
    Set GetPicklistRS = rs
    Set oHTTP = Nothing
End Function

Public Function ExecuteStatement(ByVal StatementId As Long, Optional ByVal Params As String = "")
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    
    Set oHTTP = New MSXML2.XMLHTTP40
    sURL = Protocol(mbUseSSL) & msServerName & INVLOADER_SQL_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    sContent = "StatementId=" & StatementId
    If Params <> "" Then
        sContent = sContent & "&Params=" & Params
    End If
    
'    On Error Resume Next
    On Error GoTo Error     ' kfd2, switched to returning an error for 11.0.4
    oHTTP.open "POST", sURL, False
'    If Err.Number <> 0 Then
'        If Err.Number = -2147012891 Then
'            ExecuteStatement = "Error: Invalid Server"
'            Err.Clear
'        Else
'            ExecuteStatement = "Error:" & Err.Description & Err.Number
'            Err.Clear
'        End If
'    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        'MsgBox oHTTP.responseText
    
        'CSBR-123144 JBattles 18-March-10
'        If Err.Number = "-2146697211" Then
'            MsgBox "Error: Unable to connect to the server. " & vbCrLf & _
'                   "The likely cause is that the CBOE Server is" & vbCrLf & _
'                   "not configured to use SSL.  Pleas restart " & vbCrLf & _
'                   "InvLoader without checking the 'Use SSL (https:)'" & vbCrLf & _
'                   "checkbox.", vbCritical
'            End
'        ElseIf Err.Number <> 0 Then
'            ExecuteStatement = "Error:" & Err.Description
'            Err.Clear
'        Else
            'Debug.Print oHTTP.responseText
            If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
                'CSBR 148452 SJ Uncommenting this line for a valid message if the user does not have sufficient privileges.
                ExecuteStatement = "Error: Inventory Manager returned an error.  Please contact your system administrator."
'                 Err.Raise vbError + 1, , "Inventory Manager returned an error.  Please contact your system administrator."
            Else
                Select Case StatementId
                    Case eStmtNumberOfCompoundWells
                        ExecuteStatement = oHTTP.responseText
                    Case Else
                        Dim rs  As ADODB.Recordset
                        Set rs = New ADODB.Recordset
                        rs.CursorLocation = adUseClient
                        rs.open oHTTP.responseXML
                        Set ExecuteStatement = rs
                End Select
            End If
'        End If
'    End If
    Set oHTTP = Nothing
    Exit Function
Error:
    If Err.Number = "-2146697211" Then
        MsgBox "Error: Unable to connect to the server. " & vbCrLf & _
               "The likely cause is that the CBOE Server is" & vbCrLf & _
               "not configured to use SSL.  Pleas restart " & vbCrLf & _
               "InvLoader without checking the 'Use SSL (https:)'" & vbCrLf & _
               "checkbox.", vbCritical
    End If
    Err.Raise Err.Number, Err.Source, Err.Description
End Function

Public Function AuthenticateUser(ByVal ServerName As String, ByVal UserID As String, ByVal Password As String)

    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    
    Set oHTTP = New MSXML2.XMLHTTP40
    sURL = Protocol(mbUseSSL) & ServerName & AUTHENTICATE_USER_URL
    sContent = "UserID=" & URLEncode(UserID) & "&Password=" & URLEncode(Password) & "&PrivTables=cheminv_privileges" 'CSBR-156006: SJ
        
    On Error Resume Next
    oHTTP.open "POST", sURL, False
    If Err.Number <> 0 Then
        If Err.Number = -2147012891 Then
            AuthenticateUser = "Error: Invalid Server"
            Err.Clear
        Else
            AuthenticateUser = "Error:" & Err.Description & Err.Number
            Err.Clear
        End If
    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        'MsgBox oHTTP.responseText
        'CSBR-159940: Message not relevant to the action performed SJ
        If (Err.Number = "-2146697211" And frmCFWDBWiz.chkUseSSL = vbChecked) Then
            MsgBox "Error: Unable to connect to the server. " & vbCrLf & _
                   "The likely cause is that the CBOE Server is" & vbCrLf & _
                   "not configured to use SSL.  Please restart " & vbCrLf & _
                   "InvLoader without checking the 'Use SSL (https:)'" & vbCrLf & _
                   "checkbox.", vbCritical
            End
        ElseIf Err.Number <> 0 Then
            AuthenticateUser = "Error: The remote server name could not be resolved." & vbCrLf & " Please check the server name."
            Err.Clear
        Else
            'MsgBox Err.Description
            'CSBR-123147 jbattles 18-Mar-2010
            If InStr(LCase$(oHTTP.responseText), "error") Or InStr(LCase$(oHTTP.responseText), "page cannot be found") Then
                AuthenticateUser = "Error: Inventory Manager returned an error.  Please contact your system administrator." & vbCrLf & _
                "Error returned: " & oHTTP.responseText
            'CSBR 148452 SJ To display a valid message if the server name is invalid
            ElseIf InStr(LCase$(oHTTP.responseText), "failed") Then
                AuthenticateUser = "Error: Problem Connecting to the Server.Please contact your system administrator."
            Else
                AuthenticateUser = oHTTP.responseText
            End If
        End If
    End If
    Set oHTTP = Nothing
End Function

Public Function SendPlateXML(PlateXML As String, ByVal bRegisterCompounds As Boolean) As String
    ' use MSXML to make calls to Inventory API
    Dim sReq As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim oPlateXML As MSXML2.DOMDocument40
    Dim oPlatesElement As MSXML2.IXMLDOMElement
    Dim oAttribute As MSXML2.IXMLDOMAttribute
    Dim sURL As String
    Dim tmStart As Date
    Dim tmCurr As Date
    Dim lTimeTaken As Long
    Dim sRet As String

    
    Set oHTTP = New MSXML2.XMLHTTP40
    Set oPlateXML = New MSXML2.DOMDocument40
    sURL = Protocol(mbUseSSL) & msServerName & CREATE_PLATE_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    oPlateXML.loadXML (PlateXML)
    ' add api variables to the xml doc as attributes of the root PLATES element
    Set oPlatesElement = oPlateXML.documentElement
    ' username and pass
    Call addAttribute(oPlateXML, oPlatesElement, "CSUSERNAME", msUserID)
    Call addAttribute(oPlateXML, oPlatesElement, "CSUSERID", msPassword)
    ' registercompounds
    Call addAttribute(oPlateXML, oPlatesElement, "REGISTERCOMPOUNDS", IIf(bRegisterCompounds, "true", "false"))
    ' reg user and pwd
    If bRegisterCompounds Then
        Call addAttribute(oPlateXML, oPlatesElement, "REGUSER", msUserID)
        Call addAttribute(oPlateXML, oPlatesElement, "REGPWD", msPassword)
    End If
    Call addAttribute(oPlateXML, oPlatesElement, "HTTPTIMEOUT", glHTTPTimeout)
     
    oHTTP.open "POST", sURL, True
    
    ' if server doesn't exist, send method will fail
    oHTTP.send oPlateXML.xml
    
    tmStart = Now
    Do
        tmCurr = Now
        lTimeTaken = CInt(DateDiff("s", tmStart, tmCurr))
        
        If lTimeTaken > glHTTPTimeout Then
            sRet = "Timeout Expired.  Please decrease the number of compounds you are trying to import, or increase the HTTP timeout value in Advanced Options."
            oHTTP.abort
            Exit Do
        End If
        
        DoEvents
        Sleep 250
    Loop While oHTTP.readyState <> 4
    
    If oHTTP.readyState = 4 Then
        'Debug.Print Len(oHTTP.responseText)
        sRet = oHTTP.responseText
    End If
    
    
    ' if success, then it will return a number
    'If Not IsNumeric(sRet) Then
    '    sRet = "Error connecting to Inventory Server:" & vbCrLf & _
    '            sRet & vbCrLf & vbCrLf & _
    '            "URL: " & surl & vbCrLf & _
    '            "Parameters: " & vbCrLf & "     " & oPlateXML.xml
    'Else
    '    sRet = ""
    'End If
    'If sRet <> "" Then
    '     Debug.Print sRet
    'End If
    SendPlateXML = sRet
    Set oHTTP = Nothing
    Set oPlateXML = Nothing
End Function

Public Function SendSubstanceXML(CompoundXML As String) As String
    ' use MSXML to make calls to Inventory API
    Dim sReq As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim oCompoundXML As MSXML2.DOMDocument40
    Dim oCompoundsElement As MSXML2.IXMLDOMElement
    Dim oAttribute As MSXML2.IXMLDOMAttribute
    Dim sURL As String
    Dim sRet As String
    Dim tmStart As Date
    Dim tmCurr As Date
    Dim lTimeTaken As Long
    
    Set oHTTP = New MSXML2.XMLHTTP40
    Set oCompoundXML = New MSXML2.DOMDocument40
    sURL = Protocol(mbUseSSL) & msServerName & CREATE_SUBSTANCE_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    oCompoundXML.loadXML (CompoundXML)
    ' add api variables to the xml doc as attributes of the root PLATES element
    Set oCompoundsElement = oCompoundXML.documentElement
    ' username and pass
    Call addAttribute(oCompoundXML, oCompoundsElement, "CSUSERNAME", msUserID)
    Call addAttribute(oCompoundXML, oCompoundsElement, "CSUSERID", msPassword)
    oHTTP.open "POST", sURL, True
    
    'On Error Resume Next
    ' if server doesn't exist, send method will fail
    oHTTP.send oCompoundXML.xml
    
    tmStart = Now
    Do
        tmCurr = Now
        lTimeTaken = CInt(DateDiff("s", tmStart, tmCurr))
        
        If lTimeTaken > glHTTPTimeout Then
            'Debug.Print "Timput Expired. Please retry!"
            sRet = "Timeout Expired.  Please decrease the number of compounds you are trying to import."
            oHTTP.abort
            Exit Do
        End If
        
        DoEvents
        Sleep 250
    
    Loop While oHTTP.readyState <> 4
    
    If oHTTP.readyState = 4 Then
        'Debug.Print Len(oHTTP.responseText)
        sRet = oHTTP.responseText
    End If
    
    
    ' if success, then it will return a number
    'If Not IsNumeric(sRet) Then
    '    sRet = "Error connecting to Inventory Server:" & vbCrLf & _
    '            sRet & vbCrLf & vbCrLf & _
    '            "URL: " & surl & vbCrLf & _
    '            "Parameters: " & vbCrLf & "     " & oPlateXML.xml
    'Else
    '    sRet = ""
    'End If
    'If sRet <> "" Then
    '     Debug.Print sRet
    'End If
    SendSubstanceXML = sRet
    Set oHTTP = Nothing
    Set oCompoundXML = Nothing
End Function


Private Sub addAttribute(ByRef oDOM As MSXML2.DOMDocument40, ByRef oElement As MSXML2.IXMLDOMElement, ByVal attribName As String, ByVal attribValue As String)
    Dim oAttribute As MSXML2.IXMLDOMAttribute
    Set oAttribute = oDOM.createAttribute(attribName)
    oAttribute.Text = attribValue
    oElement.setAttributeNode oAttribute
End Sub

Sub Main()
    If Not UtilsBase64.CheckBase64() Then
        MsgBox "Could not locate Base64.DLL", vbCritical
    Else
        frmCFWDBWiz.Show
    End If
End Sub

Public Function SendContainerXML(ContainerXML As String, ByVal bRegisterCompounds As Boolean) As String
    ' use MSXML to make calls to Inventory API
    Dim sReq As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim oContainerXML As MSXML2.DOMDocument40
    Dim oContainerElement As MSXML2.IXMLDOMElement
    Dim oAttribute As MSXML2.IXMLDOMAttribute
    Dim sURL As String
    Dim tmStart As Date
    Dim tmCurr As Date
    Dim lTimeTaken As Long
    Dim sRet As String
    
    Set oHTTP = New MSXML2.XMLHTTP40
    Set oContainerXML = New MSXML2.DOMDocument40
    sURL = Protocol(mbUseSSL) & msServerName & PROCESS_ACTION_BATCH_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    oContainerXML.loadXML (ContainerXML)
    ' add api variables to the xml doc as attributes of the root PLATES element
    Set oContainerElement = oContainerXML.documentElement
    ' username and pass
    Call addAttribute(oContainerXML, oContainerElement, "CSUSERNAME", msUserID)
    Call addAttribute(oContainerXML, oContainerElement, "CSUSERID", msPassword)
    ' registercompounds
    Call addAttribute(oContainerXML, oContainerElement, "REGISTERCOMPOUNDS", IIf(bRegisterCompounds, "true", "false"))
    'Checking if Register option is selected.
    'CSBR 126952 : sjacob
    If bRegisterCompounds Then
        Call addAttribute(oContainerXML, oContainerElement, "REGUSER", msUserID)
        Call addAttribute(oContainerXML, oContainerElement, "REGPWD", msPassword)
    End If
        
    oHTTP.open "POST", sURL, True
    
    ' if server doesn't exist, send method will fail
    oHTTP.send oContainerXML.xml
    
    tmStart = Now
    Do
        tmCurr = Now
        lTimeTaken = CInt(DateDiff("s", tmStart, tmCurr))
        
        If lTimeTaken > glHTTPTimeout Then
            sRet = "Timeout Expired.  Please decrease the number of compounds you are trying to import, or increase the HTTP timeout value in Advanced Options."
            oHTTP.abort
            Exit Do
        End If
        
        DoEvents
        Sleep 250
    Loop While oHTTP.readyState <> 4
    
    If oHTTP.readyState = 4 Then
        'Debug.Print Len(oHTTP.responseText)
        sRet = oHTTP.responseText
    End If
    SendContainerXML = sRet
    Set oHTTP = Nothing
    Set oContainerXML = Nothing
End Function

Public Function GetPrimaryKeyIDs(TableName As String, TableValues As String) As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    
    Set oHTTP = New MSXML2.XMLHTTP40

    sURL = Protocol(mbUseSSL) & msServerName & GET_PRIMARY_KEY_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    sContent = "TableName=" & TableName & "&TableValues=" & URLEncode(TableValues)
    
    On Error Resume Next
    oHTTP.open "POST", sURL, False
    If Err.Number <> 0 Then
        If Err.Number = -2147012891 Then
            GetPrimaryKeyIDs = "Error: Invalid Server"
            Err.Clear
        Else
            GetPrimaryKeyIDs = "Error:" & Err.Description & Err.Number
            Err.Clear
        End If
    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        
        If Err.Number <> 0 Then
            GetPrimaryKeyIDs = "Error:" & Err.Description
            Err.Clear
        Else
            If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
                GetPrimaryKeyIDs = "Error: Inventory Manager returned an error.  Please contact your system administrator."
            Else
                GetPrimaryKeyIDs = oHTTP.responseText
            End If
        End If
    End If
    Set oHTTP = Nothing
End Function

Public Function LookUpValue(TableName As String, TableValue As String) As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    
    Set oHTTP = New MSXML2.XMLHTTP40
    
    sURL = Protocol(mbUseSSL) & msServerName & LOOKUPVALUE_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    If UCase(TableName) = "INV_SOLVENTS" Then
        sContent = "TableName=" & TableName & "&TableValue=" & URLEncode(TableValue) & "&InsertIfNotFound=true"
    Else
        sContent = "TableName=" & TableName & "&TableValue=" & URLEncode(TableValue) & "&InsertIfNotFound=false"
    End If
    On Error Resume Next
    oHTTP.open "POST", sURL, False
    If Err.Number <> 0 Then
        If Err.Number = -2147012891 Then
            LookUpValue = "Error: Invalid Server"
            Err.Clear
        Else
            LookUpValue = "Error:" & Err.Description & Err.Number
            Err.Clear
        End If
    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        
        If Err.Number <> 0 Then
            LookUpValue = "Error:" & Err.Description
            Err.Clear
        Else
            If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
                LookUpValue = "Error: Inventory Manager returned an error.  Please contact your system administrator. " & vbLf & oHTTP.responseText
            Else
                LookUpValue = oHTTP.responseText
            End If
        End If
    End If
    Set oHTTP = Nothing
End Function

Public Function GetLocationFromID(LocationID As Long) As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    
    Set oHTTP = New MSXML2.XMLHTTP40
    
    sURL = Protocol(mbUseSSL) & msServerName & GETLOCATIONFROMID_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    sContent = "LocationID=" & CStr(LocationID)
    
    On Error Resume Next
    oHTTP.open "POST", sURL, False
    If Err.Number <> 0 Then
        If Err.Number = -2147012891 Then
            GetLocationFromID = "Error: Invalid Server"
            Err.Clear
        Else
            GetLocationFromID = "Error:" & Err.Description & Err.Number
            Err.Clear
        End If
    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        
        If Err.Number <> 0 Then
            GetLocationFromID = "Error:" & Err.Description
            Err.Clear
        Else
            If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
                GetLocationFromID = "Error: Inventory Manager returned an error.  Please contact your system administrator. " & vbLf & oHTTP.responseText
            Else
                GetLocationFromID = oHTTP.responseText
            End If
        End If
    End If
    Set oHTTP = Nothing
End Function

Public Function GetLocationFromBarcode(LocationBarcode As String) As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    
    Set oHTTP = New MSXML2.XMLHTTP40
    
    sURL = Protocol(mbUseSSL) & msServerName & GETLOCATIONFROMBARCODE_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    sContent = "LocationBarcode=" & CStr(LocationBarcode)
    sURL = sURL & "&" & sContent
    On Error Resume Next
    oHTTP.open "POST", sURL, False
    If Err.Number <> 0 Then
        If Err.Number = -2147012891 Then
            GetLocationFromBarcode = "Error: Invalid Server"
            Err.Clear
        Else
            GetLocationFromBarcode = "Error:" & Err.Description & Err.Number
            Err.Clear
        End If
    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        
        If Err.Number <> 0 Then
            GetLocationFromBarcode = "Error:" & Err.Description
            Err.Clear
        Else
            If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
                GetLocationFromBarcode = "Error: Inventory Manager returned an error.  Please contact your system administrator. " & vbLf & oHTTP.responseText
            Else
                GetLocationFromBarcode = oHTTP.responseText
            End If
        End If
    End If
    Set oHTTP = Nothing
End Function
Public Function IsValidCompoundID(CompoundID As Long) As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    
    Set oHTTP = New MSXML2.XMLHTTP40
    
    sURL = Protocol(mbUseSSL) & msServerName & ISVALIDCOMPOUNDID_URL & "?UserID=" & msUserID & "&Password=" & msPassword
    sContent = "compoundID=" & CStr(CompoundID)
    
    On Error Resume Next
    oHTTP.open "POST", sURL, False
    If Err.Number <> 0 Then
        If Err.Number = -2147012891 Then
            IsValidCompoundID = "Error: Invalid Server"
            Err.Clear
        Else
            IsValidCompoundID = "Error:" & Err.Description & Err.Number
            Err.Clear
        End If
    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        
        If Err.Number <> 0 Then
            IsValidCompoundID = "Error:" & Err.Description
            Err.Clear
        Else
            If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
                IsValidCompoundID = "Error: Inventory Manager returned an error.  Please contact your system administrator. " & vbLf & oHTTP.responseText
            Else
                IsValidCompoundID = oHTTP.responseText
            End If
        End If
    End If
    Set oHTTP = Nothing
End Function



Public Function GetBatchInfo(RegNumber As String, BatchNumber As Long) As String
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    Dim tempPassword
    Set oHTTP = New MSXML2.XMLHTTP40
    'CBOE-1365 SJ
    tempPassword = ""
    tempPassword = msPassword
    tempPassword = URLEncode(CryptVBS(tempPassword, "ChemInv\API\GetBatchInfo.asp"))
    sURL = Protocol(mbUseSSL) & msServerName & GETBATCHINFO_URL & "?UserID=" & msUserID & "&Password=" & tempPassword
    sContent = "RegNumber=" & RegNumber & "&BatchNumber=" & BatchNumber
    
    On Error Resume Next
    oHTTP.open "POST", sURL, False
    If Err.Number <> 0 Then
        If Err.Number = -2147012891 Then
            GetBatchInfo = "Error: Invalid Server"
            Err.Clear
        Else
            GetBatchInfo = "Error:" & Err.Description & Err.Number
            Err.Clear
        End If
    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        
        If Err.Number <> 0 Then
            GetBatchInfo = "Error:" & Err.Description
            Err.Clear
        Else
            If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
                GetBatchInfo = "Error: Inventory Manager returned an error.  Please contact your system administrator. " & vbLf & oHTTP.responseText
            ElseIf InStr(oHTTP.responseText, "Invalid REG_SERVER_NAME") Then 'CBOE-1365 SJ Checking if Inv-Reg integration is enabled.
                GetBatchInfo = "Error: Inventory Registration integration is not enabled on the middle tier server." & vbLf & "Please contact your system administrator."
            Else
                GetBatchInfo = oHTTP.responseText
            End If
        End If
    End If
    Set oHTTP = Nothing
End Function
Function CryptVBS(Text, key)
    Dim keylen, i, KeyPtr, stxtchr, wTxtChr, wKeyChr, CryptKey, hold
    Text = shiftChr(Text, -1)
    keylen = Len(key)
    For i = 1 To Len(Text)
        KeyPtr = (KeyPtr + 1) Mod keylen
        stxtchr = Mid(Text, i, 1)
        wTxtChr = Asc(stxtchr)
        wKeyChr = Asc(Mid(key, KeyPtr + 1, 1))
        CryptKey = Chr(wTxtChr Xor wKeyChr)
        hold = hold & CryptKey
    Next
    CryptVBS = shiftChr(hold, 1)
End Function
Function shiftChr(str, s)
    Dim i, hold
    For i = 1 To Len(str)
        hold = hold & Chr(Asc(Mid(str, i, 1)) + s)
    Next
    shiftChr = hold
End Function

Public Function GetApplicationVariables()
    Dim oHTTP As MSXML2.XMLHTTP40
    Dim sURL As String, sContent As String
    Set oHTTP = New MSXML2.XMLHTTP40
    
    sURL = Protocol(mbUseSSL) & msServerName & GETAPPLICATIONVARIABLES_URL & "?CSuserName=" & msUserID & "&Password=" & msPassword
    
    On Error Resume Next
    oHTTP.open "POST", sURL, False
    If Err.Number <> 0 Then
        If Err.Number = -2147012891 Then
            GetApplicationVariables = "Error: Invalid Server"
            Err.Clear
        Else
            GetApplicationVariables = "Error:" & Err.Description & Err.Number
            Err.Clear
        End If
    Else
        oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
        oHTTP.send sContent
        
        If Err.Number <> 0 Then
            GetApplicationVariables = "Error:" & Err.Description
            Err.Clear
        Else
            If InStr(oHTTP.responseText, "error") Or InStr(oHTTP.responseText, "page cannot be found") Then
                GetApplicationVariables = "Error: Inventory Manager returned an error.  Please contact your system administrator. " & vbLf & oHTTP.responseText
            Else
                GetApplicationVariables = oHTTP.responseText
            End If
        End If
    End If
    Set oHTTP = Nothing
End Function

Public Function CheckboxValueFromText(sText As String) As Integer
    Dim sTest As String
    sTest = LCase(sText)
    If sTest = "true" Or sTest = "1" Then
        CheckboxValueFromText = vbChecked
    Else
        CheckboxValueFromText = vbUnchecked
    End If
End Function
        
Public Function BooleanToText(bValue As Boolean) As String
    If bValue Then
        BooleanToText = "true"
    Else
        BooleanToText = "false"
    End If
End Function

Public Function Protocol(bIsSSL) As String

    'CSBR-123147 jbattles 18-March-10
    'Check to see if checkbox is set, ignore what is passed in
    If (frmCFWDBWiz.chkUseSSL = vbChecked) Then
        Protocol = "https://"
    Else
        Protocol = "http://"
    End If
End Function

