<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

BatchID = Request("batchid")
action = lcase(Request("action"))
subaction = lCase(Request("subaction"))
RequestID = Request("RequestID")
ContainerID = Request("ContainerID")
ContainerName = Request("ContainerName")
UOMAbbrv = Request("UOMAbbrv")
AllowBatchRequest = lcase(Request("AllowBatchRequest"))
AmountAvailable = Request("AmountAvailable")
bAllowReservation = Request("bAllowReservation")
bAllowRequest= true
dbkey = "ChemInv"
if AllowBatchRequest = "" then AllowBatchRequest = false

if Application("DEFAULT_REQUEST_DELIVERY_LOCATION") <> "" then
	LocationID = Application("DEFAULT_REQUEST_DELIVERY_LOCATION")
else
	LocationID = Session("CurrentLocationID")
end if

cLocationID = Request("LocationID")
if cLocationID = "" then cLocationID = 0
dateFormatString = Application("DATE_FORMAT_STRING")

if action = "" then action = "create"
RequestType = Request("RequestType")
if RequestType = "reservation" then
	RequestText = "Reserve an Inventory Batch"
	RequestTextAction = "Reserve"
	RequestTextPast = "Reserved"
	RequestStatusID = 9
else
	RequestText = "Request an Inventory Batch"
	RequestTextAction = "Request"
	RequestTextPast = "Requested"
	RequestStatusID = 2
