<%@ LANGUAGE="VBScript" %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<script language="JavaScript">
			var markbtnNamelist = "mark_record_red,unmark_record_red";
			var MarkedHitsbuttonname = "show_marked_red";
			var ShowLastbuttonname = "show_last_list_red";
			var ClearHitsbuttonname = "clear_marked_red";
		</script>
		<title><%=Application("appTitle")%> -- Results-List View</title>
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
<%
Response.Write "<span class=""GUIFeedback"">The following ChemACX substances matched your query:<BR><BR></span>"
if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
  listItemNumber = 0
  
  Response.Write "<table border=""1"" bgcolor=""#FFFFFF"" align=""left"">"
  Response.Write "<tr>"
else
	Response.Write "<BR>"   
end if
%>
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cheminv/invacx/GetACXSubstanceAttributes.asp"-->
<%
Set InvConn=GetConnection("cheminv", "base_form_group", "inv_containers")
sql = "SELECT Count(Container_ID) AS ContainerCount FROM inv_containers, inv_compounds WHERE inv_containers.compound_id_fk = inv_compounds.compound_id AND inv_compounds.ACX_ID = '" & ACX_ID & "'"
'Response.Write sql
Set Count_RS = InvConn.execute(sql)
ContainerCount = Clng(Count_RS("ContainerCount"))
plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
if  plugin_value  then
	displayType = "cdx"
	zoomFunction = "ACX_getStrucZoomBtn('Substance.Structure'," & BaseID & ", 0, 0,'zoom_red_btn.gif')"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('Substance.Structure'," & BaseID & ",600,450,'zoom_red_btn.gif')"
end if
%>
				<td align="center" valign="top" width="194" nowrap>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>					
							<td><a target="_new" title="View this substance in the ChemACX System" href="http://<%=Application("ACXServerName")%>/chemacx/default.asp?formgroup=base_form_group&amp;dbname=chemacx&amp;dataaction=query_string&amp;field_type=TEXT&amp;full_field_name=Substance.ACX_ID&amp;field_value=<%=ACX_ID%>"><img src="<%=Application("NavButtonGifPath")%>acx_details.gif" alt border="0"><nobr></a></td>				
							<!---<td><script language="JavaScript">getFormViewBtn("details.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit", "", "base_form_group&CompoundID=<%=BaseID%>&ClearNodes=1&sNode=1&Exp=Y#1")</script></td>-->
							<td>
							<% if ContainerCount > 0 then%>
								<script language="JavaScript">getFormViewBtn("inv_details.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit")</script></td>
							<%else%>
								<img src="<%=Application("NavButtonGifPath")%>inv_details_dim.gif"></td>
							<%end if%>
							<td><script language="JavaScript">getMarkBtn(<%=BaseID%>);</script></td>
						</tr>
					</table>	
					<div align="center">
						<font size="1" face="arial">
							<span title="<%=SubstanceName%>">Record #<%=BaseRunningIndex%></span>
							<br>
							<%=ContainerCount%> containers
						</font>
					</div>
					<%ShowCFWChemResult dbkey, formgroup, "Structure", "Substance.Structure",BaseID, displayType, 185, 130 %>
					
					<div align="center">
						<font size="1" face="Arial">
							<strong>
								<br>ACX#:
								<a target="_new" title="View this substance in the ChemACX System" href="http://<%=Application("ACXServerName")%>/chemacx/default.asp?formgroup=base_form_group&amp;dbname=chemacx&amp;dataaction=query_string&amp;field_type=TEXT&amp;full_field_name=Substance.ACX_ID&amp;field_value=<%=ACX_ID%>"><%=ACX_ID%></a>
								<br /><i><%=TruncateInSpan( SubstanceName, 30, SubstanceNameID )%></i>
							</strong>
						</font>
					</div>
					<div align="center">
						<font size="1" face="Arial">
							<strong>
							<%
								if len(Trim(CAS))> 0 AND NOT IsNull(CAS) then
 									response.write "<br>CAS#: " & CAS & "</strong></font>"
 									if CBool(Application("SHOW_MSDX_LOOKUP_LINK")) AND (Application("ACXServerName") <> "NULL") then		
										bHasMSDX = -1
										if Application("MSDX_LOOK_AHEAD") then bHasMSDX = HasMSDX(CAS, "", "")
											Response.Write " | "
											ShowMSDXLink CAS, "", "", bHasMSDX
									End if 
									if CBool(Application("SHOW_MSDS_LOOKUP_LINK")) AND (Application("ACXServerName") <> "NULL") then		
										bHasMSDS = -1
										if Application("MSDS_LOOK_AHEAD") then bHasMSDS = HasMSDS(CAS, "", "")
											Response.Write " | "
											ShowMSDSLink CAS, "", "", bHasMSDS
									End if 
								else
									response.write "<br>CAS#: n/a</strong></font>"
								end if
							%>
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

