<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug


bDebugPrint = false
bWriteError = False
strError = "Error:CreateGridFormat<BR>"

p_grid_format_type = Request("p_grid_format_type")
p_row_count = Request("p_row_count")
p_col_count = Request("p_col_count")
p_row_prefix= Request("p_row_prefix")
p_row_prefix_seq = Request("p_row_prefix_seq")
p_col_prefix= Request("p_col_prefix")
p_row_use_letters= Request("p_row_use_letters")
p_col_use_letters= Request("p_col_use_letters")
p_name_separator= Request("p_name_separator")
p_number_start_corner= Request("p_number_start_corner")
p_number_direction= Request("p_number_direction")
p_name =  Request("p_name")
p_description = Request("p_description")
p_zero_padding_count = Request("p_zero_padding_count")
p_cell_naming = Request("p_cell_naming")
p_name_delimeter = Request("p_name_delimeter")

'-- If user chooses Seq naming convention
if p_cell_naming = "1" then p_row_prefix = p_row_prefix_seq

'-- Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateGridFormat.htm"
	Response.end
End if

'-- Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

'-- Check for required parameters
If IsEmpty(p_name) then
	strError = strError & "Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(p_row_count) then
	strError = strError & "Row Count is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(p_col_count) then
	strError = strError & "Col Count is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(pZero_Padding_Count) then
	strError = strError & "Zero Padding Count is a required parameter.<BR>"
	bWriteError = True
End If

'-- Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".PLATESETTINGS.CreateGridFormat", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("p_grid_format_type",131, 1, 0, p_grid_format_type)
Cmd.Parameters("p_grid_format_type").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("p_row_count",131, 1, 0, p_row_count)
Cmd.Parameters("p_row_count").Precision = 6
Cmd.Parameters.Append Cmd.CreateParameter("p_col_count",131, 1, 0, p_col_count)
Cmd.Parameters("p_col_count").Precision = 6
Cmd.Parameters.Append Cmd.CreateParameter("p_row_prefix",200, 1, 50, p_row_prefix)
Cmd.Parameters.Append Cmd.CreateParameter("p_col_prefix",200, 1, 50, p_col_prefix)
Cmd.Parameters.Append Cmd.CreateParameter("p_row_use_letters",131, 1, 0, p_row_use_letters)
Cmd.Parameters("p_row_use_letters").Precision = 1
Cmd.Parameters.Append Cmd.CreateParameter("p_col_use_letters",131, 1, 0, p_col_use_letters)
Cmd.Parameters("p_col_use_letters").Precision = 1
Cmd.Parameters.Append Cmd.CreateParameter("p_name_separator",200, 1, 1, p_name_separator)
Cmd.Parameters.Append Cmd.CreateParameter("p_number_start_corner",131, 1, 0, p_number_start_corner)
Cmd.Parameters("p_number_start_corner").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("p_number_direction",131, 1, 0, p_number_direction)
Cmd.Parameters("p_number_direction").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("p_name", 200, 1, 4000, p_name)
Cmd.Parameters.Append Cmd.CreateParameter("p_description", 200, 1, 4000, p_description)
Cmd.Parameters.Append Cmd.CreateParameter("pZeroPaddingCount", 131, 1, 0, p_zero_padding_count)
Cmd.Parameters.Append Cmd.CreateParameter("p_Cell_Naming", 131, 1, 0, p_cell_naming)
Cmd.Parameters.Append Cmd.CreateParameter("p_Name_Delimeter", 200, 1, 50, p_name_delimeter)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".PLATESETTINGS.CreateGridFormat")
End if

' Return the newly created LocationID
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
