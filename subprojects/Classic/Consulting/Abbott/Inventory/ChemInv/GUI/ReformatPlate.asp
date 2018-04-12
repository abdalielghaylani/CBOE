<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<%
Dim Conn
Dim Cmd
Dim RS
bDebugPrint = false
Call GetInvConnection()
multiSelect = lcase(Request("multiSelect"))
LocationID = Request.QueryString("LocationID")

Set myDict = plate_multiSelect_dict
if multiSelect = "true" then 
	PlateID = DictionaryToList(myDict)
	Barcode =  myDict.count & " plates selected."
Else
	PlateID = Session("plPlate_ID")
	Barcode = Session("plBarcode")
End if
pageAction = Request("pageAction")
if pageAction = "" then pageAction = "selectMap"

if pageAction = "selectMap" then 
	instructionText = "Reformat inventory plates."
	stepCount = 4
	stepValue = 1
	nextPageAction = "selectOrder"
elseif pageAction = "selectOrder" then 
	instructionText = "Select the order for the source plates."
	stepCount = 4
	stepValue = 2
	XMLDoc_ID = Request("XMLDoc_ID")
elseif pageAction = "daughter" then
	instructionText = "Select the plate data entry mode."
	stepCount = 3
	stepValue = 1
	action="daughter"
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.GetDaughterReformatMaps", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PSOURCEPLATEIDS", 200, 1, 2000, PlateID)
	'if bDebugPrint then call DebugCommand(Cmd, true)
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reformat.GetDaughterReformatMaps")
	XMLDoc_ID = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))
	arrPlateIDs = split(PlateID, ",")
	arrXMLDocIDs = split(XMLDoc_ID, ",")
	bXMLDocExists = true
	for i = 0 to ubound(arrXMLDocIDs)
		XMLDocID = arrXMLDocIDs(i)
		if XMLDocID = "-1" then 
			noMapPlates = noMapPlates & arrPlateIDs(i) & ","
			bXMLDocExists = false
		end if
	next
End if

