<%
isFirst = true
%>

<%If Session("INV_EDIT_Plate" & dbkey) then
	call CheckIfFirst()	
%>			
	<a class="MenuLink" id="EditWellLnk" HREF="Edit plate well" onclick="OpenDialog('../GUI/EditWell.asp?isEdit=true&getData=db&WellID=<%=WellID%>&sTab=Required', 'Diag2', 2); return false">Edit Well</a>
<%End if%>


<%
sub CheckIfFirst()
	if isFirst then
		isFirst = false
	else
		Response.Write " | "
	end if
end sub
%>