<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/xml_utils.asp"-->
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

'CSBR 135264 SJ For registering compounds when RLS is at Batch Level
Sub AddToList(byRef RegDoc, byRef listParent, byVal elementNames, byVal listValue, byVal bIsUpdate)
' RegDoc is the reg XML, listParent is the parent node of the list parameter, e.g. the PropertyList node,
' elementNames is a comma-separated list of the names elements under the node, e.g. "Project,ProjectID"
' listValue is the actual value being added to the bottom element, e.g. the project ID.
    Dim arrElementNames
    Dim topElementName
    Dim elementName
    Dim valuePath
    Dim valueNode
    Dim child
    Dim parent
    Dim attr

    arrElementNames= split(elementNames,",")
    topElementName = arrElementNames(0)
    valuePath = ""
    for each elementName in arrElementNames
       if valuePath <> "" then
           valuePath = valuePath & "/"
       end if
       valuePath = valuePath & elementName
    next
    valuePath = valuePath & "[.=""" & listValue & """]"
    set valueNode = listParent.selectSingleNode (valuePath)
    if valueNode is nothing then
        set parent = listParent
        for each elementName in arrElementNames
            Set child = RegDoc.CreateElement(elementName)
            parent.AppendChild(child)
            if bIsUpdate then
                child.setAttribute "insert","yes"
            end if
            set parent = child
        next
        child.text = listvalue
    elseif bIsUpdate then
        set child = valueNode
        for each elementName in arrElementNames
            child.removeAttribute("delete")
            Set child = child.parentNode
        next
    end if     
End Sub

Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim bXMLDebug
Dim HTTPTimeout
'set trace level to 15 or 20 to get the traces here
bDebugPrint = False
bXMLDebug = False
bWriteError = False
strError = "Error:CreatePlateXML<BR>"
RegServerName = Application("RegServerName")
InvServerName = Application("InvServerName")

Set oPlateXML = Server.CreateObject("MSXML2.DOMDocument.6.0")
oPlateXML.load(request)
If bXMLDebug then oPlateXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug01_CreatePlateXML.xml")) 
'If bXMLDebug then oPlateXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug" & replace(replace(time,":","")," ","") & "_CreatePlateXML.xml"))
'Response.End

'always decrement parent quantitites
bDecrementParents = true
' Redirect to help page if no xmldoc is passed
If Len(oPlateXML.xml) = 0 then
	Response.Redirect "/cheminv/help/admin/api/CreatePlateXML.htm"
	Response.end
End if

'get api variables from the xmldoc
Set oPlatesElement = oPlateXML.documentElement
With oPlatesElement
	RegisterCompounds = lcase(.getAttribute("REGISTERCOMPOUNDS"))
	RegUser = .getAttribute("REGUSER")
	RegPwd = .getAttribute("REGPWD")
	CSUserName = .getAttribute("CSUSERNAME")
	CSUserID = .getAttribute("CSUSERID")
    HTTPTimeout = .getAttribute("HTTPTIMEOUT")
	'CSBR ID     : 128893
    'Modified by : sjacob
    'Comments    : Adding code to authenticate LDAP users.
    'Start of change
	IsLDAPUser = IsValidLDAPUser(CSUserName, CSUserID) 
	If IsLDAPUser = 1 Then
	    CSUserID = GeneratePwd(CSUserName)
	End If   
	'End of change
End With
If Not IsNull(HTTPTimeout) Then
	Server.ScriptTimeout = HTTPTimeout
End If
If RegisterCompounds then
    sURL = Server.MapPath("\cheminv\config\invloader.xml")
    Set FieldXML = CreateObject("MSXML2.DOMDocument")
    FieldXML.async = False
    FieldXML.load sURL
    Set oFieldXML = CreateObject("MSXML2.DOMDocument")
    oFieldXML.async = False
    oFieldXML.load sURL
    If Len(FieldXML.xml) = 0 then
        Response.Write "Error: Could not load invloader.xml<BR>"
        Response.end
    End if
End If

' Check for required parameters
If IsEmpty(RegisterCompounds) or IsNull(RegisterCompounds)  then
	strError = strError & "RegisterCompounds is a required parameter<BR>"
	bWriteError = True
else
	'RegUser and RegPwd are only req'd if RegisterCompounds = true
	If RegisterCompounds = "true" then
		'register compound credentials are required if register is true
		If IsEmpty(RegUser) or IsNull(RegUser) then
			strError = strError & "RegUser is a required parameter<BR>"
			bWriteError = True
		End if
		If IsEmpty(RegPwd) or IsNull(RegPwd) then
			strError = strError & "RegPwd is a required parameter<BR>"
			bWriteError = True
		End if
	End if
End if
If IsEmpty(CSUserName) or IsNull(CSUserName) then
	strError = strError & "CSUserName is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(CSUserID) or IsNull(CSUserID) then
	strError = strError & "CSUserID is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	Response.Write strError
	Response.End
end if

'Register compounds or add to inv_compounds
oPlateXML.setProperty "SelectionLanguage","XPath"
Set nlCompounds = oPlateXML.selectNodes("//COMPOUND")
'if bDebugPrint then Response.Write nlCompounds.length & "=NumCompounds<BR>"

Set oResultXML = server.CreateObject("MSXML2.DOMDocument")
Set oRootNode = oResultXML.createElement("CREATE_PLATE_RESULTS")
Set oSubstanceResultsNode = oResultXML.createElement("SUBSTANCE_RESULTS")

Set oCmpdResultXML = server.CreateObject("MSXML2.DOMDocument")

if RegisterCompounds = "true" then
    Dim soapClient
    Dim headerHandler
    Dim strXMLTemplate
    Dim xmlReg
    Dim sRLS
    Dim oParentNode
    Dim xmlSend
    xmlSend = ""
    '  Set up SOAP client
    set soapClient = Server.CreateObject("MSSOAP.SoapClient30")
    set headerHandler = Server.CreateObject("COEHeaderServer.COEHeaderHandler")
    headerHandler.UserName = RegUser
    headerHandler.Password = RegPwd
    set soapClient.HeaderHandler = headerHandler
    soapClient.ClientProperty("ServerHTTPRequest") = True
    call soapClient.MSSoapInit(Application("SERVER_TYPE") & RegServerName & "/COERegistration/webservices/COERegistrationServices.asmx?wsdl")
    soapClient.ConnectorProperty("Timeout") = HTTPTimeout * 1000    ' sets timeout to 120 secs
    'CSBR 145527 : SJ : Adding credentials to the httprequest to resolve the issue after applying the security hotfix.  
	Credentials = "&CSUserName=" & Server.URLEncode(CSUserName) & "&CSUserID=" & Server.URLEncode(CSUserID) 'CSBR-156006: SJ
    'Get registration XML template
    strXMLTemplate = soapClient.RetrieveNewRegistryRecord()
    'strXMLTemplate = soapClient.SetModuleName(strXMLTemplate, "INVLOADER") 	'CBOE-1159 method for adding module name in xml template : ASV 10JUL13

    'Create the base registration XML  
    'CSBR 166533 SJ Code change made to handle multiple fragments

    for each actionNode In oPlatesElement.SelectNodes("//COMPOUND")
        set xmlReg = Server.CreateObject("Msxml2.DOMDocument")
        xmlReg.async = false
        xmlReg.loadXML(strXMLTemplate)
        'CSBR ID : 124898 : sjacob
        'Comment : Modifying the xmlRegBase for adding Project node under ProjectList 
        'Start of change 
        Set parentnode = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/ProjectList")
        CreateNode xmlReg, "Project", parentnode, "", "", ""
	    'End of change

        'CSBR 135264 SJ For registering compounds when RLS is at Batch Level
        set NodeRLS = xmlReg.SelectSingleNode("MultiCompoundRegistryRecord")
        sRLS = NodeRLS.getAttribute("ActiveRLS")

        ' RAG
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

        If sRLS = "BatchLevelProjects" Then
            set NodeRLS = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/BatchList/Batch")
            CreateNode xmlReg, "ProjectList", NodeRLS, "", "", ""
        End If
        set oBatch = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/BatchList/Batch")

    'Remove the validation nodes
        Set oSelection = xmlReg.selectNodes("//validationRuleList")
        oSelection.removeAll()
    'Remove the AddIn nodes
        Set oSelection = xmlReg.selectNodes("/MultiCompoundRegistryRecord/AddIns")
        oSelection.removeAll()

    'Transfer oPlatesElement attributes into xmlRegBase document   
        for each actionElem In actionNode.SelectNodes("*")
            Set oParentNode = actionNode.parentNode
            strElemName = UCase(actionElem.nodeName)
            strElemText = actionElem.text     
            ' Process special nodes
            if strElemName = "REGPARAMETER" then
                select case strElemText 
                    case "B","D","N"
                        strDupFlag = strElemText    ' Add New (B)atch, Add New Compound (D)uplicate, Ignore Compound (N)ot store
                    case else
                        Response.write "Illegal DupFlag " & strElemText
                        Response.end
                end select

            elseif strElemName = "STRUCTURE" then
                set node = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/Structure")
                if not (node is Nothing) then
                    if not ((strElemText = "No Structure" or strElemText = "") and bUpdateNonnull) then
                        set node1 = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/StructureID")
                        if strElemText = "No Structure" or strElemText = "" then
                            CreateOrReplaceText xmlReg,node,""
                            CreateOrReplaceText xmlReg,node1,"-2"
                        else
                            CreateOrReplaceText xmlReg,node,strElemText
                            CreateOrReplaceText xmlReg,node1,node1.nodetypedvalue
                        end if
                    end if
                end if
            else
                set oFieldNode = oFieldXML.SelectSingleNode("/invloader/regfields/regfield[@name='" & strElemName & "']")
                if not oFieldNode is nothing then
                    strFieldType = oFieldNode.GetAttribute("type")
                    if strFieldType = "BATCH" then
                        set node = oBatch.SelectSingleNode(oFieldNode.text)
                    else
                        set node = xmlReg.SelectSingleNode(oFieldNode.text)
                    end if
                    if not node is nothing then
                        if node.TagName = "BatchComponentFragment" then 'CSBR 165285 SJ Adding Fragments for plate upload.
                            set node1 = xmlReg.SelectSingleNode("/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList")
                            for each subnode in node1.SelectNodes("*")
                                if subnode.SelectSingleNode("Equivalents") Is Nothing then
                                    set node = subnode  
                                    exit for 
                                end if
                            next
                        end if
                        'SMathur CSBR-167768 - appending the Indenter, InputText to XmlReg
                        ' for Reg identifier
                        If node.tagName = "Identifier" And strFieldType <> "BATCH" Then
                            Set node1 = xmlReg.selectSingleNode("/MultiCompoundRegistryRecord/IdentifierList")
                            For Each subnode In node1.selectNodes("*")
                                If subnode.selectSingleNode("InputText") Is Nothing Then
                                    Set node = subnode
                                    Exit For
                                End If
                            Next
                        End If
                        If node.tagName = "Identifier" And strFieldType = "BATCH" Then  
                               Set node1 = xmlReg.selectSingleNode("/MultiCompoundRegistryRecord/BatchList/Batch/IdentifierList")
                               For Each subnode In node1.selectNodes("*")
                                   If subnode.selectSingleNode("Batch_InputText") Is Nothing Then
                                      Set node = subnode
                                      Exit For
                                   End If
                                Next
                        End If
                        ' for Structure identifier
                        If node.tagName = "Identifier" And strFieldType = "COMPOUND" Then 
                               Set node1 = xmlReg.selectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/IdentifierList")
                               For Each subnode In node1.selectNodes("*")
                                   If subnode.selectSingleNode("Structure_InputText") Is Nothing Then
                                     Set node = subnode
                                     Exit For
                                  End If
                                Next
                        End If
                        
                          ' for Component identifier
                           If node.tagName = "Identifier" And strFieldType = "COMPOUND" Then
                               Set node1 = xmlReg.selectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/IdentifierList")
                               For Each subnode In node1.selectNodes("*")
                                   If subnode.selectSingleNode("Component_InputText") Is Nothing Then
                                     Set node = subnode
                                     Exit For
                                  End If
                                Next
                          End If
                          
                        strFieldListElement = oFieldNode.GetAttribute("listelement")
                        if IsBlank(strFieldListElement) then ' Simple attribute
                            if not IsBlank(strElemText) then
                                CreateOrReplaceText  xmlReg,node,strElemText
                            end if
                        else ' List attribute
                            if not IsBlank(strElemText) then
                                AddToList xmlReg,node,strFieldListElement,strElemText,bUpdateReg
                            end if
                        end if
                    end if
                end if
            end if
       next   
       'SMathur CSBR-167768 -Add order index for  Identifier for reg, batch and Fragment level
       Set node1 = xmlReg.selectSingleNode("/MultiCompoundRegistryRecord/IdentifierList")
       count = 1
       For Each subnode In node1.selectNodes("Identifier")
            Set obj1 = xmlReg.createElement("OrderIndex")
            obj1.text = count
            subnode.appendChild obj1
            count = count + 1
       Next

      ' add order index for batch identifier
        Set node1 = xmlReg.selectSingleNode("/MultiCompoundRegistryRecord/BatchList/Batch/IdentifierList")
        count = 1
      
        For Each subnode In node1.selectNodes("Identifier")
            Set obj1 = xmlReg.createElement("OrderIndex")
            obj1.text = count
            subnode.appendChild obj1
            count = count + 1

        Next
        ' for Structure identifier
            Set node1 = xmlReg.selectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/IdentifierList")
            count = 1
            For Each subnode In node1.selectNodes("Identifier")
                Set obj1 = xmlReg.createElement("OrderIndex")
                obj1.text = count
                subnode.appendChild obj1
                count = count + 1
            Next

          ' for Component identifier
            Set node1 = xmlReg.selectSingleNode("/MultiCompoundRegistryRecord/ComponentList/Component/Compound/IdentifierList")
            count = 1
            For Each subnode In node1.selectNodes("Identifier")
                Set obj1 = xmlReg.createElement("OrderIndex")
                obj1.text = count
                subnode.appendChild obj1
                count = count + 1
            Next
            
        'replace custom tags for batch and fragment level
        xmlReg.loadXML (Replace(xmlReg.xml, "Batch_InputText", "InputText"))
        xmlReg.loadXML (Replace(xmlReg.xml, "Structure_InputText", "InputText"))
        xmlReg.loadXML (Replace(xmlReg.xml, "Component_InputText", "InputText"))
        
       If Len(xmlSend) = 0 Then
            xmlSend = "<MultiCompoundRegistryRecordList>" & xmlReg.xml
       Else
            xmlSend = xmlSend & xmlReg.xml
       End If
	Next
'Send the reg document off to be registered
    If Len(xmlSend) > 0 Then
       xmlSend = xmlSend & "</MultiCompoundRegistryRecordList>"
       on error resume next
       strXMLResponse = soapClient.CreateRegistryRecordBulk(xmlSend,strDupFlag)
    End If
   	
    If (Len(strXMLResponse) > 0 or err.number > 0)  Then
        'Process responseXML to find out what happened
        Dim xmlResponse
        Dim Count 
        Count = 1
        Dim TempNode
        Dim RedBoxWarning 'CBOE-1159 veriable to store chemical warnings : ASV 10JUL13
        set xmlResponse = Server.CreateObject("Msxml2.DOMDocument")
        xmlResponse.async = false
        xmlResponse.loadXML(strXMLResponse)
     
        for each actionNode In oPlatesElement.SelectNodes("//WELL")
            If (actionNode.childNodes.length > 0) Then    
		        set oRCNode = oResultXML.createElement("REG_SUBSTANCE")
                set node = xmlResponse.selectSingleNode("//ReturnList/ReturnListItem[@itemIndex=" & Count & "]/ActionDuplicateTaken")
                action = node.text
                set node = xmlResponse.selectSingleNode("//ReturnList/ReturnListItem[@itemIndex=" & Count & "]/RegID")
                regID = node.text
                set node = xmlResponse.selectSingleNode("//ReturnList/ReturnListItem[@itemIndex=" & Count & "]/BatchNumber")
                batchNumber = node.text
                'CBOE-1159 Check for chemical Warning : ASV 10JUL13
                set node = xmlResponse.selectSingleNode("//ReturnList/ReturnListItem[@itemIndex=" & Count & "]/RedBoxWarning")
                RedBoxWarning = null
                if not (node is Nothing) then
                    RedBoxWarning = node.text
                End if
                Select Case ucase(action)
                    'Not a duplicate, new compound and batch created
                    Case "C"
			            bRegistrationFailed = false			    
                        AddMessageToNode oRCNode, oResultXML,"New compound registered, RegID=" & regID & ", BatchNumber=" & batchNumber 
                    'Batch created
                    Case "B"
			            bRegistrationFailed = false			    
                        AddMessageToNode oRCNode, oResultXML,"New batch created, RegID=" & regID & ", BatchNumber=" & batchNumber 
                    'Temporary 
                    Case "T"
			            bRegistrationFailed = false         'kfd, is this correct?			    
                        AddMessageToNode oRCNode, oResultXML,"Temporary compound created, RegID=" & regID & ", BatchNumber=" & batchNumber 
                    'Duplicate compound
                    Case "D"
			            bRegistrationFailed = false			    
                        AddMessageToNode oRCNode, oResultXML,"Duplicate compound registered, RegID=" & regID & ", BatchNumber=" & batchNumber 
					Case "N" 'CSBR 167928 SJ Ignore Compound 
			            bRegistrationFailed = false
                        If regID > 0 Then			    
                            AddMessageToNode oRCNode, oResultXML,"New compound registered, RegID=" & regID & ", BatchNumber=" & batchNumber 
                        Else
                            AddMessageToNode oRCNode, oResultXML,"Unknown RegisterCompound response, RegID=" & regID & ", BatchNumber=" & batchNumber 
                        End If	
                    'Unknown
                    Case else
			            bRegistrationFailed = false			    
                        regID = ""
                        batchNumber = ""
				        bRegistrationFailed = true				
                        AddErrorToNode oRCNode, oResultXML, "Unknown RegisterCompound response: " & strXMLResponse
                End Select
				
				'CBOE-1159 if chemical Warning exists add it as error node: ASV 10JUL13
                if not (RedBoxWarning is Nothing) then
                   AddErrorToNode oRCNode, oResultXML, RedBoxWarning
                End if

		        if Len(regID) > 0 then
		            Set oRegIDNode = oResultXML.createAttribute("REG_ID_FK")
		            oRegIDNode.Value = regID
		            oRCNode.SetAttributeNode(oRegIDNode)
		        end if
		
		        if Len(batchNumber) > 0 then
		            Set oBatchIDNode = oResultXML.createAttribute("BATCH_NUMBER_FK")
		            oBatchIDNode.Value = batchNumber
		            oRCNode.SetAttributeNode(oBatchIDNode)
		        end if
			     ' CSBR-168047 : Smathur - Adding details in the logfile
                if  (err.number>0 or bRegistrationFailed or (regID =0)) then
                    set Inputvalues = oResultXML.createElement("INPUTVALUES")
                      For x = 0 To (oPlatesElement.Attributes.length - 1)
                        sAttrName = oPlatesElement.Attributes.Item(x).nodeName  
                        sAttrVal=oPlatesElement.Attributes.Item(x).nodeValue
                        if sAttrVal <> "" and ucase(sAttrName) <> "HTTPTIMEOUT" and ucase(sAttrName) <> "REGUSER" and ucase(sAttrName) <> "REGPWD" and ucase(sAttrName) <> "CSUSERID" and ucase(sAttrName) <> "CSUSERNAME" then
                             Set oAttribute = oResultXML.createAttribute(sAttrName)
		                     oAttribute.Value = sAttrVal
		                     Inputvalues.SetAttributeNode(oAttribute)
                        end if 
                      next 
                      for each COMPOUNDNode in actionnode.SelectNodes("COMPOUND/*")
                        sAttrName = UCase(COMPOUNDNode.nodeName)
                        sAttrVal = COMPOUNDNode.text
                        if sAttrVal <>""  and ucase(sAttrName) <> "STRUCTURE"  then
                             Set oAttribute = oResultXML.createAttribute(sAttrName)
		                     oAttribute.Value = sAttrVal
		                     Inputvalues.SetAttributeNode(oAttribute)
                        end if               
                     next
                     For x = 0 To (actionnode.Attributes.length - 1)
                        sAttrName = actionnode.Attributes.Item(x).nodeName  
                        sAttrVal=actionnode.Attributes.Item(x).nodeValue                  
                        if sAttrVal <>"" then
                             Set oAttribute = oResultXML.createAttribute(sAttrName)
		                     oAttribute.Value = sAttrVal
		                     Inputvalues.SetAttributeNode(oAttribute)
                        end if 
                     next
                     
                      oSubstanceResultsNode.appendChild(Inputvalues)                 
                end if 
			    
		        ' Add to our XML results
		        oSubstanceResultsNode.appendChild(oRCNode)
            
		    
		        Set oRegIDNode = oPlateXML.createAttribute("REG_ID_FK")
                Set TempNode = xmlResponse.selectSingleNode("//ReturnList/ReturnListItem[@itemIndex=" & Count & "]/RegID")
                If Not TempNode Is Nothing Then
                    oRegIDNode.value = TempNode.text
                Else
                    oRegIDNode.value = ""
                End If
		        actionNode.SetAttributeNode(oRegIDNode)
		        Set oBatchNumberNode = oPlateXML.createAttribute("BATCH_NUMBER_FK")
                Set TempNode = xmlResponse.selectSingleNode("//ReturnList/ReturnListItem[@itemIndex=" & Count & "]/BatchNumber")
                If Not TempNode Is Nothing Then
                    oBatchNumberNode.value = TempNode.text
                Else
                    oBatchNumberNode.value = ""
                End If
		        actionNode.SetAttributeNode(oBatchNumberNode)
                Count = Count + 1
            end if
        next
    end if
elseif RegisterCompounds = "false" then
	Credentials = "&CSUserName=" & Server.URLEncode(CSUserName) & "&CSUserID=" & Server.URLEncode(CSUserID) 'CSBR-156006: SJ
	For Each oNode In nlCompounds
		Set oParentNode = oNode.parentNode
		SubstanceName = oNode.GetAttribute("SUBSTANCE_NAME")
		Structure = oNode.GetAttribute("BASE64_CDX")
		CAS = oNode.GetAttribute("CAS")
		ACX_ID = oNode.GetAttribute("ACX_ID")
		Density = oNode.GetAttribute("DENSITY")
		cLogP = oNode.GetAttribute("CLOGP")
		Rotatable_Bonds = oNode.GetAttribute("ROTATABLE_BONDS")
		Tot_Pol_Surf_Area = oNode.GetAttribute("TOT_POL_SURF_AREA")
		HBond_Acceptors = oNode.GetAttribute("HBOND_ACCEPTORS")
		HBond_Donors = oNode.GetAttribute("HBOND_DONORS")
		ALT_ID_1 = oNode.GetAttribute("ALT_ID_1")
		ALT_ID_2 = oNode.GetAttribute("ALT_ID_2")
		ALT_ID_3 = oNode.GetAttribute("ALT_ID_3")
		ALT_ID_4 = oNode.GetAttribute("ALT_ID_4")
		ALT_ID_5 = oNode.GetAttribute("ALT_ID_5")
		'Response.Write Density & ":" & cLogP & ":" & Rotatable_Bonds & ":" & Tot_Pol_Surf_Area & ":" & HBond_Acceptors & ":" & HBond_Donors & "<BR>"		

		QueryString = "inv_compounds.Substance_Name=" & Server.URLEncode(SubstanceName)
		QueryString = QueryString & "&RegisterIfConflicts=true"
		QueryString = QueryString & "&ResolveConflictsLater=1"
		QueryString = QueryString & "&inv_compounds.CAS=" & CAS
		QueryString = QueryString & "&inv_compounds.ACX_ID=" & ACX_ID
		QueryString = QueryString & "&inv_compounds.Density=" & Density
		QueryString = QueryString & "&inv_compounds.cLogP=" & cLogP
		QueryString = QueryString & "&inv_compounds.Rotatable_Bonds=" & Rotatable_Bonds
		QueryString = QueryString & "&inv_compounds.Tot_Pol_Surf_Area=" & Tot_Pol_Surf_Area
		QueryString = QueryString & "&inv_compounds.HBond_Acceptors=" & HBond_Acceptors
		QueryString = QueryString & "&inv_compounds.HBond_Donors=" & HBond_Donors								
		QueryString = QueryString & "&inv_compounds.ALT_ID_1=" & ALT_ID_1
		QueryString = QueryString & "&inv_compounds.ALT_ID_2=" & ALT_ID_2
		QueryString = QueryString & "&inv_compounds.ALT_ID_3=" & ALT_ID_3
		QueryString = QueryString & "&inv_compounds.ALT_ID_4=" & ALT_ID_4
		QueryString = QueryString & "&inv_compounds.ALT_ID_5=" & ALT_ID_5
		'QueryString = QueryString & "&inv_compounds.Structure=" & Server.URLEncode(Structure)
		QueryString = QueryString & "&inv_compounds.Structure=" & Structure
		QueryString = QueryString & Credentials
		trace QueryString, 20
		if bDebugPrint then 
			Response.Write QueryString & "<BR>"
			Response.End
		end if
		httpResponse = CShttpRequest2("POST", InvServerName, "/cheminv/api/CreateSubstance.asp", "ChemInv", QueryString)
		out2 = httpResponse
		'Response.Write out2 & "=CreateSubstance Return Value<BR>" 
		trace out2 & "=CreateSubstance Return Value", 15
		oCmpdResultXML.loadXML(out2)
		oCmpdResultXML.setProperty "SelectionLanguage","XPath"
		'substance already exists
		Set nlExistingSubstances = oCmpdResultXML.selectNodes("//EXISTINGSUBSTANCE")
		if nlExistingSubstances.Length > 0 then Set nlToCheck = nlExistingSubstances
		'substance created
		Set nlNewSubstances = oCmpdResultXML.selectNodes("//NEWSUBSTANCE")
		if nlNewSubstances.Length > 0 then Set nlToCheck = nlNewSubstances
		'duplicate substance
		Set nlDuplicateSubstances = oCmpdResultXML.selectNodes("//DUPLICATESUBSTANCE")
		if nlDuplicateSubstances.Length > 0 then Set nlToCheck = nlDuplicateSubstances
		'conflicting substances
		Set nlConflictingSubstances = oCmpdResultXML.selectNodes("//CONFLICTINGSUBSTANCES")
		if nlConflictingSubstances.Length > 0 then Set nlToCheck = oCmpdResultXML.selectNodes("//CONFLICTINGSUBSTANCES/CONFLICT")
		'RCNode means Registered Compound Node
		if isObject(nlToCheck) then
			For Each oRCNode in nlToCheck
				oSubstanceResultsNode.appendChild(oRCNode)				
				CompoundID = oRCNode.GetAttribute("CompoundID")
			Next
		end if
		Set oCompoundIDNode = oPlateXML.createAttribute("COMPOUND_ID_FK")
		oCompoundIDNode.Value = CompoundID
		oParentNode.SetAttributeNode(oCompoundIDNode)
	Next
end if

'check for solvent name if found then do a lookup and add if its not there
Set nlSolventNodes = oPlateXML.selectNodes("//*[string-length(@SOLVENT)>0]")
Set oSolventDict = server.CreateObject("scripting.dictionary")
For Each oNode In nlSolventNodes
	solventName = oNode.selectSingleNode("@SOLVENT").nodeValue
	If not oSolventDict.Exists(solventName) then
		QueryString = "TableName=inv_solvents"
		QueryString = QueryString & "&TableValue=" & Server.URLEncode(SolventName)
		QueryString = QueryString & "&InsertIfNotFound=true"
		'CSBR 145527 : SJ : Adding credentials to the httprequest to resolve the issue after applying the security hotfix.  
		QueryString = QueryString & Credentials
		httpResponse = CShttpRequest2("POST", InvServerName, "/cheminv/api/LookUpValue.asp", "ChemInv", QueryString)
		if instr(httpResponse,"Error")>0 then
			solventID = ""
		else
			solventID = httpResponse
			oSolventDict.Add solventName,solventID
		end if
	else
		solventID = oSolventDict.Item(solventName)	
	end if
	oNode.removeAttribute("SOLVENT")
	Set oSolventIDNode = oPlateXML.createAttribute("SOLVENT_ID_FK")
	oSolventIDNode.Value = solventID
	oNode.SetAttributeNode(oSolventIDNode)
Next

'check for solvent volume unit id (removed because Invloader 11.0.3 handles units ID conversions)
 Set nlSolventUnitNodes = oPlateXML.selectNodes("//*[string-length(@SOLVENT_VOLUME_UNIT_NAME)>0]")
Set oSolventUnitDict = server.CreateObject("scripting.dictionary")
For Each oNode In nlSolventUnitNodes
	solventUnitName = oNode.selectSingleNode("@SOLVENT_VOLUME_UNIT_NAME").nodeValue
	If not oSolventUnitDict.Exists(solventUnitName) then
		QueryString = "TableName=inv_units"
		QueryString = QueryString & "&TableValue=" & Server.URLEncode(solventUnitName)
		QueryString = QueryString & "&InsertIfNotFound=false"
		'CSBR 145527 : SJ : Adding credentials to the httprequest to resolve the issue after applying the security hotfix.  
		QueryString = QueryString & Credentials
		httpResponse = CShttpRequest2("POST", InvServerName, "/cheminv/api/LookUpValue.asp", "ChemInv", QueryString)
		if (instr(httpResponse,"Error") > 0 or instr(httpResponse,"NOT FOUND") > 0) then
			solventUnitID = ""
		else
			solventUnitID = httpResponse
			if Len(solventUnitID) > 0 then
			    oSolventUnitDict.Add solventUnitName,solventUnitID
			end if
		end if
	else
		solventUnitID = oSolventUnitDict.Item(solventUnitName)	
	end if
	oNode.removeAttribute("SOLVENT_VOLUME_UNIT_NAME")
	Set oSolventVolUnitIDNode = oPlateXML.createAttribute("SOLVENT_VOLUME_UNIT_ID_FK")
	oSolventVolUnitIDNode.Value = solventUnitID
	oNode.SetAttributeNode(oSolventVolUnitIDNode)
Next

If bXMLDebug then oPlateXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug02_CreatePlateXML.xml")) 
' Set up an ADO command
' can't use GetInvCommand b/c it calls GetInvConnection which references the Request object
connection_array = Application("base_connection" & "cheminv")
ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & CSUserName & ";" & Application("PWDKeyword") & "=" & CSUserID
Set Conn = Server.CreateObject("ADODB.Connection")
Conn.Open ConnStr 
Set Cmd = Server.CreateObject("ADODB.Command")
Cmd.ActiveConnection = Conn
Cmd.CommandType = adCmdStoredProc
Cmd.CommandText = Application("CHEMINV_USERNAME") & ".CreatePlateXML"

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",AdVarChar, adParamReturnValue, 16384, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pPlateXML", AdLongVarChar, adParamInput, len(oPlateXML.xml) + 1, oPlateXML.xml)
Cmd.Properties("SPPrmsLOB") = TRUE
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	'Conn.Execute("Alter session set sql_trace = true")
 on error resume next
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CreatePlateXML")
	'on error resume next
	'Cmd.Execute
	'Response.Write err.Description & ":" & err.number
	'Conn.Execute("Alter session set sql_trace = false")
End if
oRootNode.appendChild(oSubstanceResultsNode)

' Handle some known errors
bError = false
strReturnValue = trim(Cmd.Parameters("RETURN_VALUE"))
if InStr( lcase(strReturnValue), "unique constraint" ) > 0 and InStr( UCase(strReturnValue), "INV_PLATES_BARCODE_U" ) then  
    bError = true
    strError = "Plate barcode '" & GetPlateAttribute("PLATE_BARCODE") & "' already exists in Inventory"
else
    Set oPlateIDNode = oResultXML.createAttribute("PLATE_ID")
    oPlateIDNode.Value = trim(Cmd.Parameters("RETURN_VALUE"))
    oRootNode.SetAttributeNode(oPlateIDNode)
    if len(oPlateIDNode.Value) > 50 then
        set Inputvalues = oResultXML.createElement("PLATEERROR")
        set eRrorNode = oResultXML.createAttribute("Error")
	    eRrorNode.Value = "Error on Plate Creation."
	    Inputvalues.SetAttributeNode(eRrorNode)
        oSubstanceResultsNode.appendChild(Inputvalues)      
    end if 
end if

if bError then
    Set oErrorIDNode = oResultXML.createAttribute("ERROR")
    oErrorIDNode.Value = strError
    oRootNode.SetAttributeNode(oErrorIDNode)    
end if

oResultXML.appendChild(oRootNode)

'Response.Write trim(Cmd.Parameters("RETURN_VALUE"))
'Return an XML doc with the plateIDs and substance registration info
'Response.Write Cmd.Parameters("RETURN_VALUE")
Cmd.Properties("SPPrmsLOB") = FALSE
Response.Write oResultXML.xml 
If bXMLDebug then oResultXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug03_CreatePlateXML.xml")) 
'Clean up
Conn.Close
Set oPlateXML = nothing
Set oResultXML = nothing
set oCmpdResultXML = nothing
Set nlCompounds = nothing
Set nlExistingSubstances = nothing
Set nlNewSubstances = nothing
Set nlDuplicateSubstances = nothing
Set nlToCheck = nothing
Set Conn = Nothing
Set Cmd = Nothing
Set xmlReg = nothing

    set soapClient = nothing
    set headerHandler = nothing
    set strXMLTemplate = nothing
    set xmlReg = nothing
    set sRLS = nothing
    set oParentNode = nothing

Response.End

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


'DJP: Added 10/7/2004 for IM8SR2
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
	Conn.close
	if not (rsRegID.BOF or rsRegID.EOF) then
		RegID = rsRegID("reg_id")
		GetRegID = RegID
	else
		GetRegID = null
	end if
End Function

Function AttribEncode(ByRef oNode, attribName)
	AttribEncode = Server.URLEncode(oNode.GetAttribute(attribName))
End Function

Function GetPlateAttribute(strAttribute)
    Dim strValue
    strValue = ""
    Set oPlateNode = oPlateXML.SelectSingleNode("descendant::PLATE")
    if not oPlateNode is nothing then
        set oAttributes = oPlateNode.Attributes
        set oAttrib = oAttributes.GetNamedItem(strAttribute)
        if not oAttrib is nothing then            
            strValue = oAttrib.Value
        end if
    end if
    GetPlateAttribute = strValue
end Function

Sub AddSubstanceAttribute( ByRef oNode, strAttribute, ByRef oOutputXML, ByRef oOutputNode )
    Dim oAttributeNode
    if Len(oNode.GetAttribute(strAttribute)) > 0 then
        Set oAttributeNode = oOutputXML.createAttribute(strAttribute)
        oAttributeNode.Value = oNode.GetAttribute(strAttribute)
        oOutputNode.SetAttributeNode(oAttributeNode)
    end if
end sub

Sub AddErrorToNode( byRef oNode, byRef oOutputXML, strError )
    Dim oErrorNode
    Set oErrorNode = oOutputXML.createAttribute("ERROR")
    oErrorNode.Value = strError
    oNode.SetAttributeNode(oErrorNode)

end sub

Sub AddMessageToNode( byRef oNode, byRef oOutputXML, strError )
    Dim oErrorNode
    Set oErrorNode = oOutputXML.createAttribute("MESSAGE")
    oErrorNode.Value = strError
    oNode.SetAttributeNode(oErrorNode)
end sub

</SCRIPT>