if pageAction = "selectOrder" or pageAction = "daughter" then
	'check that the source plates have material in them
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REFORMAT.CheckPlateQuantity", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PSOURCEPLATEIDS", 200, 1, 2000, PlateID)
	'if bDebugPrint then call DebugCommand(Cmd, true)
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REFORMAT.CheckPlateQuantity")
	out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))
	bValidSourcePlates = false
	if out = "1" then bValidSourcePlates = true
	if not bValidSourcePlates then errText = out

	if bValidSourcePlates then
		SQL = "SELECT plate_id, plate_barcode, plate_name FROM " & Application("CHEMINV_USERNAME") & ".inv_plates WHERE plate_id IN (" & PlateID & ")"
		Set RS = Server.CreateObject("ADODB.recordset")
		RS.Open SQL, Conn, adOpenKeyset, adLockOptimistic, adCmdText 
		numSourcePlates = RS.RecordCount
		if not bXMLDocExists and len(XMLDoc_ID) = 0 then
			'remove extra comma from list
			noMapPlates = left(noMapPlates,len(noMapPlates)-1)
			while not rs.eof 
				if instr(noMapPlates,rs("plate_id")) then
					errText = errText & "No reformat map available to daughter plate " & rs("plate_barcode") & ".<BR>"
				end if
				rs.movenext
			wend
		else
			Set plateBarcode_multiSelect_dict = server.CreateObject("Scripting.Dictionary")
			While not rs.eof 
				plateBarcode_multiSelect_dict.Add cstr(RS("plate_id")),cStr(RS("plate_barcode"))
				rs.movenext
			wend
			Set Session("plateBarcode_multiSelect_dict") = plateBarcode_multiSelect_dict
			rs.moveFirst
		end if
	end if
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Reformat Inventory Plates</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	function ValidateMove(){
		var bWriteError = false;

		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.all.form1.action = "#";
			document.form1.submit();
		}
	}
	
	function ValidateOrder(numSourcePlates){
		var bWriteError = false;
		var bChecked;
		var bSpotFilled;
		var errmsg = "Please fix the following problems:\r";
		
		if (numSourcePlates == 1){
			i = 0;
			bSpotFilled = false;
			bChecked = eval("document.all.plateOrder" + i + ".checked");
			if (bChecked) 
				bSpotFilled = true; 
			if (!bSpotFilled) {
				errmsg = errmsg + "- You must select a plate for Position" + (i+1) + ".\r";
				bWriteError = true;
			}
		
		}
		else {
		for (i=0; i<numSourcePlates; i++) {
			bSpotFilled = false;
			for (j=0; j<numSourcePlates; j++){
				bChecked = eval("document.all.plateOrder" + i + "[" + j + "].checked");
				if (bChecked) 
					bSpotFilled = true; 
			}
			if (!bSpotFilled) {
				errmsg = errmsg + "- You must select a plate for Position" + (i+1) + ".\r";
				bWriteError = true;
			}
		}
		
		}
		/*
		if (document.all.dataMode[0].checked == false && document.all.dataMode[1].checked == false){
			errmsg = errmsg + "- You must select a data entry mode.\r";
			bWriteError = true;
		}
		*/
		if (bWriteError){
			alert(errmsg);
		}
		else{
			//order the plates
			numSourcePlates = document.all.numSourcePlates.value;
			tempPlateID = "";
			if (numSourcePlates == 1){
				PlateID = document.all.plateOrder0.value;
			}
			else {
			for(i=0; i<numSourcePlates; i++) {
				element = eval("document.all.plateOrder" + i);
				for(j=0; j<numSourcePlates; j++) {
					if (element[j].checked) {
						tempPlateID = tempPlateID + element[j].value + ",";
					}
				}
			}
			PlateID = tempPlateID.substr(0,tempPlateID.length-1);
			}
			document.all.PlateID.value = PlateID;
			
			
			//choose the appropriate page to go to
			//formAction = "ReformatPlate_TargetCriteria.asp";
			//if (document.all.dataMode[0].checked) 
				formAction = "ReformatPlate_SourceSolvate.asp";
			document.form1.action = formAction;
			document.form1.submit();
		}	
	
	}
	
	function ValidateDataMode() {
		var bWriteError = false;
		var bChecked;
		var bSpotFilled;
		var errmsg = "Please fix the following problems:\r";
	
		if (document.all.dataMode[0].checked == false && document.all.dataMode[1].checked == false){
			errmsg = errmsg + "- You must select a data entry mode.\r";
			bWriteError = true;
		}
		if (bWriteError){
			alert(errmsg);
		}
		else{
			//choose the appropriate page to go to
			formAction = "ReformatPlate_TargetCriteria.asp";
			if (document.all.dataMode[0].checked) 
				formAction = "ReformatPlate_SourceSolvate.asp";
			document.form1.action = formAction;
			document.form1.submit();
		}
	}
	
	function DaughterValidation() {
		var bWriteError = false;
		var bChecked;
		var bSpotFilled;
		var errmsg = "Please fix the following problems:\r";

		// numDaughterPlates is required
		if (document.form1.numDaughterPlates.value.length == 0) {
			errmsg = errmsg + "- # of Daughter Plates is required.\r";
			bWriteError = true;
		}
		else{
			// Initial amount must be a number
			if (!isPositiveNumber(document.form1.numDaughterPlates.value)){
				errmsg = errmsg + "- # of Daughter Plates must be a positive number.\r";
				bWriteError = true;
			}
			if (document.form1.numDaughterPlates.value > 999999999){
				errmsg = errmsg + "- # of Daughter Plates is too large.\r";
				bWriteError = true;
			}
		}
	
		if (bWriteError){
			alert(errmsg);
		}
		else{
			//validate Data Mode
			//ValidateDataMode();
			formAction = "ReformatPlate_SourceSolvate.asp";
			document.form1.action = formAction;
			document.form1.submit();
		}
	
	
	}
	
	function ClearSelections(plateID, element, numSourcePlates) {
		curPlateName = element.name;
		curPlateValue = element.value;
		
		for (i=0; i<numSourcePlates; i++) {
			for (j=0; j<numSourcePlates; j++){
				value = eval("document.all.plateOrder" + i + "[" + j + "].value");
				name = eval("document.all.plateOrder" + i + "[" + j + "].name");
				if (value == curPlateValue && name != curPlateName) {
					eval("document.all.plateOrder" + i + "[" + j + "].checked = false");				
				}
			}
		}
		//document.all.item("
/*
		test = document.all.Plate1[1].value;
		document.all.Plate1[2].checked = true;
		alert(curPlateValue);
*/
	}
	
-->
</script>
</head>
<body>
<center>
<form name="form1" method="POST">
<INPUT TYPE="hidden" NAME="multiselect" VALUE="<%=multiselect%>">
<input Type="hidden" name="PlateID" value="<%=PlateID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<INPUT TYPE="hidden" NAME="pageAction" VALUE="<%=nextPageAction%>">
<INPUT TYPE="hidden" NAME="numSourcePlates" VALUE="<%=numSourcePlates%>">
<INPUT TYPE="hidden" NAME="reformatAction" VALUE="<%=action%>">
<INPUT TYPE="hidden" NAME="stepCount" VALUE="<%=stepCount%>">
<INPUT TYPE="hidden" NAME="stepValue" VALUE="<%=stepValue%>">
<table border="0" width="100%">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">
			<%if len(errText) > 0  then
				Response.Write errText
			else 
				Response.Write instructionText
			end if
			%>
			</span><br><br>
			<table bgcolor="#e1e1e1" width="100%">
				<tr><td align="center">Step <strong><%=stepValue%></strong> of <%=stepCount%></td></tr>
			</table>	
		</td>
	</tr>
