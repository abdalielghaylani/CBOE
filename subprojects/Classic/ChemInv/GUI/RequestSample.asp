<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if				
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>
<%
Dim Conn
Dim Cmd
Dim RS
Dim AmountString
Dim bAllowRequest
bAllowRequest=true
action = lcase(Request("action"))
pageAction = lcase(Request("pageAction"))
RequestID = Request("RequestID")
dbkey = "ChemInv"
if Request("ContainerID")<>"" then 
    ContainerID = Clng(Request("ContainerID"))
else
    ContainerID=NULL
end if    
BatchID1 =Request("BatchID1")
BatchID2 =Request("BatchID2")
BatchID3 =Request("BatchID3")
if Request("BatchID") <> "" and BatchID1 ="" then
    BatchID1 = Request("BatchID")
end if
UseBatch = GetBatchToUse()


regAttributes1 = ""
regAttributes2 = ""
regAttributes3 = ""

    if BatchID1 <> "" then
        regAttributes1 = GetRegAttributes(BatchID1)
        if regAttributes1 <> "" then
            attarray1 = split(regAttributes1,":")
            batchtype1 = attarray1(0)
            regAttributes1 = attarray1(1)
        end if
    end if

    if BatchID2 <> "" then
        regAttributes2 = GetRegAttributes(BatchID2)
        if regAttributes2 <> "" then
            attarray2 = split(regAttributes2,":")
            batchtype2 = attarray2(0)
            regAttributes2 = attarray2(1)
        end if
    end if
    if BatchID3 <> "" then
        regAttributes3 = GetRegAttributes(BatchID3)
        if regAttributes3 <> "" then
            attarray3 = split(regAttributes3,":")
            batchtype3 = attarray3(0)
            regAttributes3 = attarray3(1)
        end if
    end if


BatchID = BatchID1
if UseBatch<>"" then 
    if UseBatch=2 then BatchID = BatchID2
    if UseBatch=3 then BatchID = BatchID3
end if 
if batchid="" and request("batchid")<>"" then batchid=request("batchid")
ContainerBarcode = Request("ContainerBarcode")
UOMAbv = Request("UOMAbv")
if Request("UOMAbv") <> "" then
     'Drop down lists uses as value the following string UOMID=UOMAbv
     'And typically only an ID is updated from other apps (IE: Reg), so we must ensure the unit of abreviation is updated too
     'Call GetInvConnection()
	 SQL = "SELECT inv_units.unit_id UOMID FROM inv_units WHERE inv_units.unit_abreviation=?"
	 Call GetInvConnection()
	 set cmd = server.createobject("ADODB.Command")
	 cmd.CommandText = sql
	 cmd.ActiveConnection = Conn
	 Cmd.Parameters.Append Cmd.CreateParameter("UOMAbv", adVarchar, adParamInput, 50, UOMAbv)
     Set RS = Cmd.Execute
     unitstring = RS("UOMID")  & "=" & UOMAbv
end if

if Request("LocationID") <> "" then 
    cLocationID = Request("LocationID")
elseif cint(Session("CurrentLocationID")) > 0 then
    cLocationID = cint(Session("CurrentLocationID"))
elseif Application("DEFAULT_REQUEST_DELIVERY_LOCATION") > 0 then
    cLocationID = Application("DEFAULT_REQUEST_DELIVERY_LOCATION")
end if

DateRequired = Request("DateRequired")
UserID = Request("UserID")
NumContainers = Request("NumContainers")
SampleQty = Request("SampleQty")
ContainerTypeID = Request("ContainerTypeID")
ExpenseCenter = Request("ExpenseCenter")
Family = Request("Family")
BatchNameArray= split(GetBatchingFieldNames(),";")

if len(BatchID) > 0 then 
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING", adCmdStoredProc)
    Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 200, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("pContainerID", adNumeric, 1, 0, ContainerID)
    Cmd.Parameters.Append Cmd.CreateParameter("pBatchID", adNumeric, 1, 200, clng(BatchID))
    if bDebugPrint then
	    For each p in Cmd.Parameters
		    Response.Write p.name & " = " & p.value & "<BR>"
	    Next	
    Else
	    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING")
    End if
	BatchAmountAvailable = Cmd.Parameters("RETURN_VALUE")
	if BatchAmountAvailable <>"" then 
	    tempArr = split(BatchAmountAvailable, ",")
	    for each element in tempArr
	        tempArr2 = split(element,":")
	        AvailabeUOM = AvailabeUOM & ",'" & tempArr2(1) & "'"    
	    next
	    AvailabeUOM = mid(AvailabeUOM,2,len(AvailabeUOM)) 
	else
	    bAllowRequest=false
	end if 
end if 

if request("ContainerId")<>"" then 
	
	if Request("QtyRequired")<>"" then 
	    ContainerAmountAvailable  = Request("QtyRequired")
	else
		SQL = "Select qty_available from inv_containers where Container_Id=?"
		Call GetInvConnection()
	    set Cmd = server.createobject("ADODB.Command")
		Cmd.CommandText = sql
		Cmd.ActiveConnection = Conn
		Cmd.Parameters.Append Cmd.CreateParameter("ContainerId", adInteger, adParamInput, , request("ContainerId"))
	    Set RS= Cmd.Execute
	    if not (RS.eof and RS.bof) then 
		    ContainerAmountAvailable= RS("qty_available")
	    end if 
	end if 
