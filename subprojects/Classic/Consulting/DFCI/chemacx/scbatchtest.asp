<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
	<title>Testing the Shopping Cart Batch Addition</title>
</head>

<body>
This page ilustrates how to POST a request to add several products at once to the ChemACX.Com Shopping Cart.
<BR>
The URL: http://serverName/ChemACX/index.asp
<br>
The payload:  A form variable named "addBatchToCart" containing a WDDX structure of the form:
<br>
qty("Packageid")= int
<br>
Where qty is a WDDX structure indexed by the ChemACX PackageID 
<br>
int is an interger representing the quantity to be added to the cart.
<br>
<BR>
<!--- Example:<bR>
We will add three items to the shop cart:
<br><br>
qty("455111")=3<BR>
qty("650123")=1<BR>
qty("532111")=16 <BR> 
<br>
<BR> --->
<%
'get connection string from application variable
'connection_array = Application( "base_connection" & "chemacx")
'ConnStr = connection_array(0) & "="  & connection_array(1)
'response.write ConnStr
'Set AdoConn = Server.CreateObject("ADODB.Connection")

'SQLQuery = "SELECT [PackageID] AS packid FROM Package WHERE PackageID > 45500 AND PackageID < 45699" 

'AdoConn.Open ConnStr
'set packRS = AdoConn.Execute(SQLQuery)
'rowsarray = packRS.GetRows()
'numItems=UBound(rowsarray, 2)+1
'set AdoConn= nothing
'response.write numItems
'packRS.MoveFirst

Set MyStruct = Server.CreateObject("WDDX.Struct.1")

MyStruct.setProp "597754","1"
MyStruct.setProp "595364","1"
MyStruct.setProp "602476","1"
MyStruct.setProp "597751","1"
MyStruct.setProp "598137","1"
MyStruct.setProp "585125","1"


'While NOT packRS.EOF 
'MyStruct.setProp packRS("packid"), "3"
'response.write packRS("packid") &"<BR>"
'packRS.MoveNext
'Wend

Set MySer = Server.CreateObject("WDDX.Serializer.1")
wddxPacket= Myser.Serialize(Mystruct)
%>

<!--- <PRE>
CODE USED TO CREATE AND SERIALIZE THE WDDX STRUCTURE:

qty_ST = CreateObject("WDDX.Struct.1")
qty_ST.setProp "455111", "3"
qty_ST.setProp "111111", "1"
qty_ST.setProp "532111", "16"

MySer = CreateObject("WDDX.Serializer.1")
wddxPacket= Myser.Serialize(qty_ST)
</pre> --->



<form method="get" name=form1 action="index.asp">
<input type=button name=showpacket value="Click to see the WDDX packet" onclick=alert(packet)><BR>
<input type=hidden name=AddBatchToCart>
<input type=submit value="Post the Request">
</form>

<SCRIPT language="JavaScript">
packet = "<%=wddxPacket%>";
document.form1.AddBatchToCart.value = "<%=wddxPacket%>";
</script>
<BR><BR>



</body>
</html>
