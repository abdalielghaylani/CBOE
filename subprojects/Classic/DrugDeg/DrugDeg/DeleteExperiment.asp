<%@ LANGUAGE=VBScript %>
<% ' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved %>

<html>

<head>
	<title></title>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</head>

<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!-- end the COWS input form -->
</form>
<%
' Connect to the database.
Dim	connDB
Set connDB = Server.CreateObject( "ADODB.Connection" )
connDB.Mode = adModeReadWrite
Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_EXPTS" )
if 0 <> err.number then
	' The connection couldn't be opened.
	Set connDB = nothing
	connDB = ""

	' Redirect to an error dialog.
	Response.Redirect( "db-not-open-error.html" )
end if

' Successfully opened the connection to the database.

' Get the experiment key.
keyExpt = Clng( Request.QueryString( "keyprimary" ) )

' Delete the experiment.
DeleteExperiment keyExpt, connDB

connDB.Close

' Announce that we did indeed delete the experiment.
ShowMessageDialog( "Experiment deleted" )


' After deleting the experiment we are to go back to the parent details page.

' Make sure the page reloads when you get to the parent details.
sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToParentDetails" & dbkey ) )
Session( "ReturnToParentDetails" & dbkey ) = sNewPath

' Set CurrentLocation now, so that refreshing will work when we get to there.
Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToParentDetails" & dbkey )

' Redirect to the parent details page.
DoRedirect dbkey, Session( "ReturnToParentDetails" & dbkey )	
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>

</html>
