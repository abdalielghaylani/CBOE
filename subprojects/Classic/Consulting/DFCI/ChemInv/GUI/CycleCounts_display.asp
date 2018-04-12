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

function UpdateCount(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),"cyclecount"))%>"	
	var ID = elm.name.substring(4,elm.name.length);
	var locationid = document.all["LOCA" + ID].value;
	var compoundid = document.all["COMP" + ID].value;
	var oldamount = document.all["OLDA" + ID].value;
	var amount = document.all["NEWA" + ID].value;
	var conv = document.all["CONV" + ID].value;
	var reason = "";
	if (document.all["RECS" + ID] == undefined) {
	reason="None given";	
	} else
	{
	reason=document.all["RECS" + ID].value;
	}
	
	
	var httpResponse ="";
	var date=new Date();
	
	var msg="";



// convert to sell units

amount = amount * conv;
	var strURL = "http://" + serverName + "/cheminv/api/DFCI_UpdateCount.asp?compoundID=" + compoundid + "&LocationId=" + locationid + "&qty=" + amount + "&reason=" + reason + "&date=" + date.getTime(); 	
	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	//alert(strURL);
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);

	
	document.all["status_" + ID].style.fontWeight="bold";
	if (httpResponse >= 0 || httpResponse == -5000){
		//change the bkgrnd color and lock the input box
		elm.style.backgroundColor="lightyellow";
		//elm.disabled = true;
		document.all["status_" + ID].innerHTML="Updated";	
		document.all["status_" + ID].style.color="green";
		document.all["status_" + ID].title = "Count has been successfully updated.";
	}
	else{
		document.all["status_" + ID].innerHTML="Error";
		document.all["status_" + ID].style.color="red";
	}


}

function UpdateFrequency(elm){
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),Session("UserName" & "cheminv")))%>"	
	var ID = elm.name.substring(4,elm.name.length);
	var locationid = document.all["LOCA" + ID].value;
	var compoundid = document.all["COMP" + ID].value;
	var oldamount = document.all["OLDF" + ID].value;
	var amount = document.all["NEWF" + ID].value;
	var httpResponse ="";
	
	var msg="";


	if (oldamount == amount) {
// skip everything
msg="";
} else

{
	var strURL = "http://" + serverName + "/cheminv/api/DFCI_UpdateCOuntFrequency.asp?compoundID=" + compoundid + "&LocationId=" + locationid + "&Frequency=" + amount; 	
	
	strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
	//alert(strURL);
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);

	
	document.all["status_" + ID].style.fontWeight="bold";
	if (httpResponse >= 0){
		//change the bkgrnd color and lock the input box
		elm.style.backgroundColor="lightyellow";
		//elm.disabled = true;
		document.all["status_" + ID].innerHTML="Updated";	
		document.all["status_" + ID].style.color="green";
		document.all["status_" + ID].title = "Frequency has been successfully updated.";
	}
	else{
		document.all["status_" + ID].innerHTML="Error";
		document.all["status_" + ID].style.color="red";
	}
}

}

