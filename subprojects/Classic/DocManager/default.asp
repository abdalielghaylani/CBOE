<%@ LANGUAGE=VBScript %>
<%Response.Expires=0%>
<%
dbkey = ""
formgroup = ""
formmode = Request("formmode")
dbkey = Request("dbname")
if not dbkey <> "" then dbkey = "docmanager"
appkey = Application("appkey")
dataaction = Request("dataaction")

formgroup = Request("formgroup")

showNavBar = request("showNavBar")

'JHS added 4/02/03
'show select handling
'if Request("showselect") = "true" or Session("showselect") = true then
if Session("showselect") = Empty then 'not defined
	if Request("showselect") = "true" then
		Session("showselect") = true
		Session("extAppName") = Request("extAppName")
		Session("LinkType") = Request("LinkType")
		Session("extLinkID") = Request("extLinkID")
		Session("LinkFieldName") = Request("LinkFieldName")
		if  Request("useReload") = "false" then
			Session("useReload") = false
		else
			Session("useReload") = true
		end if
	else
		Session("showselect") = false
	end if
'JHS 10/2/2007 this is probably not the best way to do this
elseif Request("showselect") = "true" then
		Session("showselect") = true
		Session("extAppName") = Request("extAppName")
		Session("LinkType") = Request("LinkType")
		Session("extLinkID") = Request("extLinkID")
		Session("LinkFieldName") = Request("LinkFieldName")	
		if  Request("useReload") = "false" then
			Session("useReload") = false
		else
			Session("useReload") = true
		end if		
end if
'JHS added 4/02/03 end




if not formgroup <> "" then formgroup = "base_form_group"
if Application("DBLoaded" & dbkey) = True then
	userinfopath = "/" & appkey & "/user_info.asp?dbname=" & dbkey & "&formgroup=" & formgroup
	
	'Response.write dataaction
	Select Case UCase(dataaction)
		Case "MAINPAGE"
			Session("CurrentLocation" & dbkey) = ""
			path = "/" & Application("AppKey") & "/docmanager/mainpage.asp?formgroup=base_form_group&dbname=docmanager&timer=" & Timer
		Case "LOGIN"
			Session("CurrentLocation" & dbkey) = ""
			path = "/" & Application("AppKey") & "/login.asp?formgroup=base_form_group&dbname=docmanager&timer=" & Timer
		
		Case "MANAGE_SECURITY"
			Session("CurrentLocation" & dbkey) = ""
			path = "/" & Application("AppKey") & "/cs_security/manage_security/manage_security_input_form.asp?nav_override=true&formgroup=manage_users_form_group&formmode=manage_users&dbname=" & dbkey & "&timer=" & Timer
			'path = "/" & Application("AppKey") & "/cs_security/manage_security/manage_security_frame.asp?nav_override=true&formgroup=manage_users_form_group&formmode=manage_users&dbname=" & dbkey & "&timer=" & Timer
		
		Case "MANAGE_TABLES"
			Session("CurrentLocation" & dbkey) = ""
			path = "/" & Application("AppKey") & "/cs_security/manage_tables/manage_tables_input_form.asp?nav_override=true&formgroup=manage_tables_form_group&formmode=manage_tables&dbname=" & dbkey & "&timer=" & Timer
	
		Case "GET_SQL_STRING"
			Session("CurrentLocation" & dbkey) = ""
			path= "/" & Application("AppKey") & "/docmanager/reg_action.asp?" & request.querystring & "&timer=" & Timer
			'store sql
			Session("sql_string")=Request("sql_string")
			Session("table_name") = Request("table_name")
		Case "CHANGE_PASSWORD"
			Session("CurrentLocation" & dbkey) = ""
			path = "/" & Application("AppKey") & "/cs_security/manage_security/manage_pwd_result_form.asp?nav_override=true&formgroup=manage_users_form_group&formmode=manage_passwords&dbname=" & dbkey & "&timer=" & Timer

		Case "QUERY_STRING"
			Session("CurrentLocation" & dbkey) = ""
			path= "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp?" & request.querystring & "&timer=" & Timer
		'Case "SUBMITDOCS" 'main page submitdoc button
			'Session("CurrentLocation" & dbkey) = ""
			'path = "/" & Application("AppKey") & "/docmanager/src/locatedocs.asp?nav_override=true&formgroup=base_form_group&formmode=submitdocs&dbname=" & dbkey & "&timer=" & Timer
		'Case "SEARCHDOCS" 'locatedoc page searchdoc button
			'Session("CurrentLocation" & dbkey) = ""
			''path= "/" & appkey  & "/" &  dbkey & "/docmanager_input_form.asp?formgroup=base_form_group&formmode=search&dbname=" & dbkey & "&timer=" & Timer
			'if not Session("CurrentLocation" & dbkey)<> "" then
				''Session("CurrentLocation" & dbkey) = ""
				'thearray = Application(formgroup & dbkey)
				''CONST kInputFormMode = 3
				'formmode =thearray(3)
				'path = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?formgroup=" & formgroup & "&dataaction=db&formmode=" & formmode & "&dbname=" & dbkey
				'end if
		Case "DB"
			Session("CurrentLocation" & dbkey) = ""
			path = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?formgroup=" & formgroup & "&dataaction=db&formmode=" & formmode & "&dbname=" & dbkey
			
		Case Else
			if not Session("CurrentLocation" & dbkey)<> "" then
				'Session("CurrentLocation" & dbkey) = ""
				thearray = Application(formgroup & dbkey)
				CONST kInputFormMode = 3
				formmode =thearray(kInputFormMode)
				path = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?formgroup=" & formgroup & "&dataaction=db&formmode=" & formmode & "&dbname=" & dbkey
				end if
		End Select
	if Not Session("CurrentLocation" & dbkey) <> "" then
		Session("CurrentLocation" & dbkey)=path
	end if
