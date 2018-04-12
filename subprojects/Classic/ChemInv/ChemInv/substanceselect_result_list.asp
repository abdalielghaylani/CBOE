<%@ LANGUAGE="VBScript" %>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<title><%=Application("appTitle")%> -- Results-List View</title>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
		<script language="JavaScript">
			// Open the synonym window
			function openSynWindow(leftPos,topPos,CompoundID,recordNum){
				var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable,width=300,height=200,screenX=" + leftPos + ",screenY=" + topPos + ",left=" + leftPos + ",top=" + topPos;
				SynWindow = window.open("Synlookup.asp?CompoundID=" + CompoundID + "&recordNum=" + recordNum,"Synonyms_Window",attribs);
			}
			
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
		</script>
	</head>
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<body bgcolor="#FFFFFF">
	<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
	<!--#INCLUDE VIRTUAL = "/cheminv/GUI/guiUtils.asp"-->
<%
Dim Conn
Dim RS

%>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<%
if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
  listItemNumber = 0
  Response.Write "<span class=""GUIFeedback"">Select a " & Application("appTitle") & " Substance<BR><BR></span>"
  Response.Write "<table border=""1"" bgcolor=""#FFFFFF"" align=""left"">"
  Response.Write "<tr>"
else
	Response.Write "<BR>"   
end if
%>
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%
Set DataConn=GetConnection(dbkey, formgroup, "inv_compounds")
sql = "SELECT inv_compounds.compound_id, inv_compounds.CAS,inv_compounds.Substance_Name,inv_compounds.BASE64_CDX FROM inv_compounds WHERE inv_compounds.Compound_ID =" & BaseID
Set BaseRS = DataConn.Execute(sql)
CAS = BaseRS("CAS")
SubstanceName = BaseRS("Substance_Name")
'Response.Write sql
plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
if  plugin_value  then
	displayType = "cdx"
	zoomFunction = "ACX_getStrucZoomBtn('inv_compounds.BASE64_CDX'," & BaseID & ")"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('inv_compounds.BASE64_CDX'," & BaseID & ",500,450)"
end if
%>
				<td align="center" valign="top" width="194" nowrap>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>					
							<td><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>);return false"><img src="<%=Application("NavButtonGifPath")%>names.gif" alt border="0"><nobr></a></td>				
							<!---<td><script language="JavaScript">getFormViewBtn("details.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit", "", "base_form_group&CompoundID=<%=BaseID%>&ClearNodes=1&sNode=1&Exp=Y#1")</script></td>-->
							<td><script language="JavaScript">getFormViewBtn("details.gif","substanceselect_form.asp","<%=BaseActualIndex%>")</script></td>
							<td><script language="JavaScript">getMarkBtn(<%=BaseID%>);</script></td>
						</tr>
					</table>	
					<div align="center">
						<font size="1" face="arial"><span title="<%=SubstanceName%>">Record #<%=BaseRunningIndex%></span></font>
					</div>
					<%ShowResult dbkey, formgroup, BaseRS,"inv_compounds.BASE64_CDX", "Base64CDX", 185, 130%>
					<div align="center">
						<font size="1" face="Arial">
							<strong>
								<br>Compound ID: 
								<a target="_new" title="Manage this <%=Application("appTitle")%> Substance" href="/cheminv/default.asp?formgroup=substances_form_group&amp;dbname=cheminv&amp;dataaction=query_string&amp;field_type=INTEGER&amp;full_field_name=inv_compounds.Compound_ID&amp;field_value=<%=BaseID%>"><%=BaseID%></a>
							</strong>
							<br /><i><%=TruncateInSpan( SubstanceName, 30, SubstanceNameID )%></i>
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
							<br>
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

