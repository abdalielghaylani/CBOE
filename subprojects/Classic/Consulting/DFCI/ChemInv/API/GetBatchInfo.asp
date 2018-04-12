<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim RS
Dim ConStr
Dim strError
Dim bWriteError
Dim PrintDebug
Dim CSUserName
Dim CSUserID

Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:GetBatchInfo<BR>"
RegID = Request("RegID")
BatchNumber = Request("BatchNumber")
RegNumber = Request("RegNumber")
'CsUserName = Application("INVREG_USERNAME") 
'CsUserID = Application("INVREG_PWD")
CsUserName =Request("tempCsUserName") 
CsUserID = URLDecode(CryptVBS(request("tempCsUserID"),CsUserName))
if CsUserID=NULL or CsUserID="" then CsUserID=CsUserName
' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetBatchInfo.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(RegID) AND IsEmpty(RegNumber) then
	strError = strError & "Either RegID or RegNumber is a required parameter<BR>"
	bWriteError = True
End if

' Set default batch
If BatchNumber = "" then BatchNumber = 1

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

if NOT IsEmpty(RegNumber) then 
	strWhere = "Upper(RegNumber)='" & Ucase(RegNumber) & "'"
Else
	strWhere = " RegID = '" & RegID & "'"
End if


' 1 = reg_number
' 2 = notebook_text
' 3 = notebook_page
' 4 = scientist_user_id	
' 5 = amount_units
' 6 = amount
' 7 = reg_id
' 8 = batch_number

if CBool(Application("UseNotebookTable")) then
	notebookSQL = " (SELECT distinct notebooks.notebook_name FROM notebooks,inv_vw_reg_batches WHERE notebook_number = inv_vw_reg_batches.RegNoteBookid and " & strWhere & " ) AS notebook_text," 
else
	notebookSQL = " regnotebook, "
end if

strSQL = "SELECT " &_
			" regnumber, " &_
			notebookSQL &_
			" regpage, " &_
			" regscientist, " &_
			" regamountunits, " &_
			" regamount, " &_
			" regid, " &_
			" batchnumber, " &_
			" 'END' as End_Char " &_
			" FROM inv_vw_reg_batches " &_
			" Where " & strWhere &_ 
			" AND batchnumber = '" & BatchNumber & "'"

if bdebugPrint then
	Response.Write strSQL
	Response.end
else
	Response.write GetListFromSQLRow_REG(strSQL)
end if

'Function to Decode the encrypted string 
Function URLDecode(str) 
	str = Replace(str, "+", " ") 
    For i = 1 To Len(str) 
		sT = Mid(str, i, 1) 
        If sT = "%" Then 
			If i+2 < Len(str) Then 
				sR = sR & _ 
                Chr(CLng("&H" & Mid(str, i+1, 2))) 
                i = i+2 
            End If 
        Else 
			sR = sR & sT 
        End If 
   Next 
   URLDecode = sR 
End Function 

</SCRIPT>