end if 

if cLocationID = "" then cLocationID = 0
if pageAction = "" then pageAction = "page1"
'-- begin: SM Fix for CSBR-72560
if cLocationID<>0 then 
	LocationID = cLocationID
else
	LocationID = Session("CurrentLocationID")
end if 	
'-- End: SM Fix for CSBR-72560
if ContainerBarcode = "" Then ContainerBarcode = Session("barcode")
dateFormatString = Application("DATE_FORMAT_STRING")

RequestSampleByAmount = false
if Application("RequestSampleByAmount") = "1" then RequestSampleByAmount = true

allowContainerRequest = request("allowContainerRequest")
allowSampleRequest = request("allowSampleRequest")

if len(BatchID) > 0 then
    inBatch = true
else
    inBatch = false
end if

if allowSampleRequest = "" and inBatch then allowSampleRequest = 1

Select Case action
	Case "create"
		Caption = "Request delivery of samples from this container."
		'DateRequired = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now())
		if pageAction = "page1" then
		If request.QueryString("refresh")<>"" then
            QtyRequired = Request.Form("QtyRequired") 
            DateRequired=Request.Form("DateRequired") 
            LocationID=Request.Form("LocationID")
            unitstring=Request.Form("UOM")
            UserID = Ucase(Request.Form("UserID"))
            comments=Request.Form("RequestComments")
            SpecialInstructions=Request.Form("SpecialInstructions")
            ProofFilePath = Request.Form("proof_approval")
            
            Field_1 = Request.Form("FIELD_1")
            Field_2 = Request.Form("FIELD_2")
            Field_3 = Request.Form("FIELD_3")
            Field_4 = Request.Form("FIELD_4")
	    Else   
            UserID = Ucase(Session("UserNameChemInv"))
			DateRequired = Today
			QtyRequired = Request("QtyRequired")

	   End If
			if LocationID = "" or isEmpty(LocationID) then LocationID = cLocationID
			
			ContainerTypeID = Session("ContainerTypeID")
			Family = Session("Family")
		end if
		If Application("BypassRequestApproval")  then 
            RequestStatusId = 3
        Else 
            RequestStatusId = 2
        End If 
	Case "edit"
		Caption = "Edit this request"
		Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetRequest(?,?)}", adCmdText)	
		Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
		Cmd.Parameters("PREQUESTID").Precision = 9	
		Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)		
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		If (RS.EOF AND RS.BOF) then
			Response.Write ("<table><TR><TD align=center colspan=6><span class=""GUIFeedback"">No requests found for this container</Span></TD></tr></table>")
			Response.End 
		Else
			if pageAction = "page1" then
				UserID = RS("RUserID")
				DateRequired = RS("Date_Required")
				if not RequestSampleByAmount then NumContainers = cint(RS("number_containers"))
				ContainerTypeID = RS("container_type_id_fk")			
			end if
			LocationID = RS("delivery_Location_ID_FK")
			ContainerBarcode = RS("barcode")
			RequestTypeId = RS("request_type_id_fk")
			RequestStatusId = RS("request_status_id_fk")
			comments = RS("request_comments")
			QtyList = RS("quantity_list")
			ShipToName= RS("ship_to_name")
			ExpenseCenter = RS("expense_center")
			Family = RS("Family")
			OrgUnitID = RS("org_unit_id_fk")
            QtyRequired = RS("qty_required")
            Creator = RS("creator")
            OldRequestStatusID = RS("request_status_id_fk")
            OldLocationID = RS("delivery_Location_ID_FK")
        'CSBR ID:121959
		'Change Done by : Manoj Unnikrishnan 
		'Purpose: Modified the Field names by adding underscore ("_")
		'Date: 24/02/2010
            Field_1 = RS("field_1")
            Field_2 = RS("field_2")
            Field_3 = RS("field_3")
            Field_4 = RS("field_4")
            Field_5 = RS("field_5")
            Date_1 = RS("date_1")
            Date_2 = RS("date_2")            
		'End of Change #121959#
            
            SpecialInstructions = RS("special_instructions")
            ProofApprovalFilename = RS("proof_approval_filename")
            ProofApprovalFiletype = RS("proof_approval_filetype")
            ProofApprovalFilesize = RS("proof_approval_filesize")
            
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
            
            unitstring = RS("unitstring")
		End if	
	Case "cancel"
			Caption = "Are you sure you want to cancel this request?"
	Case "undodelivery"
			Caption = "This request has been marked as delivered.<BR>Are you sure you want to undo the delivery?"		
	Case "delete"	
			Caption = "Are you sure you want to delete this request?"
			UserID = Ucase(Session("UserNameChemInv"))	
