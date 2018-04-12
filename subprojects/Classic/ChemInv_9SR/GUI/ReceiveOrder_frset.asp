<%
OrderID= Request("OrderID")
RequestID = Request("RequestID")
DeliveryLocationID = Request("DeliveryLocationID")
ShipToName = Request("ShipToName")
Action = Request("Action")
Clear = Request("Clear")
if isEmpty(Clear) or Clear = "" then Clear = 0

'create a dictionary for the validated containers
if not isObject(Session("ValidContainerDict")) then Set Session("ValidContainerDict") = server.CreateObject("Scripting.Dictionary")
%>

<html>
<head>
<title><%=Application("appTitle")%> -- Receive an Order</title>
<SCRIPT LANGUAGE=javascript>
window.focus()

</SCRIPT>
</head>
<frameset rows="300,*">
		<frame name="ScanFrame" src="ReceiveOrderScan.asp" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto" border="0">
		<frame name="ListFrame" src="ReceiveOrder_list.asp?clear=<%=Clear%>&OrderID=<%=OrderID%>&RequestID=<%=RequestID%>&DeliveryLocationID=<%=DeliveryLocationID%>&ShipToName=<%=ShipToName%>&Action=<%=Action%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto" border="0">
</frameset>
</html>
