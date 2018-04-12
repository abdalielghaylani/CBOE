<%@ LANGUAGE=VBScript %>
<html>

<head>
</head>

<body <%=Application("BODY_BACKGROUND")%>>

<%
'JHS commented out 3/25/2003
'sCalledBy = UCase( Request.QueryString( "calledby" ) )
'if "" = sCalledBy then
'	sCalledBy = "INTERNAL"
'end if
'JHS commented out 3/25/2003 end

'JHS commented out 3/25/2003
'if "INTERNAL" <> sCalledBy then
	' We came at this page from elsewhere.  We need to set up some dummy frames so we don't
	' get complaints about missing frames in some of the COWS files.
%>
<!---frameset  border = "0"  frameborder = "0"  framespacing = "0"  rows = "1,1,1,1">
	<frame  name = "navbar"    border = "0"  marginwidth = "0"  marginheight = "0">
	<frame  name = "userinfo"  border = "0"  marginwidth = "0"  marginheight = "0">
	<frame  name = "helper"    border = "0"  marginwidth = "0"  marginheight = "0">
	<frame  name = "main"      border = "0"  marginwidth = "0"  marginheight = "0">

	<noframes>
		<body>
		</body>
	</noframes>
</frameset--->
<%
'end if
'JHS commented out 3/25/2003 end

%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<%
dbkey = Request.QueryString( "dbname" )
formgroup = Request.QueryString( "formgroup" )

Session( "UserName" & dbkey ) = "DrugDeg_t1"
Session( "UserID" & dbkey ) = "DrugDeg_t1"

' Set up a variable for the parent key, since we use it in a few places.
BaseID = Request.QueryString( "keyparent" )

' Connect to the database.
' set connDrugDeg = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
MakeNewConnectionToDrugDegDB connDrugDeg

' Make and fill a record set for the parent.
set rsParent = Server.CreateObject( "ADODB.Recordset" )
sSQL = "SELECT * FROM DrugDeg_Parents WHERE Parent_Cmpd_Key = " & BaseID
rsParent.Open sSQL, connDrugDeg

' Get the base64_CDX data for the parent compound.
Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connDrugDeg

' Make sure you have the first parent compound record.  There shouldn't be more than one,
' but better to be sure...
rsParent.MoveFirst

' Set variables for the height and width of the structure field.
heightStructArea = 230
widthStructArea = 230
%>

<!--#INCLUDE VIRTUAL = "/DrugDeg/DrugDeg/Parent_details_form-View.asp"-->

<!--
<table  border = 2>
	<tr>
		<td  align = "center">
<%
			Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
			rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connDrugDeg
			ShowResult dbkey, formgroup, rsBase64, "DRUGDEG_BASE64.BASE64_CDX", "Base64CDX", widthStructArea, heightStructArea
			rsBase64.Close
%>
		</td>
	</tr>
</table>
-->

<%
' Close the Base64 record set.
CloseRS( rsBase64 )

' Close the record set for the parent.
CloseRS( rsParent )

' Close the database connection.
CloseConn( connDrugDeg )
%>

</body>

</html>
