<%@ Language=VBScript %>
<%
plateID = Request("PlateID")
wellCriterion = Request("WellCriterion")
refresh = Request("refresh")
TB = Request("TB")
if refresh = "True" then
	Session("sPlateTab") = ""
end if
%>
<html>
<head>
</head>

<frameset rows="*,1">
	<frame name="plateFrame" src="viewPlateFrame.asp?PlateID=<%=plateID%>&WellCriterion=<%=wellCriterion%>&TB=<%=TB%>&GetData=db" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto" frameborder="0">
	<frame name="wellJSFrame" MARGINWIDTH="0" MARGINHEIGHT="0" SCROLLING="no" frameborder="0">
</frameset>  

</html>