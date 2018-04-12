<%@ LANGUAGE="VBScript" %>
<%a = b%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<title><%=Application("appTitle")%> -- Results-List View</title>
		<script language="JavaScript">
			var markbtnNamelist = "mark_record_red,unmark_record_red";
			var MarkedHitsbuttonname = "show_marked_red";
			var ShowLastbuttonname = "show_last_list_red";
			var ClearHitsbuttonname = "clear_marked_red";
		</script>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<script language="JavaScript">
			// Open the synonym window
			function openSynWindow(leftPos,topPos,CompoundID,recordNum){
				var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable,width=300,height=200,screenX=" + leftPos + ",screenY=" + topPos + ",left=" + leftPos + ",top=" + topPos;
				SynWindow = window.open("Synlookup.asp?CsNum=" + CompoundID + "&recordNum=" + recordNum,"Synonyms_Window",attribs);
			}
			
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
		</script>
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
	</head>
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<body bgcolor="#FFFFFF">
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/GUI/guiUtils.asp"-->
<%
if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
  listItemNumber = 0
  Response.Write "<span class=""GUIFeedback"">Select a ChemACX Substance<BR><BR></span>"
  Response.Write "<table border=""1"" bgcolor=""#FFFFFF"" align=""left"">"
  Response.Write "<tr>"
else
	Response.Write "<BR>"   
end if
%>
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cheminv/invacx/GetACXSubstanceAttributes.asp"-->
<%
plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
if  plugin_value  then
	displayType = "cdx"
	zoomFunction = "ACX_getStrucZoomBtn('Substance.Structure'," & BaseID & ",0,0,'zoom_red_btn.gif')"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('Substance.Structure'," & BaseID & ",600,450,'zoom_red_btn.gif')"
end if
%>
				<td align="center" valign="top" width="194" nowrap>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>					
							<td><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>);return false"><img src="<%=Application("NavButtonGifPath")%>names_red.gif" alt border="0"><nobr></a></td>				
							<!---<td><script language="JavaScript">getFormViewBtn("details_red.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit", "", "base_form_group&CompoundID=<%=BaseID%>&ClearNodes=1&sNode=1&Exp=Y#1")</script></td>-->
							<td>
								<script language="JavaScript">getFormViewBtn("details_red.gif","invacx_substanceselect_form.asp","<%=BaseActualIndex%>", "edit")</script></td>
							<td><script language="JavaScript">getMarkBtn(<%=BaseID%>);</script></td>
						</tr>
					</table>	
					<div align="center">
						<font size="1" face="arial">
							<span title="<%=SubstanceName%>">Record #<%=BaseRunningIndex%></span>
						</font>
					</div>
					<%ShowCFWChemResult dbkey, formgroup, "Structure", "Substance.Structure",BaseID, displayType, 185, 130 %>
					<div align="center">
						<font size="1" face="Arial">
							<strong>
								<br>ACX#: 
								<a target="_new" title="View this substance in the ChemACX System" href="<%=Application("SERVER_TYPE") & Application("ACXServerName")%>/chemacx/default.asp?formgroup=base_form_group&amp;dbname=chemacx&amp;dataaction=query_string&amp;field_type=TEXT&amp;full_field_name=Substance.ACX_ID&amp;field_value=<%=ACX_ID%>"><%=ACX_ID%></a>
								<br /><i><%=TruncateInSpan( SubstanceName, 30, SubstanceNameID )%></i>
							</strong>
						</font>
					</div>
					<div align="center">
						<font size="1" face="Arial">
							<strong>
							<%
								if BaseRS("CAS") <> "" then
 									response.write "<br>CAS#: " & CAS
								else
									response.write "<br>CAS#: n/a"
								end if
							%>
							</strong>
						</font>
					</div>
			<% listItemNumber = listItemNumber +1%>
				</td>
			<%if (listItemNumber /3 - int(listItemNumber /3)) = 0 then%>
			</tr>
			<tr>
			<%end if %>
			<%
			CloseRS(BaseRS)
			CloseConn(DataConn)
			%>
			<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
			<%if (listItemNumber /3 - int(listItemNumber /3))<> 0 then%>
			</tr>
			<%end if %>
		</table>
<% 
'if plugin_value then
	'WriteAppletCode()
'end if
%>
	</body>
</html>

