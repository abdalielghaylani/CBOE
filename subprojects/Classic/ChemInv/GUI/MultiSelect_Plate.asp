<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
dbkey = "cheminv"
clear = Request.QueryString("clear")
Set myDict = plate_multiSelect_dict
showInList = Request("showInList")

if clear then
	Session("bMultiSelect") = false
	Response.Write "<SCRIPT LANGUAGE=javascript>(top.ListFrame)? theListFrame = top.ListFrame : theListFrame = top.main.ListFrame; theListFrame.location.href= theListFrame.location.href.replace('multiSelect=1', 'multiSelect=0');</SCRIPT>"
	myDict.RemoveAll
Else
    if showInList = "racks" then
    	addList = Request.Form("selectPlateChckBox")
        removeList = Request.Form("removeList")
    else
    	addList = Request.Form("selectChckBox")
        removeList = Request.Form("removeList")
    end if
	tempArr = split(addList,",")
	For i = 0 to Ubound(tempArr)
		if NOT myDict.Exists(Trim(tempArr(i))) then
			myDict.Add Trim(tempArr(i)), true
		End if
	Next
	tempArr = split(removeList,",")
	For i = 0 to Ubound(tempArr)
		if myDict.Exists(Trim(tempArr(i))) then
			myDict.Remove(Trim(tempArr(i)))
		End if
	Next
End if
%>
<html>
<head>
<script language="javascript" type="text/javascript" src="/cheminv/Choosecss.js"></script>
<script language="javascript" type="text/javascript" src="/cheminv/utils.js"></script>
<!--<script language="javascript" type="text/javascript">
	// move the menu over to the right so it doesn't render below the CD control
	//AlterCSS('.firstList','margin','0px 0px 0px 100px');
</script>-->
<style type="text/css">
    .firstList { margin: 0px 0px 0px 100px;}
    /* IE10 does not detect AlterCSS, hence added this style */
</style>
</head>
<body>
<%
if myDict.Count > 0 then

    numLinks = 7
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
    arrLinks(0,1) = "/Cheminv/GUI/ReformatPlate.asp?multiSelect=true"
    arrLinks(0,2) = "Reformat Plates"
    arrLinks(0,3) = "1"
    arrLinks(0,5) = arrCategories(1)
    arrLinks(0,6) = "Reformat Plates"

    arrLinks(1,1) = "/Cheminv/GUI/ReformatPlate.asp?multiSelect=true&pageAction=daughter"
    arrLinks(1,2) = "Daughter Plates"
    arrLinks(1,3) = "1"
    arrLinks(1,5) = arrCategories(1)
    arrLinks(1,6) = "Daughter Plates"

    arrLinks(2,1) = "/Cheminv/GUI/UpdatePlate.asp?multiSelect=true&action=edit"
    arrLinks(2,2) = "Update Plates"
    arrLinks(2,3) = "1"
    arrLinks(2,5) = arrCategories(0)
    arrLinks(2,6) = "Update Plates"

    arrLinks(3,1) = "/Cheminv/GUI/MovePlate.asp?multiSelect=true"
    arrLinks(3,2) = "Move Plates"
    arrLinks(3,3) = "1"
    arrLinks(3,5) = arrCategories(0)
    arrLinks(3,6) = "Move Plates"

    arrLinks(4,1) = "/Cheminv/GUI/ReformatPlate_SourceSolvate.asp?multiSelect=true&pageMode=dilute"
    arrLinks(4,2) = "Dilute Plates"
    arrLinks(4,3) = "4"
    arrLinks(4,5) = arrCategories(0)
    arrLinks(4,6) = "Dilute Plates"

    arrLinks(5,1) = "/Cheminv/GUI/RetirePlate.asp?multiSelect=true"
    arrLinks(5,2) = "Retire Plates"
    arrLinks(5,3) = "1"
    arrLinks(5,5) = arrCategories(0)
    arrLinks(5,6) = "Retire Plates"

    arrLinks(6,1) = "/Cheminv/GUI/DeletePlate.asp?multiSelect=true"
    arrLinks(6,2) = "Delete Plates"
    arrLinks(6,3) = "1"
    arrLinks(6,5) = arrCategories(0)
    arrLinks(6,6) = "Delete Plates"
    
    arrLinks(7,1) = "/Cheminv/GUI/PrintLabelOption.asp?multiSelect=true&ShowInList=plates"
    arrLinks(7,2) = "Print Labels"
    arrLinks(7,3) = "2"
    arrLinks(7,5) = arrCategories(2)
    arrLinks(7,6) = "Print Labels"

    If Session("INV_CREATE_PLATE" & dbkey) then
	    arrLinks(0,0) = 1
    end if

    If (Session("INV_CREATE_PLATE" & dbkey)) then
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
    
    If Session("INV_RETIRE_PLATE" & dbkey) then
	    arrLinks(5,0) = 1
    End if

    If Session("INV_DELETE_PLATE" & dbkey) then
	    arrLinks(6,0) = 1
    End if
    
    If Session("INV_PRINT_LABEL_PLATE" & dbkey) then
	    arrLinks(7,0) = 1
    end if

    '-- show the links
    ShowMenuLinks arrLinks, arrCategories
    'WriteContextMenu arrLinks, arrCategories

end if


Set containerDict = multiSelect_dict
if containerDict.count > 0 then
%>
 <table cellpadding="2" cellspacing="0">
    <tr>
        <td class="ListView">
        &nbsp;Show:<select name="show" onchange="document.location=this.value;">
        <option value="/cheminv/gui/multiselect_plate.asp" selected>Plate List</option>
        <option value="/cheminv/gui/multiselect.asp">Container List</option>
        </select>
        </td>
    </tr>
</table>
<%
end if

if myDict.Count = 1 then
	text = "There is 1 plate in the Selection List."
else
	text = "There are " & myDict.Count & " plates in the Selection List."
end if
Response.Write "<BR><BR><BR><BR><BR><BR><BR><center><span class=""GUIFeedback"">" & text & "</span></center>"
Set Session("plateMultiSelectDict") = myDict
Set myDict = Nothing
%>
</body>
</html>
