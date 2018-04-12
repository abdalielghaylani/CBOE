<%@ Language=VBScript %>
<%
roleID = Request("roleID")
locationList = Request("locationList")

excludedLocationIDs = " "
Set excludedDict = server.CreateObject("Scripting.Dictionary")
arrLocID = split(locationList, ",")
for each element in arrLocID
    excludedDict.Add trim(cstr(element)), cstr(roleID)
    excludedLocationIDs = excludedLocationIDs & element & ","
next
excludedLocationIDs = left(excludedLocationIDs,len(excludedLocationIDs)-1)

Set Session("excludedDict") = excludedDict
Response.Write true
Response.End
%>
