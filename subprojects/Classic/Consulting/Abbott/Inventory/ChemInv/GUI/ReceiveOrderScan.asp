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
selectChckBox = Request("selectChckBox")

Set myDict = multiSelect_dict
Set ValidContainerDict = Session("ValidContainerDict")

'Response.Write Request("selectChckBox")
ValidContainerDict.RemoveAll
if clear then
	Response.Write "<SCRIPT LANGUAGE=javascript>(top.ListFrame)? theListFrame = top.ListFrame : theListFrame = top.main.ListFrame; theListFrame.location.href= theListFrame.location.href.replace('&multiSelect=1', '')+ '&multiSelect=0'</SCRIPT>"
	myDict.RemoveAll
Else
	str = Request("selectChckBox")
	tempArr = split(str,",")
	For i = 0 to Ubound(tempArr)
		if NOT ValidContainerDict.Exists(Trim(tempArr(i))) then
			ValidContainerDict.Add Trim(tempArr(i)), true
		End if
	Next
	str = Request.Form("removeList")
	tempArr = split(str,",")
	For i = 0 to Ubound(tempArr)
		if ValidContainerDict.Exists(Trim(tempArr(i))) then
			ValidContainerDict.Remove(Trim(tempArr(i)))
		End if
	Next
End if
%>

<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript">
	function focusTextBox(){
		if (document.all.ContainerBarcode) document.all.ContainerBarcode.focus();
	}
	function clearTextBox(){
		if (document.all.ContainerBarcode) document.all.ContainerBarcode.value = "";
		focusTextBox();
	}

	function LookUpContainer(barcode){
		if (barcode =="") return;
		var strURL = "http://" + serverName + "/cheminv/api/GetKeyContainerAttributes.asp?ContainerBarcode='" + barcode + "'";	
		var httpResponse = JsHTTPGet(strURL) 
			
		if (httpResponse.length == 0){
			alert("Could not a find a container for barcode= " + barcode);
		}
		else{
			var temp = httpResponse.split(",");
			top.ListFrame.location.href = "ReceiveOrder_list.asp?Clear=0&selectChckBox=<%=selectChckBox%>&AddContainerID=" + temp[0];
		}
		clearTextBox();
	}
	
	function CheckForMatchingContainer(barcode){
		var elm;
	
		if (top.ListFrame){	
			LookUpContainer(barcode);				
		}
		clearTextBox();	
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
<form name="form1" action="" method="POST">	
<input type="hidden" name="LocationID" value="<%=LocationID%>">
<input type="hidden" name="CurrentUserID" value="<%=CurrentUserID%>">
<input type="hidden" name="MissingContainerIDList" value>
<span class="GUIFeedback">Receive containers from an order</span><br>
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
<tr>
	<td width="150" align="left" valign="top">
		<font size="1">Below is a list of containers expected to be in this order.  Use the barcode scanner to verify all containers actually found in this order.</font>
	<td>
	<td>
			<table border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td align="right">
						Scan Container Barcode: <%=GetBarcodeIcon()%>&nbsp;
					</td>
					<td align="left">
						<input type="text" name="ContainerBarcode" onchange="CheckForMatchingContainer(this.value); return false">
					</td>
				</tr>
				<tr>
					<td align="right">
						Number of containers verified:
					</td>
					<td align="left">
						<input size="6" type="text" disabled name="CountVerified" value="<%=ValidContainerDict.Count%>">
					</td>
				</tr>
				<tr>
					<td align="right">
						Number of containers missing:
					</td>
					<td align="left">
						<!--<input size="6" type="text" disabled name="misingCount" value="<%=containerCount - myDict.Count %>">-->
						<input size="6" type="text" disabled name="misingCount" value="<%=myDict.Count - ValidContainerDict.Count%>">
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
%>
<table border="0" width="60%">
	<tr>
		<td>&nbsp;</td>
		<td align="right">
			<%=GetCancelButton()%>
		</td>
	</tr>
</table>
</form>
</body>
</html>
