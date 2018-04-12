<table border="0" cellspacing="0" cellpadding="0" width="600">
			<tr>
				<td valign="top"><a href="/chemacx/default.asp?formgroup=base_form_group&amp;dataaction=db&amp;dbname=chemacx"><img src="<%=Application("NavButtonGifPath")%>logo_cs_chemacxdb_r.gif" border="0"></a></td>
				<td align="right" valign="top">
					
					<table cellpadding="0" cellspacing="0" border="0">
						<%if NOT Application("UseCSWebUserAccounts") then%>
						<tr>
							<a href="chemacx/shop" onclick="ViewCart(); return false"><img src="<%=Application("NavButtonGifPath")%>shopping_cart_2.gif" alt="View Shopping Cart" border="0"></a>
						</tr>
						<%else%>
						<tr>
							<td align="right"><a href="chemacx/shop" onclick="ViewCart(); return false"><img src="<%=Application("NavButtonGifPath")%>shopping_cart.gif" width="111" height="18" alt border="0"></a></td>
							<td align="left"><a href="chemacx/login" onclick="ACXLogin();return false"><img src="<%=Application("NavButtonGifPath")%>login.gif" width="58" height="18" alt border="0"></a></td>
						</tr>
						<tr>
							<td align="right"><a href="chemacx/Your%20Account" onclick="ACXYourAccount();return false"><img src="<%=Application("NavButtonGifPath")%>your_account.gif" width="111" height="16" alt border="0"></a></td>
							<td align="left"><a href="chemacx/Help" onclick="ACXInstructions();return false"><img src="<%=Application("NavButtonGifPath")%>help.gif" width="46" height="16" alt border="0"></a></td>
						</tr>
						<%end if%>
						<tr>
							<td align="right" colspan="2" valign="top">
							<%If Len(CSUserName)>0 then%>
								<font face="Arial" color="#42426f" size="1"><b>Current User:&nbsp;<%=Ucase(CSUserName)%></b></font>
							<%Elseif Session("SubscriptionID") > 0 then%>		
								<font face="Arial" color="#42426f" size="1">
									<b>
										SubcriptionID:&nbsp;<%=Session("SubscriptionID")%><br>
										Number of users:&nbsp;<%=Application("CurrentSubscriptionUsers")%>	
									</b>
								</font>
							<%Elseif Len(Session("CSWebUsers_UserName")) > 0 then%>		
								<font face="Arial" color="#42426f" size="1">
									<b>
										Current User:&nbsp;<%=Session("CSWebUsers_UserName")%>	
									</b>
								</font>
							<%End if%>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>