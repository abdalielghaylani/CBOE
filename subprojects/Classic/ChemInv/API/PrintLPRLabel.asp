<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%

Dim Conn
Dim Cmd
Dim PrinterRS
Dim ObjectRS
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID
Dim PrimaryKeyID
Dim LabelPrinterID
Dim ReportTypeID
Dim NumCopies
Dim aPrimaryKeyList

Response.Expires = -1

bDebugPrint = False
bWriteError = False
strError = "Error:PrintLPRLabel<BR>"
PrimaryKeyID = Request("PrimaryKeyID")
LabelPrinterID = Request("LabelPrinterID")
NumCopies = Request("NumCopies")
InvSchema = Application("CHEMINV_USERNAME")
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
    Response.Redirect "/cheminv/help/admin/api/PrintLPRLabel.htm"
    Response.end
End if

If IsEmpty(LabelPrinterID) then
    strError = strError & "LabelPrinterID must be specified.<BR>"
    bWriteError = true
end if

If IsEmpty(PrimaryKeyID) then
    strError = strError & "PrimaryKeyID must be specified.<BR>"
    bWriteError = true
end if

If bWriteError then
    ' Respond with Error
    Response.Write strError
    Response.end
End if

If IsEmpty(NumCopies) then
    NumCopies = 1
end if

GetInvConnection()
Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETLABELPRINTER", 4)
Cmd.Parameters.Append Cmd.CreateParameter("LABEL_PRINTER_ID", adInteger, adParamInput, 4, LabelPrinterID)

if bdebugPrint then
    Response.Write "Parameters:<BR>"
    For each p in Cmd.Parameters
        Response.Write p.name & " = " & p.value & "<BR>"
    Next
    Response.end
else
    Cmd.Properties ("PLSQLRSet") = TRUE  
    Set PrinterRS = Cmd.Execute
    Cmd.Properties ("PLSQLRSet") = FALSE
    if NOT (PrinterRS.EOF AND PrinterRS.BOF) then
        
        ReportTypeID = CLng(PrinterRS.Fields("reporttype_id_fk").value)
        call PrintLabels( Conn, PrinterRS, ReportTypeID, PrimaryKeyID, NumCopies )
        If Err Then
            strError = strError & CStr(Err.Number) & " (PrintLabels) " & Err.Description  & " " & cmdline          
            Err.Clear
            Response.Write strError
            Response.end
        End If
    end if
end if

Conn.Close()
set Cmd = nothing
set Conn = nothing


function ConvertZPLCharacters( strSource )
    
    strReturn = replace( strSource, "^", " " )    ' ^ is the escape sequence in ZPL
    ConvertZPLCharacters = strReturn

end function


sub PrintLabels( pConn, pPrinterRS, pReportTypeID, pPrimaryKeyID, pNumCopies )
    Dim oWSHShell
    Dim IDparm
    Dim oReplaceStrings
    Dim oFileSystem
    Dim TempFolder
    Dim TempName
    Dim TempFile
    Dim strPath
    Dim aPrimaryKeyList
    Dim cmdline
    Dim Cmd
    
    Set Cmd = Server.CreateObject("ADODB.Command")
    Cmd.ActiveConnection = pConn
    Cmd.CommandType = adCmdText
    
    select case (pReportTypeID)
        case 1  ' Container label                
%>
            <!--#INCLUDE VIRTUAL = "/cheminv/gui/ContainerSQL.asp"-->
<%
            SQL = SQL & " AND inv_containers.container_id = :ID"
        
        case 7  ' Plate label
%>
            <!--#INCLUDE VIRTUAL = "/cheminv/gui/PlateSQL.asp"-->
<%
            SQL = SQL & " p.plate_id = :ID"
        
        case 9  ' Location label
%>
            <!--#INCLUDE VIRTUAL = "/cheminv/gui/LocationSQL.asp"-->
