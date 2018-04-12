<%@ Language=VBScript %>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
</HEAD>
<BODY>

<table border="0" cellspacing="0" cellpadding="2" width="600">			
	<tr>
		<td valign="top"><img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0"></td>
		<td align="right" valign="top" nowrap>
				<br><br>
				<%If Application("Admin_required") then%>
					<font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
				<%End if%>
		</td>
	</tr>
</table>


<BR><BR><BR>

<center><SPAN class="GuiFeedback">
Data transfer has been received from ChemACX<BR><BR></SPAN>
To view a log of the actions performed in Inventory click <a href="/cheminv/api/displayactionbatch.asp?sid=<%=session.SessionID%>" >here</a>
<BR>
To go to the Inventory "On Order" location click <a href="/cheminv/cheminv/BrowseInventory_frset.asp?gotoNode=1">here</a>
<BR><BR>
<a  href="#" onclick="window.close()" title="Close dialog window"><IMG border="0" SRC="../graphics/close_dialog_btn.gif"></a>

</center>

</BODY>
</HTML>
