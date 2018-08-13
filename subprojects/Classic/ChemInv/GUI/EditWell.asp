<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
Session("bManageMode") = false

Dim Conn
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cartridge_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetWellAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%
'-- Get list of all existing Compound ID's for validation
'-- -----------------------------------------------------
'sql_compoundidlist = "Select distinct COMPOUND_ID from INV_COMPOUNDS"
'Call GetInvConnection()
'Set RS_compoundidlist = Conn.Execute(sql_compoundidlist)
'compoundidlist = ""
'While NOT RS_compoundidlist.EOF
'	compoundidlist = compoundidlist & "[" & RS_compoundidlist("COMPOUND_ID") & "],"
'RS_compoundidlist.MoveNext
'Wend
'RS_compoundidlist.Close()
'Set RS_compoundidlist = Nothing
if Request.QueryString("newCompoundId")<>"" then Compound_ID_FK= Request.QueryString("newCompoundId")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Edit a Plate Well</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->

<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;

	// Validates well attributes
	function ValidateWell(strMode){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";

<% if NOT isEdit then%>
		// Initial amount is required
		if (document.form1.iQty_Initial.value.length == 0) {
			errmsg = errmsg + "- Initial amount is required.\r";
			bWriteError = true;
		}
		else{
		
			// Initial amount must be a number
			if (!isPositiveNumber(document.form1.iQty_Initial.value)){
				errmsg = errmsg + "- Initial amount must be a positive number.\r";
				bWriteError = true;
			}
			if (document.form1.iQty_Initial.value > 999999999){
				errmsg = errmsg + "- Initial amount is too large.\r";
				bWriteError = true;
			}
		}
<%else%>
		// QtyRemaining is required
		//if (document.form1.iQty_Remaining.value.length == 0) {
		//	errmsg = errmsg + "- Quantity Remaining is required.\r";
		//	bWriteError = true;
		//}
		//else{
			// QtyRemaining must be a number
			if (document.form1.iQty_Remaining.value.length > 0 && !isWholeNumber(document.form1.iQty_Remaining.value)){
			errmsg = errmsg + "- Quantity Remaining must be zero or greater.\r";
			bWriteError = true;
			}
		//}
<% end if%>

		// Weight if present must be a number
		if (document.form1.iWeight.value.length >0 && !isWholeNumber(document.form1.iWeight.value)){
			errmsg = errmsg + "- Weight must be zero or greater.\r";
			bWriteError = true;
		}
		// Concentration if present must be a number
		if (document.form1.iConcentration.value.length >0 && !isWholeNumber(document.form1.iConcentration.value)){
			errmsg = errmsg + "- Concentration must be zero or greater.\r";
			bWriteError = true;
		}
		// Solvent Volume if present must be a number
		if (document.form1.iSolvent_Volume.value.length >0 && !isWholeNumber(document.form1.iSolvent_Volume.value)){
			errmsg = errmsg + "- Solvent Volume must be zero or greater.\r";
			bWriteError = true;
		}
		// Solution Volume if present must be a number
		if (document.form1.iSolution_Volume.value.length >0 && !isWholeNumber(document.form1.iSolution_Volume.value)){
			errmsg = errmsg + "- Solution Volume must be zero or greater.\r";
			bWriteError = true;
		}
		// Molar Amount if present must be a number
		//if (document.form1.iMolar_Amount.value.length >0 && !isPositiveNumber(document.form1.iMolar_Amount.value)){
		//	errmsg = errmsg + "- Molar Amount must be a positive number.\r";
		//	bWriteError = true;
		//}

		//Custom field validation
		<%For each Key in custom_well_fields_dict
			response.write "//" & key
			If InStr(lcase(Key), "date_") then%>
				if (document.form1.i<%=Key%>.value.length > 0 && !isDate(document.form1.i<%=Key%>.value)){
					errmsg = errmsg + "- <%=custom_well_fields_dict.Item(Key)%> must be in " + dateFormatString + " format.\r";
					bWriteError = true;
				}
			<%end if%>
		<%next%>

		//Validate required custom fields
		<% For each Key in req_custom_well_fields_dict%>
			if (document.form1.i<%=Key%>.value.length == 0) {
				errmsg = errmsg + "- <%=req_custom_well_fields_dict.Item(Key)%> is required.\r";
				bWriteError = true;
			}
		<%Next%>

        <%if cint(Session("wCompoundCount")) <= 1 then %>
		// if compoundID is present must be a positive number and a valid compoundID
		if (document.form1.RegBatchID.value.length == 0 && document.form1.iCompoundID.value.length > 0) {
			if (!isPositiveNumber(document.form1.iCompoundID.value) || document.form1.iCompoundID.value < 1){
				errmsg = errmsg + "- Compound ID must be a positive number.";
				bWriteError = true;
			}
			else {
				if (IsValidCompoundID(document.form1.iCompoundID.value, false)!=1) {
					errmsg = errmsg + "- The Compound ID you have selected is not valid.\r";
					bWriteError = true;
				}
			}
		}
        <%end if %>
		//Validate CompoundID from variable, compoundidlist
		//varCompoundIDList = "<%=compoundidlist%>";
		//varCompoundIDPos = varCompoundIDList.indexOf('[' + document.form1.iCompoundID.value + ']');
		//if (varCompoundIDPos == '-1' && !document.form1.iCompoundID.value.length == 0){
		//		errmsg = errmsg + "- The Compound ID you have selected is not valid.\r";
		//		bWriteError = true;
		//}

		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
		    
			document.form1.action = "EditWell_action.asp";
			document.form1.submit();
		}
	}

	// Post data between tabs
	function postDataFunction(sTab) {
		document.form1.action = "EditWell.asp?GetData=form&sTab=" + sTab + "&WellID=<%=WellID%>&isEdit=<%=isEdit%>"
		//alert(document.form1.action);
		document.form1.submit()
	}
