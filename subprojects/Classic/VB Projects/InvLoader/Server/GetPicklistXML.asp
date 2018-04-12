<%@ EnableSessionState=False Language=VBScript%>
<Script RUNAT="Server" Language="VbScript">
Dim UserID
Dim Password
Dim RegServerName
Dim Picklist
Dim SQL

Response.Expires = -1

UserID = Request("USRCODE") 
Password = Request("AUTHENTICATE")
Picklist = Request("PICKLIST")
RegServerName = Application("REGSERVERNAME")

Select Case UCase(Picklist) 
    Case "PREFIX"
        SQL = "SELECT SEQUENCEID, NVL(PREFIX,'No Prefix') AS PREFIX FROM VW_SEQUENCE ORDER BY PREFIX"
    Case "PROJECT" 
        SQL = "SELECT PROJECTID, NAME FROM VW_PROJECT WHERE ACTIVE='T' ORDER BY NAME"
'    Case "COMPOUNDTYPE" 
'        SQL = "SELECT COMPOUND_TYPE, DESCRIPTION FROM VW_COMPOUND_TYPE ORDER BY DESCRIPTION"
    Case "NOTEBOOK" 
        SQL = "SELECT NOTEBOOKID, NAME FROM VW_NOTEBOOK WHERE ACTIVE='T' ORDER BY NAME"
    Case "SALT" 
        SQL = "SELECT FRAGMENTID, DESCRIPTION FROM VW_FRAGMENT F, VW_FRAGMENTTYPE FT WHERE F.FRAGMENTTYPEID=FT.ID AND UPPER(FT.DESCRIPTION)='SALT' ORDER BY CODE"
    Case "FRAGMENT" 
        SQL = "SELECT FRAGMENTID, DESCRIPTION FROM VW_FRAGMENT ORDER BY CODE"
    Case "BATCHPROJECT" 
        SQL = "SELECT PROJECTID, NAME FROM VW_PROJECT WHERE ACTIVE='T' ORDER BY NAME"
    Case "ACTIVEPERSON"
        SQL = "SELECT PERSON_ID, USER_ID FROM CS_SECURITY.PEOPLE WHERE ACTIVE=1 ORDER BY USER_ID"
        
    Case Else
        SQL = "Error"
End Select 

If SQL <> "Error" Then
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

