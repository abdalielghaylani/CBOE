<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS
Dim lRecordCount
Dim ReportTypeID
Dim ReportTypeDesc
Dim ReportName
Dim ShowInList
Dim bRunReport
Dim PrimaryKeyID
Dim API
Dim FormData
Dim httpResponse

ReportName = Request("ReportName")
ShowInList = Request("ShowInList")
bRunReport = Request("bRunReport")
PrimaryKeyID = Request("PrimaryKeyID")
ReportSQL= Request("ReportSQL")
QueryName= Request("QueryName")
PageSource = Request("Source")
GetInvConnection()
				
' Build the RS manually so we can get the record count
Set RS = Server.CreateObject("ADODB.Recordset")
RS.CursorLocation = 3   ' adUseClient
RS.CursorType = 3       ' adOpenStatic
RS.ActiveConnection = Conn

select case(ShowInList)
    case "containers"
        ReportTypeDesc = "Label"
    case "plates"
        ReportTypeDesc = "Plate Label"
    case "locations"
        ReportTypeDesc = "Location Label"
        if PrimaryKeyID = "" or IsNull(PrimaryKeyID) then
			PrimaryKeyID = Session("CurrentLocationID")
		end if
   case "Requests"
       ReportTypeDesc =  "Request Label"
end select

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

' Get the report type ID
API = "/cheminv/api/LookUpValue.asp"
FormData = "TableName=inv_reporttypes&TableValue=" & Server.URLEncode(ReportTypeDesc) & "&InsertIfNotFound=false" & Credentials
httpResponse = CShttpRequest2("POST", ServerName, API, "ChemInv", FormData)
ReportTypeID = CLng(httpResponse)

'CSBR-158685: Don't show the safety data reports in the drop down based on the configuration
sql = "SELECT ReportDisplayName AS DisplayText, ReportName AS value, ReportSQL AS ReportSQL, QueryName AS QueryName FROM  " &  Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ReportType_ID = " & ReportTypeID
if lcase(Application("DISPLAY_SAFETY_DATA"))="false" then
	sql = sql & " AND REPORTNAME <> 'rptSubstanceP_HSafetyLabel' AND REPORTNAME <> 'rptSubstanceR_SSafetyLabel'"
end if
RS.Open(sql)
lRecordCount = RS.RecordCount

' If there is only one instance of the report type to run, just run it now instead of requiring the user to select 
' it from the combo box
if( lRecordCount = 1 ) then
    bRunReport = "true"
    ReportName = RS.Fields("value").Value
    ReportSQL= RS.Fields("ReportSQL").Value
    QueryName= RS.Fields("QueryName").Value
end if

ReportFormat = Application("DefaultReportFormat")
If ReportFormat = "" then ReportFormat = "SNP"
'If ReportFormat <> "SNP" AND ReportFormat <> "PDF" then ReportFormat = "SNP"

select case(lcase(ShowInList))
    case "containers"
        If ReportName = "" then ReportName = Application("DefaultContainerLabelReport")
        if PrimaryKeyID ="" Then PrimaryKeyID = Request("ContainerID")
        title = Application("appTitle")  & " -- Print Container Labels"	
        API = "/cheminv/api/CreateContainerReport.asp"
        FormData = "ReportFormat=" & ReportFormat & "&ContainerList=" & PrimaryKeyID & "&ReportName=" & ReportName & "&REPORTSQL=" &  Server.URLEncode(REPORTSQL) & "&QueryName=" & QueryName & Credentials

    case "plates"
        If ReportName = "" then ReportName = Application("DefaultPlateLabelReport")
        if PrimaryKeyID ="" Then PrimaryKeyID = Request("PlateID")
	    title = Application("appTitle") & " -- Print Plate Labels"	
	    API = "/cheminv/api/CreatePlateReport.asp"
	    FormData = "ReportFormat=" & ReportFormat & "&PlateList=" & PrimaryKeyID & "&ReportName=" & ReportName & Credentials

    case "locations"
        If ReportName = "" then ReportName = Application("DefaultLocationLabelReport")
        title = Application("appTitle")  & " -- Print Location Labels"	
        API = "/cheminv/api/CreateLocationBarcodeReport.asp"
        FormData = "ReportFormat=" & ReportFormat & "&LocationID=" & PrimaryKeyID & "&ReportName=" & ReportName & Credentials
    case "requests"
        If ReportName = "" then ReportName = Application("DefaultRequestLabelReport")
        if PrimaryKeyID ="" Then PrimaryKeyID = Request("RequestID")
	    title = Application("appTitle") & " -- Print Request Labels"	
	    API = "/cheminv/api/CreateRequestReport.asp"
	    FormData = "ReportFormat=" & ReportFormat & "&RequestList=" & PrimaryKeyID & "&ReportName=" & ReportName & Credentials
