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
Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
'connDB.Open( "PROVIDER=Microsoft.Jet.OLEDB.4.0; DATA SOURCE=C:\Inetpub\wwwroot\ChemOffice\DrugDeg\DrugDegDB\Alsante2000.mdb" )
if 0 <> err.number then
	' The connection couldn't be opened.
	Set connDB = nothing
	connDB = ""

	' Redirect to an error dialog.
	Response.Redirect( "db-not-open-error.html" )
end if

' Successfully opened the connection to the database.

' Get the parent key.
keyParent = Clng( Request.QueryString( "keyprimary" ) )

' Delete the parent.
DeleteParent keyParent, connDB

' Update the count of records in the parent table.
UpdateTableRecordCount dbkey, formgroup, "DRUGDEG_PARENTS", connDB

' Close the database connection.
connDB.Close

' Announce that we did indeed delete the parent.
ShowMessageDialog( "Parent deleted" )

' Go back to the parent detail display.
Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToDrugDegSearch" & dbkey )
dbkey = Request.QueryString( "dbname" )
DoRedirect dbkey, Session( "ReturnToDrugDegSearch" & dbkey )
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>

</html>
