<%
isFirst = true
%>

<%If Session("INV_CHANGEQTY_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/cheminv/gui/ChangeQty.asp?ContainerID=<%=ContainerID%>&QtyRemaining=<%=QtyRemaining%>&UOMAbv=<%=UOMAbv%>&TotalQtyReserved=<%=TotalQtyReserved%>&LocationID=<%=LocationID%>', 'Diag', 1); return false">Change Qty</a>
<%end if%>

<%If Session("INV_CHANGE_STATUS_CONTAINER" & dbkey) or Session("INV_EDIT_CONTAINER" & dbkey) then
	call CheckIfFirst()%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/ChemInv/GUI/UpdateContainer.asp?editMode=single&action=dataEntry&containerFields=Container Status:container_status_id_fk&iContainerID=<%=ContainerID%>', 'Diag', 1); return false">Change Status</a>
<%end if%>

<%If Session("INV_CHECKOUT_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CheckOut.asp?action=out&ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID + '&LocationName=<%=Replace(LocationName,"'","\'")%>' , 'Diag', 1); return false">Check Out</a>
<%end if%>

<%If Session("INV_CHECKIN_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CheckOut.asp?action=in&ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID, 'Diag', 1); return false">Check In</a>
<%end if%>

<!--
<%If Application("AllowRequests") AND Session("INV_CHECKOUT_CONTAINER" & dbkey) AND IsNull(RequestID) AND ContainerStatusID = "1" then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/Request.asp?action=create&ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID + '&UOMAbv=<%=UOMAbv%>&QtyRequired=<%=QtyAvailable%>', 'Diag', 1); return false">Request Container</a>

<%end if%>

<%If Application("ShowRequestSample") AND Session("INV_CHECKOUT_CONTAINER" & dbkey) then
	if cint(ContainerStatusID) = cint(Application("StatusApproved")) then
%>	
	<% call CheckIfFirst() %>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RequestSample.asp?action=create&ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID + '&UOMAbv=<%=UOMAbv%>&QtyRequired=<%=QtyAvailable%>', 'Diag', 2); return false">Request Sample</a>
	<%elseif lCase(Application("SHOW_INACTIVE_LINKS")) = "true" then 
	call CheckIfFirst()
	%>
	<a class="MenuLink" disabled>Request Sample</a>
	<%end if%>
	
<%end if%>
-->

<%If Session("INV_CREATE_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CreateOrEditContainer.asp?isCopy=true&getData=db&amp;ContainerID=<%=ContainerID%>', 'Diag', 2); return false">Copy Container</a>
<%End if%>

<%If Session("INV_CERTIFY_CONTAINER" & dbkey) and Application("ShowCertify") then
	call CheckIfFirst()
	if not IsEmpty(Session("DateCertified")) then
%>			
		<a class="MenuLink" disabled>Certify Container</a>
	<%else%>	
		<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/Certify.asp?ContainerID=<%=ContainerID%>', 'Diag', 1); return false">Certify Container</a>
	<%end if%>
<%End if%>

<%If Session("INV_CREATE_CONTAINER" & dbkey) and Application("ShowSplit") then
	call CheckIfFirst()
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/AllotContainer.asp?action=split&amp;ContainerID=<%=ContainerID%>&QtyRemaining=<%=QtyRemaining%>&UOMAbv=<%=UOMAbv%>', 'Diag', 1); return false">Split Container</a>
<%End if%>

<%If Session("INV_CREATE_CONTAINER" & dbkey) and Application("ShowSample") then
	call CheckIfFirst()
%>			
	<!--<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/AllotContainer.asp?action=sample&amp;ContainerID=<%=ContainerID%>&QtyRemaining=<%=QtyRemaining%>&UOMAbv=<%=UOMAbv%>', 'Diag', 1); return false">Create Samples</a> | -->
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CreateContainerSample.asp?action=sample&amp;ContainerID=<%=ContainerID%>&QtyRemaining=<%=QtyRemaining%>&UOMAbv=<%=UOMAbv%>&ContainerName=<%=ContainerName%>&ContainerSize=<%=QtyMax%>&LocationID=<%=LocationID%>', 'ConDiag', 2); return false">Re-Aliquot Samples</a>
	<!--<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RequestBatch.asp?batchid=5&UOMAbv=<%=UOMAbv%>', 'ConDiag', 2); return false">Request</a>-->
	<!--<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/AddBatchToRequest.asp', 'ConDiag', 2); return false">Add Containers To Request</a>-->
<%End if%>

<%If Session("INV_CREATE_CONTAINER" & dbkey) or Application("ShowMerge") then
	if not isNull(ParentContainerID) and ParentContainerID <> "" then
		call CheckIfFirst()
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/MergeContainers.asp?ContainerID=<%=ContainerID%>&ParentContainerID=<%=ParentContainerID%>&Action=SelectIDs', 'Diag', 1); return false">Merge Containers</a>
<%
	end if
End if
%>

<%
If Session("INV_REORDER_CONTAINER" & dbkey) then			
	'Only show the "Reorder" link if this is not a registry compound, and if the location is not "Trash Can" (LocTypeID 502)
	If (Len(RegID) = 0) And (LocationType <> "502") Then
		call CheckIfFirst()
%>
	<a class="MenuLink" HREF="Reorder Container" onclick="OpenDialog('/ChemInv/GUI/ReorderContainer.asp?getData=db&amp;ContainerID=<%=ContainerID%>&SupplierID=<%=SupplierID%>&CatNum=<%=SupplierCatNum%>', 'Diag', 2); return false">Reorder Container</a>
<%
	End If
End if
%>

<%If Session("INV_EDIT_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>			
	<a class="MenuLink" id="EditContainerLnk" HREF="Edit inventory container" onclick="OpenDialog('../GUI/CreateOrEditContainer.asp?isEdit=true&getData=db&amp;ContainerID=<%=ContainerID%>', 'Diag', 2); return false">Edit Container</a>
<%End if%>

<%If Session("INV_MOVE_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/MoveContainer.asp?ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName, 'Diag', 1); return false">Move Container</a>
<%End if%>

<%If Session("INV_RETIRE_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RetireContainer.asp?ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&QtyRemaining=<%=QtyRemaining%>', 'Diag', 1); return false">Retire Container</a>
<%End if%>

<%If Session("INV_DELETE_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/DeleteContainer.asp?ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName, 'Diag', 1); return false">Delete Container</a>		
<%End if%>

<%If Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then
	call CheckIfFirst()
%>		
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/history.asp?containerid=<%=ContainerID%>', 'Diag', 2); return false">History</a>		
<%end if%>

<%If Session("INV_PRINT_LABEL_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/PrintLabel.asp?ContainerID=<%=ContainerID%>', 'Diag', 1); return false">Print Label</a>		
<%end if%>

<%
sub CheckIfFirst()
	if isFirst then
		isFirst = false
	else
		Response.Write " | "
	end if
end sub
%>