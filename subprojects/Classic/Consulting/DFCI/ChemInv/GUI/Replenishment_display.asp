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

function Carousel() {
	carouselWindow=window.open('','carouselWindow','toolbar=0,scrollbars=1,location=0,statusbar=0,menubar=1,resizable=1,width=500,height=800');
	var tmp = carouselWindow.document;
	var elms = document.form1.elements;
	for (var i=0;i<elms.length; i++){
	if (elms[i].name.substr(0,4)=="PARL") {
		if (elms[i].value.length > 0) {
		var ID = elms[i].name.substring(4,elms[i].name.length);
		var locationidsource = document.getElementById("LOCS" + ID).value;
		locationidsource=locationidsource.replace("*","");
		if (locationidsource.length<document.getElementById("LOCS" + ID).value.length && document.getElementById("DELI" + ID).checked) {
			//Carousel Item

			tmp.writeln(document.getElementById("LOCA" + ID).value + document.getElementById("LOCA" + ID).value + ',' + document.getElementById("LOCA" + ID).value + document.getElementById("LOCA" + ID).value + ',,,' + document.getElementById("NDCC" + ID).value + "," + document.getElementById("NEWA" + ID).value * document.getElementById("CONV" + ID).value + "<BR>")
		}
		
				
		}	
	}
	}
	carouselWindow.focus();
	tmp.close();
}

function Deliver(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),Session("UserName" & "cheminv")))%>"
	var ID = elm.name.substring(4,elm.name.length);
	var locationid = document.all["LOCA" + ID].value;
	var locationidsource = document.all["LOCS" + ID].value;
	if (locationidsource) {
	locationidsource=locationidsource.replace("*","");
	}
	var compoundid = document.all["COMP" + ID].value;
	var oldamount = document.all["OLDA" + ID].value;
	var amount = document.all["NEWA" + ID].value * document.all["CONV" + ID].value;
	var httpResponse ="";
	
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
	

//	alert(document.all["Approve"].checked +" " +"DELI" + ID + "-" + document.all["DELI" + ID].checked);
	if (document.all["Approve"].checked && document.all["DELI" + ID].checked) {

	var strURL = "http://" + serverName + "/cheminv/api/DFCI_BulkMove.asp?compoundID=" + compoundid + "&LocationIdSource=" + locationidsource + "&LocationId=" + locationid + "&qty=" + amount; 	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	//alert(strURL);
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);

	
	document.all["status_" + ID].style.fontWeight="bold";
	if (httpResponse >= 0){
		//change the bkgrnd color and lock the input box
		elm.style.backgroundColor="lightyellow";
		elm.disabled = true;
		document.all["NEWA" + ID].disabled=true;
		document.all["LOCS" + ID].disabled=true;
		document.all["DELI" + ID].disabled=true;
		document.all["status_" + ID].innerHTML="Delivered";	
		document.all["status_" + ID].style.color="green";
		document.all["status_" + ID].title = "Supply has been moved to Destination.";
	}
	else{
		switch(httpResponse) 
		{
			case "-9000":
			msg="No compound matches";
			break;
			case "-9001":
			msg="Multiple compound matches";
			break;
			case "-9005":
			msg="Insufficient supply";
			break;
			case "-9004":
			msg="No location match"
			break;
			default:
			msg="Error"
		}	
		document.all["status_" + ID].innerHTML=msg;
		document.all["status_" + ID].style.color="red";
	}
} else {
		document.all["status_" + ID].innerHTML=msg;
		//document.all["status_" + ID].style.color="red";
}

}

