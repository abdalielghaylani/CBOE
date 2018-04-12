<%@ Language=VBScript %>
<object RunAt="Server" Scope="Page" Id="Qty_dict" ProgID="Scripting.Dictionary"></object>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!-- MCD: added for extra ordering fields -->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetACXOrderAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<%
Dim Conn
Dim RS
Dim lRS
Dim oACXxml
Dim FormData
Dim ServerName
Dim bDebugPrint
'-- store the aspID so core can kill the session
StoreASPSessionID()	

dbkey = "ChemInv"
bDebugPrint = false
Response.Expires = -1
PackageIDList = Request("PackageIDList")
PackageID_arr = split(PackageIDList, ",")
QtyList= Request("QtyList")
Qty_arr = split(QtyList, ",")
For j = 0 to Ubound(PackageID_arr)
	Qty_dict.Add trim(Cstr(PackageID_arr(j))), trim(CStr(Qty_arr(j)))
Next

'Response.Write QtyList & "::" & PackageIDList
'FormData = "product=1&package=1&Synonym=1&structType=base64cdx&fieldName=PackageID&valueList=" & PackageIDList
'strURL = "http://" & Application("ACXServerName") & "/chemacx/api/getXMLdata.asp"


GetACXConnection()
connection_array = Application("base_connection" & "invacx")
' Distinguish Oracle and Access DBMS
DBMS = Ucase(connection_array(6))
if DBMS = "ORACLE" then
	sql = "SELECT " & _
			" Package.PackageID, " & _
			" PackageSizeConversion.Container_Qty_Max, " & _
			" PackageSizeConversion.Container_UOM_ID_FK, " & _
			" PackageSizeConversion.Container_Count, " & _
			" Package.""SIZE"", " & _
			" Package.CURRENCY, " & _
			" Package.CatalogNum, " & _
			" Package.Catalog2Num, " & _
			" Package.Price, " & _
			" NVL(Package.UNSPSC, 'Not Found') UNSPSC, " & _
			" Product.ProdName, " & _
			" Substance.CAS, " & _
			" Supplier.SupplierID, " & _
			" Supplier.ShortName, " & _
			" NVL(Supplier.DUNSCODE, 'Not Found') DUNS" &_
			" FROM Package, chemacxdb.PackageSizeConversion, Product, Substance, Supplier " & _
			" WHERE " & _
			" Package.""SIZE"" = PackageSizeConversion.Size_FK(+) " & _
			" AND  Package.ProductID = Product.ProductID " & _
			" AND  Product.CSNum = Substance.CsNum " & _
			" AND Product.SupplierID = Supplier.SupplierID " & _
			" AND Package.PackageID IN (" & PackageIDList & ")"	
else
	sql=	"SELECT Package.PackageID, " &_ 
			"PackageSizeConversion.Container_Qty_Max, " &_ 
			"PackageSizeConversion.Container_UOM_ID_FK, " &_ 
			"PackageSizeConversion.Container_Count, " &_ 
			"Package.[Size], " &_ 
			"Package.CURRENCY, " &_
			"Package.CatalogNum, " &_
			"Package.Catalog2Num, " &_
			"Package.Price, " &_ 
			"Package.UNSPSC, " & _
			"Product.ProdName, " &_ 
			"Substance.CAS, " &_ 
			"Supplier.SupplierID, " &_ 
			"Supplier.ShortName, " &_
			"Supplier.DUNSCODE " &_
			"FROM (((Package LEFT JOIN PackageSizeConversion ON Package.[Size] = PackageSizeConversion.Size_FK) " &_ 
			"INNER JOIN Product ON Package.ProductID = Product.ProductID) " &_ 
			"INNER JOIN Substance ON Product.CSNum = Substance.CsNum) " &_ 
			"INNER JOIN Supplier ON Product.SupplierID = Supplier.SupplierID " &_
			"WHERE (((Package.PackageID) IN (" & PackageIDList & " )))"
