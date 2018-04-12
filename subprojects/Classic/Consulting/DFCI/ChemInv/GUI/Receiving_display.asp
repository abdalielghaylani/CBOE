<%@ EnableSessionState=True Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<%
if not Session("IsFirstRequest") then 
	StoreASPSessionID()
else
	Session("IsFirstRequest") = false
	Response.Write("<html><body onload=""window.location.reload()"">&nbsp;<form name=""form1""></form></body></html>")	
end if

CSUserName=ucase(Session("UserName" & "cheminv")) 
CSUserID= Session("UserID" & "cheminv")

if CSUserName = "" or CSUserID = "" then
	response.write "<BR>No credentials found."
	response.end
end if
%>

<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Recieve Ordered Containers</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript">

function ReceiveContainer(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),Session("UserName" & "cheminv")))%>"
	var containerID = elm.name.substring(4,elm.name.length);
	var barcode = elm.value;
	var httpResponse ="";
	var deliveryLocationID = document.all["DELL" + containerID].value;
	var containerStatusID = "1";
	var msg="Error - ";
	var barcoderesult="BARCODE EXISTS";
	var updateresult=false;
	var costresult;
	var newcost="";
	var newqty="";
	var updatecontainer=false;
	var err=0;
	var httpResponse;
	//add synonym if necessary
	if (barcode.length>0) {
	var strURL = "http://" + serverName + "/cheminv/api/DFCI_CreateSynonym.asp?compoundID=" + document.all["COMP" + containerID].value + "&SubstanceName=" + barcode; 	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP1= " + httpResponse);
	}

	if (httpResponse>0) {
		barcoderesult="BARCODE ADDED";
	}
	//update qty
	
	httpResponse=0;

	if ( document.all["PACK" + containerID].value * document.all["NUMP" + containerID].value == document.all["TOTA" + containerID].value)
	{
	updateresult="QTY UNCHANGED";
	qtym=document.all["TOTA" + containerID].value;
	} else
	{
	var qtyr= document.all["PACK" + containerID].value * document.all["NUMP" + containerID].value;
	var qtym; 
	if (document.all["PACK" + containerID].value * document.all["NUMP" + containerID].value==0)
	{
	qtym=1;
	} 
	else
	{
	qtym=document.all["PACK" + containerID].value * document.all["NUMP" + containerID].value;
	}
	newqty="qty_max%3D" + qtym + "::qty_initial%3D" + qtyr + "::qty_remaining%3D" + qtyr + "::qty_available%3D" + qtyr
	updateresult="QTY CHANGED";
	updatecontainer=true;
	} 
	
	//update cost per unit
	//check to see if cost has been updated.  If not assume it has not changed
if ( document.all["COST" + containerID].value == document.all["OCOS" + containerID].value)
{
costresult=", COST UNCHANGED";
newcost="";
//document.all["COST" + containerID].value=document.all["COST" + containerID].value * qtym / document.all["TOTA" + containerID].value;
updatecontainer=true;
} else
{
costresult=", COST CHANGED"
updatecontainer=true;
}

updateresult+=costresult;
if (updatecontainer) { // always recalculate costs if qty or the cost changes.
if (newqty=="") {
newcost= "container_cost%3D" + (document.all["COST" + containerID].value*document.all["NUMP" + containerID].value /qtym );
} else {
newcost= "::container_cost%3D" + (document.all["COST" + containerID].value*document.all["NUMP" + containerID].value/qtym );
}
}



var pkgLotNum = "";
if (!document.all["LOTN" + containerID].disabled && document.all["LOTN" + containerID].value.length>0 ) {
if (updatecontainer) {
pkgLotNum = "::";
}
pkgLotNum += "lot_num%3D'" + document.all["LOTN" + containerID].value+"'";
updatecontainer=true;
}

