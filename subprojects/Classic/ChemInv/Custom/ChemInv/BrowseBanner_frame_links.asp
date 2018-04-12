<a class="MenuLink" HREF="../inputtoggle.asp?dataaction=db&amp;dbname=cheminv" target="_top">New Search</a>
	|
	<a class="MenuLink" HREF="../inputtoggle.asp?dbname=cheminv&amp;GotoCurrentLocation=true" target="_top">Search Results</a>
	<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Create%20a%20new%20inventory%20location" onclick="OpenDialog('../gui/NewLocation.asp', 'LocDiag', 1); return false">New Location</a>
	<%End if%>
	<%If Session("INV_EDIT_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Edit%20an%20inventory%20location" onclick="OpenDialog('../gui/NewLocation.asp?GetData=db', 'LocDiag', 1); return false">Edit Location</a>
	<%End if%>
	<%If Session("INV_MOVE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Move%20an%20inventory%20location" onclick="OpenDialog('../gui/MoveLocation.asp', 'LocDiag', 1); return false">Move Location</a>
	<%End if%>
	<%If Session("INV_DELETE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Delete%20inventory%20location" onclick="OpenDialog('../gui/DeleteLocation.asp', 'LocDiag', 1); return false" target="_top" title="Delete location <%=Session("CurrentLocationName")%>">Delete Location</a>
	<%End if%>
	<%If Session("INV_CREATE_CONTAINER" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Create%20inventory%20container" onclick="OpenDialog('../gui/CreateOrEditContainer.asp?GetData=new&amp;', 'ConDiag', 2); return false" target="_top">New Container</a>
	<%End if%>
	<%If Session("INV_ORDER_CONTAINER" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Order%20New%20Substance" onclick="OpenDialog('/cheminv/custom/gui/OrderContainer.asp?GetData=new&amp;', 'ConDiag', 2); return false" target="_top">Order New Substance</a>
	<%End if%>
	<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Admin Menu" onclick="OpenDialog('../gui/menu.asp', 'MenuDiag', 2); return false" target="_top">Tasks</a>
	<%End if%>
	|
	<a class="MenuLink" HREF="Help" onclick="OpenDialog('<%=Application("HelpFile")%>', 'help', 2); return false">Help</a>
	<%if Len(Request.Cookies("CS_SEC_UserName")) > 0 then%>
	|
	<a class="MenuLink" HREF="/cs_security/home.asp" target="_top">Home</a>
	<%End if%>
	|
	<a class="MenuLink" HREF="/cheminv/logoff.asp" target="_top">Log Off</a>
<%If Application("Admin_required") then%>
	<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
<%End if%>
