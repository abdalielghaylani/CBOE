<%
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/plate_attribute_defaults.asp"-->
<%
bDebugPrint = false
'****************************************************************************************
'*	PURPOSE: fetch all plate attributes for a given plate_ID and store them  
'*			in session variables for use by plate view tab interface			 	                            
'*	INPUT: PlateID AS Long, GetDbData AS Boolean passed as QueryString parameters  	                    			
'*	OUTPUT: Populates session variables with plate attributes										
'****************************************************************************************


' Receive Posted data
PlateID = Request("PlateID")
GetData = Request.QueryString("GetData")
AutoGen = Request("AutoGen")
'sTab = Request.QueryString("TB")

If AutoGen <> "" then 
	Session("plAutoGen") = AutoGen
Else	
	AutoGen = Session("plAutoGen")
End if
If AutoGen = "" then AutoGen = "true"


if NOT PlateID = "" then 
	Session("PlateID") = PlateID
End if

if PlateID = "" and LCase(GetData)= "db" then
	Response.Redirect "/cheminv/cheminv/SelectContainerMsg.asp?entity=Plate"
End if

if CBool(Request.QueryString("isEdit")) then 
	Session("plIsEdit")= True
Else
	Session("plIsEdit")= false
	if CBool(Request.QueryString("plIsCopy")) then 
		isCopy = true
	Else
		isCopy = false
	End if	
End if
'Response.Write Session("plIsEdit") & "=plIsEdit<BR>"


Select Case GetData
	Case "db"
		Call SetPlateSessionVarsFromDb(PlateID)
	Case "session"
	Case Else
		Call SetPlateSessionVarsFromPostedData()
End Select


Sub ClearContainerSessionVars()
	
	SetDefaultAttributeValues()
	
	'set the session variables
	' Required Container attributes
	Session("LocationID") = Session("CurrentLocationID")
	Session("LocationName") = Session("CurrentLocationName")
	Session("ContainerID") = ""
	Session("ContainerName") = "" 
	Session("Barcode") = Request("Barcode")
	Session("RegID") = ""
	Session("RegNumber") = ""
	Session("RegBatchID") = ""
	Session("QtyMax") = ""
	Session("QtyInitial") = ""
	
	
	tempArr = Split(Session("UOMIDOptionValue"),"=")
	Session("UOMAbv")= tempArr(1)
	Session("UOMID") = tempArr(0)
	Session("UOMName") = "" 
	tempArr = Split(Session("UOWIDOptionValue"),"=")
	Session("UOWAbv")= tempArr(1)
	Session("UOWID") = tempArr(0)
	Session("UOWName") = ""
	tempArr = Split(Session("UOCIDOptionValue"),"=")
	Session("UOCAbv")= tempArr(1)
	Session("UOCID") = tempArr(0)
	Session("UOCName") = ""
	tempArr = Split(Session("UODIDOptionValue"),"=")
	Session("UODAbv")= tempArr(1)
	Session("UODID") = tempArr(0)
	Session("UODName") = ""
	tempArr = Split(Session("UOPIDOptionValue"),"=")
	Session("UOPAbv")= tempArr(1)
	Session("UOPID") = tempArr(0)
	Session("UOPName") = ""
	
	
	' Optional Container Attributes
	Session("TareWeight") = ""
	Session("NetWeight") = ""
	Session("FinalWeight") = ""
	Session("MinStockQty") = ""
	Session("MaxStockQty") = ""
	Session("ContainerDesc") = ""
	
	' Contents attributes
	Session("Purity") = ""
	Session("Concentration") = ""
	Session("Density") = ""
	Session("Grade") = ""
	Session("Solvent") = ""
	Session("Comments") = ""
	Session("RegID") = ""
	Session("BatchNumber") = ""
	
		
	' Calculated Quantities
	Session("QtyRemaining") = ""
	Session("QtyAvaliable") = ""
	Session("DateCreated") = ""
	
	' Substance attributes
	Session("CompoundID") = ""
	Session("CAS") = ""
	Session("ACX_ID") = ""
	Session("ALT_ID_1") = ""
	Session("ALT_ID_2") = ""
	Session("ALT_ID_3") = ""
	Session("ALT_ID_4") = ""
	Session("ALT_ID_5") = ""
	Session("SubstanceName") = ""
	Session("Base64_CDX") = ""
	
	' Supplier Attributes
	Session("SupplierName") = ""
	Session("SupplierShortName") = ""
	Session("SupplierCatNum") = ""
	Session("LotNum") = ""
	Session("SupplierCatNum") = ""
	Session("DateProduced") = ""
	Session("DateOrdered") = ""
	
	Session("ContainerCost") = ""
	Session("PONumber") = ""
	Session("POLineNumber") = ""
	Session("ReqNumber") = ""
	
	Session("CurrentUserID") = Ucase(Session("UserNameChemInv"))
	Session("RequestID") = ""
	

