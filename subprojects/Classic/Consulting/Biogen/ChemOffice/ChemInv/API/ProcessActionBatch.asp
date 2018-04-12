<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/xml_utils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetOrderContainer_cmd.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetCreateContainer_cmd.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/custom/api/orderline.asp"-->
<%
Dim Conn
Dim Cmd
Dim isCommit
Dim OrderLine
Dim pkgnum

bDebugPrint = false
bWriteError = False
bFailure = false
strError = "Error:<BR>"

LocationID = NULL
UOMID = NULL
QtyMax = NULL
QtyInitial = NULL
ContainerTypeID = NULL
ContainerStatusID = NULL
CurrentUserID= Session("UserID" & "cheminv")
Response.Expires = -1

if Request("sid")<> "" then
	sid = Request("sid")
else 
	sid = Session.sessionid	
end if
SessionDir = Application("TempFileDirectory" & "ChemInv") & "Sessiondir"  & "\" & sid
ActionBatchFile = SessionDir & "\InvActionBatch.xml" 

action = request("action")
if action = "commit" then 
	isCommit = true
Else
	isCommit = false
	action = "test"
End if

'Response.End

'Create the parser object
dim xmlDoc
set xmlDoc = Server.CreateObject("Msxml2.DOMDocument")
xmlDoc.async = false
xmlDoc.load(ActionBatchFile)

Set root = xmlDoc.documentElement
AddAttributeToElement xmlDoc, root, "timeStamp", ConvertDateToStr(Application("DATE_FORMAT"),Now())
AddAttributeToElement xmlDoc, root, "actionType", action

