<%@ EnableSessionState=False Language=VBScript%>
<%Response.Charset="UTF-8"%>
<Script RUNAT="Server" Language="VbScript">
Response.Expires = -1

UserPreferencesFieldsStr = Application("UserPreferencesFieldsStr")
FieldsArray = split(UserPreferencesFieldsStr,";")

Set XMLContainer = Server.CreateObject("Msxml2.DOMDocument")
XMLContainer.async = false
With XMLContainer
	Set oRootNode = .createElement("inventory")
	Set .documentElement = oRootNode
	With oRootNode
		Set oNode = XMLContainer.createElement("containerfields")
		.appendChild oNode
	End With
end with
With oNode
	for each item in FieldsArray
		FieldArray = split(Application(item), ":")
		Set oNode1 = XMLContainer.createElement("field")
		oNode1.setAttribute "name", trim(FieldArray(2))
        oNode1.setAttribute "DisplayName", trim(FieldArray(3))
		oNode1.setAttribute "required", trim(FieldArray(1))
		.appendChild oNode1
	Next
End With
Response.ContentType = "Text/Plain"
Response.Write XMLContainer.xml
Response.End	
</SCRIPT>
		