<% 
'Get the querystring paramter to determine which example to run
   TheExample = Request.QueryString("Example")
%>
<html>

<head>
<meta http-equiv="Content-Language" content="en-us">
<meta http-equiv="Content-Type" content="text/html; charset=windows-1252">
<meta name="GENERATOR" content="Microsoft FrontPage 4.0">
<meta name="ProgId" content="FrontPage.Editor.Document">
<title>Products Starting With</title>
</head>

<body>
<b>Report Criteria</b><br>
<form name="frmReportCriteria" action="rpt_display.asp?EXAMPLE=<%=TheExample%>" method="POST">
<table border="0" width="400">
  <tr>
    <td width="167">Products Starting With: </td>
    <td width="217"><font size "2"><select size="1" name="cboProduct">
            <option value="C">C</option>
            <option value="G">G</option>
            <option value="N">N</option>
            </select></font></td>
  </tr>
  <tr>
    <td width="167">
      Report Format:</td>
    <td width="217"><font size "2"><select size="1" name="cboReportFormat">
            <option value="PDF">Adobe PDF (PDF)</option>
            <option value="RTF">Microsoft Word (RTF)</option>
            <option value="XLS">Microsoft Excel (XLS)</option>
<% if TheExample = 5 then %>
            <option value="RAW">Microsoft Excel - Raw Data (XLS)</option>
<% End if %>
            </select></font></td>
  </tr>

  <tr>
    <td width="167"></td>
    <td width="217" align="left" height="30"><div align="left"><p><small><font
          face="Arial" color="#800080">
     <input type="submit" value="Run Report" name="cmdSubmit"></font></small></td>
  </tr>
 
</table>
</body>
</html>


