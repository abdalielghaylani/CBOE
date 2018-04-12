<% 
numLinks = 19
'Dim arrLinks()
redim arrLinks(numLinks,7)
'-- 1st dimension is on/off
'-- 2nd dimension is "on" link href
'-- 3rd dimension is link text
'-- 4th dimension is dialog size
'-- 5th dimension is show "off" link switch
'-- 6th dimension is link category
'-- 7th dimension is link title

arrCategories = array("Update","Create","Obtain","Other")

'-- set defaults
for i = 0 to ubound(arrLinks)
	'-- all links are off by default
	arrLinks(i,0) = 0
	'-- all dialog sizes are 1 by default
	arrLinks(i,3) = 1
next

'-- build the list of links
arrLinks(0,1) = "/cheminv/gui/ChangeQty.asp?ContainerID=" & ContainerID & "&QtyRemaining=" & QtyRemaining & "&UOMAbv=" & UOMAbv & "&TotalQtyReserved=" & TotalQtyReserved
arrLinks(0,2) = "Change Qty"
arrLinks(0,3) = "1"
arrLinks(0,5) = arrCategories(0)
arrLinks(0,6) = "Change Quantity"

arrLinks(1,1) = "/ChemInv/GUI/UpdateContainer.asp?editMode=single&action=dataEntry&containerFields=Container Status:container_status_id_fk&iContainerID=" & ContainerID
arrLinks(1,2) = "Change Status"
arrLinks(1,3) = "1"
arrLinks(1,5) = arrCategories(0)
arrLinks(1,6) = "Change Status"

arrLinks(2,1) = "/Cheminv/GUI/CheckOut.asp?action=out&ContainerID=" & ContainerID & "&LocationID=" &  LocationID & "&LocationName=" & Replace(LocationName,"'","\'")
arrLinks(2,2) = "Check Out"
arrLinks(2,3) = "1"
arrLinks(2,5) = arrCategories(2)
arrLinks(2,6) = "Check Out"

arrLinks(3,1) = "/Cheminv/GUI/CheckOut.asp?action=in&ContainerID=" & ContainerID & "&LocationID=" &  LocationID
arrLinks(3,2) = "Check In"
arrLinks(3,3) = "1"
arrLinks(3,5) = arrCategories(2)
arrLinks(3,6) = "Check In"

arrLinks(4,1) = "/Cheminv/GUI/ManageOrders.asp?action=AssignContainer&ContainerID=" & ContainerID
arrLinks(4,2) = "Assign to Order"
arrLinks(4,3) = "1"
arrLinks(4,5) = arrCategories(3)
arrLinks(4,6) = "Assign to Order"


if len(BatchID1)>0 then
    UseBatch="1"
elseif len(BatchID2)>0 then 
    UseBatch="2"
elseif len(BatchID3)>0 then 
    UseBatch="3"
else
    UseBatch=""
end if  
arrLinks(5,1) = "/Cheminv/GUI/RequestSample.asp?action=create&ContainerID=" & ContainerID &  "&UOMAbv=" & UOMAbv & "&QtyRequired=" & QtyAvailable & "&BatchID1=" & BatchID1 & "&BatchID2=" & BatchID2 & "&BatchID3=" & BatchID3 & "&UseBatch=" & UseBatch
arrLinks(5,2) = "Request"
arrLinks(5,3) = "2"
arrLinks(5,4) = 1
arrLinks(5,5) = arrCategories(2)
arrLinks(5,6) = "Request"

arrLinks(6,1) = "/Cheminv/GUI/CreateOrEditContainer.asp?isCopy=true&getData=db&amp;ContainerID=" & ContainerID
arrLinks(6,2) = "Copy Container"
arrLinks(6,3) = "2"
arrLinks(6,5) = arrCategories(1)
arrLinks(6,6) = "Copy Container"
	
arrLinks(7,1) = "/Cheminv/GUI/Certify.asp?ContainerID=" & ContainerID
arrLinks(7,2) = "Certify Container"
arrLinks(7,3) = "1"
arrLinks(7,4) = 1
arrLinks(7,5) = arrCategories(3)
arrLinks(7,6) = "Certify Container"

arrLinks(8,1) = "/Cheminv/GUI/AllotContainer.asp?action=split&amp;ContainerID=" & ContainerID & "&QtyRemaining=" & QtyRemaining & "&UOMAbv=" & UOMAbv
arrLinks(8,2) = "Split Container"
arrLinks(8,3) = "1"
arrLinks(8,5) = arrCategories(1)
arrLinks(8,6) = "Split Container"

arrLinks(9,1) = "/Cheminv/GUI/AllotContainer.asp?action=sample&amp;ContainerID=" & ContainerID & "&QtyRemaining=" & QtyRemaining & "&UOMAbv=" & UOMAbv
arrLinks(9,2) = "Create Samples"
arrLinks(9,3) = "1"
arrLinks(9,5) = arrCategories(1)
arrLinks(9,6) = "Create Samples"