end select
httpResponse=""
if bRunReport = "true" then
	'Response.Write ServerName & ":" & FormData & ":" & API
	httpResponse = CShttpRequest2("POST", ServerName, API, "ChemInv", FormData)
	'Response.Write  httpResponse
	'Response.end
End if
If InStr(1,httpResponse,"Report Error") = 0 Then
%>
<html>
<head>	
	<title><%=title%></title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script language=javascript>
    function OnDocLoad()
    {
        window.focus();
<%
    if( lRecordCount = 1 ) OR (Application("AUTOLAUNCH_LABELPRINT_DIALOG") = "1") then
%>
        PrintReport();
<%
    end if
%>
    }
</script>
</head>
<body style="overflow:auto" onload="OnDocLoad();">	

<center>
<div id="progressMsg" style="visibility:hidden;position:absolute"
	<br/>
	<span class="GUIFeedback">Generating Label</span>
	<br/>
	<img src="/cfserverasp/source/graphics/processing_Ybvl_Ysh_grey.gif"><br/>
	<br/>
</div>	
<%
'DGB  generate the first report on entry
DropDownDefault = "Select One"
if (request.form("bRunReport") = "") AND (Application("AUTOGENERATE_FIRST_LABEL") = "1") then
	DropDownDefault = ""
%>
	<SCRIPT LANGUAGE=javascript>document.all.progressMsg.style.visibility = 'visible';</SCRIPT>
<%		
	end if
