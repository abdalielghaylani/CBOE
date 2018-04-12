<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
ReportTypeID = Request("ReportTypeID")
if trim(ReportTypeID) = "" then ReportTypeID = -1
'-- build a string of report data to be used to hydrate the report select
GetInvConnection()
SQL = "SELECT ID, ReportDisplayName, ReportType_ID FROM " &  Application("CHEMINV_USERNAME") & ".inv_reports"
if lcase(Application("DISPLAY_SAFETY_DATA"))="false" then
	SQL = SQL & " WHERE REPORTNAME <> 'rptSubstanceP_HSafetyLabel' AND REPORTNAME <> 'rptSubstanceR_SSafetyLabel'"
	SQL = SQL & " AND REPORTNAME <> 'rptLocationSubstanceP_HSafetyReport' AND REPORTNAME <> 'rptLocationSubstanceR_SSafetyReport'"
end if
Set rs = Conn.Execute(SQL)
reportData = ""
while not rs.eof
	reportData = reportData & rs("ID") & ":" & rs("ReportDisplayName") & ":" & rs("ReportType_ID") & "||"
	rs.MoveNext
wend
reportData = left(reportData,len(reportData)-2)



%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Reports</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript">
	window.focus();
	reportData = "<%=reportData%>";
	function ValidateClick(elmName, url, action){
		if ((document.form1[elmName].value == "") && (action != "create")){
			alert("Please select an item to " + action + " from the list.")
		}
		else{
			if (action == "delete")
				document.form1.action = url + "?action=" + action + "&ReportDisplayName=" + document.form1[elmName].options[document.form1[elmName].selectedIndex].text;
			else
				document.form1.action = url + "?action=" + action;
			//alert(document.form1[elmName].options[document.form1[elmName].selectedIndex].text);
			document.form1.submit();
		}
	}
	function FilterReports(value){
		//alert(document.form1.Report_ID.length);
		//alert(reportData);
		var NewOption;

		// clear the current select options
		document.form1.Report_ID.options.length = 0;
		
		// create an array of report data
		arrReports = reportData.split("||");
		for(i=0;i<arrReports.length;i++){
			// create an array of individual report data
			arrMyReport = arrReports[i].split(":");
			
			// only add reports of the selected type
			if (value == "-1" || arrMyReport[2] == value)
			{ 
			    NewOption = new Option(arrMyReport[1], arrMyReport[0], false, false);			    
			    NewOption.id = arrMyReport[2];
				document.form1.Report_ID.options[document.form1.Report_ID.options.length] = NewOption;				
			}
		}
	}
	
	function SelectOption(objSelect)
	{
	    var objOption = objSelect.options.item(objSelect.selectedIndex);
	    document.form1.ExistingReportTypeID.value = objOption.id;	    
	}	
	
</script>

</head>
<body>

<center>
<form name="form1" method="POST">
<input type="hidden" name="ExistingReportTypeID" value="" />
<table border="0">
	<tr>
		<td colspan="5">
			<span class="GuiFeedback">Select a Report Layout from the list and click a link next to it.<BR>You can filter the Report Layout list by selecting a Report Type.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" valign="middle" nowrap>
			<span title="select a report type">Select a Report Type:</span>
		</td>
		<td>
			<%=ShowMultiSelectBox2("ReportTypeID", ReportTypeID, "SELECT ReportTypeDesc AS DisplayText, ReportType_ID AS value, '' as DefaultValue FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reporttypes", 45,"All Types", "-1", 5, false, "FilterReports(this.value);return false;","")%>
		</td>
		<td align="right" valign="middle" nowrap>
			<span title="define an Inventory report">Report Layouts:</span>
		</td>
		<td>
			<%=ShowMultiSelectBox2("Report_ID", Report_ID, "SELECT ReportDisplayName AS DisplayText, ID AS value, '' as DefaultValue FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reports ORDER BY DisplayText", 45, null, null, 5, false, "SelectOption(this);return(false);","")%>
		</td>
		<td valign="middle">			
			<%
			elmName = "Report_ID"
			str = "&nbsp;"
			str = str & GetLink("AddReport.asp", "ValidateClick('" & elmName & "','AddReport.asp','create'); return false", "New")
			str = str & " | " & GetLink("EditReport.asp", "ValidateClick('" & elmName & "','EditReport.asp','edit'); return false", "Edit")
			str= str & " | " & GetLink("DeleteReport.asp", "ValidateClick('" & elmName & "','DeleteReport.asp','delete'); return false", "Delete")
			Response.Write str
			%>					
		</td>
	</tr>
	<tr>
		<td colspan="5" align="right"><a HREF="#" onclick="window.location='/cheminv/gui/menu.asp'; return false;"><img SRC="../graphics/close.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
	</tr>
</table>	
<script language="javascript">
// set the width of the ReportTypeID select
document.form1.ReportTypeID.style.width = "200px";
// set the width of the Report_ID select
document.form1.Report_ID.style.width = "200px";

// refresh report layouts 
FilterReports(document.form1.ReportTypeID.value);
</script>
</form>
</center>
</body>
</html>
