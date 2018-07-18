<%@ Language=VBScript %>
<%Response.Buffer =false %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>

<%
Dim Conn
Dim Cmd
Dim RS

multiSelect = Request("multiSelect")
'dataMode = Request("dataMode")
reformatAction = Request("reformatAction")
PlateID = Request("PlateID")
numSourcePlates = Request("numSourcePlates")
XMLDoc_ID = Request("XMLDoc_ID")
numDaughterPlates = Request("numDaughterPlates")
numTargetPlates = numDaughterPlates
'if dataMode = "targetConcentration" then AmtType = "TargetConcentration"
instructionText = "Enter target plate criteria."
stepCount = Request("stepCount")
stepValue = cint(Request("stepValue")) + 1

If reformatAction = "daughter" then
	'numSourcePlates = plate_multiSelect_dict.count
	numSourcePlates = numSourcePlates
	numTargetPlates = cLng(numDaughterPlates) 
	numTotalPlates = cLng(numDaughterPlates) * numSourcePlates
else
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.GETNUMTARGETPLATES", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 5, adParamReturnValue, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PREFORMATMAPID", 5, 1, 0, XMLDoc_ID)
	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
		Response.End	
	Else
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reformat.GETNUMTARGETPLATES")
	end if
	out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))
	'Response.Write out & "=out<BR>"
	'Response.End
	numTargetPlates = out
	numTotalPlates = out

	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.GETPLATEFORMATS", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 500, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PREFORMATMAPID", 5, 1, 0, XMLDoc_ID)
	Cmd.Parameters.Append Cmd.CreateParameter("PPLATETYPE", 200, 1, 200, "target")
	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
		Response.End	
	Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reformat.GETPLATEFORMATS")
	end if
	out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))
	'Response.Write out & "=out<BR>"
	'Response.End
	targetPlateFormats = out

