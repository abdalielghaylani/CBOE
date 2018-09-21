<%@ LANGUAGE="VBScript" %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<title><%=Application("appTitle")%> -- Results-List View</title>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<script language="JavaScript">
			// unhide the search link
			if (!top.bannerFrame.document.all.searchLink){
			}
			else{
				top.bannerFrame.document.all.searchLink.style.visibility = "visible";
			}
			// Open the synonym window
			function openSynWindow(leftPos,topPos,CompoundID,recordNum){ 
				var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable,width=300,height=200,screenX=" + leftPos + ",screenY=" + topPos + ",left=" + leftPos + ",top=" + topPos;
				SynWindow = window.open("Synlookup.asp?CompoundID=" + CompoundID + "&recordNum=" + recordNum,"Synonyms_Window",attribs);
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
if detectModernBrowser = true then
%>
        <div style="display: none">
            <script language="JavaScript">cd_insertObject("chemical/x-cdx", "100", "100", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", "", "true", <%=ISISDraw%>)</script>
        </div>
<%
end if
%>
<%
if end_index = 1 and lcase(formgroup)<> "gs_form_group" then Response.Redirect "/cheminv/cheminv/cheminv_form_frset.asp?formgroup=base_form_group&dbname=cheminv&formmode=edit" & "&unique_id=" & BaseRunningIndex

if( lcase(formgroup)<> "base_form_group" ) then
    Response.Write "<span class=""GUIFeedback"">The following  " & Application("appTitle") & " substances matched your query:<BR><BR></span>"
else
    Response.Write "<span class=""GUIFeedback"">The following substances matched your query:<BR><BR></span>"
end if
if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
  listItemNumber = 0
  Response.Write "<table border=""1"" bgcolor=""#FFFFFF"" align=""left"">"
  Response.Write "<tr>"
else
	Response.Write "<BR>"   
end if

if lcase(formgroup) = "gs_form_group" or lcase(formgroup) = "containers_np_form_group" then
	CompoundsTable = "inv_compounds"
else
	CompoundsTable = "inv_vw_compounds"
end if

Set DataConn=GetConnection(dbkey, formgroup, CompoundsTable)

sql = "select cpd.compound_id holder, c.compound_id, c.CAS, c.BASE64_CDX, c.REG_ID_FK, c.BATCH_NUMBER_FK, c.REG_NUMBER, c.REG_BATCH_ID, decode(c.reg_id_fk,null,c.Substance_Name,ivrb.regname) as Substance_Name " &_
"from inv_compounds cpd, inv_vw_compounds c, inv_vw_reg_batches ivrb " &_
"where cpd.compound_id = c.compound_id(+) AND c.REG_BATCH_ID = ivrb.REGBATCHID(+) AND  cpd.compound_id = ?"
Set Cmd = GetCommand(DataConn, sql, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("CompoundID", 5, 1, 4, 0)
Cmd.Prepared = True

if formgroup = "plate_compounds_form_group" then
	sql = "SELECT Count(distinct p.plate_id) AS ContainerCount, Count(w.well_id) AS WellCount FROM inv_plates p, inv_wells w WHERE p.plate_id = w.plate_id_fk AND w.compound_id_fk = ?"	
else
    sql = "SELECT Count(container_id) AS ContainerCount FROM inv_containers WHERE compound_id_fk = ?"	
end if
Set CountCmd = GetCommand(DataConn, sql, adCmdText)	
CountCmd.Parameters.Append Cmd.CreateParameter("CompoundID", 5, 1, 4, 0)
CountCmd.Prepared = True

%>
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%

Cmd.Parameters("CompoundID").Value = BaseID
Set BaseRS = Cmd.Execute
CAS = BaseRS("CAS")
SubstanceName = BaseRS("Substance_Name")
RegIDFK = BaseRS("reg_id_fk").Value
RegNumber = BaseRS("reg_number").Value
BatchNumberFK = BaseRS("batch_number_fk").Value
RegBatchID = BaseRS("reg_batch_id").Value

if not IsNull(RegIDFK) then
    bRegCompound = true
else
    bRegCompound = false
end if

CountCmd.Parameters("CompoundID").Value = BaseID
Set Count_RS = CountCmd.Execute
ContainerCount = CLng(Count_RS("ContainerCount"))
if ContainerCount <> 1 then ppl = "s"

if formgroup = "plate_compounds_form_group" then	
	WellCount = CLng(Count_RS("WellCount"))
	if Wellcount <> 1 then wpl = "s"	
	CountText = WellCount & " well" & wpl & " in " & ContainerCount & " plate" & ppl
Else	
    CountText = ContainerCount & " container" & ppl
End if

plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
if  plugin_value  then
	displayType = "cdx"
	zoomFunction = "ACX_getStrucZoomBtn('inv_vw_compounds.Structure'," & BaseID & ", 'inv_vw_compoundsStructure_" & BaseID & "_orig')"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('inv_vw_compounds.Structure'," & BaseID & ", 'inv_vw_compoundsStructure_" & BaseID & "_orig',600,450)"
end if

%>
				<td align="center" valign="top" width="194" nowrap>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>					
							<td><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>);return false"><img src="<%=Application("NavButtonGifPath")%>names.gif" alt border="0"><nobr></a></td>				
							<td>
							<% if ContainerCount > 0 then%>
								<script language="JavaScript">getFormViewBtn("details.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit")</script></td>
							<%else%>
								<img src="<%=Application("NavButtonGifPath")%>details_dim.gif"></td>
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
					        Response.Write( CountText )
					    end if
					%>
						</font>
					</div>
					<%
					    if( UCase(Application("DISPLAY_STRUCTURE")) = "TRUE" ) then
					        ShowResult dbkey, formgroup, BaseRS,"inv_vw_compounds.BASE64_CDX", "Base64CDX", 185, 130
					    end if					
                        
                    %>						
					<div align="center">
						<font size="1" face="Arial">
    						<strong>
    			    <%
    			        if UCase(Application("DISPLAY_COMPOUND_ID")) = "TRUE" then
    			            if not bRegCompound then
    			    %>    
								<br>Compound ID: 
								<%if Session("INV_MANAGE_SUBSTANCES" & "ChemInv") then%>
									<a target="_new" title="Manage this <%=Application("appTitle")%> Substance" href="/cheminv/default.asp?formgroup=substances_form_group&amp;dbname=cheminv&amp;dataaction=query_string&amp;field_type=INTEGER&amp;full_field_name=inv_compounds.Compound_ID&amp;field_value=<%=BaseID%>"><%=BaseID%></a>
								<%else%>
									<%=BaseID%>
								<% end if
							else    ' if IsNull(RegIDFK) then
                                if detectModernBrowser = true then
                                    regIdParameter = RegIDFK
                                else
                                    regIdParameter = RegNumber
                                end if
%>                                
                                <br>Reg Number: <a class="MenuLink" href="/cheminv/GUI/ViewRegDetails.asp?reg_number=<% =regIdParameter %>" target="_blank"><% = RegBatchID %></a>
<%
                            end if
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
							
							<br>
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
			<%  end if 
			
			    CloseRS(BaseRS)			    
			%>
			<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
			<%if (listItemNumber /3 - int(listItemNumber /3))<> 0 then%>
			</tr>
			<%
			end if 
			CloseConn(DataConn)
			%>
		</table>
	</body>
</html>

