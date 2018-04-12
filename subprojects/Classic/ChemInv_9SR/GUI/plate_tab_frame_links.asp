<%
isFirst = true

If false and  Application("AllowRequests") AND IsNull(RequestID) AND PlateStatusID = "1" then
	call CheckIfFirst()	
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/Request.asp?action=create&ContainerID=<%=PlateID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID + '&UOMAbv=<%=UOMAbv%>&QtyRequired=<%=QtyAvailable%>', 'Diag', 1); return false">Request Plate</a>
<%end if%>

<%
If Session("INV_CREATE_PLATE" & dbkey) then
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  Conn
		cmd.CommandType = adCmdText

		'SQL = "SELECT parent_plate_id_fk, is_plate_map FROM inv_plates WHERE PLATE_ID = " & plateID
		SQL = "SELECT is_plate_map FROM inv_plates WHERE PLATE_ID = " & plateID
		cmd.CommandText = sql

		Set rsPlate = Cmd.Execute
		
		'parentPlateID = rsPlate("parent_plate_id_fk")
		isPlateMap = rsPlate("is_plate_map")
		
		rsPlate.Close
		Set rsPlate = nothing

end if
bIsPlateMap  = false
if isPlateMap = "1" then bIsPlateMap = true
%>

<%If Session("INV_CREATE_PLATE" & dbkey) and bIsPlateMap then
	call CheckIfFirst()	
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CreateorEditPlate.asp?PlateID=<%=PlateID%>&isCreate=true&refresh=true&getData=session', 'Diag',2); return false">Create Plates</a>
<%End if%>

<%
If (Session("INV_CREATE_PLATE" & dbkey)) and not bIsPlateMap then
	call CheckIfFirst()	
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/ReformatPlate_SelectOrder.asp?reformatAction=daughter&plateID=<%=PlateID%>', 'Diag', 1); return false">Create Daughter Plates</a>
<%End if%>

<%If Session("INV_EDIT_Plate" & dbkey) then
	call CheckIfFirst()	
%>			
	<a class="MenuLink" id="EditPlateLnk" HREF="Edit inventory plate" onclick="OpenDialog('../GUI/CreateOrEditPlate.asp?isEdit=true&refresh=true&getData=session&amp;PlateID=<%=PlateID%>', 'Diag', 2); return false">Edit Plate</a>
<%End if%>

<%If Session("INV_MOVE_PLATE" & dbkey) then
	call CheckIfFirst()	
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/MovePlate.asp', 'Diag', 4); return false">Move Plate</a>
<%End if%>

<%If Session("INV_DILUTE_PLATE" & dbkey) then
	call CheckIfFirst()	
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/ReformatPlate_SourceSolvate.asp?PlateID=<%=PlateID%>&BarcodeList=<%=Session("plPlate_Barcode")%>&pageMode=dilute&LocationID=<%=Session("plLocation_ID_FK")%>', 'Diag', 4); return false">Dilute Plate</a>
<%End if%>

<%If Session("INV_EDIT_Plate" & dbkey) then
	call CheckIfFirst()	
%>			
	<a class="MenuLink" id="CopyPlateLnk" HREF="Copy an inventory plate" onclick="OpenDialog('../GUI/CreateOrEditPlate.asp?plIsCopy=true&refresh=true&getData=session&amp;PlateID=<%=PlateID%>', 'Diag', 2); return false">Copy Plate</a>
<%End if%>

<%If Session("INV_RETIRE_PLATE" & dbkey) then
	call CheckIfFirst()	
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RetirePlate.asp', 'Diag', 1); return false">Retire Plate</a>
<%End if%>

<%If Session("INV_DELETE_PLATE" & dbkey) then
	call CheckIfFirst()	
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/DeletePlate.asp', 'Diag', 1); return false">Delete Plate</a>		
<%End if%>

<%If false and Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then
	call CheckIfFirst()	
%>		
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/history.asp?containerid=<%=PlateID%>', 'Diag', 2); return false">History</a>		
<%end if%>

<%If Session("INV_PRINT_LABEL_PLATE" & dbkey) then
	call CheckIfFirst()	
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/PrintLabel.asp?PlateID=<%=PlateID%>&ShowInList=plates', 'Diag', 1); return false">Print Label</a>		
<%end if%>

<%If Session("INV_MANAGE_PLATE_MAPS" & dbkey) and not bIsPlateMap then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CreatePlateMap.asp?PlateID=<%=PlateID%>', 'Diag', 1); return false">Create Plate Map</a>		
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