End Sub

Sub SetPlateSessionVarsFromDb(pPlateID)
	Call GetInvConnection()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/PlateSQL.asp"-->
<%
	SQL = SQL & " p.Plate_ID = ?"
	
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection = Conn
	cmd.CommandType = adCmdText
	cmd.CommandText = SQL
	cmd.Parameters.Append cmd.CreateParameter("pPlate_ID", 5, 1, 0, pPlateID) 
	'Response.Write cmd.CommandText
	'Response.End
	
	Set RS = Server.CreateObject("adodb.recordset")
	Set RS = cmd.Execute

	'Response.Write SQL
	'Response.end
	'Set RS = Conn.Execute(SQL)
	
	if RS.BOF AND RS.EOF then
		Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
		Response.Write "<P><CODE>Error:SetPlateSessionVarsFromDb</CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve Plate data for Plate_ID " & PlateID & "</SPAN>"
		Response.Write "</td></tr></table>"
		Response.end
	end if
	
	'set the session variables
	fieldList = ""
	for each field in RS.fields
		Session("pl" & field.name) = field.value
		fieldList = fieldList & "pl" & field.name & ","
		'Response.Write "<BR>" & Session("pl" & field.name) & "<BR>"
		'Response.Write "<BR>Session(""pl" & field.name & """) = " & field.value  & "<BR>"
		'Response.Write field.value & "<BR>"
	next
	' add numcopies
	fieldList = fieldList & "plNumCopies,"
	' add well fields for updating all wells in the plate
	fieldList = fieldList & "plwQty_Remaining,plwQty_Unit_FK,plwWeight,plwWeight_Unit_FK,plwSolvent_ID_FK,plwConcentration,plwConc_Unit_FK,plwMolar_amount,plwMolar_Unit_FK,plwSolvent_Volume,plwSolvent_Volume_Unit_ID_FK"
	Session("plFieldList") = fieldList
	
	if isCopy then
		Session("plPlate_Barcode") = ""
	End if
	
End sub

Sub SetPlateSessionVarsFromPostedData()
	' Set session variables to store posted data
	for each item in Request.Form
		'Response.Write item & " = " & Request.Form(item) & "<BR>"
		Session("pl" & mid(item,2)) = Request.Form(item)
		'Response.Write Session("w" & mid(item,2)) & " = w" &  mid(item,2) & "<BR>"
	next
End Sub

' Set local variables
'Response.Write Session("plFieldList") & "<BR>"
arrFields = split(Session("plFieldList"),",")
for i = 0 to ubound(arrFields)

	if not IsNull(Session(arrFields(i)))then
	Session(arrFields(i))=(Replace(Session(arrFields(i)),""""," "))
	end if
    on error resume next
	execute mid(arrFields(i),3) & " = """ &  Session(arrFields(i)) & """"
    if err.number <> 0 then
        execute mid(arrFields(i),3) & " = """ & Replace(Session(arrFields(i)),chr(10), " ")
        on error goto 0
    end if
next
%>
