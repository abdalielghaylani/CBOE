<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
Dim Conn
Dim Cmd
Dim RS
Dim bDebugPrint


Response.Expires = -1
InvSchema = Application("CHEMINV_USERNAME")
bDebugPrint = false
dbkey = "cheminv"
clear = Request.QueryString("clear")
containerCount = Request.QueryString("containerCount")
if containerCount = "" then containerCount = 0
awolContainerBarcode = Request("awolContainerBarcode")
LocationID = Request("LocationID")
if LocationID = "" then LocationID = Session("CurrentLocationID")
'CurrentUserID = Ucase(Session("UserNameChemInv"))
'DJP: this was returning chr(13) so I replace it with ""
CurrentUserID = replace(GetListFromSQLRow("SELECT owner_id_fk from inv_locations WHERE location_id=" & LocationID), chr(13),"")
if CurrentUserID = "" then CurrentUserID = "UNKNOWN"
MoveMissingContainers = Request("MoveMissingContainers")
showInList = lcase(Request("showInList"))
if showInList = "" or isEmpty(showInList) then showInList = "containers"
if lcase(showInList) = "plates" then
	assetNameLcase = "plate"
	aseetNameUcase = "Plate"
	assetNameLcasePlural = "plates"
	Set myDict = plate_multiSelect_dict	
else
	assetNameLcase = "container"
	aseetNameUcase = "Container"
	assetNameLcasePlural = "containers"
	Set myDict = multiSelect_dict
end if

if MoveMissingContainers = "1" then 
	Session("bMoveMissingContainers") = true
Elseif MoveMissingContainers = "0" then
	Session("bMoveMissingContainers") = false
End if


if clear then
	Response.Write "<SCRIPT LANGUAGE=javascript>(top.ListFrame)? theListFrame = top.ListFrame : theListFrame = top.main.ListFrame; theListFrame.location.href= theListFrame.location.href.replace('&multiSelect=1', '')+ '&multiSelect=0'</SCRIPT>"
	myDict.RemoveAll
	Session("awolContainerBarcodeList") = ""
Else
	
	str = Request.Form("selectChckBox")
	tempArr = split(str,",")
	For i = 0 to Ubound(tempArr)
		if NOT myDict.Exists(Trim(tempArr(i))) then
			myDict.Add Trim(tempArr(i)), true
		End if
	Next
	str = Request.Form("removeList")
	tempArr = split(str,",")
	For i = 0 to Ubound(tempArr)
		if myDict.Exists(Trim(tempArr(i))) then
			myDict.Remove(Trim(tempArr(i)))
		End if
	Next
	
	' append to awolContainerBarcode list
	if awolContainerBarcode <> "" then
		if Session("awolContainerBarcodeList") = "" then 
			Session("awolContainerBarcodeList") = awolContainerBarcode
		else
			Session("awolContainerBarcodeList") =  Session("awolContainerBarcodeList") & "," & awolContainerBarcode
		end if
	End if
	If Session("awolContainerBarcodeList") <> "" then
		GetInvConnection()
		if showInList = "containers" then
			Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETKEYCONTAINERATTRIBUTES", 4)		 
			Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERIDLIST", 200, 1, 2000, NULL) 
			Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERBARCODELIST", 200, 1, 2000, Session("awolContainerBarcodeList")) 
			if bdebugPrint then
				Response.Write "Parameters:<BR>"
				For each p in Cmd.Parameters
					Response.Write p.name & " = " & p.value & "<BR>"
				Next
					Response.write Session("awolContainerBarcodeList") & "<BR>"	
				'Response.end
			else
				Cmd.Properties ("PLSQLRSet") = TRUE  
				'Get AwolContainer Attributes
				Set RS = Cmd.Execute
				Cmd.Properties ("PLSQLRSet") = FALSE
				If NOT (RS.EOF AND RS.BOF) then
					temparr = RS.GetRows()
					RecordCount = Ubound(temparr,2) + 1
					RS.MoveFirst
				Else
					RecordCount = 0
				End if	
			end if
		elseif showInList = "plates" then
			Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETKEYPLATEATTRIBUTES", 4)		 
			Cmd.Parameters.Append Cmd.CreateParameter("PLATEIDLIST", 200, 1, 2000, NULL) 
			Cmd.Parameters.Append Cmd.CreateParameter("PLATEBARCODELIST", 200, 1, 2000, Session("awolContainerBarcodeList")) 
			if bdebugPrint then
				Response.Write "Parameters:<BR>"
				For each p in Cmd.Parameters
					Response.Write p.name & " = " & p.value & "<BR>"
				Next
					Response.write Session("awolContainerBarcodeList") & "<BR>"	
				'Response.end
			else
				Cmd.Properties ("PLSQLRSet") = TRUE  
				'Get AwolContainer Attributes
				Set RS = Cmd.Execute
				Cmd.Properties ("PLSQLRSet") = FALSE
				If NOT (RS.EOF AND RS.BOF) then
					temparr = RS.GetRows()
					RecordCount = Ubound(temparr,2) + 1
					RS.MoveFirst
				Else
					RecordCount = 0
				End if	
			end if
		end if
	end if
