<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS
Dim lRecordCount
Dim ContainerID
Dim LabelPrinterID
Dim NumCopies
Dim bError
Dim httpResponse
Dim FormData
Dim ServerName

LabelPrinterID = Request.Form("LabelPrinterID")
bRunReport = Request.Form("bRunReport")
NumCopies = Request.Form("NumCopies")
if IsEmpty(NumCopies) then
    NumCopies = Request("NumCopies")
end if

' DFCI Need to add to allow users to send multiple labels to be printed.
Set myDict = multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then
	ContainerCount =  multiSelect_dict.count
	if ContainerCount = 0 then
		if action = "in" then
			actionText = "in"
		elseif action = "out" then
			actionText = "out"
		end if
		action = "noContainers"
	else
	ContainerID = DictionaryToList(myDict)
	FirstContainerID = GetFirstInList(ContainerID,",")
	'ContainerName =  myDict.count & " containers will be checked " & action
	'ContainerBarcode =  myDict.count & " containers will be checked " & action & "."
	end if
Else
ContainerID = Request.Form("ContainerID")
if IsEmpty(ContainerID) then
    ContainerID = Request("ContainerID")
end if
End if
'End Section
'DFCI Old below
'ContainerID = Request.Form("ContainerID")
'if IsEmpty(ContainerID) then
'    ContainerID = Request("ContainerID")
'end if
'DFCI old end




GetInvConnection()

bError = false
Set RS = Server.CreateObject("ADODB.Recordset")
RS.CursorLocation = 3   ' adUseClient
RS.CursorType = 3       ' adOpenStatic
RS.ActiveConnection = Conn
		
RS.Open( "SELECT label_printer_id as value, display_name AS DisplayText FROM " &  Application("CHEMINV_USERNAME") & ".inv_label_printers where is_available = 1" )

if IsEmpty(NumCopies) then
    NumCopies = "1"
end if

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

if bRunReport = "true" then
    FormData = "LabelPrinterID=" & LabelPrinterID & "&ContainerID=" & ContainerID & "&NumCopies=" & NumCopies & Credentials
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/PrintLPRLabel.asp", "ChemInv", FormData)
'	response.write formdata
	If InStr(1,httpResponse,"Error:PrintLPRLabel") > 0 Then
	    bError = true
	end if
End if

%>
<html>
<head>	
	<title><%=Application("appTitle")%> -- Print Container Labels</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/GUI/validation.js"></SCRIPT>
<script language=javascript>
    function OnDocLoad()
    {
        window.focus();
    }
    
    function OnPrint()
    {
        var strMessage = "Please fix the following problems:\n\n";
        var bWriteMessage = false;
        var NumCopies = document.form1.NumCopies.value;
        if( !isPositiveLongNumber(NumCopies) || !isLongInteger(NumCopies) )
        {
            strMessage = strMessage + "Number of copies must be a positive integer.\n";
            bWriteMessage = true;
        }
        
        if( document.form1.LabelPrinterID.value == "NULL" )
        {
            strMessage = strMessage + "Choose a valid printer.\n";
            bWriteMessage = true;
        }
        
        if( bWriteMessage )
        {
            alert( strMessage );
        }
        else
        {
            document.form1.submit();
        }
        
    }
</script>
</head>
<body onload="OnDocLoad();">
	<center>
	<br />
<%
    if bRunReport then
        if not bError then
%>    
	<span class="GUIFeedback">Container label printed.</span>
	<br /><br />
	<a class="MenuLink" HREF="./PrintLPRLabel.asp?ContainerID=<% = ContainerID %>&NumCopies=<% = NumCopies %>" alt="Print additional labels">Print Additional Labels</a> |
	<a class="MenuLink" HREF="Close this window" onclick="window.close();return false">Close</a>
<%
        else    ' if not bError then
%>    
	<span class="GUIFeedback">There was a problem printing the label:</span>
	<p><% = httpResponse %></p>
	<a class="MenuLink" HREF="Close this window" onclick="window.close();return false">Close</a>
<%
        end if
    else    ' if bRunReport then
%>	
	<span class="GUIFeedback">Print Container Label</span>
	<p></p>
	<FORM name="form1" action="#" method="POST">				
	<input type="hidden" name="ContainerID" value="<%=ContainerID%>">				
	<input type="hidden" name="bRunReport" value="true">
	
	<table border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td align="left">
				Choose printer:
			</td>
			<td>
				<%
				    Response.write( BuildSelectBox(RS, "LabelPrinterID", label_printer_id, 0, "Select One", "NULL", 1, false, "", "" ) )
				%>
			</td>
		</tr>
		<tr>
		    <td>
		        Number of copies:
		    </td>
		    <td>
				<input type="Text" name="NumCopies" id="NumCopiesID" value="<% = NumCopies %>" size=5>
            </td>
        </tr>
		<tr>
			<td align=right colspan=2>
				<BR>
				<a class="MenuLink" HREF="#" alt="Print this label" onclick="javascript:OnPrint();return false;">Print</a> |
				<a class="MenuLink" HREF="Close this window" onclick="window.close();return false">Close</a>
			</td>
		</tr>	
		</table>
		</form>
<%
    end if      ' if bRunReport then
%>
	</center>	
<%
' Cleanup
RS.Close
Set RS = nothing
%>

</body>
</html>		