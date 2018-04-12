Attribute VB_Name = "UtilsConn"
Option Explicit
'
'
'Public Conn As ADODB.Connection
'Public Cmd As ADODB.Command
'
'
'Public Function ExecuteStatement(ByVal sSQL As String) As ADODB.Recordset
'
'    Dim rs  As ADODB.Recordset
'    'Set rs = New ADODB.Recordset
'
'    ' Show Hourglass:
'    Screen.MousePointer = vbHourglass
'
'     On Error Resume Next
'    ' Open the ADODB.Recordset:
'    ' if there is an error, then simply set the ADODB.Recordset to nothing.
'    GetInvConnection
'    'rs.CursorLocation = adUseClient
'    'rs.Open sSQL, Conn
'    Set rs = Conn.Execute(sSQL)
'
'    If Err.Number <> 0 Then
'        'MsgBox Err.Description & ":" & Err.Number
'        Set rs = Nothing
'    End If
'
'    ' Clear Hourglass:
'    Screen.MousePointer = vbNormal
'
'    Set ExecuteStatement = rs
'End Function
'
'Public Sub GetInvConnection()
'    'SYAN rewrote this function on 7/26/2004 to fix CSBR-42143
'    'Get connection only when the connection does not exist.
'
'    Dim sConnStr
'
'    'sConnStr = "Provider=OraOLEDB.Oracle;" & _
'    '    "Server=" & msOraServiceName & ";" & _
'    '    "Database=" & msInvSchemaName & ";" & _
'    '    "User ID=" & msUserID & ";" & _
'    '    "Password=" & msPassword
'
'    sConnStr = "Provider=OraOLEDB.Oracle;" & _
'        "Data Source=" & msOraServiceName & ";" & _
'        "User ID=" & msUserID & ";" & _
'        "Password=" & msPassword
'
'    'SYAN tested with this connectionstring in order to make Oracle client uneccessary.
'    'Yet does not work for now. At this point invloader can only be run either on the server
'    'or on a machine with Oracle client and have service name mapped.
'    'sConnStr = "Provider=MS Remote;" & _
'    '       "Remote Server=http://" & msServerName & ";" & _
'    '       "Remote Provider=OraOLEDB.Oracle;" & _
'    '       "Data Source=" & msOraServiceName & ";" & _
'    '       "Database=" & msInvSchemaName & ";" & _
'    '       "User ID=" & msUserID & ";" & _
'    '       "Password=" & msPassword
'
'    If UCase(TypeName(Conn)) = "NOTHING" Then 'first connection
'        Set Conn = New ADODB.Connection
'
'        On Error Resume Next
'        Conn.open sConnStr
'
'    Else 'Connection created but somehow not connected.
'        If Conn.State = 0 Then
'            On Error Resume Next
'            Conn.open sConnStr
'        End If
'    End If
'
'    If Err.Number <> 0 Then
'        MsgBox "Can not connect to database -- " & Err.Description & ":" & Err.Number
'    End If
'
'End Sub
'
'Public Sub GetInvCommand(sCommandName As String, sCommandTypeEnum As Integer)
'    GetInvConnection
'    Set Cmd = New ADODB.Command
'    Set Cmd.ActiveConnection = Conn
'    Cmd.CommandText = sCommandName
'    Cmd.CommandType = sCommandTypeEnum
'End Sub
'
'Public Sub ExecuteCmd(sCommandName As String)
'    On Error Resume Next
'    Cmd.Execute
'    If Err Then
'        Select Case True
'            Case InStr(Err.Description, "ORA-20003") > 0
'            'Response.Write strID & ": Cannot execute invalid Oracle procedure."
'            Case InStr(Err.Description, "access violation") > 0
'            'Response.Write strID & ": Current user is not allowed to execute Oracle procedure."
'            Case Else
'            'Response.Write strID & ": " & Err.Description
'        End Select
'        'Response.End
'    End If
'End Sub
'Public Function ExecSql(ByVal CommandText As String, Optional ByVal Host As String = "", Optional ByVal User As String = "", Optional ByVal Password As String = "", Optional ByVal Commit As Boolean = False) As String
'    Screen.MousePointer = vbHourglass
'
'    If Host = "" Then Host = msHost
'    ' try default host if still no host
'    If Host = "" Then Host = DEFAULT_HOST
'    If User = "" Then User = msUserID
'    If Password = "" Then Password = msPassword
'
'    Dim sReq As String
'    sReq = "commandtext=" & URLEncode(CommandText)
'    Dim xml As MSXML2.XMLHTTP
'    Set xml = New MSXML2.XMLHTTP
'    Dim surl As String, sBody As String
'    surl = Host & SQL_URL
'    sBody = "user=" & User & "&" & "password=" & Password & "&" & sReq
'    xml.open "POST", surl, False
'    xml.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
'On Error Resume Next
'    ' if server doesn't exist, send method will fail
'    xml.send sBody
'    Dim sRet As String
'    If Err.Number <> 0 Then
'        sRet = Err.Description
'        Err.Clear
'    Else
'        sRet = xml.responseText
'    End If
'
'    If sRet <> "" Then
'         Debug.Print sRet
'
'
'            Debug.Assert False
'        '  Beep
'    End If
'    ExecSql = sRet
'
'    ' DEBUG
'    If False Then
''          AppendTextToFile "C:\hts.log", CommandText
'    End If
'
'    Screen.MousePointer = vbNormal
'End Function
'



'
'Public Function DBLookupVal(ByVal table As String, ByVal LookupField As String, ByVal ValueField As String, ByVal LookupValue As Variant)
'    Dim rs As Recordset
'    If UCase(Left(table, 3)) = "INV" Then
'        table = "CHEMINVDB2." & table
'    ElseIf UCase(Left(table, 3)) = "HTS" Then
'        table = "BIOASSAYHTS." & table
'    End If
'    Set rs = ExecuteStatement("select " & ValueField & " from " & table & " where " & LookupField & " = " & SQLQuote(LookupValue))
'    If RSHasRecords(rs) Then
'        DBLookupVal = CnvLong(rs(ValueField), eDBtoVB)
'    Else
'        DBLookupVal = NULL_AS_LONG
'    End If
'End Function
'
