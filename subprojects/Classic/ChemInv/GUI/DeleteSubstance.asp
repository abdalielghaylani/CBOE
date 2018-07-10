<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/adovbs.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%
Dim Conn
ManageMode = Request.QueryString("ManageMode")
CompoundID = Request("CompoundID")
if ManageMode = "1" then 
	Session("GUIReturnURL") = "/cheminv/inputtoggle.asp?formgroup=substances_form_group&dbname=cheminv&GotoCurrentLocation=true"
Else
	Session("GUIReturnURL") = ""
End if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete an Inventory Substance</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="CalculateFromPlugin.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
	focus();
</script>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	
	function ValidateDelete(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		// LocationID is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location to delete is required is required.\r";
			bWriteError = true;
		}
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		var bcontinue = true;
		if (document.form1.Recursively.checked){
			bcontinue = confirm("You are about to delete this location and all related sublocations and containers. This may include:\r-All Locations below this one.\r-All Locations on the same location grid as this one.\r-All containers below all those locations.\rDo you really want to continue?");
		}
		if (bcontinue) document.form1.submit();
	}
	
-->
</script>
</head>
<body>

<center>
<span class="GUIFeedback">Are you sure you want to delete this substance?</span>
<form name="form1" action="DeleteSubstance_action.asp" method="POST">
<input type="hidden" name="ManageMode" value="<%=ManageMode%>">
<%
inLineMarker = "data:chemical/x-cdx;base64,"
GetSubstanceAttributesFromDb(CompoundID)
bConflicts = false
if ConflictingFields <> "" then 
	hdrText = "<font color=red>Warning: Duplicate Substance</font>"
	bConflicts = true
End if
DisplaySubstance "", hdrText, false, false, false, false, false, inLineMarker & dBStructure
Response.Write GetCancelButton()
%>
<input type="hidden" name="CompoundID" value="<%=CompoundID%>">
<input type="image" src="/cheminv/graphics/ok_dialog_btn.gif">

</form>
</center>
</body>
</html>
