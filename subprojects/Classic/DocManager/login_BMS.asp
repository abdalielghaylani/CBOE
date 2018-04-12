<%@ Language=VBScript %>

<%
Response.Expires = 0
dbkey = Request("dbname")
if dbkey = "" Then dbkey="docmanager"
Dim lo_DEBUG
lo_DEBUG = false
%>

<script language="javascript">
	if (parent.location.href != window.location.href) parent.location.href = window.location.href;
</script>

<html>
<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
</head>

<!--BODY background="<%=Application("UserWindowBackground")%>"-->
<body background="/docmanager/graphics/BMSLoginBk.gif">
<%	
Session("CurrentLocation" & dbkey) = ""
formgroup = Request("formgroup")
if formgroup = "" Then formgroup="base_form_group"

BaseTable = GetTableGroupVal(dbkey, formgroup, kBaseTable)
perform_validate = Request.QueryString("perform_validate")
%>

<!--#INCLUDE FILE = "cs_security/login_security_vbs.asp"-->

<%if perform_validate = 1 then
	'these session variables are looked at in any GetConnection calls
	Session("UserName" & dbkey) = Trim(Request.Form("user_name"))
	Session("UserID" & dbkey) = Trim(Request.Form("user_id"))
	'choose the ado_connection group for validation
	appType = UCase(Application("App_Type"))
	basetable = "docmgr_documents"
	isValid = 0
	isValid = DoUserValidate(dbkey, formgroup, appType, basetable, Session("UserName" & dbkey))
	Session("UserValidated" & dbkey) =isValid
	
	'Logaction "in login.asp, session(Edit_Users_Table) = " & session("Edit_Users_Table" & dbkey)
	'Logaction "in login.asp, session(Edit_People_Table) = " & session("Edit_People_Table" & dbkey)
	
	newlocation = "/" & Application("AppKey")& "/" & dbkey & "/mainpage.asp?timer=" & Timer%>
	
	<script language="javascript">
		document.location.href = "<%=newlocation%>"
	</script>
	
<%else%>

	
	<form name="UserLogin" action="/<%=Application("AppKey")%>/login.asp?dbname=<%=dbkey%>&amp;formgroup=base_form_group&amp;perform_validate=1" method="post" )>
	<table width="620" border="0" cellpadding="0" cellspacing="0">
		<tr><td height="200"></td>
		</tr>
		
		<tr>
			<td width="280"></td>
			<td>
				<!--table border="0" cellspacing="0" cellpadding="0">					<tr><td><img src="/docmanager/docmanager/gifs/banner.gif"></td>					</tr>				</table-->

				<table border="0">
					<tr><td valign="top">
							<table border="0"><%if Session("LoginErrorMessage" & dbkey) <> "" then%>
										<tr><td colspan="2"><font color="red" face="Arial"><%=Session("LoginErrorMessage" & dbkey)%></font></td>
										</tr>
									<%end if%>

								<tr>
									<td><font face="Arial"><b>Login ID:</b></font></td>
									<td><input type="text" size="20" name="user_name"></td>
								</tr>
								<tr>
									<td><font face="Arial"><b>Password:</b></font></td>
									<td><input type="password" size="20" name="user_id"></td>
								</tr>
								<tr>
									<td nowrap colspan="2" align="right"><a href="javascript:document.UserLogin.submit()"><img src="/<%=Application("AppKey")%>/graphics/login_btn.gif" border="0"></a>
										<!--a href="/chemoffice.asp"><img src="/<%=Application("AppKey")%>/graphics/exit_btn.gif" border="0"></a-->
									</td>
								</tr>
							</table>
						</td>
						
						<!--td><table>								<tr><td><font size="4" color="#990000">Welcome to document manager!</font>										<p>										If you have an account already, please login to access your private document 										management account. If you do not have an account yet, please 										contact administrator. 										<p>										<font size="4" color="#990000">With document manager, you can</font>										<p>										<li>View and search the shared documents at any place with Internet connection.											<br>Simply log in to the site. With privilege granted, you can either 												search the documents by chemistry structure, or by free text.										<p>										<li>Share your documents with others having privilege of viewing the documents.											<br>											Submit your documents via the web interface after log in. The document											submitted will be securely stored in database, indexed by both structure and 											text for other users to search and view.										<p>										<li>Remove documents that are no longer shared.										<p>										<li>Manage users and groups with administration privilege. Adding and remove 											users, grant and revoke privileges all via web.									</td>								</tr>										</table>						</td-->
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
<%end if%>
</body>
</html>
