<%
' Millennium; print location barcode adds one link
'numLinks = 4
numLinks = 5

Dim arrLinks()
redim arrLinks(numLinks,7)
'-- 1st dimension is on/off
'-- 2nd dimension is "on" link href
'-- 3rd dimension is link text
'-- 4th dimension is dialog size
'-- 5th dimension is show "off" link switch
'-- 6th dimension is link category
'-- 7th dimension is link title
arrCategories = array("Location")

'-- set defaults
for i = 0 to ubound(arrLinks)
	'-- all links are off by default
	arrLinks(i,0) = 0
	'-- all dialog sizes are 1 by default
	arrLinks(i,3) = 1
next

'-- build the list of links
arrLinks(0,1) = "/cheminv/gui/NewLocation.asp"
arrLinks(0,2) = "New"
arrLinks(0,3) = "1"
arrLinks(0,5) = arrCategories(0)
arrLinks(0,6) = "Create New Location"

arrLinks(1,1) = "/cheminv/gui/NewLocation.asp?GetData=db"
arrLinks(1,2) = "Edit"
arrLinks(1,3) = "1"
arrLinks(1,5) = arrCategories(0)
arrLinks(1,6) = "Edit Selected Location"

arrLinks(2,1) = "/cheminv/gui/MoveLocation.asp"
arrLinks(2,2) = "Move"
arrLinks(2,3) = "1"
arrLinks(2,5) = arrCategories(0)
arrLinks(2,6) = "Move Selected Location"

arrLinks(3,1) = "/cheminv/gui/DeleteLocation.asp"
arrLinks(3,2) = "Delete"
arrLinks(3,3) = "1"
arrLinks(3,5) = arrCategories(0)
arrLinks(3,6) = "Delete Selected Location"

arrLinks(4,1) = "/ChemInv/GUI/SetDefaultLocation.asp"
arrLinks(4,2) = "Set Default"
arrLinks(4,3) = "1"
arrLinks(4,5) = arrCategories(0)
arrLinks(4,6) = "Set Default Location"

' Turn this off for E10 release; added for Millennium project
arrLinks(5,1) = "/ChemInv/GUI/PrintLocationLabel.asp"
arrLinks(5,2) = "Print Label"
arrLinks(5,3) = "2"
arrLinks(5,5) = arrCategories(0)
arrLinks(5,6) = "Print Location Barcode"

'-- turn links on if appropriate
If Session("INV_CREATE_LOCATION" & dbkey) then
	arrLinks(0,0) = 1
end if

If Session("INV_EDIT_LOCATION" & dbkey) then
	arrLinks(1,0) = 1
end if

If Session("INV_MOVE_LOCATION" & dbkey) then
	arrLinks(2,0) = 1
end if

If Session("INV_DELETE_LOCATION" & dbkey) then
	arrLinks(3,0) = 1
end if

if TreeID = 1 then
    arrLinks(4,0) = 1
    ' Turned off for E10.  Millennium
    arrLinks(5,0) = 1
end if

'-- show the links
ShowMenuLinks arrLinks, arrCategories

%>
