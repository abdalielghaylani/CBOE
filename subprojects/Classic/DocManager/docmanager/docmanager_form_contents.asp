
<HTML>
<HEAD>
<TITLE>Document Form View</TITLE>
<!--#include file="./src/functions.asp"-->

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

</HEAD>

<body>
<%

dbkey = "docmanager"
formgroup = request("formgroup")

BaseID = CInt(Request.querystring("BaseID"))

colsToSelect = "DOCLOCATION, DOCNAME, TITLE, AUTHOR, SUBMITTER, SUBMITTER_COMMENTS, DATE_SUBMITTED"
colsToSelect = colsToSelect &  "," & Application("OPTIONAL_FIELDS")
Set BaseRSConn = GetConnection(dbkey, formgroup, "DOCMGR_DOCUMENTS")
'sql=GetDisplaySQL(dbkey, formgroup,"DOCMGR_DOCUMENTS.*","DOCMGR_DOCUMENTS", "", BaseID, "")
sql = "SELECT " & colsToSelect & " FROM DOCMGR_DOCUMENTS WHERE DOCID=" & BaseID
Set BaseRS = BaseRSConn.Execute(sql)
'Response.Write sql & "<br>"
sql = "Select * from DOCMGR_STRUCTURES where DOCMGR_STRUCTURES.docid=" & BaseID
'Response.Write sql
Set StructureRS = BaseRSConn.Execute(sql)

%>
<%'================== docmgr specific =======================
const ForReading = 1

Dim download
download = request("download")

Dim fso, subFolder, fileTempFolder, fileName
Set fso = CreateObject("Scripting.FileSystemObject")

if not fso.FolderExists(Session("sessionTempFolder")) then
	fso.CreateFolder(Session("sessionTempFolder"))
end if
			
subFolder = "at" & Month(Now) & Day(Now) & Hour(Now) & Minute(Now) & Second(Now)
fileTempFolder = fso.BuildPath(Session("sessionTempFolder"), subFolder)
fso.CreateFolder(fileTempFolder)

fileName = BaseRS("DOCNAME")
'Response.Write fileName

Dim fileFullPath, htmlFullPath, previewHtmlStr
fileFullPath = fso.BuildPath(fileTempFolder, fileName)

Dim blobHandler
Set blobHandler = Server.CreateObject("Blob.Handler")
'Response.Write Application("ORA_SERVICENAME") & "/" & Session("UserName" & dbkey) & "/" & Session("UserID" & dbkey)
'Response.end
blobHandler.SaveBlobToFile Application("ORA_SERVICENAME"), Session("UserName" & dbkey) & "/" & Session("UserID" & dbkey), "SELECT * FROM DOCMGR_DOCUMENTS WHERE DOCID = " & BaseID, "DOC", CStr(fileFullPath)
Set blobHandler = Nothing

if download = "true" then 'user want to download to local disk
	fp = Session("sessionTempFolder") & "\" & subFolder & "\" & fileName
	Response.Redirect "download.asp?filePath=" & fp & "&fileName=" & fileName
end if

'save .doc as .html
'Response.Write fileFullPath
'Response.end
viewHtmlStr = SaveFileAsHtml(fileFullPath)					
'===============================================================
%>

<%=viewHtmlStr%>
<%CloseRS(StructuresRS)%>
<%CloseRS(BaseRS)%>
<%CloseConn(BaseRSConn)%>


</body>
</html>

