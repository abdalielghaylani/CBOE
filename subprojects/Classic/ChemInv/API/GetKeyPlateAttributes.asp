<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim RS
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID
Dim PlateBarcode
Dim PlateID
Dim PlateBarcodeLen
Dim PlateIDLen
Dim aPlateBarcodes

Response.Expires = -1

bDebugPrint = False
bWriteError = False
strError = "Error:GetKeyPlateAttributes<BR>"
PlateBarcode = Request("PlateBarcode")
PlateID = Request("PlateID")
InvSchema = Application("CHEMINV_USERNAME")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetKeyPlateAttributes.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(PlateID) AND IsEmpty(PlateBarcode) then
	strError = strError & "Either PlateID or PlateBarcode is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

if IsEmpty(PlateID) then 
    PlateID= NULL
    PlateIDLen = 1
else
    PlateIDLen = Len(PlateID)
end if

if IsEmpty(PlateBarcode) then 
    PlateBarcode= NULL
    PlateBarcodeLen = 1
else
    aPlateBarcodes = split(PlateBarcode,",")
    PlateBarcode = ""
    for i = 0 to UBound(aPlateBarcodes)
        if( i <> 0 ) then
            PlateBarcode = PlateBarcode & ","
        end if
        PlateBarcode = PlateBarcode & "'" & aPlateBarcodes(i) & "'"
    next
    
    PlateBarcodeLen = Len(PlateBarcode)
end if    

GetInvConnection()
Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETKEYPLATEATTRIBUTES", 4)		 
Cmd.Parameters.Append Cmd.CreateParameter("PLATEID", 200, 1, PlateIDLen, PlateID) 
Cmd.Parameters.Append Cmd.CreateParameter("PLATEBARCODE", 200, 1, PlateBarcodeLen, PlateBarcode) 

if bdebugPrint then
	Response.Write "Parameters:<BR>"
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
	Response.end
else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.EOF AND RS.BOF) then
		Response.write RS.GetString(2,,",","|","")
	else
		Response.Write ""
	end if
end if
</SCRIPT>
