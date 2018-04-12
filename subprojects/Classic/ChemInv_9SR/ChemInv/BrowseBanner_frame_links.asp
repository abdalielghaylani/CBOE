	<a class="MenuLink" HREF="../inputtoggle.asp?dataaction=db&amp;dbname=cheminv" target="_top">New Search</a>
	|
	<a class="MenuLink" HREF="../inputtoggle.asp?dbname=cheminv&amp;GotoCurrentLocation=true" target="_top">Search Results</a>
	<%If Session("INV_DELETE_CONTAINER" & "Cheminv") OR Session("INV_MOVE_CONTAINER" & "Cheminv") OR Session("INV_CHECKOUT_CONTAINER" & "Cheminv") OR Session("INV_CHECKIN_CONTAINER" & "Cheminv") OR Session("INV_RETIRE_CONTAINER" & "Cheminv") then%>
	|
	<a class="MenuLink" HREF="/cheminv/gui/multiScan_frset.asp" target="_top">Multi Scan</a>	
	<%end if%>	
	<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Create%20a%20new%20inventory%20location" onclick="OpenDialog('../gui/NewLocation.asp', 'LocDiag', 1); return false">New Location</a>
	<%End if%>
	<%If Session("INV_EDIT_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Edit%20an%20inventory%20location" onclick="OpenDialog('../gui/NewLocation.asp?GetData=db&clearReturnURL=1', 'LocDiag', 1); return false">Edit Location</a>
	<%End if%>
	<%If Session("INV_MOVE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Move%20an%20inventory%20location" onclick="OpenDialog('../gui/MoveLocation.asp', 'LocDiag', 1); return false">Move Location</a>
	<%End if%>
	<%If Session("INV_DELETE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Delete%20inventory%20location" onclick="OpenDialog('../gui/DeleteLocation.asp', 'LocDiag', 1); return false" target="_top">Delete Location</a>
	<%End if%>
	<%If Session("INV_CREATE_CONTAINER" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Create%20inventory%20container" onclick="OpenDialog('../gui/CreateOrEditContainer.asp?GetData=new&amp;', 'ConDiag', 2); return false" target="_top">New Container</a>
	<%End if%>
	<%if Application("ShowRequestSample") AND Session("INV_CHECKOUT_CONTAINER" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Receive%20Order"  onclick="OpenDialog('/Cheminv/GUI/ReceiveOrder_frset.asp?clear=1', 'RecDiag', 2); return false" target="_top">Receive Order</a>
	<%end if%>						
	<%If Session("INV_ORDER_CONTAINER" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Order%20New%20Substance" onclick="OpenDialog('/cheminv/gui/OrderContainer.asp?GetData=new&amp;', 'ConDiag', 2); return false" target="_top">Order Container</a>
	<%End if%>
	<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Admin Menu" onclick="OpenDialog('../gui/menu.asp', 'MenuDiag', 2); return false" target="_top">Tasks</a>
	<%End if%>
	|
	<a class="MenuLink" HREF="Help" onclick="OpenDialog('/cheminv/help/help.asp', 'help', 2); return false">Help</a>
	<%if Len(Request.Cookies("CS_SEC_UserName")) > 0 then%>
	|
	<a class="MenuLink" HREF="/cs_security/home.asp" target="_top">Home</a>
	<%End if%>
	|
	<a class="MenuLink" HREF="/cheminv/logoff.asp" target="_top">Log Off</a>
<%If Application("Admin_required") then%>
	<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
<%End if%>
