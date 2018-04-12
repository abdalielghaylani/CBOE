<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
Dim Conn
Dim Cmd
Dim RS
Dim bDebugPrint
Dim ContainerCount
Dim ReceivedCount
Dim RemovedCount
Dim Action

Response.Expires = -1
InvSchema = Application("CHEMINV_USERNAME")
bDebugPrint = false
dbkey = "cheminv"
clear = Request("clear")
ContainerCount = Request("ContainerCount")
if ContainerCount = "" then ContainerCount = 0
ReceiveChckBox = Request("ReceiveChckBox")
RemoveChckBox = Request("RemoveChckBox")
Action = Request("Action")

Set myDict = multiSelect_dict
if clear then
	Response.Write "<SCRIPT LANGUAGE=javascript>(top.ListFrame)? theListFrame = top.ListFrame : theListFrame = top.main.ListFrame; theListFrame.location.href= theListFrame.location.href.replace('&multiSelect=1', '')+ '&multiSelect=0'</SCRIPT>"
	myDict.RemoveAll
Else
	tempArr = split( ReceiveChckBox, "," )
	ReceivedCount = Ubound(tempArr) + 1
	tempArr = split( RemoveChckBox, "," )
	RemovedCount = Ubound(tempArr) + 1
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

	function LookUpContainer(barcode)
	{
		if( barcode == "" ) return;
		var strURL = "http://" + serverName + "/cheminv/api/GetKeyContainerAttributes.asp?ContainerBarcode='" + barcode + "'";	
		var httpResponse = JsHTTPGet(strURL) 
			
		if( httpResponse.length == 0 )
		{
			alert("Could not a find a container for barcode= " + barcode);
		}
		else
		{
			var temp = httpResponse.split(",");
			top.ListFrame.window.ScanContainer( temp[0] );
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
	</td>
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
						Number of containers received:
					</td>
					<td align="left">
						<input size="6" type="text" disabled name="ReceivedCount" value="<%=ReceivedCount %>">
					</td>
				</tr>
				<tr>
					<td align="right">
						Number of containers removed:
					</td>
					<td align="left">
						<input size="6" type="text" disabled name="RemovedCount" value="<%=RemovedCount %>">
					</td>
				</tr>
				<tr>
					<td align="right">
						Number of containers missing:
					</td>
					<td align="left">
						<input size="6" type="text" disabled name="MissingCount" value="<%=ContainerCount - ReceivedCount - RemovedCount%>">
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
</form>
</body>
</html>