arrLinks(10,1) = "/Cheminv/GUI/MergeContainers.asp?ContainerID=" & ContainerID & "&ParentContainerID=" & ParentContainerID & "&Action=SelectIDs"
arrLinks(10,2) = "Merge Containers"
arrLinks(10,3) = "1"
arrLinks(10,5) = arrCategories(1)
arrLinks(10,6) = "Merge Containers"

arrLinks(11,1) = "/ChemInv/GUI/ReorderContainer.asp?getData=db&amp;ContainerID=" & ContainerID & "&SupplierID=" & SupplierID & "&CatNum=" & SupplierCatNum
arrLinks(11,2) = "Reorder Container"
arrLinks(11,3) = "2"
arrLinks(11,5) = arrCategories(1)
arrLinks(11,6) = "Reorder Container"

arrLinks(12,1) = "/ChemInv/GUI/CreateOrEditContainer.asp?isEdit=true&getData=db&amp;ContainerID=" & ContainerID
arrLinks(12,2) = "Edit Container"
arrLinks(12,3) = "2"
arrLinks(12,5) = arrCategories(0)
arrLinks(12,6) = "Edit Container"

arrLinks(13,1) = "/Cheminv/GUI/MoveContainer.asp?ContainerID=" & ContainerID
arrLinks(13,2) = "Move Container"
arrLinks(13,3) = "1"
arrLinks(13,5) = arrCategories(0)
arrLinks(13,6) = "Move Container"

arrLinks(14,1) = "/Cheminv/GUI/RetireContainer.asp?ContainerID=" & ContainerID & "&QtyRemaining=" & QtyRemaining & "&ContainerName=" & Barcode
arrLinks(14,2) = "Retire Container"
arrLinks(14,3) = "1"
arrLinks(14,5) = arrCategories(0)
arrLinks(14,6) = "Retire Container"

arrLinks(15,1) = "/Cheminv/GUI/DeleteContainer.asp?ContainerID=" & ContainerID
arrLinks(15,2) = "Delete Container"
arrLinks(15,3) = "1"
arrLinks(15,5) = arrCategories(0)
arrLinks(16,6) = "Delete Container"

arrLinks(16,1) = "/Cheminv/GUI/history.asp?containerid=" & ContainerID
arrLinks(16,2) = "History"
arrLinks(16,3) = "2"
arrLinks(16,5) = arrCategories(3)
arrLinks(16,6) = "History"

arrLinks(17,1) = "/cheminv/gui/LineageTree.asp?refresh=1&assetType=container&selectedID=" & ContainerID
arrLinks(17,2) = "Lineage"
arrLinks(17,3) = "5"
arrLinks(17,5) = arrCategories(3)
arrLinks(17,6) = "Lineage"

arrLinks(18,1) = "/Cheminv/GUI/PrintLabelOption.asp?ShowInList=containers&ContainerID=" & ContainerID
arrLinks(18,2) = "Print Label"
arrLinks(18,3) = "2"
arrLinks(18,5) = arrCategories(3)
arrLinks(18,6) = "Print Label"

arrLinks(19,1) = "/cheminv/gui/manageLinks.asp?FK_value=" & ContainerID & "&fk_name=container_id&Table_Name=inv_containers"
arrLinks(19,2) = "Manage Links"
arrLinks(19,3) = "1"
arrLinks(19,5) = arrCategories(3)
arrLinks(19,6) = "Manage Links"

'-- turn links on if appropriate
If Session("INV_CHANGEQTY_CONTAINER" & dbkey) then
	arrLinks(0,0) = 1
end if

If Session("INV_CHANGE_STATUS_CONTAINER" & dbkey) then
	arrLinks(1,0) = 1
end if

If Session("INV_CHECKOUT_CONTAINER" & dbkey) then
	arrLinks(2,0) = 1
end if

If Session("INV_CHECKIN_CONTAINER" & dbkey) then
	arrLinks(3,0) = 1
end if

If Session("INV_APPROVE_CONTAINER" & dbkey) then
	arrLinks(4,0) = 1
end if

If (Application("AllowRequests")) then
    '-- turn container requests and sample requests on/off
    allowContainerRequest = "0"
    allowSampleRequest = "0"
'-- CSBR-138430-SMathur -  Relpaced Session("INV_CHECKOUT_CONTAINER" & dbkey) with Session("INV_CHECKOUT_CONTAINER" & dbkey)
    if (IsNull(RequestID) AND (not Application("RequireApprovalForSamples") or ContainerStatusID = "1")) then
        allowContainerRequest="1"
        arrLinks(5,1) = arrLinks(5,1) & "&allowContainerRequest=1"
    end if
