<%@ LANGUAGE=VBScript %>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
</head>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->

<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<%
' Set up a session variable for this as the current page displayed.
Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' Make and fill a session variable to be used for getting back to this display
' with the proper querystring parameter settings.
' CAP 26 Apr 2001: Added the test of formmode to allow cancelled degradant
'	edit sessions to return to the _display_ of degradant details.
if "edit_record" <> formmode then
	Session( "ReturnToMechanismDetails" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )
end if

' Set up a variable for the degradant key, since we use it in a few places.
keyDeg = Request.QueryString( "keydeg" )

' Connect to the database.
set connChem = GetConnection( dbkey, formgroup, "D3_MECHS" )

' Make and fill a record set for the mechanism.
set rsMechanism = Server.CreateObject( "ADODB.Recordset" )
rsMechanism.Open "Select * from D3_MECHS where DEG_CMPD_FK = " & keyDeg, connChem

' Make sure you have the first record.  There shouldn't be more than one, but better to be sure...
rsMechanism.MoveFirst

' Set up a session variable for the primary key of the object (mechanism) being displayed.
Session( "PrimaryKey" & dbkey ) = rsMechanism.Fields( "MECH_KEY" )

' Set variables for the height and width of the mechanism drawing.
heightMechDrawing = 530
widthMechDrawing = 530
%>

<table  border = 2>
	<tr>
		<td  align = "center">
<%
			Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
			rsBase64.Open "Select * from D3_BASE64 where MOL_ID = " & rsMechanism.Fields( "MOL_ID" ), connChem
			ShowResult dbkey, formgroup, rsBase64, "D3_BASE64.BASE64_CDX", "Base64CDX", widthMechDrawing, heightMechDrawing
			rsBase64.Close
%>
		</td>
	</tr>
</table>

<%
' Close the record set for the mechanism.
rsMechanism.Close

' Close the database connection.
connChem.Close
%>

<!-- START: MUST HAVE ALL OF THE FOLLOWING FROM THE FOOTER FILE SINCE NO RECORDSET IS BEING USED -->
</form>
<%WriteAppletCode()%>

<form name = "nav_variables" method = "post" Action = "<%=Session("CurrentLocation" & dbkey & formgroup)%>">
<input type = "hidden" name = "RecordRange" Value = "<%=Session("RecordRange" & dbkey & formgroup)%>">
<input type = "hidden" name = "CurrentRecord" Value = "<%=Session("RecordRange" & dbkey & formgroup)%>">
<input type = "hidden" name = "AtStart" Value = "<%=Session("AtStart" & dbkey & formgroup)%>">
<input type = "hidden" name = "AtEnd" Value = "<%=Session("AtEnd" & dbkey & formgroup)%>">
<input type = "hidden" name = "Base_RSRecordCount" Value = "<%=Session("Base_RSRecordCount" & dbkey & formgroup)%>">
<input type = "hidden" name = "TotalRecords" Value = "<%=Session("Base_RSRecordCount" & dbkey & formgroup)%>">
<input type = "hidden" name = "PagingMove" Value = "<%=Session("PagingMove" & dbkey & formgroup)%>">
<input type = "hidden" name = "CommitType" Value = "<%=commit_type%>">
<input type = "hidden" name = "TableName" Value = "<%=table_name%>">
<input type = "hidden" name = "UniqueID" Value = "<%=uniqueid%>">
<input type = "hidden" name = "BaseID" Value = "<%=BaseID%>">
<input type = "hidden" name = "CPDDBCounter" Value = "<%=cpdDBCounter%>">
<input type = "hidden" name = "CurrentIndex" Value = "<%=currentindex%>">
<input type = "hidden" name = "BaseActualIndex" Value = "<%=BaseActualIndex%>">
<input type = "hidden" name = "Base64" Value = "<%=Session("BASE64_CDX" & uniqueid & dbkey & formgroup)%>">
<input type = "hidden" name = "Stored_Location" Value = "">

</form>
<script language = "javascript">
	window.onload = function(){loadframes()}
	function loadframes(){
		loadUserInfoFrame()
		loadNavBarFrame()

		DoAfterOnLoad ? AfterOnLoad():true;
	}
</script>
<!-- END: MUST HAVE ALL OF THE FOLLOWING FROM THE FOOTER FILE SINCE NO RECORDSET IS BEING USED -->

</body>

</html>