end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Reformat Plates</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	function Validate(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
		!document.form1.Ownershiplst ? document.form1.PrincipalID.value="" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
		<% end if %>
		<%
		if reformatAction = "daughter" then
			response.write "numTargetPlates = 1;"
		else
			response.write "numTargetPlates = document.all.numTargetPlates.value;"
		end if
		%>
		var numTotalPlates = <%=numTotalPlates%>;
		//alert(document.form1.AutoGen_r.length);
		//AutoGen = document.form1.AutoGen_.checked;
		if (!document.form1.AutoGen_r[1].checked){
			var Barcodes = "";
			var	bPlateBarcodeErr = false;
			//manual barcode entry
			if (document.form1.AutoGen_r[0].checked){
				<%for j = 0 to numTotalPlates-1%>
					if(document.form1.Plate_Barcode<%=j%>.value.length == 0) {
						if(!bPlateBarcodeErr){errmsg = errmsg + "- A Plate Barcode is missing.\r";}
						bPlateBarcodeErr = true;
						bWriteError = true;
					}		
				<%next%>
				<%for j = 0 to numTotalPlates -1%>
					Barcodes = Barcodes + document.form1.Plate_Barcode<%=j%>.value + ","
				<%next%>
				Barcodes = Barcodes.substr(0,Barcodes.length-1);
			}
			//barcode start at
			if (document.form1.AutoGen_r[2].checked){
				if(document.form1.Barcode_Start.value.length == 0) {
					errmsg = errmsg + "- Barcode Start is required.\r";
					bPlateBarcodeErr = true
					bWriteError = true
				}		
				else if (!isPosLongInteger(document.form1.Barcode_Start.value)){
					errmsg = errmsg + "- Barcode Start must be a positive integer less than 2,147,483,648.\r";
					bPlateBarcodeErr = true
					bWriteError = true
				}
				else {
					prefix = document.form1.Barcode_Prefix.value;
					start = parseInt(document.form1.Barcode_Start.value);
					end = start + parseInt(numTotalPlates); 
					for (j=start;j<end;j++){
						Barcodes =  Barcodes + prefix + j + ",";
					}
					Barcodes = Barcodes.substr(0,Barcodes.length-1);
				}
			}				
			//check for duplicate barcodes
			if (!bPlateBarcodeErr){
				var strURL = serverType + serverName + "/cheminv/api/CheckDuplicateBarcode.asp?Barcodes=" + Barcodes + "&BarcodeType=plate";	
				var httpResponse = JsHTTPGet(strURL) 
				if (httpResponse.length > 0) {
					errmsg = errmsg + "- Barcode conflict for barcode(s): " + httpResponse + " .\r"
					bWriteError = true;
				}
			}
		}

		//location is required
		LocElement = document.all.LocationID;
		if(LocElement.value.length == 0) {
			errmsg = errmsg + "- Location is required.\r";
			bWriteError = true;
		}	
   		<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		     if(GetAuthorizedLocation(document.form1.LocationID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not athorized for this location.\r";
			    alert(errmsg);
			    return;
            }	
		<%end if%>
		AmtElement = document.all.SourceAmtTaken;
		if (AmtElement.value.length == 0) {
			errmsg = errmsg + "- You must enter a Source Amount Taken.\r";
			bWriteError = true;
		}
		else if (AmtElement.value.length > 0) {
			//Source Amount Taken must be a number
			if (!isNumber(AmtElement.value)){
				errmsg = errmsg + "- Source Amount Taken must be a number.\r";
				bWriteError = true;
			}
			else {
				//Source Amount Taken must be a positive number
				if (AmtElement.value <= 0){
					errmsg = errmsg + "- Source Amount Taken must be a positive number.\r";
					bWriteError = true;
				}
			}
		}
		
		// if solvent amount is entered then solvent is required
		if (document.form1.SolventVolume.value.length > 0) {
			if (document.form1.SolventID.value == "-1") {
				errmsg = errmsg + "- Solvent is required if a Solvent Volume is entered.\r";
				bWriteError = true;
			}
		}
		// if Plate Admin is required
		<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
        if (document.form1.PrincipalID.value.length==0){
		        errmsg = errmsg + "- Plate Admin is required.\r";
				bWriteError = true;
		}
        if (document.form1.LocationAdmin.value!='' && document.form1.LocationAdmin.value !=document.form1.PrincipalID.value){
            if (confirm("- The Location Admin and Plate Admin are not the same,\r Do you really want to continue?")!=true){     
                return;
            }
        }
		<% end if %>
        // LocationID must be valid for this plate type
		if (!(IsPlateTypeAllowed(document.form1.PlateID.value,document.form1.LocationID.value,document.form1.PlateTypeID.value,'') == 1)){
			errmsg = errmsg + "- This location does not accept this plate type.\r";
			bWriteError = true;
		}
		
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
			document.form1.submit();
		}
	}
	
	/*
	function ShowHide(divNumber, element){
		displayValue = "none";

		if (element.value == "yes") 
			displayValue = "block";
		eval("document.all.div" + divNumber + ".style.display = '" + displayValue + "'");
	}
	
	function UpdateSourceAmount(plateNum, element) {
		//alert(element.value == "");		
		if (element.value == "") {
			EnableInput('SourceAmtTaken' + plateNum);
			Show('SourceAmountSpan' + plateNum);
			document.all.AmtType.value = '';
			eval("document.all.SourceAmountCaptionSpan" + plateNum + ".className = 'required'");
		}
		else {
			DisableInput('SourceAmtTaken' + plateNum); 
			Hide('SourceAmountSpan' + plateNum);	
			document.all.AmtType.value = 'SourceVolumeTaken';
			eval("document.all.SourceAmountCaptionSpan" + plateNum + ".className = ''");
			//alert(document.all.AmtType.value);
		}
	}
	function UpdateVolumeTaken(plateNum, element) {
		//alert(element.value == "");		
		if (element.value == "") {
			EnableInput('SourceVolTaken' + plateNum);
			Show('VolumeTakenSpan' + plateNum);
			document.all.AmtType.value = '';
			eval("document.all.VolumeTakenCaptionSpan" + plateNum + ".className = 'required'");
		}
		else {
			DisableInput('SourceVolTaken' + plateNum); 
			Hide('VolumeTakenSpan' + plateNum);	
			document.all.AmtType.value = 'SourceAmountTaken';
			eval("document.all.VolumeTakenCaptionSpan" + plateNum + ".className = ''");
			//alert(eval("document.all.VolumeTakenCaptionSpan" + plateNum + ".className"));
			//alert(document.all.AmtType.value);
		}
	}


	function DisableInput(elementName) {
		eval("document.all." + elementName + ".readOnly = true");
		eval("document.all." + elementName + ".className = 'GrayedText'");
	}
	
	function Hide(element){
		displayValue = "none";
		eval("document.all." + element + ".style.display = '" + displayValue + "'");
	}	
	
	function EnableInput(elementName) {
		eval("document.all." + elementName + ".readOnly = false");
		eval("document.all." + elementName + ".className = ''");
	}
	
	function Show(element){
		displayValue = "";
		eval("document.all." + element + ".style.display = '" + displayValue + "'");
	}
	*/	
