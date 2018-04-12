<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim Cmd
Dim LocationID
Dim BumpUpPercent
Dim sReturnValue
Dim aSplit
Dim PackageIDList
Dim QtyList
Dim bDebugPrint

LocationID = CLng(Request.Form("LocationID"))
BumpUpPercent = CDbl(Request.Form("BumpUpPercent"))

bDebugPrint = false

Call GetInvCommand(Application("CHEMINV_USERNAME") & ".DFCI_CREATEORDERS", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 30000 )
Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID", adNumeric, adParamInput, 4, LocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PBUMPPERCENTAGE", adDouble, adParamInput, 8, BumpUpPercent)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".DFCI_CREATEORDERS")
End if

sReturnValue = Cmd.Parameters("RETURN_VALUE")
if Len(sReturnValue) > 0 then
    aSplit= split(sReturnValue,"::")
    PackageIDList = aSplit(0)
    QtyList = aSplit(1)    
    QueryString = "/ChemInv/GUI/ImportFromChemACX.asp?LocationId=" & LocationID & "&PackageIDList=" & PackageIDList & "&QtyList=" & QtyList
    Response.Redirect(QueryString)
end if
%>
<html>
<body>
<center>
<span class="GuiFeedback">All stocks are currently up to date or have already been reordered.</span>
<br /><br />
<table border="0" cellspacing="0" cellpadding="0" width="300">
	<tr>
		<td colspan="2" align="right" height="20" valign="bottom"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="../graphics/close_dialog_btn.gif" border="0" alt="Close" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>	
</center>
</body>
</html>