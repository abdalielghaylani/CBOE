<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/ContainerSQL.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim PrinterRS
Dim ContainerRS
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID
Dim ContainerID
Dim LabelPrinterID
Dim NumCopies

bDebugPrint=true

Dim oWSHShell
Dim oReplaceStrings
Dim oFileSystem
Dim TempFolder
Dim TempName
Dim TempFile
Dim strPath

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

Response.Expires = -1

bDebugPrint = False
bWriteError = False
strError = "Error:PrintLPRLabel<BR>"
ContainerID = Request("ContainerID")
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

' Check for required parameters
If IsEmpty(ContainerID) then
	strError = strError & "ContainerID must be specified.<BR>"
	bWriteError = True
End if

If IsEmpty(LabelPrinterID) then
	strError = strError & "LabelPrinterID must be specified.<BR>"
	bWriteError = True
End if

If IsEmpty(NumCopies) then
    NumCopies = 1
end if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

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
        Set oFileSystem = Server.CreateObject("Scripting.FileSystemObject") 
        Set TempFolder = oFileSystem.getFolder("h:\LabelFiles")
        TempName = oFileSystem.GetTempName
        strPath = TempFolder.path & "\" & TempName
        Set TempFile = TempFolder.CreateTextFile(TempName) 


	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set PrinterRS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (PrinterRS.EOF AND PrinterRS.BOF) then
		'DFCI Add multi select print
		for each item in Split(ContainerId,",")
	
		set oReplaceStrings = Server.CreateObject("Scripting.Dictionary")


        ' This SQL is generated within ContainerSQL.asp
        SQL2 = SQL & " AND inv_containers.container_id = " & item ' DFCI Change containerid to item, SQL to SQL2	
		Cmd.CommandText = SQL2 ' DFCI Change SQL to SQL2
		Cmd.CommandType = adCmdText
        set ContainerRS = Cmd.Execute
        
        ' Add all of the available container fields to the dictionary so we can replace any references
        ' to them in the source files
        for each field in ContainerRS.Fields
            if not isnull(field.value) then
                oReplaceStrings.Add "$" & field.name & "$", field.value                
            else
                oReplaceStrings.Add "$" & field.name & "$", ""                
            end if
        next
        
        ' Set the number of copies
        oReplaceStrings.Add "$NUM_COPIES$", NumCopies        
        
        strHeaderFile = PrinterRS.Fields("header_filename").value
        strLabelFile = PrinterRS.Fields("label_filename").value
        strFooterFile = PrinterRS.Fields("footer_filename").value        
        
        
        ' Go load the header file, if there
        if not isnull(strHeaderFile) then
            call CopyFileContents( oFileSystem, strHeaderFile, TempFile, oReplaceStrings )
        end if
        
        call CopyFileContents( oFileSystem, strLabelFile, TempFile, oReplaceStrings )
        
        ' Go load the footer file, if there
        if not isnull(strFooterFile) then
            call CopyFileContents( oFileSystem, strFooterFile, TempFile, oReplaceStrings )
        end if        

      'on error resume next        
  
	'pjd add lpr path
  '    	Set oWSHShell = CreateObject("WScript.Shell")

	If Err Then
            strError = strError & CStr(Err.Number) & " " & Err.Description & "   strPath=" & strPath
            Err.Clear
            Response.Write strError
	        'Response.end
        end if
        
        set oReplaceStrings = nothing	

	next


     	Set oWSHShell = CreateObject("WScript.Shell")
        oWSHShell.exec "H:\Inetpub\Wwwroot\ChemOffice\ChemInv\config\xml_templates\lpr.exe -S " & PrinterRS.Fields("server_name") & " -P " & PrinterRS.Fields("queue_name") & " " & strPath	

        Set TempFile = nothing
        Set TempName = nothing
	Set oWSHShell = nothing
        Set TempFolder = nothing
	set oFileSystem = nothing

 	
	If Err Then
            strError = strError & CStr(Err.Number) & " " & Err.Description & "   strPath=" & strPath
            Err.Clear
            Response.Write strError
'	        'Response.end
        end if
	end if
end if


</SCRIPT>
