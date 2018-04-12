<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<html>
<head>
<title>Create Test Request from Registry Compounds</title>
<script LANGUAGE="javascript" src="/chem_reg/RegAnaliticsRequest/Choosecss.js"></script>
</head>

<body>
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top">
			<img src="../graphics/logo_regsys_250.gif" border="0" WIDTH="286" HEIGHT="50">
		</td>
		<td align="right">
			<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameReg"))%></b></font>
		</td>
	</tr>
</table>
<%
Dim Conn
Dim RegNumRS
Dim fso
Dim filePath
Dim plateSize

'stop

filePath = "c:\test\"
singletonFileName = "singletons.txt"

plateSize = request("plateSize")
httpReferer = Request.ServerVariables("HTTP_REFERER")

expandListStart = Instr(httpReferer, "expandList=")
If expandListStart > 0 then
	expandList = Mid(httpReferer, expandListStart + Len("expandList="), 1)
end if

if plateSize = "" then
	plateSize = 80
end if

Set fso = Server.CreateObject("Scripting.FileSystemObject")

Set Conn = GetNewConnection("reg", "base_form_group", "base_connection")

checkedExperiments = request("checkedExperiment")

if expandList = "1" then
	checkedBatches = request("checkedBatch")
else
	'regidListStart = Instr(httpReferer, "RegIDList=")
	'If regidListStart > 0 then
	'	regidListStart = regidListStart + Len("RegIDList=")
	'	regidList = Mid(httpReferer, regidListStart, Instr(regidListStart, httpReferer, "&") - regidListStart)
	'end if
	
	regidList = request("RegIDList")
	regidListArr = Split(regidList, ",")
	
	checkedBatches = ""
	for q = 0 to UBound(regidListArr)
		if request("BatchNum_" & regidListArr(q)) <> "" and IsEmpty(request("BatchNum_" & regidListArr(q))) = false and IsNull(request("BatchNum_" & regidListArr(q))) = false then
			checkedBatches = checkedBatches & "," & regidListArr(q) & "_" & request("BatchNum_" & regidListArr(q))
		end if
	next
	
	checkedBatches = right(checkedBatches, Len(checkedBatches) - 1)
end if

experimentArr = split(checkedExperiments, ",")

batchArr = split(checkedBatches, ",")

if UBound(batchArr) > 0 then
	plateNumber = (UBound(batchArr) + 1) \ plateSize

	if (UBound(batchArr) + 1) mod plateSize = 0 then

	else
		plateNumber = plateNumber + 1
	end if

	Response.write "<p><span class=""GUIFeedback"">Perform following experiments</span></p>"
	k = 1
	for k = 1 to UBound(experimentArr) + 1
		Response.Write "&nbsp;&nbsp;&nbsp;&nbsp;" & (Trim(experimentArr(k - 1))) & "<br>"
	next
		
	Response.Write "<p><span class=""GUIFeedback"">On following batches</span></p>"
	
	for i = 1 to plateNumber
		Response.Write "<p><span class=""GUIFeedback"">Plate " & i & "</span></p>"
		Set f = fso.CreateTextFile(filePath & ZeroPadding(Year(Now), 4) & ZeroPadding(Month(Now), 2) & ZeroPadding(Day(Now), 2) & ZeroPadding(Hour(Now), 2) & ZeroPadding(Minute(Now), 2) & ZeroPadding(Second(Now), 2) & "_" & i & "_B.txt")
		
		'f.writeline "Perform following experiments " & vbcrlf
		'k = 1
		'for k = 1 to UBound(experimentArr) + 1
		'	f.writeline(Trim(experimentArr(k - 1)))
		'next
		
		'f.writeline  vbcrlf & "on following batches" & vbcrlf
		
		j = 1
		for j = 1 to plateSize
			
			if ((i - 1) * plateSize + j - 1) <= UBound(batchArr) then
				regIDBatchIDArr = Split(batchArr((i - 1) * plateSize + j - 1), "_")
			
				regID = regIDBatchIDArr(0)
				BatchID = regIDBatchIDArr(1)
			
				sql= "SELECT reg_number FROM Reg_Numbers WHERE Reg_id = " & regID
				Set RegNumRS = Conn.Execute(sql)
			
				if not (RegNumRS.BOF and RegNumRS.EOF) then
					regNumber = RegNumRS("REG_NUMBER")
				end if
			
				f.WriteLine regNumber & "|" & batchID
				Response.Write "&nbsp;&nbsp;&nbsp;&nbsp;" & regNumber & "|" & batchID & "<br>"
			end if
		next
		
		f.Close
		Response.Write "<br>"
	next
	Response.Write "<input type=""button"" value=""Print"" onclick=""window.print()""><br>"
else 'singleton
	Set f = fso.OpenTextFile(filePath & singletonFileName, 8, true)
	
	regIDBatchIDArr = Split(batchArr(0), "_")
			
	regID = regIDBatchIDArr(0)
	BatchID = regIDBatchIDArr(1)
			
	sql= "SELECT reg_number FROM Reg_Numbers WHERE Reg_id = " & regID
	Set RegNumRS = Conn.Execute(sql)
			
	if not (RegNumRS.BOF and RegNumRS.EOF) then
		regNumber = RegNumRS("REG_NUMBER")
	end if
			
	f.WriteLine regNumber & "|" & batchID

end if

' CSBR-74509, 8/1/2007, Tao Ran, just replaced requesets with requests
Response.Write "Your test requests are saved in " %><%=filePath%>

<%
Function ZeroPadding(ByVal inputStr, ByVal n)

Dim retVal
Dim k

if len(inputStr) < n then
	numOfZeros = n - len(inputStr)
	
	zeros = ""
	
	for k = 1 to numOfZeros
		zeros = "0" & zeros 
	next
end if

retVal = zeros & inputStr

ZeroPadding = retVal

End Function
%>
</body>
</html>