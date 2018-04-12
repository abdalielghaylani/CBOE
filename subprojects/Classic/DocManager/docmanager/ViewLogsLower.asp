<%Option Explicit%>

<%
dim fso, logFileName, f, logsLocation

logFileName = Request("LogFileName")
logsLocation = request("logsLocation")

if logFileName <> "" then
	Set fso = CreateObject("Scripting.FileSystemObject")
	Set f = fso.OpenTextFile(logsLocation & logFileName, 1) 'ForReading
end if

%>

<html>
<head>
<title>View Logs</title>


</head>

<body background="<%=Application("UserWindowBackground")%>">

<table>
	<tr><td width="100"></td>
		<td>
			<%
			if TypeName(f) = "TextStream" then
				Do While f.AtEndOfStream <> true%>
					<%=f.ReadLine%>
					<br>
				<%Loop%>
			
				<%
				f.Close
				Set fso = nothing
			end if
			%>

		</td>
	</tr>
</table>
</body>
</html>
