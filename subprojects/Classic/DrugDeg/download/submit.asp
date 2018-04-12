<%@LANGUAGE = "VBScript"%>
<%


Response.Expires = 0
dbkey = "DrugDeg"
Session("wizard") = ""
Session( "CurrentLocation" & dbkey & formgroup ) = ""
Session( "CurrentLocation" & dbkey ) = ""
Session( "LastLocation" & dbkey ) = ""

if dbkey = "" then
	response.write "the name of the database (dbkey) is missing in the query string or form that requested this page."
	response.end
end if

if 1 = Application( "LoginRequired" & dbkey ) then
	if not 1 = Session( "UserValidated" & dbkey ) then
		response.redirect "/" & Application("appkey") & "/login.asp?dbname=" & dbkey & "&formgroup=base_form_group&perform_validate=0"
	end if
end if


%>
<script language="javascript">
<!-- Hide from older browsers
if ( parent.location.href != window.location.href ) {
	parent.location.href = window.location.href;
}



// End script hiding -->

</script>
<%
Session( "CurrentLocation" & dbkey & formgroup ) = ""
If Not Session( "CurrentUser" & dbkey ) <> "" then
	theuser = Session( "UserName" & dbkey )
	if theuser <> "" then
		Session( "CurrentUser" & dbkey ) = theuser
	end if
end if
%>
<html>

<head>
<%'if CBool(Application("UseCSSecurityApp")) = true then%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js" -->
<!-- script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/cs_security_utils.js"></script -->
<%'end if%>

	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
	<!--#INCLUDE VIRTUAL = "/drugdeg/source/app_vbs.asp" -->

		<title>Download ChemDraw Plugin</title>
	<link REL="stylesheet" TYPE="text/css" HREF="/<%=Application("appkey")%>/styles.css">
</head>



<body <%=Application("BODY_BACKGROUND")%>>



<table>
	<tr>
		<td>
			<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
				<!-- The table for the banner. -->
				<tr>
					<td valign="top">
						<img src="/<%=Application("appkey")%>/graphics/D3_Logo_SideWeb.jpg" alt="Drug Degradation" align="center">
					</td>
					
				</tr>
			</table>
		</td>
	</tr>
</table>
	
<%
dim conn


Const cdoSendUsingMethod        = _
	"http://schemas.microsoft.com/cdo/configuration/sendusing"
Const cdoSendUsingPickup = 1 
Const cdoSendUsingPort = 2 
Const cdoSMTPServer             = _
	"http://schemas.microsoft.com/cdo/configuration/smtpserver"
Const cdoSMTPServerPort         = _
	"http://schemas.microsoft.com/cdo/configuration/smtpserverport"
Const cdoSMTPConnectionTimeout  = _
	"http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout"
Const cdoSMTPAuthenticate       = _
	"http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"
Const cdoAnonymous = 0
' Use basic (clear-text) authentication. 
Const cdoBasic = 1
' Use NTLM authentication 
Const cdoNTLM = 2 'NTLM
Const cdoSendUserName           = _
	"http://schemas.microsoft.com/cdo/configuration/sendusername"
Const cdoSendPassword           = _
	"http://schemas.microsoft.com/cdo/configuration/sendpassword"

Dim objConfig  ' As CDO.Configuration
Dim myMail	 ' As CDO.Message
Dim Fields     ' As ADODB.Fields

' Get a handle on the config object and it's fields
Set objConfig = Server.CreateObject("CDO.Configuration")
Set Fields = objConfig.Fields

' Set config fields we care about
With Fields
	.Item(cdoSendUsingMethod)       = cdoSendUsingPort
	.Item(cdoSMTPServer)            = "outgoing.cambridgesoft.com"
	.Item(cdoSMTPServerPort)        = 25
	.Item(cdoSMTPConnectionTimeout) = 10
	.Item(cdoSMTPAuthenticate)      = cdoAnonymous
	'.Item(cdoSendUserName)          = "username"
	'.Item(cdoSendPassword)          = "password"

	.Update
End With

Set myMail = Server.CreateObject("CDO.Message")

Set myMail.Configuration = objConfig

'With objMessage
'	.To       = "Display Name <email_address>"
'	.From     = "Display Name <email_address>"
'	.Subject  = "SMTP Relay Test"
'	.TextBody = "SMTP Relay Test Sent @ " & Now()
'	.Send
'End With



Set conn = Server.CreateObject("ADODB.Connection")
conn.Open("File Name=C:\Inetpub\wwwroot\ChemOffice\DrugDeg\download\download.udl")

sql = "Insert Into ChemDraw9(FirstName, LastName, EmailAddress, Company, DownloadDate) Values"
sql2 = "('" & Request.Form("FirstName") & "','" & Request.Form("LastName") & "','" & Request.Form("Email") & "','" & Request.Form("Company") & "','" & Now() & "')"

conn.Execute(sql & sql2)
	
            'bSendMail = true

            'Set myMail=CreateObject("CDO.Message")

            myMail.To= Request.Form("Email")

            myMail.From= "jshatsoff@cambridgesoft.com"
			myMail.Subject= "ChemDraw Plugin Activation"
			
			myMail.TextBody = "This serial number should let you activate your ChemDraw Pro Plugin" & Chr(13) & "072-419449-5294"

            myMail.Send


            'SendMail = bSendMail


	
	
%>	

An email with a serial number has been sent to <%=Request.Form("Email")%>
<br><br>
You can download the <a href="ChemDraw903.exe">plugin here</a>

<div align="center">
<table border="0" bordercolor="red" cellspacing="15">
	<tr>
		<td height="20" colspan="2">&nbsp;</td>
	</tr>
	
	<tr>
		<td valign="top"><a class="headinglink" href="/cs_security/login.asp?ClearCookies=true">Logout</a>
	
		</td>
		
		<td valign="top"><a href="#" class="headinglink" onclick="window.open('/<%=Application("appkey")%>/help/help.asp?formgroup=base_form_group&amp;dbname=drugdeg','help','width=800,height=600,scrollbars=yes,status=no,resizable=yes');">Help</a>
	
		</td>
	
		<td valign="top"><a href="#" class="headinglink" onclick="window.open('/<%=Application("appkey")%>/about.asp?formgroup=base_form_group&amp;dbname=drugdeg','about','width=560,height=450,status=no,resizable=yes')">About the database</a>
		</td>
	</tr>
	<tr>
		<td height="20" colspan="2">&nbsp;</td>
	</tr>
</table>
</div>

</body>

</html>
