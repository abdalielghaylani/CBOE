<%@LANGUAGE="VBScript"%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
Response.Expires=0
dbkey=Request("dbname")
if dbkey = "" then
	response.write "the name of the database(dbkey) is missing in the query string or form that requested this page"
	response.end
end if
if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if


%>
<script language="javascript">
<!-- Hide from older browsers
if(parent.location.href != window.location.href) parent.location.href = window.location.href;


function go_get_sql_string(formgroup,sql_string,table_name,limit_access_to){
		document.cows_input_form.sql_string.value = sql_string
		document.cows_input_form.action= "/<%=Application("Appkey")%>/default.asp?dbname=" & dbkey & "&formgroup=" + formgroup + "&dataaction=get_sql_string&sql_source=session_sql_string&table_name=" + table_name + "&limit_access=" + limit_access_to
		document.cows_input_form.submit()
}
// End script hiding --></script>
<%
Session("CurrentLocation" & dbkey)= ""
If Not Session("CurrentUser" & dbkey) <> "" then
	theuser = Session("UserName" & dbkey)
	if theuser <> "" then
		Session("CurrentUser" & dbkey) = theuser
	end if
end if
%>
<html>
<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
<meta NAME="GENERATOR" Content="Microsoft FrontPage 4.0">
<title>Main Page</title>
</head>
<body bgcolor="#e0e0ff">
<form name="cows_input_form" method="post" action="/<%=Application("appkey")%>/default.asp">
<input type="hidden" name="sql_string">
</form>

<img  src="/<%=Application("appkey")%>/graphics/cnco.gif" height="55" ><img src="/<%=Application("appkey")%>/graphics/mainpage_bnr.gif"> <br>
<table border="0" bgcolor="#e0e0ff" bordercolor="#42426f" width="620">
	<tr>
		<td valign="top" width="4">
            &nbsp;
		</td>
		<td valign="top" width="621">
			<table border="0" cellspacing="0" cellpadding="0" bgcolor="#e0e0ff" bordercolor="#42426f" width="100%">
				<tr>
					<td width="250">
					<table border="0"  bordercolor="#42426f" width="202" height="131">
							<tr>
								<td colspan="2" align="middle" width="184" height="19"></td>
							</tr>
										<tr>
								<td width="120" nowrap height="30" valign="top" bgcolor="#e0e0ff"><font face="Arial" color="#42426f" size="2">Search
                                  </font></td>
								<td width="58" height="30" valign="top">
                                 <a href="/<%=Application("appkey")%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>">
									<img src="/<%=Application("appkey")%>/graphics/search_btn.gif" align="left" border="0"></a>
                                 								</td>
							</tr>
						</table>
					</td>
					<td width="80">&nbsp;</td>
					<td width="250" valign="top">
                    &nbsp;
					</td>
				</tr>
				<tr>
					<td width="196">&nbsp;</td>
					<td width="5">&nbsp;</td>
				</tr>
				<tr>
					<td valign="top" width="196">
                    &nbsp;
					</td>
					<td width="5"></td>
					<td width="422" valign="top">						
                      &nbsp;
					</td>
				</tr>
				<tr>
					<td width="196">&nbsp;</td>
					<td width="5">&nbsp;</td>
					<td width="422">&nbsp;</td>
				</tr>
				<tr><% if Application("LoginRequired" & dbkey)=1 then%>
					<td width="100">&nbsp;
                      <table border="0" cellpadding="0" width="228">
                        <tr>
                          <td width="170"><font face="Arial" color="#42426f" size="2">You are currently<br>logged in as:<%=UCase(Session("UserName" & dbkey))%>
                            </font></td>
                          <td width="119"><a href="/<%=Application("appkey")%>/logoff.asp"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a></td>
                        </tr>
                      </table>
                      <p><font face="Arial" color="#42426f" size="2"></font></p>
                  </td>
					<td width="25"><a href="/<%=Application("appkey")%>/logoff.asp"></a></td>
					<td width="50">&nbsp;</td>
					<%end if%>
				</tr>
			</table>
		</td>
	</tr>
</table>



    
</body>
</html>
