<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim ShapeConn
Dim Cmd
Dim Conn
Dim RS
dbkey = "ChemInv"
ServerName = Request.ServerVariables("Server_Name")

BatchID = Request("BatchID")
ContainerID = Request("ContainerID")
ShowLinks = Request("ShowLinks")
if ShowLinks = "" or isEmpty(ShowLinks) then 
	ShowLinks = true
else
	ShowLinks = false
end if
%>
<html>
<head>

<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<style>
		A {Text-Decoration: none;}
		.TabView {color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: verdana}
</style>
<title><%=Application("appTitle")%> -- View Batch Information</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="CalculateFromPlugin.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
   var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
   var CD_AUTODOWNLOAD_PLUGIN = "<%=APPLICATION("CD_PLUGIN_DOWNLOAD_PATH")%>";
   var ContainerName = "<%=ContainerName%>"
   var LocationID = "<%=LocationID%>"
   // Posts the form when a tab is clicked
   /*function postDataFunction(sTab) {
	document.form1.action = "ViewContainer.asp?TB=" + sTab
	document.form1.submit()
	}*/
	//DJP: this is a copy of a function in navbar_js that is necessary for chemdraw_js to check for the plugin properly
	function getCookie(name){
		var cname = name  + "=";
		var dc= document.cookie;
		if(dc.length > 0){
			begin = dc.indexOf(cname);
				if(begin != -1){
					begin += cname.length;
					end = dc.indexOf(";",begin);
						if(end == -1) end = dc.length;
							 temp = unescape(dc.substring(begin,end));
							 theResult = temp

							  return theResult
				}
			}
		return null;
	}
	
	function showContainerDetails(ContainerBarcode){
		var detailsUrl = "";
		detailsUrl = "http://<%=ServerName%>/cheminv/default.asp?dbname=cheminv&dataaction=query_string&formgroup=containers_np_form_group&field_type=TEXT&full_field_name=inv_containers.Barcode&field_value=" + ContainerBarcode;
		window.open(detailsUrl);
		window.close();
	}
//-->
</script>
<script LANGUAGE="javascript" src="/cheminv/gui/refreshGUI.js"></script>
<% if Session("isCDP") = "TRUE" then %>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<% end if %>
</head>
<body>
<form name="form1" action="echo.asp" xaction="NewLocation_action.asp" method="POST">
<input type="hidden" name="ContainerID" value="<%=ContainerID%>">

<div align="center">

<table border="0">
	<tr>
		<td>
