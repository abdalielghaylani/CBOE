<%@ LANGUAGE="VBScript" %>

<%
'LogAction "I am here because " & Request.ServerVariables("HTTP_REFERER") & " wants me to"
response.expires = 0
'dbkey = Request("dbname")
dbkey = "docmanager"

if Not Session("UserValidated" & dbkey) = 1 then
	response.redirect "../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if

toBeDeletedBaseID = request("toBeDeletedBaseID")

'Set cnn = Server.CreateObject("ADODB.Connection")
'with cnn
'	.ConnectionString = Application("cnnStr")
'end with
'cnn.Open
Set BaseRSConn = GetConnection(dbkey, formgroup, "DOCMGR_DOCUMENTS")

if toBeDeletedBaseID <> "" then
	'Delete the doc given docid
	Set cmd = Server.CreateObject("ADODB.Command")

	with cmd
		.ActiveConnection = BaseRSConn
		'.ActiveConnection = cnn
		.CommandText = "DELETE FROM docmgr.DOCMGR_DOCUMENTS WHERE DOCID=" & toBeDeletedBaseID
		.CommandType = adCmdText
	end with
	'Response.Write cmd.CommandText
	'Response.end	
	cmd.Execute()	
end if
'cnn.Close

%>

<html>

<head>
<SCRIPT LANGUAGE="JavaScript1.1">
//SYAN added on 1/12/2005 to disable right-click on this page per S-P reports.
document.oncontextmenu = function () { return false; };
</script>

<!--#INCLUDE VIRTUAL="/cfserverasp/source/cows_func_js.asp"-->

<title>docmanager Results-List View</title>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</head>
<body background="<%=Application("UserWindowBackground")%>">

<%
'JHS added 4/02/03
if session("showselect") then
	Response.Write "<br>"
	Response.Write "<span align=""left"">To add a link to " & Session("LinkFieldName") & " " & Session("extLinkID") & " click the ""Add Doc Link"" button.</span>" 
	Response.Write "<br><br>"
end if	
'JHS added 4/02/03 end
%>
<br clear="all">
<table width="560" border="0" cellpadding="0" cellspacing="0">
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"--><!--cows_input_form starts here-->
<form name="documentForm" method="post" action="docmanager_result_list.asp">

<input type="hidden" name="toBeDeletedBaseID" value>

<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
	
	<%
	Set CountLinkRS = Server.CreateObject("ADODB.Recordset")
	sql = "SELECT COUNT(*) As linkCount FROM DOCMGR.DOCMGR_EXTERNAL_LINKS WHERE DOCID=" & BaseID
	CountLinkRS = BaseRSConn.Execute(sql)

	'BaseID represents the primary key for the recordset from the base array for the current row
	'BaseActualIndex is the actual id for the index the record is at in the array
	'BaseRunningIndex if the id based on the number shown in list view
	'BaseTotalRecords is the recordcount for the array
	'BaseRS (below is the recordset that is pulled for each record generated
	colsToSelect = "DOCLOCATION, DOCNAME, TITLE, AUTHOR, SUBMITTER, SUBMITTER_COMMENTS, DATE_SUBMITTED,REG_RLS_PROJECT_ID"
