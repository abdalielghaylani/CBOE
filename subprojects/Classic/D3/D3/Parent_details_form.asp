<%@ LANGUAGE=VBScript  %>
<%response.expires = 0%>
<%' Copyright 1998-2002, CambridgeSoft Corp., All Rights Reserved%>
<%
'ShowMessageDialog( "Got here" )

'ShowMessageDialog( "Session( 'LoginRequired' & dbkey ) = " & Session( "LoginRequired" & dbkey ) )
'ShowMessageDialog( "Session( 'UserValidated' & dbkey ) = " & Session( "UserValidated" & dbkey ) )
'MRE added clear this session so that we will see the results list again when we do a search
Session("addParent") = ""

			
if Session( "LoginRequired" & dbkey ) = 1 then
	if Not Session( "UserValidated" & dbkey ) = 1 then
		response.redirect "/" & Application("Appkey") & "/logged_out.asp"
	end if
end if
%><html>

<head>
	<script language="javascript">
		var baseid = ""
	</script>
	
	<title>Parent details view</title>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
	<!--#INCLUDE FILE = "../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
	<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
	<link REL="stylesheet" TYPE="text/css" HREF="/<%=Application("appkey")%>/styles.css">
</head>

<body  <%=Application("BODY_BACKGROUND")%>>

<script language = "javascript">
<!-- Hide from older browsers
	//if(formmode.toLowerCase = "edit_record"){
		document.forms["cows_input_form"].onSubmit = "removeFromRelFields()"
	//}
// end script hiding -->
</script> 
<%

'JHS commented out 3/25/2003
'sCalledBy = UCase( Request.QueryString( "calledby" ) )
'if "" = sCalledBy then
'	sCalledBy = "INTERNAL"
'end if
'JHS commented out 3/25/2003 end

' BaseID represents the primary key in the recordset for the current record.
'     It may be passed in via "keyparent" in the query string, or it may be set
'     in form_action_vbs.asp.
if 0 < Request.QueryString( "keyparent" ).Count then
	' There _is_ a "keyparent" entry in the query string, so get BaseID from "keyparent".
	BaseID = Request.QueryString( "keyparent" )( 1 )
end if

' Set up a session variable for this as the current page displayed.  Tack on "keyparent".
Session( "ReturnToCurrentPage" & dbkey ) = lcase(Session("CurrentLocation" & dbkey & formgroup)) & "&keyparent=" & BaseID

' Make and fill a session variable to be used for getting back to this display
' with the proper querystring parameter settings.
' CAP 26 Apr 2001: Added the test of formmode to make sure the return location is that
'     of the _display_, not _edit_, form of the page.
if "edit_record" <> lcase(formmode) then
	Session( "ReturnToParentDetails" & dbkey ) = Session( "ReturnToCurrentPage" & dbkey )
end if
Session( "ReturnToParentDetails" & dbkey ) = Session( "ReturnToCurrentPage" & dbkey )
Session( "ReturnToExperimentDetails" & dbkey ) = Session( "ReturnToParentDetails" & dbkey )
' Set variables for the height and width of the structure area.  This
' makes it easier to change them (no searching around in the code).
heightStructArea = 230
widthStructArea = 230

Set connD3 = GetConnection( dbkey, formgroup, "D3_PARENTS" )

Dim	rsParent
Set rsParent = Server.CreateObject( "ADODB.Recordset" )
sSQL = GetDisplaySQL( dbkey, formgroup, "D3_PARENTS.*", "D3_PARENTS", "", BaseID, "" )

rsParent.Open sSQL, connD3

' Get the base64_CDX data for the parent compound.
Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
rsBase64.Open "Select * from D3_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connD3

commit_type = "full_commit"

' Set up a session variable for the primary key of the object (parent) being displayed.
Session( "PrimaryKey" & dbkey ) = rsParent.Fields( "PARENT_CMPD_KEY" )
Session( "EDIT_THIS_RECORD" & dbkey ) = IsRecordEditable("D3_PARENTS", "PARENT_CMPD_KEY", rsParent.Fields( "PARENT_CMPD_KEY" ), connD3)
%>

	<script language="javascript">
		var baseid = <%=baseid%>
		var Edit_This_Record = "<%=Session( "EDIT_THIS_RECORD" & dbkey )%>"
	</script>
<script language="JavaScript">
	var commit_type = "<%=commit_type%>"
	var formmode = "<%=lcase(formmode)%>"
	var uniqueid = "<%=baseid%>"
	windowloaded = false
	
</script>


<%

if request("degs") = "true" then
	
%>
<!--#INCLUDE FILE="Parent_details_form-Degs.asp"-->
<%elseif "edit_record" <> LCase( formmode ) then
' The user is not editting the parent information.%>
<!--#INCLUDE FILE="Parent_details_form-View.asp"-->
<%
end if  ' if the user is editting the parent information...
%>

&nbsp;<br>

<%
CloseRS( rsBase64 )
CloseRS( rsParent )
CloseRS( rsExperiment )

CloseConn( connD3 )

%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>
</html>
