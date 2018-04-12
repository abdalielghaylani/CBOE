<%@ Language=VBScript %>
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
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</HEAD>
<BODY>
<%if myDict.Count > 0 then%>
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>
			<%If Session("INV_CHECKOUT_CONTAINER" & dbkey) then%>
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CheckOut.asp?multiSelect=true&action=out' , 'Diag', 1); return false">Check Out</a>
				|			
			<%end if%>
			<%If Session("INV_CHECKIN_CONTAINER" & dbkey) then%>
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CheckOut.asp?multiSelect=true&action=in', 'Diag', 1); return false">Check In</a>
				|
			<%end if%>
			<%If Session("INV_MOVE_CONTAINER" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/MoveContainer.asp?multiSelect=true', 'Diag', 1); return false">Move Containers</a>
				|
			<%end if%>
			<%If Session("INV_RETIRE_CONTAINER" & dbkey) then%>
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RetireContainer.asp?multiSelect=true', 'Diag', 1); return false">Retire Containers</a>
				|
			<%End if%>
			<%If Session("INV_DELETE_CONTAINER" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/DeleteContainer.asp?multiSelect=true', 'Diag', 1); return false">Delete Containers</a>		
				|			
			<%End if%>						
			<%If Session("INV_EDIT_CONTAINER" & dbkey) then%>			
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/ChemInv/GUI/UpdateContainer.asp?multiSelect=true&action=edit', 'Diag', 1); return false">Update Containers</a>
			<%End if%>
		</td>
	</tr>
</table>
<%end if%>
<%
	if myDict.Count = 1 then
		text = "There is 1 container in the Selection List."
	else
		text = "There are " & myDict.Count & " containers in the Selection List."
	end if
	Response.Write "<BR><BR><BR><BR><BR><BR><BR><center><span class=""GUIFeedback"">" & text & "</span></center>"
	Set Session("multiSelectDict") = myDict
	Set myDict = Nothing
%>
</BODY>
</HTML>
