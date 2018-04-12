<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/xml_source/RS2HTML.asp"-->
<%
Dim ShapeConn
Dim Cmd
Dim RS
dbkey = "ChemInv"

ContainerID = Request("ContainerID")
ShowLinks = Request("ShowLinks")
if ShowLinks = "" or isEmpty(ShowLinks) then ShowLinks = "1"
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
//-->
</script>
<script LANGUAGE="javascript" src="/cheminv/gui/refreshGUI.js"></script>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
</head>
<body>
<form name="form1" action="echo.asp" xaction="NewLocation_action.asp" method="POST">
<input type="hidden" name="ContainerID" value="<%=ContainerID%>">
<table border="0">
	<tr>
		<td>
<%	
	schemaName = Application("ORASCHEMANAME")
	call GetInvShapeConnection()
	
	SQL = "SELECT " & schemaName & ".GUIUTILS.GETBATCHAMOUNTSTRING(Container_ID) AS Batch_Amount_String, " & vblf & _
			" CONTAINER_ID," & vblf & _
			" CONTAINER_NAME," & vblf & _
		    " TO_CHAR(TRUNC(inv_Containers.DATE_Expires),'" & Application("DATE_FORMAT_STRING") & "') AS DATE_Expires,  " & vblf & _
			" QTY_MINSTOCK," & vblf & _
			" DECODE(inv_Containers.Qty_MinStock, NULL, ' ', inv_Containers.Qty_MinStock||' '|| Unit_Abreviation) AS Qty_MinStock_String, " & vblf & _
			" UNIT_ABREVIATION," & vblf & _
			" CONTAINER_STATUS_NAME AS STATUS," & vblf
			if Application("RegServerName") <> "NULL" then
				SQL = SQL & "DECODE(inv_containers.Reg_ID_FK, NULL, NULL, (SELECT reg_numbers.reg_number||'-'||batches.batch_Number FROM reg_numbers, batches WHERE reg_numbers.reg_id = batches.reg_internal_id AND reg_numbers.reg_id= inv_containers.reg_id_fk AND batches.batch_number= inv_containers.Batch_Number_FK)) AS Reg_Batch_ID, "
				SQL = SQL & "DECODE(inv_containers.Reg_ID_FK, NULL, inv_Compounds.BASE64_CDX, (SELECT Structures.BASE64_CDX AS REG_BASE64_CDX FROM structures, batches, reg_numbers WHERE reg_numbers.reg_id = batches.reg_internal_id AND structures.CPD_Internal_ID = batches.CPD_Internal_ID AND reg_numbers.reg_id=inv_containers.reg_id_fk AND batches.batch_number=inv_containers.Batch_Number_FK)) AS Base64_CDX, "
			else
				SQL = SQL & "inv_Compounds.BASE64_CDX AS BASE64_CDX, " & vblf
			End if
	SQL = SQL & " COMPOUND_ID" & vblf & _
			" FROM inv_containers, inv_container_status, inv_compounds, inv_units" & vblf & _
			" WHERE container_status_id_fk = container_status_id" & vblf & _
			    " AND inv_containers.UNIT_OF_MEAS_ID_FK = inv_units.UNIT_ID" & vblf & _
				" AND compound_id_fk = compound_id(+)" & vblf & _
				" AND container_id = " & ContainerID

	'Response.Write SQL 
	'Response.End			
	Shapestr = "SHAPE {" & SQL & "}" & _
               " APPEND ({" & _
               " SELECT container_id, parent_container_id_fk, " & vblf & _
               " DECODE(inv_Containers.Qty_Available, NULL, ' ', inv_Containers.Qty_Available||' '|| Unit_Abreviation) AS AmountAvailable ,  " & vblf & _
			   " " & schemaName & ".GUIUTILS.GETLOCATIONPATH(location_id) AS Location_Path, " & vblf & _               
			   " barcode, location_name, location_id FROM inv_containers, inv_locations, inv_units WHERE location_id_fk = location_id AND unit_of_meas_id_fk = unit_id AND parent_container_id_fk = " & ContainerID & "}" & _
               " RELATE container_id TO parent_container_id_fk) as rsBatches"
	'Response.Write ShapeStr
	'Response.End
	Set ShapeCmd = Server.CreateObject("ADODB.Command")
	ShapeCmd.ActiveConnection = ShapeConn
	ShapeCmd.commandtext = Shapestr
	Set RS = ShapeCmd.execute

	
	'Response.End
	If not (RS.BOF or RS.EOF) then
		BatchAmountString = RS("Batch_Amount_String")
		ContainerName = RS("Container_Name")
		DateExpires = RS("Date_Expires")
		QtyMinStock = RS("Qty_MinStock")
		QtyUnits = RS("UNIT_ABREVIATION")
		QtyMinStockString = RS("Qty_MinStock_String")
		Status = RS("Status")
		Base64CDX = RS("Base64_CDX")	
		Set rsBatches = RS("rsBatches").value
