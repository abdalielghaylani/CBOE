<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
	
	Dim Cmd
	Dim Conn
	
	FK_value = Request("FK_value")
	FK_name = Request("FK_name")
	Table_name = Request("Table_name")
	URLType = Request("URLType")
	LinkType = Request("LinkType")
	
	if IsEmpty(FK_value) OR FK_value = "" then FK_value = NULL
	if IsEmpty(FK_name) OR FK_name = "" then FK_name = NULL
	if IsEmpty(Table_name) OR Table_name = "" then Table_name = NULL
	if IsEmpty(URLType) OR URLType = "" then URLType = NULL
	
	Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".LINKS.GetLinks(?,?,?,?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PFK_VALUE",200, 1, 10, FK_value)
	Cmd.Parameters("PFK_VALUE").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PFK_NAME", 200, 1, 50, FK_Name)
	Cmd.Parameters.Append Cmd.CreateParameter("PTABLE_NAME", 200, 1, 50, Table_Name)
	Cmd.Parameters.Append Cmd.CreateParameter("PURLTYPE", 200, 1, 255, URLType)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
%>
<html>
<head>
	<title>Manage Links</title>
	<script LANGUAGE="javascript">
		window.focus();
	</script>

	<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
	<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<body>
	<center><br>
	<table border="1">
	<tr>
		<th>TableName</th>
		<th>FK Name</th>
		<th>FK Value</th>
		<th>URL</th>
		<th>Link Text</th>
		<% if LinkType = "Batch" then %>
			<th>Document Type</th>
		<% else%>
		    <th>Image Source</th>		
		<%end if %>		
		<th align="center">
			<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Links.asp?fk_value=<%=fk_value%>&amp;fk_name=<%=fk_name%>&amp;Table_Name=<%=table_name%>&amp;URLtype=<%=URLType%>&amp;LinkType=<%=LinkType%>', 'DiagNewLink', 2); return false">New</a>
		</td>
	</tr>
<%
	If (RS.EOF AND RS.BOF) then
		Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No URLS found</Span></TD></tr></table>")
	Else
		While (Not RS.EOF)
			
			URLID = RS("URL_ID")
			URLhref = RS("URL")
			Link_Text = RS("Link_Txt")
			Image_source = RS("Image_src")
			FK_value = RS("FK_value")
			FK_name = RS("FK_name")
			Table_Name = RS("Table_Name")
			URLType = RS("URL_Type")
			if Image_source <> "" then 
				strImageSource = Server.URLEncode(Image_source)
			else
				strImageSource = ""
			end if
			editData = "URLID=" & URLID & "&URLhref=" & Server.URLEncode(URLhref) & "&Link_text=" & server.URLEncode(replace(Link_Text,"'","\'")) & "&URLType=" & URLType & "&Image_source=" & strImageSource & "&FK_value=" & FK_value & "&FK_name=" & FK_name & "&Table_Name=" & Table_Name & "&LinkType=" & LinkType
%>
			<tr>
				<td align="center">
					<%=Table_Name%>
				</td>
				<td align="center">
					<%=FK_Name%>
				</td>
				<td align="center">
					<%=FK_value%>
				</td>
				<td align="left">
					<%=HTMLNull(TruncateInSpan(URLhref, 20, "url"))%>
				</td>
				<!--				<td align="center">					<%=HTMLNull(URLType)%>				</td>				-->
				<td align="center">
					<%=HTMLNull(TruncateInSpan(Link_Text,15,"lt"))%>
				</td>
				<td align="center">
					<% if LinkType = "Batch" then %>
			            <%=URLType%>
		            <% else%>
					    <%=HTMLNull(TruncateInSpan(Image_source, 15, "is"))%>
					<%end if%>    
				</td>
				<td align="center">
					<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Links.asp?<%=editData%>', 'Diag1', 2); return false">&nbsp;Edit</a>
					|
					<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Links.asp?action=delete&amp;<%=editData%>', 'Diag2',2); return false">Delete&nbsp;</a>
				</td>
			</tr>
			<%rs.MoveNext
		Wend
		Response.Write "</table>"
	End if
	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing		
%>
<br><a HREF="#" onclick="opener.location.reload(); window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
</center>
</body>
</html>
