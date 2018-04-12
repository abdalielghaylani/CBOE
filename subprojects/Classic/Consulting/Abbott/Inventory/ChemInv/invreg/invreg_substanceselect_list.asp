<%@ LANGUAGE="VBScript" %>

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<title><%=Application("appTitle")%> -- Results-List View</title>
		<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
		<script language="JavaScript">
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
	<body bgcolor="#FFFFFF">
	<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<%
if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
  listItemNumber = 0
  Response.Write "<span class=""GUIFeedback"">Select a Registry Substance<BR><BR></span>"
  Response.Write "<table border=""1"" cellpadding=""5"" bgcolor=""#FFFFFF"" align=""left"">"
  Response.Write "<tr>"
else
  Response.Write "<br>"   
end if

BaseIDList = ""
%>
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%

'-- Do not display any duplicates
if instr(BaseIDList,BaseID & ", ") = 0 then
BaseIDList = BaseIDList & BaseID & ", "

%>
<!--#INCLUDE VIRTUAL ="/cheminv/invreg/GetRegSubstanceAttributes.asp"-->
<%
plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
if  plugin_value  then
	displayType = "cdx"
	zoomFunction = "ACX_getStrucZoomBtn('Structures.BASE64_CDX'," & cpdDBCounter & ")"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('Structures.BASE64_CDX'," & cpdDBCounter & ",500,450)"
end if

if lCase(Application("DISPLAY_SUBSTANCE_RESULT_AS_LIST")) = "true" then
divNumber = 1
%>
				<td valign="top" nowrap>
					<table border="0" cellpadding="0" cellspacing="0" width="80">
						<tr>
							<td width="24"><script language="JavaScript"><%=zoomFunction%></script></td>
							<td width="54"><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>);return false"><img src="<%=Application("NavButtonGifPath")%>names.gif" alt border="0"><nobr></a></td>
							<td width="54">
								<script language="JavaScript">getFormViewBtn("details.gif","invreg_substanceselect_form.asp","<%=BaseActualIndex%>", "edit")</script></td>
							</td>
							<td width="54"><script language="JavaScript">getMarkBtn(<%=BaseID%>);</script></td>
							<td width="10">&nbsp;&nbsp;</td>
							<td nowrap width="65"><font size="1" face="arial"><span title="<%=SubstanceName%>">Record #<%=BaseRunningIndex%></span></font></td>
							<td width="10">&nbsp;&nbsp;</td>
							<td nowrap <% if len(SubstanceName) >= 25 then Response.Write("width=""200""") end if %>><%=Left(SubstanceName,25)%><% if len(SubstanceName) >= 25 then Response.Write("...") end if %></td>
							<td width="10">&nbsp;&nbsp;</td>
							
							<%
							'-- If registration system if CS Registration System, show link to app
							if Application("INVREG_USERNAME") = "REGDB" then
							%>
								<td nowrap>Reg#: <a target="_new" title="View this subtance in the Chemical Registration System" href="http://<%=Application("RegServerName")%>/chem_reg/default.asp?formgroup=base_form_group&amp;dbname=reg&amp;dataaction=query_string&amp;full_field_name=Reg_Numbers.Reg_ID&amp;field_value=<%=Reg_ID%>"><%=baseRegNumber%></a></td>
							<% 
							else
							%>
								<td nowrap width="120">Reg#: <%=baseRegNumber%></td><td width="10">&nbsp;&nbsp;</td>
								<!--<td nowrap>Lot #: <%=batchNumber%></td>-->
							<%
							end if
							%>
						</tr>
					</table>	
			<% listItemNumber = listItemNumber +1%>
				</td>
<% else 
divNumber = 3
%>

				<td align="center" valign="top" width="194" nowrap>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>					
							<td><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>);return false"><img src="<%=Application("NavButtonGifPath")%>names.gif" alt border="0"><nobr></a></td>				
							<!---<td><script language="JavaScript">getFormViewBtn("details.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit", "", "base_form_group&CompoundID=<%=BaseID%>&ClearNodes=1&sNode=1&Exp=Y#1")</script></td>-->
							<td>
								<script language="JavaScript">getFormViewBtn("details.gif","invreg_substanceselect_form.asp","<%=BaseActualIndex%>", "edit")</script></td>
							</td>
							<td><script language="JavaScript">getMarkBtn(<%=BaseID%>);</script></td>
						</tr>
					</table>	
					<div align="center">
						<font size="1" face="arial"><span title="<%=SubstanceName%>">Record #<%=BaseRunningIndex%></span></font>
					</div>
					<%
					if Session("isCDP") = "TRUE" then
						specifier = 185
					else
						specifier = "185:gif"
					end if
					Base64DecodeDirect "invreg", "base_form_group", StructuresRS("BASE64_CDX"), "Structures.BASE64_CDX", cpdDBCounter, cpdDBCounter, specifier, 130%>
					<div align="center">
						<font size="1" face="Arial">
							<strong>
								Reg#: 
								<%
								'-- If registration system if CS Registration System, show link to app
								if Application("INVREG_USERNAME") = "REGDB" then
								%>
									<a target="_new" title="View this subtance in the Chemical Registration System" href="http://<%=Application("RegServerName")%>/chem_reg/default.asp?formgroup=base_form_group&amp;dbname=reg&amp;dataaction=query_string&amp;full_field_name=Reg_Numbers.Reg_ID&amp;field_value=<%=Reg_ID%>"><%=baseRegNumber%></a>
								<% 
								else
								%>
									<%=baseRegNumber%><br>
									Lot #: <%=batchNumber%>
								<%
								end if
								%>
								
							</strong>
						</font>
					</div>
					<div align="center">
						<font size="1" face="Arial">
							<strong>
							<%
								if CAS <> "" then
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
<% end if %>

<% end if %>			
			<%if (listItemNumber /divNumber - int(listItemNumber /divNumber)) = 0 then%>
			</tr>
			<tr>
			<%end if %>
			<%
			CloseRS(BaseRS)
			CloseConn(DataConn)
			%>
			<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
			<%
			if listItemNumber <> "" and divNumber <> "" then
				if (listItemNumber /divNumber - int(listItemNumber /divNumber))<> 0 then
			%>
			</tr>
			<%
				end if 
			end if
			%>
		</table>
	</body>
</html>

