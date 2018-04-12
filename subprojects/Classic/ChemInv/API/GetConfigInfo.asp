<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<SCRIPT RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim oReturnXML

bTimer = False
if bTimer then theStart = timer	
bDebugPrint = False
bWriteError = False
strError = "Error:GetConfigInfo<BR>"


' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetConfigInfo.htm"
	Response.end
End if

ConfigKey = Request("ConfigKey")

' Check for required parameters
If IsEmpty(ConfigKey) or ConfigKey = "" then
	strError = strError & "ConfigKey is a required parameter"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

'ON ERROR RESUME NEXT
select case lcase(ConfigKey)
	case "reg_server_name"
		Response.Write Application("RegServerName")
	case "acx_server_name"
		Response.Write Application("ACXServerName")
	case "custom_fields"
		Response.Write GetCustomFields(custom_fields_dict, req_custom_fields_dict, null)
	case "alt_ids"
		Response.Write GetCustomFields(alt_ids_dict, req_alt_ids_dict, unique_alt_ids_dict)
	case "custom_plate_fields"
		Response.Write GetCustomFields(custom_plate_fields_dict, req_custom_plate_fields_dict, null)
	case "custom_well_fields"
		Response.Write GetCustomFields(custom_well_fields_dict,req_custom_well_fields_dict, null)
	case else
		Response.Write ""
end select

function GetCustomFields(ByRef fieldsDict, ByRef reqFieldsDict, ByRef uniqueFieldsDict)
	CustomFields = ""
	for each key in fieldsDict
		field = key
		fieldLabel = fieldsDict(key)
		required = 0
		for	each key2 in reqFieldsDict
			if key2 = key then required = 1
		next
		if isObject(uniqueFieldsDict) then
			unique = ""
			for each key3 in uniqueFieldsDict
				if key3 = key then unique = "-U"
			next
		end if
		CustomFields = CustomFields & field & ":" & fieldLabel & ";" & required & unique & ","
	next
	if CustomFields <> "" then CustomFields = left(CustomFields,len(CustomFields)-1)
	GetCustomFields = CustomFields
end function
</SCRIPT>