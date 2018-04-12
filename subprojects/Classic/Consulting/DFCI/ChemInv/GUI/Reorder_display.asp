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
<title><%=Application("appTitle")%> -- Reorder Containers</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript">

function Deliver(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),Session("UserName" & "cheminv")))%>"
//	alert(elm.name.substring(4,elm.name.length));
	var ID = elm.name.substring(4,elm.name.length);
	var locationid = document.all["LOCA" + ID].value;
	var compoundid = document.all["COMP" + ID].value;
	var amount = document.all["NEWA" + ID].value * document.all["CONV" + ID].value;
	var UOMID = document.all["UOMI" + ID].value;
	var PONumber = document.all["PONumber"].value;
	var ContainerName = document.all["CONN" + ID].value;
	var httpResponse ="";
	var DueDate = document.all["DUED" + ID].value;
	
	var msg="";

//Validation steps
	var valErr = 0;
	
	if (isNaN(amount)) {
	msg="Order amount not valid number";
	valErr=1;
	}
	if (document.all["NEWA" + ID].value>Math.floor(document.all["NEWA" + ID].value)) {
	msg="Order amount must be a whole number";
	valErr=1;
	}

	if (amount<0) {
	msg="Order amount must be a positive number.";
	valErr=1;
	}
	

	if (document.all["Approve"].checked && amount>0 && valErr==0) {

	var strURL = "http://" + serverName + "/cheminv/api/DFCI_OrderContainer.asp?compoundID=" + compoundid + "&LocationId=1"; 	
	strURL=strURL + "&qtymax=" + amount + "&UOMID=" + UOMID + "&containerTypeId=6&containerstatusid=3&PONumber=" + PONumber;
	strURL=strURL + "&containercost=1&Project=1&Job=1&DeliveryLocationId=" + locationid + "&OrderReason=1&ContainerName="+ContainerName+"&DueDate=" + DueDate + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	//alert(strURL);
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);

	
	document.all["status_" + ID].style.fontWeight="bold";
	if (httpResponse > 0){
		//change the bkgrnd color and lock the input box
		elm.style.backgroundColor="lightyellow";
		elm.disabled = true;
		document.all["NEWA" + ID].disabled=true;
		document.all["status_" + ID].innerHTML="Ordered";	
		document.all["status_" + ID].style.color="green";
		document.all["status_" + ID].title = "Item has been ordered.";
	}
	else{	
		document.all["status_" + ID].innerHTML="Error";
		document.all["status_" + ID].style.color="red";
	}
} 

else {
if (valErr>0) {
		document.all["status_" + ID].innerHTML=msg;
		document.all["status_" + ID].style.color="red";
	}
}



}

function doIt(){
	var PONumber = document.all["PONumber"].value;
	if (PONumber.length>0) {
	var s="";
	var elms = document.form1.elements;
	for (var i=0;i<elms.length; i++){
	if ((elms[i].name.substr(0,4)=="PARL")) {
		if ((elms[i].value.length > 0) && (!elms[i].disabled)) Deliver(elms[i]);
		}	
	}
	} else
	{
	alert("You must supply a PO Number to order!");
	}
}

</script>


</head>
<body>

<%
Dim Conn
Dim Cmd
Dim RS



bDebugPrint = false
bWriteError = False
strError = "Error:Receiving_display.asp<BR>"

locationID = Request("ilocationID")
if locationID = "" then locationID = NULL

NDC = Request("NDC")
if NDC = "" then NDC = NULL

BumpUp = Request("BumpUp")

