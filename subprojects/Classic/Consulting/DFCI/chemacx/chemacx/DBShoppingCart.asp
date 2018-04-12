<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
'SYAN modified on 3/31/2004 to support parameterized SQL and global search.
Dim conn
Dim cmd
bUseDBShopCart = 0

CSUserName = Session("CSSUserName" & "chemacx")
bSaveCart = Request("SaveCart")

If Cbool(bSaveCart) then
	bUseDBShopCart = 1
	ShoppingCartWDDXPacket = Request("ShoppingCartWDDXPacket")
	
	Response.Write "<script language=""JavaScript"">if(top.main.mainFrame){productsShown = top.main.mainFrame.productsShown;}else {productsShown = false;} var theMainFrame = top.main.mainFrame; var	theDisplayFrame = top.main.DisplayFrame;</script>"
	GetACXCommand "updateCart", adCmdText
	If Application("DBMS") = "ORACLE" then
		Sql = "UPDATE ChemAcxDb.ShoppingCart SET LastUPdate = sysdate, WddxPacket= ? WHERE CSUserName= ?"
	else
		Sql = "UPDATE ShoppingCart SET LastUPdate = Date(), WddxPacket= ? WHERE CSUserName= ?"
	end if
	'Response.Write sql
	'Response.end
	on error resume next
	'Conn.Execute Sql, lRecsAffected, adCmdText + adExecuteNoRecords
	Cmd.Parameters.Append Cmd.CreateParameter("Wddx",adLongVarChar, 1, len(ShoppingCartWDDXPacket), ShoppingCartWDDXPacket)
	Cmd.Parameters.Append Cmd.CreateParameter("user",200, 1, len(CSUserName), CSUserName)
	
	Cmd.CommandText = sql
	Cmd.execute lRecsAffected
	If Err.number <> 0 then
			Response.Write "<script language=""JavaScript"">alert(""Error occurred while Updating Shopping Cart Table\r" & Err.description &  """)</script>"
	End if
	If lRecsAffected = 0 then
		If Application("DBMS") = "ORACLE" then
			SQL = "INSERT INTO ChemACXDB.ShoppingCart (WddxPacket, CSUserName, LastUpdate) VALUES (?, ?, sysdate)"
		Else
			SQL = "INSERT INTO ShoppingCart (WddxPacket, CSUserName, LastUpdate) VALUES (?,?, Date())"
		End if
		'Response.Write sql
		'Response.end
		'Conn.Execute Sql, lRecsAffected, adCmdText + adExecuteNoRecords
		Cmd.CommandText = sql
		Cmd.execute lRecsAffected
		If Err.number <> 0 then
			Response.Write "<script language=""JavaScript"">alert(""Error occurred while Inserting into Shopping Cart Table\r" & Err.description &  """)</script>"
		End if
	End if
	'Response.Write sql
	'Response.end
	Conn.Close
	Set Conn = Nothing
	Response.Write "<script language=""JavaScript"">var	ShoppingCartWddxPacket = """ & ShoppingCartWDDXPacket & """;</script>"
Else
	If Len(CSUserName)>0 AND CBool(Application("SaveShopCartToDB")) then
		bUseDBShopCart = 1
		GetACXConnection()
		If Application("BASE_DBMS") = "ORACLE" then 'determined by base db type. The shopping cart info is in main db only.
			SQL = "SELECT WddxPacket FROM CHEMACXDB.ShoppingCart WHERE CSUserName=?"
		Else
			SQL = "SELECT WddxPacket FROM ShoppingCart WHERE CSUserName=?"
		End if
		SQL_Parameters = "UserName" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & CSUserName
		'Response.Write sql
		'Response.end
		'on error resume next
		'Set RS = Conn.execute(sql)
		Set RS = GetRecordSet(SQL, SQL_Parameters)
		If Err.number <> 0 then
			Response.Write "<script language=""JavaScript"">alert(""Error occurred while Reading Shopping Cart Table\r" & Err.description &  """)</script>"
		End if
		If RS.EOF AND RS.BOF then
			ShoppingCartWDDXPacket = ""
		Else
			ShoppingCartWDDXPacket = RS("WddxPacket")
		End if
		Response.Write "<script language=""JavaScript"">var	ShoppingCartWddxPacket = """ & ShoppingCartWDDXPacket & """;</script>"
		RS.Close
		Set RS = Nothing
		Conn.Close
		Set Conn = Nothing
	Else
		bUseDBShopCart = 0
	End if
End if
Response.Write "<script language=""JavaScript"">var bWriteShopCartToDb =" & bUseDBShopCart & ";</script>"
%>