//Validation steps

	if (isNaN(document.all["NUMP" + containerID].value)) {
	msg="Order amount not valid number - ";
	err=4;
	}
	if (document.all["NUMP" + containerID].value>Math.floor(document.all["NUMP" + containerID].value)) {
	msg="Order amount must be a whole number - ";
	err=3;
	}

	if (document.all["NUMP" + containerID].value<0) {
	msg="Order amount must be a positive number - ";
	err=2;
	}

	if (isNaN(document.all["COST" + containerID].value)) {
	msg="Per unit cost must be entered - ";
	err=5;
	} 

	if (document.all["COST" + containerID].value.length==0) {
	msg="Per unit cost must be entered - ";
	err=5;
	} 

	if (updatecontainer && err==0 ) {
	var strURL = "http://" + serverName + "/cheminv/api/DFCI_UpdateContainer2.asp?containerIDs=" + containerID + "&VALUEPAIRS=" + newqty + newcost + pkgLotNum; 	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;


	var httpResponse = JsHTTPGet(strURL);
//	alert(strURL);
//	alert("update" + httpResponse);
//	document.write(strURL);
	}
	//record record of transaction
if (document.all["RECS" + containerID].value=="Put Away" && err==0) {
// if status is ok
containerStatusID = "1"; // for available
} else {
containerStatusID = "3"; //for on order
deliveryLocationID = "1"; // for on order	
}
if (err==0) {
	var strURL = "http://" + serverName + "/cheminv/api/DFCI_CheckOutContainer.asp?action=in&iField_9=" + document.all["COMM" + containerID].value + "&iField_10=" + document.all["RECS" + containerID].value + "&containerID=" + containerID + "&LocationID=" + deliveryLocationID + "&iField_2=" + barcode + "&containerStatusID=" + containerStatusID + "&iFIELD_1=" + barcoderesult + "&iField_3=" + document.all["NUMP" + containerID].value + "&iField_4=" + updateresult + "&iField_5=" + document.all["TOTA" + containerID].value ; 	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	var httResponse = JsHTTPGet(strURL);	
	//alert("CHECKIN JSHTTP= " + httpResponse);
} else
 {
	httpResponse = -400;
}



//	alert("receiving! JSHTTP= " + httpResponse);
	document.all["status_" + containerID].style.fontWeight="bold"
	if (httpResponse >= 0){
		//change the bkgrnd color and lock the input box
		elm.style.backgroundColor="lightyellow";
		elm.disabled = true;
		document.all["NUMP" + containerID].disabled=true;
		document.all["COMM" + containerID].disabled=true;
		document.all["COST" + containerID].disabled=true;
		document.all["LOTN" + containerID].disabled=true;
		if (document.all["RECS" + containerID].value=="Put Away") {
		document.all["status_" + containerID].innerHTML="Received";	
		document.all["status_" + containerID].style.color="green";
		} else {
		document.all["status_" + containerID].innerHTML="Logged, Requires Attention";	
		document.all["status_" + containerID].style.color="red";
}
		document.all["status_" + containerID].title = "Container has been succesfully received."
	}
	else{
		if(httpResponse == -102) {
			msg = "A container with same barcode already exists: " + barcode;
		}
		 if (httpResponse == -128){
			msg = "The designated delivery location does not allow containers of this type.  \rThis container must be received by the adminstrator.";
		}
		if (httpResponse==-400) {
		msg=msg + "Not Put Away";
		}
		document.all["status_" + containerID].innerHTML= msg;
		document.all["status_" + containerID].style.color="red"
	
	}

}


function doIt(){
	var s="";
	var elms = document.form1.elements;
	for (var i=0;i<elms.length; i++){
	if ((elms[i].name.substr(0,4)=="CONT")) {
		if ((elms[i].value.length > 0  || document.all["RECS" + elms[i].name.substring(4,elms[i].name.length)].value=="Problem" ) && (!elms[i].disabled)) ReceiveContainer(elms[i]);
		}	
	}
}


function changeField(inputName){
top.frames['ActionFrame'].addHiddenField(inputName, document.getElementById(inputName).value);
}

</script>


