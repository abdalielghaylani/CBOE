<%
'-- check if this is a trash can location
bTrashLocation = false
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".IsTrashLocation", adCmdStoredProc)	
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adnumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pLocationID", adnumeric, 1, 0, LocationID)
Cmd.Execute()
IsTrashLocation = Trim(Cmd.Parameters("RETURN_VALUE"))
if IsTrashLocation = 1 then bTrashLocation = true

ulID = "nav"
if Session("bMultiSelect") then
%>
    <div class="dropDownMenuControl">
    <ul id="nav">
        <li><a href="#" title="Select All" onclick="CheckAll(true); return false" class="contextMenu" onmouseover="this.className = 'menuOn'" onmouseout="this.className = 'contextMenu';" >Select All</a></li>
        <li><a href="#" title="Clear All" onclick="CheckAll(false); return false" class="contextMenu" onmouseover="this.className = 'menuOn'" onmouseout="this.className = 'contextMenu';" >Clear All</a></li>
        <li><a href="/cheminv/gui/<%=Session("TabFrameURL")%>?containerCount=0&clear=1" target="TabFrame" title="Clear All" class="contextMenu" onmouseover="this.className = 'menuOn'" onmouseout="this.className = 'contextMenu';">Cancel MultiSelect</a></li>
    </ul>
    </div>
<%
elseif reconcile then%>
<span id="Span1" style="visibility=hidden"></span>

<%
else
    '--plate list links
    if showInList = "plates" then
        numLinks = 9
        'Dim arrLinks()
        redim arrLinks(numLinks,7)
        '-- 1st dimension is on/off
        '-- 2nd dimension is "on" link href
        '-- 3rd dimension is link text
        '-- 4th dimension is dialog size
        '--     if this value is set to "NULL" then the link will not open a dialog
        '-- 5th dimension is show "off" link switch
        '-- 6th dimension is link category
        '-- 7th dimension is link title
        'Dim arrCategories(1)
        'redim arrCategories(1)
        'arrCategories = null
        'arrCategories(0) = "View,NULL"
        arrCategories = array("View","NULL")

        '-- set defaults
        for i = 0 to ubound(arrLinks)
    	    '-- all links are off by default
	        arrLinks(i,0) = 0
    	    '-- all dialog sizes are 1 by default
	        arrLinks(i,3) = 1
        next

        '-- build the list of links
        arrLinks(0,1) = "BuildList.asp?view=3&multiSelect=1&" &QS
        arrLinks(0,2) = "Multi-Select"
        arrLinks(0,3) = "NULL"
        arrLinks(0,5) = arrCategories(0)
        arrLinks(0,6) = "Multi-Select"

        arrLinks(1,1) = "BuildList.asp?view=0&" & QS
        arrLinks(1,2) = "Large Icons"
        arrLinks(1,3) = "NULL"
        arrLinks(1,5) = arrCategories(0)
        arrLinks(1,6) = "Large Icons"

        arrLinks(2,1) = "BuildList.asp?view=1&" & QS
        arrLinks(2,2) = "Small Icons"
        arrLinks(2,3) = "NULL"
        arrLinks(2,5) = arrCategories(0)
        arrLinks(2,6) = "Small Icons"

        arrLinks(3,1) = "BuildList.asp?view=3&" & QS
        arrLinks(3,2) = "Details"
        arrLinks(3,3) = "NULL"
        arrLinks(3,5) = arrCategories(0)
        arrLinks(3,6) = "Details"

        arrLinks(4,1) = "columnPicker2.asp?ArrayID=2&showplates=" & showplates
        arrLinks(4,2) = "Column Chooser"
        arrLinks(4,3) = "4"
        arrLinks(4,5) = arrCategories(0)
        arrLinks(4,6) = "Choose columns for detail view"

        arrLinks(5,1) = "/cheminv/gui/reconcileLocation_frset.asp?LocationID=" & LocationID & "&MoveMissingContainers=0"
        arrLinks(5,2) = "Update Contents"
        arrLinks(5,3) = "5"
        arrLinks(5,5) = arrCategories(1)
        arrLinks(5,6) = "Verify which plates are actually at this location"

        arrLinks(6,1) = "/cheminv/gui/reconcileLocation_frset.asp?LocationID=" & LocationID & "&MoveMissingContainers=1&showInList=" & showInList
        arrLinks(6,2) = "Rectify Contents"
        arrLinks(6,3) = "5"
        arrLinks(6,5) = arrCategories(1)
        arrLinks(6,6) = "Verify which plates are actually at this location and remove missing containers"

        arrLinks(7,1) = "/cheminv/Gui/CreateReport_frset.asp?LocationID=" & LocationID & "&CompoundID=" & CompoundID & "&PlateID=" & PlateID & "&showInList=" & showInList
        arrLinks(7,2) = "Print Report"
        arrLinks(7,3) = "2"
        arrLinks(7,5) = arrCategories(1)
        arrLinks(7,6) = "Print Report"

        arrLinks(8,1) = "/cheminv/Gui/EmptyTrash.asp?LocationID=" & LocationID & "&trashType=plates"
        arrLinks(8,2) = "Empty Trash"
        arrLinks(8,3) = "1"
        arrLinks(8,5) = arrCategories(1)
        arrLinks(8,6) = "Delete all plates in this location"

        '-- turn links on if appropriate
        If Session("INV_DELETE_PLATE" & "Cheminv") OR Session("INV_MOVE_PLATE" & "Cheminv") OR Session("INV_CHECKOUT_CONTAINER" & "Cheminv") OR Session("INV_CHECKIN_CONTAINER" & "Cheminv") OR Session("INV_RETIRE_PLATE" & "Cheminv") then
	        arrLinks(0,0) = 1
        end if
        arrLinks(1,0) = 1
        arrLinks(2,0) = 1
        arrLinks(3,0) = 1
        arrLinks(4,0) = 1
		If Session("INV_UPDATE_LOCATION_CONTENTS" & "Cheminv") then
            arrLinks(5,0) = 1
    	End if
		If Session("INV_RECTIFY_LOCATION_CONTENTS" & "Cheminv") then
            arrLinks(6,0) = 1
    	End if
		If Session("INV_PRINT_REPORT" & "Cheminv") then
            arrLinks(7,0) = 1
    	End if
        If Session("INV_DELETE_PLATE" & "Cheminv") and bTrashLocation then
            arrLinks(8,0) = 1
	    end if
	        
        '-- show the links
        ShowMenuLinks arrLinks, arrCategories
 %>
