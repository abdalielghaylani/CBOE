<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
dbkey = "cheminv"
clear = Request.QueryString("clear")
Set myDict = multiSelect_dict

if clear then
	Session("bMultiSelect") = false
	Response.Write "<SCRIPT LANGUAGE=javascript>(top.ListFrame)? theListFrame = top.ListFrame : theListFrame = top.main.ListFrame; theListFrame.location.href= theListFrame.location.href.replace('multiSelect=1', 'multiSelect=0')</SCRIPT>"
	myDict.RemoveAll
Else
	str = Request.Form("selectChckBox")
	tempArr = split(str,",")
	For i = 0 to Ubound(tempArr)
		if NOT myDict.Exists(Trim(tempArr(i))) then
			myDict.Add Trim(tempArr(i)), true
		End if
	Next
	str = Request.Form("removeList")
	tempArr = split(str,",")
	For i = 0 to Ubound(tempArr)
		if myDict.Exists(Trim(tempArr(i))) then
			myDict.Remove(Trim(tempArr(i)))
		End if
	Next
End if
%>
<HTML>
<HEAD>
<script language="javascript" type="text/javascript" src="/cheminv/Choosecss.js"></script>
<script language="javascript" type="text/javascript" src="/cheminv/utils.js"></script>
<script language="javascript" type="text/javascript">
	// move the menu over to the right so it doesn't render below the CD control
	AlterCSS('.firstList','margin','0px 0px 0px 100px');
</script>
</HEAD>
<BODY>
<%if myDict.Count > 0 then

        numLinks = 6
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
        arrCategories = array("Manage")

        '-- set defaults
        for i = 0 to ubound(arrLinks)
    	    '-- all links are off by default
	        arrLinks(i,0) = 0
    	    '-- all dialog sizes are 1 by default
	        arrLinks(i,3) = 1
        next

        '-- build the list of links
        arrLinks(0,1) = "/Cheminv/GUI/CheckOut.asp?multiSelect=true&action=out"
        arrLinks(0,2) = "Check Out"
        arrLinks(0,3) = "1"
        arrLinks(0,5) = arrCategories(0)
        arrLinks(0,6) = "Check Out"

        arrLinks(1,1) = "/Cheminv/GUI/CheckOut.asp?multiSelect=true&action=in"
        arrLinks(1,2) = "Move Containers"
        arrLinks(1,3) = "1"
        arrLinks(1,5) = arrCategories(0)
        arrLinks(1,6) = "Move Containers"

        arrLinks(2,1) = "/Cheminv/GUI/MoveContainer.asp?multiSelect=true"
        arrLinks(2,2) = "Move Containers"
        arrLinks(2,3) = "1"
        arrLinks(2,5) = arrCategories(0)
        arrLinks(2,6) = "Move Containers"

        arrLinks(4,1) = "/Cheminv/GUI/DeleteContainer.asp?multiSelect=true"
        arrLinks(4,2) = "Delete Containers"
        arrLinks(4,3) = "1"
        arrLinks(4,5) = arrCategories(0)
        arrLinks(4,6) = "Delete Containers"

        arrLinks(5,1) = "/ChemInv/GUI/UpdateContainer.asp?multiSelect=true&action=edit"
        arrLinks(5,2) = "Update Containers"
        arrLinks(5,3) = "1"
        arrLinks(5,5) = arrCategories(0)
        arrLinks(5,6) = "Update Containers"


        arrLinks(3,1) = "/Cheminv/GUI/RetireContainer.asp?multiSelect=true"
        arrLinks(3,2) = "Retire Containers"
        arrLinks(3,3) = "1"
        arrLinks(3,5) = arrCategories(0)
        arrLinks(3,6) = "Retire Containers"

        arrLinks(6,1) = "/ChemInv/GUI/PrintLPRLabel.asp?multiSelect=true"
        arrLinks(6,2) = "Print Labels"
        arrLinks(6,3) = "1"
        arrLinks(6,5) = arrCategories(0)
        arrLinks(6,6) = "Print Labels"




        '-- turn links on if appropriate
        If Session("INV_CHECKOUT_CONTAINER" & dbkey) then
	        arrLinks(0,0) = 0
        end if
        If Session("INV_CHECKIN_CONTAINER" & dbkey) then
            arrLinks(1,0) = 1
        end if
        If Session("INV_MOVE_CONTAINER" & dbkey) then
            arrLinks(2,0) = 0
        end if
        If Session("INV_RETIRE_CONTAINER" & dbkey) then
            arrLinks(3,0) = 1
        end if
        If Session("INV_DELETE_CONTAINER" & dbkey) then
            arrLinks(4,0) = 1
        end if
		If Session("INV_EDIT_CONTAINER" & dbkey) then
            arrLinks(5,0) = 1
    	End if
	arrLinks(6,0) = 1
	        
        '-- show the links
        ShowMenuLinks arrLinks, arrCategories
        WriteContextMenu arrLinks, arrCategories

end if


Set plateDict = plate_multiSelect_dict
if plateDict.count > 0 then
%>
 <table cellpadding="2" cellspacing="0">
    <tr>
        <td class="ListView">
        &nbsp;Show:<select name="show" onchange="document.location=this.value;">
        <option value="/cheminv/gui/multiselect.asp" selected>Container List</option>
        <option value="/cheminv/gui/multiselect_plate.asp" >Plate List</option>
        </select>
        </td>
    </tr>
</table>
<%
end if

if myDict.Count = 1 then
	text = "There is 1 container in the Selection List."
else
	text = "There are " & myDict.Count & " containers in the Selection List."
end if
Response.Write "<BR><BR><BR><BR><BR><BR><BR><center><span class=""GUIFeedback"">" & text & "</span></center>"
Set Session("multiSelectDict") = myDict
Set myDict = Nothing
%>
</body>
</html>