if BumpUP = "" then bumpUp=0

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Response.Expires = -1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.DFCI_GetReorderAmounts(?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("pLocationID",131, 1, 0, LocationID)
Cmd.Parameters.Append Cmd.CreateParameter("pNDC",200, 1, 50, NDC)
Cmd.Parameters.Append Cmd.CreateParameter("pBumpUp",131, 1, 0, bumpUp)

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

caption = "Update the reorder amounts in the grid, and check the order box for each item to be ordered. Check the order Checked Items box to enable the transfer.Press OK to submit the changes:"
disable2 = "disabled"

If (RS.EOF AND RS.BOF) then
	Response.Write ("<BR><BR><span class=""GUIFeedback"">&nbsp;&nbsp;&nbsp;No matching combinations found</Span>")
	Response.end
End if
%>


	<br>
	<p><span class="GUIFeedback"><%=Caption%></span></p>
	<form name="form1" action="deliver_request_action.asp" method="POST">
	<table border="1">
	<tr> <td colspan=10> 
	PO Number <input type=text maxsize=20 size=20 name="PONumber">
	Approve Order <input type=checkbox name="Approve" value="Yes">
			<tr>
				<td colspan="13" align="center">
					<a href="#" onclick="doIt();"><img border="0" src="../graphics/ok_dialog_btn.gif"></a>
				</td>
			</tr>

<%		ic=0

		While (Not RS.EOF)
			ACXId = RS("ACX_ID").value
 			NDC = RS("NDC").value
			CompoundId = Rs("Compound_Id")
			CompoundName = Rs("CompoundName") 
			LocationName = RS("LocationName").value
			LocationId = RS("Location_Id").value
			Amount = RS("Amount").value
			qtyrequired = RS("qty_required").value
			packagesize=RS("Package_Size").value
			containerqtymax=CDbl(RS("container_qty_max").value)
			containercount=CInt(RS("container_count").value)				
			uom=rs("unit_abreviation").value
			onhand=rs("onhand").value
			uomid=rs("container_uom_id_fk").value
			DUED = rs("DueDate").value
			OnOrder = rs("OnOrder").value
%>

<% if cdbl(qtyrequired) >0 then %>

<%					
			if ic=0 then %>

	<tr>
		<th>
			Replenishment Status
		</th>
		<th>
			Delivery Location
		</th>
		<th>
			ACX_ID
		</th>
		<th>
			NDC
		</th>
		<th>
			Drug Name
		</th>
		<th> Package Size (buy unit) </th>
		<th> Current </th>
		<th> On Order </th>
		<th>
			Par Level (sell units)
		</th>
		<th>
			Reorder Amount (buy units)
		</th>
		<th> Reorder Amount (dispense units) </th>
	
	</tr>	

			<%
 			end if		
			ic=ic+1
			if ic=9 then
			ic=0
			end if
%>
			<span id="R<%=ContainerID%>" style="background-color:green"> 
			
			<tr>
				<td align="left"> <span title id="status_<%=locationid& "." &compoundid%>"></span>
					<input type=hidden name="PARL<%=locationid& "." &compoundid%>" value="<%=locationid& "." &compoundid%>">
					<input type=hidden name="CONV<%=locationid& "." &compoundid%>" value="<%=containerqtymax*containercount%>">
					</td>	
				
				<td align="center">
					<%=LocationName%>
					<input type=hidden name="LOCA<%=locationid& "." &compoundid%>" value="<%=LocationId%>">
				</td>

				<td align="center">
					<%=AcxId%>
					<input type=hidden name="ACXI<%=locationid& "." &compoundid%>" value="<%=LocationId%>">
				</td>				
				<td align="center">
					<%=NDC%>
				<input type=hidden name="NDCC<%=locationid& "." &compoundid%>" value="<%=NDC%>">
				</td>
				<td align="center">
					<%=CompoundName%>
					<input type=hidden name="COMP<%=locationid& "." &compoundid%>" value="<%=CompoundId%>">
					<input type=hidden name="CONN<%=locationid& "." &compoundid%>" value="<%=CompoundName%>">
					<input type=hidden name="UOMI<%=locationid& "." &compoundid%>" value="<%=uomid%>">
					<input type=hidden name="DUED<%=locationid& "." &compoundid%>" value="<%=DUED%>">
					
				</td>

				<td align="center">
				<%=PackageSize%>
				</td>
				<td align="center">
				<%= Cdbl(OnHand)/containerqtymax & " S.U.<BR>" & OnHand & " " & UOM%>
				</td>
				<td align="center">
				<%= Cdbl(OnOrder)/containerqtymax & " S.U. <BR>" & OnOrder & " " & UOM%>
				</td>
				<td align="center">
				<%=Amount%>
				</td>

				<td align="center">
					<input type=text name="NEWA<%=locationid& "." &compoundid%>" value="<%=Cdbl(qtyrequired)/containercount/containerqtymax%>">
				</td>
				<td align="center">
					<%=Cdbl(qtyRequired)/containerqtymax & " S.U.<BR>" & qtyRequired & UOM%>
				</td>
				</tr>

			</span>
<% end if %>
			<%rs.MoveNext
		Wend%>
			<tr>
				<td colspan="13" align="center">
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



