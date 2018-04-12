'//////////////////////////////////////////////////////////////////////
'/ 
'/  Name:            WriteBase64ToTable.vbs
'/  Author:          David Gosalvez
'/  
'/  Bug Reports &
'/  Enhancement Req: dgosalvez@cambridgesoft.com
'/
'/  Version:         1.0
'/
'/  Revision History:
'/        08/24/02 - 1.0 - inital version (dgb)
'/
'/  Requires: ChemDraw > 6.0, MolServer 7.0 and Base64Decode.dll. 
'/   
'/  This script will read structures from either an mst file or a collection of ChemDraw 
'/  readable files and update a database table with the base64cdx representation of the  
'/  chemical structures.
'/
'/  A database table must exist which contains a numerical key field and a large
'/  enough text field to store the base64cdx data.
'/
'/  When using and mst file as a structure source, the values of the key field 
'/  in the database will be matched to the MOL_ID values in the mst file.
'/
'/  When using a collection of files as a structure source, the values of the structure key 
'/  field in the database will be matched to the file names withot extension.  
'/  The extension must be readable by ChemDraw as a chemical structure file.
'/
'/
'/  USAGE:  Modify the values in the PARAMETERS Section of the script to match your
'/  source and destination database.  Doubleclick the script from the windows explorer. 
'/
'/  The script produces a log file which reports both success and error conditions for each
'/  structure processed.
'/
'/  This script uses tracelogerror.vbs script by Darwin Sanoy for all login and
'/  tracing operations.  For description and license information see below.
'//////////////////////////////////////////////////////////////////////


'//////////////////////////////////////////////////////////////////////
'/                             PARAMETERS
'/////////////////////////////////////////////////////////////////////
' Destination Database input parameters
ConStr = "dsn=chemacxdb;uid=chemacxdb;pwd=oracle"
' connStr can be any ADO compliant connection string to the database

TableName = "chemacxdb.substance" 

PkFieldName = "MOL_ID"

b64FieldName = "BASE64_CDX"

sLogFileName = "c:\WriteBase64ToTable_log.txt"
' If SLogFileName is blank a randon name will be used to write a file to the users temp folder

StructureSource = "MST"
' StructureSource has the following possible values:
' MST	 - Structures are read from an mst file
' FOLDER - Structures are read from a collection of ChemDraw readable files in a folder
   
' Used only when StructureSource is  MST
SourcePath = "c:\chemoffice_data\chemacx\"
SourceName = "chemacx.mst"
    
' Used only when StructureSource is FILES
FolderPath = "C:\mol\"
FileExtension = ".mol"  



'//////////////////////////////////////////////////////////////////////
'/                                       TRACE PARAMETERS
'/

' Constants for tracing routine.
Const VERBOSEMSG 		= 5
Const INFOMSG 			= 10
Const ERRORMSG 			= 30
Const NOMSG			= 100
sTraceLevel = ERRORMSG
' Trace level can be set to any numeric value.  It determines the threshold at
' which a message will be displayed.  It can also be set by retrieving an 
' environment variable (must customize this script) so that tracing can be
' controlled without editing script code.

msgAction = "LOG"
' msgAction has the following possible values:
'   INTERACTIVE	- all error and tracing messages are displayed interactively
'   LOG			- all error messages are logged to a file
'   BOTH		- all error messages are displayed and logged
'   NONE		- do not generate any messages - it is better to set sTraceLevel
'				  to 100 (or constant NOMSG)


bLogFileOverwrite = True


'//////////////////////////////////////////////////////////////////////
'/                                 GLOBAL CODE
'/
'/
Dim FileSetup, oLogFileObject    
Dim oFS
Dim oCDApp 
Dim oDecoder 
Dim	RS
Dim i 		
Dim SMsDoc

i = 0
Set oFs = CreateObject("Scripting.FileSystemObject")
Set oCDApp = CreateObject("ChemDraw.Application")
oCDApp.visible = false
Set oDecoder = CreateObject("Base64Decode.Decode")
Set conn = CreateObject("ADODB.Connection")
Set RS = CreateObject("ADODB.Recordset")    
      