End select
if (ContainerID="" or isnull(ContainerID)) and inBatch then
    Call GetInvConnection()
    SQL = "SELECT container_id FROM inv_containers WHERE ROWNUM =1 AND (batch_id_fk = ? or batch_id2_fk = ? or batch_id3_fk = ?)  "
	Cmd.Parameters.Append Cmd.CreateParameter("BatchID",131, 1, 0, BatchID)
    Set Cmd = GetCommand(Conn, SQL, adCmdText)
    Set RS = Cmd.Execute
    ContainerID = RS("container_id")
end if 

Function GetBatchToUse()
    UseBatch=Request("UseBatch")
    If UseBatch="" And Request("RequestType") <> "" Then
        'Registration uses that parameter to indicate which grouping field was requested:
        'C: Request from Container
        'B: Request from Batch
        'R: Request from any batch of the Registry Record
        dim requestType
        requestType = Request("RequestType")
        Call GetInvConnection()
        If requestType="B" Then
            SQL = "select min(batch_type_id_fk) btype from " & Application("CHEMINV_USERNAME") & ".INV_CONTAINER_BATCH_FIELDS having listagg(field_name, ',') within group (order by field_name) like('%REG_ID_FK%') AND listagg(field_name, ',') within group (order by field_name) like('%BATCH_NUMBER%') group by batch_type_id_fk"
        ElseIf requestType="R" Then
            SQL = "select min(batch_type_id_fk) btype from " & Application("CHEMINV_USERNAME") & ".INV_CONTAINER_BATCH_FIELDS having listagg(field_name, ',') within group (order by field_name) like('%REG_ID_FK%') AND not listagg(field_name, ',') within group (order by field_name) like('%BATCH_NUMBER%') group by batch_type_id_fk"
        Else
            SQL = "select min(batch_type_id_fk) btype from " & Application("CHEMINV_USERNAME") & ".INV_CONTAINER_BATCH_FIELDS having listagg(field_name, ',') within group (order by field_name) like('%CONTAINER_ID%') group by batch_type_id_fk"
        End If
        Set Cmd = GetCommand(Conn, SQL, adCmdText)
        Set RS = Cmd.Execute
        If NOT (RS.BOF AND RS.EOF) then 
            GetBatchToUse = CInt(RS("btype"))
        Else
            GetBatchToUse = UseBatch
        End If
    Else
        GetBatchToUse = UseBatch
    End If
