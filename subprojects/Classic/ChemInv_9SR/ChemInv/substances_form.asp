<%@ LANGUAGE=VBScript  %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%response.expires = 0%>
<%' Copyright 2001-2002, CambridgeSoft Corp., All Rights Reserved%>
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if				
%>
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
	
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
	var CD_AUTODOWNLOAD_PLUGIN = "<%=APPLICATION("CD_PLUGIN_DOWNLOAD_PATH")%>";
	var holdTime = 3000;
	if (cd_getBrowserVersion() >= 6) holdTime = 1;
	window.onload = function(){setTimeout("GetFormula();GetMolWeight();",holdTime)}
	
	// This function is called by recordset footer at the end of onload event
	function AfterOnLoad(){
		// Calculate the formula and molw from the plugin data
		var holdTime = 3000;
		if (cd_getBrowserVersion() >= 6) holdTime = 1;
		setTimeout("GetFormula();GetMolWeight();",holdTime);
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
CompoundID = dbCompoundID
theCAS = dbCAS
theSN = Server.URLEncode(dbSubstanceName)
%>
<center>
<table border="0" width="550">
	<tr>
		<td colspan="4" align="right">
			<%If Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then%>
			<a class="MenuLink" href="View%20history%20for%20this%20substance" onclick="OpenDialog('/cheminv/gui/AuditReport_frset.asp?ft=std&amp;filter=true&amp;CompoundID=<%=BaseID%>&amp;recordNum=1', 'Synonyms_Window', 2); return false;" title="View history for this substance">View History</a>
			<%end if%>
			| 
			<a class="MenuLink" href="Manage%20synonyms%20for%20this%20substance" onclick="OpenDialog('/cheminv/gui/manageSynonyms.asp?CompoundID=<%=BaseID%>&amp;recordNum=1', 'Synonyms_Window', 1); return false;" title="Look up synonyms for this substance">Manage Synonyms</a>
			<%If Session("INV_MANAGE_LINKS" & dbkey) then%>
				<%'SYAN added 3/2/2004 to link to docmanager%>
					<%IF CBool(Application("SHOW_DOCMANAGER_LINK")) then%>
						|
						<%If Session("SEARCH_DOCS" & dbkey) then%>
						<a class="MenuLink" href="Manage%20Documents%20for%20this%20substance" onclick="OpenDialog('/cheminv/gui/manageDocuments.asp?FK_value=<%=BaseID%>&amp;FK_name=COMPOUND_ID&amp;Table_Name=INV_COMPOUNDS&amp;LINK_TYPE=CHEMINVCOMPOUNDID', 'Documents_Window', 2); return false;" title="Manage documents associated to this substance">Manage Documents</a>
						<%else%>
						<a class="MenuLink" href="Manage%20Documents%20for%20this%20substance" onclick="alert('You do not have the appropriate privileges to add or view documents. Please ask the administrators to grant you DOCMGR_EXTERNAL role and log back in to try again.'); return false;">Manage Documents</a>
						<%end if%>
					<%end if%>
					<%'End of SYAN modification%>

			|
			<a class="MenuLink" href="Manage%20Links%20for%20this%20substance" onclick="OpenDialog('/cheminv/gui/manageLinks.asp?FK_value=<%=BaseID%>&amp;FK_name=COMPOUND_ID&amp;Table_Name=INV_COMPOUNDS&amp;URLType=MSDS', 'Links_Window', 2); return false;" title="Manage links associated to this substance">Manage Links</a>
			<%end if%>					
		</td>
	</tr>
	
<%if Session("IsPopUP") then%>
<tr>
	<td colspan="2" align="right">
		<input type="hidden" name="SubstanceName" value="<%=SubstanceName%>"> 
		<a HREF="#" onclick="top.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="DoSelectSubstance(<%=BaseID%>, document.cows_input_form.SubstanceName.value); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
	</td>
</tr>
<%end if%>
</table>
</center>	
<%CloseRS(BaseRS)%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/GetEHSAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/displayEHSData.asp"-->
<center>
<table width="60%">
	<tr>
		<td align="right">
			<%If Session("INV_EDIT_EHS_DATA" & dbkey) then%>			
				<%If EHSFoundData = "1" Then%>
				<a class="MenuLink" HREF="Edit%20Substance%20EH&amp;S%20Info" onclick="OpenDialog('/Cheminv/custom/GUI/EditEHSInfo.asp?TrackBy=CAS&amp;action=edit&amp;SN=<%=theSN%>&amp;CAS=<%=theCAS%>&amp;CompoundID=<%=CompoundID%>', 'EHSDiag', 1); return false">Edit EH&amp;S Data</a>		
				<%elseif theCAS <> "" then %>
				<a class="MenuLink" HREF="Add%20Substance%20EH&amp;S%20Info" onclick="OpenDialog('/Cheminv/custom/GUI/EditEHSInfo.asp?TrackBy=CAS&amp;action=add&amp;SN=<%=theSN%>&amp;CAS=<%=theCAS%>&amp;CompoundID=<%=CompoundID%>', 'EHSDiag', 1); return false">Add EH&amp;S Data</a>			
				<%else%>
				<a class="MenuLink" HREF="Add%20Substance%20EH&amp;S%20Info" onclick="alert('Cannot add EH&amp;S data to this substance because it does not have a CAS number');return false">Add EH&amp;S Data</a>
				<%end if%>
			<%End if%>
		</td>
	</tr>
	<%if EHSFoundData = "1" then%>
	<tr>
		<%If EHSIsDefaultSource = "1" Then%>
		<td colspan="7" align="center"><em>(Note: displayed values are the defaults for this substance)</em></td>
		<%ElseIf EHSIsDefaultSource = "0" Then%>
		<td colspan="7" align="center"><em>(Note: displayed values are for this particular supplier &amp; product, not the defaults for this substance)</em></td>
		<%Else%>
		<td colspan="7" align="center"><em>(Note: displayed values are from another product with the same substance)</em></td>
		<%End If%>
	</tr>
	<%end if%>
	</table>
</center>	
</body>

</html>