</table>
<table border="0">
<%
	if pageAction = "selectMap" then
		'get valid maps for selected plates, also checks that the source plates are valid
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.GETVALIDREFORMATMAPS", adCmdStoredProc)
		Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
		Cmd.Parameters.Append Cmd.CreateParameter("PSOURCEPLATEIDS", 200, 1, 2000, PlateID)
		'if bDebugPrint call then DebugCommand(Cmd, true)
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reformat.GETVALIDREFORMATMAPS")
		validMaps = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))
		'Response.Write validMaps & "=validMaps<BR>"
		'Response.Write PlateID
		if cInt(validMaps) < 0 then
%>
		<TR>
			<TD COLSPAN="2">There are no valid reformat maps for the selected plates.</TD>
		</TR>
		<tr>
			<td colspan="2" align="right"> 
				<a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
			</td>
		</tr>	
<%	
		else
%>	
		<tr>
			<td align="right" nowrap>Plates to reformat:</td>
			<td>
				<input TYPE="text" SIZE="33" onfocus="blur()" VALUE="<%=Barcode%>" disabled>
			</td>
		</tr>
		<tr height="25">
			<%=ShowPickList("<span class=""required"" title=""Select a Reformat Map"">Select a Reformat Map:</span>", "XMLDoc_ID", xmldoc_id, "SELECT XMLDOC_ID AS Value, NAME AS DisplayText FROM cheminvdb2.inv_xmldocs WHERE xmldoc_type_id_FK IN (1) AND xmldoc_id in (" & validMaps & ") ORDER BY lower(DisplayText) ASC")%>
		</tr>
		<tr>
			<td colspan="2" align="right"> 
				<a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;<a HREF="#" onclick="ValidateMove(); return false;"><img SRC="../graphics/next_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
			</td>
		</tr>	
		<%end if%>

<%elseif pageAction = "selectOrder" and bValidSourcePlates then%>	
	<INPUT TYPE="hidden" NAME="XMLDoc_ID" VALUE="<%=XMLDoc_ID%>">
	<%if numsourcePlates > 1 then%>
	<TR>
		<TD><B>Barcode</B></TD>
		<TD><B>Position</B></TD>
	</TR>
	<%
	end if
	barcodeList = ""
	selected = ""
	selectNum = 0
	While not RS.EOF 
		curPlateID = RS("plate_id")
		curPlateBarcode = RS("plate_barcode")
		barcodeList = barcodeList & curPlateBarcode & ","
		if numsourcePlates = 1 then
			i=0
			theRow = "<span style=""display:none"">"
			theRow = theRow & "<INPUT TYPE=""radio"" NAME=""plateOrder" & i & """ VALUE=""" & curPlateID & """ CHECKED>" & (i+1)
			theRow = theRow & "</span>"
		else
			theRow = "<TR><TD ALIGN=""left"">" & curPlateBarcode & "</TD>"
			theRow = theRow & "<TD>"
			for i=0 to numSourcePlates-1
				if i = selectNum then selected = "CHECKED"
				theRow = theRow & "<INPUT TYPE=""radio"" NAME=""plateOrder" & i & """ VALUE=""" & curPlateID & """ ONCLICK=""ClearSelections(" & curPlateID & ", this, " & numSourcePlates & "); "" " & selected & ">" & (i+1)
				selected = ""
			next
			theRow = theRow & "</TD></TR>" & chr(13)
		end if
		Response.Write theRow
		selectNum = selectNum + 1
		RS.MoveNext
	wend
	barcodeList = left(barcodeList,len(barcodeList)-1)%>
	<tr>
		<td colspan="2" align="right"> 
			<INPUT TYPE="hidden" NAME="barcodeList" VALUE="<%=barcodeList%>">
			<a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;<a HREF="#" onclick="history.back(); return false;"><img SRC="../graphics/btn_back_61.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateOrder(<%=numSourcePlates%>); return false;"><img SRC="../graphics/btn_next_61.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
<%elseif pageAction = "daughter" then%>
	<%if not bXMLDocExists or not bValidSourcePlates then%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	

	<%else%>
	<INPUT TYPE="hidden" NAME="XMLDoc_ID" VALUE="<%=XMLDoc_ID%>">
	<%
	While not RS.EOF 
		curPlateBarcode = RS("plate_barcode")
		barcodeList = barcodeList & curPlateBarcode & ","
		RS.MoveNext
	wend
	barcodeList = left(barcodeList,len(barcodeList)-1)
	%>
	<tr>
		<td align="right" width="150" nowrap><span class="required"># of Daughter Plates:</span></td>
		<td><input TYPE="text" SIZE="15" Maxlength="50" NAME="numDaughterPlates" VALUE="1"></td>
	</tr>	
	<tr>
		<td colspan="2" align="right"> 
			<INPUT TYPE="hidden" NAME="barcodeList" VALUE="<%=barcodeList%>">
			<a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;<a HREF="#" onclick="DaughterValidation(); return false;"><img SRC="../graphics/btn_next_61.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
	<%end if%>

<%else%>
	<TR>
		<TD COLSPAN="2" ALIGN="right">Please adjust your source plate selections.</TD>
	</TR>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
<%end if%>	
	
</table>	
</form>
</center>
</body>
</html>
<%
Conn.Close
Set Cmd = Nothing
Set Conn = Nothing
%>
