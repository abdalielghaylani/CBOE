	<a class="MenuLink" HREF="../inputtoggle.asp?dataaction=db&amp;dbname=cheminv" target="_top">Search</a>
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
<!--
	|
    <a class="MenuLink" HREF="../inputtoggle.asp?dbname=cheminv&amp;GotoCurrentLocation=true" target="_top">Search Results</a>
-->	

    |
    <!--CSBR ID : 126848 : sjacob Comments : Changed the 'My Requests' caption to 'My Batch Requests'-->
	<a class="MenuLink" HREF="View My Batch Requests" onclick="OpenDialog('../gui/ViewUserBatchRequests.asp', 'LocDiag', 2); return false">My Batch Requests</a>

	<%If Session("INV_DELETE_CONTAINER" & "Cheminv") OR Session("INV_MOVE_CONTAINER" & "Cheminv") OR Session("INV_CHECKOUT_CONTAINER" & "Cheminv") OR Session("INV_CHECKIN_CONTAINER" & "Cheminv") OR Session("INV_RETIRE_CONTAINER" & "Cheminv") then%>
	|
	<a class="MenuLink" HREF="/cheminv/gui/multiScan_frset.asp" target="_top">Scan</a>	
	<%end if%>	
	<%If Session("INV_CREATE_CONTAINER" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Create%20inventory%20container" onclick="OpenDialog('../gui/CreateOrEditContainer.asp?GetData=new&amp;', 'ConDiag', 2); return false" target="_top">New Container</a>
	|
	<%if Application("SHOWUSERPREFERENCESLINK") then %>
    <a class="MenuLink" HREF="Create%20inventory%20container" onclick="OpenDialog('../gui/UserPreferences.asp?Usersetting=true&GetData=new&amp;', 'ConDiag', 2); return false" target="_top">User Preferences</a>
	|
    <%end if %>
	<a class="MenuLink" HREF="Create Container Samples" onclick="OpenDialog('../gui/CreateContainerSample.asp?Action=new&amp;', 'ConDiag', 2); return false" target="_top">New Samples</a>
	<%End if%>
	<%if Application("ShowRequestSample") AND Session("INV_CHECKOUT_CONTAINER" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Receive%20Order"  onclick="OpenDialog('/Cheminv/GUI/ReceiveFindOrder.asp', 'RecDiag', 2); return false" target="_top">Receive Order</a>
	<%end if%>						
	<%If Session("INV_ORDER_CONTAINER" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Order%20New%20Substance" onclick="OpenDialog('/cheminv/gui/OrderContainer.asp?GetData=new&amp;', 'ConDiag', 2); return false" target="_top">Order Container</a>
	<%End if%>
	<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
	|
	<a class="MenuLink" HREF="Admin Menu" onclick="OpenDialog('../gui/menu.asp', 'MenuDiag', 7); return false" target="_top">Tasks</a>
	<%End if%>
	|
	<%if Len(Request.Cookies("CS_SEC_UserName")) > 0 then%>
	<a class="MenuLink" HREF="/cs_security/home.asp" target="_top">Home</a>
	|
	<%End if%>
	<a class="MenuLink" HREF="Help" onclick="OpenDialog('<%=Application("HelpFile")%>', 'help', 2); return false">Help</a>
	|
	<a class="MenuLink" HREF="/cheminv/logoff.asp" target="_top">Log Off</a>

<%If Application("Admin_required") then%>
	<br><br>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%>
<%End if%>
