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

function UpdateProtocol(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),"protocol"))%>"
	var ID = elm.name.substring(4,elm.name.length);
	var identifier = document.all["PROT" + ID].value;
	var startdate = document.all["STAR" + ID].value;
	var name = document.all["PRNA" + ID].value;
	var ncinum = document.all["PRNC" + ID].value;
	var enddate = document.all["ENDD" + ID].value;
	var sponsorid = document.all["PRSP" + ID].value;
	var httpResponse ="";
	var msg="";


	if (document.all["PUPD"].checked) {

	var strURL = "http://" + serverName + "/cheminv/api/dfci_manageprotocol.asp?ProtocolID=" + ID + "&ProtocolIdentifier=" + identifier + "&StartDate=" + startdate + "&EndDate=" + enddate + "&ProtocolName=" + name + "&NCIProtocolNum=" + ncinum + "&SponsorID=" + sponsorid; 	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	var httpResponse = JsHTTPGet(strURL);
	if (httpResponse >= 0){
	alert("Protocol updated.");
	document.all["PUPD"].checked = false;
	}
	else{
	alert("Error updating Protocol information. \n Please check the data and submit again.");
	}
}

}

function UpdateProtocolPI(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),"protocolpi"))%>"
	var ID = elm.name.substring(4,elm.name.length);
	var pi = document.all["PRPI" + ID].value;
	var ncinum = document.all["PRPN" + ID].value;
	var startdate = document.all["PIST" + ID].value;
	var enddate = document.all["PIEN" + ID].value;
	var httpResponse ="";
	var msg="";


	if (document.all["PIUP" + ID].checked) {

	var strURL = "http://" + serverName + "/cheminv/api/dfci_manageprotocolpi.asp?ProtocolId=" + document.all["PROTID"].value + "&ProtocolPiID=" + ID + "&PI=" + pi + "&StartDate=" + startdate + "&EndDate=" + enddate + "&PINCINUM=" + ncinum; 	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	var httpResponse = JsHTTPGet(strURL);
 
	if (httpResponse >= 0){
	alert("PI " + pi + " was updated.");
	document.all["PIUP" + ID].checked = false;	
	}
	else{
	alert("Error updating PI information for " + pi + ".\n Please check the data and submit again.");		
	}
}

}


function AddProtocolPI(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),"protocolpi"))%>"
	var pi = document.all["NPRP"].value;
	var ncinum = document.all["NPRN"].value;
	var startdate = document.all["NPIS" ].value;
	var enddate = document.all["NPIE" ].value;
	var httpResponse ="";
	var msg="";


	if (pi.length > 0) {

	var strURL = "http://" + serverName + "/cheminv/api/dfci_manageprotocolpi.asp?ProtocolId=" + document.all["PROTID"].value + "&PI=" + pi + "&StartDate=" + startdate + "&EndDate=" + enddate + "&PINCINUM=" + ncinum; 	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	var httpResponse = JsHTTPGet(strURL);
	if (httpResponse >= 0){
	alert("PI " + pi + " was added.");
	document.all["NPRP"].value = "";
	document.all["NPIS"].value = "";
	document.all["NPIE"].value = "";
	document.all["NPRN"].value = "";
	location.href="/cheminv/gui/protocol_display.asp?protocol=<%=request("Protocol")%>";
	}
	else{
	alert("There as a problem adding the new PI. \n Please try again.");		
	}
}

}