-->
</script>

</head>
<body>
<center>
<form name="form1" action="ReformatPlate_TargetCriteria_action.asp" method="POST">
<INPUT TYPE="hidden" NAME="multiselect" VALUE="<%=multiselect%>">
<input Type="hidden" name="PlateID" value="<%=PlateID%>">
<INPUT TYPE="hidden" NAME="numSourcePlates" VALUE="<%=numSourcePlates%>">
<INPUT TYPE="hidden" NAME="numTargetPlates" VALUE="<%=numTargetPlates%>">
<INPUT TYPE="hidden" NAME="numTotalPlates" VALUE="<%=numTotalPlates%>">
<INPUT TYPE="hidden" NAME="reformatAction" VALUE="<%=reformatAction%>">
<INPUT TYPE="hidden" NAME="XMLDoc_ID" VALUE="<%=xmldoc_id%>">
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<input TYPE="hidden" NAME="OwnerShipGroupList" Value="<%=GetOwnerShipGroupList()%>">
<input TYPE="hidden" NAME="OwnerShipUserList" Value="<%=GetOwnerShipUserList()%>">
<input TYPE="hidden" NAME="PrincipalID" Value>
<input TYPE="hidden" NAME="LocationAdmin" Value>
<% end if %>
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input type="hidden" name="tempCsUserID" id="tempCsUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>" />
<!--<INPUT TYPE="hidden" NAME="AmtType" VALUE="<%=AmtType%>">-->

