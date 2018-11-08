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
                   Optional logPath As String = "C:\InetPub\wwwroot\ChemOffice\chem_reg\logfiles\sqlldr.log", _
                   Optional directKeyword As Long = "1" _
                   ) As Boolean
                   
    ' create the ctl file
On Error Resume Next
    Kill ExportCtlPath
On Error GoTo 0
    Dim fileNum As Integer
    
    fileNum = FreeFile
    Open ExportCtlPath For Append Access Write As fileNum
    Print #fileNum, "load data"
    Print #fileNum, "preserve blanks"
    Print #fileNum, "into table " & tempTableName & " append"
    Print #fileNum, "fields terminated by '|'"
    Print #fileNum, "TRAILING NULLCOLS"
    Print #fileNum, "("
    Print #fileNum, "   ID"
    Print #fileNum, ")"
    Close #fileNum
    
    ' create the batch file
On Error Resume Next
    Kill batchPath
On Error GoTo 0
    fileNum = FreeFile
    Open batchPath For Append Access Write As fileNum
    
    ' Create the correct batchCmd and print it to the batchPath.
    Dim batchCmd As String
    batchCmd = sqlLdrPath & " userid=" & uname & "/" & pwd
    
    If (ServiceName <> "") Then
        batchCmd = batchCmd & "@" & ServiceName
    End If
    
    batchCmd = batchCmd & _
        " control=" & ExportCtlPath & _
        " data=" & ExportIDsPath & _
        " skip=1"
     '!LJB! 10/02/01 Add Direct Keyword support
    Select Case directKeyword
        Case 0
             batchCmd = batchCmd & " direct=false"
        Case 1
             batchCmd = batchCmd & " direct=true"
        Case 2
             'direct keyword not supported at all. no change to batchCmd
        Case Else
             batchCmd = batchCmd & " direct=true" ' this is the default if nothing is specified
    End Select
   
    
    batchCmd = batchCmd & " > " & logPath  ' skip param skips MOL_ID in id file
    
    Print #fileNum, batchCmd
    Close #fileNum

    ' run sql commands to do table load
    Dim cmd As ADODB.Command
    Set cmd = New ADODB.Command
    With cmd
        Set .ActiveConnection = conn
        .CommandType = adCmdText
        On Error Resume Next
        .CommandText = "drop table " & tempTableName
        .Execute
         On Error GoTo 0
        .CommandText = "create table " & tempTableName & " (ID NUMBER(7))"
        .Execute
        .CommandText = "commit"
        .Execute
        
    End With
    
    ' runs the sqlldr program
    SyncShell batchPath
    On Error Resume Next
    
    '!LJB! 10/02/01 Don't index the temptable
    'With cmd
    '     .CommandText = "create index ID_INDX" & Left(tempTableName, 15) & " on " & tempTableName & " (ID)"
    '    .Execute
    'End With
   
    ' might do some logfile checking here at some point
    Set cmd = Nothing
    RunSQLLdr = True
    
End Function


Private Function SupportsDirectKeyword(ByRef conn As ADODB.Connection) As Boolean
    ' return TRUE if we support the DIRECT keyword.
    Dim DBMSVersion As String
    Dim dbMSName As String
    
    On Error GoTo EndFunction
    
    SupportsDirectKeyword = False
            
    ' Here is an example Oracle DBMS Version property value: 08.00.0000 Oracle8 Enterprise Edition Release 8.0.4.0.0 - Production
    ' The supportDirectKeyword is true for oracle 8 or later.
    DBMSVersion = conn.Properties("DBMS Version").Value
    dbMSName = conn.Properties("DBMS Name").Value
    SupportsDirectKeyword = InStr(dbMSName, "Oracle") > 0 And _
                            CInt(Split(DBMSVersion, ".", Limit:=2)(0)) >= 8

EndFunction:

End Function


