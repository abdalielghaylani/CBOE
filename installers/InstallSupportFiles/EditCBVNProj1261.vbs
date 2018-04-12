Const ForReading = 1

Dim sFile, sRoot, sKey, sValue
Dim sTemp
Dim sArray

Dim oXMLdoc
Dim oNode, Node, oRNode, oRootNode, oRootNode1, oRootNode2, Node2, Node1, oRootNode3, nFlag, n1Flag
Dim oRootNodeList
dim filesys, args, arg1, arg2

Set args = WScript.Arguments

sFile = args.Item(0)

arg2 = args.Item(1)

Set filesys = CreateObject("Scripting.FileSystemObject")
      


If filesys.FileExists(sFile) Then
'Prepare XML parser object
Set oXMLdoc = CreateObject("Microsoft.XMLDOM" )

'Load the document
oXMLdoc.Load sFile

'Get the childnode list for the sRoot node
Set oRNode = oXMLdoc.selectNodes("Project")

For Each Node in oRNode
Set oRootNode = Node.selectSingleNode("PropertyGroup")
Set oRootNodeList = oRootNode.childNodes
Next

'For each childnode of sRoot set the value attribute to sValue if the key attribute matches sKey
For Each oNode In oRootNodeList
If oNode.nodeName = "ApplicationRevision" Then
		oNode.Text =arg2
 	End If	
If oNode.nodeName = "ApplicationVersion" Then
		oNode.Text ="17.1.1." + arg2
 	End If
Next

'Save back to the document
oXMLdoc.save sFile
End If