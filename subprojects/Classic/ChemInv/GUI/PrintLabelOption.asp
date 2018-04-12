<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim ReportRS
Dim Cmd
Dim LabelPrinterRS
Dim lRecordCount
Dim ReportTypeID
Dim bRunReport
Dim ReportName
Dim strLabelType
Dim bMultiSelect

ShowInList = Request("ShowInList")
if Request("multiSelect") = "true" then
    bMultiSelect = true
else
    bMultiSelect = false
end if

InvSchema = Application("CHEMINV_USERNAME")


select case( ShowInList )
    case "containers"    
        ReportTypeDesc = "Label"
        strLabelType = "Container"
        if bMultiSelect then
            PrimaryKeyID = DictionaryToList(multiSelect_dict)
        else
            PrimaryKeyID = Request("ContainerID")
        end if
    case "plates"
        ReportTypeDesc = "Plate Label"
        strLabelType = "Plate"
        if bMultiSelect then
            PrimaryKeyID = DictionaryToList(plate_multiSelect_dict)
        else
            PrimaryKeyID = Request("PlateID")
        end if
    case "locations"
        ReportTypeDesc = "Location Label"
        strLabelType = "Location"
        ' Currently there is no method for multi-selecting locations
        PrimaryKeyID = Session("CurrentLocationID")        
        bMultiSelect = false
    case "Requests"
        ReportTypeDesc = "Request Label"
        strLabelType = "Request"
        ' Currently there is no method for multi-selecting locations
        PrimaryKeyID = Request("RequestID")        
        bMultiSelect = false
end select

GetInvConnection()

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))

' Get the report type ID
API = "/cheminv/api/LookUpValue.asp"
FormData = "TableName=inv_reporttypes&TableValue=" & Server.URLEncode(ReportTypeDesc) & "&InsertIfNotFound=false" & Credentials
'response.write "FormData=" & FormData & "<BR>"

httpResponse = CShttpRequest2("POST", ServerName, API, "ChemInv", FormData)

'response.write "ServerName=" & ServerName & "<BR>"
'response.write "httpResponse=" & httpResponse & "<BR>"
'response.end

ReportTypeID = CLng(httpResponse)

' Get the list of LPR printers
Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETLABELPRINTERS", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("pReportTypeID", adNumeric, adParamInput, , ReportTypeID)
Cmd.Properties ("PLSQLRSet") = TRUE  
Set LabelPrinterRS = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE

if bMultiSelect then
		if ucase(Application("UseLprReports"))="FALSE" then 
		Response.Redirect("/ChemInv/GUI/PrintLabel.asp?ShowInList=" & ShowInList & "&ContainerID=" & PrimaryKeyID)	
  else 
    if LabelPrinterRS.BOF and LabelPrinterRS.EOF then
        Response.Redirect("/ChemInv/GUI/PrintLPRLabel.asp?NoNetworkPrinters=true")
    end if
  end if
else
    ' If there are no entries in inv_label_printers for this type, just redirect to the PrintLabel.asp form...
    if LabelPrinterRS.BOF and LabelPrinterRS.EOF then
        Server.Transfer("/ChemInv/GUI/PrintLabel.asp")
    end if
    
    ' Build the ReportRS manually so we can get the record count
    Set ReportRS = Server.CreateObject("ADODB.Recordset")
    ReportRS.CursorLocation = 3   ' adUseClient
    ReportRS.CursorType = 3       ' adOpenStatic
    ReportRS.ActiveConnection = Conn
'CSBR-158685: Don't show the safety data reports in the drop down based on the configuration
    sql = "SELECT ReportDisplayName AS DisplayText, ReportName AS value FROM  " &  Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ReportType_ID = " & ReportTypeID
	if lcase(Application("DISPLAY_SAFETY_DATA"))="false" then
		sql = sql & " AND REPORTNAME <> 'rptSubstanceP_HSafetyLabel' AND REPORTNAME <> 'rptSubstanceR_SSafetyLabel'"
	end if
    ReportRS.Open(sql)
    lRecordCount = ReportRS.RecordCount
