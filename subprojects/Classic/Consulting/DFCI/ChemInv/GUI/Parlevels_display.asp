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

function UpdateParLevel(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),Session("UserName" & "cheminv")))%>"	
	var ID = elm.name.substring(4,elm.name.length);
	var locationid = document.all["LOCA" + ID].value;
	var compoundid = document.all["COMP" + ID].value;
	var oldamount = document.all["OLDA" + ID].value;
	var amount = document.all["NEWA" + ID].value;
	var httpResponse ="";
	
	var msg="";


	if (oldamount == amount) {
// skip everything
msg="";
} else

{
	var strURL = "http://" + serverName + "/cheminv/api/DFCI_UpdateParLevel.asp?compoundID=" + compoundid + "&LocationId=" + locationid + "&Amount=" + amount; 	
	
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
		document.all["status_" + ID].innerHTML="Updated";	
		document.all["status_" + ID].style.color="green";
		document.all["status_" + ID].title = "Par level has been successfully updated.";
	}
	else{
		document.all["status_" + ID].innerHTML="Error";
		document.all["status_" + ID].style.color="red";
	}
}

}

function NewParLevel(){
	var locationid = document.all["NEWLOCATION"].value;
	var cas = document.all["NEWNDC"].value;
	var amount = document.all["NEWAMOUNT"].value;
	var httpResponse ="";
	
	var msg="";
	if (cas && locationid && amount) {
	var strURL = "http://" + serverName + "/cheminv/api/DFCI_UpdateParLevel.asp?cas=" + cas + "&LocationId=" + locationid + "&Amount=" + amount; 	
	//alert(strURL);
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);

	
	document.all["status_new"].style.fontWeight="bold";
	if (httpResponse >= 0){
		//change the bkgrnd color and lock the input box
		document.all["NEWNDC"].value="";
		document.all["NEWAMOUNT"].value="";
		document.all["status_new"].innerHTML= cas + " at locid " + locationid + " added...";	
		document.all["status_new"].style.color="green";
		document.all["status_new"].title = "Par level has been successfully updated.";
	}
	else{
		document.all["status_new"].innerHTML="Error" + httpResponse;
		document.all["status_new"].style.color="red";
	}
}
else
{
		document.all["status_new"].innerHTML="No addition.";
		document.all["status_new"].style.color="red";
}


}


function doIt(){
	NewParLevel();
	var s="";
	var elms = document.form1.elements;
	for (var i=0;i<elms.length; i++){
	if ((elms[i].name.substr(0,4)=="PARL")) {
		if ((elms[i].value.length > 0) && (!elms[i].disabled)) UpdateParLevel(elms[i]);
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

DrugName=Request("DrugName")
if DrugName="" then DrugName=null

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if



Response.Expires = -1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.DFCI_GetParLevels(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("pNDC",200, 1, 11, NDC)
Cmd.Parameters.Append Cmd.CreateParameter("pDestinationLocationID",131, 1, 0, LocationID)


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

caption = "Update the par levels in the grid below and press OK to submit the changes:"
disable2 = "disabled"
'DFCI MOVE EOF to after the edit block
%>



	<br>
	<p><span class="GUIFeedback"><%=Caption%></span></p>
	<form name="form1" action="deliver_request_action.asp" method="POST">
	<table border="1">
	<tr>
		<td colspan="13" align="center">
			<a href="#" onclick="doIt()"><img border="0" src="../graphics/ok_dialog_btn.gif"></a>
		</td>
	</tr>
	<tr>
		<th>
			Par Level Status
		</th>
		<th>
			Location
		</th>
		<th>
			NDC
		</th>
		<th>
			Drug
		</th>
		<th>
			Package Size
		</th>
		
		<th>
			Par Level
		</th>
	
		
		<!---		<th>			PO LineNum		</th>		<th>			Req Number		</th>		--->
	</tr>	

			
			<tr>
				<td align="left"> <span title id="status_new"></span>
				
				</td>	
				
			
				<%= ShowPickList2("", "NEWLOCATION", "", "select location_id as value, location_name as displaytext from inv_locations order by location_name", 25, " ","","")%>
			
				<td align="center">
					<input type=text maxsize=11 name="NEWNDC">
				</td>
				<td align="center">
				</td>
				<td align="center">
				</td>
				<td align="center">
				<input type=text maxsize=11 name="NEWAMOUNT">
				</td>

				</tr>


<%	
		While (Not RS.EOF)
			
			NDC = RS("NDC").value
			CompoundId = Rs("Compound_Id")
			CompoundName = Rs("CompoundName") 
			LocationName = RS("LocationName").value
			LocationId = RS("Location_Id").value
			Amount = RS("Amount").value
			PackageSize = RS("Package_Size").value
				
%>
			<span id="R<%=ContainerID%>" style="background-color:green"> 
		

	
			<tr>
				<td align="left"> <span title id="status_<%=locationid& "." &compoundid%>">Current</span>
					<input type=hidden name="PARL<%=locationid& "." &compoundid%>" value="<%=locationid& "." &compoundid%>">
				</td>	
				
				<td align="left">
					<%=LocationName%>
					<input type=hidden name="LOCA<%=locationid& "." &compoundid%>" value="<%=LocationId%>">
				</td>
				<td align="left">
					<%=NDC%>
				</td>
				<td align="left">
					<%=CompoundName%>
					<input type=hidden name="COMP<%=locationid& "." &compoundid%>" value="<%=CompoundId%>">
				</td>
				<td align="left">
					<%=PackageSize%>
				</td>

				<td align="right">
					<input type=text name="NEWA<%=locationid& "." &compoundid%>" value="<%=Amount%>">
					<input type=hidden name="OLDA<%=locationid& "." &compoundid%>" value="<%=AMount%>">
				</td>

				</tr>
			</span>
			<%rs.MoveNext
		Wend%>
			<tr>
				<td colspan="13" align="center">
					<a href="#" onclick="doIt();"><img border="0" src="../graphics/ok_dialog_btn.gif"></a>
				</td>
			</tr>
	</table>
<%	
'DFCI move this down here to allow edits to be submitted.
If (RS.EOF AND RS.BOF) then
	Response.Write ("<BR><BR><span class=""GUIFeedback"">&nbsp;&nbsp;&nbsp;No matching combinations found</Span>")
'DFCI comment out	Response.end
End if
'end change
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