<%
	'-- Get list of locations user have privs to see
	SQL_userLocations = "Select l.Location_ID from inv_containers c,inv_locations l where c.location_id_fk = l.location_id and c.batch_id_fk=" & BatchID
	userLocations = GetListFromSQLRS(SQL_userLocations)
	
	'-- Show all containers in batch regardless of RLS privs
	schemaName = Application("ORASCHEMANAME")
	call GetChemInvShapeConnection()
	if Application("DEFAULT_SAMPLE_REQUEST_CONC") <> "" then
		tmpConc = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
		strConcAbbrv = tmpConc(1)
	else
		strConcAbbrv = "UNIT_ABREVIATION"	
	end if
	'SQL = "SELECT " & schemaName & ".GUIUTILS.GETBATCHAMOUNTSTRING(Container_ID) AS Batch_Amount_String, "
	SQL = "SELECT (Select Sum(Qty_Available*concentration) From inv_containers where batch_id_fk=" & BatchID & ") || ' ' || '" & strConcAbbrv & "' AS Batch_Amount_String, " & vblf & _
			" (Select Sum(Qty_Available*concentration) From inv_containers where batch_id_fk=" & BatchID & ") AS Batch_Amount_Value, " & vblf & _
			" CONTAINER_ID," & vblf & _
			" BATCH_ID_FK," & vblf & _
			" rb.RegNumber as RegNumber," & vblf & _
			" rb.BatchNumber as BatchNumber," & vblf & _
			" rb.RegName as SubstanceName," & vblf & _
			" INV_CONTAINER_BATCHES.MINIMUM_STOCK_THRESHOLD," & vblf & _
			" INV_CONTAINER_BATCHES.COMMENTS," & vblf & _
			" INV_CONTAINER_BATCHES.FIELD_1," & vblf & _
			" INV_CONTAINER_BATCHES.FIELD_2," & vblf & _
			" INV_CONTAINER_BATCHES.FIELD_3," & vblf & _
			" INV_CONTAINER_BATCHES.FIELD_4," & vblf & _
			" INV_CONTAINER_BATCHES.FIELD_5," & vblf & _
			" INV_CONTAINER_BATCHES.DATE_1," & vblf & _
			" INV_CONTAINER_BATCHES.DATE_2," & vblf & _
			" MINIMUM_STOCK_THRESHOLD," & vblf & _
			" BATCH_FIELD_1," & vblf & _
			" BATCH_FIELD_2," & vblf & _
			" BATCH_FIELD_3," & vblf & _
			" CONTAINER_NAME," & vblf & _
		    " TO_CHAR(TRUNC(inv_Containers.DATE_Expires),'" & Application("DATE_FORMAT_STRING") & "') AS DATE_Expires,  " & vblf & _
			" QTY_MINSTOCK," & vblf & _
			" DECODE(inv_Containers.Qty_MinStock, NULL, ' ', inv_Containers.Qty_MinStock||' '|| Unit_Abreviation) AS Qty_MinStock_String, " & vblf & _
			" UNIT_ABREVIATION," & vblf & _
			" CONTAINER_STATUS_NAME AS STATUS," & vblf
			if Application("RegServerName") <> "NULL" then
				SQL = SQL & "DECODE(inv_containers.Reg_ID_FK, NULL, NULL, (Select RegBatchID from inv_vw_reg_batches Where inv_vw_reg_batches.regid = inv_containers.reg_id_fk AND inv_vw_reg_batches.batchnumber = inv_containers.Batch_Number_FK)) AS Reg_Batch_ID, "
				SQL = SQL & "DECODE(inv_containers.Reg_ID_FK, NULL, inv_Compounds.BASE64_CDX, (Select BASE64_CDX From cheminvdb2.inv_vw_reg_structures Where inv_vw_reg_structures.regid=inv_containers.reg_id_fk)) AS Base64_CDX, "
			else
				SQL = SQL & "inv_Compounds.BASE64_CDX AS BASE64_CDX, " & vblf
			End if
	SQL = SQL & " COMPOUND_ID" & vblf & _
			" FROM inv_containers, inv_container_status, inv_compounds, inv_units, inv_container_batches, inv_vw_reg_batches rb " & vblf & _
			" WHERE inv_containers.container_status_id_fk = container_status_id" & vblf & _
			    " AND inv_containers.UNIT_OF_MEAS_ID_FK = inv_units.UNIT_ID" & vblf & _
			    " AND inv_containers.batch_id_fk = inv_container_batches.batch_id" & vblf & _
				" AND compound_id_fk = compound_id(+)" & vblf & _
				" AND inv_containers.container_status_id_fk not in (6,7)" & vblf & _
				" AND rb.regid = inv_containers.reg_id_fk" & vblf & _
				" AND rb.batchnumber = inv_containers.batch_number_fk" & vblf &_
				" AND batch_id_fk = " & BatchID

	'Response.Write SQL
	'Response.End
	Shapestr = "SHAPE {" & SQL & "}" & _
               " APPEND ({" & _
               " SELECT container_id, batch_id_fk, parent_container_id_fk, " & vblf & _
               " inv_Containers.Qty_Available,uom.Unit_Abreviation UOMString,inv_Containers.Concentration,uoc.Unit_Abreviation UOCString," & _
			   " " & schemaName & ".GUIUTILS.GETLOCATIONPATH(l.location_id) AS Location_Path, " & vblf & _
			   " " & schemaName & ".GUIUTILS.GETRACKLOCATIONPATH(l.location_id) AS Rack_Path, " & vblf & _
			   " lp.collapse_child_nodes as IsRack, barcode, l.location_name, l.location_id, " & vblf & _
			   " case when (select count(( select e.location_id_fk from inv_grid_element e, inv_grid_storage s, inv_locations p where e.grid_storage_id_fk = s.grid_storage_id and p.location_id = s.location_id_fk and p.collapse_child_nodes = 1 and e.location_id_fk=l.location_id)) from dual) = 0 then l.location_id else        (select p.location_id from inv_grid_element e, inv_grid_storage s, inv_locations p where e.grid_storage_id_fk = s.grid_storage_id and p.location_id = s.location_id_fk and p.collapse_child_nodes = 1 and e.location_id_fk=l.location_id) end as parent_location_id " & vblf & _
			   " FROM inv_containers, inv_locations l, inv_units uom, inv_units uoc, inv_vw_grid_location gl, inv_locations lp " & vblf & _
			   " WHERE location_id_fk = l.location_id AND l.location_id not in (3) AND unit_of_meas_id_fk = uom.unit_id AND unit_of_conc_id_fk = uoc.unit_id AND container_status_id_fk not in (6,7) And l.location_id= gl.location_id(+) and gl.parent_id = lp.location_id(+) AND batch_id_fk = " & BatchID & "}" & _
               " RELATE batch_id_fk TO batch_id_fk) as rsBatches"
	'Response.Write ShapeStr
	'Response.End
	Set ShapeCmd = Server.CreateObject("ADODB.Command")
	ShapeCmd.ActiveConnection = ShapeConn
	ShapeCmd.commandtext = Shapestr
	Set RS = ShapeCmd.execute


	'Response.End
	If not (RS.BOF or RS.EOF) then
		BatchAmountString = RS("Batch_Amount_String")
		BatchAmountValue = RS("Batch_Amount_Value")
		'ConcentrationString = RS("Concentration_String")
		MinimumThreshold = RS("MINIMUM_STOCK_THRESHOLD")
		if MinimumThreshold <> "" then
			MinimumThresholdDisplay = RS("MINIMUM_STOCK_THRESHOLD") & "&nbsp;" & strConcAbbrv
		else
			MinimumThresholdDisplay = ""
		end if
		ContainerName = RS("Container_Name")
		SubstanceName = RS("SubstanceName")
		RegNumber = RS("RegNumber")
		BatchNumber = RS("BatchNumber")
		DateExpires = RS("Date_Expires")
		QtyMinStock = RS("Qty_MinStock")
		QtyUnits = RS("UNIT_ABREVIATION")
		QtyMinStockString = RS("Qty_MinStock_String")
		Status = RS("Status")
		Base64CDX = RS("Base64_CDX")
		BatchField1 = RS("batch_field_1")
		BatchField2 = RS("batch_field_2")
		BatchField3 = RS("batch_field_3")
		Comments = RS("Comments")
		Field_1 = RS("Field_1")
		Field_2 = RS("Field_2")
		Field_3 = RS("Field_3")
		Field_4 = RS("Field_4")
		Field_5 = RS("Field_5")
		Date_1 = RS("Date_1")
		Date_2 = RS("Date_2")
		Set rsBatches = RS("rsBatches").value
