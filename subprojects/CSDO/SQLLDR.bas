Attribute VB_Name = "SQLLDR"
Option Explicit

' contains functions for dealing with the command line program
' SQLLDR, which bulk loads stuff into an Oracle database in an
' expeditious fashion

' note that for this to work, CSDO and Oracle must be running
' on the same machine

Public Function RunSQLLdr(ByRef conn As ADODB.Connection, _
                   Optional uname As String = "scott", _
                   Optional pwd As String = "tiger", _
                   Optional tempTableName As String = "CS_TEMP", _
                   Optional batchPath As String = "C:\InetPub\wwwroot\ChemOffice\chem_reg\logfiles\frontloader.bat", _
                   Optional sqlLdrPath As String = "D:\Orant\bin\sqlldr80.exe", _
                   Optional ServiceName As String = "", _
                   Optional ExportIDsPath As String = "C:\InetPub\wwwroot\ChemOffice\chem_reg\logfiles\exportids.dat", _
                   Optional ExportCtlPath As String = "C:\InetPub\wwwroot\ChemOffice\chem_reg\logfiles\exportids.ctl", _
                   Optional logPath As String = "C:\InetPub\wwwroot\ChemOffice\chem_reg\logfiles\sqlldr.log" _
                   ) As Boolean
                   
    ' create the ctl file
On Error Resume Next
    Kill ExportCtlPath
On Error GoTo 0
    Open ExportCtlPath For Append Access Write As #1
    Dim lineStr As String
    lineStr = "load data"
    Print #1, lineStr
    lineStr = "preserve blanks"
    Print #1, lineStr
    lineStr = "into table " & tempTableName & " append"
    Print #1, lineStr
    lineStr = "fields terminated by '|'"
    Print #1, lineStr
    lineStr = "TRAILING NULLCOLS"
    Print #1, lineStr
    lineStr = "("
    Print #1, lineStr
    lineStr = "   ID"
    Print #1, lineStr
    lineStr = ")"
    Print #1, lineStr
    Close #1
    
    ' create the batch file
On Error Resume Next
    Kill batchPath
On Error GoTo 0
    Open batchPath For Append Access Write As #1
    Dim batchCmd As String
    If ServiceName <> "" Then
         batchCmd = sqlLdrPath & " userid=" & uname & "/" & pwd & "@" & ServiceName & _
        " control=" & ExportCtlPath & " data=" & ExportIDsPath & _
        " skip=1 direct=true > " & logPath  ' skip param skips MOL_ID in id file
    Else
         batchCmd = sqlLdrPath & " userid=" & uname & "/" & pwd & _
        " control=" & ExportCtlPath & " data=" & ExportIDsPath & _
        " skip=1 direct=true > " & logPath  ' skip param skips MOL_ID in id file
    End If
    Print #1, batchCmd
    Close #1

    ' run sql commands to do table load
    Dim cmd As ADODB.Command
    Set cmd = New ADODB.Command
    Dim rs As ADODB.Recordset
    With cmd
        Set .ActiveConnection = conn
        .CommandType = adCmdText
        On Error Resume Next
        .CommandText = "drop table " & tempTableName
        .Execute
        On Error GoTo 0
        .CommandText = "create table " & tempTableName & " (ID NUMBER(7))"
        .Execute
    End With
    
    ' runs the sqlldr program
    SyncShell batchPath
    
    With cmd
        .CommandText = "commit"
        .Execute
    End With

    With cmd
        .CommandText = "create index ID_INDX" & tempTableName & " on " & tempTableName & " (ID)"
        .Execute
    End With
    
    
    ' might do some logfile checking here at some point
    Set cmd = Nothing
    Set rs = Nothing
    RunSQLLdr = True
    
End Function
