<%
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/well_attribute_defaults.asp"-->
<%
bDebugPrint = false
'****************************************************************************************
'*	PURPOSE: fetch all well attributes for a given well_ID and store them
'*			in session variables for use by well view tab interface
'*	INPUT: WellID AS Long, GetDbData AS Boolean passed as QueryString parameters
'*	OUTPUT: Populates session variables with well attributes
'****************************************************************************************


' Receive Posted data
WellID = Request("WellID")
if Request("WellID")="" and Request("iWellID")<>"" then WellID = Request("iWellID")
GetData = Request.QueryString("GetData")
NewCompoundID = Request("NewCompoundID")
sTab = Request("sTab")

RegID = ""
BatchNumber = ""
RegCAS = ""
NoteBook = ""
Page = ""
RegAmount = ""
RegPurity = ""
RegName = ""
BASE64_CDX = ""
RegBatchId = ""
RegScientist = ""

if NOT WellID = "" then
	Session("wWell_ID") = WellID
End if

if WellID = "" and LCase(GetData)= "db" then
	Response.Redirect "/cheminv/cheminv/SelectContainerMsg.asp?entity=Well"
End if

if CBool(Request.QueryString("isEdit")) then
	Session("wIsEdit")= True
Elseif not Session("wIsEdit") then 
	Session("wIsEdit")= false
	if CBool(Request.QueryString("wIsCopy")) then
		isCopy = true
	Else
		isCopy = false
	End if
End if
isEdit = Session("wIsEdit")

Select Case GetData
	Case "db"
		Call SetWellSessionVarsFromDb(WellID)
	Case "session"
	Case Else
		Call SetWellSessionVarsFromPostedData()
		if Len(Request("newRegID"))>0 then
			if sTab = "Substance" OR sTab="" then sTab = "RegSubstance"
			Session("wCompound_ID_FK") = ""
			GetRegBatchAttributes Request("newRegID"), request("newBatchNumber")
			RegID = Session("RegID")
			Call setWellRegAttributes()
		elseif Len(Request("newCompoundID"))>0 then 
			if sTab = "RegSubstance" OR sTab="" then sTab = "Substance"
			Session("RegID") = ""
			Session("BatchNumber") = ""
			Session("RegBatchID") = ""
			Session("wReg_ID_FK") = ""
			Session("wBatch_Number_FK") = ""
			RegId=""
			BatchNumber=""
			RegBatchID=""
			ChangeSubstanceAttributesFromDb(newCompoundID)
		End if
		wRegBatchID = Session("wRegBatchID")
End Select


Sub ClearWellSessionVars()

	arrFields = split(wFieldList, ",")
	For i = 0 to ubound(arrFields)
		field = arrFields(i)
		Session(field) = ""
	next

End Sub

Sub SetWellSessionVarsFromDb(pPlateID)
	Call GetInvConnection()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/WellSQL.asp"-->
<%
	SQL = SQL & " w.Well_ID = ?"
	Call GetInvConnection()
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection = Conn
	cmd.CommandType = adCmdText
	cmd.CommandText = SQL
	cmd.Parameters.Append cmd.CreateParameter("pWell_ID", 5, 1, 0, WellID)
	'Response.Write SQL
	'Response.end

	Set RS = Server.CreateObject("adodb.recordset")
	Set RS = cmd.Execute

	if RS.BOF AND RS.EOF then
		Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
		Response.Write "<P><CODE>Error:SetWellSessionVarsFromDb</CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve Well data for Well_ID " & WellID & "</SPAN>"
		Response.Write "</td></tr></table>"
		Response.end
	end if

	'set the session variables
	fieldList = ""
	for each field in RS.fields
		Session("w" & field.name) = field.value
		fieldList = fieldList & "w" & field.name & ","
		'Response.Write "<BR>" & Session("pl" & field.name) & "<BR>"
		'Response.Write "<BR>Session(""pl" & field.name & """) = field.value <BR>"
		'Response.Write field.value & "<BR>"
	next
	if cint(RS("CompoundCount")) = 1 then
		'get compound info
		SQL = WellCompoundSQL & " well_id_fk = ?"
		Set Cmd = GetCommand(Conn, SQL, adCmdText)
		cmd.Parameters.Append cmd.CreateParameter("pWellID", 5, 1, 0, WellID)
		'Response.Write SQL
		'Response.end
		Set RS = cmd.Execute
	    if not IsEmpty(RS("reg_id_fk")) and not IsNull(RS("reg_id_fk")) then
		    Session("wCompound_ID_FK") = ""
		    Session("wReg_ID_FK") = RS("reg_id_fk")
		    Session("wBatch_Number_FK") = RS("batch_number_fk")
	    else
   		    Session("wCompound_ID_FK") = RS("compound_id_fk")
   	    end if 	
		if Application("RegServerName") <> "NULL" then
    		Session("wREGBATCHID") = RS("reg_batch_id")
    	end if
    else
        Session("wCompound_ID_FK") = ""
	end if
	fieldList = fieldList & "wCOMPOUND_ID_FK,wREG_ID_FK,wBATCH_NUMBER_FK,wREGBATCHID,wBASE64_CDX,"
	Session("wFieldList") = left(fieldList,len(fieldList)-1)

	if Session("wReg_ID_FK") <> "" then
		GetRegBatchAttributes Session("wReg_ID_FK"), Session("wBatch_Number_FK")
		RegID = Session("wReg_ID_FK")
		BatchNumber=Session("wBatch_Number_FK")
		Call setWellRegAttributes()
	else
		Call clearWellRegAttributes()
	End if

