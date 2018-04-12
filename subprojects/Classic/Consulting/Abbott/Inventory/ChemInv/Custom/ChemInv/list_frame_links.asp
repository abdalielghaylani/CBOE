<%
'plate list links
if showInList = "plates" then
%>
<!-- MCD reconcile shouldn't have menu items -->
<%if Session("bMultiSelect") then%>	
	<span id="multiSelectLink" style="visibility=hidden">
	<A CLASS="MenuLink" HREF="#" onclick="CheckAll(true); return true">Select All</A>
	|
	<A CLASS="MenuLink" HREF="#" onclick="CheckAll(false); return false">Clear All</A>
	|
	<a class="MenuLink" HREF="/cheminv/gui/<%=Session("TabFrameURL")%>?containerCount=0&clear=1" target="TabFrame">Cancel MultiSelect</a>					
	</span>	
<%elseif reconcile then%>
	<span id="multiSelectLink" style="visibility=hidden"></span>
<%Else%>
	<%If Session("INV_DELETE_PLATE" & "Cheminv") then%>	
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
		<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('columnPicker2.asp?ArrayID=2', 'Diag', 550, 450, 200,200); return false">Column Chooser</a>
		|
		<a class="MenuLink" HREF="Reconcile Location" onclick="OpenDialog('/cheminv/gui/reconcileLocation_frset.asp?LocationID=<%=LocationID%>', 'RecDiag', 800, 600, 100, 100); return false" target="_top">Reconcile Location</a>
	<%If Session("INV_PRINT_REPORT" & "Cheminv") then%>	
		|
		<A CLASS="MenuLink" HREF="/cheminv/Gui/CreateReport_frset.asp?LocationID=<%=LocationID%>&CompoundID=<%=CompoundID%>&PlateID=<%=PlateID%>&ShowInList=<%showInList%>" target="RPT">Print Report</A>
	<%end if%>
<%End if%>
<%
'container list links
else
%>
<!-- MCD reconcile shouldn't have menu items -->
<%if Session("bMultiSelect") then%>	
	<span id="multiSelectLink" style="visibility=hidden">
	<A CLASS="MenuLink" HREF="#" onclick="CheckAll(true); return true">Select All</A>
	|
	<A CLASS="MenuLink" HREF="#" onclick="CheckAll(false); return false">Clear All</A>
	|
	<a class="MenuLink" HREF="/cheminv/gui/<%=Session("TabFrameURL")%>?containerCount=0&clear=1" target="TabFrame">Cancel MultiSelect</a>					
	</span>	
<%elseif reconcile then%>
	<span id="multiSelectLink" style="visibility=hidden"></span>
<%Else%>
	<%If Session("INV_DELETE_CONTAINER" & "Cheminv") then%>	
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
		<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('columnPicker2.asp?ArrayID=2', 'Diag', 550, 450, 200,200); return false">Column Chooser</a>
		|
		<a class="MenuLink" HREF="Reconcile Location" onclick="OpenDialog('/cheminv/gui/reconcileLocation_frset.asp?LocationID=<%=LocationID%>', 'RecDiag', 800, 600, 100, 100); return false" target="_top">Reconcile Location</a>
	<%If Session("INV_PRINT_REPORT" & "Cheminv") then%>	
		|
		<A CLASS="MenuLink" HREF="/cheminv/Gui/CreateReport_frset.asp?LocationID=<%=LocationID%>&CompoundID=<%=CompoundID%>&ContainerID=<%=ContainerID%>&ShowInList=<%showInList%>" target="RPT">Print Report</A>
	<%end if%>
<%End if%>
<%End if%>
