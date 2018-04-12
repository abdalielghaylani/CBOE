<%@ LANGUAGE=VBScript  %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%response.expires = 0%>
<%' Copyright 2001-2002, CambridgeSoft Corp., All Rights Reserved%>
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<script LANGUAGE="javascript" src="../gui/CalculateFromPlugin.js"></script>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title><%=Application("appTitle")%> Substances -- Form View</title>
<script LANGUAGE="javascript">
<!--
	// This flag tells the onload function called from recordset_footer to process the AfterOnLoad() function
	DoAfterOnLoad = true;
	
	// unhide the search link
	if (!top.bannerFrame.document.all.searchLink){
	}
	else{
		top.bannerFrame.document.all.searchLink.style.visibility = "visible";
	}
	
	var holdTime = 3000;
	<%if Session("isCDP") = "TRUE" then%>
	if (cd_getBrowserVersion() >= 6) holdTime = 1;
	window.onload = function(){setTimeout("GetFormula();GetMolWeight();",holdTime)}
	<%end if%>
	// This function is called by recordset footer at the end of onload event
	function AfterOnLoad(){
		// Calculate the formula and molw from the plugin data
		var holdTime = 3000;
		<%if Session("isCDP") = "TRUE" then%>
		if (cd_getBrowserVersion() >= 6) holdTime = 1;
		setTimeout("GetFormula();GetMolWeight();",holdTime);
		<%end if%>
	}		
//-->
</script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>

</head>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%
Dim Conn
Dim RS

inLineMarker = "data:chemical/x-cdx;base64,"
GetSubstanceAttributesFromDb(baseID)

bConflicts = false
if ConflictingFields <> "" then 
	hdrText = "<font color=red>Warning: Duplicate Substance</font>"
	bConflicts = true
End if
DisplaySubstance "", hdrText, false, false, false, false, false, inLineMarker & dBStructure
%>
<center>
<table border="0" width="550">
	<tr>
		<td colspan="4" align="right">
			<%If Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then%>
			<a class="MenuLink" href="View%20history%20for%20this%20substance" onclick="OpenDialog('/cheminv/gui/AuditReport_frset.asp?ft=std&amp;filter=true&amp;CompoundID=<%=BaseID%>&amp;recordNum=1', 'Synonyms_Window', 2); return false;" title="View history for this substance">View History</a>
			<%end if%>				
		</td>
	</tr>
		
<%'if Session("IsPopUP") then%>
<tr>
	<td colspan="2" align="right">
		<input type="hidden" name="SubstanceName" value="<%=dbSubstanceName%>"> 
		<a HREF="#" onclick="top.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
		<a HREF="#" onclick="DoSelectSubstance(<%=BaseID%>, document.cows_input_form.SubstanceName.value); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
	</td>
</tr>
</table>
</center>
<%'end if
CloseRS(BaseRS)%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>

</html>
