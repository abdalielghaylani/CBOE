<%
  if lcase(Request("dataaction")) = "search" then 
	'Remember the exclude locations checkbox 
	if Request.Form("ExcludeSpecialLocations") <> "" and not isEmpty(Request.Form("ExcludeSpecialLocations")) then
		Session("ExcludeChecked") = Request.Form("ExcludeSpecialLocations")
	else
		Session("ExcludeChecked") = "off"
	end if
	'-- Remember the search sublocations checkbox
	if Request.Form("searchSubLocations") <> "" and not isEmpty(Request.Form("searchSubLocations")) then
		Session("SearchSubLocations") = Request.Form("ExcludeSpecialLocations")
	else
		Session("SearchSubLocations") = "off"
	end if
  End if		
  
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/form_action_vbs.asp"-->
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<%

custom_action = request("dataaction2")
Select Case UCase(custom_action)

End Select%>