'-- CSBR-138430-SMathur- Relpaced Session("INV_CHECKOUT_CONTAINER" & dbkey) with Session("INV_CHECKOUT_CONTAINER" & dbkey)
    if Application("ShowRequestSample") and Session("INV_SAMPLE_REQUEST" & dbkey) and (not Application("RequireApprovalForSamples")  or cint(ContainerStatusID) = cint(Application("StatusApproved"))) then
        allowSampleRequest="1"
        arrLinks(5,1) = arrLinks(5,1) & "&allowSampleRequest=1"
    end if        
    if (allowContainerRequest = "1" or allowSampleRequest = "1") And (LocationType <> "502") then  '-- CBOE-1401 turn container requests and sample requests on/off if container is from trash location.
        arrLinks(5,0) = 1
    else
        arrLinks(5,4) = 0
    end if
end if       

If Session("INV_CREATE_CONTAINER" & dbkey) then
	arrLinks(6,0) = 1
End if

if Application("ShowCertify") then
	If Session("INV_CERTIFY_CONTAINER" & dbkey) then
		if IsEmpty(Session("DateCertified")) then
			arrLinks(7,0) = 1
		end if
	end if
else
	arrLinks(7,4) = 0
End if

If Session("INV_CREATE_CONTAINER" & dbkey) and Application("ShowSplit") then
	arrLinks(8,0) = 1
End if

If Session("INV_CREATE_CONTAINER" & dbkey) and Application("ShowSample") then
	arrLinks(9,0) = 1
End if

If Session("INV_CREATE_CONTAINER" & dbkey) or Application("ShowMerge") then
	if not isNull(ParentContainerID) and ParentContainerID <> "" then
		arrLinks(10,0) = 1
	end if
End if

If Session("INV_REORDER_CONTAINER" & dbkey) then			
	'Only show the "Reorder" link if this is not a registry compound, and if the location is not "Trash Can" (LocTypeID 502)
	If (Len(RegID) = 0) And (LocationType <> "502") Then
		arrLinks(11,0) = 1
	End If
End if

If Session("INV_EDIT_CONTAINER" & dbkey) then
	arrLinks(12,0) = 1
End if

If Session("INV_MOVE_CONTAINER" & dbkey) then
	arrLinks(13,0) = 1
End if

If Session("INV_RETIRE_CONTAINER" & dbkey) then
	arrLinks(14,0) = 1
End if

If Session("INV_DELETE_CONTAINER" & dbkey) and cdbl(Session("LocationID"))<>3 then 'do not show delete link in Trash location
	arrLinks(15,0) = 1
End if

If Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then
	arrLinks(16,0) = 1
end if

arrLinks(17,0) = 1

If Session("INV_PRINT_LABEL_CONTAINER" & dbkey) then
	arrLinks(18,0) = 1
end if

If Session("INV_MANAGE_LINKS" & dbkey) then
	arrLinks(19,0) = 1
end if
' Checking Authority
if  Application("ENABLE_OWNERSHIP")="TRUE" AND cint(Session("isAuthorised")) =0 then 
    for i=0 to 19
        if i<>16 and i<>17 and i<>18 and i<>5 then
            arrLinks(i,0) = 0
            arrLinks(i,4) = 1
        end if 
    next 
end if  

'-- show the links
ShowMenuLinks arrLinks, arrCategories
WriteContextMenu arrLinks, arrCategories

sub ShowClassicLinks(byref arrLinks)
	Response.Write "<table border=""0"" cellspacing=""0"" cellpadding=""2"" width=""100%""><tr><td align=""right"" valign=""top"" nowrap>"
	isFirst = true
	for i = 0 to ubound(arrLinks)
       if arrLinks(i,0) = 1 and arrLinks(i,2) <> "Split Container" then
			if not isFirst then Response.Write " | " 
            Response.Write "<a class=""MenuLink"" HREF=""#"" onclick=""OpenDialog('" & arrLinks(i,1) & "', 'Diag', " & arrLinks(i,3) & "); return false"">" & arrLinks(i,2) & "</a>"
			isFirst = false
        'CBOE-1399: after splitting container, page should be refreshed to avoid confusion of selection of original container
       elseif arrLinks(i,0) = 1 and arrLinks(i,2) = "Split Container" then
            if not isFirst then Response.Write " | " 
            Response.Write "<a class=""MenuLink"" HREF=""#"" onclick=""OpenDialog('" & arrLinks(i,1) & "', 'Diag', " & arrLinks(i,3) & "); return true"">" & arrLinks(i,2) & "</a>"
			isFirst = false
		elseif arrLinks(i,4) = 1 then
			if not isFirst then Response.Write " | " 
			Response.Write "<a class=""MenuLink"" disabled>" & arrLinks(i,2) & "</a>"
			isFirst = false
		end if
	next
	Response.Write "</td></tr></table>"
	
end sub

%>
