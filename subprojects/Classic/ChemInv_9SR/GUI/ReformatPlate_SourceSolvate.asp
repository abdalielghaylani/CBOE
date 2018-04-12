<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<%
Dim Conn
Dim RS

multiSelect = Request("multiSelect")
'dataMode = Request("dataMode")
PlateID = Request("PlateID")
multiscan = Request("multiscan")
numSourcePlates = Request("numSourcePlates")
XMLDoc_ID = Request("XMLDoc_ID")
if numSourcePlates = "" then numSourcePlates = 1
barcodeList = Request("barcodeList")
arrBarcodeList = split(barcodeList,",")
reformatAction = Request("reformatAction")
numDaughterPlates = Request("numDaughterPlates")
stepCount = Request("stepCount")
stepValue = cint(Request("stepValue")) + 1
pageMode = Request("pageMode")
if isEmpty(pageMode) then pageMode = "reformat" 
locationID = Request("locationID")

if pageMode = "reformat" then
	pageVerb = "Solvate"
	displayStyle = "none"
	displayStyleCB = "block"
	yesChecked = ""
	noChecked = "CHECKED"
else
	pageVerb = "Dilute"
	displayStyle = "block"
	displayStyleCB = "none"
	yesChecked = "CHECKED"
	noChecked = ""
end if

if multiSelect = "true" then 
	Set myDict = plate_multiSelect_dict
	if pageMode = "dilute" then 
		PlateID = DictionaryToList(myDict) 
		numSourcePlates = myDict.count
	end if
	Barcode =  myDict.count & " plates selected."
	if pageMode = "reformat" then
		instructionText = "Add solvent to the source plates."
	else
		instructionText = "Dilute these inventory plates."
	end if
Else
	if pageMode = "reformat" then
		instructionText = "Add solvent to the source plate."
	else
		instructionText = "Dilute this inventory plate."
	end if
