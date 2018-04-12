<title>CambridgeSoft's ChemACX Enterprise - Search chemical catalogs.</title>
</head>
<body bgcolor="white" vlink="#000099" link="#000099">

<table border="0" cellspacing="0" cellpadding="0" height="67" width="600">
	<tr>
		<td width="8"></td>
		<td valign="top"><img src="<%=Application("NavButtonGifPath")%>logo_cs_chemacxdb_r.gif" border="0"></td>
		<td valign="center" align="center">
			<table cellpadding="0" cellspacing="0" border="0">
				<%if NOT Application("UseCSWebUserAccounts") then%>
						<tr>
							<!---<a href="shop" onclick="ViewCart(); return false"><img src="<%=Application("NavButtonGifPath")%>shopping_cart_2.gif" alt="View Shopping Cart" border="0"></a>--->
						</tr>
				<%else%>
				<tr>
					<td align="right"><a href="shop" onclick="ViewCart(); return false"><img src="<%=Application("NavButtonGifPath")%>shopping_cart.gif" width="111" height="18" alt border="0"></a></td>
					<td align="left"><a href="login" onclick="ACXLogin();return false"><img src="<%=Application("NavButtonGifPath")%>login.gif" width="58" height="18" alt border="0"></a></td>
				</tr>
				<tr>
					<td align="right"><a href="Your%20Account" onclick="ACXYourAccount();return false"><img src="<%=Application("NavButtonGifPath")%>your_account.gif" width="111" height="16" alt border="0"></a></td>
					<td align="left"><a href="Help" onclick="ACXInstructions();return false"><img src="<%=Application("NavButtonGifPath")%>help.gif" width="46" height="16" alt border="0"></a></td>
				</tr>
				<%end if%>
			</table>		
		</td>
	</tr>
</table>
<table border="0" cellpadding="0" cellspacing="0">
<tr>
	<td valign="top" width="500" bgcolor="#FFFFD6">
	<br>
<form name="npSearchForm" onsubmit="ifCAS()" method="POST" target="_top">
&nbsp;&nbsp;<font size="-1">Enter a chemical name or a CAS Number</font><br>
&nbsp;&nbsp;<input type="text" name="npSearchText" size="40" value>
<input type="submit" value="Search" id="submit1" name="submit1">
<br>
&nbsp;&nbsp;Or choose: <a target="_top" href="<%=Application("AppPathHTTP")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=chemacx">Advanced Query with Plugin</a>
<input type="hidden" name="Substance.CAS" value>
<input type="hidden" name="Synonym.Name" value>
</form>
</td>
	<td valign="middle" width="100" align="center">
	</td>
</tr>
</table>