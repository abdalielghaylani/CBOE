<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
dbkey = "cheminv"
clear = Request.QueryString("clear")
dictType = Request.QueryString("dictType")
if dictType = "" then dictType = "container"
If dictType = "container" then
	Set myDict = multiSelect_dict
elseif dictType = "plate" then	
	Set myDict = plate_multiSelect_dict
end if

if clear then
	myDict.RemoveAll
Else
	str = Request("selectChckBox")
	'Response.Write str & ":str<BR>"
	tempArr = split(str,",")
	For i = 0 to Ubound(tempArr)
		if NOT myDict.Exists(Trim(tempArr(i))) then
			myDict.Add Trim(tempArr(i)), true
		End if
	Next
	str = Request("removeList")
	'Response.Write str & ":str<BR>"
	tempArr = split(str,",")
	For i = 0 to Ubound(tempArr)
		if myDict.Exists(Trim(tempArr(i))) then
			myDict.Remove(Trim(tempArr(i)))
		End if
	Next
End if
'Response.Write "<BR><BR><BR><BR><BR><BR><BR><center><span class=""GUIFeedback"">There are " & myDict.Count & " containers in the Selection List</span></center>"
'Response.End
Set Session("multiSelectDict") = myDict
Set myDict = Nothing

if dictType = "container" then
	ContainerID = DictionaryToList(multiSelect_dict)
	ContainerCount =  multiSelect_dict.count 
	'Response.Write Session("multiSelectDict") & ":multi<BR>"
	'Response.Write ContainerID & ":ids<BR>"
	Response.Write ContainerCount
	Response.End
else
	PlateID = DictionaryToList(plate_multiSelect_dict)
	PlateCount = plate_multiSelect_dict.count
	Response.Write PlateCount
	Response.End
end if

%>