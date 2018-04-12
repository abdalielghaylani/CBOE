<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%

Dim URL
Dim ServerName
Dim FormData
Dim httpResponse
Dim aStrings
Dim LabelPrinterID
Dim PrimaryKeyID
Dim NumCopies
Dim NoNetworkPrinters
Dim bError

LabelPrinterID = Request.Form("LabelPrinterID")
PrimaryKeyID = Request.Form("PrimaryKeyID")
NumCopies = Request.Form("NumCopies")
NoNetworkPrinters = Request.QueryString("NoNetworkPrinters")

bError = false
if NoNetworkPrinters = "true" then
    bError = true
    strErrorMessage =   "<p><SPAN class=GuiFeedback>No network printers are currently configured (or none are enabled).<p>" &_
                        "Please contact your administrator." &_
                        "</p></SPAN></p>"                        
end if

if not bError then
    ServerName = Application("InvServerName")    
    URL = "/cheminv/api/PrintLPRLabel.asp"
	Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
    FormData = "PrimaryKeyID=" & PrimaryKeyID & "&LabelPrinterID=" & LabelPrinterID & "&NumCopies=" & NumCopies & Credentials
    httpResponse = CShttpRequest2("POST", ServerName, URL, "ChemInv", FormData)
    
	if( instr( httpResponse, "Error:" ) ) then
        bError = true
        strErrorMessage =   "<p>" &_
                            "<CODE>" & httpResponse & "</CODE>" &_
                            "</p><br /><br />" &_
                            "<SPAN class=GuiFeedback>Label could not be created.</SPAN>"
    end if
end if

%>
<html>
<head>	
	<title>Network Label Printer Results</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script language=javascript>
    
</script>
</head>
<body>
<br />
<br />
	<center>
<%
if bError then
    response.Write strErrorMessage
else
%>
<p><SPAN class="GuiFeedback">Label was successfully printed.</SPAN></p>
<%
end if
%>    
    <a HREF="#" onclick="if (opener){opener.DialogWindow=null;} window.close(); return false;"><img SRC="/ChemInv/graphics/sq_btn/close_dialog_btn.gif" border="0"></a>
</center>
</body>
</html>		