<%
            SQL = SQL & " and inv_locations.location_id = :ID"               
    end select    
    
    on error resume next
    Cmd.CommandText = SQL
    Cmd.CommandType = adCmdText
    Set IDparm = Cmd.CreateParameter("ID", adInteger, adParamInput)
    Cmd.Parameters.Append IDparm
    If Err Then
        strError = strError & " (command def) " & CStr(Err.Number) & " " & Err.Description
        Err.Clear
        Response.Write strError
        Response.end
    end if
    on error goto 0
        
    ' Set the number of copies replacement string to 1.  Older version put the copy count in ZPL,
    ' so this allows old installations to work correctly. 
    'oReplaceStrings.Add "$NUM_COPIES$", 1        
    
    strHeaderFile = pPrinterRS.Fields("header_filename").value
    strLabelFile = pPrinterRS.Fields("label_filename").value
    strFooterFile = pPrinterRS.Fields("footer_filename").value        
    
    Set oFileSystem = Server.CreateObject("Scripting.FileSystemObject") 
    
    Set TempFolder = oFileSystem.GetSpecialFolder(2)      ' 2 = TemporaryFolder
    TempName = oFileSystem.GetTempName
    strPath = TempFolder.Path & "\" & TempName
    Set TempFile = TempFolder.CreateTextFile(TempName) 
    
    ' Process the ID list
    aPrimaryKeyList = split(pPrimaryKeyID,",")
    for i = 0 to ubound(aPrimaryKeyList)
    ' Get the recordset
        on error resume next
        IDparm.Value = aPrimaryKeyList(i)   
        set ObjectRS = Cmd.Execute
        If Err Then
            strError = strError & " (command exec) " & CStr(Err.Number) & " " & Err.Description
            Err.Clear
            Response.Write strError
            Response.end
        end if
        on error goto 0
   
    ' Add all of the available object fields to the dictionary so we can replace any references
    ' to them in the source files
        set oReplaceStrings = Server.CreateObject("Scripting.Dictionary")
        for each field in ObjectRS.Fields
            if not isnull(field.value) then
                oReplaceStrings.Add "$" & field.name & "$", ConvertZPLCharacters( CStr(field.value) )
            else
                oReplaceStrings.Add "$" & field.name & "$", ""                
            end if
        next
        
    ' Go load the header file, if there
        if not isnull(strHeaderFile) then
            call CopyFileContents( oFileSystem, strHeaderFile, TempFile, oReplaceStrings )
        end if

    ' Go load the labels
        for j = 1 to pNumCopies
            call CopyFileContents( oFileSystem, strLabelFile, TempFile, oReplaceStrings )
        next

    ' Go load the footer file, if there
        if not isnull(strFooterFile) then
            call CopyFileContents( oFileSystem, strFooterFile, TempFile, oReplaceStrings )
        end if

    ' Cleanup
        set oReplaceStrings = Nothing
        set ObjectRS = Nothing
     next
    
    on error resume next
    Set oWSHShell = CreateObject("WScript.Shell")
    
	
	cmdline = "lpr.exe -S " & PrinterRS.Fields("server_name") & " -P " & PrinterRS.Fields("queue_name") & " " & strPath
    oWSHShell.exec(cmdline)
    If Err Then
        strError = strError & CStr(Err.Number) & " (Shell) " & Err.Description  & " " & cmdline          
        Err.Clear
        Response.Write strError
        Response.end
 ' For debugging
 '    Else
 '        strError = "No Error: " & cmdline          
 '        Err.Clear
 '        Response.Write strError
 '        Response.end
    End If
    on error goto 0
    
    Set oWSHShell = nothing
    Set TempFile = nothing
    Set TempFolder = nothing
    Set TempName = nothing
    set oFileSystem = nothing
    set oReplaceStrings = nothing
end sub

sub CopyFileContents( oFS, strFileName, oTempFile, oReplaceStrings )
    Dim oSourceFile
    Dim strLine
    Dim key
    
    set oSourceFile = oFS.OpenTextFile( strFileName, 1, True )        '1 = ForReading
    While not (oSourceFile.AtEndofStream)
        strLine = oSourceFile.readline
        for each key in oReplaceStrings
            strLine = Replace( strLine, key, oReplaceStrings(key) )
        next
        oTempFile.WriteLine strLine
    wend
    set oSourceFile = nothing
end sub

%>