</head>
<body onLoad="">
<%
Dim Conn
Dim Cmd
Dim RS

bDebugPrint = false
bWriteError = False
strError = "Error:Receiving_display.asp<BR>"

containerID = Request("containerID")
if containerID = "" then containerID = NULL
dateFormatString = Application("DATE_FORMAT_STRING")
CurrentUserIDList = request("CurrentUserIDList")
DeliverToLocationID = request("DeliverToLocationID")
fromDate = request("fromDate")
toDate = request("toDate")
SupplierID = request("SupplierID")
if supplierID = "" then supplierID = Null
CAS = replace(request("CAS"),"*","%")
ACXID = replace(request("ACXID"),"*","%")
SupplierCatNum = replace(request("SupplierCatNum"),"*","%")
SubstanceName = replace(request("SubstanceName"),"*","%")
Barcodelist=request("Barcodelist")
PONumber = request("PONumber")
Barcode=request("Barcode")

if replace(barcodelist,"|","")="" then
	barcodelist=null
else
barcodeList="'" & replace(replace(barcodelist,"|",","),",", "','") & "'"
end if

if CurrentUserIDList = "" then 
	CurrentUserIDList = NULL
else
	CurrentUserIDList = replace(CurrentUserIDList, ",", "','")
	CurrentUserIDList  = "'" & CurrentUserIDList  & "'" 
end if	
if DeliverToLocationID = "" then DeliverToLocationID =0
if fromDate = "" then
	fromDate = "NULL"
Elseif IsDate(fromDate) then
	fromDate = GetOracleDateString3(fromDate)
Else
	strError = strError & "From Date could not be interpreted as a valid date<BR>"
	bWriteError = True
End if
if toDate = "" then
	toDate = "NULL"
Elseif IsDate(toDate) then
	toDate = GetOracleDateString2(toDate)
Else
	strError = strError & "To Date could not be interpreted as a valid date<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if



