<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
Dim Conn
Dim Cmd
Dim RS
Dim SQL
Dim bDebugPrint
Dim ContainerID
Dim OrderID
Dim bRedirect
Dim sMessage

Response.Expires = -1
InvSchema = Application("CHEMINV_USERNAME")
dbkey = "cheminv"

ContainerID = Request.Form("ContainerID")
if( IsNull(ContainerID) or ContainerID = "" ) then
    ContainerID = Request("ContainerID")
end if
bDebugPrint = false
bRedirect = false
sMessage = ""

if( not IsNull(ContainerID) and ContainerID <> "" ) then
    ' See if there are any shipped orders with this container in them.
    GetInvConnection()
    SQL = "SELECT DISTINCT order_id_fk FROM inv_order_containers, inv_orders WHERE order_id_fk = order_id AND order_status_id_fk = 2 AND container_id_fk = ?"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("AddContainerID", 5, 1, 0, ContainerID)
	Set RS = server.CreateObject("ADODB.recordset")
	RS.Open Cmd
		
	if not (RS.BOF or RS.EOF) then 
		OrderID = RS("order_id_fk")
		bRedirect = true
	else		
		sMessage = "This container is not associated with a shipped order."
	end if
	
	RS.Close()
	Conn.Close()
	
	if( bRedirect ) then
	    Response.Redirect( "/cheminv/GUI/ReceiveOrder_frset.asp?OrderID=" & OrderID & "&Clear=1&ScannedContainerID=" & ContainerID)
    end if
end if
%>

<html>
<head>
<title>Receive Order</title>
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
		var strURL = serverType + serverName + "/cheminv/api/GetKeyContainerAttributes.asp?ContainerBarcode='" + barcode + "'";	
		var httpResponse = JsHTTPGet(strURL)
		var bSubmit = false;
			
		if( httpResponse.length == 0 )
		{
			alert("Could not a find a container for barcode= " + barcode);
			clearTextBox();
		}
		else
		{
			var temp = httpResponse.split(",");
			document.form1.ContainerID.value = temp[0];
			document.form1.submit();		
		}		
	}
	
	function CheckForMatchingContainer()
	{
		var elm;
		elm = document.getElementById('ContainerBarcodeID');
		if( elm )
		{
		    LookUpContainer( elm.value );
		}
		clearTextBox();	
	}
</script>

</head>
<body onload="focusTextBox();" style="margin: 10;">
<center>
<form name="form1" action="./ReceiveFindOrder.asp" method="POST">	
<input type="hidden" name="ContainerID" value="">

<span class="GUIFeedback">Receive containers from an order</span><br>
<p></p>
<table border=0 cellspacing="2" cellpadding="0">
<tr>
<td>
    <table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
    <tr>
	    <td width="150" align="left" valign="top">
        <font size="1">Use the barcode scanner to search for a shipped order with that container.</font>
	    </td>
	    <td>
			    <table border="0" cellspacing="0" cellpadding="0">
				    <tr>
					    <td align="right">
						    Scan Container Barcode: <%=GetBarcodeIcon()%>&nbsp;
					    </td>
					    <td align="left">
						    <input type="text" id="ContainerBarcodeID" name="ContainerBarcode" onchange="CheckForMatchingContainer(); return false">
					    </td>
				    </tr>
			    </table>
        </td>
    </tr>
    </table>
</td>
</tr>
<tr>
<td align=right>
    <a class="MenuLink" HREF="#" onclick="window.close();return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" alt="Close this dialog"></a>
    <a class="MenuLink" HREF="#" onclick="CheckForMatchingContainer();return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0" alt="Search for order"></a>
</td>
</tr>
</table>

<p>
</form>
<script language=javascript>
<%
    if( sMessage <> "" ) then
%>
    alert( '<%=sMessage %>' );
<%
    end if
%>
</script>
</body>
</html>
