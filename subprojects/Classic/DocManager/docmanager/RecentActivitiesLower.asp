<!--#INCLUDE VIRTUAL="/docmanager/docmanager/src/datetimefunc.asp"-->
<%'Option Explicit

Dim BaseRSConn, BaseRS
Dim submissionTime, sortBy, sortByClause, days
Dim dbkey, formgroup
Dim cnn, rst, sql, cmd
Dim docCount

submissionTime = Request.QueryString("submissionTime")
sortBy = UCase(request.QueryString("sortBy"))
days = request.QueryString("days")

if submissionTime = "today" then
	days = "1"
end if

if days = "" then
	days = "1"
end if

dbkey = "docmanager"
formgroup = "base_form_group"

select Case sortBy
	Case "TIME"
		sortByClause = "ORDER BY DATE_SUBMITTED"
	Case "SUBMITTER"
		sortByClause = "ORDER BY SUBMITTER"
	Case "TITLE"
		sortByClause = "ORDER BY TITLE"
	Case "TYPE"
		sortByClause = "ORDER BY DOCTYPE"
	Case "FILENAME"
		sortByClause = "ORDER BY DOCNAME"
	Case else
		sortByClause = "ORDER BY DATE_SUBMITTED"
end select


Set cnn = Server.CreateObject("ADODB.Connection")
		
with cnn
	.ConnectionString = Application("cnnStr")
end with
cnn.Open
		
if cnn.State = 1 then 'adStateOpen
	'sql = "SELECT COUNT(*) FROM DOCMGR_DOCUMENTS WHERE (DATE_SUBMITTED > SYSDATE - INTERVAL '" & days & "' DAY)" 
	sql = "SELECT COUNT(*) FROM DOCMGR_DOCUMENTS WHERE (TRUNC(SYSDATE) - TRUNC(DATE_SUBMITTED) < " & CInt(days) & ")" 
	Set rst = cnn.Execute(sql)
	docCount = rst(0)
	
	'JHS
	
	'sql = "SELECT DOCID, DOCNAME, TITLE, AUTHOR, SUBMITTER, DOCTYPE, DOCNAME, SUBMITTER_COMMENTS, DATE_SUBMITTED FROM DOCMGR_DOCUMENTS WHERE (DATE_SUBMITTED > SYSDATE - INTERVAL '" & days & "' DAY) " & sortByClause
	sql = "SELECT DOCID, DOCNAME, TITLE, AUTHOR, SUBMITTER, DOCTYPE, DOCNAME, SUBMITTER_COMMENTS, DATE_SUBMITTED FROM DOCMGR_DOCUMENTS WHERE (TRUNC(SYSDATE) - TRUNC(DATE_SUBMITTED) < " & CInt(days) & ") "  & sortByClause
	Set rst = cnn.Execute(sql)
else
	Response.Write "ERROR: Cannot connect to database."
end if


	Dim dt
%>

<html>
<head>
<title>View Logs</title>

</head>

<body background="<%=Application("UserWindowBackground")%>">
<table>
	<tr><td width="100"></td>
		<td><%=docCount%> documents submitted. </td>
	</tr>
	
	<tr><td width="100"></td>
		<td>
			<table border="1">
				<tr>
					<td>Title</td>
					<td>Author</td>
					<td>Submitted by</td>
					<td>File Type</td>
					<td>File Name</td>
					<td>Date Submitted</td>
				</tr>
				
				<%
				if not (rst.BOF and rst.EOF) then
					rst.MoveFirst
					while not rst.EOF
				%>
						<tr><td nowrap><a href="javascript:window.open('/docmanager/default.asp?formgroup=base_form_group&dbname=docmanager&formmode_override=edit&dataaction=query_string&field_type=integer&full_field_name=docmgr_documents.docid&field_value=<%=rst("DOCID")%>&showNavBar=false', 'ShowDocWin'); window.location.reload();"><%=rst("TITLE")%></a></td>
							<td><%if rst("AUTHOR") <> "" then%>
									<%=rst("AUTHOR")%>
								<%else%>
									&nbsp;
								<%end if%>
							</td>
							<td><%=rst("SUBMITTER")%></td>
							<td><%=rst("DOCTYPE")%></td>
							<td><%=rst("DOCNAME")%></td>
							
							<%
							
							dt=fmtDateTime(CDATE(rst("DATE_SUBMITTED")),Application("DATE_FORMAT_DISPLAY")  & " hh:mm:ss")
							'dt = rst("DATE_SUBMITTED")%>
							<td><%=dt%></td>
						</tr>
				<%
						rst.MoveNext
					wend%>
				<%end if%>
			</table>
		</td>
	</tr>
</table>
</body>
</html>
