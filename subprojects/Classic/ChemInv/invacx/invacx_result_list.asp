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
if detectModernBrowser = true then
%>
        <div style="display: none">
            <script language="JavaScript">cd_insertObject("chemical/x-cdx", "100", "100", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", "", "true", <%=ISISDraw%>)</script>
        </div>
<%
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
	zoomFunction = "ACX_getStrucZoomBtn('Substance.Structure'," & BaseID & ", 'SubstanceStructure_" & BaseID & "_orig', 0, 0,'zoom_red_btn.gif')"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('Substance.Structure'," & BaseID & ", 'SubstanceStructure_" & BaseID & "_orig',600,450,'zoom_red_btn.gif')"
end if
%>
				<td align="center" valign="top" width="194" nowrap>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>					
							<td><a target="_new" title="View this substance in the ChemACX System" href="<%=Application("SERVER_TYPE") & Application("ACXServerName")%>/chemacx/default.asp?formgroup=base_form_group&amp;dbname=chemacx&amp;dataaction=query_string&amp;field_type=TEXT&amp;full_field_name=Substance.ACX_ID&amp;field_value=<%=ACX_ID%>"><img src="<%=Application("NavButtonGifPath")%>acx_details.gif" alt border="0"><nobr></a></td>				
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
						<%
                        if UCase(Application("DISPLAY_RECORD_NUM")) = "TRUE" then
				        %>
							<span title="<%=SubstanceName%>">Record #<%=BaseRunningIndex%></span>
							<br>
						<%
						end if
						if UCase(Application("DISPLAY_NUM_CONTAINERS")) = "TRUE" then
						    Response.Write( ContainerCount & " containers" )
						end if
						%>
						</font>
					</div>
					<%
					    if( UCase(Application("DISPLAY_STRUCTURE")) = "TRUE" ) then
					        ShowCFWChemResult dbkey, formgroup, "Structure", "Substance.Structure",BaseID, displayType, 185, 130
                        end if
                    %>					
					<div align="center">
						<font size="1" face="Arial">
							<strong>
                    <%
    			        if UCase(Application("DISPLAY_COMPOUND_ID")) = "TRUE" then
    			    %>
								<br>ACX#:
								<a target="_new" title="View this substance in the ChemACX System" href="<%=Application("SERVER_TYPE") & Application("ACXServerName")%>/chemacx/default.asp?formgroup=base_form_group&amp;dbname=chemacx&amp;dataaction=query_string&amp;field_type=TEXT&amp;full_field_name=Substance.ACX_ID&amp;field_value=<%=ACX_ID%>"><%=ACX_ID%></a>
                    <%
                        end if
                        if UCase(Application("DISPLAY_SUBSTANCE_NAME")) = "TRUE" then
                    %>
								<br /><i><%=TruncateInSpan( SubstanceName, 30, SubstanceNameID )%></i>
                    <% 
					    end if
                    %>
							</strong>
						</font>
					</div>
					<%
                        if UCase(Application("DISPLAY_CAS_NUMBER")) = "TRUE" then
                    %>
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
					<%
					    end if
					%>
				</td>
			<%
			    listItemNumber = listItemNumber +1
			    if (listItemNumber /3 - int(listItemNumber /3)) = 0 then
            %>
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

