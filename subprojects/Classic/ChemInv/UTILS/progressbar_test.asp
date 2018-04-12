<%@ Language=VBScript %>


<!--#INCLUDE FILE = "progressbar_funcs.asp"-->
<%
InitializeProgressBar()

For i = 1 to 5000									
		Progressbar i, 50, 100
Next		

%>


