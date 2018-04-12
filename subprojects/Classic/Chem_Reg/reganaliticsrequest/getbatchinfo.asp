<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/chem_reg/RegAnaliticsRequest/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/chem_reg/RegAnaliticsRequest/apiUtils.asp"-->
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

CsUserName = Application("REG_USERNAME") 
CsUserID = Application("REG_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.write "no parameters passed in"
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
	strWhere = "Upper(Reg_numbers.Reg_Number)='" & Ucase(RegNumber) & "'"
Else
	strWhere = "reg_numbers.Reg_ID = " & RegID
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
	notebookSQL = " (SELECT notebook_name FROM notebooks WHERE notebook_number = batches.notebook_internal_id) AS notebook_text," 
else
	notebookSQL = " batches.notebook_text AS notebook_text,"
end if

strSQL = "SELECT " &_
		 " reg_numbers.reg_number," &_	
		 notebookSQL &_	
		 " batches.notebook_page," &_			
		 " cs_security.people.user_id," &_		
		 " amount_units," &_					
		 " amount," &_					
		 " reg_numbers.reg_id," &_				
		 " batches.Batch_Number" &_		
		 " FROM reg_numbers, batches, cs_security.people" &_
		 " WHERE reg_numbers.reg_id = batches.reg_internal_id" &_
		 " AND batches.scientist_id = cs_security.people.person_id(+)" &_
		 " AND " & strWhere &_ 
		 " AND batches.batch_number=" & BatchNumber


if bdebugPrint then
	Response.Write strSQL
	Response.end
else
	Response.write GetListFromSQLRow_REG(strSQL)
end if
</SCRIPT>
