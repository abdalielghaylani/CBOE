<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/xml_utils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim RS
Dim Conn
Dim Cmd
'MCD: store project and job
WriteUserProperty Session("UserNameCheminv"), "Project", Request("Project")
WriteUserProperty Session("UserNameCheminv"), "Job", Request("Job")
'MCD: end changes
'DumpRequest()
'Response.End
LocationID = Request("LocationID")
PackageIDList = Request("PackageIDList")
PackageID_arr = split(PackageIDList, ",")

'showformvars true

QtyList= Request("QtyList")
Qty_arr = split(QtyList, ",")
UOMIDList= Request("UOMIDList")
UOMID_arr = split(UOMIDList, ",")
PriceList= Request("PriceList")
Price_arr = split(PriceList, ",")
ContainerTypeName = Request("ContainerTypeName")
ContainerTypeName_arr = split(ContainerTypeName, ",")
ContainerTypeIDList= Request("ContainerTypeIDList")
ContainerTypeID_arr = split(ContainerTypeIDList, ",")
ContainerSizeList= Request("ContainerSizeList")
ContainerSize_arr = split(ContainerSizeList, ",")
ContainerCountList= Request("ContainerCountList")
ContainerCount_arr = split(ContainerCountList, ",")
DunsList=Request("DunsList")
DunsList_arr=split(DunsList,",")
UNSPSCList=Request("UNSPSCList")
UNSPSCList_arr=split(UNSPSCList,",")
CategoryList=Request("CategoryList")
CategoryList_arr=split(CategoryList,",")


'MCD: replaced InitailAmountList with ContainerSizeList since all containers are assumed to be full
InitialAmountList= Request("ContainerSizeList")
InitialAmount_arr = split(InitialAmountList, ",")
UOCostIDList= Request("UOCostIDList")
UOCostID_arr = split(UOCostIDList, ",")

' Iproc Integration
sql = "select cheminvdb2.getCartSeq() seq_num from dual"
Call GetInvCommand(sql, 1)
Set RS = Cmd.Execute
if not rs.eof then
cartId=rs("seq_num")
end if
' End Iproc integration

'response.write cartid & "*"
'response.end

'Response.Write Price_arr(1)
'Response.Write ContainerSizeList
'Response.Write QtyList
'Response.End

Field_1_list = Request("Field_1_List")   
Field_1_arr    = split(Field_1_List, ",")
Field_2_list = Request("Field_2_List")   
Field_2_arr    = split(Field_2_List, ",")
Field_3_list = Request("Field_3_List")   
Field_3_arr    = split(Field_3_List, ",")
Field_4_list = Request("Field_4_List")   
Field_4_arr    = split(Field_4_List, ",")
Field_5_list = Request("Field_5_List")   
Field_5_arr    = split(Field_5_List, ",")

'MCD: extra ordering fields
DeliveryLocationID = Request("DeliveryLocationID")
if Application("ORDER_WORKFLOW_OPTION")=2 then LocationID=DeliveryLocationID
Project = Request("Project")
Job = Request("Job")
DueDate = Request("DueDate")
RushOrder = Request("RushOrder")
ContainerStatusID = Request("ContainerStatusID")
OwnerID = Request("OwnerID")
OrderReason = Request("OrderReason")
OrderReasonOther = Request("OrderReasonOther")
OrderedByID = Ucase(Session("UserIDChemInv"))
'MCD: end changes
if Request("sid")<> "" then
	sid = Request("sid")
else 
	sid = Session.sessionid	