End sub

Sub SetWellSessionVarsFromPostedData()
	' Set session variables to store posted data
	for each item in Request.Form
		'Response.Write item & " = " & Request.Form(item) & "<BR>"
		'Session("w" & mid(item,2)) = Request.Form(item)
		'Response.Write "w" & mid(item,2) & " = " & Session("w" & mid(item,2)) & "<BR>"
		if instr(item,"i") = 1 then
			Session("w" & mid(item,2)) = Request.Form(item)
			'Response.Write("w" & mid(item,2) & " = " & Request.Form(item) & "<br>")
		else
			Session("w" & item) = Request.Form(item)
			'Response.Write("w" & item & " = " & Request.Form(item) & "<br>")
		end if
	next
	RegID = Session("wRegID")
	BatchNumber=Session("wBatchNumber")
	wRegBatchID = Session("wRegBatchID")
End Sub

' Set local variables
IsEdit = Session("wIsEdit")

'Response.Write Session("wFieldList")
arrFields = split(Session("wFieldList"),",")
for i = 0 to ubound(arrFields)
	'Response.Write Session(arrFields(i)) & "=" & arrFields(i) & "<BR>"
	execute mid(arrFields(i),2) & " = Session(arrFields(i))"
next


Sub ChangeSubstanceAttributesFromDb(pCompoundID)
	Call GetInvConnection()
	SQL = "SELECT inv_compounds.Compound_ID, inv_compounds.CAS, inv_compounds.ACX_ID, inv_compounds.ALT_ID_1, inv_compounds.ALT_ID_2, inv_compounds.ALT_ID_3, inv_compounds.ALT_ID_4, inv_compounds.ALT_ID_5, inv_compounds.Substance_Name, inv_compounds.Base64_CDX FROM inv_compounds WHERE inv_compounds.Compound_ID=" & pCompoundID
	if bDebugPrint then
		Response.Write sql
		Response.end
	end if
	Set RS= Conn.Execute(SQL)
	if RS.BOF AND RS.EOF then
		Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
		Response.Write "<P><CODE>Error:ChangeSubstanceAttributesFromDb</CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve compound data for Compound_ID " & pCompoundID & "</SPAN>"
		Response.Write "</td></tr></table>"
		Response.end
	end if

	' Substance attributes
	Session("wCompound_ID_FK") = RS("Compound_ID").value
	Session("wCAS") = RS("CAS").value
	Session("wACX_ID") = RS("ACX_ID").value
	Session("wSubstance_Name") = RS("Substance_Name").value
	Session("wBase64_CDX") = RS("Base64_CDX").value
	Compound_ID_FK=RS("Compound_ID").value
End Sub

Sub setWellRegAttributes()

	'set well attribs
	Session("wReg_ID_FK") = Session("RegID")
	Session("wBatch_Number_FK") = Session("BatchNumber")
	'Session("wRegCAS") = Session("RegCAS")
	'Session("wNoteBook") = Session("NoteBook")
	'Session("wPage") = Session("Page")
	'Session("wRegAmount") = Session("RegAmount")
	'Session("wRegPurity") = Session("RegPurity")
	'Session("wRegName") = Session("RegName")
	'Session("wRegSynonym") = Session("RegSynonym")
	'Session("wBASE64_CDX") = Session("BASE64_CDX")
	'Session("wRegBatchID") = Session("RegBatchID")
	'Session("wRegScientist") = Session("RegScientist")
	
	'empty placeholder attribs
	'Session("RegID") = ""
	'Session("NoteBook") = ""
	'Session("Page") = ""
	'Session("RegAmount") = ""
	'Session("RegPurity") = ""
	'Session("RegName") = ""
	'Session("RegSynonym") = ""
	'Session("BASE64_CDX") =  ""
	'Session("RegBatchID") = ""
	'Session("RegScientist") = ""
	for each key in reg_fields_dict
		'set well attribs
		tempKey="w" & key
		execute("session (""" & tempKey & """)" &  " = Session(cStr(key))")
		execute(tempKey &  " = Session(cStr(key))")
		'empty placeholder attribs
		Session(key) = ""
	next

End Sub

Sub clearWellRegAttributes()
	'Clear Well Reg Attributes
	Session("wReg_ID_FK") = ""
	Session("wBatch_Number_FK") = ""
	Session("wRegCAS") = ""
	Session("wcpdID") = ""
	Session("wNoteBook") = ""
	Session("wPage") = ""
	Session("wRegAmount") = ""
	Session("wRegPurity") = ""
	Session("wRegName") = ""
	Session("wRegSynonym") = ""
	Session("wBASE64_CDX") = ""
	Session("wRegBatchID") = ""
	Session("wRegScientist") = ""
	
End Sub
%>