Response.Expires = -1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetOnOrderContainers(?,?,?,?,?,?,?,?,?,?,?,?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("pContainerID",131, 1, 0, containerID)
Cmd.Parameters.Append Cmd.CreateParameter("pDestinationLocationID",131, 1, 0, DeliverToLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PFROMDATE",200, 1, 200, FromDate)
Cmd.Parameters.Append Cmd.CreateParameter("PTODATE",200, 1, 200, ToDate)
Cmd.Parameters.Append Cmd.CreateParameter("pCurrentUserIDList",200, 1, 4000, CurrentUserIDList)
Cmd.Parameters.Append Cmd.CreateParameter("pSupplierID",131, 1, 0, SupplierID)
Cmd.Parameters.Append Cmd.CreateParameter("pSupplierCatNum",200, 1, 50, SupplierCatNum)
Cmd.Parameters.Append Cmd.CreateParameter("pCAS",200, 1, 15, CAS)
Cmd.Parameters.Append Cmd.CreateParameter("pACXID",200, 1, 15, AcxID)
Cmd.Parameters.Append Cmd.CreateParameter("pSubstanceName",200, 1, 255, SubstanceName)
Cmd.Parameters.Append Cmd.CreateParameter("pPONumber",200, 1, 50, PONumber)
Cmd.Parameters.Append Cmd.CreateParameter("pPONumber",131, 1, 0, POLineNumber)
Cmd.Parameters.Append Cmd.CreateParameter("PReqNumber",200,1,50, ReqNumber)
Cmd.Parameters.Append Cmd.CreateParameter("pBarcodeList",200, 1, 4000, BarcodeList)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
Else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE

'Response.Write RS.getString()
'Response.End

caption = "Find the container(s) you wish to receive, afix a pre-printed barcode to each, scan into the box(es) below, and click OK button to submit changes."
disable2 = "disabled"

If (RS.EOF AND RS.BOF) then
	Response.Write ("<BR><BR><span class=""GUIFeedback"">&nbsp;&nbsp;&nbsp;No matching containers found in the ""On Order"" location</Span>")
	Response.end
End if

'START DFCI Change to show if a barcode was found when you scan it.
if not barcodelist="" then 
for each barcodetemp in Split(barcodelist,",")
	if barcodetemp<>"''" then
	lastBarcode = replace(barcodetemp,"'","")
	end if
next
barcodeCount = 0
while (Not RS.EOF)
if lastBarcode = rs("rbarcode") then
	barcodeCount = barcodeCount + 1
end if
rs.movenext
wend

if barcodeCount = 0 then 
	Response.Write ("<BR><BR><span class=""GUIFeedback"">&nbsp;&nbsp;&nbsp;No matching containers found in the ""On Order"" location for barcode " & lastBarcode & "</Span>")
else 
	Response.Write ("<BR><BR><span class=""GUIFeedback"">&nbsp;&nbsp;&nbsp; " & barcodeCount & " matching container(s) found in the ""On Order"" location for barcode " & lastBarcode & "</Span>")
end if
end if
RS.MoveFirst

'END DFCI Change
%>


	<br>
	<p><span class="GUIFeedback"><%=Caption%></span></p>
	<form name="form1" action="deliver_request_action.asp" method="POST">
	<table border="1">
	<tr>
		<td colspan="14" align="center">
			<a href="#" onclick="doIt()"><img border="0" src="../graphics/ok_dialog_btn.gif"></a>
		</td>
	</tr>
	<tr>
		<th>
			<span title="Afix a pre-printed barcode to the container and scan it to receive it">Scan Barcode</span>
		</th>
		<th>
			Order Status
		</th>
		<th>
			Comments
		</th>
		<th> Lot Num</th>
		<th>
			Receiving Status
		</th>
		<th>
			Delivery Location
		</th>
		<th>
			Date Created
		</th>
		<th>
		Package Desc.
		</th>
		<th>
			Packages
		</th>
		<th>Package Cost $</th>
		<th>
			Drug Name
		</th>
		<th>
			NDC
		</th>
		<th>
			ABC ID
		</th>
		<th>
			Tot. Disp. Units
		</th>
		
		<!---		<th>			PO LineNum		</th>		<th>			Req Number		</th>		--->
	</tr>	
<%	
		While (Not RS.EOF)
			ContainerID = RS("container_id").value 
			UserID = RS("current_user_id_fk").value
			UserName = RS("UserName").value
			SupplierName = RS("supplier_name").value
			DeliveryLocation = RS("DeliveryLocation").value
			DeliveryLocationID = RS("DeliveryLocationID").value
			DeliveryPath = RS("DeliveryPath").value
			size = RS("csize").value
			SupplierCatNum = RS("supplier_catnum").value
			DateCreated = RS("date_created").value
			PoNumber = RS("po_number").value
			PoLineNumber = RS("Po_Line_Number").value
			ReqNumber = RS("req_number").value
			cas = RS("cas").value
			SubstanceName = RS("Substance_Name").value
			ACXID = RS("ACX_ID").value
			cBarcode = rs("barcode")
			compoundid= rs("Compound_Id")
			packsize=rs("packsize")
			maxqty=rs("qty_max")
			rBarcode=rs("rBarcode")
			packdescription=rs("package_size")
			unit_cost=rs("price")
			drugType=rs("Alt_id_5")	
			LotNum = rs("Lot_Num")			
%>
			<span id="R<%=ContainerID%>" style="background-color:green"> 
			
			<tr>
				<td align="left"> 
					<input type="text" name="CONT<%=ContainerID%>" id="<%=DeliveryLocationID%>" value="<%	
					if rBarcode<>"" then 
					response.write rBarcode
					end if
					%>" size="10" width="50"> 
		
					<input type="hidden" name="CBAR<%=containerId%>" value="<%=cBarcode%>">
					<input type="hidden" name="NBAR<%=containerId%>" value="<%
					'if the barcode does not come prom the db, it is new and needs to be added
					if rBarcode="" then 
					response.write "new"
					else
					response.write "old"
					end if
					%>">	
					<input type="hidden" name="COMP<%=containerId%>" value="<%=compoundId%>">
					
				</td>
				<td align="center">
				<% 'DFCI check to see if there is a variable for this status, otherwise default to put away.
						if request("RECS"&containerid)="" then
						tempVal="Put Away"
						else 
						tempVal=request("RECS"&containerid)
						end if			


				

				%> 
				<%=ShowSelectBox3("RECS" & containerid, tempVal, "SELECT Picklist_display AS Value,  Picklist_display AS DisplayText FROM inv_picklists where picklist_domain=100 ORDER BY picklist_id ASC", 20, RepeatString(18, "&nbsp;"), "", "changeField('RECS"&ContainerId&"');")%>
				</td>
				<td align="center">
				<input type="text" name="COMM<%=containerId%>" id="COMM<%=containerId%>" value="<%=request("COMM"&ContainerId) %>" onchange="changeField('<%="COMM"&COntainerId%>');">	
				</td>
				<td align="center">
				<input type="text" name="LOTN<%=containerId%>" id="LOTN<%=containerId%>" value="<% if request("LOTN"&containerid)="" then 
						tempVal=LotNum
						else
						tempVal=request("LOTN"&containerid)
						end if			
%>" onchange="changeField('<%="LOTN"&COntainerId%>');"
<%
' Disable lot num for commercial drugs.
if drugType="1000" then
response.write "disabled"
end if

%>
>	
				</td>
				<td align="center">
					<span title id="status_<%=ContainerID%>">On Order</span>	
				</td>
				
				<td align="center"> 
					<span title="<%=DeliveryPath%>"><%=DeliveryLocation%>
				<input type="hidden" name="DELL<%=containerID%>" value="<%=DeliveryLocationId%>">
</span> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(DateCreated, 12, "")%> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(packdescription, 12, "")%> 
				</td>
					<% 'DFCI Add need temp value
						if request("NUMP"&containerid)="" then
						tempVal=cdbl(maxqty)/cdbl(packsize)
						else
						tempVal=request("NUMP"&containerid)
						end if			


					%>
				<td align="right"><input type=text name="NUMP<%=containerid%>" id="NUMP<%=containerid%>" value="<%=tempVal%>" size=8 onchange="changeField('<%="NUMP"&COntainerId%>');">
				<input type="hidden" name="PACK<%=containerid%>" value="<%=packsize%>">
				<input type="hidden" name="TOTA<%=containerid%>" value="<%=maxqty%>">
				</td>
				<td align="center">
					<% 'DFCI Add need temp value
						if request("COST"&containerid)="" then
						tempVal=unit_cost
						else
						tempVal=request("COST"&containerid)
						end if			


					%>
					<input type="text" size="10" name="COST<%=containerid%>" id="COST<%=containerid%>" value="<%=tempVal%>" onchange="changeField('<%="COST"&COntainerId%>');">
					<input type="hidden" name="OCOS<%=containerid%>" value="<%=unit_cost%>">
				</td>
				<td align="center">
					<%=TruncateInSpan(SubstanceName, 20, "")%>
				</td>
				<td align="center">
					<%=TruncateInSpan(CAS,20,"")%>
				</td>
				<td align="center">
					<%=TruncateInSpan(ACXID,20,"")%>
				</td>
				<td align="left">
					<%=TruncateInSpan(Size,10,"")%>
				</td>
				<!---				<td align="center">					<%=TruncateInSpan(POLineNumber, 15, "")%>					</td>				<td align="center">					<%=TruncateInSpan(ReqNumber, 15, "")%>					</td>				--->
			</tr>
			</span>
			<%rs.MoveNext
		Wend%>
			<tr>
				<td colspan="14" align="center">
					<a href="#" onclick="doIt();"><img border="0" src="../graphics/ok_dialog_btn.gif"></a>
				</td>
			</tr>
	</table>
<%	

	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing
End if
%>
</form>
</body>
</html>



