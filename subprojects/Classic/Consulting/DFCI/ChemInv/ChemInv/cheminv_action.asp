<%'Remember the exclude locations checkbox 
  if lcase(Request("dataaction")) = "search" then 
	Session("ExcludeChecked") = Request.Form("ExcludeSpecialLocations")
  End if		
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/form_action_vbs.asp"-->
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<%

custom_action = request("dataaction2")
Select Case UCase(custom_action)

End Select%>