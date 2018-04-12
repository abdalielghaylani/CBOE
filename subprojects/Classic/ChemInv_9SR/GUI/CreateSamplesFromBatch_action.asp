<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint

bDebugPrint = false

BatchContainerIDs = Request("BatchContainerIDs")
Distribution = Request("Distribution")
LocationID = Request("LocationID")
ContainerTypeID = Request("ContainerTypeID")
ContainerSize = Request("ContainerSize")
RequestID = Request("RequestID")
BarcodeDescID = Request("BarcodeDescID")
DateCertified = Request("DateCertified")
'for each key in Request.Form
'	Response.Write key & "=" & Request.Form(key) & "<BR>"
'next
'Response.End



ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

arrTemp1 = split(Distribution,",")
currSampleIndex = 1
for i=0 to ubound(arrTemp1)
	arrTemp2 = split(arrTemp1(i),":")
	currContainerID = arrTemp2(0)
	currContainerBarcode = arrTemp2(1)
	currNumSamples = cint(arrTemp2(2))
	currQtyRemaining = arrTemp2(3)
	currUOMAbv = arrTemp2(4)
	if currNumSamples > 0 then
		FormData = "ContainerID=" & currContainerID
		FormData = FormData & "&NumContainers=" & currNumSamples
		FormData = FormData & "&LocationID=" & LocationID
		FormData = FormData & "&ContainerTypeID=" & ContainerTypeID
		FormData = FormData & "&Action=sample"
		FormData = FormData & "&ContainerSize=" & ContainerSize
		FormData = FormData & "&BarcodeDescID=" & BarcodeDescID
		for j = 1 to currNumSamples
			currQty = eval("Request(""Sample" & currSampleIndex & """)")
			FormData = FormData & "&Sample" & j & "=" & currQty
			currSampleIndex = currSampleIndex + 1
		next
		FormData = FormData & "&SampleQtyUnit=" & SampleQtyUnit
		FormData = FormData & "&QtyRemaining=" & currQtyRemaining
		FormData = FormData & "&RequestID=" & RequestID
		FormData = FormData & "&DateCertified=" & DateCertified
		if len(Application("StatusRequestedSamples")) > 0 then	FormData = FormData & "&ContainerStatusID=" & Application("StatusRequestedSamples")
		FormData = FormData & Credentials
	
		'Response.Write FormData & "<BR>"
		'Response.End
		'allot the containers
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/AllotContainer.asp", "ChemInv", FormData)
		out = trim(httpResponse)
		'Response.Write out & "<BR>"

	end if
next


'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Samples from Batch</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
<%
			If isNumeric(out) then
				if Clng(out) > 0 then
					containerList = out
					theAction = "SelectContainer"
				Else
					theAction = "WriteAPIError"
				End if
			Elseif InStr(Left(out,18),"|") then
				containerList = out
				out = left(out,InStr(out,"|")-1)
				theAction = "SelectContainer"	
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:"&theAction)
			Select Case theAction
				Case "SelectContainer"
					Session("CurrentContainerID")= Request("ContainerID")
					Session("CurrentLocationID")= Request("LocationID") 
					Response.Write "<SPAN class=""GuiFeedback"">New Container has been created</SPAN>"
					Response.Write "<script language=javascript>"
					'Response.Write	"if (opener){opener.top.ListFrame.location.reload();}"
					Response.Write	"if (opener){opener.location.reload();}"
					Response.Write "</script>"
					Response.write "<Script language=javascript>window.close();</script>"		
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>New Container cannot be created</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
