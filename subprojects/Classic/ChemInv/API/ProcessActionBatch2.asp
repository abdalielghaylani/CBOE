<%@ EnableSessionState="False" Language=VBScript %>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/xml_utils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetOrderContainer_cmd.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetCreateContainer_cmd.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->

<Script RUNAT="Server" Language="VbScript">

Sub	CreateOrReplaceText(byref theDOM, byRef parentNode, byval text)
    Dim nodes
    Dim node
    Dim newChild

    set nodes = parentNode.SelectNodes("text()")
    for each node in nodes
        parentNode.RemoveChild(node)
    next
    set newChild = theDOM.createTextNode(text)
    set node = parentNode.firstchild
    if node is nothing then
        parentNode.appendChild newChild
    else
        parentnode.insertbefore newchild,node
    end if
End Sub


Server.ScriptTimeout = 3600 '1 hour timeout
Dim Conn
Dim Cmd
Dim isCommit
Dim aValuePairs
Dim strReturn

bDebugPrint = False
bWriteError = False
bFailure = false
strError = "Error:<BR>"

LocationID = NULL
UOMID = NULL
QtyMax = NULL
QtyInitial = NULL
ContainerTypeID = NULL
ContainerStatusID = NULL
actionType = NULL
Response.Expires = -1

RegServerName = Application("RegServerName")
InvServerName = Application("InvServerName")

isCommit = true

'-- base64_cdx for empty structure
INVMTCDX = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMAEAAAAENoZW1EcmF3IDYuMC4xCAAMAAAAbXl0ZXN0LmNkeAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wEJCAAAAFkAAAAEAAIJCAAAAKcCAAAXAgIIEAAAAAAAAAAAAAAAAAAAAAAAAwgEAAAAeAAECAIAeAAFCAQAAJoVAAYIBAAAAAQABwgEAAAAAQAICAQAAAACAAkIBAAAswIACggIAAMAYAC0AAMACwgIAAQAAADwAAMADQgAAAAIeAAAAwAAAAEAAQAAAAAACwARAAAAAAALABEDZQf4BSgAAgAAAAEAAQAAAAAACwARAAEAZABkAAAAAQABAQEABwABJw8AAQABAAAAAAAAAAAAAAAAAAIAGQGQAAAAAAJAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAEAhAAAAD+/wAA/v8AAAIAAAACAAABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAEAAAAEAhAAAAD+/wAA/v8AAAIAAAACAA8IAgABABAIAgABABYIBAAAACQAGAgEAAAAJAAAAAAA"
REGMTCDX = "VmpDRDAxMD"

'Create the parser object
dim xmlDoc
set xmlDoc = Server.CreateObject("Msxml2.DOMDocument")
xmlDoc.async = false

'Load ActionBatchFile if exists or request object
xmlDoc.load(request)

Set root = xmlDoc.documentElement
AddAttributeToElement xmlDoc, root, "timeStamp", CStr(Now())
AddAttributeToElement xmlDoc, root, "actionType", action
'CSBR ID     : 128893
'Modified by : sjacob
'Comments    : Adding code to authenticate LDAP users.
'Start of change
RegUserName = root.getAttribute("CSUSERNAME")
RegPwd = root.getAttribute("CSUSERID")
' Set Cred. for Create Substance
CSUserName = root.getAttribute("CSUSERNAME")
CSUserID = root.getAttribute("CSUSERID")

IsLDAPUser = IsValidLDAPUser(CSUserName, CSUserID)
If IsLDAPUser = 1 Then
    CSUserID = GeneratePwd(CSUserName)
End If
'End of change

'To check if Registration option is selected
'CSBR 126952 : sjacob
RegisterCompounds = root.getAttribute("REGISTERCOMPOUNDS")

