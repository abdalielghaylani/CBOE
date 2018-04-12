<%@ LANGUAGE=VBScript %>
<%Response.Expires=0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%

formmode = Request("formmode")
dbkey = Request("dbname")
appkey = Application("appkey")
dataaction = Request("dataaction")
formgroup = Request("formgroup")
if Application("Main_Page" & dbkey) = "1" then
	mytest=Split(Request.ServerVariables("HTTP_REFERER"),"/",-1)
	on error resume next
	if UCase(mytest(Ubound(mytest))) = "CHEMOFFICE.ASP" then
		dataaction = "MAINPAGE"
	end if
	if err.number <> 0 then
		dataaction = "MAINPAGE"
	end if
	on error goto 0
end if

if Application("LoginRequired" & dbkey)=1 and Session("UserValidated" & dbkey)=0 then
	dataaction = "LOGIN"
end if
if not formgroup <> "" then formgroup = "base_form_group"
if Application("DBLoaded" & dbkey) = True then
	userinfopath = "/" & appkey & "/user_info.asp?dbname=" & dbkey & "&formgroup=" & formgroup
	
	Select Case UCase(dataaction)
		Case "MAINPAGE"
			Session("CurrentLocation" & dbkey) = ""
			formmode = "search"
				
			path="/biosar_browser/biosar_browser/mainpage.asp?dbname=biosar_browser&formgroup=base_form_group&formmode=search"
		Case "LOGIN"
			Session("CurrentLocation" & dbkey) = ""
			path = "/" & Application("AppKey") & "/login.asp?formgroup=base_form_group&dbname=" & dbkey & "&timer=" & Timer
		Case "FORM_EDIT"
			Session("CurrentLocation" & dbkey) = ""
			path = "/" & Application("AppKey") & "/" & dbkey & "/mainpage.asp?formgroup=base_form_group&dbname=" & dbkey & "&timer=" & Timer

		Case "GET_SQL_STRING"
			Session("CurrentLocation" & dbkey) = ""
			path= "/" & Application("AppKey") & "/" & dbkey  & "/" & dbkey & "_action.asp?" & request.querystring & "&timer=" & Timer
			'store sql
			Session("sql_string")=Request("sql_string")
			Session("table_name") = Request("table_name")
		Case "QUERY_STRING"
			Session("CurrentLocation" & dbkey) = ""
			path= "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp?" & request.querystring & "&timer=" & Timer
		Case "RESTORE_HITLIST"
			Session("CurrentLocation" & dbkey) = ""
			path= "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp?" & request.querystring & "&timer=" & Timer
		Case "PASS_THROUGH"
			Session("CurrentLocation" & dbkey) = ""
			formmode = "search"
			path = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?" & request.querystring
		Case "SEARCH"
			Session("CurrentLocation" & dbkey) = ""
			formmode = "search"
			path = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?" & request.querystring
		Case "MANAGE_SECURITY"
			Session("CurrentLocation" & dbkey & formgroup) = ""
			path = "/" & Application("AppKey") & "/cs_security/manage_security/manage_security_input_form.asp?nav_override=true&formgroup=manage_users_form_group&formmode=manage_users&dbname=" & dbkey & "&timer=" & Timer
			Session("bypass_ini" & dbkey & "manage_users_form_group") = false
		Case "CHANGE_PASSWORD"
			Session("CurrentLocation" & dbkey & formgroup) = ""
			path = "/" & Application("AppKey") & "/cs_security/manage_security/manage_pwd_result_form.asp?nav_override=true&formgroup=manage_users_form_group&formmode=manage_passwords&dbname=" & dbkey & "&timer=" & Timer
			userinfopath = "/" & appkey & "/user_info.asp?dbname=" & dbkey & "&formgroup=manage_users_form_group&formmode=manage_passwords&timer=" & Timer
		Case "SCHEMA_MANAGEMENT"
			
			Session("CurrentLocation" & dbkey ) = ""
			path="/biosar_browser/biosar_browser_admin/admin_tool/admin_table_list.asp?action=get_schema_list&dbname=biosar_browser&formgroup=base_form_group&formmode=admin"
		Case "FORM_MANAGEMENT"
		
			Session("CurrentLocation" & dbkey ) = ""
			path="/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?dbname=biosar_browser&formgroup=base_form_group&formmode=admin"
			
		Case "OPEN_FORM"
		
			
			
			Session("CurrentLocation" & dbkey ) = ""
			path="/biosar_browser/biosar_browser/mainpage.asp?bypass_ini=true&dbname=biosar_browser&formgroup=base_form_group&refreshPath=/biosar_browser/biosar_browser/mainpage.asp"&_
			"&formmode=search&TreeID=1&TreeTypeID=" & Session("PRIVATE_CATEGORY_TREE_TYPE_ID") &_
			",2&ItemURL=/biosar_browser/biosar_browser/biosar_browser_action.asp%3Fdataaction=db%26bypass_ini=true" &_
			"&ItemQSId=formgroup&ItemTarget=main&ItemTypeID=2&TreeMode=open_items" &_
			"&ClearNodes=0&QsRelay=formgroup,bypass_ini,formmode,refreshPath,refreshFormgroup,showItems,qshelp,ItemQSId" & "&showitems=true&QSHelp=Click a link to select a query form."
		
		'DGB new action to reach a given query form.
		Case "GOTOQUERYPAGE"
			
				Session("CurrentLocation" & dbkey) = "/biosar_browser/biosar_browser/biosar_browser_action.asp?dataaction=db&dbname=biosar_browser&formgroup=" & Request("formgroup")
				formmode = "search"
				path="/biosar_browser/biosar_browser/mainpage.asp?dbname=biosar_browser&formgroup=base_form_group&formmode=search"	
			

		
		Case Else
			if not Session("CurrentLocation" & dbkey)<> "" then
				Session("CurrentLocation" & dbkey) = ""
				formmode = "search"
				path="/biosar_browser/biosar_browser/mainpage.asp?dbname=biosar_browser&formgroup=base_form_group&formmode=search"	
			end if
		End Select
	if Not Session("CurrentLocation" & dbkey) <> "" then
		Session("CurrentLocation" & dbkey)=path
	end if
if Application("OPTIMIZE_RESULT_DISPLAY_AREA") = 1 then
	numRows = "110"
else
	numRows = "168"
end if
%>
<html>
<head>
<title>BioSAR Browser</title>
</head>

<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" rows="<%=numRows%>,*" >
	<frame BORDER="0" SCROLLING="no" SRC="javascript:''" NAME="navbar">
		<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" cols="20,*">
			<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" rows="50,*">
				<frame BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="<%=userinfopath%>" NAME="userinfo" scrolling="no">
				<frame BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="blank.html" NAME="helper" scrolling="no">
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
