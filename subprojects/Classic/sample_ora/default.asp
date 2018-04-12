<%@ LANGUAGE=VBScript %>
<%Response.Expires=0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
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
			Session("CurrentLocation" & dbkey & formgroup) = ""
			path = "/" & Application("AppKey") & "/" & dbkey & "/mainpage.asp?formgroup=base_form_group&dbname=" & dbkey & "&timer=" & Timer
		Case "LOGIN"
			Session("CurrentLocation" & dbkey & formgroup) = ""
			path = "/" & Application("AppKey") & "/login.asp?formgroup=base_form_group&dbname=" & dbkey & "&timer=" & Timer
		Case "ADD_COMPOUND"
			thearray = Session("Base_RS" & dbkey)
			thearray= ""
			Session("Base_RS" & dbkey) = thearray
			Session("CurrentLocation" & dbkey & formgroup) = ""
			path = "/" & Application("AppKey") & "/" & dbkey & "/Add_Compound_input_form.asp?record_added=false&formmode=add_compound&formgroup=Add_Compound_form_group&dbname=" & dbkey & "&commit_type=full_commit&timer=" & Timer
		Case "GET_SQL_STRING"
			Session("CurrentLocation" & dbkey & formgroup) = ""
			path= "/" & Application("AppKey") & "/" & dbkey  & "/" & dbkey & "_action.asp?" & request.querystring & "&timer=" & Timer
			'store sql
			Session("sql_string")=Request("sql_string")
			Session("table_name") = Request("table_name")
		Case "QUERY_STRING"
			Session("CurrentLocation" & dbkey & formgroup) = ""
			path= "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp?" & request.querystring & "&timer=" & Timer
		Case Else
			if not Session("CurrentLocation" & dbkey & formgroup)<> "" then
				'Session("CurrentLocation" & dbkey) = ""
				
					thearray = Application(formgroup & dbkey)
					CONST kInputFormMode = 3
					formmode =thearray(kInputFormMode)
					path = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?formgroup=" & formgroup & "&dataaction=db&formmode=" & formmode & "&dbname=" & dbkey
				
			end if
		
		End Select
	
	if Not Session("CurrentLocation" & dbkey & formgroup) <> "" then
		Session("CurrentLocation" & dbkey & formgroup)=path
	end if
	
%>
<html>
<head>
<title>Frameset Page</title>
</head>

<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" rows="168,*">
	<frame BORDER="0" SCROLLING="NO" SRC="javascript:''" NAME="navbar">
		<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" cols="20,*">
			<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" rows="300,*">
				<frame BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="<%=userinfopath%>" NAME="userinfo" scrolling="auto">
				<frame BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="helper.asp" NAME="helper" scrolling="no">
			</frameset>
			<frame BORDER="0" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="<%=Session("CurrentLocation" & dbkey & formgroup)%>" NAME="main">
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