end if
    Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING", adCmdStoredProc)
    Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 200, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("pContainerID", adNumeric, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("pBatchID", adNumeric, 1, 200, clng(BatchID))
    if bDebugPrint then
	    For each p in Cmd.Parameters
		    Response.Write p.name & " = " & p.value & "<BR>"
	    Next	
    Else
	    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING")
    End if
	BatchAmountAvailable = Cmd.Parameters("RETURN_VALUE")
    if trim(BatchAmountAvailable) = "" or isnull(BatchAmountAvailable) then 
        bAllowRequest = false
    else
        tempArr = split(BatchAmountAvailable, ",")
	    for each element in tempArr
	        tempArr2 = split(element,":")
	        AvailabeUOM = AvailabeUOM & ",'" & tempArr2(1) & "'"    
	    next
	    AvailabeUOM = mid(AvailabeUOM,2,len(AvailabeUOM)) 
	
        Select Case action
	    Case "create"
		    Caption = RequestText & " delivery of samples from this batch."
		    DateRequired = Today
		    UserID = uCase(Session("UserName" & "cheminv"))
	    Case "edit"
		    if subaction = "convert" or subaction = "partial" then
			    Caption = "Request this reservation."
		    else
			    if RequestStatusID = 9 then 
			        Caption = "Edit this reservation."
			    else
			        Caption = "Edit this request."
			    end if     
		    end if
		    Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchRequest(?,?)}", adCmdText)	
		    Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
		    Cmd.Parameters("PREQUESTID").Precision = 9	
		    Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)		
		    Cmd.Properties ("PLSQLRSet") = TRUE  
		    Set RS = Cmd.Execute
		    Cmd.Properties ("PLSQLRSet") = FALSE
		    If (RS.EOF AND RS.BOF) then
			    Response.Write ("<table><tr><td align=center colspan=6><span class=""GUIFeedback"">No " & lcase(RequestText) & " found for this container</Span></TD></tr></table>")
			    Response.End 
		    Else
			    UserID = RS("RUserID")
			    OrgUnitID = RS("ORG_UNIT_ID_FK")
			    DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
			    LocationID = RS("delivery_Location_ID_FK")
			    QtyRequired = RS("qty_required")
			    if isBlank(RS("AmountReserved")) then AmountReserved = 0 else AmountReserved = RS("AmountReserved") end if
			    if isBlank(RS("AmountRemaining")) then AmountRemaining = 0 else AmountRemaining = RS("AmountRemaining") end if
			    '-- Calculate the amount reserved of batch, minus the amount reserved for current reservation
			    AmountAvailable = cDbl(AmountRemaining)-(cDbl(AmountReserved)-cDbl(QtyRequired))
			    comments = RS("request_comments")
			    Field_1 = RS("field_1")
			    Field_2 = RS("field_2")
			    Field_3 = RS("field_3")
			    SpecialInstructions = RS("special_instructions")
			    ProofApprovalFilename = RS("proof_approval_filename")
			    ProofApprovalFiletype = RS("proof_approval_filetype")
			    ProofApprovalFilesize = RS("proof_approval_filesize")
                OldRequestStatusID = RS("request_status_id_fk")
                OldLocationID = RS("delivery_Location_ID_FK")
			    If isNull(ProofApprovalFilename) = FALSE then
	                    'AMENDEZ
			      Dim ret
			      FileFullPath = Session("sessionTempFolder") & "\" & ProofApprovalFilename
			      ret = Application("blobHandler").SaveBlobToFile(Session("UserName" & "cheminv") & "/" & Session("UserID" & "cheminv"), "SELECT * FROM Inv_Requests WHERE REQUEST_ID = " & RequestID, "PROOF_APPROVAL", CStr(FileFullPath))
			      if Instr(ret, "ERROR:") > 0 then
			         Response.Write ret 'pass on the error
			      else
			         fp = Replace(FileFullPath, " ", "%20")
			         fp = Replace(FileFullPath, "\", "\\")
			         fname = Replace(ProofApprovalFilename, " ", "%20")
			      end if
			    End If
	    '-- CSBR ID:122188
	    '-- Change Done by : Manoj Unnikrishnan
	    '-- Purpose: Correcting the RS field assignments to the Custom Create Request fields for 4 & 5
	    '-- Date: 27/02/2010
			    Field_4 = RS("field_4")
			    Field_5 = RS("field_5")
	    '-- End of Change #122188#
			    Date_1 = RS("date_1")
			    Date_2 = RS("date_2")
			    UOMAbbrv = RS("unit_abreviation")
			    unitstring = RS("unitstring")
		    End if	
	    Case "cancel"
		    RequestText = "Cancel an Inventory Batch Request"
		    Caption = "Are you sure you want to cancel this " & RequestText & "?"
    End select

    if not IsEmpty(Application("DEFAULT_SAMPLE_REQUEST_CONC")) then
	    arrUOM = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
	    UOMAbbrv = arrUOM(1)
    end if

    '-- If UserID is blank, default to current user
    if IsBlank(UserID) then 
	    UserID = uCase(Session("UserName" & "cheminv"))
    end if
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=RequestText%></title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function Initialize()
	{	
		if ((document.form1.ShipToName) && (document.form1.UserID))
			UpdateShipTo(document.form1.UserID.options[document.form1.UserID.selectedIndex].text);
     }   
	function ValidateRequest(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		//LocationID is required
		//document.form1.LocationID.value = document.form1.lpLocationID.value;
		<%if action <> "cancel" then%>
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID must be a psositve number
			if (!isPositiveNumber(document.form1.LocationID.value)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}

		// Date required must be a date
		if (document.form1.DateRequired.value.length == 0){
			errmsg = errmsg + "- Date Required is required.\r";
			bWriteError = true;
		}
		if (document.form1.DateRequired.value.length > 0 && !isDate(document.form1.DateRequired.value)){
			errmsg = errmsg + "- Date Required must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}

		// Quantity reserved is required
		if (document.form1.QtyRequired){
			if (document.form1.QtyRequired.value.length == 0) {
				errmsg = errmsg + "- Quantity is required.\r";
				bWriteError = true;
			}
			// Quantity required if present must be a number
			if (!isPositiveNumber(document.form1.QtyRequired.value)){
				errmsg = errmsg + "- Quantity required must be a positive number greater than zero.\r";
				bWriteError = true;
			}
		}

		if (document.form1.PartialQtyRequired){
			if (document.form1.PartialQtyRequired.value.length == 0) {
				errmsg = errmsg + "- Partial Quantity is required.\r";
				bWriteError = true;
			}
			// Quantity required if present must be a number
			if (!isPositiveNumber(document.form1.PartialQtyRequired.value)){
				errmsg = errmsg + "- Partial Quantity required must be a positive number greater than zero.\r";
				bWriteError = true;
			}
		}
		<% if subaction="partial" then %>
		if (isPositiveNumber(document.form1.PartialQtyRequired.value)) {
			if (document.form1.PartialQtyRequired.value == document.form1.QtyRequired.value) {
				document.form1.subaction.value = 'convert';
			} else if (Math.abs(document.form1.PartialQtyRequired.value) > Math.abs(document.form1.QtyRequired.value)) {
				errmsg = errmsg + "- The partial amount must be less than the original amount.\r";
				bWriteError = true;
			} else {			
				document.form1.QtyRequired.value = document.form1.QtyRequired.value - document.form1.PartialQtyRequired.value;
				document.form1.RequestStatusID.value = 9;
			}
		} else {
			document.form1.PartialQtyRequired.value = '';
		}
		<% end if       ' if subaction="partial" then %>
		
		// Build JS validation for custom Request fields
		<%For each Key in req_custom_createrequest_fields_dict%>
		if (document.form1.<%=Key%>.value.length == 0){
			errmsg = errmsg + "- <%=req_custom_createrequest_fields_dict.item(Key)%> is a required field.\r";
			bWriteError = true;
		}
		<% next %>		
        /*CSBR ID:121959
		 *Change Done by : Manoj Unnikrishnan 
		 *Purpose: Validate the length of Custom Request Fields
		 *Date: 24/02/2010
		*/
		// Build JS validation for validating the length of custom Request fields not more than 2000
		<%For each Key in custom_createrequest_fields_dict%>
		if (document.form1.<%=Key%>.value.length > 2000){
			errmsg = errmsg + "- <%=custom_createrequest_fields_dict.item(Key)%> cannot be greater than 2000 characters.\r";
			bWriteError = true;
		}
		<% next %>
		/*End of Change #121959#*/	
		
		//Checking for the batch quantity available for the selected UOM
	       var ValueSelected; 
	       var myArray;
	       var i;
	       var quantity;
	       var arraySelectedVal;
	       myArray = document.form1.BatchAmountAvailable.value.split(",");
	       ValueSelected = document.form1.UOM.options[document.form1.UOM.selectedIndex].value;
	       arraySelectedVal = ValueSelected.split("=");
	       for(i=0;i<myArray.length;i++){
	           quantity = myArray[i].split(":");
	           if (quantity[1] == arraySelectedVal[1]){
	                document.form1.RequiredUOM.value = arraySelectedVal[0];
	                if (Number(document.form1.QtyRequired.value) > Number(quantity[0])){
	                    errmsg = errmsg + "- Quantity required can not be exceed than Quantity Available with selected Unit of Measure.\r";
		                bWriteError = true;
		                break;
	                }
	           }
	       }
        /*CSBR ID:121371
		 *Change Done by : Siby 
		 *Purpose: Restrict Comments Textfield to 4000 characters
		 *Date: 18/02/2010		
		*/
		//Comments Field must not have more than 4000 characters
		if (document.form1.RequestComments.value.length > 4000)
		    {			
			errmsg = errmsg + "- Comments Field cannot be greater than 4000 characters.\r";
			bWriteError = true;
			}
		/*End of Change #121371#*/	
		
		<%end if        ' if action <> "cancel" then %>
		  
        <%if Application("ENABLE_OWNERSHIP")="TRUE"  then%>    
		     if(GetAuthorizedLocation(document.form1.LocationID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0 && document.form1.LocationID.value != <%=LocationID %>)
            {
                errmsg = errmsg + "- Not authorized for this location.\r";
			    alert(errmsg);
			    return;
            }
         <%end if  %> 
		
		//bWriteError = true;
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
	function UpdateShipTo(name)
	{
		var ShipTo;
		//Parameters: name (last, first)
		//Purpose: reverses the order of the anem and updates ShipTo field
		if (name.indexOf(',') > 0)
		{
			ShipTo = name.substr(name.indexOf(',')+1) + ' ' + name.substr(0, name.indexOf(','));
		}
		else
		{
			ShipTo = name;
		}
		document.form1.ShipToName.value = Trim(ShipTo);
}
		function Trim(s) 
	{
	  // Remove leading spaces and carriage returns
	  
	  while ((s.substring(0,1) == ' ') || (s.substring(0,1) == '\n') || (s.substring(0,1) == '\r'))
	  {
	    s = s.substring(1,s.length);
	  }

	  // Remove trailing spaces and carriage returns

	  while ((s.substring(s.length-1,s.length) == ' ') || (s.substring(s.length-1,s.length) == '\n') || (s.substring(s.length-1,s.length) == '\r'))
	  {
	    s = s.substring(0,s.length-1);
	  }
	  return s;
	}
	function ShowFileSelector()
	{
	   
	    document.all.proof_div.style.display='block';
	    document.all.download_link.style.display="none";
	    document.all.fileDeleted.value = "1";
	}
-->
</script>
</head>
<body ONLOAD="Initialize();">
<center>

<% if AllowBatchRequest = "false" and action="create" then %>

	<br /><br /><br /><br />
	<span class="GuiFeedback">This batch is not requestable.</span><br /><br />
	Reason: Pending Certificate of Testing (COT)
	<br><br>
	<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>

<% elseif bAllowReservation = "False" then %>

	<br /><br /><br /><br />
	<span class="GuiFeedback">You are not allowed to reserved against this batch.</span><br /><br />
	Reason: Not currently associated with an organization
	<br><br>
	<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
<%elseif not bAllowRequest then%>
<center>
	<br /><br /><br /><br />
	<span class="GuiFeedback">You are not allowed to make requests on this batch.</span><br /><br />
	Reason: Containers of this batch are not available in non system locations or you are not Admin of the containers of this batch.  
	<br/><br/>
	<a href="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>	
<% else %>

<form name="form1" action="RequestBatch_action.asp" method="POST">
<input type="hidden" name="action" value="<%=action%>" />
<input type="hidden" name="subaction" value="<%=subaction%>" />
<input type="hidden" name="cLocationID" value="<%=cLocationID%>" />
<input type="hidden" name="RequestType" value="<%=RequestType%>" />
<input type="hidden" name="RequestTypeID" value="2" />
<input type="hidden" name="RequestStatusID" value="<%=RequestStatusID%>" />
<% if action = "cancel" then %>
	<input type="hidden" name="BatchID" value="<%=BatchID%>" />
<% else %>
	<input type="hidden" name="RequestID" value="<%=RequestID%>" />
<% end if %>
<input type="hidden" name="AmountAvailable" value="<%=AmountAvailable%>" />
<% if RequestType <> "reservation" then %>
<input type="hidden" name="OrgUnitID" value="<%=OrgUnitID%>" />
<% end if %>
<input type="hidden" name="BatchAmountAvailable" value="<%=BatchAmountAvailable%>"/>
<input type="hidden" name="RequiredUOM" value=""/>
<input type="hidden" name="ShipToName" value=""/>
<input type="hidden" name="Olduserid" value="<%=UserID%>"/>
<input type="hidden" name="OldOrgUnitID" value="<%=OrgUnitID%>"/>
<input type="hidden" name="OldDateRequired" value="<%=DateRequired%>"/>
<input type="hidden" name="OldLocationID" value="<%=OldLocationID%>"/>
<input type="hidden" name="OldQtyRequired" value="<%=QtyRequired%>"/>
<input type="hidden" name="OldAmountReserved" value="<%=AmountReserved%>"/>
<input type="hidden" name="OldAmountRemaining" value="<%=AmountRemaining%>"/>
<input type="hidden" name="OldAmountAvailable" value="<%=AmountAvailable%>"/>
<input type="hidden" name="Oldcomments" value="<%=comments%>"/>
<input type="hidden" name="OldField_1" value="<%=Field_1%>"/>
<input type="hidden" name="OldField_2" value="<%=Field_2%>"/>
<input type="hidden" name="OldField_3" value="<%=Field_3%>"/>
<input type="hidden" name="OldField_4" value="<%=Field_4%>"/>
<input type="hidden" name="OldField_5" value="<%=Field_5%>"/>
<input type="hidden" name="OldDate_1" value="<%=Date_1%>"/>
<input type="hidden" name="OldDate_2" value="<%=Date_2%>"/>
<input type="hidden" name="OldUOMAbbrv" value="<%=UOMAbbrv%>"/>
<input type="hidden" name="Oldunitstring" value="<%=unitstring%>"/>
<input type="hidden" name="Oldfname" value="<%=fname%>"/>
<input type="hidden" name="OldRequestStatusID" value="<%=OldRequestStatusID%>"/>
<input type="hidden" name="OldSpecialInstructions" value="<%=SpecialInstructions%>"/>
<input type="hidden" name="OldProofApprovalFilename" value="<%=ProofApprovalFilename%>"/>
<input type="hidden" name="OldProofApprovalFiletype" value="<%=ProofApprovalFiletype%>"/>
<input type="hidden" name="OldProofApprovalFilesize" value="<%=ProofApprovalFilesize%>"/>
<% if Session("INV_SAMPLE_APPROVE" & dbkey) then%>
    <input type="hidden" name="InvSampleApprove" value="Approve"/>
<%end if %>
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input type="hidden" name="tempCSUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>" />
<table border="0">

	<tr><td colspan="2">
			<span class="GuiFeedback"><%=caption%></span><br><br>
	</td></tr>
	<% if action = "cancel" then %>
	<tr><td align="right" nowrap>
			<span class="required">Request ID:</span>
		</td><td>
			<input type="text" size="20" maxlength="50" name="RequestID" value="<%=RequestID%>" class="readOnly" readonly>
	</td></tr>
	<% else %>
	<tr><td align="right" nowrap>
			<span class="required">Batch ID:</span>
		</td><td>
			<input type="text" size="20" maxlength="50" name="BatchID" value="<%=BatchID%>" class="readOnly" readonly>
	</td></tr>
	<% end if %>
<%if action = "cancel" then%>
	<tr><td align="right" nowrap valign="top">
			Cancellation Reason:</span>
		</td><td>
			<textarea name="CancelReason" cols="40" rows="5"></textarea>
	</td></tr>
<% end if %>
<%if action <> "cancel" then%>
	<tr height="25">
		<td align="right" valign="top" nowrap>
			<span class="required">Delivery Location ID:</span>
		</td>
		<td colspan="3">
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker8 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false,"",false%> 
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Date Required:</span>
		</td>
		<td>
			<%call ShowInputField("", "", "DateRequired:form1:" & DateRequired , "DATE_PICKER:TEXT", "15")%>
			<!--<input type="text" name="DateRequired" size="15" value="<%=DateRequired%>"><a href onclick="return PopUpDate(&quot;DateRequired&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
	</tr>
	<tr>
		<% if RequestType = "reservation" then %>
		<%=ShowPickList("<span class=""required"">Reserved For:</span>", "OrgUnitID", OrgUnitID, "select distinct Org_Unit_ID AS Value, Org_Name AS DisplayText from inv_org_unit orgunit, inv_org_users orguser where orgunit.org_unit_id = orguser.org_unit_id_fk and orguser.user_id_fk='" & UserID & "'")%>
		<% else %>
		<%=ShowPickList2("<span class=""required"">Requested For:</span>", "UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC",25,"","","UpdateShipTo(this.options[selectedIndex].text);")%>
		</td>
		<% end if %>
	</tr>
	<% if subaction = "partial" then %>
	<tr>
		<td align="right" nowrap><span class="required">Original Amount (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>" class="readOnly" readonly></td>
	</tr>
	<tr>
		<td align="right" nowrap><span class="required">Partial Amount (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="PartialQtyRequired" value>
		<%=ShowSelectBox("UOM", unitstring, "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & AvailabeUOM & ") ORDER BY lower(DisplayText) ASC") %>   
		</td>
	</tr>	
	<% elseif subaction = "convert" then %>
	<tr>
		<td align="right" nowrap><span class="required">Amount <%=RequestTextPast%> (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>" class="readOnly" readonly>ww
		<%=ShowSelectBox("UOM", unitstring, "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & AvailabeUOM & ") ORDER BY lower(DisplayText) ASC") %>   
		</td>
	</tr>
	<% else %>
	<tr>
		<td align="right" nowrap><span class="required">Amount <%=RequestTextPast%> (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>">
		<%=ShowSelectBox("UOM", unitstring, "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & AvailabeUOM & ") ORDER BY lower(DisplayText) ASC") %>   
		</td>
	</tr>	
	<% end if %>
	<%
	'-- Display customer Request fields: custom_createrequest_fields_dict, req_custom_createrequest_fields_dict
	For each Key in custom_createrequest_fields_dict
		Response.write("<tr>" & vbcrlf)
		Response.write(vbtab & "<td align=""right"" nowrap>")
		For each Key1 in req_custom_createrequest_fields_dict
			if Key1 = Key then
				Response.Write("<span class=""required"">")
			end if
		Next
		Response.Write(custom_createrequest_fields_dict.item(key) & ":</span></td>" & vbcrlf)
		execute("FieldValue = " & Key)
		
		'-- When converting reservation into request, do not show the custom field values
		if subaction <> "" then FieldValue = ""
	'-- CSBR ID:121959
	'-- Change Done by : Manoj Unnikrishnan
	'-- Purpose: To show the Custom Fullfill Request field as a text area
	'-- Date: 24/02/2010
		
            if IsEmpty(custom_createrequest_fields_type_dict.item(key)) then
                Response.write(vbtab & "<td><textarea rows=""5"" cols=""30"" wrap=""hard"" name=""" & key & """>" & FieldValue & "</textarea></td>" & vbcrlf)                
            else
                if custom_createrequest_fields_type_dict.item(key) = "1" then
                    Response.write(vbtab & "<td><textarea rows=""5"" cols=""30"" wrap=""hard"" name=""" & key & """>" & FieldValue & "</textarea></td>" & vbcrlf)
                else 
                    Response.write(vbtab & "<td><input type=""text"" value=""" & FieldValue & """ name=""" & key & """/></td>" & vbcrlf)
                end if                
            end if
	'-- End of Change #121959#
		Response.write("</tr>" & vbcrlf)	
	Next	
	%>
	<!-- CSBR ID:121371 -->
	<!-- Change Done by : Siby Jacob -->
	<!-- Purpose: To show the Request comments -->
	<!-- Date: 18/02/2010 -->
	<tr><td align="right" nowrap valign="top">
			Comments:</span>
		</td><td>
			<textarea name="RequestComments" cols="30" rows="5"><%=comments%></textarea>
	</td></tr>
	<%if action <> "cancel"  and lcase(RequestType)<>"reservation" then%>
	<tr>
	    <td class="fieldLabel">
	        Special Instructions:
	    </td>
		<td valign="top">
			<input TYPE="text" SIZE="44" Maxlength="50" VALUE="<%=SpecialInstructions%>" id="Text8" name="specialInstructions">			
		</td>
	</tr>
	<tr>
	    <td class="fieldLabel">
	        <span  class="fieldLabel">Proof of Approval:</span>	        
	    </td>
		<td valign="top">
                
	                <%if  (action = "edit" and isNull(ProofApprovalFilename)) then%>
	                <div id="proof_div" name="proof_div">
	            <%else%>
	                <div id="download_link" name="download_link" >
	                    <a href="#" onClick="window.location.href('download.asp?filePath=<%=fp%>&fileName=<%=fname%>')" id="proof_link")"><%=ProofApprovalFilename%></a> <%=ProofApprovalFilesize%>KB | <a href="#" onClick="ShowFileSelector();")">Delete</a>
	                </div>
	                <div id="proof_div" name="proof_div" style="display:none;">
	            <%end if%>
	                
		                <input TYPE="text" SIZE="44" Maxlength="50" onfocus="blur()" disabled id="proof_file" name="proof_file" value='<%=ProofFilePath%>'>
		                <input TYPE="hidden"  value='<%=ProofFilePath%>' id="proof_approval" name="proof_approval">
		                <input TYPE="hidden"  value="0" id="fileDeleted" name="fileDeleted">
			            <a href="#" onclick="window.open('Request_Proof_Approval.asp', 'Request', 'width=560,height=140,status=yes,resizable=no')">Attach Proof of Approval</a> 	            
	                
                     </div>
			<!-- <a href="#" onclick="OpenFileUpload();">Attach Proof of Approval</a> -->
		</td>
	</tr>
	<%end if %>
	<!-- End of Change #121371# -->
   <%if action <> "cancel" and action <> "undodelivery"  and action <> "delete" then%>	
	<tr>
		<td class="fieldLabel">
		</td>
		<td valign="top">
		<input type="checkbox" name="toggleShipmentInfo" value="" onclick="if (this.checked) document.all.shipmentInfo.style.display='block'; else document.all.shipmentInfo.style.display='none'; " /> Show Shipment Info
		</td>
    </tr>
	<tbody id="shipmentInfo" style="display:none;">
	<tr>
		<td class="fieldLabel">
			Ship To:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="ShipToName" VALUE="" class="readOnly" READONLY>
		</td>
	</tr>	
	<tr>
		<td class="fieldLabel">
			Address1:
		</td>
		<td>
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
				<td><input TYPE="text" SIZE="30" Maxlength="50" NAME="Address1" class="readOnly" READONLY>&nbsp;</td>
				<td><span id="EditAddressLink" style="display:none;"><A HREF="Edit Address" onclick="OpenDialog('/cheminv/gui/EditAddress.asp?TableName=inv_locations&TablePKID=' +  document.form1.LocationID.value + '&AddressID=' + document.form1.AddressID.value,'AddressDiag', 4); return false;" CLASS="MenuLink">Edit Address</a></span></td>
			</tr>
			</table>
			
			
		</td>
	</tr>
	<tr>
		<td class="fieldLabel">
			Address2:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="Address2" class="readOnly" READONLY>
		</td>
	</tr>
	<tr>
		<td class="fieldLabel">
			Address3:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="Address3" class="readOnly" READONLY>
		</td>
	</tr>
	<tr>
		<td class="fieldLabel">
			Address4:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="Address4" class="readOnly" READONLY>
		</td>
	</tr>
	<tr>
		<td class="fieldLabel">
			City,State Postal Code:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="CityState" class="readOnly" READONLY>
		</td>
	</tr>
	<tr>
		<td class="fieldLabel">
			Country:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="Country" class="readOnly" READONLY>
		</td>
	</tr>
	<script type="text/javascript" language="JavaScript">
		//Update the address when the page loads
		//alert(document.form1.cLocationID.value);
		UpdateAddress(document.form1.LocationID.value);
	</script>
	
    </tbody>
    <%end if%>
	<% end if %>
	
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img src="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a HREF="#" onclick="ValidateRequest(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>		
</table>	
</form>

<% end if %>


</center>
</body>
</html>