end if


%>
<html>
<head>	
<title>Print <% = strLabelType %> Label</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script language=javascript>
    function ValidateForm()
    {
        var NumCopies = document.NetworkPrinterForm.NumCopies.value;
        var bWriteError = false;
        var errmsg = "Please fix the following problems:\r\r";
        
        // NumCopies is required
		if( NumCopies.length == 0)
		{
			errmsg = errmsg + "- Number of copies is required.\r";
			bWriteError = true;
		}
		else
		{
			// NumCopies must be a number
			if( !isPositiveNumber( NumCopies ) || !isLongInteger( NumCopies ))
			{
			    errmsg = errmsg + "- Number of copies must be a positive integer number.\r";
			    bWriteError = true;
			}			
		}
		if( bWriteError )
		{
			alert(errmsg);
		}
		else
		{
		    document.NetworkPrinterForm.submit();
		}
    }
</script>
</head>
<body>
	<center>
	<br />
	<span class="GUIFeedback">
	Please select which kind of report you would<br /> like to run to generate this label:<br />
	</span>
	<p></p>
	<table border="0" cellspacing="0" cellpadding="2">
<form method=post name="NetworkPrinterForm" action="./PrintLPRLabel.asp">
<input type=hidden name="PrimaryKeyID" value=<% = PrimaryKeyID %> />
	<tr>
	    <td>Network printer reports:</td>
	    <td>#Copies</td>
	    <td></td>
	</tr>
	<tr>
	    <td>
	    <select name="LabelPrinterID" style="width: 200px;">
<%
    LabelPrinterRS.MoveFirst
    while not LabelPrinterRS.EOF
%>
        <option value="<% = LabelPrinterRS("label_printer_id") %>"><% = LabelPrinterRS("display_name") %></option>
<%
        LabelPrinterRS.MoveNext
    wend
%>
	    
	    </select>
        </td>
	    <td>
	    <input type=text name="NumCopies" value="1" maxlength=3 size=5 />
	    </td>
	    <td><input type=image src="/ChemInv/graphics/sq_btn/go_dialog_btn.gif" onclick="javascript:ValidateForm();return(false);" /></td>
	</tr>
</form>
<%
if lRecordCount > 0 and not bMultiSelect then
%>
    <tr><td colspan=3>&nbsp;</td></tr>
<form method=post name="LocalPrinterForm" action="./PrintLabel.asp">
<input type=hidden name="ShowInList" value=<% = Request("ShowInList") %> />
<input type=hidden name="bRunReport" value="true" />
<input type=hidden name="PrimaryKeyID" value=<% = PrimaryKeyID %> />
	<tr>
	    <td>My local printer reports:</td>
	    <td colspan="2"></td>
	</tr>
	<tr>
	    <td>	    
	    <select name="ReportName" style="width: 200px;">
<%
    ReportRS.MoveFirst
    while not ReportRS.EOF  'DisplayText, ReportName AS value
%>
        <option value="<% = ReportRS("value") %>"><% = ReportRS("DisplayText") %></option>
<%
        ReportRS.MoveNext
    wend
%>
	    </select>
	    </td>
	    <td></td>
	    <td><input type=image src="/ChemInv/graphics/sq_btn/go_dialog_btn.gif" /></td>	    	    
	</tr>
</form>
<%
end if   ' if lRecordCount > 0 and multiSelect <> "true" then
%>
    <tr><td colspan=3>&nbsp;</td></tr>
    <tr>
	    <td colspan=2></td>
	    <td><a HREF="#" onclick="if (opener){opener.DialogWindow=null;} window.close(); return false;"><img SRC="/ChemInv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a></td>
	</tr>
	</table>
<%	
' Cleanup
if not bMultiSelect then
    LabelPrinterRS.Close
    Set LabelPrinterRS = nothing
    ReportRS.Close
    Set ReportRS = nothing
end if

Conn.Close
set Conn = nothing
set Cmd = nothing
%>

</body>
</html>		