Const ForReading = 1

Dim sFile, sRoot, sKey, sValue
Dim sTemp
Dim sArray

Dim oXMLdoc
Dim oNode, Node, oRNode, oRootNode, oRootNode1, oRootNode2, Node2, Node1, oRootNode3, nFlag, n1Flag
Dim oRootNodeList
dim filesys
Set filesys = CreateObject("Scripting.FileSystemObject")
'sAddress=Session.Property("SADDRESS")
sFile = "D:\CBVN110X_Deployment\ChemBioViz.Net.application"

If filesys.FileExists(sFile) Then
'Prepare XML parser object
Set oXMLdoc = CreateObject("Microsoft.XMLDOM" )

'Load the document
oXMLdoc.Load sFile

'Get the childnode list for the sRoot node
Set oRNode = oXMLdoc.selectNodes("asmv1:assembly")

For Each Node11 in oRNode
Set oRootNode11 = Node11.selectSingleNode("deployment")
'MsgBox oRootNode11.Attributes.getNamedItem("trustURLParameters").Text

'Set oRootNodeList11 = oRootNode11.childNodes
Next     
Dim objAttrib
    
 Set objAttrib = oXMLdoc.createAttribute("mapFileExtensions")
 objAttrib.Text ="false"
 oRootNode11.Attributes.setNamedItem objAttrib
 oXMLdoc.documentElement.appendChild oRootNode11

oXMLdoc.save sFile
End If