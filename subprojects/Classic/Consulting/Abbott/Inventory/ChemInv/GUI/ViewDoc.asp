<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

DocID = Request("DocID")

'-- Retreive document information
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".DOCS.GETDOC(?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("P_DOCID",131, 1, 0, DocID)
Cmd.Parameters("P_DOCID").Precision = 9	
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE
If (RS.EOF AND RS.BOF) then
	Response.Write ("<table><tr><td align=center colspan=6><span class=""GUIFeedback"">No document found</Span></td></tr></table>")
	Response.End 
Else
	Response.Write RS("Doc")
End if	

%>