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
	userinfopath = URLWithMadeInArgument( userinfopath, "DDDefault2" )
	
	Select Case UCase(dataaction)
		Case "MAINPAGE"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/mainpage.asp?formgroup=base_form_group&dbname=" & dbkey

		Case "LOGIN"
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			'sPath = "/" & Application( "AppKey" ) & "/login.asp?formgroup=base_form_group&dbname=" & dbkey
			sPath = "/cs_security/login.asp"

		Case "PARENT_FULL_COMMIT"
			' LJB 4/9/2001 For entering this functionality from the MainPage.asp only:
			' - clear last search array so after add, you go to clean details view with only the new record
			' - changed formmode to be consistent with action
			' - clear salt list.
			thearray = Session( "Base_RS" & dbkey & formgroup )
			thearray= ""
			Session( "Base_RS" & dbkey & formgroup ) = thearray
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/AddParent_input_form.asp?record_added=false&formmode=add_parent&formgroup=AddParent_form_group&dbname=" & dbkey & "&commit_type=full_commit"
			Session( "SelectedSalts" & dbkey ) = ""

		Case "EXPERIMENT_FULL_COMMIT"
			' LJB 4/9/2001 For entering this functionality from the MainPage.asp only:
			' - clear last search array so after add, you go to clean details view with only the new record
			' - changed formmode to be consistent with action
			thearray = Session( "Base_RS" & dbkey & formgroup )
			thearray= ""
			Session( "Base_RS" & dbkey & formgroup ) = thearray
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/AddExperiment_input_form.asp?record_added=false&formmode=add_experiment&formgroup=AddExperiment_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"

		Case "DEGRADANT_FULL_COMMIT"
			' LJB 4/9/2001 For entering this functionality from the MainPage.asp only:
			' - clear last search array so after add, you go to clean details view with only the new record
			' - changed formmode to be consistent with action
			thearray = Session( "Base_RS" & dbkey & formgroup )
			thearray= ""
			Session( "Base_RS" & dbkey & formgroup ) = thearray
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/AddDegradant_input_form.asp?record_added=false&formmode=add_degradant&formgroup=AddDegradant_form_group&dbname=" & dbkey & "&commit_type=full_commit"


		Case "CONDITION_LIST_ADMIN"
			' For entering this functionality from the MainPage.asp only:
			' - clear last search array so after add, you go to clean details view with only the new record
			' - changed formmode to be consistent with action
			thearray = Session( "Base_RS" & dbkey & formgroup )
			thearray= ""
			Session( "Base_RS" & dbkey & formgroup ) = thearray
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/Condition_input_form.asp?record_added=false&formmode=add_condition&formgroup=Condition_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"

		Case "FGROUP_LIST_ADMIN"
			' For entering this functionality from the MainPage.asp only:
			' - clear last search array so after add, you go to clean details view with only the new record
			' - changed formmode to be consistent with action
			thearray = Session( "Base_RS" & dbkey & formgroup )
			thearray= ""
			Session( "Base_RS" & dbkey & formgroup ) = thearray
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/FunctionalGroup_input_form.asp?record_added=false&formmode=add_fgroup&formgroup=FGroup_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"

		Case "STATUS_LIST_ADMIN"
			' For entering this functionality from the MainPage.asp only:
			' - clear last search array so after add, you go to clean details view with only the new record
			' - changed formmode to be consistent with action
			thearray = Session( "Base_RS" & dbkey & formgroup )
			thearray= ""
			Session( "Base_RS" & dbkey & formgroup ) = thearray
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/Status_input_form.asp?record_added=false&formmode=add_status&formgroup=Status_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"

		Case "SALT_LIST_ADMIN"
			' For entering this functionality from the MainPage.asp only:
			' - clear last search array so after add, you go to clean details view with only the new record
			' - changed formmode to be consistent with action
			thearray = Session( "Base_RS" & dbkey & formgroup )
			thearray= ""
			Session( "Base_RS" & dbkey & formgroup ) = thearray
			Session( "CurrentLocation" & dbkey & formgroup ) = ""
			sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/Salt_input_form.asp?record_added=false&formmode=add_salt&formgroup=Salt_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"


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
	sNewPath = URLWithMadeInArgument( sPath, "DDDefault2" )
	sPath = sNewPath

	' Make sure the query string in the path is different from the last time, to make sure the
	' browser reloads the page instead of simply taking it from the cache.
	sNewPath = MakeQueryStringBeDifferent( sPath )
	sPath = sNewPath

	' Set CurrentLocation if it doesn't already have a value.
	if "" = Session( "CurrentLocation" & dbkey & formgroup ) then
		Session( "CurrentLocation" & dbkey & formgroup ) = sPath
	end if
	wizard = request("wizard")
	Session("wizard") = wizard
%>
<html>

<head>
	<title>Frameset Page </title>
</head>

<frameset  border = "0"  frameborder = "0"  framespacing = "0"  rows = "168,*">
	<frame  border = "0"  scrolling = "no"  src = "javascript:''"  name = "navbar">
	<frameset  border = "0"  frameborder = "0"  framespacing = "0"  cols = "100,*">
		<frameset  border = "0"  frameborder = "0"  framespacing = "0"  rows = "300,*">
			<frame  border = "0"  marginwidth = "0"  marginheight = "0"  src = "<%=userinfopath%>"  name = "userinfo"  scrolling = "auto">
			<frame  border = "0"  marginwidth = "0"  marginheight = "0"  src = "helper.asp"  name = "helper"  scrolling = "auto">
		</frameset>
		<frame  border = "0"  marginwidth = "0"  marginheight = "0"  src = "<%=Session( "CurrentLocation" & dbkey & formgroup )  %>"  name = "main">
	</frameset>
	<noframes>
		<body <%=Application("BODY_BACKGROUND")%>>
		</body>
	</noframes>
</frameset>

</html>
<%else 'db not loaded
	Response.Write "The web database requested did not initialize.<br>"
	Response.Write "For details, see the log file at <a href=" & Application( "AppPathHTTP" ) & "/logfiles/" & dbkey & "log.html" & ">" & Application( "AppPathHTTP" ) & "/logfiles/" & dbkey & "log.html</a>"
end if
%>