'Process CREATESUBSTANCE Tags
For Each actionNode In root.SelectNodes("CREATESUBSTANCE")
	FormData = ""
	For Each reqAttrib In actionNode.Attributes
		FormData = FormData & "&inv_compounds." & reqAttrib.NodeName & "=" & CnvString(reqAttrib.text)
	Next
	'-- check for empty substance name
	substanceName = actionNode.SelectSingleNode("substanceName").text
	if isEmpty(substanceName) or substanceName = "" then
		substanceName = "[Empty Substance Name]"
	end if
	FormData = FormData & "&inv_compounds.substance_name=" &  substanceName
	strucData = actionNode.SelectSingleNode("structure").text 
	FormData = FormData & "&inv_compounds.structure=" & Server.URLEncode(strucData)
	FormData = Right(FormData, Len(FormData)-1)
	ServerName = Application("InvServerName")
	Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
	Target = "/cheminv/api/CreateSubstance.asp?ResolveConflictsLater=1&RegisterIfConflicts=true&action=" & action
	FormData = FormData & Credentials
	
	httpResponse = CShttpRequest2("POST", ServerName, Target, "ChemInv", FormData)
	
	
	firstQuote = InStr(1,httpResponse,"""")
	secondQuote = InStr(firstQuote +1,httpResponse,"""")
	cID = Mid(httpResponse, firstQuote + 1 , SecondQuote - firstQuote - 1)
	
	Select Case true
		Case inStr(1,httpResponse,"<DUPLICATESUBSTANCE") > 0
			result = "WARNING"
			message = "Duplicate Substance Created CompoundID= " & cID
		Case inStr(1,httpResponse,"<NEWSUBSTANCE") > 0
			result = "SUCCEEDED"
			message = "New substance Created " & cID
		Case inStr(1,httpResponse,"<EXISTINGSUBSTANCE") > 0
			result = "SUCCEEDED"
			message = "Existing substance used CompoundID= " & cID
		Case else
			Response.Write "Error while creating inventory substance.<BR>"
			Response.Write httpResponse & "<BR><BR>"
			Response.end
	End Select
	CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "result", result
	packageID = ActionNode.SelectSingleNode("./@packageID").text
	Set MatchingNode = root.SelectSingleNode("CREATECONTAINER[@ID='" & packageID & "']")
	AddAttributeToElement xmlDoc, MatchingNode, "CompoundID", cID
Next 
'xmlDoc.save(ActionBatchFile)
'Server.transfer("/cheminv/api/displayactionbatch.asp")
'Response.End
Call GetInvConnection()

' Process CREATECONTAINER Tags
Conn.BeginTrans()
Set Cmd = Server.CreateObject("ADODB.Command")
Set Cmd.ActiveConnection = Conn
Cmd.CommandType = adCmdStoredProc

'The following vars are expected by the GetOrderContainer_cmd but are reset later as params
DueDate="1/1/2000"
Project="1"
Job="1"
DeliveryLocationID="1"
ActionNodeName = "CREATECONTAINER"
bIsFirst = True

For Each actionNode In root.SelectNodes(ActionNodeName)
	' TODO generalize to other actions
	actionName = actionNode.NodeName
	'Response.Write actionName & "<BR>"
	lastCommandText = Cmd.CommandText
	isRegContainer = CBool(actionNode.selectnodes("OPTIONALPARAMS/REGID").length)
	if NOT isRegContainer and Application("ORDER_WORKFLOW_OPTION")=1 then ' it's ACX container so we order it 	
		CommandText = Application("CHEMINV_USERNAME") & ".OrderContainer"
		Cmd.CommandText = CommandText
		if lastCommandText <> CommandText then
			if not bIsFirst then Call DeleteParameters()
			Call GetOrderContainer_cmd()
		end if
	else ' it's reg container so we just create it 	
		CommandText = Application("CHEMINV_USERNAME") & ".CreateContainer"
		Cmd.CommandText = CommandText
		if lastCommandText <> CommandText then
			if not bIsFirst then Call DeleteParameters()
			Call GetCreateContainer_cmd()
		end if
	End if
	'Loop the required params
	For Each reqAttrib In actionNode.Attributes
		'Response.Write  reqAttrib.NodeName & " = " & reqAttrib.Value & "<BR>"
		pname= "p" & reqAttrib.NodeName
		
		if pname <> "pID" and  pname <> "pUOMName" and pname <> "pContainerTypeName" then
			if reqAttrib.text <> ""  then
				'Response.Write pname & "<BR>"
				'Response.Write CnvString(reqAttrib.text) & "<BR>"
				if not ((pname="pDeliveryLocationID" or pname="pProjectNo" or pname="pJobNo" or pname="pDueDate" or pname="pOrderReasonID") and Application("ORDER_WORKFLOW_OPTION")=2) then 
				    Cmd.Parameters(pname) = CnvString(reqAttrib.text)
				end if 
			end if
		end if
	Next ' required Attribute
	
	'Response.end
	Cmd.Parameters("pCurrentUserID") = ucase(Session("USerName" & "ChemInv"))
	'Loop optional attributes
	For Each optAttrib In actionNode.selectNodes("./OPTIONALPARAMS/*")
		'Response.Write  optAttrib.NodeName & " = " & optAttrib.text & "<BR>"
		if optAttrib.NodeName = "PKGNUM" Then
		pkgnum=CnvString(optAttrib.text)
		elseif optAttrib.NodeName = "DUNS" then
		duns=CnvString(optAttrib.text)
		elseif optAttrib.NodeName = "UNSPSC" then
		unspsc=CnvString(optAttrib.text)
		elseif optAttrib.NodeName = "CATEGORY" then
		category=CnvString(optAttrib.text)
		else
		Cmd.Parameters("p" & optAttrib.NodeName) = CnvString(optAttrib.text)
		end if
		'Response.Write  optAttrib.NodeName & " = " & optAttrib.text & "<BR>"
	Next ' optional attribute
	'response.end
	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
		Response.write "<HR>"
	Else

	' Iproc call
	Dim RS5

	Set RS5 = Conn.Execute("select unit_abreviation from inv_units where unit_id=" & cmd.Parameters("PUOMID") & " and rownum=1" )
	if NOT (RS5.EOF AND RS5.BOF) then
	    result = RS5.GetString(2,1,",","","")
	    result = left(result,len(result)-1)
		
	Else
		result=""
	End if

	unit=result


	' end iproc update


		Call ExecuteCmd(Application("CHEMINV_USERNAME") & "." & ActionNodeName)
		returnCode =  Cstr(Cmd.Parameters("RETURN_VALUE"))
		'Response.Write returnCode & "<BR>"
		if returnCode > 0 then
			result = "SUCCEEDED"
			message = "ContainerID " & returnCode




		Call CreateOrderLineXML()
		'response.write OrderLine
		iProcCartId=Cmd.Parameters("PPONUMBER")
		iProcUserId=Cmd.Parameters("PCURRENTUSERID")
		
		Call DeleteParameters()
		CommandText = Application("CHEMINV_USERNAME") & ".insertShoppingCartXML"
		Cmd.CommandText = CommandText
		Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
		Cmd.Parameters("RETURN_VALUE").Precision = 9
		Cmd.Parameters.Append Cmd.CreateParameter("PCARTID",200, 1, 4000, iProcCartID)
		Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, 1, 50, iProcUserID)
		Cmd.Parameters.Append Cmd.CreateParameter("PORDERLINE",200, 1, 4000,  OrderLine)


		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".insertShoppingCartXML")	
		' End Iproc Call

		Else
			bFailure = true
			result = "FAILED"
			message = Application(returnCode)
		End if
		CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "result", result
		CreateOrReplaceNode xmlDoc, "RETURNCODE", actionNode, returnCode, "", null
	End if	
	bIsFirst = false
Next ' actionNode

AddAttributeToElement xmlDoc, root, "ErrorsFound", Cstr(bFailure)

xmlDoc.save(ActionBatchFile)
if isCommit then
		Call DeleteParameters()
		CommandText = Application("CHEMINV_USERNAME") & ".finishShoppingCartXML"
		Cmd.CommandText = CommandText
		Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
		Cmd.Parameters("RETURN_VALUE").Precision = 9
		Cmd.Parameters.Append Cmd.CreateParameter("PCARTID",200, 1, 4000, iProcCartID)
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".finishShoppingCartXML")	
		' End Iproc Call

	Conn.CommitTrans()
	if Not bFailure then
		AddAttributeToElement xmlDoc, root, "EmptyCart", "1"
	else
		AddAttributeToElement xmlDoc, root, "EmptyCart", "0"
	end if

else
	Conn.RollBackTrans()
end if

Server.transfer("displayactionbatch.asp")

Sub DeleteParameters()
	for i = 0 to (Cmd.Parameters.Count-1)
		Cmd.Parameters.Delete(0)
	next
end sub
%>