' Set ID expected by GetCreateContainer_cmd
CurrentUserID = root.getAttribute("CSUSERID")
'Process CREATESUBSTANCE Tags
For Each actionNode In root.SelectNodes("CREATESUBSTANCE")
	'Response.Write("In Create Substance")
	FormData = ""
	For Each reqAttrib In actionNode.Attributes
    	'-- InvLoader 9.1 is adding substance name as an attribute and as a sub-element of CREATESUBSTANCE so for now i'll check for it here and not add it twice
        if reqAttrib.NodeName <> "SUBSTANCE_NAME" then
		    FormData = FormData & "&inv_compounds." & reqAttrib.NodeName & "=" & CnvString(reqAttrib.text)
		end if
	Next
	FormData = FormData & "&inv_compounds.substance_name=" & actionNode.SelectSingleNode("substanceName").text 
	strucData = actionNode.SelectSingleNode("structure").text 
	if Len(strucData) > 0 then
		FormData = FormData & "&inv_compounds.structure=" & Server.URLEncode(strucData)
	else
		FormData = FormData & "&inv_compounds.structure=" & Server.URLEncode(INVMTCDX)
		'FormData = FormData & "&inv_compounds.structure="
	end if
	FormData = Right(FormData, Len(FormData)-1)
	ServerName = Application("InvServerName")
	Credentials = "&CSUserName=" & CSUserName & "&CSUSerID=" & CSUserID
	'Target = "/cheminv/api/CreateSubstance.asp?ResolveConflictsLater=1&RegisterIfConflicts=true&action=" & action
	Target = "/cheminv/api/CreateSubstance.asp?action=" & action

	FormData = FormData & Credentials & "&ResolveConflictsLater=1&RegisterIfConflicts=true"
	'FormData = FormData & Credentials
	'logaction(formData)
	sendTO = 5*60*1000
	receiveTO = 5*60*1000
	httpResponse = CShttpRequest3("POST", ServerName, Target, "ChemInv", FormData, sendTO, receiveTO)
	
	firstQuote = InStr(1,httpResponse,"""")
	secondQuote = InStr(firstQuote +1,httpResponse,"""")
	cID = Mid(httpResponse, firstQuote + 1 , SecondQuote - firstQuote - 1)
	
	Select Case true
		Case inStr(1,httpResponse,"<DUPLICATESUBSTANCE") > 0
			result = "WARNING"
			message = "Duplicate substance created CompoundID= " & cID
		Case inStr(1,httpResponse,"<NEWSUBSTANCE") > 0
			result = "SUCCEEDED"
			message = "New substance created " & cID
		Case inStr(1,httpResponse,"<EXISTINGSUBSTANCE") > 0
			result = "SUCCEEDED"
			message = "Existing substance used CompoundID= " & cID
		Case inStr(1,httpResponse,"<CONFLICTINGSUBSTANCES") > 0
			result = "SUCCEEDED"
			message = "Conflicting substance created, CompoundID= " & cID
		Case else
			Response.Write "Error while creating inventory substance.<BR>"
			Response.Write httpResponse & "<BR><BR>"
	End Select
	CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "RESULT", result
	Set returnValueNode = actionNode.SelectSingleNode("RETURNVALUE")
	AddAttributeToElement xmlDoc, returnValueNode, "COMPOUNDID", cID
	packageID = ActionNode.SelectSingleNode("./@PACKAGEID").text
	Set MatchingNode = root.SelectSingleNode("CREATECONTAINER[@ID='" & packageID & "']")
	AddAttributeToElement xmlDoc, MatchingNode, "CompoundID", cID
	actionNode.removeChild(actionNode.SelectSingleNode("structure"))
	'Response.Write(packageID)
	Response.Write ""
	Response.Flush
		
Next 

' Process REGISTERSUBSTANCE Tags
' ------------------------------
'Only if Registration is selected.
'CSBR 126952 : sjacob
If RegisterCompounds Then
    Dim soapClient
    Dim headerHandler
    Dim strXMLTemplate
    Dim xmlReg
    Dim nodes
    Dim node
    Dim structNode
    Dim strDupFlag
    Dim strXMLResponse
    Dim oSelection

    '  Set up SOAP client
    set soapClient = Server.CreateObject("MSSOAP.SoapClient30")
    set headerHandler = Server.CreateObject("COEHeaderServer.COEHeaderHandler")
    'CSBR ID     : 128893
    'Modified by : sjacob
    'Comments    : Changed the CsUsername and CsUserId to RegUserName and RegPwd.
    headerHandler.UserName = RegUserName
    headerHandler.Password = RegPwd
    set soapClient.HeaderHandler = headerHandler
    soapClient.ClientProperty("ServerHTTPRequest") = True
    call soapClient.MSSoapInit(Application("SERVER_TYPE") & RegServerName & "/COERegistration/webservices/COERegistrationServices.asmx?wsdl")
    soapClient.ConnectorProperty("Timeout") = 120000    ' sets timeout to 120 secs

    'Get registration XML template
    strXMLTemplate = soapClient.RetrieveNewRegistryRecord()
    'strXMLTemplate = soapClient.SetModuleName(strXMLTemplate, "INVLOADER") 'CBOE-1159 method for adding module name in xml template : ASV 10JUL13

    for each actionNode in root.SelectNodes("REGISTERSUBSTANCE")
    ' kfd debug -----------------
    '    Response.write vbCrLf & "actionNode: " & vbCrLf & actionNode.xml & vbCrLf & vbCrLf
    '    Response.flush
    ' ---------------------------    

    'Create the registration XML
        set xmlReg = Server.CreateObject("Msxml2.DOMDocument")
        xmlReg.async = false
        xmlReg.loadXML(strXMLTemplate)

        'RAG
        ' Set Module Name, 'CBOE-1159 method for adding module name in xml template RAG
        set regnode = xmlReg.SelectSingleNode("MultiCompoundRegistryRecord")
        if not (regnode is Nothing) then
            for each actionAttr in regnode.Attributes
                strAttrName = UCase(actionAttr.Name)
            
                if strAttrName = "MODULENAME" then
                     actionAttr.Value = "INVLOADER"
                end if
            next
        end if    

    'CSBR ID : 124898 : sjacob
    'Comment : Modifying the xmlReg for adding Project node under ProjectList    
    'Start of change 
        Set parentnode = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/ProjectList")
        CreateNode xmlReg, "Project", parentnode, "", "", ""
    'End of change
       
    ' kfd debug -----------------
    '    if xmlReg.parseError.errorCode <> 0 then
    '        Response.write "Parse error: " & xmlReg.parseError.reason
    '    end if   
    '    Response.write vbCrLf & "xmlReg.xml (before): " & vbCrLf & xmlReg.xml & vbCrLf & vbCrLf
    '    Response.flush
    ' ---------------------------

     'Remove the validation nodes
        Set oSelection = xmlDoc.selectNodes("//validationRuleList")
        oSelection.removeAll()
      
       strDupFlag = "T"     ' Default      

    'Transfer <structure> element from action node
        Set structNode = actionNode.SelectSingleNode("structure")
        if not (structNode is Nothing) then
            set node = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/Structure")
            if not (node is Nothing) then
                CreateOrReplaceText xmlReg,node,structNode.text
            end if
            'CSBR ID : 134649 :sjacob
            'Comments: The code below checks for 'No Structure' in the <Structure> element and assign the value -2 to the <StrutureID> for compounds
            '          which have no structure
            'Start of change     
            set node1 = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/StructureID")
            If node.Text = "No Structure" Then
                CreateOrReplaceText xmlReg,node1,"-2"
                'Removing the 'No Structure' text from <Structure> element
                CreateOrReplaceText xmlReg,node,""
            End If
            'End of change 
        end if

    'Transfer actionNode parameters into xmlReg document
        for each actionAttr in actionNode.Attributes
            strAttrName = UCase(actionAttr.Name)
            ' Process special nodes
            if strAttrName = "REGPARAMETER" then
                if actionAttr.Value = "USER_INPUT" then             ' Add Duplicate to Temp Table (T)emporary
                    strDupFlag = "T"
                elseif actionAttr.Value = "NEW_BATCH" then          ' Add New Batch to Registered Compound (B)atch
                    strDupFlag = "B"
                elseif actionAttr.Value = "OVERRIDE" then           ' Add New Compound (D)uplicate
                    strDupFlag = "D"
                elseif actionAttr.Value = "UNIQUE_DEL_TEMP" then    ' Ignore Compound (N)ot store
                    strDupFlag = "N"
                end if
    ' BASE64_CDX is urlencoded.  Replaced with structure tag extracted above.
    '        elseif strAttrName = "BASE64_CDX" then
    '            set node = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/Structure")
    '            if not (node is Nothing) then
    '                CreateOrReplaceText xmlReg,node,URLDecode(actionAttr.Value)
    ''                node.text = actionAttr.Value
    '            end if
            elseif strAttrName = "SEQUENCE" then
                set node = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/RegNumber/SequenceID")
                if not (node is Nothing) then
                    node.text = actionAttr.Value
                end if
        'CSBR ID : 124898 : sjacob
        'Comment : Changed PROJECT to PROJECTID
            elseif strAttrName = "PROJECTID" then
                Set node = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/ProjectList/Project")
                if not (node is Nothing) then
            	    CreateOrReplaceNode xmlReg, "ProjectID", node, actionAttr.Value, "", ""
                end if
            elseif strAttrName = "BATCH_PROJECT" then
                Set node = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/BatchList/Batch/ProjectList")
                if not (node is Nothing) then
            	    CreateOrReplaceNode xmlReg, "ProjectID", node, actionAttr.Value, "", ""
                end if
            else    ' Not a special node
                ' Rename nodes whose names changed from old Registration
                if strAttrName = "NOTEBOOK" then strAttrName = "NOTEBOOK_REF"
                
                If actionAttr.Value<>"" then
                    Set nodes = xmlReg.SelectNodes("//PropertyList/Property[@name=""" & strAttrName & """]")
                    for each node in nodes
                        CreateOrReplaceText xmlReg,node,actionAttr.Value
                    next
                end If
            end if
        next
    'Send the reg document off to be registered
    ' kfd debug -----------------
    '    Response.write vbCrLf & "xmlReg.xml (after): " & vbCrLf & xmlReg.xml & vbCrLf & vbCrLf
    '    Response.flush
    ' ---------------------------
        strXMLResponse = soapClient.CreateRegistryRecord(xmlReg.xml,strDupFlag)
    ' kfd debug -----------------
    '    Response.write vbCrLf & "Response: " & vbCrLf & strXMLResponse & vbCrLf & vbCrLf
    '    Response.flush
    ' ---------------------------

    'Process responseXML to find out what happened
        Dim xmlResponse
        set xmlResponse = Server.CreateObject("Msxml2.DOMDocument")
        xmlResponse.async = false
        xmlResponse.loadXML(strXMLResponse)
        
        set node = xmlResponse.selectSingleNode("/ReturnList/ActionDuplicateTaken")
        if node is nothing then
			    regID = ""
			    batchNumber = ""
			    result = "WARNING"
			    message = "Unknown RegisterCompound response: " & strXMLResponse
        else
            action = node.text
            set node = xmlResponse.selectSingleNode("/ReturnList/RegID")
            regID = node.text
            set node = xmlResponse.selectSingleNode("/ReturnList/BatchNumber")
            batchNumber = node.text
            select case ucase(action)
                'Not a duplicate, new compound and batch created
                case "N"
                    result = "SUCCEEDED"
                    message = "New compound registered, RegID=" & regID & ", BatchNumber=" & batchNumber 
                'Batch created
                case "B"
                    result = "SUCCEEDED"
                    message = "New batch created, RegID=" & regID & ", BatchNumber=" & batchNumber 
                'Temporary 
                case "T"
                    result = "SUCCEEDED"
                    message = "Temporary compound created, RegID=" & regID & ", BatchNumber=" & batchNumber 
                'Duplicate compound
                case "D"
                    result = "SUCCEEDED"
                    message = "Duplicate compound registered RegID=" & regID & ", BatchNumber=" & batchNumber 
                'Unknown
                case else
                    regID = ""
                    batchNumber = ""
                    result = "WARNING"
                    message = "Unknown RegisterCompound action returned: " & strXMLResponse
            end select
        end if
    	 'CBOE-1159 if chemical Warning exists add it as error: ASV 10JUL13
        set node = xmlResponse.selectSingleNode("/ReturnList/RedBoxWarning")
        if not (node is Nothing) then
            message = message & " " & node.text
        End if	
	    CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "RESULT", result
	    set returnValueNode = actionNode.SelectSingleNode("RETURNVALUE")
	    AddAttributeToElement xmlDoc, returnValueNode, "REGID", cStr(regID)
	    AddAttributeToElement xmlDoc, returnValueNode, "BATCHNUMBER", batchNumber
	    packageID = ActionNode.SelectSingleNode("./@PACKAGEID").text
	    Set MatchingNode = root.SelectSingleNode("CREATECONTAINER[@ID='" & packageID & "']")
	    AddAttributeToElement xmlDoc, MatchingNode, "REGID", cStr(regID)
	    AddAttributeToElement xmlDoc, MatchingNode, "BATCHNUMBER", batchNumber
	    actionNode.removeChild(actionNode.SelectSingleNode("structure"))
	    Response.Write ""
	    Response.Flush

    Next 
End If

GetSessionlessInvConnection()

'Process MOVECONTAINER Tags
set actionNodeList = root.SelectNodes("MOVECONTAINER")
if actionNodeList.length > 0 then
    Set GetPrimaryKeyCmd = Server.CreateObject("ADODB.Command")
    Set GetPrimaryKeyCmd.ActiveConnection = Conn
    GetPrimaryKeyCmd.CommandType = adCmdStoredProc
    GetPrimaryKeyCmd.CommandText = Application("CHEMINV_USERNAME") & ".GETPRIMARYKEYIDS"
    Set PKReturnValueParam = GetPrimaryKeyCmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 32000, NULL)
    GetPrimaryKeyCmd.Parameters.Append PKReturnValueParam
    Set TableNameParam = GetPrimaryKeyCmd.CreateParameter("PTABLENAME", adVarChar, adParamInput, 1, "")
    GetPrimaryKeyCmd.Parameters.Append TableNameParam
    Set TableValuesParam = GetPrimaryKeyCmd.CreateParameter("PTABLEVALUES", adVarChar, adParamInput, 1, "")
    GetPrimaryKeyCmd.Parameters.Append TableValuesParam
    GetPrimaryKeyCmd.Prepared = true

    Set MoveContainerCmd = Server.CreateObject("ADODB.Command")
    Set MoveContainerCmd.ActiveConnection = Conn
    MoveContainerCmd.CommandType = adCmdStoredProc
    MoveContainerCmd.CommandText = Application("CHEMINV_USERNAME") & ".MOVECONTAINER"
    Set MCReturnValueParam = MoveContainerCmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 8000, NULL)
    MoveContainerCmd.Parameters.Append MCReturnValueParam
    Set LocationIDParam = MoveContainerCmd.CreateParameter("PLOCATIONID", adVarChar, adParamInput, 1, "")
    MoveContainerCmd.Parameters.Append LocationIDParam
    Set ContainerIDParam = MoveContainerCmd.CreateParameter("PCONTAINERID", adVarChar, adParamInput, 1, "")
    MoveContainerCmd.Parameters.Append ContainerIDParam
    MoveContainerCmd.Prepared = true

    For Each actionNode In actionNodeList
	    ContainerID = null
	    LocationID = null
	    message = ""
	    For Each reqAttrib In actionNode.Attributes
	        if reqAttrib.NodeName = "CONTAINER_BARCODE" then
	            ContainerBarcode = CnvString(reqAttrib.text)
	            TableNameParam.size = Len("inv_containers")
	            TableNameParam.value = "inv_containers"	        
	            TableValuesParam.size = Len(ContainerBarcode)
	            TableValuesParam.value = ContainerBarcode
	            strReturn = ExecuteOracleFunction( GetPrimaryKeyCmd, "GETPRIMARYKEYIDS" )
	            if strReturn <> "" then
	                message = strReturn
	            else
	                if (instr(PKReturnValueParam.value,"Error") > 0) then
	                    message = PKReturnValueParam.value
	                elseif (PKReturnValueParam.value <> "NOT FOUND") then	                    
		                aValuePairs = split(PKReturnValueParam.value, "=")
		                if UBound(aValuePairs) > 0 then
			                ContainerID = aValuePairs(1)
			            end if
		            end if
		        end if
	        elseif reqAttrib.NodeName = "LOCATION_BARCODE" then
	            LocationBarcode = CnvString(reqAttrib.text)
	            TableNameParam.size = Len("inv_locations")
	            TableNameParam.value = "inv_locations"
	            TableValuesParam.size = Len(LocationBarcode)
	            TableValuesParam.value = LocationBarcode
	            strReturn = ExecuteOracleFunction( GetPrimaryKeyCmd, "GETPRIMARYKEYIDS" )
	            if strReturn <> "" then
	                message = strReturn
	            else
	                if (instr(PKReturnValueParam.value,"Error") > 0) then
	                    message = PKReturnValueParam.value
	                elseif (PKReturnValueParam.value <> "NOT FOUND") then
		                aValuePairs = split(PKReturnValueParam.value, "=")
		                if UBound(aValuePairs) > 0 then
			                LocationID = aValuePairs(1)
			            end if
		            end if
		        end if
	        end if
	    Next
    	
	    if not IsNull(ContainerID) and not IsNull(LocationID) then
	        LocationIDParam.size = Len(LocationID)
	        LocationIDParam.value = LocationID
	        ContainerIDParam.size = Len(ContainerID)
            ContainerIDParam.value = ContainerID
            strReturn = ExecuteOracleFunction( MoveContainerCmd, "MOVECONTAINER" )
            if strReturn <> "" then
                result = "FAILED"
                message = strReturn
	        else            
                if (instr(MCReturnValueParam.value,"Error") > 0) then
	                result = "FAILED"
			        message =  MCReturnValueParam.value
	            else
	                result = "SUCCEEDED"
			        message = "Container moved"
	            end if
	        end if
	    else
	        result = "FAILED"		    
	    end if	
    	
	    CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "RESULT", result
	    Set returnValueNode = actionNode.SelectSingleNode("RETURNVALUE")
	    Response.Write ""
	    Response.Flush
    Next 
    
    set GetPrimaryKeyCmd = nothing
    set MoveContainerCmd = nothing

end if      ' if actionNodeList.length > 0 then


' Process CREATECONTAINER Tags
' ----------------------------

'The following vars are expected by the GetOrderContainer_cmd but are reset later as params
DueDate="1/1/2000"
Project="1"
Job="1"
DeliveryLocationID="1"
ActionNodeName = "CREATECONTAINER"
bIsFirst = True

Set Cmd = Server.CreateObject("ADODB.Command")
Set Cmd.ActiveConnection = Conn
Cmd.CommandType = adCmdStoredProc

For Each actionNode In root.SelectNodes(ActionNodeName)

	' TODO generalize to other actions
	actionName = actionNode.NodeName
	
	lastCommandText = Cmd.CommandText

	CommandText = Application("CHEMINV_USERNAME") & ".CreateContainer"
	Cmd.CommandText = CommandText
	if lastCommandText <> CommandText then
		if not bIsFirst then Call DeleteParameters()
		Call GetCreateContainer_cmd()
	end if

	'Loop the required params
	For Each reqAttrib In actionNode.Attributes
		pname= "p" & reqAttrib.NodeName
		if pname <> "pID" then
			if reqAttrib.text <> ""  then
				if pname = "pQTY_MAX" then
					pname = "pMAXQTY"
				end if
				Cmd.Parameters(pname) = CnvString(reqAttrib.text)
			end if
		end if

	Next ' required Attribute

	Cmd.Parameters("pCurrentUserID") = ucase(CSUserName)
	'Loop optional attributes

	For Each optAttrib In actionNode.selectNodes("./OPTIONALPARAMS/*")
		Cmd.Parameters("p" & optAttrib.NodeName) = CnvString(optAttrib.text)
		'Response.Write  optAttrib.NodeName & " = " & optAttrib.text & "<BR>"
	Next ' optional attribute

	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
		Response.write "<HR>"
	Else
	
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & "." & ActionNodeName)
		returnCode =  Cstr(Cmd.Parameters("RETURN_VALUE"))

		if returnCode > 0 then
			result = "SUCCEEDED"
			message = "ContainerID " & returnCode
		Else
			bFailure = true
			result = "FAILED"
			message = Application(returnCode)
		End if
		CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "RESULT", result
		Set returnValueNode = actionNode.SelectSingleNode("RETURNVALUE")
		AddAttributeToElement xmlDoc, returnValueNode, "CONTAINERID", returnCode
		CreateOrReplaceNode xmlDoc, "RETURNCODE", actionNode, returnCode, "", null
		actionNode.removeChild(actionNode.SelectSingleNode("OPTIONALPARAMS"))
	End if	
	bIsFirst = false
	
	Response.Write ""
	Response.Flush	
Next ' actionNode

AddAttributeToElement xmlDoc, root, "ErrorsFound", Cstr(bFailure)
Root.removeAttribute ("CSUSERNAME")
Root.removeAttribute ("CSUSERID")

Response.Write(xmlDoc.xml)

Set oXMLHTTP = Nothing

Sub DeleteParameters()
	for i = 0 to (Cmd.Parameters.Count-1)
		Cmd.Parameters.Delete(0)
	next
end sub


'-------------------------------------------------------------------------------
' Name: LogAction(inputstr)
' Type: Sub
' Purpose:  writes imformation to a output file 
' Inputs:   inputstr  as string - variable to output
' Returns:	none
' Comments: writes informtion to /inetput/cfwlog.txt file
'-------------------------------------------------------------------------------
Sub LogAction(ByVal inputstr)
		on error resume next
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub

Function ExecuteOracleFunction(objCommand, strName)
    Dim strReturn
    strReturn = ""
	On Error Resume Next
	objCommand.Execute
	If Err then
		Select Case True
			Case inStr(Err.description,"ORA-20003") > 0
		        strReturn = strName & ": Cannot execute invalid Oracle procedure."
			Case inStr(Err.description,"access violation") > 0
			    strReturn = strName & ": Current user is not allowed to execute Oracle procedure."
			Case Else
			    strReturn = strName & ": " & Err.description
		End Select		
	End if
	on error goto 0
	
	ExecuteOracleFunction = strReturn
End function
	
Function GetRegID(RegNumber, user_id, user_pwd)
	If (Application("RegServerName") = "NULL")then
		Response.Write "<center><BR><BR><BR><BR><span class=""GUIFeedback"">You have insufficient Privileges to view Registration Database<BR>Contact your system administrator.</span></center>"
		Response.end
	End if
	
	if CBool(Application("UseNotebookTable")) then
		notebookSQL = " (SELECT notebook_name FROM notebooks WHERE notebook_number = batches.notebook_internal_id) AS NoteBook," 
	else
		notebookSQL = " batches.notebook_text AS NoteBook,"
	end if
	strSQL = "SELECT" &_
			 " reg_id " &_
			 " FROM reg_numbers" &_
			 " WHERE reg_numbers.reg_number =?" 

	'Response.Write "<BR><BR>"
	'Response.Write strSQL & ":" & RegNumber
	'Response.end
	connection_array = Application("base_connection" & "invreg")
	'Cannot use the oledb connection because it truncates the Base64_cdx Long field
	'ConnStr =  "dsn=chem_reg;UID=" & UserName & ";PWD=" & UserID
	'Now that reg uses clobs we can use the udl
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & user_id & ";" & Application("PWDKeyword") & "=" & user_pwd
	'Response.Write "<BR><BR><BR><BR>" & ConnStr
	'Response.end
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr 

	'GetRegConnection()
	Set Cmd = GetCommand(Conn, strSQL, &H0001) 'adCmdText
	Cmd.Parameters.Append Cmd.CreateParameter("RegNumber", 200, 1, 255, RegNumber)
	Set rsRegID = Cmd.Execute
	
	if not (rsRegID.BOF or rsRegID.EOF) then
		RegID = rsRegID("reg_id")
		GetRegID = RegID
	else
		GetRegID = null
	end if
End Function

Function GetRegID1(RegNumber, user_id, user_pwd)
	If (Application("RegServerName") = "NULL")then
		Response.Write "<center><BR><BR><BR><BR><span class=""GUIFeedback"">You have insufficient Privileges to view Registration Database<BR>Contact your system administrator.</span></center>"
		Response.end
	End if
	
	if CBool(Application("UseNotebookTable")) then
		notebookSQL = " (SELECT notebook_name FROM notebooks WHERE notebook_number = batches.notebook_internal_id) AS NoteBook," 
	else
		notebookSQL = " batches.notebook_text AS NoteBook,"
	end if
	strSQL = "SELECT" &_
			 " reg_id " &_
			 " FROM reg_numbers" &_
			 " WHERE reg_numbers.reg_number =?" 

	connection_array = Application("base_connection" & "invreg")
	'Cannot use the oledb connection because it truncates the Base64_cdx Long field
	'ConnStr =  "dsn=chem_reg;UID=" & UserName & ";PWD=" & UserID
	'Now that reg uses clobs we can use the udl
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & user_id & ";" & Application("PWDKeyword") & "=" & user_pwd
	
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr 

	'GetRegConnection()
	Set Cmd = GetCommand(Conn, strSQL, &H0001) 'adCmdText
	Cmd.Parameters.Append Cmd.CreateParameter("RegNumber", 200, 1, 255, RegNumber)
	Set rsRegID = Cmd.Execute
	
	if not (rsRegID.BOF or rsRegID.EOF) then
		RegID = rsRegID("reg_id")
		GetRegID = RegID
	else
		GetRegID = null
	end if
End Function
</SCRIPT>