End if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=pageVerb%> Plates</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	function Validate(numSourcePlates){
		var bApplyToAll = false;
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r\r";
		
		for (i=0; i<numSourcePlates; i++) {
			<% if numSourcePlates > 1 then %>
				ApplyToAll = eval("document.all.applyToAll" + i + ".checked");
			<% else %>
				ApplyToAll = false;
			<% end if %>
			MolarAmount = eval("document.all.MolarAmount" + i + ".value");
			PlateBarcode = eval("document.all.PlateBarcode" + i + ".value");
			//molar amount for dilute plate must be a positive number			
			//if (MolarAmount.length == 0 && bApplyToAll == false) {
			//	errmsg = errmsg + " - The Molar Amount for " + PlateBarcode + " must be a positive number greater than zero.\r";
			//	bWriteError = true;
			//}
			//alert(document.all.solvate0[0].value);
			if (eval("document.all.solvate" + i + "[0].checked") == true){
				bApplyToAll = true;
				//must select a solvent
				if (eval("document.all.SolventID" + i + ".value") == "null") {
					bWriteError = true
					errmsg = errmsg + "- You must select a Solvent for plate " + PlateBarcode  + ".\r";
				}
				//must enter a solvent volume
				if (eval("document.all.SolventVolumeAdded" + i + ".value.length") == 0) {
					bWriteError = true
					errmsg = errmsg + "- You must enter a Solvent Volume for plate " + PlateBarcode + ".\r";
				}
				//target concentration for dilute plate must be a positive number			
				if (eval("document.all.Concentration" + i + ".value.length") == 0) {
					errmsg = errmsg + "- You must enter a Target Concentration for plate " + PlateBarcode + ".\r";
					bWriteError = true;
				}
				
			}
		} 

		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
			document.form1.submit();
		}
	}
	
	function ShowHide(plateNumber, elementValue){
		displayValue = "none";

		if (elementValue == "yes") 
			displayValue = "block";
		eval("document.all.div" + plateNumber + ".style.display = '" + displayValue + "'");
	}
	
	function CalculateConcentration(frmField, plateNumber){
		var errmsg = 'Please fix the following error(s):\r\r';

		//calculate molar concentration
		MolarAmount = eval("document.all.MolarAmount" + plateNumber + ".value");
		MolarAmountEncoded = eval("document.all.MolarAmountEncoded" + plateNumber + ".value");
		MolarUnitFK = eval("document.all.MolarUnitFK" + plateNumber + ".value");
		SolventVolumeAdded = eval("document.all.SolventVolumeAdded" + plateNumber + ".value");
		SolventVolumeAddedUnitID = eval("document.all.SolventVolumeAddedUnitID" + plateNumber + ".value");
		CurrSolventVolume = eval("document.all.CurrSolventVolume" + plateNumber + ".value");
		CurrSolventVolumeUnitID = eval("document.all.CurrSolventVolumeUnitID" + plateNumber + ".value");
		//ConcentrationUnitID = eval("document.all.ConcentrationUnitID" + plateNumber + ".value");
		ConcentrationUnitID = 15;
		if (!isPositiveNumber(SolventVolumeAdded)) {
				errmsg = errmsg + " - Please enter a positive number.\r";
				frmField.value = '';
				alert(errmsg);
				frmField.focus();				
		} else if (MolarAmount.length == 0) {
			errmsg = errmsg + " - The concentration could not be calculated.  The plate has no material.\r";
			alert(errmsg);
		} else if (SolventVolumeAdded.length != 0){
			var strURL = "http://" + serverName + "/cheminv/api/CalcMolarConc.asp?MolarAmount=" + MolarAmountEncoded + "&MolarUnitFK=" + MolarUnitFK + "&SolventVolume1=" + SolventVolumeAdded + "&SolventVolumeUnitID1=" + SolventVolumeAddedUnitID + "&SolventVolume2=" + CurrSolventVolume + "&SolventVolumeUnitID2=" + CurrSolventVolumeUnitID + "&ConcentrationUnitID=" + ConcentrationUnitID;	
			strURL = URLEncode(strURL);
			var httpResponse = JsHTTPGet(strURL) 
			/*
			alert(httpResponse);
			alert(strURL);
			httpResonse = 1;
			*/
			if (httpResponse == -1) {
				errmsg = errmsg + " - The concentration could not be calculated.  Please try a different volume unit.\r";
				eval("document.all.Concentration" + plateNumber + ".value = 'Invalid'");
				alert(errmsg);
			}
			else if (httpResponse < 0) {
				errmsg = errmsg + " - The concentration could not be calculated.  Please try a different volume unit.\r";
				eval("document.all.Concentration" + plateNumber + ".value = 'Invalid'");
				alert(errmsg);
			}
			else {
				concentration = parseFloat(httpResponse);
				eval("document.all.Concentration" + plateNumber + ".value = concentration");
				eval("theSelect = document.all.ConcentrationUnitID" + plateNumber);
				//find the molar option
				for (i=0;i<theSelect.options.length;i++)
				{
					if (theSelect.options[i].value == 15)
						newSelectedIndex = i;
				}
				theSelect.selectedIndex = newSelectedIndex;
			}	
		}
	}
	function CalculateVolume(frmField, plateNumber){
		var errmsg = 'Error:\r';
	
		//calculate molar concentration
		MolarAmount = eval("document.all.MolarAmount" + plateNumber + ".value");
		MolarAmountEncoded = eval("document.all.MolarAmountEncoded" + plateNumber + ".value");
		MolarUnitFK = eval("document.all.MolarUnitFK" + plateNumber + ".value");
		Concentration = eval("document.all.Concentration" + plateNumber + ".value");
		ConcentrationUnitID = eval("document.all.ConcentrationUnitID" + plateNumber + ".value");
		//ConcentrationUnitID = eval("document.all.ConcentrationUnitID" + plateNumber + ".value");
		CurrSolventVolume = eval("document.all.CurrSolventVolume" + plateNumber + ".value");
		CurrSolventVolumeUnitID = eval("document.all.CurrSolventVolumeUnitID" + plateNumber + ".value");
		VolumeUnitID = 4;
		if (!isPositiveNumber(frmField.value)) {
				errmsg = errmsg + " - Please enter a positive number.\r";
				frmField.value = '';
				alert(errmsg);
				frmField.focus();				
		} 
		else if (MolarAmount.length == 0) {
			errmsg = errmsg + "- The solvent added could not be calculated.  The plate has no material.\r";
			alert(errmsg);
		}
		else if (Concentration.length != 0){
			var strURL = "http://" + serverName + "/cheminv/api/CalcVolume.asp?MolarAmount=" + MolarAmountEncoded + "&MolarUnitFK=" + MolarUnitFK + "&Concentration=" + Concentration + "&ConcentrationUnitID=" + ConcentrationUnitID + "&VolumeUnitID=" + VolumeUnitID + "&CurrSolventVolume=" + CurrSolventVolume + "&CurrSolventVolumeUnitID=" + CurrSolventVolumeUnitID;	
			strURL = URLEncode(strURL);
			var httpResponse = JsHTTPGet(strURL) 
			var concUnitName = eval("document.all.ConcentrationUnitID" + plateNumber + ".options[document.all.ConcentrationUnitID" + plateNumber + ".selectedIndex].text");
			/*
			alert(strURL);
			alert(httpResponse);
			httpResponse = 1;
			*/
			if (httpResponse == -1) {
				errmsg = errmsg + "- The solvent added could not be calculated.  Possible reasons include:\r";
				errmsg = errmsg + "  - no conversion factor from " + concUnitName + " to Molar exists in your system.\r";
				errmsg = errmsg + "    Try selecting the concentration unit before entering the concentration value.\r";
				errmsg = errmsg + "  - an error occurred.\r";
				eval("document.all.Concentration" + plateNumber + ".value = ''");
				eval("document.all.SolventVolumeAdded" + plateNumber + ".value = ''");
				eval("document.all.Concentration" + plateNumber + ".focus()");
				alert(errmsg);
				//eval("document.all.SolventVolumeAdded" + plateNumber + ".value = 'Invalid'");
			}
			else if (httpResponse < 0) {
				errmsg = errmsg + "- This concentration is greater than the current concentration.\r"
				alert(errmsg);
				eval("document.all.Concentration" + plateNumber + ".value = ''");
				eval("document.all.Concentration" + plateNumber + ".focus()");
			}
			else {
				SolventVolumeAdded = httpResponse;
				eval("document.all.SolventVolumeAdded" + plateNumber + ".value = SolventVolumeAdded");
				eval("document.all.SolventVolumeAddedUnitID" + plateNumber + ".value = VolumeUnitID");
				//totalSolventVolume = parseFloat(SolventVolumeAdded) + parseFloat(CurrSolventVolume);
				//eval("document.all.SolventVolume" + plateNumber + ".value = totalSolventVolume");
			}	
		}
	}

	function applyToAll(plateNumber, element) {
		var i;
		if (element.checked) {
			SolventID = eval("document.all.SolventID" + plateNumber + ".value");
			SolventVolumeAdded = eval("document.all.SolventVolumeAdded" + plateNumber + ".value");
			Concentration = eval("document.all.Concentration" + plateNumber + ".value");
			numSourcePlates = document.all.numSourcePlates.value;
			for (i=0; i<numSourcePlates; i++) {
				if (i != plateNumber) {
					eval("document.all.solvate" + i + "[0].checked = true");
					ShowHide(i,"yes");
					if (eval("document.all.SolventID" + i + ".options")) {
						selectOption("SolventID"+i,SolventID);
					}
					SolventVolumeAddedUnitID = eval("document.all.SolventVolumeAddedUnitID" + plateNumber + ".options[document.all.SolventVolumeAddedUnitID" + plateNumber + ".selectedIndex].value");
					selectOption("SolventVolumeAddedUnitID" + i, SolventVolumeAddedUnitID);						
					eval("document.all.SolventVolumeAdded" + i + ".value = SolventVolumeAdded");
					eval("document.all.applyToAll" + i + ".checked = false");
					CalculateConcentration(element,i);
				}
			}
		}
	}
	
	function selectOption(elementName, selectedValue) {
		element = eval("document.all." + elementName);	
		optionLength = element.length;
		for (j=0; j<optionLength; j++) {
			if (element.options[j].value == selectedValue)
				element.options[j].selected = true;
		}		
	}
	
