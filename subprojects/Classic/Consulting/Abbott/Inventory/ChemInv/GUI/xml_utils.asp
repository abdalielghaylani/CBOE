<SCRIPT LANGUAGE=vbscript RUNAT=Server>




'-------------------------------------------------------------------------------
' Purpose: Adds an attribute name-value pair to a given element tag
' Inputs:  parser object, element node object, attribute name, attribute value
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub AddAttributeToElement(byRef theDOM,byRef theElement, byVal sAttrName, byval sAttrValue)
	Dim attr
	if Len(sAttrValue) >0  then
		Set attr = theDOM.CreateAttribute(sAttrName)
		theElement.setAttributeNode(attr)
		theElement.setAttribute sAttrName, sAttrValue
	End if
	Set attr = nothing
End Sub

Sub AttributeToElement(byRef theDOM,byRef theElement, byVal sAttrName, byval sAttrValue)
	Dim attr
	if Len(sAttrValue) >0  then
		Set attr = theDOM.CreateAttribute(sAttrName)
		theElement.setAttributeNode(attr)
		theElement.setAttribute sAttrName, sAttrValue
	End if
	Set attr = nothing
End Sub

Sub	CreateOrReplaceNode(byref theDOM, byval NodeName, byRef parentNode, byval Nodevalue, byval attribName, byval attribValue)
	Set oldChild = parentNode.selectSingleNode("./" & NodeName)
	Set newChild = theDOM.createElement(NodeName)
	newChild.text = NodeValue
	if attribName <> "" then AddAttributeToElement xmlDoc, newChild, attribName, attribValue
	
	if NOT (TypeName(oldChild) = "Nothing") then
		parentNode.replaceChild newChild, oldChild 	
	Else
		parentNode.appendChild newChild
	End if
End sub

Sub	CreateNode(byref theDOM, byval NodeName, byRef parentNode, byval Nodevalue, byval attribName, byval attribValue)
	Set oldChild = parentNode.selectSingleNode("./" & NodeName)
	Set newChild = theDOM.createElement(NodeName)
	newChild.text = NodeValue
	if attribName <> "" then AddAttributeToElement xmlDoc, newChild, attribName, attribValue
	parentNode.appendChild newChild
End sub
</SCRIPT>