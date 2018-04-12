<%@ LANGUAGE="VBScript" %>
<%'SYAN modified on 3/31/2004 to support parameterized SQL%>

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<title>ChemACX Results-List View</title>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<!--#INCLUDE VIRTUAL = "/chemacx/chemacx/msdx_utils.asp"-->
		<!--#INCLUDE VIRTUAL = "/chemacx/chemacx/reg_utils.asp"-->
		<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
		<script language="JavaScript">
			// Open the synonym window
			//SYAN added dbkey parameter to the function to support global search 4/6/2004
			function openSynWindow(leftPos,topPos,csNum,recordNum,dbkey){
				var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable,width=300,height=200,screenX=" + leftPos + ",screenY=" + topPos + ",left=" + leftPos + ",top=" + topPos;
				SynWindow = window.open("Synlookup.asp?CsNum=" + csNum + "&recordNum=" + recordNum + "&dbkey=" + dbkey,"Synonyms_Window",attribs);
			}
			
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
		</script>
		<script LANGUAGE="javascript" src="/chemacx/Choosecss.js"></script>
	</head>
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<body bgcolor="#FFFFFF">
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<%
if Not Session("fEmptyRecordset" & dbkey) = True  then
  listItemNumber = 0 
end if

plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)

%>
		<table border="1" bgcolor="#FFFFFF" align="left" width="200">
			<tr>
			<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%
' If there is only one record to display then Go to form view
if end_index = 1 then Response.Redirect "/chemacx/chemacx/chemacx_form_frset.asp?formgroup=" & formgroup & "&dbname=chemacx&formmode=edit" & "&unique_id=" & BaseRunningIndex

Set DataConn=GetConnection(dbkey, formgroup, "Substance")
'sql = "SELECT CAS, HasMSDS FROM substance WHERE csNum=" & BaseID
sql = "SELECT substance.ACX_ID as ACX_ID, substance.CAS as CAS, substance.HasMSDS as HasMSDS, product.prodname as ProdName, product.proddescrip as ProdDesc FROM substance, product WHERE product.csnum=substance.csnum and substance.csNum=? "  'pjd added fields for display
sql_parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID
'Set BaseRS = DataConn.Execute(sql)
Set BaseRS = GetRecordSet(sql, sql_parameters)

CAS = BaseRS("CAS").value
ACX_ID = BaseRS("ACX_ID").value
bHasMSDX = BaseRS("HasMSDS").value
ProdName = BaseRS("ProdName").value
ProdDesc = BaseRS("ProdDesc").value

if  plugin_value  then
	displayType = "cdx"
	zoomFunction = "ACX_getStrucZoomBtn('Substance.Structure'," & BaseID & ")"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('Substance.Structure'," & BaseID & ",300,300)"
end if
%>
				<td align="" valign="top" width="194" nowrap>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>					
							<td><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>,'<%=dbkey%>');return false"><img src="<%=Application("NavButtonGifPath")%>names.gif" alt border="0"><nobr></a></td>				
							<td><script language="JavaScript">getFormViewBtn("details.gif","chemacx_form_frset.asp","<%=BaseActualIndex%>")</script></td>
							<td><script language="JavaScript">getMarkBtn(<%=BaseID%>);</script></td>
						</tr>
					</table>	
					<div align="center">
						<font size="1" face="arial">Record #<%=BaseRunningIndex%></font>
					</div>
					<%'ShowCFWChemResult dbkey, formgroup, "Structure", "Substance.Structure",BaseID, displayType, 185, 130 %>
					<div align="left">
						<table>
						<font size="1" face="Arial">

							<%
								if CBool(Application("RegisterFromACX")) AND (Application("RegServer")<> "NULL") then
									ShowSendtoRegLink(BaseID)
									Response.Write " | "
								End if
								if len(Trim(CAS))> 0 AND NOT IsNull(CAS) then
									
 									response.write "<tr><td><strong>Catalog Number:</strong></td><td> " & ACX_ID & "</strong></td></tr>"
									response.write "<tr><td><strong>NDC#:</strong></td><td> " & CAS & "</td></tr>"
									response.write "<tr><td><strong>Name:</strong></td><td>" & ProdName & "</td></tr>"
									response.write "<tr><td><strong>Brand/ Description:</strong></td><td>" & ProdDesc & "</td></tr>"
 									if CBool(Application("SHOW_MSDX_LOOKUP_LINK")) then		
											'Response.Write " | "
											'ShowMSDXLink ACX_ID, CAS, "", "", bHasMSDX
									End if 
								else
									response.write "<br><strong>NDC#: n/a</strong>"
								end if
							%>
							<br>
						</font>
					</table>	
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

