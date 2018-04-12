<%@ LANGUAGE=VBScript %>
<%Response.Expires=0%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/DrugDeg/source/app_vbs.asp"-->
<%
formmode = Request( "formmode" )
dbkey = Request( "dbname" )
appkey = Application( "appkey" )
dataaction = Request( "dataaction" )
formgroup = Request( "formgroup" )
if Application( "Main_Page" & dbkey ) = "1" then
	mytest=Split(Request.ServerVariables( "HTTP_REFERER" ),"/",-1)
	on error resume next
	if UCase(mytest(Ubound(mytest))) = "CHEMOFFICE.ASP" then
		dataaction = "MAINPAGE"
	end if
	if err.number <> 0 then
		dataaction = "MAINPAGE"
	end if
	on error goto 0
end if

if Application( "LoginRequired" & dbkey )=1 and Session( "UserValidated" & dbkey )=0 then
	dataaction = "LOGIN"
end if
if formgroup = "" then formgroup = "base_form_group"
if Application( "DBLoaded" & dbkey ) = True then
	userinfopath = "/" & appkey & "/user_info.asp?dbname=" & dbkey & "&formgroup=" & formgroup
	userinfopath = URLWithMadeInArgument( userinfopath, "DDDefault" )

	Select Case UCase(dataaction)
		Case "MAINPAGE"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/mainpage.asp?formgroup=base_form_group&dbname=" & dbkey

		Case "VIEW_LINKED_PARENT"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			'sPath = "/" & Application( "AppKey" ) & "/login.asp?formgroup=base_form_group&dbname=" & dbkey
			'sPath = "/DrugDeg/drugdeg/Parent_details-Display_only.asp?dbname=drugdeg&formgroup=base_form_group&keyparent=" & rsParentLinks.Fields( "Parent_Cmpd_Key" ) & "&calledby=docmanager"

			sParentKey = Request( "keyparent" )
			'JHS commented out 3/25/2003
			'sCalledBy = Request( "calledby" )
			'sPath = "/DrugDeg/drugdeg/Parent_details-Display_only.asp?dbname=drugdeg&formgroup=base_form_group&dataaction=mainpage&keyparent=" & rsParentLinks.Fields( "Parent_Cmpd_Key" ) & "&calledby=docmanager"
			'sPath = sPath & "&keyparent=" & sParentKey & "&calledby=" & sCalledBy
			'JHS commented out 3/25/2003 end
			
			'JHS added 3/25/2003
			sPath = "/DrugDeg/drugdeg/Parent_details-Display_only.asp?dbname=drugdeg&formgroup=base_form_group&dataaction=mainpage&keyparent=" & rsParentLinks.Fields( "Parent_Cmpd_Key" )
			sPath = sPath & "&keyparent=" & sParentKey
			'JHS added 3/25/2003 end


		Case Else
			if not Session( "CurrentLocation" & dbkey & formgroup )<> "" then
				'Session( "CurrentLocation" & dbkey & formgroup ) = ""
				thearray = Application( formgroup & dbkey )
				CONST kInputFormMode = 3
				formmode = thearray( kInputFormMode )
				sPath = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?formgroup=" & formgroup & "&dataaction=db&formmode=" & formmode & "&dbname=" & dbkey
			end if
	End Select

	' Add a tag to the query string to indicate its origin.
	sNewPath = URLWithMadeInArgument( sPath, "DDDefault" )
	sPath = sNewPath

	' Make sure the query string in the path is different from the last time, to make sure the
	' browser reloads the page instead of simply taking it from the cache.
	sNewPath = MakeQueryStringBeDifferent( sPath )
	sPath = sNewPath

	' Set CurrentLocation if it doesn't already have a value.
	if "" = Session( "CurrentLocation" & dbkey & formgroup ) then
		Session( "CurrentLocation" & dbkey & formgroup ) = sPath
	end if
%>
<html>

<head>
	<title>Frameset Page</title>
</head>

<frameset  border = "0"  frameborder = "0"  framespacing = "0"  rows = "1,1,1,*">
	<frame  border = "0"  name = "navbar"    marginwidth = "0"  marginheight = "0">
	<frame  border = "0"  name = "userinfo"  marginwidth = "0"  marginheight = "0">
	<frame  border = "0"  name = "helper"    marginwidth = "0"  marginheight = "0">
	<frame  border = "0"  name = "main"      marginwidth = "0"  marginheight = "0"  src = "<%=Session( "CurrentLocation" & dbkey & formgroup )%>">
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