//-->
</script>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<SCRIPT LANGUAGE="javascript" src="<%=Application("CDJSUrl")%>"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/gui/CalculateFromPlugin.js"></script>
<!--#INCLUDE FILE="../source/app_js.js"-->
</head>
<body>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/EditWellTabs.asp"-->
<form name="form1" method="POST">
<br/>
<table border="0" cellspacing="0" cellpadding="0" width="800px">
<INPUT TYPE="hidden" NAME="iWellIDs" VALUE="<%=Well_ID%>">
<INPUT TYPE="hidden" NAME="iCompound_ID_FK" VALUE="<%=Compound_ID_FK%>">
<INPUT TYPE="hidden" NAME="NewCompoundID">
<% if sTab<>"RegSubstance" then %>
<INPUT TYPE="hidden" NAME="iRegID" value="<%=RegID%>">
<INPUT TYPE="hidden" NAME="iBatchNumber" value="<%=batchnumber%>">
<%end if%>
<input TYPE="hidden" NAME="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" NAME="tempCsUserID" Value=<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp"))%>>
<!-- here b/c the utils.js doselectsubstance function looks for it -->
<INPUT TYPE="hidden" NAME="iContainerName">
<%
Select Case sTab

	Case "Required"
%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Well ID:</span>
		</td>
		<td>
			<input type="text" name="iWell_ID" size="15" value="<%=Well_ID%>" CLASS="GrayedText" READONLY>
		</td>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%=ShowPickList("<span class=""required"">Well Format:</span>", "iWell_Format_ID_FK", Well_Format_ID_FK, "SELECT enum_ID AS Value, enum_value AS DisplayText FROM inv_enumeration WHERE eset_id_fk = 1 ORDER BY lower(DisplayText) ASC")%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Plate ID:</span>
		</td>
		<td>
			<input type="text" name="iPlate_ID_FK" size="15" value="<%=Plate_ID_FK%>" CLASS="GrayedText" READONLY>
		</td>
		<td></td>
		<td></td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Plate format:</span>
		</td>
		<td>
			<input type="text" name="iPlate_Format_Name" size="15" value="<%=Plate_Format_Name%>" CLASS="GrayedText" READONLY>
		</td>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Grid Position:</span>
		</td>
		<td>
			<input type="text" name="iGrid_Position_Name" size="15" value="<%=Grid_Position_Name%>" CLASS="GrayedText" READONLY>
		</td>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Quantity remaining:", "Qty_Remaining", 150, 15, ShowSelectBox("iQty_Unit_FK", Session("wQty_Unit_FK"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1,2) ORDER BY lower(DisplayText) ASC"), False, False)%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Weight:", "Weight", 150, 15, ShowSelectBox("iWeight_Unit_FK", Session("wWeight_Unit_FK"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (2) ORDER BY lower(DisplayText) ASC"), False, False)%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<td CLASS="required" VALIGN="top" align=right width="150" nowrap>Select Solvent:</td>
		<td>
		<%Response.Write(ShowSelectBox2("iSolvent_ID_FK",Session("wSOLVENT_ID_FK"),"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", ""))%>
		</TD>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Solvent Volume:", "Solvent_Volume", 150, 15, "", False, False)%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Solution Volume:", "Solution_Volume", 150, 15, "", False, False)%>
		<td colspan="2" align="center">&nbsp;
			<%if cint(Session("wCompoundCount")) <= 1 then %>
				<%if Session("INV_MANAGE_SUBSTANCES" & "cheminv") then%>
				<a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'SubsManager', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
				&nbsp;|&nbsp;
				<%end if%>
				<a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'SubsManager', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>
			<%else%>
				<span class="GUIFeedback">Mixture well compounds cannot be edited.</span>
			<%end if%>
		</td>
	</tr>
	<tr height="25">
		<td align="right" VALIGN="top" nowrap>Solvent/Solution Unit:</td>
		<td><%=ShowSelectBox("iSolvent_Volume_Unit_ID_FK", Session("wSolvent_Volume_Unit_ID_FK"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC")%></td>
		<%if cint(Session("wCompoundCount")) <= 1 then %>
			<td width="200" align="right">Compound ID:</td>
			<td>
				<%if Application("RegServerName") <> "NULL" then%>
					<input type="text" Size="15" name="iCompoundID" value="<%=Compound_ID_FK%>" onchange="if((this.value)!=''){if(IsValidCompoundID(this.value, true)==1){NewCompoundID.value= this.value; RegBatchID.value=''; iRegID.value=''; iBatchNumber.value='';iCompound_ID_FK.value=this.value;NewRegID.value='';}else{this.value = '';}}else{iCompound_ID_FK.value='';}">
				<%else%>
					<input type="text" Size="15" name="iCompoundID" value="<%=Compound_ID_FK%>" onchange="iCompound_ID_FK.value=this.value;NewCompoundID.value= this.value;">
				<%End if%>
			</td>
		<%else%>
			<td></td>
			<td></td>
		<%end if%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Concentration:", "Concentration", 150, 15, ShowSelectBox("iConc_Unit_FK", Session("wConc_Unit_FK"),"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (3) ORDER BY lower(DisplayText) ASC"), False, False)%>
		<%if cint(Session("wCompoundCount")) <= 1 then %>
			<%if Application("RegServerName") <> "NULL" then%>
			<td width="200" align="right">Registry Batch ID:</td>
			<td>
				<input type="text" Size="15" name="RegBatchID" value="<%=Session("wRegBatchID")%>" onchange="if((this.value)!=''){GetRegIDFromRegNum(this.value); iCompoundID.value='';}else{iReg_ID_FK.value=''; iBatch_Number_FK.value='';NewRegID.value='';}">
				<input type="hidden" name="iReg_ID_FK" value="<%=Reg_ID_FK%>">
				<input type="hidden" name="iBatch_Number_FK" value="<%=Batch_Number_FK%>">
				<input type="hidden" name="NewRegID" value="<%=Reg_ID_FK%>">
				<input type="hidden" name="NewBatchNumber" value="<%=Batch_Number_FK%>">
				<!--need these fields for the substance select-->
				<input type="hidden" name="RegID" value="<%=RegID%>">
				<input type="hidden" name="BatchNumber" value="<%=BatchNumber%>">
			</td>
			<%
		    else
		    '--even without reg integration these form elements are expected for substance selection
            %>
	        <input type="hidden" name="RegBatchID" value="<%=Session("wREGBATCHID")%>" />
	        <input type="hidden" name="RegID" value="<%=RegID%>"/>
	        <input type="hidden" name="BatchNumber" value="<%=BatchNumber%>"/>
			<%end if%>
		<%else%>
		<td></td>
		<td></td>
		<%End if%>
	</tr>
	<%
	For each key in custom_well_fields_dict
		Response.Write "<TR height=""25"">" & vblf
		if inStr(uCase(Key), "DATE_") then
			Response.Write "<td align=""right"" valign=""top"" nowrap width=""150"">"
			if req_custom_well_fields_dict.Exists(Key) then Response.Write "<span class=""required"">"
			Response.Write custom_well_fields_dict.Item(key)
			if req_custom_well_fields_dict.Exists(Key) then Response.Write "</span>"
			Response.Write ":</td>"
			Response.Write "<td>"
			call ShowInputField("", "", "i" & Key & ":form1:" & eval(Key) , "DATE_PICKER:TEXT", "15")
			Response.Write "</td>"

			'str = ShowInputBox2(custom_plate_fields_dict.Item(key) & ":", Key, 250, 25, "", False, req_custom_plate_fields_dict.Exists(Key))
			'Response.write Left(str,len(str)-5)
			'Response.Write "<a href onclick=""return PopUpDate(&quot;i" & Key & "&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)""><img src=""/cfserverasp/source/graphics/navbuttons/icon_mview.gif"" height=""16"" width=""16"" border=""0""></a>"
			'Response.Write "</td>" & vblf
		Else
			Response.write ShowInputBox2(custom_well_fields_dict.Item(key) & ":", Key, 150, 25, "", False, req_custom_well_fields_dict.Exists(Key)) & vblf
		end if
		Response.Write "</TR>" & vblf
	Next

	%>

	<tr height="25">
		<td align="right" colspan="4">
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
				<%if NOT isEdit then%>
					<a HREF="#" onclick="ValidateWell('Create'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				<%Else%>
					<a HREF="#" onclick="ValidateWell('Edit'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				<%End if%>
		</td>
	</tr>
<%
	Case "Substance"

	'for each item in Request.Form
		'Response.Write item & " = " & Request.Form(item) & "<BR>"
	'next
%>
	<input type="hidden" name="isSubstanceTab">
	<tr>
		<td Colspan="2">
<%
	if NOT IsEmpty(Compound_ID_FK) then
		clearWellRegAttributes()
		GetSubstanceAttributesFromDb(Compound_ID_FK)
		hdrText = ""
		bConflicts = false
		if ConflictingFields <> "" then
			hdrText = "<font color=red>Warning: Duplicate Substance</font>"
			bConflicts = true
		End if
		DisplaySubstance "", hdrText, false, false, false, false, false, inLineMarker & dBStructure
	End if
%>


		</td>
	</tr>
	<tr>
		<td colspan="2" align="right">
		<%if cint(Session("wCompoundCount")) <= 1 then %>
			<%if Session("INV_MANAGE_SUBSTANCES" & "cheminv") then%>
			<a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'newDialog', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
			&nbsp;|&nbsp;
			<%end if%>
			<a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'newDialog', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>
		<%else%>
			<span class="GUIFeedback">Mixture well compounds cannot be edited.</span>
		<%end if%>
		</td>
	</tr>
	<tr height="25">
		<td align="right" colspan="2">
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
				<%'if NOT isEdit then%>
					<!--<a HREF="#" onclick="ValidateWell('Create'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>-->
				<%'Else%>
					<a HREF="#" onclick="document.form1.action = 'EditWell_action.asp';	document.form1.submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				<%'End if%>
		</td>
	</tr>

<%
Case "RegSubstance"
	Session("IsPopUP") = false
%>

<% if Reg_ID_FK<>"" then  %> 
<input type="hidden" name="NewRegID" value="<%=Reg_ID_FK%>">
<input type="hidden" name="NewBatchNumber" value="<%=Batch_Number_FK%>">
<% end if  %>
	<input type="hidden" name="isSubstanceTab">
	<tr height="150">
		<td>
			<table border="1">
				<tr>
					<!-- Structure -->
					<td>
						<%
						if Session("isCDP") = "TRUE" and detectModernBrowser = false then
							specifier = 185
						else
							specifier = "185:gif"
						end if
							Base64DecodeDirect "invreg", "base_form_group", Session("wBASE64_CDX"), "Structures.BASE64_CDX",  Session("wregid"),  Session("wregid"), specifier, 130
						%>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top">
			<table border="0" cellpadding="1" cellspacing="2">
				<tr><!-- Header Row -->
					<td colspan="4" align="center">
						&nbsp;<em><b><%=TruncateInSpan(Session("wRegName"), 50, "")%></b></em>
					</td>
				</tr>
				<tr>
					<%=ShowField("Molecular Weight:", "MOLWEIGHT0", 15, "MOLWEIGHT0")%>
					<%=ShowField("Molecular Formula:", "FORMULA0", 15, "FORMULA0")%>
					<%Response.Write "<script languaje='javascript'> GetMolWeightAndFormula('MOLWEIGHT0', 'FORMULA0', '" & Round(ConvertBase64toMW(Mid(Session("wBASE64_CDX"), InStr(Session("wBASE64_CDX"), "VmpD"))),3) &"','" & ConvertBase64toMFormula(Mid(Session("wBASE64_CDX"), InStr(Session("wBASE64_CDX"), "VmpD"))) &"'); </script>" %>
				</tr>
				<!--
				<tr>
					<%=ShowField("RegBatchID:", "wRegBatchID", 15, "")%>
					<input type="hidden" name="iRegID" value="<%=RegID%>">
					<input type="hidden" name="iBatchNumber" value="<%=Session("wBatchNumber")%>">
					<%=ShowField("RegBatchAmount:", "RegAmount", 15, "")%>
				</tr>
				<tr>
					<%=ShowField("NoteBook:", "NoteBook", 15, "")%>
					<%=ShowField("Page:", "Page", 15, "")%>
				</tr>
				<tr>
					<%=ShowField("Chemist:", "RegScientist", 15, "")%>
					<%=ShowField("Purity:", "Purity", 15, "")%>
				</tr>
				-->
				<%
				k = 0
				for each key in reg_fields_dict
					if key <> "BASE64_CDX" and key <> "REGNAME" then
						'Response.Write("@@" & key & " : " & Session(key) & "@@")
						if k = 0 then
							Response.Write("<tr>" & vbcrlf)
						end if
						Response.Write(ShowField(reg_fields_dict.item(key) & ":",  key, 15, "") & vbcrlf)
						if key = "REGID" then
							Response.Write("<input type=""hidden"" name=""iRegID"" value=""" & RegID & """>" & vbcrlf)
						end if
						if key = "BATCHNUMBER" then
							Response.Write("<input type=""hidden"" name=""iBatchNumber"" value=""" & Session("wBatchNumber") & """>" & vbcrlf)
						end if
						if k = cInt(1) then
							Response.write("</tr>")
							k = 0
						else
							k = k + 1
						end if
					end if
				next
				%>
				
				<tr>
					<td colspan="4" align="right">
						<%if Session("INV_MANAGE_SUBSTANCES" & "cheminv") then%>
						<a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'newDialog', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
						&nbsp;|&nbsp;
						<%end if%>
						<a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'newDialog', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>
					</td>
				</tr>
			</table>
		</td>
	</tr>
<%
end select
%>
</table>
</form>
<%If Request("showconflicts") then%>
<script LANGUAGE="javascript">
	OpenDialog('CreateSubstance2.asp?action=showconflicts&CompoundID=<%=CompoundID%>', 'SubsManager', 2);
</script>
<%end if%>
<%If Request("editsubstance") then%>
<script LANGUAGE="javascript">
	OpenDialog('CreateOrEditSubstance.asp?action=edit&CompoundID=<%=CompoundID%>', 'SubsManager', 2);
</script>
<%end if%>

<%
If err.number <> 0 then
	Response.write "<!--<br>" & err.number & err.description & "<br>-->"
End if
%>

</body>
</html>


