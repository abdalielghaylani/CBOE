<%
Sub ShowSendToRegLink(CsNum)
	Dim QS
	QS = "CsNum=" & CsNum & "&CAS=" & CAS 
	Response.Write "<a href=""#"" onclick=""OpenDialog('/chemacx/chemacx/sendToChemReg.asp?" & QS & "','SendToReg', 1); return false"" class=""MenuLink"" title=""Add this compound to Chemical Registy"">Register</a>"
End Sub
%>