End if
%>

<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript">
	var showInList = '<%=showInList%>';
	function focusTextBox(){
		if (document.all.ContainerBarcode) document.all.ContainerBarcode.focus();
	}
	function clearTextBox(){
		if (document.all.ContainerBarcode) document.all.ContainerBarcode.value = "";
		focusTextBox();
	}
	
	function CheckForMatchingContainer(barcode){
		var elm;
		if (top.ListFrame){	
			if (top.ListFrame.document.all["_" + barcode]){
				top.ListFrame.HappySound();
				elm = top.ListFrame.document.all["_" + barcode].firstChild;
				if (!elm.checked) {
					elm.checked = true;
					top.ListFrame.SelectMarked();
				}				
			}
			else{
				if (showInList == 'containers') {
					if (IsKnownContainerBarcode(barcode)){
						this.location.href = "/cheminv/gui/reconcile.asp?containerCount=<%=ContainerCount%>&awolContainerBarcode='" + barcode + "'&showInList=" + showInList;
						top.ListFrame.SadSound()
					}
					else{
						top.ListFrame.SadSound2()
						<%If Session("INV_CREATE_CONTAINER" & "ChemInv") then%>
						bCreateNew = confirm("Unknown Container Barcode.  Would you like to create a new container?");
						if (bCreateNew) OpenDialog('/cheminv/gui/CreateOrEditContainer.asp?ReturnToReconcile=true&AutoGen=false&GetData=new&Barcode=' + barcode, 'ConDiag', 2);
						<%else%>
						alert("Unknown Container Barcode.  There is no inventory information for barcode " + barcode + ". Please contact an authorized inventory administrator to have the container added to the system.");	
						<%end if%>
					}
				}
				else if (showInList == 'plates') {
					if (CheckDuplicateBarcode(barcode, 'plate')){
						this.location.href = "/cheminv/gui/reconcile.asp?containerCount=<%=ContainerCount%>&awolContainerBarcode='" + barcode + "'&showInList=" + showInList;
						top.ListFrame.SadSound()
					}
					else{
						top.ListFrame.SadSound2()
						alert("Unknown Plate Barcode.  There is no inventory information for barcode " + barcode + ". Please contact an authorized inventory administrator to have the plate added to the system.");	
					}
				}
			}	
		}
		clearTextBox();	
	}
	
	function DoSubmit(){
		var elm;
		var missingIDList="";
		var bContinue = true;
		
		<%If Session("bMoveMissingContainers") then%>
		if (top.ListFrame){	
				if (top.ListFrame.form1.selectChckBox){
					elm = top.ListFrame.form1.selectChckBox;
					if (elm.length){
						for (i=0; i < elm.length ; i++){
							if (!elm[i].checked){
								missingIDList += elm[i].value + ",";
							}
						}
						missingIDList = missingIDList.substring(0, missingIDList.length-1)				
					}
					else{
						if (!elm.checked){
							missingIDList += elm.value;
						}
					}
				}
		}
		document.form1.MissingContainerIDList.value = missingIDList;
		if (missingIDList.length){
			bContinue = confirm("Warning: All <%=assetNameLcasePlural%> currently marked as missing will be removed to the missing location.  Are you sure you have scanned all <%=assetNameLcasePlural%> currently at this location?");
		}
		<%end if%>
		if (bContinue){
			document.form1.submit();
		}
	}

	//MCD: the first time this page is loaded:
	//       wait until the list of containers is completely loaded, then call SelectMarked() to get
	//       get the number of containers at this location.
	<%If clear = 1 Then%>
		var checkForLoadComplete = setInterval("if (top.ListFrame.finishedLoading){clearInterval(checkForLoadComplete);top.ListFrame.SelectMarked();}", 100);
	<%End If%>
	//MCD: end changes
