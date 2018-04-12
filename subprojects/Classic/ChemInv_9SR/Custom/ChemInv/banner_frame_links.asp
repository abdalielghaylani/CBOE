<a class="MenuLink" HREF="BrowseInventory_frset.asp" target="_top" title="Browse chemical inventory by location">Manage Containers</a>
<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
|
<a class="MenuLink" HREF="Admin Menu" onclick="OpenDialog('../gui/menu.asp', 'MenuDiag', 2); return false" target="_top">Admin</a>
<%End if%>
<%If Session("INV_MANAGE_SUBSTANCES" & dbkey) then%>	
<span id="searchSubstancesLink">
|
<a id="SearchSubs" class="MenuLink" HREF="../inputtoggle.asp?formgroup=substances_form_group&amp;dataaction=db&amp;dbname=cheminv" target="_top" title="Manage chemical inventory substances">Manage Substances</a>	
</span>
<%End if%>
|
<a class="MenuLink" HREF="/cheminv/logoff.asp" target="_top">Log Off</a>
