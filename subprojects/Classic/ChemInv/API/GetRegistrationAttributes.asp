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
stop
Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:GetBatchInfo<BR>"
Action= ucase(Request("action"))
RegID = Request("RegID")
BatchNumber = Request("BatchNumber")
RegNumber = Request("RegNumber")
if Request.QueryString<>"" Then
    RegBatchID= Request.QueryString("REGBATCHID")
else
    RegBatchID= Request("REGBATCHID") 
End if    
CsUserName =Request("tempCsUserName") 
' The key to CryptVBS can be anything, so long as it matches what was used to generate the 
' encrypted string inside InvLoader
CsUserID = URLDecode(CryptVBS(request("tempCsUserID"),"ChemInv\API\GetBatchInfo.asp"))
if CsUserID=NULL or CsUserID="" then CsUserID=CsUserName


select case Action
case "GETREGBATCHID"
    strSQL = "SELECT " &_
			" REGBATCHID " &_
			" FROM inv_vw_reg_batches " &_
			" Where  batchnumber = '" & BatchNumber & "'" &_
			" AND REGID=" & REGID  
    if bdebugPrint then
	    Response.Write strSQL
	    Response.end
    else
	    Response.write GetListFromSQLRow_REG(strSQL) 
    end if

case "GETREGATTRIBUTES"
    ' 1 = reg_number
    ' 2 = notebook_text
    ' 3 = notebook_page
    ' 4 = scientist_user_id	
    ' 5 = amount_units
    ' 6 = amount
    ' 7 = reg_id
    ' 8 = batch_number
strWhere = " RegID = '" & RegID & "'"
    if CBool(Application("UseNotebookTable")) then
	    notebookSQL = " (SELECT distinct notebooks.notebook_name FROM regdb.notebooks,inv_vw_reg_batches WHERE notebook_number = inv_vw_reg_batches.RegNoteBookid and " & strWhere & " ) AS notebook_text," 
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
			    " Where REGBATCHID='"  & REGBATCHID & "'"
        if bdebugPrint then
	        Response.Write strSQL
	        Response.end
        else
	        Response.write GetListFromSQLRow_REG(strSQL)
        end if

end select
</SCRIPT>