</script>

</head>
<body onload="focusTextBox()">
<center>
<form name="form1" action="reconcile_action.asp" method="POST">	
<input type="hidden" name="LocationID" value="<%=LocationID%>">
<input type="hidden" name="CurrentUserID" value="<%=CurrentUserID%>">
<input type="hidden" name="MissingContainerIDList" value>
<INPUT TYPE="hidden" NAME="showInList" VALUE="<%=showInList%>">
<span class="GUIFeedback">Reconcile <%=assetNameLcasePlural%> at this location</span><br>
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
<tr>
	<td width="150" align="left" valign="top">
		<font size="1">Below is a list of <%=assetNameLcasePlural%> expected to be at this location.  Use the barcode scanner to verify all <%=assetNameLcasePlural%> actually found at this location.</font>
	<td>
	<td>
			<table border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td align="right">
						Scan <%=assetNameUcase%> Barcode: <%=GetBarcodeIcon()%>&nbsp;
					</td>
					<td align="left">
						<input type="text" name="ContainerBarcode" onchange="CheckForMatchingContainer(this.value); return false">
					</td>
				</tr>
				<tr>
					<td align="right">
						Number of <%=assetNameLcasePlural%> verified:
					</td>
					<td align="left">
						<input size="6" type="text" disabled name="CountVerified" value="<%=myDict.Count%>">
					</td>
				</tr>
				<tr>
					<td align="right">
						Number of <%=assetNameLcasePlural%> missing:
					</td>
					<td align="left">
						<input size="6" type="text" disabled name="misingCount" value="<%=containerCount - myDict.Count %>">
					</td>
				</tr>
				<tr>
					<td align="right">
						Number of misplaced <%=assetNameLcasePlural%>:
					</td>
					<td align="left">
						<input size="6" type="text" disabled name="CountVerified" value="<%=RecordCount%>">
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<br>

<%
if NOT clear then
	Set Session("multiSelectDict") = myDict
	Set myDict = Nothing
end if
If Session("awolContainerBarcodeList") <> "" then%>
	<span class="GUIFeedback">Misplaced <%=assetNameLcasePlural%></span>
	<br>
<%if showInList = "containers" then%>