conn.Open ConStr
Set RS = CreateObject("ADODB.Recordset")  
Set SMsDoc = CreateObject("MolServer9.Document")
SMsDoc.Open sourcePath & sourceName, 1, ""
msgbox SMsDoc.Count & "structures in mst"	    
		
GetMolesFromMst()    



'//////////////////////////////////////////////////////////////////////
'/
'/
'/    
function GetBase64fromMst(molid)
	'msgbox molid
	Set oMol = SMsDoc.GetMol(molid)
	on error resume next
	oMol.write(sourcePath & "\temp.cdx")        
	if err.number then
		Tracelogerror ERRORMSG, "GetBase64fromMst::", "Failed to get moleclue with molid= " & molid
		GetBase64fromMst = Null
	else
		GetBase64fromMst = oDecoder.Encode(sourcePath & "\temp.cdx")
	End if
End function


Sub GetMolesFromMst()
	
	sql = "SELECT " & PkFieldName & ", " & b64FieldName & " FROM " & TableName & " WHERE " & PkFieldName & " >0 AND "  & b64FieldName & " is null order by mol_id asc"	
	'msgbox sql
	RS.Open sql, conn, 2, 3, 1
	If Err.Number Then
		Tracelogerror ERRORMSG, "WriteBase64ToTable", "Error 0x" & CStr(Hex(Err.Number)) & err.description & " SQL=" & sql
		Err.clear
	Else
		If Not (RS.EOF And RS.BOF) Then
        	While NOT RS.EOF
        		sbase64 = GetBase64fromMst(RS(pkFieldName).value)
			'msgbox sbase64
			RS(b64FieldName) = sBase64
        		RS.Update
        		Tracelogerror INFOMSG, "WriteBase64ToTable", "Updated Structure with " & PkFieldName & "= " & pKValue 
			i = i + 1
			if i MOD 500 = 0 then Tracelogerror ERRORMSG, "GetMolesFromMst::", i & " Records processed."
			RS.MoveNext
		Wend	    
		else
			msgbox("No records found")    		

		End If
    		RS.Close
	End If

End sub

Sub GetMolsFromFolder(FolderPath, FileExtension)
 Set ColFiles = GetFilesCollection(FolderPath)
 
 For Each File In ColFiles
    fnLenght = Len(File.Name)
    fndot = InStr(1, File.Name, ".")
    fnExt = Mid(File.Name, fndot, fnLenght - fndot + 1)
    fileName = Left(File.Name, fndot - 1)
    If lcase(fnExt) = lcase(FileExtension) Then
	fullFileName = FolderPath & fileName & FileExtension
        Set oCDDoc = oCDApp.Documents.Open(fullFileName)
        If oCDApp.Documents.Count = 1 Then
            oCDDoc.SaveAs (FolderPath & "temp.cdx")
            oCDDoc.Close
			WriteBase64ToTable FolderPath & "temp.cdx", fileName 
			If Err.Number Then
				Tracelogerror ERRORMSG, "GetMolsFromFolder", "Error 0x" & CStr(Hex(Err.Number)) & err.description
			End If		 
        Else
             Tracelogerror INFOMSG, "GetMolsFromFolder" ,"Error: ChemDraw could not open file:" & fullFileName
        End If
    End If
 Next  
End Sub

Sub WriteBase64ToTable(PathtoMolecule, PkValue)
	sBase64 = oDecoder.Encode(PathtoMolecule)
	sql = "SELECT " & PkFieldName & ", " & b64FieldName & " FROM " & TableName & " WHERE " & PkFieldName & " =" & PkValue	
	Tracelogerror VERBOSEMSG, "WriteBase64ToTable", sql
        on error resume next		
	RS.Open sql, conn, 2, 3, 1
	If Err.Number Then
		Tracelogerror ERRORMSG, "WriteBase64ToTable", "Error 0x" & CStr(Hex(Err.Number)) & err.description & " SQL=" & sql
		Err.clear
	Else
		If Not (RS.EOF And RS.BOF) Then
        		RS.MoveFirst
        		RS(b64FieldName) = sBase64
        		RS.Update
        		Tracelogerror INFOMSG, "WriteBase64ToTable", "Updated Structure with " & PkFieldName & "= " & pKValue 
			i = i + 1    
		Else
			Tracelogerror ERRORMSG, "WriteBase64ToTable", "No match found for " & PkFieldName & "= " & pKValue 
    		End If
    		RS.Close
	End If
