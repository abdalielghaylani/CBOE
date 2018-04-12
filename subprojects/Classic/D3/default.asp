<%@ LANGUAGE=VBScript %>
<%Response.Expires=0%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/D3/source/app_vbs.asp"-->
<%

CONST kInputFormMode = 3
				
formmode = lcase(Request( "formmode" ))
dbkey = Request( "dbname" )
appkey = Application( "appkey" )
dataaction = Request( "dataaction" )
formgroup = Request( "formgroup" )



if Application( "LoginRequired" & dbkey )=1 and Session( "UserValidated" & dbkey )=0 then
	dataaction = "LOGIN"
end if

if formgroup = "" then formgroup = "base_form_group"
	'Response.write Session( "CurrentLocation" & dbkey & formgroup ) & "<br>"

if Application( "DBLoaded" & dbkey ) = True then
	userinfopath = "/" & appkey & "/user_info.asp?dbname=" & dbkey & "&formgroup=" & formgroup
	userinfopath = URLWithMadeInArgument( userinfopath, "DDDefault" )

	
	
	
'Response.end

	Select Case UCase(dataaction)
		Case "MAINPAGE"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/mainpage.asp?formgroup=base_form_group&dbname=" & dbkey

		Case "LOGIN"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/login.asp?formgroup=base_form_group&dbname=" & dbkey

		
		Case "GET_SQL_STRING"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey  & "/" & dbkey & "_action.asp?" & request.querystring
			'store sql
			Session( "sql_string" ) = Request( "sql_string" )
			Session( "table_name" ) = Request( "table_name" )

		Case "QUERY_STRING"

			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp?" & request.querystring
			
		Case "PARENT_INDEX"
			'LJB 4/9/2001 For entering this functionality from the MainPage.asp only -			
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/ParentIndex_form.asp?formmode=list&formgroup=ParentIndex_form_group&dbname=" & dbkey & "&dataaction=retrieve_all"

		Case "MANAGE_SECURITY"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/cs_security/manage_security/manage_security_input_form.asp?nav_override=true&formgroup=manage_users_form_group&formmode=manage_users&dbname=" & dbkey

		Case "CHANGE_PASSWORD"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/cs_security/manage_security/manage_pwd_result_form.asp?nav_override=true&formgroup=manage_users_form_group&formmode=manage_passwords&dbname=" & dbkey
		
		Case "DB"
				Session( "CurrentLocation" & dbkey & formgroup ) = ""
				thearray = Application( formgroup & dbkey )
				'CONST kInputFormMode = 3
				formmode = thearray( kInputFormMode )
				sPath = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?formgroup=" & formgroup & "&dataaction=db&formmode=" & formmode & "&dbname=" & dbkey
		
		Case Else
			
			if not Session( "CurrentLocation" & dbkey & formgroup )<> "" then
				Session( "CurrentLocation" & dbkey & formgroup ) = ""
				thearray = Application( formgroup & dbkey )
				'CONST kInputFormMode = 3
				formmode = thearray( kInputFormMode )
				sPath = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?formgroup=" & formgroup & "&dataaction=db&formmode=" & formmode & "&dbname=" & dbkey
			
			end if

	End Select

	
	wizard = request("wizard")
	Session("wizard") = wizard
	
	' Add a tag to the query string to indicate its origin.
	sNewPath = URLWithMadeInArgument( sPath, "DDDefault" )
	sPath = sNewPath

	'Response.Write "1" & sPath & "<br>"
	'Response.End

	' Make sure the query string in the path is different from the last time, to make sure the
	' browser reloads the page instead of simply taking it from the cache.
	sNewPath = MakeQueryStringBeDifferent( sPath )
	
	sPath = sNewPath

	'Response.Write sPath
	

	' Set CurrentLocation if it doesn't already have a value.
	if "" = Session( "CurrentLocation" & dbkey & formgroup ) then
		Session( "CurrentLocation" & dbkey & formgroup ) = sPath
	end if
	'Response.Write Session( "CurrentLocation" & dbkey & formgroup )
	'Response.End
	
%>
<html>

<head>
	<title>Drug Degradation Database</title>

</head>

<!--<frameset  border = "0"  frameborder = "0"  framespacing = "0"  rows = "168,*">-->
<frameset  border = "0"  frameborder = "0"  framespacing = "0"  rows = "218,*">
	<frame  border = "0"  scrolling = "no"  src = "javascript:''"  name = "navbar">
	<frameset  border = "0"  frameborder = "0"  framespacing = "0"  cols = "100,*">
		<frameset  border = "0"  frameborder = "0"  framespacing = "0"  rows = "300,*">
			<frame  border = "0"  marginwidth = "0"  marginheight = "0"  src = "<%=userinfopath%>"  name = "userinfo"  scrolling = "auto">
			<frame  border = "0"  marginwidth = "0"  marginheight = "0"  src = "helper.asp"  name = "helper"  scrolling = "auto">
		</frameset>
		<frame  border = "0"  marginwidth = "0"  marginheight = "0"  src = "<%=Session( "CurrentLocation" & dbkey & formgroup )  %>"  name = "main">
	</frameset>
	<noframes>
		<body>
		</body>
	</noframes>
</frameset>

</html>
<%else 'db not loaded
	Response.Write "The web database requested did not initialize.<br>"
	Response.Write "For details, see the log file at <a href=" & Application( "AppPathHTTP" ) & "/logfiles/" & dbkey & "log.html" & ">" & Application( "AppPathHTTP" ) & "/logfiles/" & dbkey & "log.html</a>"
end if
%>