function doIt(){
	var s="";
	var elms = document.form1.elements;
	for (var i=0;i<elms.length; i++){
	if ((elms[i].name.substr(0,4)=="PARL")) {
		if ((elms[i].value.length > 0) && (!elms[i].disabled) && (!document.getElementById(elms[i].name.replace("PARL","DELI")).disabled)) Deliver(elms[i]);
		}	
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
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.DFCI_GetParLevelRequests(?,?,?)}", adCmdText)	
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

caption = "Update the request amounts in the grid, and check the deliver box for each item to be transfered. Use the Generate Carousel file to generate a list of items that have been checked off the list and are stored in the carousel.  Check the Deliver Checked Items box to enable the transfer.Press OK to submit the changes:"
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
	<tr>
		<td colspan="13" align="center">
		<a href="#" onclick="Carousel();" >Generate Carousel File For Checked Items </a><BR><BR>
		Deliver Checked Items <input type=checkbox name="Approve" value="Yes"> <br><BR>			
			<a href="#" onclick="doIt()"><img border="0" src="../graphics/ok_dialog_btn.gif"></a>
		</td>
	</tr>
	
<% 
			ic=0
			While (Not RS.EOF)							
 			NDC = RS("NDC").value
			CompoundId = Rs("Compound_Id")
			CompoundName = Rs("Substance_Name") 
			LocationName = RS("Location_Name").value & "-" & rs.index
			LocationId = RS("Location_Id").value
			Amount = RS("Amount").value
			qtyrequired = RS("qty_required").value
			packagesize=RS("Package_Size").value
			containerqtymax=CDbl(RS("container_qty_max").value)
			containercount=CInt(RS("container_count").value)				
			uom=rs("UOM").value
			onhand=rs("onhand").value
			mainsupplylocation = rs("main_supply_location")
			mainsupply = rs ("main_supply")
			mainsupplyname = rs("main_location_name")
			mainsupplytype = rs("main_location_type")

			rowId=locationid& "." &compoundid

			if mainsupplylocation<>"" then 
			RowId = RowId & "." & mainsupplylocation
			else
			RowId = RowId & ".-1"
			end if


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
			Source Location
		</th>
		<th>
			NDC
		</th>
		<th>
			Drug Name
		</th>
		<th> Package Size (buy unit) </th>
		<th> Current </th>
		<th>
			Par Level (sell units)
		</th>
		<th>
			Requested Amount (sell units)
		</th>
		<th> Requested Amount (dispense units) </th>
		<th> Deliver? </th>
	
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
				<td align="left"> <span title id="status_<%=rowId%>"></span>
					<input type=hidden name="PARL<%=rowId%>" value="<%=rowId%>">
					<input type=hidden name="CONV<%=rowId%>" value="<%=containerqtymax%>">
					</td>	
				
				<td align="left">
					<%=LocationName%>
					<input type=hidden name="LOCA<%=rowId%>" value="<%=LocationId%>">
<input type="hidden" name="<%= "LOCS"&rowId %>" value="<%
if mainsupplylocation<>"" then 
response.write mainsupplylocation
else
response.write "-1"
end if
%><%
 if mainsupplytype<>"" then
 if CStr(mainsupplytype)="1003" then 
 response.write "*"
end if
end if
 %>">

				</td>
				<!--The following should be used if multiple source locations are available-->
				<%'= ShowPickList2("", "LOCS"&locationid& "." &compoundid, "","select sum(qty_available) total, decode(location_type_id_fk,1003,'*','')||location_name || ',' ||sum(qty_available) || unit_abreviation  as displaytext,  decode(location_type_id_fk,1003,'*','')||location_id as value from inv_locations, inv_containers, inv_units where inv_containers.container_status_id_fk = 1 and inv_locations.location_id=inv_containers.location_id_fk and inv_units.unit_id=inv_containers.unit_of_meas_id_fk and location_type_id_fk in (1000,1003,1004) and compound_id_fk="&compoundid&" group by location_name, location_type_id_fk, location_id, unit_abreviation order by 1",30,"", "", "")%>
				<!--Otherwise use this -->
<td align="left"> <%=mainsupplyname & "<BR>" & cDbl(mainsupply)/containerqtymax & " S.U.<BR>" & mainsupply & " " & uom %> 
 				</td>

				
				<td align="left">
					<%=NDC%>
				<input type=hidden name="NDCC<%=rowId%>" value="<%=NDC%>">
				</td>
				<td align="left">
					<%=CompoundName%>
					<input type=hidden name="COMP<%=rowId%>" value="<%=CompoundId%>">
				</td>

				<td align="center">
				<%=PackageSize%>
				</td>
				<td align="right">
				<%
Response.write Cdbl(OnHand)/containerqtymax & " S.U.<br>" & OnHand & " " & UOM %>
				</td>
				<td align="right">
				<%=Amount%>
				</td>

				<td align="left">
	<% if round(Cdbl(qtyrequired)/containerqtymax,0)=Cdbl(qtyrequired)/containerqtymax then
		tempAmount=Cdbl(qtyrequired)/containerqtymax
		else
		tempAmount=Round(Cdbl(qtyrequired)/containerqtymax+1,0)
		end if %>

					<input type=text name="NEWA<%=rowId%>" value="<%=tempAmount%>">
					<input type=hidden name="OLDA<%=rowId%>" value="<%=tempAmount%>">
				</td>
				<td align="right">
					<%=tempAmount * containerqtymax & UOM%>
				</td>

				<td align="center">
				
				<input type=CheckBox name="DELI<%=rowId%>" value="OK" <%
	if mainsupplylocation<>"" then
		response.write ">"
		else
		response.write " Disabled >"
	end if
	%>
				
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



