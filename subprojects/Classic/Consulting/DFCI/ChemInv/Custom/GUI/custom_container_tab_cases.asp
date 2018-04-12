
<%
	Case "EHS"
%>	
	<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/displayEHSData.asp"-->
	<center>
	<table width ="85%">
	
	<tr>
		<td align=right>
			<%If Session("INV_EDIT_EHS_DATA" & dbkey) AND EHSFoundData = "1" AND Len(SupplierCatNum) > 0 then%>			
				<a class="MenuLink" HREF="Edit Container EH&S Info" onclick="OpenDialog('/Cheminv/custom/GUI/EditEHSInfo.asp?ContainerID=<%=ContainerID%>', 'Diag', 1); return false">Edit EH&S Data</a>		
			<%End if%>
		</td>
	</tr>
	<%if EHSFoundData = "1" then%>
	<tr>
		<%If EHSIsDefaultSource = "1" AND Application("DISPLAY_EHS_DATA") Then%>
		<td colspan=7 align=center><em>(Note: displayed values are the defaults for this substance)</em></td>
		<%ElseIf EHSIsDefaultSource = "0" Then%>
		<td colspan=7 align=center><em>(Note: displayed values are for this particular supplier & product, not the defaults for this substance)</em></td>
		<%Else%>
		<td colspan=7 align=center><em>(Note: displayed values are from another product with the same substance)</em></td>
		<%End If%>
	</tr>
	<%End if%>
	</table>
	</center>
