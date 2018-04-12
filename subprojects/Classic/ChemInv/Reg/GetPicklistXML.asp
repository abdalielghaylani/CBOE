<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim UserID
Dim Password
Dim OraPassword
Dim RegServerName
Dim Picklist
Dim SQL
Dim strError
Dim Conn
Dim StrSql
Dim Rs
Dim PersonId

Response.Expires = -1
strError = "Error:InvLoaderPicklist<BR>"

UserID = Request("USRCODE") 
Password = Request("AUTHENTICATE")
Picklist = Request("PICKLIST")
RegServerName = Application("REGSERVERNAME")
SQL = ""

' Check for required parameters
If IsEmpty(Picklist) then
	strError = strError & "PicklistName is a required parameter<BR>"
	Response.Write strError
	Response.end
End if
'This XML load should be moved where it is done less often

sURL = Server.MapPath("\cheminv\config\invloader.xml")
Set oFieldXML = CreateObject("MSXML2.DOMDocument")
oFieldXML.async = False
oFieldXML.load sURL

If Len(oFieldXML.xml) = 0 then
	strError = strError & "Could not load invloader.xml<BR>"
	Response.Write strError
	Response.end
End if

Set oNode = oFieldXML.SelectSingleNode("//regpicklists/picklist[@name='" & PickList & "']")
If oNode is nothing then
	strError = strError & "Could not find regpicklists with name '" & PickList & "' in invloader.xml<BR>"
	Response.Write strError
	Response.end
End if

SQL = oNode.text
If Picklist = "PREFIX" then 'CBOE-56 SJ Getting the person id from the COEDB.PEOPLE  table for passing it to the SQL for getting the sequence.
	IsLDAPUser = IsValidLDAPUser(UserID, Password) 
    If IsLDAPUser = 1 Then  'CBOE-1393 SJ Checking if LDAP user and generating the password.
	    OraPassword = GeneratePwd(UserID)
    Else
        OraPassword = Password
    End If  
    Conn = "FILE NAME=" & Application("CS_SECURITY_UDL_PATH") & ";User ID=" & UserID & ";Password=" & OraPassword
    Set Rs = Server.CreateObject("ADODB.Recordset")
    StrSql = "Select Person_Id from COEDB.PEOPLE where User_id = '" & UCase(UserID) & "'"
    Rs.Open StrSql, Conn
    If not Rs.EOF then
        PersonId = rs("Person_Id")
        SQL = SQL & " Where (PersonId = " & PersonId & " OR SiteID is null) and TYPE IN('R','A') Order by PREFIX"  'CBOE-859 SJ getting default prefixes
    End If
    Rs.Close
    Set Rs = nothing
End If

If SQL <> "" Then
    Set soapClient = Server.CreateObject("MSSOAP.SoapClient30")
    soapClient.ClientProperty("ServerHTTPRequest") = True
    Set headerHandler = Server.CreateObject("COEHeaderServer.COEHeaderHandler")
    Call soapClient.MSSoapInit(Application("SERVER_TYPE") & RegServerName & "/COERegistration/webservices/COERegistrationServices.asmx?wsdl")
    headerHandler.UserName = UserID
    headerHandler.Password = Password
    Set soapClient.HeaderHandler = headerHandler
    strXML = soapClient.RetrievePicklist(SQL)
    Response.Write strXML 
    Response.End
Else
    Response.Write "Error: Invalid picklist - " & Picklist
    Response.End
End If
</SCRIPT>

