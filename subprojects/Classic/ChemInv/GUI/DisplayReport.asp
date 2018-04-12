<%@ Language=VBScript %>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
</HEAD>
<BODY>
<%
ReportFormat = Request.QueryString("ReportFormat")
ReportURL = Request.QueryString("ReportURL")
If ReportURL = "" then ReportFormat = ""

'Response.Write "ReportURL= " & ReportURL & "<BR>"
'Response.Write "ReportFormat= " & ReportFormat & "<BR>"

Select Case UCase(ReportFormat)
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
	Case "PDF", "XLS", "RTF", "XLSX"
		Response.Clear
		Response.Redirect ReportURL
End Select
%>
</BODY>
</HTML>
