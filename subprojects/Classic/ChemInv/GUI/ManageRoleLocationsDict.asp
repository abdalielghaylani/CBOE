<%
response.Expires = 0
locationID = request("locationID")
set excludedDict = Session("excludedDict")
if excludedDict.exists(locationID) then 
	excludedDict.remove(locationID)
else
	excludeddict.Add locationID, "click"
end if
set Session("excludedDict") = excludedDict
Response.Write locationID
Response.End
%>