<%
OrderID= Request("OrderID")
RequestID = Request("RequestID")
DeliveryLocationID = Request("DeliveryLocationID")
ShipToName = Request("ShipToName")
Action = Request("Action")
Clear = Request("Clear")
if isEmpty(Clear) or Clear = "" then Clear = 0
if OrderID = "" and lcase(action) = "edit" then OrderID = Session("OrderID")

%>

<html>
<head>
<title><%=Application("appTitle")%> -- Create or Edit an Order</title>
<SCRIPT LANGUAGE=javascript>
window.focus();
</SCRIPT>
</head>
<frameset rows="80,*">
		<frame name="ScanFrame" src="OrderScan.asp?OrderID=<%=OrderID%>&RequestID=<%=RequestID%>&DeliveryLocationID=<%=DeliveryLocationID%>&ShipToName=<%=ShipToName%>&Action=<%=Action%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto" border="0">
		<frame name="ListFrame" src="Order_list.asp?clear=<%=Clear%>&OrderID=<%=OrderID%>&RequestID=<%=RequestID%>&DeliveryLocationID=<%=DeliveryLocationID%>&ShipToName=<%=ShipToName%>&Action=<%=Action%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto" border="0">
</frameset>
</html>
