<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'-- this page maintains the dictionary of selected racks and returns a comma delimited list of the 
'--   selected location ids and location names
response.Expires = 0
action=request("action")
set rackDict = Session("rackDict")

select case action
    case "click"
        locationID = request("locationID")
        locationText = request("locationText")

        if rackDict.exists(locationID) then 
	        rackDict.remove(locationID)
        else
	        rackDict.Add locationID, locationText
        end if
end select

'-- return the list    
list = ""
'-- write the ids
for each key in rackDict
    list = list &  """" & key & ""","
next
'-- write the names
for each key in rackDict
    list = list &  """" & rackDict.Item(key) & ""","
next
if list <> "" then list = left(list,len(list)-1)
response.Write list

set Session("rackDict") = rackDict
set rackDict = nothing
Response.End
%>