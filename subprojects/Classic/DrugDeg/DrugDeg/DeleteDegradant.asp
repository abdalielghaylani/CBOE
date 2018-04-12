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
Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_DEGS" )
if 0 <> err.number then
	' The connection couldn't be opened.
	Set connDB = nothing
	connDB = ""

	' Redirect to an error dialog.
	Response.Redirect( "db-not-open-error.html" )
end if

' Successfully opened the connection to the database.

' Get the degradant compound key.
keyDegCmpd = CLng( Request.QueryString( "keyprimary" ) )

' Delete the degradant.
DeleteDegradant keyDegCmpd, connDB

' Close the data connection.
connDB.Close

' Announce that we did indeed delete the degradant.
ShowMessageDialog( "Degradant deleted" )


' After deleting the degradant we are to go back to the experiment details page.

' Make sure the page reloads when you get to the experiment details.
sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToExperimentDetails" & dbkey ) )
Session( "ReturnToExperimentDetails" & dbkey ) = sNewPath

' Set CurrentLocation now, so that refreshing will work when we get to there.
Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToExperimentDetails" & dbkey )

' Redirect to the experiment details page.
DoRedirect dbkey, Session( "ReturnToExperimentDetails" & dbkey )	
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>

</html>
