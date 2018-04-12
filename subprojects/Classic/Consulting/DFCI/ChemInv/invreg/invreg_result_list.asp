<%@ LANGUAGE="VBScript" %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<script language="JavaScript">
			var markbtnNamelist = "mark_record_blue,unmark_record_blue";
			var MarkedHitsbuttonname = "show_marked_blue";
			var ShowLastbuttonname = "show_last_list_blue";
			var ClearHitsbuttonname = "clear_marked_blue";
		</script>
		<title><%=Application("appTitle")%> -- Results-List View</title>
		<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
		<script language="JavaScript">
			
			// unhide the search link
			if (!top.bannerFrame.document.all.searchLink){
			}
			else{
				top.bannerFrame.document.all.searchLink.style.visibility = "visible";
			}
			// Open the synonym window
			function openSynWindow(leftPos,topPos,RegID,recordNum){
				var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable,width=300,height=200,screenX=" + leftPos + ",screenY=" + topPos + ",left=" + leftPos + ",top=" + topPos;
				SynWindow = window.open("Synlookup.asp?RegID=" + RegID + "&recordNum=" + recordNum,"Synonyms_Window",attribs);
			}
			
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
		</script>
	</head>
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<body bgcolor="#ffffff">
	<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<%

if end_index = 1 then Response.Redirect "/cheminv/invreg/cheminv_form_frset.asp?formgroup=gs_form_group&dbname=invreg&formmode=edit" & "&unique_id=" & BaseRunningIndex
Response.Write "<span class=""GUIFeedback"">The following Registry substances matched your query:<BR><BR></span>"
if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
  listItemNumber = 0
  Response.Write "<table border=""1"" bgcolor=""#FFFFFF"" align=""left"">"
  Response.Write "<tr>"
else
	Response.Write "<BR>"   
end if
%>
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cheminv/invreg/GetRegSubstanceAttributes.asp"-->
<%
plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
if  plugin_value  then
	displayType = "cdx"
	zoomFunction = "ACX_getStrucZoomBtn('INV_VW_REG_STRUCTURES.BASE64_CDX'," & cpdDBCounter & ", 0, 0, 'zoom_blue_btn.gif')"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('INV_VW_REG_STRUCTURES.BASE64_CDX'," & cpdDBCounter & ",500,450,'zoom_blue_btn.gif')"
end if
Set InvConn=GetConnection("cheminv", "base_form_group", "inv_containers")
sql = "SELECT Count(Container_ID) AS ContainerCount FROM inv_containers WHERE Reg_ID_FK = " & Reg_ID
'Response.Write sql
Set Count_RS = InvConn.execute(sql)
ContainerCount = Clng(Count_RS("ContainerCount"))
%>
				<td align="center" valign="top" width="194" nowrap>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>					
							<td><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>);return false"><img src="<%=Application("NavButtonGifPath")%>names_blue.gif" alt border="0"><nobr></a></td>				
							<!---<td><script language="JavaScript">getFormViewBtn("details.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit", "", "base_form_group&CompoundID=<%=BaseID%>&ClearNodes=1&sNode=1&Exp=Y#1")</script></td>-->
							<td>
							<% if ContainerCount > 0 then%>
								<script language="JavaScript">getFormViewBtn("details_blue.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit")</script></td>
							<%else%>
								<img src="<%=Application("NavButtonGifPath")%>details_dim.gif"></td>
							<%end if%>
							</td>
							<td><script language="JavaScript">getMarkBtn(<%=BaseID%>);</script></td>
						</tr>
					</table>
					<div align="center">
						<font size="1" face="arial">
							<span title="<%=SubstanceName%>">Record #<%=BaseRunningIndex%></span>
							<br>
							<%=ContainerCount%> Containers	
						</font>
					</div>
						<%
						if Session("isCDP") = "TRUE" then
							specifier = 185
						else
							specifier = "185:gif"
						end if
						Base64DecodeDirect "invreg", "base_form_group", StructuresRS("BASE64_CDX"), "Structures.BASE64_CDX", cpdDBCounter, cpdDBCounter, specifier, 130
						%>
					<div align="center">
						<font size="1" face="Arial">
							<strong>
								<br>Reg#: 
								<% if Application("INVREG_USERNAME") = "REGDB" then %>
								<a target="_new" href="http://<%=Application("RegServerName")%>/chem_reg/default.asp?formgroup=base_form_group&amp;dbname=reg&amp;dataaction=query_string&amp;full_field_name=Reg_Numbers.Reg_ID&amp;field_value=<%=Reg_ID%>&amp;PostRelay=1&amp;CSUserName=invadmin&amp;CSUSerID=invadmin"><%=baseRegNumber%></a>
								<% else %>
								<%=baseRegNumber%>
								<% end if %>
							</strong>
							<br /><i><%=TruncateInSpan( SubstanceName, 30, SubstanceNameID )%></i>
							<br>
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
	</body>
</html>