End Function
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Request Samples from an Inventory Container</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script type="text/javascript" language="javascript">
<!--Hide JavaScript

	var DialogWindow;
	var popupWin = null;
	window.focus();

	function ValidateRequest(action, pageAction){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		var Sum,x,y;
		Sum = 0;

        //determine the request type
		var requestType = "";		
		if(document.all.cbRequestType)
		{
            for (var i=0; i < document.all.cbRequestType.length; i++)
            {
                if (document.all.cbRequestType[i].checked)
                {
                    requestType = document.all.cbRequestType[i].value;
                }
            }
        }        
          <%if Application("ENABLE_OWNERSHIP")="TRUE"  then%>    
		     if(GetAuthorizedLocation(document.form1.LocationID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not authorized for this location.\r";
			    alert(errmsg);
			    return;
            }
         <%end if  %>
        if (requestType == "aliquot")
        {
        
            <%if RequestSampleByAmount then %>
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
			    }//Fixing CSBR-70831
			   
			   //Checking for the batch quantity available for the selected UOM
			   var ValueSelected; 
			   var myArray;
			   var i;
			   var quantity;
			   var arraySelectedVal;
			   myArray = document.form1.BatchAmountAvailable.value.split(",");
			   if(document.form1.UOM)
			   {
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
			   }
			    
		    }

            <%else %>            
		    var NumContainers = document.form1.NumContainers.value;
		    var SampleQty = document.form1.SampleQty.value;
		    var arrValues = new Array(NumContainers);
    
		    // Number of Containers is required
		    if (document.form1.NumContainers.value.length == 0) {
			    errmsg = errmsg + "- Number of Samples is required.\r";
			    bWriteError = true;
		    }
		    else{
			    // Number of Containers must be a number
			    if (!isNumber(NumContainers)){
				    errmsg = errmsg + "- Number of Samples must be a number.\r";
				    bWriteError = true;
			    }
			    if (NumContainers < 1){
				    errmsg = errmsg + "- Number of Samples must be a positive number.\r";
				    bWriteError = true;
			    }
		    }

		    if (action != "edit" && pageAction == "page1"){
			    // Quantity per Sample is required
			    if (document.form1.SampleQty.value.length == 0) {
				    errmsg = errmsg + "- Quantity per Sample is required.\r";
				    bWriteError = true;
			    }
			    else{
				    // Quantity per Sample must be a number
				    if (!isNumber(SampleQty)){
					    errmsg = errmsg + "- Quantity per Sample must be a number.\r";
					    bWriteError = true;
				    }
				    if (SampleQty <= 0){
					    errmsg = errmsg + "- Quantity per Sample must be a positive number.\r";
					    bWriteError = true;
				    }
			    }
    		
		    }
		    <%end if %>
		}//"End of IF Block" To Fix the bug CSBR-75306
		// Quantity required if present must be a number
		else if (requestType == "container")
		{
		    document.form1.RequiredUOM.value = '';
		    if (document.form1.ContainerQtyRequired){
                if(Number(document.form1.ContainerAmountAvailable.value) == 0)
                {
                    errmsg = errmsg + "- This request can not be created as the quantity available for the container is zero.\r";
				    bWriteError = true;
                }
                else
                {
			        if (document.form1.ContainerQtyRequired.value.length == 0) {
				        errmsg = errmsg + "- Quantity is required.\r";
				        bWriteError = true;
			        }			    
			        if (!isPositiveNumber(document.form1.ContainerQtyRequired.value)){
				        errmsg = errmsg + "- Quantity required must be a positive number greater than zero.\r";
				        bWriteError = true;
			        }                
                    if (Number(document.form1.ContainerQtyRequired.value) > Number(document.form1.ContainerAmountAvailable.value)){
				        errmsg = errmsg + "- Quantity required can not be exceed than Quantity Available.\r";
				        bWriteError = true;
			        }
					//QtyRequired should not have comma
					var m = document.form1.ContainerQtyRequired.value.toString();		
					if(m.indexOf(",") != -1){
					    errmsg = errmsg + "- Quantity Required has wrong decimal operator.\r";
						bWriteError = true;
					}
                }
		    }
		}	//Fixing CSBR-70831	
			//LocationID is required
			//document.form1.LocationID.value = document.form1.lpLocationID.value;
			if (document.form1.LocationID.value.length == 0) {
				errmsg = errmsg + "- Delivery Location is required.\r";
				bWriteError = true;
			}
			else{
				// LocationID must be a psositve number
				if (!isPositiveNumber(document.form1.LocationID.value)){
					errmsg = errmsg + "- Location ID must be a positive number.\r";
					bWriteError = true;
				}
			}
        //} Comment to Fix the bug CSBR-75306
    		
		if (pageAction == "page2") {
			//LocationID is required
			//document.form1.LocationID.value = document.form1.lpLocationID.value;
			<%'if action <> "cancel" AND action <> "undodelivery" AND action <> "delete" then%>
			if (document.form1.LocationID.value.length == 0) {
				errmsg = errmsg + "- Delivery Location is required.\r";
				bWriteError = true;
			}
			else{
				// LocationID must be a psositve number
				if (!isPositiveNumber(document.form1.LocationID.value)){
					errmsg = errmsg + "- Location ID must be a positive number.\r";
					bWriteError = true;
				}
			}
			for(i=1; i<=NumContainers; i++) {
				x = eval("document.form1.sample" + String(i) + ".value;");
				arrValues[i-1] = x;
				Sum = Sum + Number(x);
			}
			//check the validity of all of the new quantities
			for (i=0; i<NumContainers; i++) {
				//must be a number
				if (!isNumber(arrValues[i])) {
					errmsg = errmsg + "- All quantities must be numbers.\r";
					bWriteError = true;
					break;
				}
				//must be positive numbers
				else if (arrValues[i] <= 0) {
					errmsg = errmsg + "- All quantities must be positive numbers.\r";
					bWriteError = true;
					break;
				}
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
        /*CSBR ID:121959
		 *Change Done by : Manoj Unnikrishnan 
		 *Purpose: Validate the Required fields and length of Custom Request Fields
		 *Date: 24/02/2010
		*/
		// Build JS validation for required custom Request fields
		<%For each Key in req_custom_createrequest_fields_dict%>
		if (document.form1.<%=Key%>.value.length == 0){
			errmsg = errmsg + "- <%=req_custom_createrequest_fields_dict.item(Key)%> is a required field.\r";
			bWriteError = true;
		}
		<% next %>
		// Build JS validation for validating the length of custom Request fields not more than 2000
		<%For each Key in custom_createrequest_fields_dict%>
		if (document.form1.<%=Key%>.value.length > 2000){
			errmsg = errmsg + "- <%=custom_createrequest_fields_dict.item(Key)%> cannot be greater than 2000 characters.\r";
			bWriteError = true;
		}
		<% next %>
		/*End of Change #121959#*/
        /*CSBR 164201 Adding date validation for custom date fields */	
        <%For each Key in custom_createrequest_fields_dict
			If InStr(lcase(Key), "date") then%>
				if (document.form1.<%=Key%>.value.length > 0 && !isDate(document.form1.<%=Key%>.value)){
					errmsg = errmsg + "- <%=custom_createrequest_fields_dict.Item(Key)%> must be in " + dateFormatString + " format.\r";
					bWriteError = true;
				}
			<%end if%>
		<%next%>
		<%'end if%>
		if (bWriteError){
			alert(errmsg);
		}
		else{
            <%if RequestSampleByAmount then %>
            formAction = "request_action.asp?action=<%=action%>";
			<%else %>
			if (pageAction=="page1" && (requestType == "aliquot" || requestType == "aliquot2" || requestType == "aliquot3"))
		        {formAction = "#";}
			else
				{formAction = "request_action.asp?action=<%=action%>";}
            <%end if %>            			        
			document.form1.action = formAction;
			document.form1.submit();
		}
	}
	function ShowFileSelector()
	{
	    document.all.proof_div.style.display="";
	    document.all.download_link.style.display="none";
	    document.all.fileDeleted.value = "1";
	}
	function UpdateAddress(value)
	{
		//handle the page load case
		if (document.form1.isLoad.value == 'false'){
			
			//alert(value);
			if (value == ""){
				document.form1.AddressID.value = "";
				document.form1.Address1.value = "";
				document.form1.Address2.value = "";
				document.form1.Address3.value = "";
				document.form1.Address4.value = "";
				document.form1.CityState.value = "";
				document.form1.Country.value = "";
			
				document.all.EditAddressLink.style.display = "none";
			}
			else {
				var strURL = "/cheminv/api/GetLocationAddress.asp?locationid=" + value;
				//alert(strURL);
				var httpResponse = JsHTTPGet(strURL); 
				//alert(httpResponse);

				if (httpResponse.length>0) {
					document.all.EditAddressLink.style.display = "";
			
					arrAddress = httpResponse.split("::");
					document.form1.AddressID.value = arrAddress[0];
					document.form1.Address1.value = arrAddress[2];
					document.form1.Address2.value = arrAddress[3];
					document.form1.Address3.value = arrAddress[4];
					document.form1.Address4.value = arrAddress[5];
					document.form1.CityState.value = arrAddress[6] + ', ' + arrAddress[14] + ' ' + arrAddress[9];
					document.form1.Country.value = arrAddress[15];
				
					/*
					0=ADDRESS_ID
					1=Contact_Name
					2=ADDRESS1
					3=ADDRESS2
					4=ADDRESS3
					5=ADDRESS4
					6=CITY
					7=STATE_ID_FK
					8=a.COUNTRY_ID_FK
					9=ZIP 
					10=FAX
					11=PHONE
					12=EMAIL
					13=State_Name
					14=State_Abbreviation
					15=Country_Name		
					*/
				}
				else{
				    document.form1.AddressID.value = "";
				    document.form1.Address1.value = "";
				    document.form1.Address2.value = "";
				    document.form1.Address3.value = "";
				    document.form1.Address4.value = "";
				    document.form1.CityState.value = "";
				    document.form1.Country.value = "";
    			
				    document.all.EditAddressLink.style.display = "none";
				}
			}
		}
		else 
		{
			document.form1.isLoad.value = 'false';
		}
		
		//http://dpdev2/cheminv/api/GetLocationAddress.asp?locationid=0	
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
	
	function Initialize()
	{	
		if (document.form1.ShipToName)
			UpdateShipTo(document.form1.UserID.options[document.form1.UserID.selectedIndex].text);
        
        <%if allowContainerRequest = "1" and allowSampleRequest = "1" then %>        
            <%if inBatch or RequestTypeID = "2" then %>
            //show/hide the appropriate form elements
            RequestType_Click('aliquot');			
            if (document.all.requestOption) document.all.requestOption.style.display = "";
            <%elseif (inBatch = false) or (RequestTypeId = "1") then %>
            //show/hide the appropriate form elements
            RequestType_Click('container');			
             if (document.all.requestOption) document.all.requestOption.style.display = "none";
             if (document.all.cbRequestType) document.all.cbRequestType[1].checked = true;
            <%end if %>
        <%elseif allowContainerRequest = "1" and allowSampleRequest <> "1" then %>            
            RequestType_Click('container');			
            if (document.all.requestOption) document.all.requestOption.style.display = "none";
            if (document.all.cbRequestType) document.all.cbRequestType[1].checked = true;
        <%elseif allowContainerRequest <> "1" and allowSampleRequest = "1" then %>            
            RequestType_Click('aliquot');			
            <%if(BatchID1="" and BatchID2="") or (BatchID1="" and BatchID3="") or (BatchID2="" and BatchID3="") then%>		
             if (document.all.requestOption) document.all.requestOption.style.display = "none";
            <%end if %>
            if (document.all.optContainer) document.all.optContainer.style.display = "none";
        <%end if%>
        
        
        <%if BatchID1="" then%>
            if(document.all.optbatch1) document.all.optbatch1.style.display = 'none';
        <%end if %> 
        <%if BatchID2="" then %>
           if(document.all.optbatch2) document.all.optbatch2.style.display = "none";
        <%end if%>
        <%if BatchID3="" then %>   
           if(document.all.optbatch3) document.all.optbatch3.style.display = "none";
        <%end if %>
        <% if UseBatch<>"" then%>
            if(document.getElementById("radBatch<%=UseBatch%>")) document.getElementById("radBatch<%=UseBatch%>").checked=true;
        <% else %>
       
         <%if cint(RequestTypeId)<>2 then %>
            RequestType_Click('container');
              document.getElementById("rdbcontainer").checked=true;
         <%end if %>  
        <% end if %>
            
	}
	
	function RequestType_Click(value)
	{
	    // show/hide the form elements and buttons for each action
	    // change the request type
	    
	    if (value == "aliquot" )    
	    {
            if  (document.all.aliquot) document.all.aliquot.style.display = "";
             if (document.all.container) document.all.container.style.display = "none";
            <%if RequestSampleByAmount then %>
            if (document.all.nextButton) document.all.nextButton.style.display = "none";
            if (document.all.okButton) document.all.okButton.style.display = "";
            <%else %>
            if (document.all.nextButton) document.all.nextButton.style.display = "";
            if (document.all.okButton) document.all.okButton.style.display = "none";
            <%end if %>
            if (document.all.RequestTypeID) document.all.RequestTypeID.value = "2";
	    }
	    else if (value == "container")
	    {
            if (document.all.aliquot) document.all.aliquot.style.display = "none";
            if (document.all.container) document.all.container.style.display = "";
            if (document.all.nextButton) document.all.nextButton.style.display = "none";
            if (document.all.okButton) document.all.okButton.style.display = "";
            if (document.all.RequestTypeID) document.all.RequestTypeID.value = "1";
	    }
	}
	
function ChangeBatch(BatchType)
{
    var QueryString="<%=Request.QueryString%>";
    var ReplaceStr='';
    <%if Request.QueryString("UseBatch") <> "" then
        vbreplaceStr = "UseBatch=" & Request.QueryString("UseBatch")
      else
        vbreplaceStr = ""
      end if
    %> 
    ReplaceStr = '<%=vbreplaceStr%>';
    if (ReplaceStr.length > 0)
    {
        QueryString= QueryString.replace(ReplaceStr, "UseBatch=" + BatchType.toString()); 
    }
    else
    {
        QueryString = QueryString.toString() + "&UseBatch=" + BatchType.toString();
    }


 document.form1.pageAction.value="";
 <%if instr(Request.QueryString,"refresh=1")=0 then%>
    QueryString ="refresh=1&" + QueryString;
 <% end if %>
var formAction = "requestSample.asp?" + QueryString;
document.form1.action = formAction;
document.form1.submit();

}
-->
</script>
</head>
<%
if not bAllowRequest then
%>
<body>
<center>
	<br /><br /><br /><br />
	<span class="GuiFeedback">You are not allowed to make requests on this batch.</span><br /><br />
	Reason: Containers of this batch are not available in non system locations.  
	<br/><br/>
	<a href="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>	
<%else %> 
<body ONLOAD="Initialize();">
<center>
<form name="form1"  id="form1" xaction="echo.asp" action="" method="POST">
<input type="hidden" name="ContainerID" value="<%=ContainerID%>"/>
<input type="hidden" name="BatchID" value="<%=BatchID%>"/>
<input type="hidden" name="cLocationID" value="<%=cLocationID%>"/>
<input type="hidden" name="RequestID" value="<%=RequestID%>"/>
<input type="hidden" name="Family" value="<%=Family%>"/>
<input type="hidden" name="pageAction" value="page2"/>
<input type="hidden" name="isLoad" value="true"/>
<input type="hidden" name="AddressID" value=""/>
<input type="hidden" name="isRequestSamples" value="true"/>
<input type="hidden" name="RequestTypeID" value="<%=RequestTypeId%>"/>
<input type="hidden" name="RequestStatusID" value="<%=RequestStatusId%>"/>
<input type="hidden" name="ContainerAmountAvailable" value="<%=ContainerAmountAvailable%>"/>
<input type="hidden" name="BatchAmountAvailable" value="<%=BatchAmountAvailable%>"/>
<input type="hidden" name="RequiredUOM" value=""/>
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input type="hidden" name="tempCsUserID" id="tempCsUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>" />
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
<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback"><%=Caption%></span><br/><br/>
		</td>
	</tr>
	<%if action <> "cancel" then%>
	<tr>
		<td class="fieldLabel">
			Requestor:
		</td>
		<td>
			<input TYPE="text" SIZE="44" Maxlength="50" VALUE="<%=iif(action="edit", Creator,UserID)%>"  disabled id="Requestor" name="Requestor">
		</td>
	</tr>
	<%end if %>
	<%if allowContainerRequest = "1" then %>
	<tr>
		<td class="fieldLabel">
			Container ID:
		</td>
		<td>
			<input TYPE="text" SIZE="44" Maxlength="50" VALUE="<%=ContainerBarcode%>" onfocus="blur()" disabled id="Text3" name="Text1">
		</td>
	</tr>
	<%end if %>
	<%if action <> "cancel" AND action <> "undodelivery" and action <> "delete" then%>
	<% if inBatch then %>
	<tr>
		<td class="fieldLabel">
			Batch ID:
		</td>
		<td>
			<input TYPE="text" SIZE="44" Maxlength="50" VALUE="<%=BatchID%>" onfocus="blur()" disabled id="Text1" name="Text1">
		</td>
	</tr>
	<% else %>
	<tr>
		<td class="fieldLabel">
			Family:
		</td>
		<td>
			<input TYPE="text" SIZE="44" Maxlength="50" VALUE="<%=Family%>" onfocus="blur()" disabled id="Text2" name="Text2">
		</td>
	</tr>
	<% end if %>
	<tbody id="requestOption" style="display:block;">
	<tr>
	    <td class="fieldLabel"><span class="required">Request Option:</span></td>
	    <td>
	       <span id="optbatch1"><input type="radio"  name="cbRequestType" id="radBatch1" checked value="aliquot" onclick="RequestType_Click(this.value);ChangeBatch(1);" />Request a sample from <%=split(BatchNameArray(0),"|")(0) %> <%if BatchID1<>"" then Response.Write  iif(regAttributes1<>"", " (" & regAttributes1 & ")",null) %> <br /> </span>
           <% if batchtype1 = "FULLREGNUM" then DisplayRegBatchInfo(regAttributes1)%>
   	       <span id="optbatch2"><input type="radio"  name="cbRequestType" id="radBatch2" value="aliquot" onclick="RequestType_Click(this.value);ChangeBatch(2);"/>Request a sample from <%=split(BatchNameArray(1),"|")(0) %><%if BatchID2<>"" then Response.Write  iif(regAttributes2<>"", " (" & regAttributes2 & ")",null) %> <br /> </span>
	       <% if batchtype2 = "FULLREGNUM" then DisplayRegBatchInfo(regAttributes2)%>
           <span id="optbatch3"> <input type="radio"  name="cbRequestType" id="radBatch3" value="aliquot" onclick="RequestType_Click(this.value);ChangeBatch(3);"/>Request a sample from <%=split(BatchNameArray(2),"|")(0) %><%if BatchID3<>"" then Response.Write  iif(regAttributes3<>"", " (" & regAttributes3 & ")",null) %> <br /> </span>
	       <% if batchtype3 = "FULLREGNUM" then DisplayRegBatchInfo(regAttributes3)%>
           <span id="optContainer"> <input type="radio" ID ="rdbcontainer" name="cbRequestType" value="container" onclick="RequestType_Click(this.value);"/> Request this specific container</span>
	    </td>
	</tr>
    </tbody>   
    
    <%
    if ((action="edit") and (batchtype1 = "FULLREGNUM" or batchtype1="REGNUM")) or (BatchID1<>"" and BatchID2="" and batchID3="" and allowContainerRequest="" and regAttributes1<>"" )  then%>
	<tbody id="Tbody1" style="display:block;">
	<tr>
	     <td align="center" colspan="2 valign="top">
           <%
           if BatchID1<>"" then Response.Write  iif(regAttributes1<>"", "" & regAttributes1 & "",null) %> <br />
	       <% if batchtype1 = "FULLREGNUM" then DisplayRegBatchInfo(regAttributes1)%>
	    </td>
	</tr>
    </tbody>  
    <%end if %>
	<tr>
		<td class="fieldLabel">
			<span class="required" title="Location where the container should be delivered to">Delivery Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
		<%'LocationID = iif((action="create"), "", LocationID)%>
		<%ShowLocationPicker8 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 30, false, "UpdateAddress(this.value);",false%> 
		</td>
	</tr>
	 <tr>
	     <td class="fieldLabel">
		    <span class="required">Date Required:</span>
		 </td>
		 <td>
		    <%call ShowInputField("", "", "DateRequired:form1:" & DateRequired , "DATE_PICKER:TEXT", "15")%>
		 </td>
	</tr>
	<tr>
		<td class="fieldLabel"><span class="required">Requested For:</span></td>
		<td>
		<% if action="edit" then  
			Response.Write ShowSelectBox3("UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 25, "", "", iif(pageAction="page1","UpdateShipTo(this.options[selectedIndex].text);//alert(this.options[selectedIndex].text);",""))
		else 
			'Response.Write ShowSelectBox3("UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People where active = 1 ORDER BY lower(Last_Name) ASC", 25, "", "", iif(pageAction="page1","","UpdateShipTo(this.options[selectedIndex].text);//alert(this.options[selectedIndex].text);"))			
			Response.Write ShowSelectBox3("UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People where active = 1 ORDER BY lower(Last_Name) ASC", 25, "", "", iif(pageAction="page1","UpdateShipTo(this.options[selectedIndex].text);",""))
		end if%>
		</td>
	</tr>
	<tbody id="aliquot" style="display:block;">
	<%if RequestSampleByAmount then%>
	<tr>
		<td align="right" nowrap><span class="required">Amount Required :</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>"> &nbsp;
		<% if len(batchid)>0 then  %>
		<%=ShowSelectBox("UOM", unitstring, "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & AvailabeUOM & ") ORDER BY lower(DisplayText) ASC") %>   
		<%end if %>

		(<b>Total Available:</b><%= replace(replace(BatchAmountAvailable,":", " "),",", ", ") %>)
		</td>

	</tr>	
	<!--<tr>
	<td align="right" nowrap><%=FilterFieldName %>:</td>
	<td><input type="text" size="10" maxlength="50" name="FilterValue" value=""> &nbsp;</td>
	</tr>-->
    <%else%>
	<tr>
		<%=ShowPickList("<SPAN class=required>Sample Container Type:</span>", "ContainerTypeID", ContainerTypeID,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
		<td class="fieldLabel">
			<span class="required">Number of Samples:</span>
		</td>
		<td>
		<%if pageAction="page1" then%>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="NumContainers" VALUE="<%=NumContainers%>">
		<%else%>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="NumContainers" class="readOnly" VALUE="<%=NumContainers%>" READONLY>
		<%end if%>
		</td>
	</tr>
		<%if pageAction="page1" and action <> "edit" then%>
		<tr>
			<td class="fieldLabel">
				<span class="required">Quantity per Sample (<%=UOMAbv%>):</span>
			</td>
			<td>
				<input TYPE="text" SIZE="10" Maxlength="50" NAME="SampleQty" VALUE="">
			</td>
		</tr>	
		<%else%>
			<input TYPE="hidden" SIZE="10" Maxlength="50" NAME="SampleQty" VALUE="<%=SampleQty%>">
		<%end if%>
    <%end if%>		
</tbody>		
	<tbody id="container" style="display:none;">
	<tr>
		<td class="fieldLabel">
			<span class="required">Amount Required (<%=Request("UOMAbv")%>):</span>
		</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="ContainerQtyRequired" value="<%=QtyRequired%>">
		</td>
	</tr>
		<tr>
			<td class="fieldLabel">
				Expense Center:
			</td>
			<td>
				<input TYPE="text" SIZE="10" Maxlength="50" NAME="ExpenseCenter" VALUE="<%=ExpenseCenter%>">
			</td>
		</tr>	
</tbody>		
	<!-- CSBR ID:121959 -->
	<!-- Change Done by : Manoj Unnikrishnan -->
	<!-- Purpose: To show the Custom Request Fields -->
	<!-- Date: 24/02/2010 -->
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

            if IsEmpty(custom_createrequest_fields_type_dict.item(key)) then
                Response.write(vbtab & "<td><textarea rows=""5"" cols=""30"" wrap=""hard"" name=""" & key & """>" & FieldValue & "</textarea></td>" & vbcrlf)                
            else
                if custom_createrequest_fields_type_dict.item(key) = "1" then
                    Response.write(vbtab & "<td><textarea rows=""5"" cols=""30"" wrap=""hard"" name=""" & key & """>" & FieldValue & "</textarea></td>" & vbcrlf)
                else 
                    Response.write(vbtab & "<td><input type=""text"" value=""" & FieldValue & """ name=""" & key & """/></td>" & vbcrlf)
                end if                
            end if
		    
		    Response.write("</tr>" & vbcrlf)	
	    Next	
	    %>
	<!-- End of Change #121959# -->
	<%end if%>

	<%if (pageAction = "page2" or RequestSampleByAmount) and (action <> "cancel" or action <> "undodelivery" or action <> "delete") then%>
	<%
	    if Application("RequestSampleByAmount") = "0" then
		    if action = "edit" then
		        arrValues = split(QtyList,",")
		        diff = NumContainers - ubound(arrValues)
		        if diff > 0 then
			        for i =0 to diff
				        QtyList = QtyList & ","
    		        next
			        arrValues = split(QtyList,",")
		        end if
		    else 
			    Dim arrValues()
			    reDim  arrValues(NumContainers)
			    for i = 0 to (NumContainers-1)
				    arrValues(i) = SampleQty
			    next
		    end if
		    Response.Write GenerateFields("Sample", "Quantity (" & UOMAbv & ")", "sample", NumContainers, arrValues)
        end if
	%>
	<tr>
		<td class="fieldLabel">
			Comments:
		</td>
		<td valign="top">
			<textarea rows="5" cols="30" name="RequestComments" wrap="hard"><%=Comments%></textarea>
		</td>
	</tr>
	<%if action <> "cancel" then%>
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
                
	            <%if (pageAction="page1" and action <> "edit") OR (action = "edit" and isNull(ProofApprovalFilename)) then%>
	                <div id="proof_div" name="proof_div">
	            <%else%>
	                <div id="download_link" name="download_link">
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
	
	<%if action <> "cancel" and action <> "undodelivery"  and action <> "delete" then%>	
	<tr>
		<td class="fieldLabel">
		</td>
		<td valign="top">
		<input type="checkbox" name="toggleShipmentInfo" value="" onclick="if (this.checked) document.all.shipmentInfo.style.display=''; else document.all.shipmentInfo.style.display='none'; " /> Show Shipment Info
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
	<%end if%>	
	<%if action = "cancel" or action = "undodelivery" or action = "delete" then%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="document.form1.action = 'request_action.asp?action=<%=action%>';document.form1.submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
	<%elseif pageAction="page2" then%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="history.go(-1); return false;"><img SRC="../graphics/btn_back_61.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateRequest('<%=action%>','<%=pageAction%>'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
	<%else%>
	<tbody id="okButton" style="display:none;">
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateRequest('<%=action%>','<%=pageAction%>'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
	</tbody>
	<tbody id="nextButton" style="display:block;">
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateRequest('<%=action%>','<%=pageAction%>'); return false;"><img SRC="../graphics/btn_next_61.gif" border="0" alt WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
	</tbody>
	<%end if%>
	
</table>	
</form>
<%end if %>
</center>



</body>
</html>
