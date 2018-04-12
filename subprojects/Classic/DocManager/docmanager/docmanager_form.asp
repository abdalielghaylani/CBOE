<%@ LANGUAGE=VBScript  %>
<%response.expires = 0%>
<%'Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved
Server.ScriptTimeout = 1200

dbkey = Request("dbname")

if Not Session("UserValidated" & dbkey) = 1 then
	response.redirect "../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if

%><!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<script language = "javascript">
	var doPluginDetect = false
</script>

<!--#include file="./src/functions.asp"-->

<!--#INCLUDE VIRTUAL="/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->

<!--#INCLUDE VIRTUAL="/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL="/cfserverasp/source/recordset_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->

<%
'ShowMessageDialog( "BaseID (docmanager_form) = '" & BaseID & "'" )

Set BaseRSConn = GetConnection(dbkey, formgroup, "DOCMGR_DOCUMENTS")
'sql=GetDisplaySQL(dbkey, formgroup,"DOCMGR_DOCUMENTS.*","DOCMGR_DOCUMENTS", "", BaseID, "")
sql = "SELECT DOCLOCATION, DOCNAME, TITLE, AUTHOR, SUBMITTER, SUBMITTER_COMMENTS, DATE_SUBMITTED FROM DOCMGR_DOCUMENTS WHERE DOCID=" & BaseID
Set BaseRS = BaseRSConn.Execute(sql)
'ShowMessageDialog( "sql = '" & sql & "'" )
sql = "Select * from DOCMGR_STRUCTURES where DOCMGR_STRUCTURES.docid=" & BaseID
Set StructureRS = BaseRSConn.Execute(sql)


'================== docmgr specific =======================
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

Dim fileFullPath, htmlFullPath, previewHtmlStr
fileFullPath = fso.BuildPath(fileTempFolder, fileName)

'stop
Dim ret
ret = Application("blobHandler").SaveBlobToFile(Session("UserName" & dbkey) & "/" & Session("UserID" & dbkey), "SELECT * FROM DOCMGR_DOCUMENTS WHERE DOCID = " & BaseID, "DOC", CStr(fileFullPath))

if Instr(ret, "ERROR:") > 0 then
	Response.Write ret 'pass on the error
else
	if download = "true" then 'user want to download to local disk
		'if InStr(Request.ServerVariables("HTTP_USER_AGENT"), "MSIE 5.5") then
			'this is a work around of a IE5.5 bug that save file to disk doesn't work
	%>
			<!--script language="javascript">
				//alert('In order to download the document to disk, you need to upgrade to IE 6.0.');
			</script-->
					
	<%	'else
			fp = Session("sessionTempFolder") & "\" & subFolder & "\" & fileName
			'Response.Write Server.URLEncode("download.asp?filePath=" & fp & "&fileName=" & fileName)
			'Response.end
			Response.Redirect Replace("download.asp?filePath=" & fp & "&fileName=" & fileName, " ", "%20")
		'end if
	end if

	'save .doc as .html
	'Response.Write fileFullPath
	'Response.end
	htmlFullPath = SaveFileAsHtml(fileFullPath)
	'===============================================================

	' Set up a session variable for this as the current page displayed.  Tack on "keyparent".
	sCurrentLocation = Session( "CurrentLocation" & dbkey & formgroup ) & "&BaseID=" & BaseID
	if 1 < Len( download ) then
		sCurrentLocation = sCurrentLocation & "&download=" & download
	end if
	Session( "ReturnToCurrentPage" & dbkey ) = sCurrentLocation
	Session( "ReturnToDocumentDetails" & dbkey ) = Session( "ReturnToCurrentPage" & dbkey )

	CloseRS( StructuresRS )
	CloseRS( BaseRS )
	CloseConn( BaseRSConn )

	sContentsPath = Replace( htmlFullPath, " ", "%20" )
	'ShowMessageDialog( "sContentsPath = '" & sContentsPath & "'" )
	%>

	<title>Document Manager - Results Form View</title>
	</head>
	<%'javascript:goFormView('/docmanager/docmanager/docmanager_form.asp?formgroup=base_form_group&dbname=docmanager&formmode=edit&unique_id=2&commit_type=', 2)%>

	<%if Application("SHOW_EXTERNAL_LINKS") then%>
				
		<%framequerystring = Request.QueryString
		if instr(1,framequerystring,"unique_id")< 1 then
			framequerystring  = framequerystring & "&unique_id=" & baseid
		end if
			
		'Response.write framequerystring
		%>
			
		<%if Request.QueryString("showpage")="links" then%>
				
			<%framequerystring = replace(framequerystring,"showpage=links","showpage=contents")%>
				
			<frameset rows="50,500,*" border="0">
				<frame name="subnav" src="docmanager_form_nav.asp?<%=framequerystring%>">
				<frame name="externallinks" src="external_links.asp?<%=framequerystring%>">	
			</frameset>
				
		<%else%>	
				
			<%
				If Request.QueryString("showpage")="contents" then
					framequerystring = replace(framequerystring,"showpage=contents","showpage=links")
						
				Else
					framequerystring = framequerystring & "&showpage=links"
					'framequerystring = Request.QueryString
					'Response.Write framequerystring
					'response.end
						
				End If
			%>
										
			<frameset rows="50,500,*" border="0">
				<frame name="subnav" src="docmanager_form_nav.asp?<%=framequerystring%>">
				<frame name="contents" src="<%=sContentsPath%>">
			</frameset>
				
		<%end if%>	
			
	<%else%>

		<frameset rows="500,*" border="0">
			<frame name="contents" src="<%=sContentsPath%>">
		</frameset>

	<%end if%>

	<!---JHS commented out 4/4/03
		<frameset rows="500,*" border="0">
			<frame name="contents" src="<%=sContentsPath%>">
		</frameset>
	--->
<%end if%>
</html>