<table border="0" cellspacing="0" cellpadding="1">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback"><%=instructionText%></span><br><br>
			<table bgcolor="#e1e1e1" width="100%">
				<tr><td align="center">Step <strong><%=stepValue%></strong> of <%=stepCount%></td></tr>
			</table>	
		</td>
	</tr>

		<tr><td colspan="2">
		<%
		if multiselect = "true" then
			arrSourcePlateIDs = plate_multiSelect_dict.Keys
			currSourcePlateBarcode = Session("plateBarcode_multiSelect_dict").Item(arrSourcePlateIDs(0))
		else
			currSourcePlateBarcode = Session("plBarcode")				
		end if
		numCurrDaughterPlate = 1
		numSourcePlate = 0
		
		for i = 0 to numTotalPlates-2
		%>
			<div id="div_<%=i%>" style="display:none;">
			<table cellspacing="0" cellpadding="1">
			<tr height="25">
				<td align="right" valign="top" nowrap width="150"><span id="sp0_<%=i%>" style="display:block;"><span class="required">Plate Barcode:</span></span></td>
				<td>
					<span id="sp2_<%=i%>" style="display:block;">
						&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="Plate_Barcode<%=i%>" size="15" value=""><%="(" & currSourcePlateBarcode & "  Daughter" & numCurrDaughterPlate & ")"%><br>
					</span>
				</td>		</tr>
			</table>
			</div>
		<%
			numCurrDaughterPlate = numCurrDaughterPlate + 1
			if cLng(numCurrDaughterPlate) > clng(numTargetPlates) then
				numCurrDaughterPlate = 1
				numSourcePlate = numSourcePlate + 1
				currSourcePlateBarcode = Session("plateBarcode_multiSelect_dict").Item(arrSourcePlateIDs(numSourcePlate))
			end if
		next
		%>
		<tr height="25">
			<td align="right" valign="top" nowrap width="150">
				<span id="sp0_<%=i%>" style="display:none;"><span class="required">Plate Barcode:</span></span>
				<span id="sp1_<%=i%>" style="display:block;"><span class="required">Barcode Description:</span></span>
				<span id="sp4_<%=i%>" style="display:none">Prefix:<BR><img SRC="../graphics/pixel.gif" border="0" WIDTH="1" HEIGHT="25"><span class="required">Sequence Start:</span></span>
			</td>
			<td>
				<input type="hidden" name="bUseBarcodeDesc_0" value="">
				<span id="sp2_<%=i%>" style="display:none;">
					<%if reformatAction = "daughter" then%>
					&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="Plate_Barcode<%=i%>" size="15" value=""><%="(" & currSourcePlateBarcode & "  Daughter" & numCurrDaughterPlate & ")"%><br>
					<%else%>
					&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="Plate_Barcode<%=i%>" size="15" value=""><%="(" & "Target Plate" & numCurrDaughterPlate & ")"%><br>
					<%end if%>
				</span>
				<span id="sp3_<%=i%>" style="display:block">
					<%=ShowSelectBox("BarcodeDescID0", Barcode_desc_ID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC")%>
				</span>
				<span id="sp5_<%=i%>" style="display:none">
					<input type="text" size="3" maxlength="3" name="Barcode_Prefix"><BR><input type="text" size="10" name="Barcode_Start">				
				</span>
				<script language="JavaScript">
				function AutoGen_OnClick(element) {
					if (element.value == "0") {
						<%for j = 0 to numTotalPlates -2%>
						document.form1.Plate_Barcode<%=j%>.value=''; 
						element.checked ? document.all.div_<%=j%>.style.display = 'block' :document.all.div_<%=j%>.style.display = 'none';
						<%next%>
						element.checked ? document.all.sp0_<%=i%>.style.display = 'block' :document.all.sp0_<%=i%>.style.display = 'none';
						document.all.sp2_<%=i%>.style.display = document.all.sp0_<%=i%>.style.display;
						element.checked ? document.all.sp1_<%=i%>.style.display = 'none':document.all.sp1_<%=i%>.style.display = 'block';
						document.all.sp3_<%=i%>.style.display = document.all.sp1_<%=i%>.style.display;
						element.checked ? document.all.sp4_<%=i%>.style.display = 'none':document.all.sp4_<%=i%>.style.display = 'block';
						document.all.sp5_<%=i%>.style.display = document.all.sp4_<%=i%>.style.display;
						//update the bUseBarcodeDesc value
						element.checked ? document.all.bUseBarcodeDesc_0.value = 'true' : document.all.bUseBarcodeDesc_0.value = 'false';
					}
					else if (element.value == "1") {
						<%for j = 0 to numTotalPlates -2%>
						document.form1.Plate_Barcode<%=j%>.value=''; 
						element.checked ? document.all.div_<%=j%>.style.display = 'none' :document.all.div_<%=j%>.style.display = 'block';
						<%next%>
						element.checked ? document.all.sp0_<%=i%>.style.display = 'none' :document.all.sp0_<%=i%>.style.display = 'block';
						document.all.sp2_<%=i%>.style.display = document.all.sp0_<%=i%>.style.display;
						element.checked ? document.all.sp1_<%=i%>.style.display = 'block':document.all.sp1_<%=i%>.style.display = 'none';
						document.all.sp3_<%=i%>.style.display = document.all.sp1_<%=i%>.style.display;
						element.checked ? document.all.sp4_<%=i%>.style.display = 'none':document.all.sp4_<%=i%>.style.display = 'block';
						document.all.sp5_<%=i%>.style.display = document.all.sp4_<%=i%>.style.display;
						//update the bUseBarcodeDesc value
						element.checked ? document.all.bUseBarcodeDesc_0.value = 'true' : document.all.bUseBarcodeDesc_0.value = 'false';
					}
					else{
						<%for j = 0 to numTotalPlates -2%>
						document.form1.Plate_Barcode<%=j%>.value=''; 
						element.checked ? document.all.div_<%=j%>.style.display = 'none' :document.all.div_<%=j%>.style.display = 'block';
						<%next%>
						element.checked ? document.all.sp0_<%=i%>.style.display = 'none' :document.all.sp0_<%=i%>.style.display = 'block';
						document.all.sp2_<%=i%>.style.display = 'none';
						element.checked ? document.all.sp1_<%=i%>.style.display = 'none':document.all.sp1_<%=i%>.style.display = 'block';
						document.all.sp3_<%=i%>.style.display = document.all.sp2_<%=i%>.style.display;
						element.checked ? document.all.sp4_<%=i%>.style.display = 'block':document.all.sp4_<%=i%>.style.display = 'none';
						document.all.sp5_<%=i%>.style.display = document.all.sp4_<%=i%>.style.display;
						//update the bUseBarcodeDesc value
						element.checked ? document.all.bUseBarcodeDesc_0.value = 'false' : document.all.bUseBarcodeDesc_0.value = 'true';
					}
					
				}
				</script>
				<input type="radio" name="AutoGen_r" onclick="AutoGen_OnClick(this);" value="0" >Assign barcodes manually<BR>
				<input type="radio" name="AutoGen_r" onclick="AutoGen_OnClick(this);" value="1" CHECKED>Auto-generate barcode from barcode description<BR>
				<input type="radio" name="AutoGen_r" onclick="AutoGen_OnClick(this);" value="2">Auto-generate barcode from custom sequence

			</td>
		</tr>
		</td></tr>

	<tr>
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Location ID:</span>
		</td>
		<td>
            <%  if Application("ENABLE_OWNERSHIP")="TRUE" then
                authorityFunction = "SetOwnerInfo('location');"
            else
                authorityFunction= ""
            end if %>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker9 "document.form1", "LocationID", "lpLocationBarCode" & num, "lpLocationName" & num, 10, 28, false, Session("CurrentLocationID"), authorityFunction%> 
		</td>
	</tr>	
	<tr>
		<%=ShowPickList("<span class=""required"">Select Plate Type:</span>", "PlateTypeID", "", "SELECT plate_type_id AS Value, plate_type_name AS DisplayText FROM inv_plate_types ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
		<td align="right" width="150" valign=top nowrap>
			<span class="required">Source Amount Taken:</span>
		</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="SourceAmtTaken" VALUE="">
			<%=ShowSelectBox3("SourceAmtTakenUnitID", Application("plDefVolUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1,2) ORDER BY lower(DisplayText) ASC",null,null,null,"")%>
		</td>
	</tr>
	<TR>
		<TD COLSPAN="2" ALIGN="center"><BR>Solvent Added to Target Plates</TD>
	</TR>
	<tr>
		<td align=right  width="150" valign=top nowrap>Select Solvent:</td>
		<td><%=ShowSelectBox2("SolventID",null,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "-1")%>
	</tr>
	<tr>
		<td align="right" width="150" nowrap>Solvent Volume:</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="SolventVolume" VALUE="">
			<%=ShowSelectBox3("SolventVolumeUnitID", Application("plDefVolUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC",null,null,null,"//CalculateConcentration(" & num &", this);")%>
		</td>
	</tr>
	<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
	<tr height="25">
		<td align=right> <span title="Pick an option from the list" class="required">Plate Admin:</span></td>
		<td align=left>
		<table border=.5>
		<tr><td><input type="Radio" name="Ownership" name="User_cb" id="User_cb" onclick="SetOwnerInfo('user');"/>by user</td>
		<td><input type="Radio" name="Ownership" name="Group_cb" id="Group_cb" onclick="SetOwnerInfo('group');" />by Group</td></tr>
		<tr><td colspan="2"><SELECT id="Ownershiplst" ><OPTION></OPTION></SELECT></td></tr></table></td>
	</tr>
	<% end if %>
	<%

'	arrPlateIDs = split(PlateID)
'If reformatAction = "daughter" then
'	call CreateForm(0)
'else
'	call CreateForm(0)
	'Call GetInvConnection()
	'username = Application("CHEMINV_USERNAME")
	'SQL = "SELECT plate_id, well_capacity, unit_abreviation "
	'SQL = SQL & "FROM inv_plates, inv_units, inv_plate_format "
	'SQL = SQL & "WHERE phys_plate_id_fk = plate_id"
	'SQL = SQL & " AND well_capacity_unit_id_fk = unit_id"
	'SQL = SQL & " AND plate_format_id IN (?)"
	'Set Cmd = GetCommand(Conn, SQL, adCmdText)
	'Cmd.Parameters.Append Cmd.CreateParameter("PlateIDs", 200, 1, 2000, targetPlateFormats)
	'Set RS = Server.CreateObject("ADODB.recordset")
	'RS.Open SQL, Conn, adOpenKeyset, adLockOptimistic, adCmdText 
	'Set RS = Cmd.Execute


		'for i=0 to numTargetPlates-1
			'unit_abreviation = ""
			'well_capacity = ""
			'if not RS.EOF then
			'	if RS("plate_id") = arrPlateIDs(i) then
			'		unit_abreviation = " (" & RS("unit_abreviation") & ")"
			'		well_capacity = RS("well_capacity")
			'	end if
			'end if
			'call CreateForm(i)
			'if not RS.EOF then
			'	RS.MoveNext
			'end if
		'next
	
'	end if
	%>
	<tr>
		<td colspan="2" align="center">Dry source plates after reformat:&nbsp;<input type="checkbox" name="DrySourcePlates" Value="true"></td>
	</tr>
	<%if reformatAction="daughter" then%>
	<tr>
		<td colspan="2" align="right"><a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;<a HREF="#" onclick="history.go(-2);"><img SRC="../graphics/btn_back_61.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="Validate(); return false;"><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
	</tr>	
	<%else%>
	<tr>
		<td colspan="2" align="right"><a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;<a HREF="#" onclick="history.go(-2);"><img SRC="../graphics/btn_back_61.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="Validate(); return false;"><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
	</tr>	
	<%end if%>

</table>	
<% if Application("ENABLE_OWNERSHIP")="TRUE" and PrincipalID="" then %>
<script language="javascript">
    //set the inital location group
    SetOwnerInfo('location');
 </script>
<%end if %>
</form>
</center>
</body>
</html>


<%
sub CreateForm(num)
%>
	<tr>
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Location ID:</span>
		</td>
		<td>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker2 "document.form1", "LocationID" & num, "lpLocationBarCode" & num, "lpLocationName" & num, 10, 28, false, Session("CurrentLocationID")%> 
		</td>
	</tr>	
	<tr>
		<%=ShowPickList("<span class=""required"">Select Plate Type:</span>", "PlateTypeID" & num, "", "SELECT plate_type_id AS Value, plate_type_name AS DisplayText FROM inv_plate_types ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
		<td align="right" width="150" valign=top nowrap>
			<span class="required">Source Amount Taken:</span>
		</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="SourceAmtTaken<%=num%>" VALUE="">
			<%=ShowSelectBox3("SourceAmtTakenUnitID" & num, Application("plDefVolUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1,2) ORDER BY lower(DisplayText) ASC",null,null,null,"")%>
		</td>
	</tr>
	<TR>
		<TD COLSPAN="2" ALIGN="center"><BR>Solvent Added to Target Plates</TD>
	</TR>
	<tr>
		<td align=right  width="150" valign=top nowrap>Select Solvent:</td>
		<td><%=ShowSelectBox2("SolventID" & num,null,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "-1")%>
	</tr>
	<tr>
		<td align="right" width="150" nowrap>Solvent Volume:</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="SolventVolume<%=i%>" VALUE="">
			<%=ShowSelectBox3("SolventVolumeUnitID" & num, Application("plDefVolUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC",null,null,null,"//CalculateConcentration(" & num &", this);")%>
		</td>
	</tr>
<%
end sub
%>
<!--	
	<tr>
		<td align="right" width="150" valign=top nowrap>
			<span class="required" ID="VolumeTakenCaptionSpan<%=num%>">Source Volume Taken:</span>
		</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="SourceVolTaken<%=num%>" VALUE="" ONCHANGE="UpdateSourceAmount(<%=num%>,this);">
			<SPAN ID="VolumeTakenSpan<%=num%>"><%=ShowSelectBox3("SourceVolTakenUnitID" & num, Application("plDefVolUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC",null,null,null,"")%><SPAN>
		</td>
	</tr>
	<TR>
		<TD COLSPAN="2" ALIGN="center">OR</TD>
	</TR>
	<tr>
		<td align="right" width="150" nowrap>
			<span class="required" ID="SourceAmountCaptionSpan<%=num%>">Source Amount:</span>
		</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="SourceAmtTaken<%=num%>" VALUE="" ONCHANGE="UpdateVolumeTaken(<%=num%>,this);">
			<SPAN ID="SourceAmountSpan<%=num%>"><%=ShowSelectBox3("SourceAmountUnitID" & num, Application("plDefMassUnitID"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (2) ORDER BY lower(DisplayText) ASC",null,null,null,"")%></SPAN>
		</td>
	</tr>
-->
