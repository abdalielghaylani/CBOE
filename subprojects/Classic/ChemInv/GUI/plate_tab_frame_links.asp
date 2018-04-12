<%
'*************** Added for plate menu************************* 
numLinks = 11
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
arrCategories = array("Update","Create","Other")

'-- set defaults
for i = 0 to ubound(arrLinks)
    	    '-- all links are off by default
	arrLinks(i,0) = 0
    	    '-- all dialog sizes are 1 by default
	arrLinks(i,3) = 1
next

'-- build the list of links
arrLinks(0,1) = "/Cheminv/GUI/CreateorEditPlate.asp?PlateID=" & PlateID & "&isCreate=true&refresh=true&getData=session"
arrLinks(0,2) = "Create Plate"
arrLinks(0,3) = "2"
arrLinks(0,5) = arrCategories(1)
arrLinks(0,6) = "Create Plate"

arrLinks(1,1) = "/Cheminv/GUI/ReformatPlate_SelectOrder.asp?reformatAction=daughter&plateID=" & PlateID
arrLinks(1,2) = "Create Daughter Plates"
arrLinks(1,3) = "1"
arrLinks(1,5) = arrCategories(1)
arrLinks(1,6) = "Create Daughter Plates"

arrLinks(2,1) = "/Cheminv/GUI/CreateOrEditPlate.asp?isEdit=true&refresh=true&getData=session&amp;PlateID=" & PlateID
arrLinks(2,2) = "Edit Plate"
arrLinks(2,3) = "2"
arrLinks(2,5) = arrCategories(0)
arrLinks(2,6) = "Edit Plate"

arrLinks(3,1) = "/Cheminv/GUI/MovePlate.asp"
arrLinks(3,2) = "Move Plate"
arrLinks(3,3) = "4"
arrLinks(3,5) = arrCategories(0)
arrLinks(3,6) = "Move Plate"

arrLinks(4,1) = "/Cheminv/GUI/ReformatPlate_SourceSolvate.asp?PlateID=" & PlateID & "&BarcodeList=" & Session("plPlate_Barcode") & "&pageMode=dilute&LocationID=" & Session("plLocation_ID_FK")
arrLinks(4,2) = "Dilute Plate"
arrLinks(4,3) = "4"
arrLinks(4,5) = arrCategories(0)
arrLinks(4,6) = "Dilute Plate"

arrLinks(5,1) = "/Cheminv/GUI/CreateOrEditPlate.asp?plIsCopy=true&refresh=true&getData=session&amp;PlateID=" & PlateID
arrLinks(5,2) = "Copy Plate"
arrLinks(5,3) = "2"
arrLinks(5,5) = arrCategories(1)
arrLinks(5,6) = "Copy Plate"

arrLinks(6,1) = "/Cheminv/GUI/RetirePlate.asp"
arrLinks(6,2) = "Retire Plate"
arrLinks(6,3) = "1"
arrLinks(6,5) = arrCategories(0)
arrLinks(6,6) = "Retire Plate"

arrLinks(7,1) = "/Cheminv/GUI/DeletePlate.asp"
arrLinks(7,2) = "Delete Plate"
arrLinks(7,3) = "1"
arrLinks(7,5) = arrCategories(0)
arrLinks(7,6) = "Delete Plate"

arrLinks(8,1) = "/cheminv/gui/LineageTree.asp?refresh=1&assetType=plate&selectedID=" & plateID
arrLinks(8,2) = "Lineage"
arrLinks(8,3) = "5"
arrLinks(8,5) = arrCategories(2)
arrLinks(8,6) = "Lineage"

arrLinks(9,1) = "/Cheminv/GUI/PrintLabelOption.asp?PlateID=" & PlateID & "&ShowInList=plates"
arrLinks(9,2) = "Print Label"
arrLinks(9,3) = "2"
arrLinks(9,5) = arrCategories(2)
arrLinks(9,6) = "Print Label"

arrLinks(10,1) = "/Cheminv/GUI/CreatePlateMap.asp?PlateID=" & PlateID
arrLinks(10,2) = "Create Plate Map"
arrLinks(10,3) = "1"
arrLinks(10,5) = arrCategories(1)
arrLinks(10,6) = "Create Plate Map"

arrLinks(11,1) = "/cheminv/gui/manageLinks.asp?FK_value=" & plateID & "&FK_name=PLATE_ID&Table_Name=INV_PLATES"
arrLinks(11,2) = "Manage Links"
arrLinks(11,3) = "1"
arrLinks(11,5) = arrCategories(2)
arrLinks(11,6) = "Manage Links"

If Session("INV_CREATE_PLATE" & dbkey) then
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  Conn
		cmd.CommandType = adCmdText

		'SQL = "SELECT parent_plate_id_fk, is_plate_map FROM inv_plates WHERE PLATE_ID = " & plateID
		SQL = "SELECT is_plate_map FROM inv_plates WHERE PLATE_ID = ?"
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("plateID", 131, 1, 0, plateID)
		Set rsPlate = Cmd.Execute
		
		'parentPlateID = rsPlate("parent_plate_id_fk")
		isPlateMap = rsPlate("is_plate_map")
		
		rsPlate.Close
		Set rsPlate = nothing

end if

bIsPlateMap  = false
if isPlateMap = "1" then bIsPlateMap = true
If Session("INV_CREATE_PLATE" & dbkey) and bIsPlateMap then
	arrLinks(0,0) = 1
end if

If (Session("INV_CREATE_PLATE" & dbkey)) and not bIsPlateMap then
	arrLinks(1,0) = 1
end if

If Session("INV_EDIT_Plate" & dbkey) then
	arrLinks(2,0) = 1
End if

If Session("INV_MOVE_PLATE" & dbkey) then
	arrLinks(3,0) = 1
End if

If Session("INV_DILUTE_PLATE" & dbkey) then
	arrLinks(4,0) = 1
End if

If Session("INV_CREATE_PLATE" & dbkey) then
	arrLinks(5,0) = 1
End if

If Session("INV_RETIRE_PLATE" & dbkey) then
	arrLinks(6,0) = 1
End if

If Session("INV_DELETE_PLATE" & dbkey) then
	arrLinks(7,0) = 1
End if

arrLinks(8,0) = 1

If Session("INV_PRINT_LABEL_PLATE" & dbkey) then
	arrLinks(9,0) = 1
end if

If Session("INV_MANAGE_PLATE_MAPS" & dbkey) and not bIsPlateMap then
	arrLinks(10,0) = 1
end if

If Session("INV_MANAGE_LINKS" & dbkey) then
	arrLinks(11,0) = 1
end if

' Checking Authority
if  Application("ENABLE_OWNERSHIP")="TRUE" AND cint(Session("plisAuthorised")) =0 then 
    for i=0 to 11
        if i<>8 and i<>9 and i<>10  then
            arrLinks(i,0) = 0
            arrLinks(i,4) = 1
        end if 
    next 
end if  
'-- show the links
ShowMenuLinks arrLinks, arrCategories
WriteContextMenu arrLinks, arrCategories

%>