end if
SessionDir = Application("TempFileDirectory" & "ChemInv") & "Sessiondir"  & "\" & sid
ActionBatchFile = SessionDir & "\InvActionBatch.xml" 
ServerName = Application("ACXServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = "product=1&package=1&Synonym=1&structType=base64cdx&fieldName=PackageID&valueList=" & PackageIDList & credentials
' Use XMLHTTP to post against the ChemACX to get ACXXML
Set oXMLHTTP = GetXMLHTTP("POST", ServerName, "chemacx/api/getXMLdata.asp", "ChemACX", FormData)
'Response.Write oXMLHTTP.ResponseText

' Store the ACXxml returned by the server in a DOM object
Set oACXxml = oXMLHTTP.responseXML

'Check for parsing errors
If oACXxml.parseError.errorCode <> 0 Then
  Response.Write "An MSXML parser error occurred while parsing ACXXML" & "<BR>"
  Response.Write oACXxml.parseError.reason & "<BR>"
  Response.Write "Text being parsed: " & oACXxml.parseError.srcText & "<BR>" 
  Response.end
End If
oACXxml.setProperty "SelectionLanguage", "XPath"

'Response.Write oACXxml.xml
'Response.end
'on error resume next
'  Incorporate Posted data into ACXXML document
For i = 0 to Ubound(PackageID_arr)
' Iproc Integration
sql = "select cheminvdb2.getCartSeq() seq_num from dual"
Call GetInvCommand(sql, 1)
Set RS = Cmd.Execute
if not rs.eof then
reqId=rs("seq_num")
end if
' End Iproc integration
	strXPath = "descendant::package[attribute::ACXpackID=""" & Trim(PackageID_arr(i)) & """]"
	Set PackageNode = oACXxml.SelectSingleNode(strXPath)
	PackageNode.SelectSingleNode("@ContainerUOMID").text= UOMID_arr(i)
	PackageNode.SelectSingleNode("@ContainerSize").text= ContainerSize_arr(i)
	Call AddAttributeToElement(oACXxml, PackageNode, "initialQty", InitialAmount_arr(i))
	Call AddAttributeToElement(oACXxml, PackageNode, "QtyRemaining", InitialAmount_arr(i))
	Call AddAttributeToElement(oACXxml, PackageNode, "locationID", LocationID)
	Call AddAttributeToElement(oACXxml, PackageNode, "containerTypeID", ContainerTypeID_arr(i))
	Call AddAttributeToElement(oACXxml, PackageNode, "containerTypeName", ContainerTypeName_arr(i))
	Call AddAttributeToElement(oACXxml, PackageNode, "containerStatusID", ContainerStatusID)
	'Call AddAttributeToElement(oACXxml, PackageNode, "NumCopies", Qty_arr(i), ContainerCount_arr(i))
	'Call AddAttributeToElement(oACXxml, PackageNode, "NumCopies", ContainerCount_arr(i))
	Call AddAttributeToElement(oACXxml, PackageNode, "NumCopies", ContainerCount_arr(i)*Qty_arr(i))
	'MCD: extra ordering fields
	Call AddAttributeToElement(oACXxml, PackageNode, "DeliveryLocationID", DeliveryLocationID)
	Call AddAttributeToElement(oACXxml, PackageNode, "ProjectNo", Project)
	Call AddAttributeToElement(oACXxml, PackageNode, "JobNo", Job)
	Call AddAttributeToElement(oACXxml, PackageNode, "DueDate", DueDate)
	Call AddAttributeToElement(oACXxml, PackageNode, "IsRushOrder", RushOrder)
	Call AddAttributeToElement(oACXxml, PackageNode, "OrderedByID", OrderedByID)
	Call AddAttributeToElement(oACXxml, PackageNode, "OwnerID", OwnerID)
	Call AddAttributeToElement(oACXxml, PackageNode, "OrderReasonID", OrderReason)
	Call AddAttributeToElement(oACXxml, PackageNode, "OrderReasonIfOtherText", OrderReasonOther)
	Call AddAttributeToElement(oACXxml, PackageNode, "QtyRemaining", InitialAmount_arr(i))
	'DJP: allow edit of price
	Set currNode = oACXxml.selectSingleNode("//package[@ACXpackID="& Trim(PackageID_arr(i)) & "]/packPrice")
	currNode.text = Price_arr(i)
	'Response.Write Price_arr(i)
	'MCD: end changes
	'Custom Fields
	' Iproc integration
	Call AddAttributeToElement(oACXxml, PackageNode, "duns", DunsList_arr(i))
	Call AddAttributeToElement(oACXxml, PackageNode, "unspsc", UNSPSCList_arr(i))
	Call AddAttributeToElement(oACXxml, PackageNode, "reqnumber", reqId)
	Call AddAttributeToElement(oACXxml, PackageNode, "ponumber", CartId)
	Call AddAttributeToElement(oACXxml, PackageNode, "pkgnum", Qty_arr(i))
	Call AddAttributeToElement(oACXxml, PackageNode, "category", CategoryList_arr(i))

	'Iproc integration end	

	if Field_1_List <> "" then
		Call AddAttributeToElement(oACXxml, PackageNode, "Field_1", Field_1_arr(i))
	End if
	if Field_2_List <> "" then
		Call AddAttributeToElement(oACXxml, PackageNode, "Field_2", Field_2_arr(i))
	End if
	if Field_3_List <> "" then
		Call AddAttributeToElement(oACXxml, PackageNode, "Field_3", Field_3_arr(i))
	End if
	if Field_4_List <> "" then
		Call AddAttributeToElement(oACXxml, PackageNode, "Field_4", Field_4_arr(i))
	End if
	'if Field_5_List <> "" then
	'	Call AddAttributeToElement(oACXxml, PackageNode, "Field_5", Field_5_arr(i))
	'End if
Next
Set PackageNode = Nothing

'Response.Write oACXxml.xml
'Response.end


' Create xsl transform tree
dim xslDoc
set xslDoc = Server.CreateObject("Msxml2.DOMDocument")
xslDoc.async = false
xslDoc.load(server.MapPath("/cheminv/gui/ACXxml2ActionBatch.xsl"))

'Response.write xslDoc.xml
'Response.end
' Transform and return in response
'Response.ContentType = "text/xml"
dim oActionBatchXML
Set oActionBatchXML = Server.CreateObject("Msxml2.DOMDocument")
oACXxml.transformNodeToObject xslDoc, oActionBatchXML

'oActionBatchXML.setProperty "SelectionLanguage", "XPath"
'Process the CreateSubstance tags
oActionBatchXML.save(ActionBatchFile)
'Response.write oActionBatchXML.xml
'Response.end
Server.transfer("/cheminv/api/displayactionbatch.asp")
%>
