<%@ Language=VBScript %>
<%
dbkey = "cheminv"
clear = Request.QueryString("clear")
Set myDict = plate_multiSelect_dict

if clear then
%>
	<SCRIPT LANGUAGE=javascript>
		(top.ListFrame)? theListFrame = top.ListFrame : theListFrame = top.main.ListFrame; 
		if (theListFrame.location.href.indexOf('multiSelect=1') > 0) {
			theListFrame.location.href= theListFrame.location.href.replace('multiSelect=1', 'multiSelect=0')
		}
		else {
			//put the multiselect value first
			//alert(theListFrame.location.href.substr(0,theListFrame.location.href.indexOf('?')) + '?multiSelect=0&' + theListFrame.location.href.substr(theListFrame.location.href.indexOf('?')+1));
			theListFrame.location.href = theListFrame.location.href.substr(0,theListFrame.location.href.indexOf('?')) + '?multiSelect=0&' + theListFrame.location.href.substr(theListFrame.location.href.indexOf('?')+1);
			theListFrame.location.href= theListFrame.location.href + '&multiSelect=0';
		}
	</SCRIPT>
<%
	'Response.Write "<SCRIPT LANGUAGE=javascript>(top.ListFrame)? theListFrame = top.ListFrame : theListFrame = top.main.ListFrame; theListFrame.location.href= theListFrame.location.href.replace('multiSelect=1', 'multiSelect=0')</SCRIPT>"
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
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</HEAD>
<BODY>
<%if myDict.Count > 0 then%>
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>
			<%If Session("INV_CREATE_PLATE" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/ReformatPlate_SelectMap.asp?multiSelect=true&reformatAction=reformat', 'Diag', 1); return false">Reformat Plates</a>
				|
			<%end if%>
			<%If Session("INV_CREATE_PLATE" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/ReformatPlate_SelectOrder.asp?multiSelect=true&reformatAction=daughter', 'Diag', 1); return false">Daughter Plates</a>
				|
			<%end if%>
			<%If Session("INV_EDIT_PLATE" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/UpdatePlate.asp?multiSelect=true&action=edit', 'Diag', 1); return false">Update Plates</a>
				|
			<%end if%>
			<%If Session("INV_MOVE_PLATE" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/MovePlate.asp?multiSelect=true', 'Diag', 1); return false">Move Plates</a>
				|
			<%end if%>
			<%If Session("INV_DILUTE_PLATE" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/ReformatPlate_SourceSolvate.asp?multiSelect=true&pageMode=dilute&LocationID=<%=Session("CurrentLocationID")%>', 'Diag', 4); return false">Dilute Plates</a>
				|
			<%End if%>

			<%If Session("INV_RETIRE_PLATE" & dbkey) then%>
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RetirePlate.asp?multiSelect=true', 'Diag', 1); return false">Retire Plates</a>
				|
			<%End if%>
			<%If Session("INV_DELETE_PLATE" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/DeletePlate.asp?multiSelect=true', 'Diag', 1); return false">Delete Plates</a>		
			<%End if%>						
		</td>
	</tr>
</table>
<%end if%>
<%
	if myDict.Count = 1 then
		text = "There is 1 plate in the Selection List."
	else
		text = "There are " & myDict.Count & " plates in the Selection List."
	end if
	Response.Write "<BR><BR><BR><BR><BR><BR><BR><center><span class=""GUIFeedback"">" & text & "</span></center>"
	Set Session("plateMultiSelectDict") = myDict
	Set myDict = Nothing
%>
</BODY>
</HTML>
