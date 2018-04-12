<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
RequestID = Request("RequestID")
'LocationID = Request("DeliveryLocationID")
'ShipToName = Request("ShipToName")
OrderID = Request("OrderID")
Action = Request("Action")
CreateType = Request("CreateType")
%>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft FrontPage 5.0">
	<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
	<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
	<SCRIPT LANGUAGE=javascript>
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
				top.ListFrame.location.href = "Order_list.asp?Clear=0&OrderID=<%=OrderID%>&RequestID=<%=RequestID%>&Action=<%=Action%>&CreateType=<%=CreateType %>&OrderStatusID=" + document.form1.OrderStatusID.value + "&AddContainerID=" + temp[0] + "&DeliveryLocationID=" + document.form1.LocationID.value + "&ShipToName=" + document.form1.ShipToName.value;
			}
			clearTextBox();
		}
	</SCRIPT>
</HEAD>
<body onload="focusTextBox()">
<!--
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>
			<a class="MenuLink" HREF="/cheminv/cheminv/BrowseInventory_frset.asp" target="_top" title="Browse chemical inventory by location">Manage Containers</a>
			|
			<a class="MenuLink" HREF="/cheminv/inputtoggle.asp?dataaction=db&amp;dbname=cheminv" target="_top" title="Go to the main search screen">New Search</a>
			|
			<a class="MenuLink" HREF="/cs_security/home.asp" target="_top" title="Return to the home page">Home</a>
			|
			<a class="MenuLink" HREF="/cheminv/logoff.asp" target="_top">Log Off</a>
		</td>
	</tr>
</table>
-->
<form name="form1" action="" method="POST">
<INPUT TYPE="hidden" NAME="LocationID" VALUE="">
<INPUT TYPE="hidden" NAME="ShipToName" VALUE="">
<INPUT TYPE="hidden" NAME="OrderStatusID" VALUE="">
</form
<BR clear="all">
<center>
<table border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td align="right">
			Scan Container Barcode: <%=GetBarcodeIcon()%>&nbsp;
		</td>
		<td align="left">
			<input type="text" name="ContainerBarcode"  onchange="LookUpContainer(this.value); return false" size="20">
		</td>
	</tr>
</table>
</center>
</body>
</html>