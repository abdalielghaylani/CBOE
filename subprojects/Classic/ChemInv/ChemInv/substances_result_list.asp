<%@ LANGUAGE="VBScript" %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<title><%=Application("appTitle")%> -- Results-List View</title>
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
	<body>
<%
Dim Conn
Dim RS
%>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<%
if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
  listItemNumber = 0 
end if

plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
recordsFound = false
%>
<%
if detectModernBrowser = true then
%>
        <div style="display: none">
            <script language="JavaScript">cd_insertObject("chemical/x-cdx", "100", "100", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", "", "true", <%=ISISDraw%>)</script>
        </div>
<%
end if
%>
		<table id="resultsTable" border="1" bgcolor="#FFFFFF" align="left" width="600">
			<tr>
			<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%
recordsFound = true
' If there is only one record to display then Go to form view
if end_index = 1 then
	resultsURL = "/cheminv/cheminv/substances_form.asp?formgroup=" & formgroup & "&dbname=" & dbkey & "&formmode=edit" & "&unique_id=" & BaseRunningIndex
	Response.Redirect resultsURL
End if

Set DataConn=GetConnection(dbkey, formgroup, "inv_compounds")
sql = "SELECT inv_compounds.compound_id, inv_compounds.CAS,inv_compounds.Substance_Name,inv_compounds.BASE64_CDX FROM inv_compounds WHERE inv_compounds.Compound_ID =" & BaseID
Set BaseRS = DataConn.Execute(sql)
CAS = BaseRS("CAS")
'Response.Write sql
if  plugin_value  then
	displayType = "cdx"
	zoomFunction = "ACX_getStrucZoomBtn('inv_compounds.BASE64_CDX'," & BaseID & ", 'inv_compoundsBASE64_CDX_" & BaseID & "_orig')"
else
	displayType = "SizedGif"
	zoomFunction = "ACX_getStrucZoomBtn('inv_compounds.BASE64_CDX'," & BaseID & ", 'inv_compoundsBASE64_CDX_" & BaseID & "_orig',500,450)"
end if
'copyFunction = "getStrucCopyBtn('inv_compounds', 'BASE64_CDX', '" & BaseID & "')"
%>
				 <td align="center" valign="top" width="194" nowrap>
                     <table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><script language="JavaScript"><%=zoomFunction%></script></td>		
<!--                            <%if detectModernBrowser = true then %><td><script language="JavaScript"><%=copyFunction%></script></td><%end if %>	-->		
							<td><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>);return false"><img src="<%=Application("NavButtonGifPath")%>names.gif" alt border="0"><nobr></a></td>				
							<!---<td><script language="JavaScript">getFormViewBtn("details.gif","cheminv_form_frset.asp","<%=BaseActualIndex%>", "edit", "", "base_form_group&CompoundID=<%=BaseID%>&ClearNodes=1&sNode=1&Exp=Y#1")</script></td>-->
							<td><script language="JavaScript">getFormViewBtn("details.gif","substances_form.asp","<%=BaseActualIndex%>")</script></td>
							<td><script language="JavaScript">getMarkBtn(<%=BaseID%>);</script></td>
						</tr>
					</table>	
					<div align="center">
						<font size="1" face="arial"><span>Record #<%=BaseRunningIndex%></span></font>
					</div>
					<%ShowResult dbkey, formgroup, BaseRS,"inv_compounds.BASE64_CDX", "Base64CDX", 185, 130					%>
					<div align="center">
						<font size="1" face="Arial">
							<strong>
							<%
							<!--CSBR# 125084-->
							<!--Purpose of Change: To display the Substance name in Manage substance search-->
							    if UCase(Application("DISPLAY_SUBSTANCE_NAME")) = "TRUE" then
							    	Response.Write("<br><i>" & TruncateInSpan(BaseRS("Substance_Name"), 30, SubstanceNameID) & "</i><br>")
							    End if
							<!--End of fix CSBR-125804-->
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
<%if not recordsFound then %>		
    <script type="text/javascript" language="javascript">
        document.getElementById("resultsTable").border = 0;
    </script>
<%end if %>
	</body>
</html>

