<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim Cmd
Dim RS
Dim bDebugPrint 
Dim LocationID
Dim LocationName
Dim SQL
bDebugPrint = false

LocationID = Request("LocationID")

GetInvConnection()	
SQL = "select location_name from inv_locations where location_id = " & LocationID
Set RS = Conn.Execute(SQL)
if not RS.EOF and not RS.BOF then
    LocationName = RS("location_name")
end if
Set RS = Nothing 

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Bump-up Critical Inventory</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	
	function ValidateForm()
	{	    
	    var bError = false;
	    var Percent = document.form1.BumpUpPercent.value;
	    if( !isNumber(Percent) )
	    {
	        alert( "Please enter a numeric value for the percentage" );
	        bError = true;
	    }
	    else
	    {
	        if (Percent < 0)
	        {
	            alert( "Please enter a positive value (or zero) for the percentage" );
	            bError = true;
	        }
	    }
	    
	    if( !bError )
	    {
	        document.form1.submit();
	    }
	}
	 
//-->
</script>
</head>

<body>
<form name="form1" method="POST" action="DFCI_Reorder_action.asp">
<input TYPE="hidden" NAME="LocationID" Value="<%=LocationID%>">
<br />
<center>
<span class="GuiFeedback">This form creates orders based on configured par levels for central pharmacies.  Click OK to generate a list.  You will be able to edit the order prior to submission.  Filling in a percentage increase will increase the orders for drugs marked as critical.</span>
<br /><br />
<table border="0" cellspacing="0" cellpadding="0" width="300">
	<tr height="25">
		<td align="right">Location:</td>
        <td>
            <%=LocationName%>
        </td>		
	</tr>
	<tr height="25">
		<td align="right">Bump Up Percent:</td>
		<td>
			<input type="text" name="BumpUpPercent" size="10" value="0">
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right" height="20" valign="bottom"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
			<a HREF="#" onclick="ValidateForm(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>	
</form>
</center>
</body>
</html>