colsToSelect = colsToSelect &  "," & Application("OPTIONAL_FIELDS")
Set BaseRSConn = GetConnection(dbkey, formgroup, "DOCMGR_DOCUMENTS")
'sql=GetDisplaySQL(dbkey, formgroup,"DOCMGR_DOCUMENTS.*","DOCMGR_DOCUMENTS", "", BaseID, "")
sql = "SELECT " & colsToSelect & " FROM DOCMGR_DOCUMENTS WHERE DOCID=" & BaseID
Set BaseRS = BaseRSConn.Execute(sql)

	'sql = GetDisplaySQL(dbkey, formgroup,"DOCMGR_DOCUMENTS.*","DOCMGR_DOCUMENTS", "", BaseID, "")
	'sql = "SELECT DOCLOCATION, DOCNAME, TITLE, AUTHOR, SUBMITTER, SUBMITTER_COMMENTS, DATE_SUBMITTED, REG_RLS_PROJECT_ID FROM DOCMGR_DOCUMENTS WHERE DOCID=" & BaseID
	'Set BaseRS = BaseRSConn.Execute(sql)
	
	if Not (BaseRS.BOF and BaseRS.EOF) then 'this record exists
		Set projectRS = Server.CreateObject("ADODB.RecordSet")
		sql = "SELECT PROJECT_NAME FROM REGDB.PROJECTS WHERE PROJECT_INTERNAL_ID = " & BaseRS("REG_RLS_PROJECT_ID")
		Set projectRS = BaseRSConn.Execute(sql)
		
		'sql = "Select * from DOCMGR_STRUCTURES where DOCMGR_STRUCTURES.docid=" & BaseID
		'Set StructureRS = BaseRSConn.Execute(sql)
		Set peopleRS = Server.CreateObject("ADODB.RecordSet")
		sql = "select * from PEOPLE where user_id='" & UCase(BaseRS("SUBMITTER")) & "'"
		'Response.Write sql
		Set peopleRS = BaseRSConn.Execute(sql)
		if not (peopleRS.BOF and peopleRS.EOF) then
			peopleRS.MoveFirst
		end if
		%>
		<script language="javascript">
		//getRecordNumber(<%=BaseRunningIndex%>)
		//document.write ('<br>')
		//getMarkBtn(<%=BaseID%>)
		//document.write ('<br>')
		//getFormViewBtn("show_details_btn.gif","docmanager_form.asp","<%=BaseActualIndex%>")
		</script>
		<%
		'if not (StructureRS.BOF and StructureRS.EOF) then
			'StructureRS.MoveFirst%>
			<%'Do While Not StructureRS.EOF%>
			<%'ShowCFWChemResult dbkey, formgroup, "Structure","DOCMGR_STRUCTURES.STRUCTURE", StructureRS("MOL_ID"), "cdx","470","302"%>
			<%'ShowCFWChemResult dbkey, formgroup, "Formula","DOCMGR_STRUCTURES.FORMULA",  StructureRS("MOL_ID"), "raw", 1,"32"%>
			<%'ShowCFWChemResult dbkey, formgroup, "MolWeight","DOCMGR_STRUCTURES.MOLWEIGHT",  StructureRS("MOL_ID"), "raw", 1,"31.2"%>
	
			<%'StructureRS.MoveNext%>
			<%'loop%>
		<%'end if%>
	
		<tr><td valign="top">
				<%if LCase(Right(BaseRS("DOCNAME"), 4)) = ".txt" then%>
					<img src="/docmanager/graphics/txt.gif" border="0" width="32" height="32">
				<%elseif LCase(Right(BaseRS("DOCNAME"), 4)) = ".doc" or LCase(Right(BaseRS("DOCNAME"), 5)) = ".docx"  then%>
					<img src="/docmanager/graphics/word.gif" border="0" width="32" height="32">
				<%elseif LCase(Right(BaseRS("DOCNAME"), 4)) = ".xls" or LCase(Right(BaseRS("DOCNAME"), 5)) = ".xlsx" then%>
					<img src="/docmanager/graphics/excel.gif" border="0" width="32" height="32">
				<%elseif LCase(Right(BaseRS("DOCNAME"), 4)) = ".pdf" then%>
					<img src="/docmanager/graphics/pdf.gif" border="0" width="32" height="32">
				<%elseif LCase(Right(BaseRS("DOCNAME"), 4)) = ".ppt" or LCase(Right(BaseRS("DOCNAME"), 5)) = ".pptx" then%>
					<img src="/docmanager/graphics/ppt.gif" border="0" width="32" height="32">
				<%end if%>
			</td>
			
			<td width="10"></td>
			<td width="550">
			<div id="test<%=BaseID%>">
				<a href="javascript:goFormView('/docmanager/docmanager/docmanager_form.asp?formgroup=base_form_group&amp;dbname=docmanager&amp;formmode=edit&amp;unique_id=<%=BaseID%>&amp;commit_type=', <%=BaseActualIndex%>)">
					<%if BaseRS("TITLE") <> "" and BaseRS("TITLE") <> " " then%>
						<b><%=BaseRS("TITLE")%></b>
					<%else%>
						<!--b>No title</b-->
					<%end if%>	
				</a>
				<br>
				<!---font---><b><i>File name:</i></b><!---/font---> <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.DOCNAME","raw", 0, 0%>
				<br>
				<!---font---><b><i>Author:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.AUTHOR","raw", 0, 0%>
				<br>
				<!---font---><b><i>Submitted by: </i></b><!---/font--->
				<%if peopleRS("EMAIL") <> "" then%>
					<a href="mailto:<%=peopleRS("EMAIL")%>"><%=peopleRS("FIRST_NAME") & " " & peopleRS("LAST_NAME")%></a>
				<%else%>
					<%=peopleRS("FIRST_NAME") & " " & peopleRS("LAST_NAME")%>
				<%end if%>
				 at <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.DATE_SUBMITTED","raw", 0, 0%>
				<br>
				<!---font---> <b><i>Comments of submitter:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.SUBMITTER_COMMENTS","raw", 0, 0%>
				<br><b><i>Registration Project:</i></b> <%=projectRS("PROJECT_NAME")%>
				<br>

				<%'REPORT_NUMBER,MAIN_AUTHOR,STATUS,WRITER,ABSTRACT,DOCUMENT_DATE,DOCUMENT_CLASS,SEC_DOC_CAT%>
				
				<%if BaseRS("REPORT_NUMBER") <> "" then%>
					<b><i>Report Number:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.REPORT_NUMBER","raw", 0, 0%>
				<br>
				<%end if%>
				
				<%if BaseRS("MAIN_AUTHOR") <> "" then%>
					<b><i>Main Author:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.MAIN_AUTHOR","raw", 0, 0%>
				<br>
				<%end if%>
				<%if BaseRS("STATUS") <> "" then%>
					<b><i>Status:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.STATUS","raw", 0, 0%>
				<br>
				<%end if%>
				<%if BaseRS("WRITER") <> "" then%>
					<b><i>Writer:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.WRITER","raw", 0, 0%>
				<br>
				<%end if%>
				<%if BaseRS("ABSTRACT") <> "" then%>
					<b><i>Abstract:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.ABSTRACT","raw", 0, 0%>
				<br>
				<%end if%>
				<%if BaseRS("DOCUMENT_DATE") <> "" then%>
					<b><i>Document Date:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.DOCUMENT_DATE","raw", 0, 0%>
				<br>
				<%end if%>
				<%if BaseRS("DOCUMENT_CLASS") <> "" then%>
					<b><i>Document Class:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.DOCUMENT_CLASS","raw", 0, 0%>
				<br>
				<%end if%>

				<%if BaseRS("SEC_DOC_CAT") <> "" then%>
					<b><i>Document Security Category:</i></b><!---/font--->  <%ShowResult dbkey, formgroup, BaseRS,"DOCMGR_DOCUMENTS.SEC_DOC_CAT","raw", 0, 0%>
				<br>
				<%end if%>
				
			</div>
			</td>

			<td valign="top"><table>
			
					<%
					'JHS added 4/02/03
					if not Session("showSelect") then
					'JHS added 4/02/03 end
					%>
					<tr><td><a href="javascript:goFormView('/docmanager/docmanager/docmanager_form.asp?formgroup=base_form_group&amp;dbname=docmanager&amp;formmode=edit&amp;unique_id=<%=BaseRS("DOCID")%>&amp;download=true', <%=BaseActualIndex%>)"><img src="/docmanager/graphics/download.gif" border="0"></a></td>
						<td><a href="javascript:goFormView('/docmanager/docmanager/docmanager_form.asp?formgroup=base_form_group&amp;dbname=docmanager&amp;formmode=edit&amp;unique_id=<%=BaseRS("DOCID")%>&amp;download=true', <%=BaseActualIndex%>)"><%=BaseRS("DOCNAME")%></a></td>
					</tr>
					<%'JHS added 4/02/03
						else
					%>
					<tr>
						<td colspan="2">
							<a href="externalLinks/processLinks.asp?maction=Add&amp;docid=<%=BaseID%>" target="_top"><img src="/docmanager/graphics/Button_AddDocLink.gif" border="0"></a>
						</td>
					</tr>					
					<%	
						end if
					'JHS added 4/02/03 end
					%>
					
					
					<%'=session("ADMIN" & dbkey) & "/" & session("DELETE_DOCS" & dbkey)%>
					<%if session("DELETE_ALL_DOCS" & dbkey) = true then%>
							<%if CLng(CountLinkRS("linkCount")) > 0 then%>
								<%'admin can delete all docs%>
								<tr><td colspan="2"><input type="button" value="Delete" onclick="javascript:alert('Delete Denied. This document is associated with <%=CLng(CountLinkRS("linkCount"))%> external links.')" id="button1" name="button1"></td>
								</tr>
							<%else%>
								<%'admin can delete all docs%>
								<tr><td colspan="2"><input type="button" value="Delete" onclick="javascript:if(confirm('Remove <%=BaseRS("DOCNAME")%>?')) {this.form.toBeDeletedBaseID.value=<%=BaseID%>;this.form.submit()}" id="button1" name="button1"></td>
								</tr>
							<%end if%>
					<%else 'not admin%>
						<%if Session("DELETE_MY_DOCS" & dbkey) = True and LCase(BaseRS("SUBMITTER")) = LCase(Session("UserName" & Application("appkey"))) then%>
							<%if CLng(CountLinkRS("linkCount")) > 0 then%>
								<tr><td colspan="2"><input type="button" value="Delete" onclick="javascript:alert('Delete Denied. This document is associated with <%=CLng(CountLinkRS("linkCount"))%> external links.')" id="button1" name="button1"></td>
								</tr>
							<%else%>
								<%'only submitter of this document can delete it%>
								<tr><td colspan="2"><input type="button" value="Delete" onclick="javascript:if(confirm('Remove <%=BaseRS("DOCNAME")%>?')) {this.form.toBeDeletedBaseID.value=<%=BaseID%>;this.form.submit()}" id="button1" name="button1"></td>
								</tr>
							<%end if%>	
						<%end if%>
					<%end if%>
				</table>
			</td>
		</tr>
		<tr><td><br /></td></tr>
	<%else 'this record has been deleted%>
		<tr><td><b><font color="red">Document removed.</font></b><br><br></td></tr>
	<%end if%>
	<%CloseRS(LinkCountRS)%>
	<%CloseRS(BaseRS)%>
	<%CloseRS(StructureRS)%>
	<%CloseRS(peopleRS)%>
	<%CloseRS(projectRS)%>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" --><!--there is a </form> in this-->
	<%CloseConn(BaseRSConn)%>
</form>
</table>
</body>
</html>