<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td>
			<table border="0">
			<tr height="40">
				<td colspan="3">
					<font size="1">Check off <%=assetNameLcasePlural%> that should<br> be moved to this location</font>
				</td>
				
				<th colspan="3" align="center">
					Last Known
				</th>
			</tr>
			<tr>
				<th>
					Move?
				</th>
				<th>
					<%=assetNameUcase%> ID
				</th>
				<th>
					<%=assetNameUcase%> Name
				</th>
				<th>
					Location
				</th>
				<th>
					User
				</th>
				<th>
					Qty Remaining
				</th>
				
			</tr>
		<%
			If (RS.EOF AND RS.BOF) then
				Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No awol " & assetNameLcasePlural & ".</Span></TD></tr>")
			Else
				While (Not RS.EOF)
					ContainerID = RS("Container_ID")
					ContainerBarcode = RS("barcode")
					ContainerName = RS("Container_Name")
					LocationName = RS("Location_Name")
					CurrentUserID = RS("User_ID")
					QtyRemaining = RS("Qty_Remaining") & " " & RS("Unit_Abreviation")
					Path = RS("Path")
		%>
					<tr>
						<td align="center">
							<input type="checkbox" name="awolContainerIDList" value="<%=ContainerID%>" checked>
						</td>
						<td align="center">
							<%=ContainerBarcode%>
						</td>
						<td align="right"> 
							<%=TruncateInSpan(ContainerName, 15, "")%>
						</td>
						<td align="center"> 
							<span title="<%=Path%>"><%=LocationName%></span> 
						</td>
						<td align="center">
							<%=CurrentUserID%>
						</td>
						<td align="center">
							<%=QtyRemaining%>
						</td>
					</tr>
					<%rs.MoveNext
				Wend
				Response.Write "</table></center>"
			End if
			RS.Close
			Conn.Close
			Set RS = nothing
			Set Cmd = nothing
			Set Conn = nothing
%>
		</td>
	</tr>
</table>

<%elseif showInList = "plates" then%>

<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td>
			<table border="0">
			<tr height="40">
				<td colspan="3">
					<font size="1">Check off <%=assetNameLcasePlural%> that should<br> be moved to this location</font>
				</td>
				
				<th colspan="3" align="center">
					Last Known
				</th>
			</tr>
			<tr>
				<th>
					Move?
				</th>
				<th>
					<%=assetNameUcase%> ID
				</th>
				<th>
					Location
				</th>
				<th>
					Format
				</th>
				<th>
					Type
				</th>				
				<th>
					Qty Remaining
				</th>
				
			</tr>
		<%
			If (RS.EOF AND RS.BOF) then
				Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No awol " & assetNameLcasePlural & ".</Span></TD></tr>")
			Else
				While (Not RS.EOF)
					PlateID = RS("Plate_ID")
					PlateBarcode = RS("plate_barcode")
					LocationName = RS("Location_Name")
					PlateFormat = RS("plate_format_name")
					PlateType = RS("plate_type_name")
					QtyRemaining = RS("Qty_Remaining") & " " & RS("Unit_Abreviation")
					Path = RS("Path")
		%>
					<tr>
						<td align="center">
							<input type="checkbox" name="awolContainerIDList" value="<%=PlateID%>" checked>
						</td>
						<td align="center">
							<%=PlateBarcode%>
						</td>
						<td align="center"> 
							<span title="<%=Path%>"><%=LocationName%></span> 
						</td>
						<td align="center">
							<%=TruncateInSpan(PlateFormat, 15, "")%>
						</td>
						<td align="center">
							<%=PlateType%>
						</td>
						<td align="center">
							<%=QtyRemaining%>
						</td>
					</tr>
					<%rs.MoveNext
				Wend
				Response.Write "</table></center>"
			End if
			RS.Close
			Conn.Close
			Set RS = nothing
			Set Cmd = nothing
			Set Conn = nothing
%>
		</td>
	</tr>
</table>


<%end if
end if%>
<table border="0" width="60%">
	<tr>
		<%If Session("awolContainerBarcodeList") <> "" then%>
		<td nowrap colspan="2">
		<input type="hidden" name="MakeDefault" value>
		</td>
		<%end if%>
	</tr>
	<tr>
		<td>&nbsp;</td>
		<td align="right">
			<%=CancelButton("Close this window", "top.close(); top.opener.focus(); return false")%><a HREF="#" onclick="DoSubmit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>
</form>
</body>
</html>
