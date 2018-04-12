<%@ EnableSessionState=False Language=VBScript%>
<Script RUNAT="Server" Language="VbScript">
Dim UserName
Dim Password
Dim soapClient
Dim headerHandler
Dim strResponse

Response.Expires = -1

UserName = Request("USRCODE") 
Password = Request("AUTHENTICATE")
RegServerName = Application("RegServerName")

' Set up SOAP client
set soapClient = Server.CreateObject("MSSOAP.SoapClient30")
set headerHandler = Server.CreateObject("COEHeaderServer.COEHeaderHandler")
headerHandler.UserName = UserName
headerHandler.Password = Password
set soapClient.HeaderHandler = headerHandler
soapClient.ClientProperty("ServerHTTPRequest") = True
call soapClient.MSSoapInit(Application("SERVER_TYPE") & RegServerName & "/COERegistration/webservices/COERegistrationServices.asmx?wsdl")
soapClient.ConnectorProperty("Timeout") = 120000    ' sets timeout to 120 secs

'Get registration XML template
strResponse = soapClient.UserIsInRoles("REGISTER_DIRECT")
strRegTemplate = soapClient.RetrieveNewRegistryRecord()
set RegXml = Server.CreateObject("Msxml2.DOMDocument")
RegXml.loadXML(strRegTemplate)
set Node = RegXml.SelectSingleNode("MultiCompoundRegistryRecord")
RLS = Node.getAttribute("ActiveRLS")

If UCase(strResponse)="TRUE" then
    Response.Write "user authenticated" & "+" & RLS
Else
    Response.Write "User not authenticated to register compounds: " & UserName
End If    
Response.end
</SCRIPT>