<a class="MenuLink" HREF="BrowseInventory_frset.asp" target="_top" title="Browse chemical inventory by location">Manage Containers</a>
<%If Session("INV_DELETE_CONTAINER" & "Cheminv") OR Session("INV_MOVE_CONTAINER" & "Cheminv") OR Session("INV_CHECKOUT_CONTAINER" & "Cheminv") OR Session("INV_CHECKIN_CONTAINER" & "Cheminv") OR Session("INV_RETIRE_CONTAINER" & "Cheminv") then%>
|
<a class="MenuLink" HREF="/cheminv/gui/multiScan_frset.asp" target="_top">Multi Scan</a>	
<%end if%>	
<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
|
<a class="MenuLink" HREF="Admin Menu" onclick="OpenDialog('../gui/menu.asp', 'MenuDiag', 2); return false" target="_top">Tasks</a>
<%End if%>
<%If Session("INV_MANAGE_SUBSTANCES" & dbkey) then%>	
<span id="searchSubstancesLink">
|
<%
if Session("isCDP") = "TRUE" then
	substanceFG = "substances_form_group"		
else
	substanceFG = "substances_np_form_group"		
end if
%>
<a id="SearchSubs" class="MenuLink" HREF="../inputtoggle.asp?formgroup=<%=substanceFG%>&amp;dataaction=db&amp;dbname=cheminv&amp;returnaction=<%=formgroup%>" target="_top" title="Manage chemical inventory substances">Manage Substances</a>

</span>
<%End if%>
|
<a class="MenuLink" HREF="/cheminv/logoff.asp" target="_top">Log Off</a>
