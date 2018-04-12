<%@ Language=VBScript %>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft FrontPage 5.0">
</HEAD>
<BODY>


<%
ReportFormat = Request.QueryString("ReportFormat")
ReportURL = Request.QueryString("ReportURL")

If ReportURL = "" then ReportFormat = ""

if ReportURL = "" then
	on error resume next
	'check ReportQ dll
	if Session("ReportQ_OKAY") = "" OR Session("ReportQ_OKAY")= "FALSE" then
	Set ReportQ = Server.CreateObject("ReportQ.CReportQ")
	if err.number <> 0 then
		response.Write err.number & err.description
	else
		Session("ReportQ_OKAY") = "TRUE"
	end if
	Set ReportQ = Nothing
	end if
else

Select Case UCase(ReportFormat)
	Case "PROCESSING"
		if Not request("return_data") <> "" then	
		on error resume next
		Response.Buffer = True
		if detectNS6 = true then
			response.Write "."
		end if	
		Response.Flush
		Response.Write String(255," ")
		'!DGB! 10/17/01
		'Path of gif should point to /source/graphics not to application navbuttons path.
		'Unless there is an explicit override from the ini file
		Response.Write "<BR><BR><BR><TABLE Width=""600""  border=""0""><tr><td valign=middle align=center><IMG SRC=""" & Application("ANIMATED_GIF_PATH") & """></td></tr></TABLE>"
		Response.Flush
		end if
	
	Case "" 
		Response.Write "<center><table border=0 width=""90%""><tr><td ALIGN=center><BR><BR><BR><BR><BR><BR><BR>"
		Response.Write "<SPAN class=""GuiFeedback"">Select the desired report and format from above</SPAN></td></tr></table></center>"
	Case "SNP"
%>
	<center>
	
	<OBJECT classid="CLSID:F0E42D40-368C-11D0-AD81-00A0C90DC8D9" codeBase="/cfserverasp/rpt/Snapview.ocx" 
	height=600 id=SnapshotViewer style="LEFT: 0px; TOP: 0px" width=800 VIEWASTEXT>
	<PARAM NAME="_ExtentX" VALUE="21167">
	<PARAM NAME="_ExtentY" VALUE="15875">
	<PARAM NAME="_Version" VALUE="65536">
	<PARAM NAME="SnapshotPath" VALUE="<%=ReportURL%>">
	<PARAM NAME="Zoom" VALUE="5">
	<PARAM NAME="AllowContextMenu" VALUE="-1">
	<PARAM NAME="ShowNavigationButtons" VALUE="-1"></OBJECT>
	</center>
<%
	Case "PDF", "XLS", "RTF"
		Response.Clear
		Response.Redirect ReportURL
		
	Case "GUI_FEEDBACK"
		Response.Write "<table border=0 width=""90%""><tr><td><BR><BR><CENTER><P><CODE><font color = ""red"">" & ReportURL & "</font></CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Report could not be created</SPAN></td></tr></table></center>"			

End Select
end if%>

</BODY>
</HTML>