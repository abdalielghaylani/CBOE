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
Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_CONDS" )
if 0 <> err.number then
	' The connection couldn't be opened.
	Set connDB = nothing
	connDB = ""

	' Redirect to an error dialog.
	Response.Redirect( "db-not-open-error.html" )
end if

' Successfully opened the connection to the database.

' Get the condition key.
'keyCond = CInt( Request.QueryString( "keyprimary" ) )
keyCond = Request.QueryString( "keyprimary" )

Dim	rsCondition
Set rsCondition = Server.CreateObject( "ADODB.Recordset" )
sSQL = "select * from DRUGDEG_CONDS where DEG_COND_KEY = " & keyCond
rsCondition.Open sSQL, connDB
if not rsCondition.BOF and not rsCondition.EOF then
	' Make the message announcing the deletion _before_ we delete the record.
	sMessage = "The condition, '" & rsCondition.Fields( "DEG_COND_TEXT" ) & "', was deleted."

	' There is a condition for that key.  Delete the condition.
	DeleteCondition keyCond, connDB

	' Announce that we did indeed delete the condition.
	ShowMessageDialog( sMessage )
else
	sMessage = "No matching conditions\n"
	sMessage = sMessage & "QueryString = '" & Request.QueryString & "'"
	ShowMessageDialog( sMessage )
end if

' Close the recordset.
rsCondition.Close

' Close the data connection.
connDB.Close


' After deleting the condition we are to go back to the conditions list page.

' Make sure the page reloads when you get to the conditions list.
sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToConditionListAdmin" & dbkey ) )
Session( "ReturnToConditionListAdmin" & dbkey ) = sNewPath

' Set CurrentLocation now, so that refreshing will work when we get to there.
Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToConditionListAdmin" & dbkey )

' Redirect to the conditions list page.
DoRedirect dbkey, Session( "ReturnToConditionListAdmin" & dbkey )	
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>

</html>