end if
'Response.Write sql
'Response.end
Set Cmd = GetCommand(Conn, sql, adCmdText)
Set lRS = Cmd.Execute()
'Response.end
' DGB Added code to optinally bypass the GUI of the ordering screens.
' This block of code sets all the necessary page variables required
' by the BuildActionBatchFromACXXML.asp and then uses CSHTTP function
' to post the data which results in the creation of the InvActionBatch.xml
' Upon successful post, this page posts to ProcessActionBatch.asp to 
' complete the import operation.
if Application("BYPASS_ORDERING_SCREENS") then
	Project= Application("DEFAULT_CONTAINER_ORDER_PROJECT")
	Job= Application("DEFAULT_CONTAINER_ORDER_JOB")
	LocationID = 1    'means "On Order"
	' Read Delivery location user setting
	Session("DeliveryLocationID") = GetUserProperty(Session("UserNameCheminv"), "INVContainerOrderDeliveryLoc")
	' Fall back to Default location user setting
	if Session("DeliveryLocationID") = "" then
		Session("DeliveryLocationID") = GetUserProperty(Session("UserNameCheminv"), "INVDefLoc")
	end if
	' Fall back to application default delivery location
	if Session("DeliveryLocationID") = "" or Session("DeliveryLocationID") = "0" then
		Session("DeliveryLocationID") = Application("DEFAULT_CONTAINER_ORDER_DELIVERY_LOCATION")
	end if
	DeliveryLocationID = Session("DeliveryLocationID")
	DueDate = Month(now()) & "/" & Day(now()) & "/" & Year(now())
	RushOrder = 0
	ContainerStatusID = Application("DEFAULT_CONTAINER_ORDER_STATUS")  '3 means Order Pending
	OwnerID = Application("DEFAULT_CONTAINER_ORDER_OWNER")	
	OrderReason =  Application("DEFAULT_CONTAINER_ORDER_REASON")  '1 means Insuficient Qty
	OrderReasonOther = "" 
	AddNameValuePair FormData, "Project", Project
	AddNameValuePair FormData, "Job", Job
	AddNameValuePair FormData, "DeliveryLocationID", DeliveryLocationID
	AddNameValuePair FormData, "LocationID", LocationID
	AddNameValuePair FormData, "DueDate", DueDate
	AddNameValuePair FormData, "RushOrder", RushOrder
	AddNameValuePair FormData, "ContainerStatusID", ContainerStatusID
	AddNameValuePair FormData, "OwnerID", OwnerID
	AddNameValuePair FormData, "OrderReason", OrderReason
	AddNameValuePair FormData, "OrderReasonOther", OrderReasonOther
	AddNameValuePair FormData, "sid", session.SessionID
	DefUOCostID = 6
	DefContainerTypeID= 1 'means bottle
	DefContainerTypeName = "bottle"
	QtyList = ""
	PackageIDList =""
	If lRS.BOF AND lRS.EOF then
		Response.Write "Error while fetching data from ChemACX"
	Else
		lRS.MoveFirst
		Do While NOT lRS.EOF
			packID = lRS("packageID").value
			Duns = lRS("DUNS").value
			UNSPSC = lRS("UNSPSC").value
			size = lRS("SIZE").value
			blank = InStr(1, size," ")
			sizeUnit = lRS("Container_UOM_ID_FK").value 
			if Not IsNumeric(sizeUnit) then
				sizeUnit = Application("DEFAULT_CONTAINER_ORDER_UNIT_OF_MEAS")
			end if
			sizeValue = lRS("Container_Qty_Max").value
			if Not IsNumeric(sizeValue ) then
				sizeValue = Application("DEFAULT_CONTAINER_ORDER_SIZE")
			end if
			Container_Count = lRS("Container_Count").value
			if IsNull(Container_Count) then Container_Count = 1
			Qty = Qty_dict.Item(Cstr(packID)) 
			price = lRS("price")
			if NOT (IsNumeric(price)) then price = 0
			AddItemToList PackageIDList, PackID 
			AddItemToList QtyList, Qty 
			AddItemToList UOMIDList, sizeUnit
			AddItemToList PriceList, Price
			AddItemToList ContainerTypeName, DefContainerTypeName
			AddItemToList ContainerTypeIDList, DefContainerTypeID
			AddItemToList ContainerSizeList, sizeValue 
			AddItemToList ContainerCountList, Container_Count
			AddItemToList UOCostIDList, DefUOCostID
			AddItemToLIst DUNSList, Duns
			AddItemToList UNSPSCList, UNSPSC
			AddItemToList CategoryList, Category
			'D Deshmukh added for correct iProc integration 15-Feb-2010
			DDCatalogNum = lRS("Catalog2Num")
			if len(trim(DDCatalogNum))=0 then
				DDCatalogNum = lRS("CatalogNum")
			end if
			AddItemToList DSDCatalogNumList, DDCatalogNum
			'D Deshmukh end of additions
			lRS.MoveNext
		Loop
		AddNameValuePair FormData, "PackageIDList", PackageIDList
		AddNameValuePair FormData, "QtyList", QtyList
		AddNameValuePair FormData, "UOMIDList", UOMIDList
		AddNameValuePair FormData, "PriceList", PriceList
		AddNameValuePair FormData, "ContainerTypeName", ContainerTypeName
		AddNameValuePair FormData, "ContainerTypeIDList", ContainerTypeIDList
		AddNameValuePair FormData, "ContainerSizeList", ContainerSizeList
		AddNameValuePair FormData, "ContainerCountList", ContainerCountList
		AddNameValuePair FormData, "UOCostIDList", UOCostIDList
		AddNameValuePair FormData, "CSUserName", Session("UserName" & "cheminv")
		AddNameValuePair FormData, "DUNSList", DUNSList
		AddNameValuePair FormData, "UNSPSCList", UNSPSCList
		AddNameValuePair FormData, "CategoryList", CategoryList
		'D Deshmukh added for correct iProc integration 15-Feb-2010
		AddNameValuePair FormData, "DSDCatalogNumList", DSDCatalogNumList
		'D Deshmukh end of additions

		AddNameValuePair FormData, "CSUSerID", Session("UserID" & "cheminv")
		ServerName= Request.ServerVariables("Server_Name")
		pUserAgent= "CShttpRequest3"
		pSendTimeout= 30 * 1000
		pReceiveTimout= 120 * 1000
		'Response.Write FormData
		'Response.End
		' Post to BuildActionBatch
		pTarget= "/cheminv/gui/BuildActionBatchFromACXXML.asp"
		httpResponse = CShttpRequest3("POST", ServerName, pTarget, pUserAgent, FormData, pSendTimeout, pReceiveTimout)
		' Post ProcessActionBatch
		FormData = ""
		AddNameValuePair FormData, "sid", session.SessionID
		AddNameValuePair FormData, "CSUserName", Session("UserName" & "cheminv")
		AddNameValuePair FormData, "CSUSerID", Session("UserID" & "cheminv")
		pTarget= "/cheminv/api/ProcessActionBatch.asp?action=commit" 
		httpResponse = CShttpRequest3("POST", ServerName, pTarget, pUserAgent, FormData, pSendTimeout, pReceiveTimout)
		if InStr(httpResponse,"ORA-")> 0 OR bDebugPrint then
			response.write httpResponse 
			response.end
		end if
		server.transfer("SendACXToInvMessage.asp")
	End if
	Response.End