-->
</script>

</head>
<body>
<center>
<form name="form1" action="ReformatPlate_SourceSolvate_action.asp" method="POST">
<INPUT TYPE="hidden" NAME="multiselect" VALUE="<%=multiselect%>">
<!--<INPUT TYPE="hidden" NAME="dataMode" VALUE="<%=dataMode%>">-->
<input Type="hidden" name="PlateID" value="<%=PlateID%>">
<INPUT TYPE="hidden" NAME="numSourcePlates" VALUE="<%=numSourcePlates%>">
<INPUT TYPE="hidden" NAME="XMLDoc_ID" VALUE="<%=XMLDoc_ID%>">
<INPUT TYPE="hidden" NAME="reformatAction" VALUE="<%=reformatAction%>">
<INPUT TYPE="hidden" NAME="numDaughterPlates" VALUE="<%=numDaughterPlates%>">
<INPUT TYPE="hidden" NAME="stepCount" VALUE="<%=stepCount%>">
<INPUT TYPE="hidden" NAME="stepValue" VALUE="<%=stepValue%>">
<INPUT TYPE="hidden" NAME="pageMode" VALUE="<%=pageMode%>">
<INPUT TYPE="hidden" NAME="locationID" VALUE="<%=locationID%>">
<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback"><%=instructionText%></span><br><br>
			<%if pageMode = "reformat" then%>
			<table bgcolor="#e1e1e1" width="100%">
				<tr><td align="center">Step <strong><%=stepValue%></strong> of <%=stepCount%></td></tr>
			</table>	
			<%end if%>
		</td>
	</tr>
	<%
	Call GetInvConnection()
	SQL = "SELECT plate_barcode, to_char(molar_amount, '9.99EEEE') as molar_amount, to_char(molar_conc, '9.99EEEE') as molar_conc, " & Application("CHEMINV_USERNAME") & ".chemcalcs.convert(solvent_volume, solvent_volume_unit_id_fk,4) as solvent_volume, plate_id, solvent_id_fk, solvent_name FROM inv_plates p, inv_solvents WHERE plate_id IN (" & PlateID & ") AND p.solvent_id_fk = solvent_id(+)"
	'Response.Write SQL
	Set RS = Server.CreateObject("ADODB.recordset")
	RS.Open SQL, Conn, adOpenKeyset, adLockOptimistic, adCmdText 
	numSourcePlates = RS.RecordCount

	for i=0 to numSourcePlates-1
		molar_amount = RS("molar_amount")
		molar_conc = RS("molar_conc")
		solvent_volume = RS("solvent_volume")
		if solvent_volume = "-1" or isNull(solvent_volume) then solvent_volume = "0"
	%>
	<TR>
		<TD ALIGN="right" WIDTH="200" NOWRAP><%=pageVerb%> plate <B><%=RS("plate_barcode")%><%'=arrBarcodeList(i)%></B>:</TD>
		<TD WIDTH="300">
		<DIV STYLE="display:<%=displayStyleCB%>;" ID="divCB<%=i%>">
			<INPUT TYPE="radio" NAME="solvate<%=i%>" VALUE="yes" ONCLICK="ShowHide(<%=i%>, this.value);" <%=yesChecked%>>Yes
			<INPUT TYPE="radio" NAME="solvate<%=i%>" VALUE="no" ONCLICK="ShowHide(<%=i%>, this.value);" <%=noChecked%>>No
		</DIV>
			<INPUT TYPE="hidden" NAME="plateOrder<%=i%>" VALUE="<%execute("Request(""plateOrder" & i & """)")%>">
			<INPUT TYPE="hidden" NAME="PlateID<%=i%>" VALUE="<%=RS("plate_id")%>">
			<INPUT TYPE="hidden" NAME="PlateBarcode<%=i%>" VALUE="<%=RS("plate_barcode")%><%'=arrBarcodeList(i)%>">
			<INPUT TYPE="hidden" NAME="CurrSolventVolume<%=i%>" VALUE="<%=solvent_volume%>">			
			<INPUT TYPE="hidden" NAME="CurrSolventVolumeUnitID<%=i%>" VALUE="4">			
		</TD>
	</TR>
	<TR><TD COLSPAN="2"><DIV STYLE="display:<%=displayStyle%>;" ID="div<%=i%>">
	<TABLE BORDER="0">
	<%
	'-- don't show concentration fields or molar amount
	if isnull(RS("molar_amount")) then%>
		<INPUT TYPE="hidden" NAME="Concentration<%=i%>" VALUE="null">			
		<INPUT type="hidden" name="ConcentrationUnitID<%=i%>" value="15" />
		<tr>
			<%
			if isNull(RS("solvent_id_fk")) then
			%>
			<td CLASS="required" align=right width="200" nowrap>Select Solvent:</td>
			<td>
			<%
				Response.Write(ShowSelectBox2("SolventID" & i,RS("solvent_id_fk"),"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null"))
			else
			%>
			<td CLASS="required" align=right width="200" nowrap>Solvent:</td>
			<td>
			<%
				Response.Write("<INPUT TYPE=""hidden"" NAME=""SolventID" & i & """ VALUE=""" & RS("solvent_id_fk") & """>")					
				Response.Write("<INPUT TYPE=""text"" CLASS=""GrayedText"" SIZE=""10"" NAME=""SolventName" & i & """ VALUE=""" & RS("solvent_name") & """>")					
			end if	
			%>
			</td>
		</tr>
		<tr>
			<td CLASS="required" align="right" width="200" nowrap>Solvent Volume Added:</td>
			<td>
				<input TYPE="text" SIZE="10" Maxlength="50" NAME="SolventVolumeAdded<%=i%>" VALUE="<%=SolventVolume%>" ONCHANGE="CalculateConcentration(this,<%=i%>);">
				<%=ShowSelectBox3("SolventVolumeAddedUnitID" & i, Application("plDefVolUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC",null,null,null,"")%>
			</td>
		</tr>
		<%if cLng(solvent_volume) > 0 then%>
		<tr>
			<td align="right" width="200" nowrap>Current Solvent Volume (&#956;L):</td>
			<td>
				<input TYPE="text" CLASS="GrayedText" SIZE="10" Maxlength="50" NAME="dummySolventVolume" VALUE="<%=solvent_volume%>" READONLY>
			</td>
		</tr>
		<%end if%>
    <%else%>
		<tr>
			<td align="right" width="200" nowrap>Molar Amount (mol):</td>
			<td>
			<input TYPE="text" CLASS="GrayedText" SIZE="10" Maxlength="50" NAME="MolarAmount<%=i%>" VALUE="<%=RS("molar_amount")%>" READONLY>
			<INPUT TYPE="hidden" NAME="MolarUnitFK<%=i%>" VALUE="<%'=RS("molar_unit_fk")%>">
			<INPUT TYPE="hidden" NAME="MolarAmountEncoded<%=i%>" VALUE="<%=server.URLEncode(iif(isNull(molar_amount),"",trim(molar_amount)))%>">
			</td>
		</tr>
		<tr>
			<%
			if isNull(RS("solvent_id_fk")) then
			%>
			<td CLASS="required" align=right width="200" nowrap>Select Solvent:</td>
			<td>
			<%
				Response.Write(ShowSelectBox2("SolventID" & i,RS("solvent_id_fk"),"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null"))
			else
			%>
			<td CLASS="required" align=right width="200" nowrap>Solvent:</td>
			<td>
			<%
				Response.Write("<INPUT TYPE=""hidden"" NAME=""SolventID" & i & """ VALUE=""" & RS("solvent_id_fk") & """>")					
				Response.Write("<INPUT TYPE=""text"" CLASS=""GrayedText"" SIZE=""10"" NAME=""SolventName" & i & """ VALUE=""" & RS("solvent_name") & """>")					
			end if	
			%>
			</td>
		</tr>
		<tr>
			<td CLASS="required" align="right" width="200" nowrap>Solvent Volume Added:</td>
			<td>
				<input TYPE="text" SIZE="10" Maxlength="50" NAME="SolventVolumeAdded<%=i%>" VALUE="<%=SolventVolume%>" ONCHANGE="CalculateConcentration(this,<%=i%>);">
				<%=ShowSelectBox3("SolventVolumeAddedUnitID" & i, Application("plDefVolUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC",null,null,null,"CalculateConcentration(this," & i &");")%>
			</td>
		</tr>
		<TR>
			<TD CLASS="required" ALIGN="right" WIDTH="200" NOWRAP>Target Concentration:</TD>
			<TD>
				<INPUT TYPE="text" SIZE="10" MAXLENGTH="50" NAME="Concentration<%=i%>" VALUE="" ONCHANGE="CalculateVolume(this,<%=i%>);">
				<%=ShowSelectBox3("ConcentrationUnitID" & i, Application("plDefConcUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (3) ORDER BY lower(DisplayText) ASC",null,null,null,"CalculateVolume(this," & i &");")%>
			</TD>
		</TR>
		<%if cLng(solvent_volume) > 0 then%>
		<tr>
			<td align="right" width="200" nowrap>Current Solvent Volume (&#956;L):</td>
			<td>
				<input TYPE="text" CLASS="GrayedText" SIZE="10" Maxlength="50" NAME="dummySolventVolume" VALUE="<%=solvent_volume%>" READONLY>
			</td>
		</tr>
		<tr>
			<td align="right" width="200" nowrap>Current Concentration (M):</td>
			<td>
				<input TYPE="text" CLASS="GrayedText" SIZE="10" Maxlength="50" NAME="CurrConcentration<%=i%>" VALUE="<%=RS("molar_conc")%>" READONLY>
				<INPUT TYPE="hidden" NAME="CurrConcentrationUnitID<%=i%>" VALUE="15">
			</td>
		</tr>
		<%end if%>

    <%end if%>
		<%if numSourcePlates > 1 then%>
		<TR>
			<TD ALIGN="right" WIDTH="200">Apply to all:</TD>
			<TD><INPUT TYPE="checkbox" NAME="applyToAll<%=i%>" ONCLICK="applyToAll(<%=i%>, this);"></TD>
		</TR>
		<%end if%>
	</TABLE>
	</DIV>
	</TD></TR>
	<%
		RS.MoveNext
	next
	%>
	<tr>
		<%if pageMode = "reformat" then%>
			<td colspan="2" align="right"><a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;<a HREF="#" onclick="history.back();"><img SRC="../graphics/btn_back_61.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="Validate(<%=numSourcePlates%>); return false;"><input type="image" SRC="../graphics/btn_next_61.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
		<%else%>
			<td colspan="2" align="right"><a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="Validate(<%=numSourcePlates%>); return false;"><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
		<%end if%>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