%>
<html>
<head>
<title>Document Manager</title>
<!-- CBOE-1823 added code to display Document Manager help on F1 click. Debu 05SEP13 -->
 <script language="javascript" type="text/javascript">
     function onkeydown_handler() {
         switch (event.keyCode) {
             case 112: // 'F1'
                 document.onhelp = function () { return (false); }
                 window.onhelp = function () { return (false); }
                 event.returnValue = false;
                 event.keyCode = 0;
                 window.open('../../../../CBOEHelp/CBOEContextHelp/Doc%20Manager%20Webhelp/Default.htm');
                 return false;
                 break;
         }
     }
     document.attachEvent("onkeydown", onkeydown_handler);
    </script>
</head>

<%if showNavBar = "false" then%>
<frameset BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" FRAMEBORDER="0" FRAMESPACING="0" rows="0,*">
<%else%>
<frameset BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" FRAMEBORDER="0" FRAMESPACING="0" rows="170,*">
<%end if%>
	<frame BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" SCROLLING="NO" SRC="javascript:''" NAME="navbar">
		<frameset BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" FRAMEBORDER="0" FRAMESPACING="0" cols="105,*">
			<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" rows="300,*">
				<frame BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="<%=userinfopath%>" NAME="userinfo" scrolling="auto">
				<frame BORDER="0" SCROLLING="NO" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="helper.asp" NAME="helper" scrolling="auto">
			</frameset>
			<frame BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="<%=Session("CurrentLocation" & dbkey)%>" NAME="main">
		</frameset>
	<noframes>
	<body>
	</body>
	</noframes>
</frameset>
</html>
<%else 'db not loaded
	Response.Write "The web database requested did not initialize.<br>"
	Response.Write "For details, see the log file at <a href=" & Application("AppPathHTTP") & "/logfiles/" & dbkey & "log.html" & ">" & Application("AppPathHTTP") & "/logfiles/" & dbkey & "log.html</a>"
end if
%>