end if
Sub AddItemToList(byref v, byVal str)
	dim maybeComma
	maybeComma = ","
	if len(v) = 0 then maybeComma = ""
	v = v & maybeComma & str
End sub
Sub AddNameValuePair(byref sOut, byval sName, byval sVal)
	dim maybeAmp
	maybeAmp = "&"
	if len(sOut) = 0 then maybeAmp = ""
	sOut = sOut & maybeAmp & sName & "=" & sVal  
end sub
%>
<html>
<head>
<title>Create Inventory Containers from ChemACX</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript">
	// default container status = 1 (available)
	var ContainerStatusID = 1;
	var PackageIDList = "<%=PackageIDList%>";
	var Container_arr = PackageIDList.split(",");
	
	function Validate(){
		
		// MCD: added extra ordering fields
		!document.form1.iDueDate ? document.form1.DueDate.value="<%=DueDate%>" : document.form1.DueDate.value = document.form1.iDueDate.value;	
		!document.form1.iProject ? document.form1.Project.value="<%=Project%>" : document.form1.Project.value = document.form1.iProject.value;	
		!document.form1.iJob ? document.form1.Job.value="<%=Job%>" : document.form1.Job.value = document.form1.iJob.value;	
		!document.form1.iOwnerID ? document.form1.OwnerID.value="<%=OwnerID%>" : document.form1.OwnerID.value = document.form1.iOwnerID.value;	
		!document.form1.iDeliveryLocationID ? document.form1.DeliveryLocationID.value="<%=DeliveryLocationID%>" : document.form1.DeliveryLocationID.value = document.form1.iDeliveryLocationID.value;
		<%if Application("ORDER_WORKFLOW_OPTION")=1 then %>
		!document.form1.iOrderReason ? document.form1.OrderReason.value="<%=OrderReason%>" : document.form1.OrderReason.value = document.form1.iOrderReason.value;	
		!document.form1.iOrderReasonOther ? document.form1.OrderReasonOther.value=document.form1.OrderReasonOther.value : document.form1.OrderReasonOther.value = document.form1.iOrderReasonOther.value;
		<%end if %>
		<%if Application("BYPASS_ORDERING_SCREENS")=1 then %>
		    document.form1.RushOrder.value = document.form1.rushOrder_cb.checked?1:0;
		<%end if  %>
		// MCD: end changes
		var size = "";
		var Initial = "";
		var UOMID = "";
		var errmsg = "Please fix the following problems:\r\r";
		var bWriteError = false;
		var blocalError;
		
		// MCD: validation for extra ordering fields

		//DeliveryLocationID is required
		if (document.form1.DeliveryLocationID.value.length == 0) {
			errmsg = errmsg + "- Delivery Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// DeliveryLocationID must be a positive number
			if (!isPositiveNumber(document.form1.DeliveryLocationID.value)){
				errmsg = errmsg + "- Delivery Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}
				// determine whether there are enough open rack positions
		if (document.form1.isRack.value == "1") {
		    if (AreEnoughRackPositions(document.form1.totalQty.value,document.form1.iDeliveryLocationID .value) == false) {
				errmsg = errmsg + "- There are not enough open positions from the selected start position\rin the rack(s) you have selected.\r";
				bWriteError = true;
		    }
		}

		// Project is required.
		if (document.form1.Project.value.length == 0){
			errmsg = errmsg + "- The Project is required.\r";
			bWriteError = true;
		}

		// Job is required.
		if (document.form1.Job.value.length == 0){
			errmsg = errmsg + "- The Job is required.\r";
			bWriteError = true;
		}
		// The due date must be present.
		if (document.form1.DueDate.value.length == 0){
			errmsg = errmsg + "- The Due Date is required.\r";
			bWriteError = true;
		}

		// Due Date must be a date
		if (document.form1.DueDate.value.length > 0 && !isDate(document.form1.DueDate.value)){
			errmsg = errmsg + "- The Due Date must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}

		// Due Date must be at least tomorrow

	    var d, dueDate;
	    dueDate = getDate(document.form1.DueDate.value);
	    d = new Date();
	    d.setHours(0,0,0,0);		// set time to 00:00:00.000
	    d.setDate(d.getDate()+1);	// set date to "tomorrow"
	    
		if (document.form1.DueDate.value.length > 0 && isDate(document.form1.DueDate.value) && Date.parse(dueDate) < Date.parse(d)) {
			errmsg = errmsg + "- The Due Date must be at least tomorrow.\r";
			bWriteError = true;
		}
		
		// D Deshmukh: 08-Feb-2010: Eliminate validation for Order Reason for Biogen Idec
		// D Deshmukh: 08-Feb-2010: Biogen Idec does not want Order Reason fields to be mandatory
		// if (document.form1.OrderReason.value.length == 0){
		//	errmsg = errmsg + "- The Order Reason is required.\r";
		//	bWriteError = true;
		// }
		// if (document.form1.OrderReason.value == 3 && document.form1.OrderReasonOther.value.length == 0) {
		//	errmsg = errmsg + "- The Order Reason is required.\r";
		//	bWriteError = true;
		// }
		
		// MCD: end changes
		// Destination is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r\r";
			alert(errmsg);
			return false;
		}
		else{
			// Destination must be a number
			if (!isNumber(document.form1.LocationID.value)){
			errmsg = errmsg + "- Destination must be a number.\r\r";
			bWriteError = true;
			}
		}
		
		NumContainers = Container_arr.length;
		// initialize the containerTypeName
		document.form1.ContainerTypeName.value = "";
		for (i=0; i< NumContainers; i++){
			blocalError =  false;
			//(NumContainers > 1) ? alert(document.form1.ContainerTypeIDList[i].options[document.form1.ContainerTypeIDList[i].selectedIndex].text) : alert(document.form1.ContainerTypeIDList.options[document.form1.ContainerTypeIDList.selectedIndex].text);
			// set the container type name 
			//document.form1.ContainerTypeName.value = document.form1.ContainerTypeName.value + ","
			if (NumContainers > 1) 
			{
				if (i == (NumContainers - 1))
					document.form1.ContainerTypeName.value = document.form1.ContainerTypeName.value + document.form1.ContainerTypeIDList[i].options[document.form1.ContainerTypeIDList[i].selectedIndex].text;
				else
					document.form1.ContainerTypeName.value = document.form1.ContainerTypeName.value + document.form1.ContainerTypeIDList[i].options[document.form1.ContainerTypeIDList[i].selectedIndex].text + ","
			}
			else
			{
				document.form1.ContainerTypeName.value = document.form1.ContainerTypeName.value + document.form1.ContainerTypeIDList.options[document.form1.ContainerTypeIDList.selectedIndex].text
			}
			
			(NumContainers > 1) ? price = document.form1.PriceList[i].value : price = document.form1.PriceList.value;
			(NumContainers > 1) ? size = document.form1.ContainerSizeList[i].value : size = document.form1.ContainerSizeList.value;
			//MCD: changed InitialAmountList to ContainerSizeList since all containers are assumed to be full
			(NumContainers > 1) ? Initial = document.form1.ContainerSizeList[i].value : Initial = document.form1.ContainerSizeList.value;
			//MCD: end changes
			(NumContainers > 1) ? UOMID = document.form1.UOMIDList[i].value : UOMID = document.form1.UOMIDList.value;
		
			(NumContainers > 1) ? Category = document.form1.CategoryList[i].value : Category = document.form1.CategoryList.value;
			
			errmsg = errmsg + "For Container " + (i + 1) + ":\r";

			if (price.length == 0) {
				errmsg = errmsg + "   - Price is required.\r";
				bWriteError = true;
				blocalError = true
			}
			else{
				 //price must be a number
				if (price.length > 0 && !isNumber(price)){
					errmsg = errmsg + "   - Price must be a number.\r";
					bWriteError = true;
					blocalError = true;
				}
			}
			if (size.length == 0) {
				errmsg = errmsg + "   - Size is required.\r";
				bWriteError = true;
				blocalError = true
			}
			else{
				 //size must be a number
				if (!isNumber(size)){
					errmsg = errmsg + "   - Size must be a number.\r";
					bWriteError = true;
					blocalError = true;
				}
			}
			if (Initial.length == 0) {
				errmsg = errmsg + "   - Initial Amount is required.\r";
				bWriteError = true;
				blocalError = true
			}
			else{
				 //Initial Amount must be a number
				if (!isNumber(Initial)){
					errmsg = errmsg + "   - Initial Amount must be a number.\r";
					bWriteError = true;
					blocalError = true;
				}
			}
			if (UOMID.length == 0) {
				errmsg = errmsg + "   - Unit of measure is required.\r";
				bWriteError = true;
				blocalError = true
			}
if (Category.length == 0) {
				errmsg = errmsg + "   - Category is required.\r";
				bWriteError = true;
				blocalError = true
			}

			if (!blocalError) errmsg +=  "   -No errors.\r"
		}
			
		
		if (bWriteError){
			alert(errmsg);
		}
		else{
			//var xml = GetActionBatchXML();
			//alert(xml)
			//document.form1.ActionBatchXML.value = xml;
			//alert(document.form1.ActionBatchXML.value);
			document.form1.submit();
		}
	}
	// MCD: added for extra ordering fields
	function postDataFunction(sTab, newFocus) {
		document.form1.action = "ImportFromChemACX.asp?GetData=form&TB=" + sTab + "&setFocus=" + newFocus
		document.form1.submit()
	}
	// MCD: end changes
</script>

</head>
<body>
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top">
			<img src="../graphics/cheminventory_banner.gif" border="0" / WIDTH="286" HEIGHT="50">
		</td>
		<td align="right">
			<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
		</td>
	</tr>
</table>
<%If not Session("INV_ORDER_CONTAINER" & dbkey) or not Session("INV_MANAGE_SUBSTANCES" & dbkey) then%>
<table ALIGN="CENTER" BORDER="1" CELLPADDING="0" CELLSPACING="0" BGCOLOR="#ffffff" Width="90%">
	<tr>
		<td>
			<center><span class="GuiFeedback">This action requires both the Order Container and Manage Substances privileges.<br>Please contact your system administrator.</span></center>
			<p><center><a HREF="3" onclick="window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a></center>
		</td>
	</tr>
</table>

<%else%>
<form name="form1" action="BuildActionBatchFromACXXML.asp" method="POST">
	<!-- MCD: added extra ordering fields -->
	<input TYPE="hidden" NAME="DueDate" Value>
	<%if Application("ORDER_WORKFLOW_OPTION")=1 then %>
	    <input TYPE="hidden" NAME="Project" Value>
	    <input TYPE="hidden" NAME="Job" Value>
    	<input TYPE="hidden" NAME="OrderReason" Value>
    <%end if %>	
    <input TYPE="hidden" NAME="OrderReasonOther" Value="<%=OrderReasonOther%>">
	<input TYPE="hidden" NAME="RushOrder" Value="<%=RushOrder%>">
	<input TYPE="hidden" NAME="DeliveryLocationID" Value> 
	<input TYPE="hidden" NAME="LocationID" Value="1">
	<input TYPE="hidden" NAME="ContainerStatusID" Value="3">
	<input TYPE="hidden" NAME="OwnerID" Value="<%=Owner%>">
	<input TYPE="hidden" NAME="ContainerTypeName" VALUE="">
	

	<!-- MCD: end changes -->
<table border="0">
<tr>
	<td align="right" nowrap>
		<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
	</td>
	<td>
		<%'MCD: replaced this
			'ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 31, false 
				'with this:
			ShowLocationPicker2 "document.form1", "iDeliveryLocationID", "lpDeliveryLocationBarCode", "lpDeliveryLocationName", 10, 49, false, DeliveryLocationID
		%> 
	</td>
<!-- MCD: added extra ordering fields -->
    <!--SM added if Application("ORDER_WORKFLOW_OPTION")=2 then show only Location and Owner -->
    <%if Application("ORDER_WORKFLOW_OPTION")=1 then %>
	    <tr height="25">
		    <td align="right" valign="top">
			    <span class="required" title="Pick an option from the list">Project:</span>
		    </td>
		    <td>
		    <%=ShowSelectBox3("iProject", Project, "SELECT DISTINCT project_no AS Value, project_description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_Project_Job_Info ORDER BY UPPER(project_description)", 27, RepeatString(43, "&nbsp;"), "", "postDataFunction('Required', 'iJob');")%>
		    <%If SupplierID = 1000 Then%>
			    <%=ShowInputBox("Phone#:", "UnknownSupplierPhoneNumber", 25, "", False, False)%>
		    <%End if%>
		    </td>
	    </tr>
	    <tr height="25">
		    <td align="right" valign="top">
			    <span class="required" title="Pick an option from the list">Job:</span>
		    </td>
		    <td>
		    <%=ShowSelectBox2("iJob", Job, "SELECT job_no AS Value, job_description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_Project_Job_Info WHERE project_no = '" & Project & "' ORDER BY UPPER(job_description)", 27, RepeatString(43, "&nbsp;"), "")%>
		    <%If SupplierID = 1000 Then%>
			    <%=ShowInputBox("FAX#:", "UnknownSupplierFAXNumber", 25, "", False, False)%>
		    <%End if%>
	    </tr>
	    <tr height="25">
		    <td align="right" valign="top" nowrap width="150"><span class="required">Due Date:</span></td>
		    <td>
		    <%call ShowInputField("", "", "iDueDate:form1:" & DueDate , "DATE_PICKER:TEXT", "15")%>		
		    <!--<input type="text" name="iDueDate" size="15" value="<%=DueDate%>"><a href onclick="return PopUpDate(&quot;iDueDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		    </td>
	    </tr>
	    <tr height="25">
		    <td>&nbsp;</td><td><input type="checkbox" name="rushOrder_cb" onclick="document.form1.RushOrder.value = this.checked;">Rush order<br></td>
		    <script language="Javascript">if (document.form1.RushOrder.value == "true") document.form1.rushOrder_cb.click(); </script>
	    </tr>
	    <tr height="25">
		    <td align="right" valign="top">
				<!-- D Deshmukh 08-Feb-2010 Eliminated need for Order Reason to be a mandatory field for Biogen Idec -->
			    <!-- D Deshmukh 08-Feb-2010 <span class="required" title="Pick an option from the list">Order Reason:</span> -->
				<span title="Pick an option from the list">Order Reason:</span>
		    </td>
		    <td>
		    <%=ShowSelectBox2("iOrderReason", OrderReason, "SELECT DISTINCT container_order_reason_id AS Value, name AS DisplayText, sort_order FROM " & Application("CHEMINV_USERNAME") & ".inv_Container_Order_Reason ORDER BY sort_order ASC", 27, RepeatString(43, "&nbsp;"), "")%>
		    </td>
	    </tr>
	    <tr height="50">
		    <td align="right" valign="top" nowrap>
			    Reason (other):
		    </td>
		    <td valign="top">
			    <textarea rows="2" cols="60" name="iOrderReasonOther" wrap="hard"><%=OrderReasonOther%></textarea>
		    </td>
	    </tr>
	<%else   
	    Project= Application("DEFAULT_CONTAINER_ORDER_PROJECT")
	    Job= Application("DEFAULT_CONTAINER_ORDER_JOB")
	    OrderReason =  Application("DEFAULT_CONTAINER_ORDER_REASON")  '1 means Insuficient Qty %>
	    
	    <input TYPE="hidden" NAME="Project" Value=<%=Project%>>
	    <input TYPE="hidden" NAME="Job" Value=<%=Job%>>
	    <input TYPE="hidden" NAME="OrderReason" Value=<%=OrderReason%>>
    <%end if %>
	<tr height="25">
		<td align="right" valign="top">
			<span title="Select the owner of the order">Owner:</span>
		</td>
		<td>
		<%=ShowSelectBox2("iOwnerID", OwnerID, "SELECT owner_id AS Value, owner_id AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_owners", 27, RepeatString(43, "&nbsp;"), "")%>
	</tr>
<!-- MCD: end changes -->
</tr>
</table>
<p><span class="GUIFeedback">Specify the type, size and initial amount of containers to be created</span></p>
<table border="0">
	<tr>
		<th>Container</th>
		<th>Qty</th>
		<th>CAS #</th>
		<th>Chemical Name</th>
		<th>Supplier</th>
		<th>Size</th>
		<th>Catalog Num</th>
		<th><span class="required">Price</span></th>
		<th>Currency</th>
		<th><span class="required">Type</span></th>
		<th><span class="required">Size</span></th>
		<th><span class="required">UOM</span></th>
		<th><span class="required">Category</span></th>
		<%
		if NOT (UCase(Application("IMPORT_CONTAINER_PICKLIST_1_FIELD")) = "NULL" OR Application("IMPORT_CONTAINER_PICKLIST_1_FIELD") = "") then
			' First Custom PickList
			Response.Write "<th>" & Application("IMPORT_CONTAINER_PICKLIST_1_TITLE") & "</th>"
		end if
		if NOT (UCase(Application("IMPORT_CONTAINER_PICKLIST_2_FIELD")) = "NULL" OR Application("IMPORT_CONTAINER_PICKLIST_2_FIELD") = "") then		
			' Second Custom PickList
			Response.Write "<th>"  & Application("IMPORT_CONTAINER_PICKLIST_2_TITLE") & "</th>"
		end if
		%>
	</tr>
<%
If lRS.BOF AND lRS.EOF then
	Response.Write "Error: No rows returned"
Else
	lRS.MoveFirst
	n = 0
	'-- use form values not lRS ones if the form has been submitted, eg, from the postdata function
	PriceList = Request("PriceList")
	if PriceList <> "" then
		PriceList_arr = split(PriceList,",")
		ContainerTypeIDList = Request("ContainerTypeIDList")
		ContainerTypeIDList_arr = split(ContainerTypeIDList, ",")
		ContainerSizeList = Request("ContainerSizeList")
		ContainerSizeList_arr = split(ContainerSizeList, ",")
		UOMIDList = Request("UOMIDList")
		UOMIDList_arr = split(UOMIDList, ",")
	end if
	Do While NOT lRS.EOF
		n = n + 1
		packID = lRS("packageID").value
		size = lRS("SIZE").value
		blank = InStr(1, size," ")
		sizeUnit = lRS("Container_UOM_ID_FK").value 
		sizeValue = lRS("Container_Qty_Max").value
		Container_Count = lRS("Container_Count").value
		DUNS=lRS("DUNS").value
		UNSPSC=lRS("UNSPSC").value
		'D Deshmukh added for iProc integration 15-Feb-2010
		DDCatalogNum = lRS("Catalog2Num")
		if len(trim(DDCatalogNum))=0 then
			DDCatalogNum = lRS("CatalogNum")
		end if
		'D Deshmukh end of additions
		if IsNull(Container_Count) then Container_Count = 1
		Qty = Qty_dict.Item(Cstr(packID)) 
		Response.Write "<tr>"
		Response.Write "<td align=center>" & n & "</td>"
		Response.Write "<td>" & Qty 
		totalQty = totalQty + Qty
		Response.Write "<input type=hidden name=""PackageIDList"" value=""" & PackID & """>"
		Response.Write "<input type=hidden name=""QtyList"" value=""" & Qty & """>"
		Response.Write "<input type=hidden name=""DunsList"" value=""" & Duns & """>"
		Response.Write "<input type=hidden name=""UNSPSCList"" value=""" & UNSPSC & """>"
		'D Deshmukh added for iProc integration 15-Feb-2010
		Response.Write "<input type=hidden name=""DSDCatalogNumList"" value=""" & DDCatalogNum & """>"
		'D Deshmukh end of additions
		Response.Write "</td>"
		Response.Write "<td align=right>" & lRS("CAS") & "</td>"
		'--D Deshmukh 11-Feb-2010: Changing display to eliminate truncation; Original line below followed by modified line
		'Response.Write "<td align=right>" & truncateInSpan(lRS("prodName"),20, "prodName") & "</td>"
		Response.Write "<td align=right>" & lRS("prodName") & "</td>"
		Response.Write "<td align=right>" & lRS("ShortName") & "</td>"
		Response.Write "<td align=right>" & lRS("Size") & "</td>"
		'--D Deshmukh 11-Feb-2010: Changing display to Catalog2Num; Original line below followed by modified lines
		'Response.Write "<td align=right>" & lRS("CatalogNum") & "</td>"
		DDCatalogNum = lRS("Catalog2Num")
		if len(trim(DDCatalogNum))<>0 then
			Response.Write "<td align=right>" & lRS("Catalog2Num") & "</td>"
		else
			Response.Write "<td align=right>" & lRS("CatalogNum") & "</td>"
		end if
		'--Real time price from Sigma Aldrich 
		RTP=""
		if (lRS("ShortName")="Aldrich") or (lRS("ShortName")="CNH Tech") or (lRS("ShortName")="Fluka") or (lRS("ShortName")="Genosys") or (lRS("ShortName")="IVD") or (lRS("ShortName")="RDH") or (lRS("ShortName")="RCLSALOR") or (lRS("ShortName")="Sigma") or (lRS("ShortName")="Sigma-Aldrich Brand") or (lRS("ShortName")="Supelco") or (lRS("ShortName")="Trinity") Then
    		RTP=GetRealtimeprice(lRS("Catalog2Num"),Qty,sizeUnit)
		end if
		'-- use form values not lRS ones if the form has been submitted, eg, from the postdata function
		if PriceList <> "" then
			Price = trim(PriceList_arr(n-1))
			UOCostIDList = Request("UOCostIDList" & packID)
			ContainerTypeID = trim(ContainerTypeIDList_arr(n-1))
			if (n-1) <= ubound(ContainerSizeList_arr) then
				ContainerSize = trim(ContainerSizeList_arr(n-1))
			else
				ContainerSize = ""
			end if
			UOMID = trim(UOMIDList_arr(n-1))
		else
			Price = lRS("Price")
			UOCostIDList = 6
			ContainerTypeID = 1
			ContainerSize = sizeValue
			UOMID = sizeUnit
		end if
		'DGB Note that there are packages in ACX that don not have a valid size, unit or price. 
		'By allowing the price, size and unit to be overwritten by the user we avoid halting the
		'process. 
		'MCD: these two lines
		'Response.Write "<td>" & lRS("Price")
		'Response.Write "<input type=hidden size=5 maxlength=5 name=""PriceList"" value=""" & lRS("Price") & """></td>"
		'MCD: instead of this one
		if RTP<>""  then
		Response.Write "<td align=right><input type=""text"" size=7 name=""PriceList"" value="""  & RTP & """ style=""font-style:inherit; font-weight:bold; color:Blue; face:verdana""></td>"
		else
		Response.Write "<td align=right><input type=""text"" size=7 name=""PriceList"" value="""  & Price & """></td>"
		end if
		'MCD: and these two
		'Response.Write "<td align=center>" & GetListFromSQLRow("SELECT Unit_Name AS DisplayText FROM inv_units WHERE Unit_id = 23")
		'Response.Write "<input type=hidden name=""UOCostIDList" & packID & """ value=""" & 23 & """></td>"
		'MCD: instead of this one
		Response.Write "<td>" & ShowSelectBox("UOCostIDList" & packID, UOCostIDList,"SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 5 ORDER BY lower(DisplayText) ASC") & "</td>"
		'MCD: end changes
		Response.Write "<td>" & ShowSelectBox("ContainerTypeIDList" , ContainerTypeID,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>"
		'MCD: these two lines
		'Response.Write "<td>" & sizeValue
		'Response.Write "<input type=hidden size=5 maxlength=5 name=""ContainerSizeList"" value=""" & sizeValue & """></td>"
		'MCD: instead of this one
		Response.Write "<td><input type=text size=5 maxlength=5 name=""ContainerSizeList"" value=""" & ContainerSize & """></td>"
		'MCD: and these two
		'Response.Write "<td align=center>" & GetListFromSQLRow("SELECT Unit_Abreviation FROM inv_units WHERE Unit_id =" & sizeUnit)
		'Response.Write "<input type=hidden name=""UOMIDList"" value=""" & sizeUnit & """></td>"
		'MCD: instead of this one
		Response.Write "<td>" & ShowSelectBox("UOMIDList" , UOMID,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC") & "</td>"
		'MCD:end changes
		Response.Write "<td>" & ShowSelectBox("CategoryList" ,Category,"SELECT picklist_display AS Value, picklist_display AS DisplayText FROM inv_picklists WHERE picklist_domain=499 ORDER BY lower(picklist_code) ASC") & "</td>"
		'MCD: Removed
		'Response.Write "<td align=right><input type=text size=10 maxlength=10 name=""InitialAmountList"" value=""" & sizeValue & """></td>"
		'MCD: end changes
		Response.Write "<input type=hidden name=""ContainerCountList"" value=""" & Container_Count & """></td>"
		
		if NOT (UCase(Application("IMPORT_CONTAINER_PICKLIST_1_FIELD")) = "NULL" OR Application("IMPORT_CONTAINER_PICKLIST_1_FIELD") = "") then		
			' First Custom PickList
			Response.Write "<td>" & ShowSelectBox(Application("IMPORT_CONTAINER_PICKLIST_1_FIELD") &  "_List", "", Application("IMPORT_CONTAINER_PICKLIST_1_SQL")) & "</td>"
		end if
		if NOT (UCase(Application("IMPORT_CONTAINER_PICKLIST_2_FIELD")) = "NULL" OR Application("IMPORT_CONTAINER_PICKLIST_2_FIELD") = "") then		
			' Second Custom PickList
			Response.Write "<td>" & ShowSelectBox(Application("IMPORT_CONTAINER_PICKLIST_2_FIELD") &  "_List", "", Application("IMPORT_CONTAINER_PICKLIST_2_SQL")) & "</td>"
		end if
		Response.Write "</tr>" 	
		lRS.MoveNext
	Loop
	Response.Write "<tr><td><input type=hidden name=""totalQty"" value=""" & totalQty & """></td></tr>"
End if

%>
	<tr>
		<td colspan="10" align="right">
			<br> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>
</form>
<%end if%>
</body>
</html>
<%If Not IsEmpty(Request("setFocus")) Then%>
<script language="JavaScript">
<!--Hide JavaScript
	var a = document.all.item("<%=Request("setFocus")%>");
	if (a!=null) {
	    document.all.<%=Request("setFocus")%>.focus();
	}
//-->
</script>
<%End If%>