<!--

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
-->		
<%
    '--container list links
    else
        numLinks = 8
        'Dim arrLinks()
        redim arrLinks(numLinks,7)
        '-- 1st dimension is on/off
        '-- 2nd dimension is "on" link href
        '-- 3rd dimension is link text
        '-- 4th dimension is dialog size
        '--     if this value is set to "NULL" then the link will not open a dialog
        '-- 5th dimension is show "off" link switch
        '-- 6th dimension is link category
        '-- 7th dimension is link title
        'Dim arrCategories(1)
        'redim arrCategories(1)
        'arrCategories = null
        'arrCategories(0) = "View,NULL"
        arrCategories = array("View","NULL")

        '-- set defaults
        for i = 0 to ubound(arrLinks)
    	    '-- all links are off by default
	        arrLinks(i,0) = 0
    	    '-- all dialog sizes are 1 by default
	        arrLinks(i,3) = 1
        next

        '-- build the list of links
        arrLinks(0,1) = "BuildList.asp?view=3&multiSelect=1&" & QS
        arrLinks(0,2) = "Multi-Select"
        arrLinks(0,3) = "NULL"
        arrLinks(0,5) = arrCategories(0)
        arrLinks(0,6) = "Multi-Select"

        arrLinks(1,1) = "BuildList.asp?view=0&" & QS
        arrLinks(1,2) = "Large Icons"
        arrLinks(1,3) = "NULL"
        arrLinks(1,5) = arrCategories(0)
        arrLinks(1,6) = "Large Icons"

        arrLinks(2,1) = "BuildList.asp?view=1&" & QS
        arrLinks(2,2) = "Small Icons"
        arrLinks(2,3) = "NULL"
        arrLinks(2,5) = arrCategories(0)
        arrLinks(2,6) = "Small Icons"

        arrLinks(3,1) = "BuildList.asp?view=3&" & QS
        arrLinks(3,2) = "Details"
        arrLinks(3,3) = "NULL"
        arrLinks(3,5) = arrCategories(0)
        arrLinks(3,6) = "Details"

        arrLinks(4,1) = "columnPicker2.asp?ArrayID=2&showplates=" & showplates
        arrLinks(4,2) = "Column Chooser"
        arrLinks(4,3) = "4"
        arrLinks(4,5) = arrCategories(0)
        arrLinks(4,6) = "Choose columns for detail view"

        arrLinks(5,1) = "/cheminv/gui/reconcileLocation_frset.asp?LocationID=" & LocationID & "&MoveMissingContainers=1&showInList=" & showInList
        arrLinks(5,2) = "Rectify Contents"
        arrLinks(5,3) = "5"
        arrLinks(5,5) = arrCategories(1)
        arrLinks(5,6) = "Rectify Contents"

        arrLinks(6,1) = "/cheminv/Gui/CreateReport_frset.asp?LocationID=" & LocationID & "&CompoundID=" & CompoundID & "&ContainerID=" & ContainerID & "&showInList=" & showInList
        arrLinks(6,2) = "Print Report"
        arrLinks(6,3) = "2"
        arrLinks(6,5) = arrCategories(1)
        arrLinks(6,6) = "Print Report"

        arrLinks(7,1) = "/cheminv/Gui/EmptyTrash.asp?LocationID=" & LocationID & "&trashType=containers"
        arrLinks(7,2) = "Empty Trash"
        arrLinks(7,3) = "1"
        arrLinks(7,5) = arrCategories(1)
        arrLinks(7,6) = "Delete all containers in this location"

        '-- turn links on if appropriate
        If Session("INV_DELETE_CONTAINER" & "Cheminv") OR Session("INV_MOVE_CONTAINER" & "Cheminv") OR Session("INV_CHECKOUT_CONTAINER" & "Cheminv") OR Session("INV_CHECKIN_CONTAINER" & "Cheminv") OR Session("INV_RETIRE_CONTAINER" & "Cheminv") then
	        arrLinks(0,0) = 1
        end if
        arrLinks(1,0) = 1
        arrLinks(2,0) = 1
        arrLinks(3,0) = 1
        arrLinks(4,0) = 1
		If Session("INV_RECTIFY_LOCATION_CONTENTS" & "Cheminv") then
            arrLinks(5,0) = 1
    	End if
		If Session("INV_PRINT_REPORT" & "Cheminv") then
            arrLinks(6,0) = 1
    	End if
		If Session("INV_DELETE_CONTAINER" & "Cheminv") and bTrashLocation then
            arrLinks(7,0) = 1
	    end if
	        
        '-- show the links
        ShowMenuLinks arrLinks, arrCategories
        

    End if

End if
%>