function doIt(){
	var s="";
	var elms = document.form1.elements;
	for (var i=0;i<elms.length; i++){
	if ((elms[i].name.substr(0,4)=="PROT")) {
		if ((elms[i].value.length > 0) && (!elms[i].disabled)) UpdateProtocol(elms[i]);
		}	
	
	
	if ((elms[i].name.substr(0,4)=="PRPI")) {
		if ((elms[i].value.length > 0) && (!elms[i].disabled)) UpdateProtocolPI(elms[i]);
		}
	
		
	if (elms[i].name.substr(0,4)=="NPRP") {
		if ((elms[i].value.length > 0) && (!elms[i].disabled)) AddProtocolPI(elms[i]);
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
strError = "Error:BulkMove_display.asp<BR>"

ProtocolIdentifier = Request("Protocol")
if ProtocolIdentifier = "" then  ProtocolIdentifier = null

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Response.Expires = -1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.DFCI_Getprotocol(?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("pProtocolIdentifier",200, 1, 50, ProtocolIdentifier)

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

caption = "To update the protocol, edit the fields, click on the checkboxes next to the records to update and click on OK.  All changes are audited."
disable2 = "disabled"

If (RS.EOF AND RS.BOF) then
	Response.Write ("<BR><BR><span class=""GUIFeedback"">&nbsp;&nbsp;&nbsp;No matching combinations found</Span>")
	Response.end
end if
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
			DFCI Protocol ID
		</th>
		<th> Name </th>
		<th> NCI No. </th>
		<th> Sponsor Id </th>
		<th>
			Start Date
		</th>
		<th>
			End Date
		</th>	
		<th> Update? </th>
	</tr>
	<% 
	PROTOCOLID=RS("PROTOCOL_ID") 
	%>
	<tr>
		<td> <input type="text" name="PROT<%=RS("protocol_id") %>" size=5 maxsize=5 value="<%=RS("Protocol_Identifier")%>"> </td>
		<td> <input type="text" name="PRNA<%=RS("protocol_id") %>" size=50 maxsize=500 value="<%=RS("Protocol_Name")%>"> </td>
 		<td> <input type="text" name="PRNC<%=RS("protocol_id") %>" size=20 maxsize=20 value="<%=RS("NCI_Protocol_NUM")%>"> </td>
		<td> <input type="text" name="PRSP<%=RS("protocol_id") %>" size=20 maxsize=100 value="<%=RS("SPONSOR_ID")%>"> </td>
		<td> <input type="text" name="STAR<%=Rs("protocol_id") %>" size=15 value="<%=RS("Start_Date")%>"> </td>
		<td> <input type="text" name="ENDD<%=Rs("protocol_id") %>" size=15 value="<%=RS("End_Date")%>"> </td> </td>
		<td> <input type="checkbox" name="PUPD"></td>
	</tr>
	</table> <HR>


	<% RS.MoveNext %>
	<table border=1>
	<tr>
		<th>
			PI
		</th>
		<th> PI NCI No. </th>
		<th>	Start Date </th>
		<th> 	End Date </th>
		<th>Update?</th>
	</tr>		


	
<%	
		While (Not RS.EOF)
			
 			PI = RS("PI").value
			PINCINUM = rs("PI_NCI_NUM")
			StartDate = Rs("Start_date")
			enddate= Rs("End_Date") 
%>

			<span id="R<%=ContainerID%>" style="background-color:green"> 
			
			<tr>
			<td> <input type="text" name="PRPI<%=rs("protocol_pi_id")%>" size=50 maxsize=255 value="<%=PI%>"> </td>
			<td> <input type="text" name="PRPN<%=rs("protocol_pi_id")%>" size=20 maxsize=100 value="<%=PINCINUM%>"> </td>
			<td> <input type="text" name="PIST<%=Rs("protocol_pi_id") %>" size=15 value="<%=RS("Start_Date")%>"> </td>
			<td> <input type="text" name="PIEN<%=Rs("protocol_pi_id") %>" size=15 value="<%=RS("End_Date")%>"> </td> </td>				
			<td> <input type="checkbox" name="PIUP<%=rs("protocol_pi_id") %>" value="yes"></td>
			</tr>

			</span>

			<%rs.MoveNext
		Wend%>


			<td> <input type="text" name="NPRP" size=50 value=""> </td>
			<td> <input type="text" name="NPRN" size=20 value=""> </td>
			<td> <input type="text" name="NPIS" size=15 value=""> </td>
			<td> <input type="text" name="NPIE" size=15 value=""> <input type="hidden" name="PROTID" VALUE="<%=PROTOCOLID%>"> </td> </td>				
			<td>  </td>
			<tr>
				<td colspan="4" align="center">
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



