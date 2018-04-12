<%@language="vbscript"%>
<%
	'SYAN added 1/14/2004 to fix CSBR-35466
	aDate = CDate("31/12/99")
	
	systemShortDateFormat = CStr(aDate)
	
	mm = Month(aDate)
	dd = Day(aDate)
	yyyy = Year(aDate)
	
	wsFormat8 = mm & "/" & dd & "/" & yyyy
	wsFormat9 = dd & "/" & mm & "/" & yyyy
	wsFormat10 = yyyy & "/" & mm & "/" & dd
	
	'SYAN added on 9/15/2004 to fix the Danish Date format issue
	systemShortDateFormat = Replace(systemShortDateFormat, "-", "/")
	'End of SYAN modification
	
	if systemShortDateFormat = wsFormat8 then
		flipFrom = "8"
	elseif systemShortDateFormat = wsFormat9 then
		flipFrom = "9"
	elseif systemShortDateFormat = wsFormat10 then
		flipFrom = "10"
	end if
	
	'DJP added to fix bug
	if Request.QueryString("flipTo") <> "" then
		flipTo = Request.QueryString("flipTo")
	else
		flipTo = CStr(Request.QueryString("date_format"))
	end if
	'End of SYAN modification

	'DJP added to support diff form names
	formName = Request("formName")
	if formName = "" or IsEmpty("formName") then formName = "cows_input_form"
	'End of SYAN modification
%>

<script Language="jscript" runat="server">
	Response.Buffer=true;
</script>

<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 7.0">
</HEAD>
<BODY TOPMARGIN=5 LEFTMARGIN=5 BGCOLOR="#FFFFFF" onmouseleave="DatePicker_onmouseleave()">

<script Language="JavaScript">
<!--
function DatePicker_onmouseleave()
{
window.close();
}
//-->
</script>

<STYLE>
A {Text-Decoration: none;}
.MonthView {color:#000000; font-size:9pt; font-family: arial}
A.MonthView:LINK {Text-Decoration: none; color:#000000; font-size:9pt;font-family: arial}
A.MonthView:VISITED {Text-Decoration: none; color:#000000; font-size:9pt;font-family: arial}
A.MonthView:HOVER {background:#808080;Text-Decoration: none;font-size:9pt; color:#ffffff; font-family: arial}
</STYLE>

<%
strControl = Request.QueryString("CTRL")

if strControl <> "" then
   Session("DateCtrl") = strControl
end if

Set objMonthView = Server.CreateObject("VASPMV.ASPMonthView")
objMonthView.Class = "MonthView"
objMonthView.MonthBackColor = "#FFFFFF"
objMonthView.TitleBackColor = "#C0C0C0"
objMonthView.TitleForeColor = "#000000"
objMonthView.TrailingForeColor = "#C0C0C0"
objMonthView.HighlightTodayColor = ""

'SYAN added 1/7/2003 to fix CSBR-40195
if flipFrom = flipTo then
	objMonthView.ShowToday = true
else 
	objMonthView.ShowToday = false
end if

objMonthView.StartDate = ""
objMonthView.LanguageSetting = 7
objMonthView.Width = 180
objMonthView.Height = 0
objMonthView.RedirectURL = "month_view2.asp"
objMonthView.RedirectTarget = ""
objMonthView.ForceDHTML = false
objMonthView.DHTML = ""

'SYAN added to fix CSBR-35466
'DJP added formName
objMonthView.QueryString = "flipFrom=" & flipFrom & "&flipTo=" & flipTo & "&formName=" & formName

objMonthView.LicenseKey = "7758-AB7C-AF3B"

objMonthView.Show()

objMonthView.Unload()
set objMonthView = nothing

%>
</BODY>
</HTML>
