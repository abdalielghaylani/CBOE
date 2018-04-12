<%@ EnableSessionState=False Language=VBScript%>
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Recieve Ordered Containers</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript">

function ReceiveContainer(elm){
	var containerID = elm.name;
	var barcode = elm.value;
	var httpResponse ="";
	var deliveryLocationID = elm.id;
	var containerStatusID = "1";
	var msg="";
	
	var strURL = serverType + serverName + "/cheminv/api/ReceiveContainer.asp?containerID=" + containerID + "&deliveryLocationID=" + deliveryLocationID + "&containerStatusID=" + containerStatusID + "&Barcode=" + barcode; 	
	//alert(strURL);
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);
	document.all["status_" + elm.name].style.fontWeight="bold"
	if (httpResponse == 1){
		//change the bkgrnd color and lock the input box
		elm.style.backgroundColor="lightyellow";
		elm.disabled = true;
		document.all["status_" + elm.name].innerHTML="Received";	
		document.all["status_" + elm.name].style.color="green"
		document.all["status_" + elm.name].title = "Container has been succesfully received."
	}
	else{
		if(httpResponse == -102) {
			msg = "A container with same barcode already exists: " + barcode;
			alert(msg);
			document.all["status_" + elm.name].title = msg;
		}
		else if (httpResponse == -128){
			msg = "The designated delivery location does not allow containers of this type.  \rThis container must be received by the adminstrator.";
			alert(msg);
			document.all["status_" + elm.name].title = msg;
		}
		document.all["status_" + elm.name].innerHTML="Error";
		document.all["status_" + elm.name].style.color="red"
	}
}


function doIt(){
	var s="";
	var elms = document.form1.elements;
	for (var i=0;i<elms.length; i++){
	if ((elms[i].value.length > 0) && (!elms[i].disabled)) ReceiveContainer(elms[i]);	
	}
}

</script>


</head>
<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

CSUserName= Application("RECEIVING_USERNAME")
CSUserID= Application("RECEIVING_PWD")

if CSUserName = "" or CSUserID = "" then
	response.write "<BR>Access Error:<BR>RECEIVING_USERNAME and/or RECEIVING_PWD parameters could not be read from the [CHEMINV] section of cfserver.ini.<BR>  Please inform the system administrator."
	response.end
end if

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
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetOnOrderContainers(?,?,?,?,?,?,?,?,?,?,?,?,?)}", adCmdText)	
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
%>


	<br>
	<p><span class="GUIFeedback"><%=Caption%></span></p>
	<form name="form1" action="deliver_request_action.asp" method="POST">
	<table border="1">
	<tr>
		<td colspan="11" align="center">
			<a href="#" onclick="doIt()"><img border="0" src="../graphics/ok_dialog_btn.gif"></a>
		</td>
	</tr>
	<tr>
		<th>
			<span title="Afix a pre-printed barcode to the container and scan it to receive it">Scan Barcode</span>
		</th>
		<th>
			Status
		</th>
		<th>
			Ordered By
		</th>
		<th>
			Delivery Location
		</th>
		<th>
			Date Created
		</th>
		<th>
			Supplier
		</th>
		<th>
			CatalogNumber
		</th>
		<th>
			Substance Name
		</th>
		<th>
			CAS
		</th>
		<th>
			ACX ID
		</th>
		<th>
			Size
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
				
%>
			<span id="R<%=ContainerID%>" style="background-color:green"> 
			
			<tr>
				<td align="left"> 
					<input type="text" name="<%=ContainerID%>" id="<%=DeliveryLocationID%>" value size="10" width="50"> 
					
				</td>
				<td align="center">
					<span title id="status_<%=ContainerID%>">On Order</span>	
				</td>
				<td align="center"> 
					<%=TruncateInSpan(UserID, 15, "")%> 
				</td>
				<td align="center"> 
					<span title="<%=DeliveryPath%>"><%=DeliveryLocation%></span> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(DateCreated, 12, "")%> 
				</td>
				<td align="right"> 
					<%=TruncateInSpan(SupplierName,15,"")%>
				</td>
				<td align="center">
					<%=TruncateInSpan(SupplierCatNum,15,"")%>
				</td>
			
				<td align="center">
					<%=TruncateInSpan(SubstanceName, 20, "")%>
				</td>
				
				<td align="center">
					<%=TruncateInSpan(CAS,15,"")%>
				</td>
				<td align="center">
					<%=TruncateInSpan(ACXID,15,"")%>
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
				<td colspan="11" align="center">
					<a href="#" onclick="doIt()"><img border="0" src="../graphics/ok_dialog_btn.gif"></a>
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