%>
	<center>
	<table border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td>
	<%
		CompoundID = RS("Compound_ID")
		if NOT (IsEmpty(CompoundID) or CompoundID = "") then
		
%>
<table cellpadding="1" cellspacing="2" BORDER="1"><tr><td WIDTH="185" HEIGHT="130">
<%Response.Write ShowInlineStructure(structure, 200, 300, 1, Base64CDX)%>
</td></tr></table>
<%
		Else
			Response.Write "<BR><BR><span class=""GUIFeedback"">No substance associated with this container</span>"
		End if
	%>
		</td>
		<td valign="top">
			<table border="0" cellpadding="1" cellspacing="2">
			<tr><!-- Header Row -->
					<td colspan="4" align="center">
						&nbsp;<em><b><%=TruncateInSpan(ContainerName, 50, "")%></b></em>
					</td>
				</tr>
				<tr>
					<!-- Row 1 Col 1-->
					<!-- for formula id must be MOLWEIGHT -->		
					<%=ShowField("Amount on Hand:", "BatchAmountString", 15, "BatchAmountString")%>
					<!-- Row 1 Col 2-->
					<%=ShowField("ReOrder Point:", "QtyMinStockString", 15, "QtyMinStockString")%>
					
				</tr>
				<tr>
					<!-- Row 1 Col 1-->
					<!-- for formula id must be MOLWEIGHT -->		
					<%=ShowField("Expiration Date:", "DateExpires", 15, "DateExpires")%>
					<!-- Row 1 Col 2-->
					<TD></TD>
					<TD></TD>
					<%'=ShowField("Status:", "Status", 15, "Status")%>
					
				</tr>
				<tr>
					<td colspan="4">&nbsp;<br></td>
				</tr>
				<%
				IsFirst = true
				while not (rsBatches.BOF or rsBatches.EOF)
					if IsFirst then
						DisplayName="Batch Details:"
						IsFirst = false
					else
						DisplayName=""
					end if
					LocationName = rsBatches("location_name")
					LocationPath = rsBatches("Location_Path")
					LocationID = rsBatches("Location_ID")
					BatchContainerID = rsBatches("Container_ID")
					DisplayText = ""
					if ShowLinks = "1" then
						DisplayText = "<A CLASS=""MenuLink"" HREF=""#"" TITLE=""" & barcode & """ ONCLICK=""SelectLocationNode(0," & LocationID & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & BatchContainerID & "); opener.focus(); window.close();"" onmouseover=""javascript:this.style.cursor='hand';"" onmouseout=""javascript:this.style.cursor='default';"">"
						DisplayText = DisplayText & rsBatches("AmountAvailable") & " in " & rsBatches("barcode") 
						DisplayText = DisplayText & "</a>"
					else
						DisplayText = DisplayText & rsBatches("AmountAvailable") & " in " & rsBatches("barcode") 
					end if
				%>
				<tr>
					<td align="right" valign="top" nowrap><%=DisplayName%></td>
					<td bgcolor="#d3d3d3" align="right"><%=DisplayText%></td>
					<%'=ShowField(DisplayName, "DisplayText", 15, "DisplayText")%>
					<td align="right" valign="top" nowrap>in: </td>
					<td bgcolor="#d3d3d3" align="right"><span name="LocationPath" title="<%=LocationPath%>"><%=LocationName%></span></td>
				</tr>
				
				<%
					rsBatches.MoveNext
				wend
				
				%>
			</table>

		</td>
		</tr>
	</table>
	</center>	
<%end if%>
		</td>
	</tr>
	<tr>
		<td align="right"><a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
</td>
	</tr>
</table>

</form>
	
</body>
</html>
<%

%>