function NewFrequency(){
	var locationid = document.all["NEWLOCATION"].value;
	var cas = document.all["NEWNDC"].value;
	var frequency = document.all["NEWFREQ"].value;
	var httpResponse ="";
	
	var msg="";
	if (cas && locationid && frequency) {
	var strURL = "http://" + serverName + "/cheminv/api/DFCI_UpdateCountFrequency.asp?cas=" + cas + "&LocationId=" + locationid + "&Frequency=" + frequency; 	
	//alert(strURL);
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);

	
	document.all["status_new"].style.fontWeight="bold";
	if (httpResponse >= 0){
		//change the bkgrnd color and lock the input box
		document.all["NEWNDC"].value="";
		document.all["NEWFREQ"].value="";
		document.all["status_new"].innerHTML= cas + " at locid " + locationid + " added...";	
		document.all["status_new"].style.color="green";
		document.all["status_new"].title = "CC Frequency has been successfully updated.";
	}
	else{
		document.all["status_new"].innerHTML="Error" + + httpResponse;
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
	NewFrequency();
var ID;

	var s="";
	var elms = document.form1.elements;
	for (var i=0;i<elms.length; i++){
	var ID;
	var count=0;
	if ((elms[i].name.substr(0,4)=="PARL")) {

	ID=elms[i].name.substring(4,elms[i].name.length);
	if (document.all["UPDQ" + ID].checked) {
		if ((elms[i].value.length > 0) && (!document.all["UPDQ" + ID].disabled)) 
		{
			UpdateCount(elms[i]);
			count++;
		}		
		}
	if (document.all["UPDF" + ID].checked) {
		if ((elms[i].value.length > 0) && (!document.all["UPDF" + ID].disabled)) 
			{
			UpdateFrequency(elms[i]);
			count++;
		
		}
		}
	
		if (count>0) {
		document.all["NEWA" + ID].disabled=true;
		document.all["UPDQ" + ID].disabled=true;
		document.all["NEWF" + ID].disabled=true;
		document.all["UPDF" + ID].disabled=true;
		elms[i].disabled=true;
		}
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
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.DFCI_GetCycleCounts(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("pLocationID",131, 1, 0, LocationID)
Cmd.Parameters.Append Cmd.CreateParameter("pCAS",200, 1, 100, NDC)





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

caption = "Update the cycle counts in the grid below and press OK to submit the changes:"
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
			Cycle Count Status
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
			Count in DB (Sell Units)
		</th>
		
		<th>
			New Count (Sell Units)
		</th>
		<th> 
			Reason
		</th>	
		<th>
			Update Amount?
		</th>
		<th>
			Last Count
		</th>
		<th>
			Count Frequency (Days)
		</th>

		<th>
			Update Frequency?
		</th>

		</td>
		
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
				</td>
				<td align="center">
				</td>
				<td align="center">
				</td>
				<td align="center">
				</td>
				<td align="center">
				<input type=text maxsize=11 name="NEWFREQ">
				</td>
	

				</tr>


<%	
		While (Not RS.EOF)
			
			NDC = RS("NDC").value
			CompoundId = Rs("Compound_Id")
			CompoundName = Rs("CompoundName") 
			LocationName = RS("LocationName").value
			LocationId = RS("Location_Id").value

			PackageSize = RS("Package_Size").value
		'Add additional data records
			CurrentCount =  Rs("CurrentCount") 
			qtyconversion = RS("qty_conversion")
			CountFrequency = Rs("Frequency")
			LastCountDate = rs("Last_Cycle_Count")
			CurrentCount = CDbl(CurrentCount)/CDBL(qtyconversion)
			
%>
			<span id="R<%=ContainerID%>" style="background-color:green"> 
		

	
			<tr>
				<td align="left"> <span title id="status_<%=locationid& "." &compoundid%>">
<% 
if LastCountDate<>"none" then 

If dateadd("d",CInt(countfrequency),CDate(LastCountDate))< now and CInt(countFrequency)>0 then 
response.write "Overdue"
else
response.write "Current"
end if
else 
response.write "Unknown"
end if
%>


</span>
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
					<%=CurrentCount%>
				</td>
				<td align="right">
					<input type=text name="NEWA<%=locationid& "." &compoundid%>" value="<%=CurrentCount%>">
					<input type=hidden name="OLDA<%=locationid& "." &compoundid%>" value="<%=CurrentCount%>">
					<input type=hidden name="CONV<%=locationid& "."  & compoundid %>" value="<%=qtyconversion%>">
				</td>
				<td>
				<%=ShowSelectBox3("RECS" & locationid & "." & compoundid, "", "SELECT Picklist_display AS Value,  Picklist_display AS DisplayText FROM inv_picklists where picklist_domain=102 ORDER BY picklist_id ASC", 20, RepeatString(18, "&nbsp;"), "", "")%>
			
				</td>

				<td align="right">														
				<input type=CHECKBOX name="UPDQ<%=locationid& "." &compoundid%>" value="Yes">			
				</td>	
				<td align="right">
<%if LastCountDate<>"" then %>
					<%=LastCountDate%>
<%else 
response.write "none"
end if
%>
				</td>
				<td align="right">
					<input type=text name="NEWF<%=locationid& "." &compoundid%>" value="<%=CountFrequency%>">
					<input type=hidden name="OLDF<%=locationid& "." &compoundid%>" value="<%=CountFrequency%>">
				</td>


				<td align="right">														
				<input type=CHECKBOX name="UPDF<%=locationid& "." &compoundid%>" value="Yes">			
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