%>
	<center>
	<span class="GuiFeedback">Manage Batch Details</span><br /><br />
	<div style="background-color:#e1e1e1;padding:3px;">
	<% if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then %>
	<a class="MenuLink" style="cursor:hand; " href onclick="OpenDialog('/cheminv/gui/ManageBatch.asp?MinThreshold=<%=MinimumThreshold%>&amp;UOMAbbrv=<%=strConcAbbrv%>&amp;BatchID=<%=BatchID%>&amp;action=update&amp;BatchAmountValue=<%=BatchAmountValue%>', 'UpdateThreshold', 2); return false;" title="Edit Minimum Stock Threshold">Edit Batch</a>&nbsp;|
	<% end if %>
	<% if Session("INV_MANAGE_LINKS" & dbkey) then %>
	<a class="MenuLink" style="cursor:hand; " href onclick="OpenDialog('/cheminv/gui/manageLinks.asp?FK_value=<%=BatchID%>&amp;FK_name=BATCH_ID&amp;Table_Name=INV_CONTAINER_BATCHES&amp;LinkType=Batch', 'Links_Window', 2); return false;" title="Manage links associated to this Batch">Edit Batch Links</a>
	<% end if %>
	</div><br />
	
	<table border="0" cellspacing="0" cellpadding="0">
		<tr>
		<td valign="top">
			<table border="0" cellpadding="5" cellspacing="2">
				<tr>
					<%=ShowField("BatchID:", "BatchID", 15, "BatchID")%>
				</tr>
				<tr>
					<%=ShowField("Amount on Hand:", "BatchAmountString", 15, "BatchAmountString")%>
					<%=ShowField("Minimum Threshold:", "MinimumThresholdDisplay", 15, "MinimumThresholdDisplay")%>
				</tr>
				<tr>
					<%=ShowField("A-Code:", "RegNumber", 15, "BatchField1")%>
					<%=ShowField("Lot Number:", "BatchNumber", 15, "BatchField2")%>
				</tr>
				<tr><td valign="top" align="right">
					Comments:
				</td><td colspan="2">
					<textarea name="OrigComments" rows="6" cols="75" style="background-color:#e1e1e1;" readonly><%=Comments%></textarea>
				</td><td valign="top"><br />
					<div class="tasktitle">Batch Links</div>
					<%
					GetURLs BatchID, "inv_container_batches", "batch_id", "", "", ""
					%>
				</td></tr>
				<%
				For each Key in custom_batch_property_fields_dict
					execute("FieldValue = RS(""" & Key & """)")
					Response.Write("<tr><td align=""right"">" & custom_batch_property_fields_dict.item(key) & ":</td><td colspan=""3"" style=""background-color:#e1e1e1;padding:3px;"">" & FieldValue & "</td></tr>")
				Next	
				%>
				<!--<tr><td></td><td colspan="3" align="right"><a HREF="#" onclick="if (opener.location) {opener.location.reload();} window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a></td></tr>-->
				<tr>
					<td align="right" valign="top" nowrap>Batch Details:</td>
					<td colspan="3" align="right">
						
						<div style="height:200px; overflow: auto;">	
						<table border="0" cellpadding="4" cellspacing="3">
						<tr>
							<th bgcolor="#d3d3d3">Barcode</th>
							<% if Application("DEFAULT_SAMPLE_REQUEST_CONC") <> "" then %>
							<th bgcolor="#d3d3d3">Qty</th>
							<% end if %>
							<th bgcolor="#d3d3d3">Amount</th>
							<th bgcolor="#d3d3d3">Conc</th>
							<th bgcolor="#d3d3d3">Location</th>
							<th bgcolor="#d3d3d3">Substance</th>
						</tr>
							<%
							while not (rsBatches.BOF or rsBatches.EOF)
								LocationName = rsBatches("location_name")
								LocationPath = rsBatches("Location_Path")
								ContainerLocationID = rsBatches("Location_ID")
								RackPath = rsBatches("Rack_Path")
								if RackPath <> "" then 
									DisplayPath = RackPath & LocationName
								else 
									DisplayPath = LocationPath
								end if
								LocationID = rsBatches("parent_location_id")
								BatchContainerID = rsBatches("Container_ID")
								response.write("<tr>")
								if ShowLinks then 
									response.write("<td align=""right"">")
									if instr(userLocations,ContainerLocationID) > 0 then
										Response.Write("<a class=""MenuLink"" href=""#"" title=""" & rsBatches("barcode") & """ onclick=""SelectLocationNode(0," & LocationID & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & BatchContainerID & "); opener.focus(); window.close();"" onmouseover=""javascript:this.style.cursor='hand';"" onmouseout=""javascript:this.style.cursor='default';"">" & rsBatches("barcode") & "</a><b />")
									else
										Response.Write "<font style=""font-size: 11px;"">" & rsBatches("barcode") & "</font>"
									end if
									Response.Write("</td>")
								else
									response.write("<td align=""right"">")
									Response.Write("<a class=""menulink"" title=""View Container Details"" href=""#"" onclick=""showContainerDetails('" & rsBatches("barcode") & "')"">")
									Response.write(rsBatches("barcode"))
									Response.Write("</a>")
									Response.Write("</td>")
								end if
								
								if Application("DEFAULT_SAMPLE_REQUEST_CONC") <> "" then
									arrDeliveryUnit = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
									DeliveryUnitID = arrDeliveryUnit(0)
									DeliveryUnitName = arrDeliveryUnit(1)
									'TODO: Create generalized cacl Function(Amount to display, UOM, UOC)
									if not IsBlank(rsBatches("Qty_Available")) and not IsBlank(rsBatches("Concentration")) then
										if rsBatches("UOMString") = "ml" then
											response.write("<td align=""right"">" & FormatNumber(cDbl(rsBatches("Qty_Available"))*cDbl(rsBatches("Concentration")),2) & "&nbsp;" & DeliveryUnitName & "</td>")
										else
											response.write("<td align=""right"">" & FormatNumber(rsBatches("Qty_Available"),2) & "&nbsp;" & rsBatches("UOMString") & "</td>")
										end if
									else
										response.write("<td align=""right"">" & FormatNumber(rsBatches("Qty_Available"),2) & "&nbsp;" & rsBatches("UOMString") & "</td>")
									end if
								end if
								response.write("<td align=""right"">" & FormatNumber(rsBatches("Qty_Available"),2) & "&nbsp;" & rsBatches("UOMString") & "</td>")
								if rsBatches("Concentration") <> "" then
									response.write("<td align=""right"">" & rsBatches("Concentration") & "&nbsp;" & rsBatches("UOCString") & "</td>")
								else
									response.write("<td align=""right""></td>")
								end if
								'response.write("<td><span name=""LocationPath"" title=""" & LocationPath & """>" & DisplayPath & "</span></td>")
								if instr(userLocations,ContainerLocationID) > 0 then
									if rsBatches("isRack") = "1" then 
										Response.Write "<td><span name=""LocationPath"" title=""" & Location_Path & """><a class=""MenuLink"" href=""#"" onclick=""OpenDialog('/cheminv/gui/ViewSimpleRackLayout.asp?rackid=" & rsBatches("parent_location_id") & "&locationid=" & LocationID & "&containerid=" & BatchContainerID & "&RackPath=" & server.urlencode(LocationPath) & "', 'Diag1', 2); return false;"">" & DisplayPath & "</a></span></td>"
									else
										Response.Write "<td><span name=""LocationPath"" title=""" & Location_Path & """>" & DisplayPath & "</span></td>"
									end if								
								else								
									Response.Write "<td>Insufficient privilege to view location</span>"
								end if
								response.write("<td>" & RS("SubstanceName") & "</td>")
								response.write("</tr>")
								rsBatches.MoveNext
							wend

							%>
						</table>
						</div>
						
						
					</td>
				</tr>

			</table>

		</td>
		</tr>
	</table>
	</center>
<%end if%>
		</td>
	</tr>
	<tr>		<td align="right"><a HREF="#" onclick="if (opener.location) {opener.location.reload();} window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a></td></td>	</tr>
</table>

</div>

</form>

</body>
</html>
