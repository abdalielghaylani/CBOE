<%
bTrashLocation = false
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".IsTrashLocation", adCmdStoredProc)	
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adnumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pLocationID", adnumeric, 1, 0, LocationID)
Cmd.Execute()
IsTrashLocation = Trim(Cmd.Parameters("RETURN_VALUE"))
if IsTrashLocation = 1 then bTrashLocation = true
'plate list links
if showInList = "plates" then
%>
<%if Session("bMultiSelect") then%>
		<span id="multiSelectLink" style="visibility=hidden">
		<A CLASS="MenuLink" HREF="#" onclick="CheckAll(true); return false">Select All</A>
		|
		<A CLASS="MenuLink" HREF="#" onclick="CheckAll(false); return false">Clear All</A>
		|
		<a class="MenuLink" HREF="/cheminv/gui/<%=Session("TabFrameURL")%>?containerCount=0&clear=1" target="TabFrame">Cancel MultiSelect</a>					
		</span>
<%elseif reconcile then%>
		<span id="multiSelectLink" style="visibility=hidden"></span>
<%Else%>
		<%If Session("INV_DELETE_PLATE" & "Cheminv") OR Session("INV_MOVE_PLATE" & "Cheminv") OR Session("INV_CHECKOUT_CONTAINER" & "Cheminv") OR Session("INV_CHECKIN_CONTAINER" & "Cheminv") OR Session("INV_RETIRE_PLATE" & "Cheminv") then%>	
			<span id="multiSelectLink" style="visibility=hidden">
			<A CLASS="MenuLink" HREF="BuildList.asp?view=3&multiSelect=1&<%=QS%>" >Multi Select</A>
			|
		</span>
		<%end if%>
			<A CLASS="MenuLink" HREF="BuildList.asp?view=0&<%=QS%>" >Large Icons</A>
			|
			<A CLASS="MenuLink" HREF="BuildList.asp?view=1&<%=QS%>" >Small Icons</A>
			|
			<A CLASS="MenuLink" HREF="BuildList.asp?view=3&<%=QS%>" >Details</A>
			|
			<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('columnPicker2.asp?ArrayID=2&showplates=<%=showplates%>', 'CCDiag', 4); return false">Column Chooser</a>
		<%If Session("INV_UPDATE_LOCATION_CONTENTS" & "Cheminv") then%>	
			|
			<a class="MenuLink" HREF="Verify which plates are actually at this location" onclick="OpenDialog('/cheminv/gui/reconcileLocation_frset.asp?LocationID=<%=LocationID%>&MoveMissingContainers=0', 'RecDiag', 5); return false" target="_top">Update Contents</a>
		<%End if%>	
		<%If Session("INV_RECTIFY_LOCATION_CONTENTS" & "Cheminv") then%>	
			|
			<a class="MenuLink" HREF="Verify which plates are actually at this location and remove missing containers" onclick="OpenDialog('/cheminv/gui/reconcileLocation_frset.asp?LocationID=<%=LocationID%>&MoveMissingContainers=1&showInList=<%=showInList%>', 'RecDiag', 5); return false" target="_top">Rectify Contents</a>
		<%End if%>
		<%If Session("INV_PRINT_REPORT" & "Cheminv") then%>	
			|
			<A CLASS="MenuLink" HREF="Print Report" onclick="OpenDialog('/cheminv/Gui/CreateReport_frset.asp?LocationID=<%=LocationID%>&CompoundID=<%=CompoundID%>&PlateID=<%=PlateID%>&showInList=<%=showInList%>', 'RptDiag', 2); return false;">Print Report</A>
		<%end if%>
		<%If Session("INV_DELETE_PLATE" & "Cheminv") and bTrashLocation then%>
			|
			<A CLASS="MenuLink" HREF="Delete all plates in this location"  onclick="OpenDialog('/cheminv/Gui/EmptyTrash.asp?LocationID=<%=LocationID%>&trashType=plates', 'RecDiag', 1); return false" target="_top">Empty Trash</A>
		<%end if%>
<%End if%>
<%
'container list links
else
%>
<%if Session("bMultiSelect") then%>
		<span id="multiSelectLink" style="visibility=hidden">
		<A CLASS="MenuLink" HREF="#" onclick="CheckAll(true); return false">Select All</A>
		|
		<A CLASS="MenuLink" HREF="#" onclick="CheckAll(false); return false">Clear All</A>
		|
		<a class="MenuLink" HREF="/cheminv/gui/<%=Session("TabFrameURL")%>?containerCount=0&clear=1" target="TabFrame">Cancel MultiSelect</a>					
		</span>
<%elseif reconcile then%>
		<span id="multiSelectLink" style="visibility=hidden"></span>
<%Else%>
		<%If Session("INV_DELETE_CONTAINER" & "Cheminv") OR Session("INV_MOVE_CONTAINER" & "Cheminv") OR Session("INV_CHECKOUT_CONTAINER" & "Cheminv") OR Session("INV_CHECKIN_CONTAINER" & "Cheminv") OR Session("INV_RETIRE_CONTAINER" & "Cheminv") then%>	
			<span id="multiSelectLink" style="visibility=hidden">
			<A CLASS="MenuLink" HREF="BuildList.asp?view=3&multiSelect=1&<%=QS%>" >Multi Select</A>
			|
		</span>
		<%end if%>
			<A CLASS="MenuLink" HREF="BuildList.asp?view=0&<%=QS%>" >Large Icons</A>
			|
			<A CLASS="MenuLink" HREF="BuildList.asp?view=1&<%=QS%>" >Small Icons</A>
			|
			<A CLASS="MenuLink" HREF="BuildList.asp?view=3&<%=QS%>" >Details</A>
			|
			<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('columnPicker2.asp?ArrayID=2&showplates=<%=showplates%>', 'CCDiag', 4); return false">Column Chooser</a>
		<%If Session("INV_UPDATE_LOCATION_CONTENTS" & "Cheminv") then%>	
			|
			<a class="MenuLink" HREF="Verify which containers are actually at this location" onclick="OpenDialog('/cheminv/gui/reconcileLocation_frset.asp?LocationID=<%=LocationID%>&MoveMissingContainers=0', 'RecDiag', 5); return false" target="_top">Update Contents</a>
		<%End if%>	
		<%If Session("INV_RECTIFY_LOCATION_CONTENTS" & "Cheminv") then%>	
			|
			<a class="MenuLink" HREF="Verify which containers are actually at this location and remove missing containers" onclick="OpenDialog('/cheminv/gui/reconcileLocation_frset.asp?LocationID=<%=LocationID%>&MoveMissingContainers=1&showInList=<%=showInList%>', 'RecDiag', 5); return false" target="_top">Rectify Contents</a>
		<%End if%>
		<%If Session("INV_PRINT_REPORT" & "Cheminv") then%>	
			|
			<A CLASS="MenuLink" HREF="Print Report" ONCLICK="OpenDialog('/cheminv/Gui/CreateReport_frset.asp?LocationID=<%=LocationID%>&CompoundID=<%=CompoundID%>&ContainerID=<%=ContainerID%>&showInList=<%=showInList%>', 'RptDiag', 2); return false;">Print Report</A>
		<%end if%>
		<%If Session("INV_DELETE_CONTAINER" & "Cheminv") and bTrashLocation then%>
			|
			<A CLASS="MenuLink" HREF="Delete all containers and plates in this location"  onclick="OpenDialog('/cheminv/Gui/EmptyTrash.asp?LocationID=<%=LocationID%>&trashType=containers', 'RecDiag', 1); return false" target="_top">Empty Trash</A>
		<%end if%>
<%End if%>
<%End if%>