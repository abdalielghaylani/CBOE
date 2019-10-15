
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
<script language="JavaScript">focus(); </script>
<head>
<title>ChemACX - Vendor Detail</title>

</head>
<body bgcolor="#FFFFFF">
<p>
<!--#INCLUDE virtual = "/chemacx/api/apiutils.asp"-->
<%
Dim Conn
'get connection string from application variable
'connection_array = Application("base_connection" & Application("appkey"))
'connStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & Session("UserName" & Application("appkey")) & ";" & Application("PWDKeyword") & "=" & Session("UserID" & Application("appkey"))
'response.write ConnStr

GetACXConnection()

'Set Conn = Server.CreateObject("ADODB.Connection")

	SQLQuery = "SELECT SupplierAddr.Addr1, SupplierAddr.Addr2, SupplierAddr.City, SupplierAddr.State, SupplierAddr.Code, SupplierAddr.Country, SupplierAddr.Type FROM SupplierAddr WHERE  SupplierAddr.Type Is Not Null AND upper(SupplierAddr.Type)=upper('postal') AND SupplierAddr.SupplierID=" & request.querystring("supid")
	'Conn.Open ConnStr
	set AddrRS = Conn.Execute(SQLQuery)
	
	if AddrRS.EOF then 
Addr1="&nbsp;"
Addr2="&nbsp;"
city="&nbsp;"
state="&nbsp;"
code="&nbsp;"
coountry="&nbsp;"
else 
Addr1= AddrRS("Addr1")
Addr2= AddrRS("Addr2")
city= AddrRS("city")
state= AddrRS("state")
code= AddrRS("code")
country= AddrRS("country")
end if 	
	
	SQLQuery = "SELECT SupplierAddr.Addr1 FROM SupplierAddr WHERE SupplierAddr.Type='http' AND SupplierAddr.SupplierID=" & request.querystring("supid")
	set URLRS = Conn.Execute(SQLQuery)
	SQLQuery = "SELECT SupplierAddr.Addr1 FROM SupplierAddr WHERE SupplierAddr.Type='email' AND SupplierAddr.SupplierID=" & request.querystring("supid")
	set EMAILRS = Conn.Execute(SQLQuery)
if EMAILRS.EOF then 
email="&nbsp;"
else 
email= EMAILRS("Addr1")
end if 	

SQLQuery = "SELECT Supplier.Name, Supplier.LogoPath FROM Supplier WHERE Supplier.SupplierID=" & request.querystring("supid")
	set SupRS = Conn.Execute(SQLQuery)
if URLRS.EOF then 
url="&nbsp;"
else 
url= URLRS("Addr1")
end if

SQLQuery = "SELECT SupplierPhoneID.Type, SupplierPhoneID.CountryCode, SupplierPhoneID.AreaCode, SupplierPhoneID.PhoneNum, SupplierPhoneID.Location FROM SupplierPhoneID WHERE SupplierPhoneID.SupplierID="& request.querystring("supid") & " ORDER BY SupplierPhoneID.Type DESC"	
set PhoneRS = Conn.Execute(SQLQuery)

set Conn= nothing
%>

<form name="proddet">
<table cellspacing="0" cellpadding="0" bordercolor="#4A5AA9" border="1" align="center">

<tr>
    <td align="center">
		<% logo =  SupRS("logopath")
		  	if NOT isNull(logo) then%>
			<img SRC="graphics/<%=logo%>">
		<%else%>
			&nbsp;
		<%end if%>
	</td>
</tr>

<tr>
    <td align="center"><table border="0"><tr><td></td></tr></table></td>
</tr>

<tr>
<td>
<table border="0">
<tr><td align="right" width="80"><b>Supplier:</b></td><td><%=SupRS("Name") %></td></tr>
<tr><td align="right"><b>Address1:</b></td><td><%=Addr1 %></td></tr>
<tr><td align="right"><b>Address2:</b></td><td><%=Addr2 %></td></tr>
<tr><td align="right"><b>City:</b></td><td><%=city %>&nbsp; <%= state%>&nbsp;  <%=code %> </td></tr>
<tr><td align="right"><b>Country:</b></td><td><%=country %></td></tr>
</table>
</td>
</tr>

<tr>
<td>
<table><tr><td width="80">
<% 
while NOT PhoneRs.EOF
phone = "+" & PhoneRS("countrycode") & " &nbsp;" &  PhoneRS("areacode") &  " &nbsp; " & PhoneRS("phonenum") & " &nbsp; (" & PhoneRS("location") & ")&nbsp;"
response.write "<TR><TD align=right><b>" & PhoneRS("type") & ":</b></td><td>" & phone & "</td></tr>"
PhoneRS.MoveNext  
wend
%></td></tr></table>

</tr></td>


<tr>
<td>
<table border="0">
<tr><td align="right" width="80"><b>E-mail:</b></td><td><%=email %></td></tr>
<tr><td align="right"><b>URL:</b></td><td><a target="_new" href="http://<%=url %>"><%=url %></a> </td></tr>
<tr><td></td></tr></table>
</td>
</tr>

<tr><td align="center"><br><a href="Close" onclick="window.self.close(); return false;"><img src="<%=Application("NavButtonGifPath")%>close.gif" alt="Close" border="0"></a><br></td></tr>

</table>


</form>

</body>



</html>


