<%@ Language=VBScript %>
<%
Newpath= Request.Form("LocationBox")

openNodes = Session("TreeViewOpenNodes1")
//Response.Write "<SCRIPT language=JavaScript>alert('" & openNodes & "')</script>"
If IsNumeric(NewPath) then
	Response.Write "<script language=javascript>top.TreeFrame.location.href='BrowseTree.asp?TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" & NewPath &  "&sNode=" & NewPath & openNodes & "&Exp=Y#" & NewPath & "';</script>"
Else
	NewLocationID = GetLocationIDFromPath(NewPath)
	if NewLocationID = 0 then
		Response.Write "<script language=javascript>alert(""Cannot find location '"  & NewPath & "'. Make sure the path is correct."");</script>"
	Else
		Response.Write "<script language=javascript>top.TreeFrame.location.href='BrowseTree.asp?TreeID=0&GotoNode=0&sNode=0&Exp=Y#0';</script>"
	End if
End if



Function GetLocationIDFromPath(pPath)
	GetLocationIDFromPath= 0
End Function






%>