End Sub

Function GetFilesCollection(ByVal strFolderPath)
    Set oFs = CreateObject("Scripting.FileSystemObject")
    Set oFolder = oFs.GetFolder(strFolderPath)
    Set GetFilesCollection = oFolder.Files
    Set oFs = Nothing
    Set oFolder = Nothing
End Function

'////////////////////////////////////////////////////////////////////
'/
'/  Name:            tracelogerror.vbs
'/  Author:          Darwin Sanoy
'/  Updates:         http://www.desktopengineer.com
'/  Bug Reports &
'   Enhancement Req: Darwin@DesktopEngineer.com
'/
'/  Built/Tested On: Windows XP
'/  Requires:        OS: Any
'/                   
'/  Main Function:   Allows interactive and log based tracing and error reporting
'/                   for any program.  Can be used for interactive debugging, permanent
'/                   debug logging and reporting of errors to users.
'/ 
'/  Syntax:          all variables configured in the script
'/
'/  Usage and Notes: *) Download VPNRASLOGONHOOK (http://desktopengineer.com/vpnraslogonhook)
'/                      to see a practical implementation of this routine.
'/
'/  Assumptions &    *) Although this routine can be used for many purposes, it probably
'/  Limitations:        cannot be effectively used for more than one purpose at a time.
'/                      Effectively it can be used for debugging during development and then
'/                      configured for error logging, verbose logging or user error reporting
'/                      when the script is used in production.
'/
'/  Documentation:   http://desktopengineer.com/
'/
'/  License:         GNU General Public License see "license.txt" or 
'/                   http://www.gnu.org/copyleft/gpl.html
'/
'/  Version:         1.1
'/
'/  Revision History:
'/        10/20/01 - 1.1 - inital version (djs)
'/
'////////////////////////////////////////////////////////////////////

'//////////////////////////////////////////////////////////////////////
'  Setup for Tracing, Logging and Error Reporting
'//////////////////////////////////////////////////////////////////////


'//////////////////////////////////////////////////////////////////////
'  TraceandLogError
'//////////////////////////////////////////////////////////////////////
Sub TraceLogError(nLevel, sCodeLocation, sText)

    if sTraceLevel <= nLevel Then 
    	
    	LogLine =  Now & "  Message Occurred in: " & sCodeLocation & _
    			vbNewline & "   Severity: " & nLevel & _
    			vbNewline & "   Message: " & sText
    	
    	If msgAction = "INTERACTIVE" or msgAction = "BOTH" Then
    		wscript.echo LogLine
    	End If

    	If msgAction = "LOG" or msgAction = "BOTH" Then
    		If FileSetup <> True Then 
    			SetupFileLogging sLogFileName
    		End If
    		oLogFileObject.writeline LogLine
    	End If
    End If
End Sub

'//////////////////////////////////////////////////////////////////////
'  Log File Setup (required for tracelogerror
'//////////////////////////////////////////////////////////////////////
Sub SetupFileLogging(sFilename)
' Could be removed if file logging will never be used
	Set oFSO = CreateObject("Scripting.FileSystemObject")
	sTempFolder = oFSO.GetSpecialFolder(2)
	
	If sFilename <> "" Then
		'sTempName = oFSO.GetFileName(sFilename)
		'sLogFileName = sTempFolder & "\" & sTempName
	Else
		'Generate random name
		sTempName = oFSO.GetFileName(oFSO.GetTempName)
		sLogFileName = sTempFolder & "\" & lcase(left(sTempName, InstrRev(sTempName,".")-1)) & ".log"
	End If
	
	If oFSO.FileExists(sLogFileName) and not bLogFileOverwrite Then
		Set oLogFileObject = oFSO.OpenTextFile(sLogFileName, 8, False)
	Else
		Set oLogFileObject = oFSO.OpenTextFile(sLogFileName, 2, True)
	End If
	oLogFileObject.writeline Now & "  Logging Started"
	
	FileSetup = True
	
End Sub