%>	
<div  id="reportPanel">
	<table border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td align="left">
				<FORM name="form1" action="#" method="POST">
				<input type="hidden" name="ShowInList" value="<%=ShowInList%>">
				<input type="hidden" name="PrimaryKeyID" value="<%=PrimaryKeyID%>">
				<input type="hidden" name="bRunReport" value="true">
				<input type="hidden" name="ReportSQL" value="" />
				<input type="hidden" name="QueryName" value="" />
				<%
				    if( lRecordCount > 1 ) then
				%>
				Label Type:&nbsp;
				<%
				        'Response.write ShowRPTSelectBox("ReportName", ReportName, )
				        Response.write( BuildSelectBox(RS, "ReportName", ReportName, 0, DropDownDefault, "NULL", 1, false, "", "" ) )				
				    else				
				%>
				<input type="hidden" name="ReportName" value="<% = ReportName %>">				
				<%
				    end if
				count=1
				RS.movefirst
                while not (RS.EOF or RS.BOF)
                    Response.Write "<input type=""hidden"" name=""ReportVal_" & count & """ value=""" &  RS.Fields("value").Value & "@#" & RS.Fields("ReportSQL").Value  & "@#" & RS.Fields("QueryName").Value & """/>"
                    count=count+1
                    RS.movenext
                wend
				%>
			
				</form>
				<SCRIPT LANGUAGE=javascript>
				//document.form1.ReportName.onchange = function(){document.form1.submit()}
				document.form1.ReportName.onchange = function(){if (document.form1.ReportName.value != "NULL") {SetReportvalues();document.all.reportPanel.style.visibility = 'hidden';document.all.progressMsg.style.visibility = 'visible';document.form1.submit()}}
				</SCRIPT>
<%
	'DGB  generate the first report on entry 
	if (request.form("bRunReport") = "") AND (Application("AUTOGENERATE_FIRST_LABEL") = "1")  then
%>
	<SCRIPT LANGUAGE=javascript>
	if (document.form1.ReportName.value != "NULL"){  
		document.form1.submit();
	}
	</SCRIPT>
<%		
	end if
%>
			</td>	
		</tr>
		<tr>
				<td align=right>
 <% if bRunReport = "true" then %>
                <a class="MenuLink" HREF="#" alt="Print this label" onclick="PrintReport();return false;">Print</a> 
 <% end if
 if lcase(PageSource) <> "eln" AND UCase(ReportFormat) = "SNP" then
 %>     
					| <a class="MenuLink" HREF="Close this window" onclick="window.close();return false">Close</a><BR><br>
<%end if  %>
				</td>
			</tr>	
			<tr>
				<td>
<%Select Case UCase(ReportFormat)
	Case "" 
		Response.Write "<center><table border=0 width=""90%""><tr><td ALIGN=center><BR><BR><BR><BR><BR><BR><BR>"
	Response.Write "<SPAN class=""GuiFeedback"">Select the desired report and format from above</SPAN></td></tr></table></center>"
	Case "SNP"
%>
	<center>
	<OBJECT classid="CLSID:F0E42D40-368C-11D0-AD81-00A0C90DC8D9" codeBase="/cfserverasp/rpt/Snapview.ocx" 
	height=300 id=SnapshotViewer style="LEFT: 0px; TOP: 0px" width=450 VIEWASTEXT>
    <param name="_ExtentX" value="10583">
    <param name="_ExtentY" value="3307">
    <param name="_Version" value="65536">
    <param name="SnapshotPath" value="<%=Application("ReportsHTTPPath") & httpResponse%>">
    <param name="Zoom" value="5">
    <param name="AllowContextMenu" value="-1">
    <param name="ShowNavigationButtons" value="-1"></OBJECT>
	</center>
	<script language=javascript>
	    function PrintReport() {
            try {
	            SnapshotViewer.PrintSnapshot(true);
	        }
	        catch (err)
             {
            // do nothing   
	        }
        }
     	function SetReportvalues()
	    {
            var index= document.form1.ReportName.selectedIndex;
            var valueString= eval("document.form1.ReportVal_" + index + ".value"  );
            var arrTemp = valueString.split("@#");
            document.form1.ReportSQL.value=arrTemp[1];
            document.form1.QueryName.value=arrTemp[2];
	    }
	</script>
<%
	Case "PDF", "XLS", "RTF"
		'Response.Clear
		if bRunReport = "true" then
		Response.Redirect Application("ReportsHTTPPath") & httpResponse
		end if
%>
    <script language=javascript>
	    function PrintReport()
	    {
            window.print();
        }
     	function SetReportvalues()
	    {
            var index= document.form1.ReportName.selectedIndex;
            var valueString= eval("document.form1.ReportVal_" + index + ".value"  );
            var arrTemp = valueString.split("@#");
            document.form1.ReportSQL.value=arrTemp[1];
            document.form1.QueryName.value=arrTemp[2];
	    }
    </script>
<%		
End Select
%>
				</td>
			</tr>
			<tr>
				<td align=right>
					<BR>
<% if bRunReport = "true" then %>
					<a class="MenuLink" HREF="#" alt="Print this label" onclick="PrintReport();return false;">Print</a> 
<% end if 
if lcase(PageSource) <> "eln" AND UCase(ReportFormat) = "SNP" then%>
					| <a class="MenuLink" HREF="Close this window" onclick="window.close();return false">Close</a>
                    <%end if %>
				</td>
			</tr>	
		</table>
</div>		
	</center>	
<%Else
%>
<CENTER>
    <BR><BR><P><CODE><% = httpResponse %></CODE></P>
    <SPAN class="GuiFeedback">Label could not be created</SPAN><br/><br/>
	<a class="MenuLink" HREF="Close this window" onclick="window.close();return false"><img src="/ChemInv/graphics/sq_btn/close_dialog_btn.gif" border=0 /></a>
</center>
<%
End if

' Cleanup
RS.Close
Set RS = nothing
%>

</body>
</html>		
