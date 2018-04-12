<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetPlateAttributes.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
QueryString = ""
ValuePairs = ""
WellValuePairs = ""
fieldList = ""
plFieldList = Session("plFieldList")
NumCopies = Request("iNumCopies")
isCopy = Request("isCopy")
bWellUpdateError = false

if lcase(isCopy) = "true" then 	Session("plqty_remaining") = Session("plqty_initial")
arrFields = split(plFieldList, ",")
For i = 0 to ubound(arrFields)
	field = arrFields(i)
	if len(Session(field)) > 0 then
		if InStr("iContainerIDs,multiscan,multiSelect,fieldList,AutoGen", field) = 0 then
			Select case lcase(field)
				'number fields
				case "pllocation_id_fk","plplate_type_id_fk","plqty_unit_fk","plqty_initial","plqty_remaining","plstatus_id_fk","plsupplier_shipment_number","pllibrary_id_fk","plft_cycles","plweight_unit_fk","plweight","plconc_unit_fk","plconcentration","plsolvent_id_fk","plsolvent_volume","plsolvent_volume_unit_id_fk","plsolution_volume"
					ValuePairs = ValuePairs & mid(field,3) & "%3D" & Session(field) & "::"
				'text fields
				case "plplate_barcode","plplate_name","plsupplier_shipment_code","plgroup_name","plfield_1","plfield_2","plfield_3","plfield_4","plfield_5","plsupplier","plsupplier_barcode"
					ValuePairs = ValuePairs & mid(field,3) & "%3D'" & Server.URLEncode(Replace(Session(field),"'","''")) & "'::"
				'date fields
				case "plsupplier_shipment_date","pldate_1","pldate_2"
					ValuePairs = ValuePairs & mid(field,3) & "%3D TO_DATE('" & Session(field) & "','MM-DD-YYYY')::"
				'don't update the field
				case else
			end select
			fieldList = fieldList & field & ","
		end if
	end if
next
For i = 0 to ubound(arrFields)
	field = arrFields(i)
	if len(Session(field)) > 0 then
		if InStr("iContainerIDs,multiscan,multiSelect,fieldList,AutoGen", field) = 0 then
			Select case lcase(field)
				'use relevant plate values to populate well values
				'well number field
				case "plqty_unit_fk","plqty_initial","plqty_remaining","plweight_unit_fk","plweight","plconc_unit_fk","plconcentration","plsolvent_id_fk","plsolvent_volume","plsolvent_volume_unit_id_fk","plsolution_volume"
					WellValuePairs = WellValuePairs & mid(field,3) & "%3D" & Session(field) & "::"			
				'well text field
				case "none"
					WellValuePairs = WellValuePairs & mid(field,4) & "%3D'" & Server.URLEncode(Replace(Session(field),"'","''")) & "'::"
				'don't update the field
				case else
			end select
		end if
	end if
next

'ValuePairs = ValuePairs & "barcode_desc_id%3D" & Server.URLEncode(Replace(Request("ibarcode_desc_id"),"'","''")) & "::"

'Response.Write "<BR>fieldList=" & fieldList & "<BR>"
'Response.Write "<BR>valuePairs=" & ValuePairs & "<BR>"
'Response.Write "<BR>WellValuePairs=" & WellValuePairs & "<BR>"
'Response.Write isCopy & "=isCopy<BR>"
'Response.End

QueryString = QueryString & "PlateIDs=" & Request("iPlateIDs")
QueryString = QueryString & "&NumCopies=" & NumCopies
'set the barcode_desc_id if necessary
if Request("bUseBarcodeDesc") = "true" then
	QueryString = QueryString & "&BarcodeDescID=" & Request("ibarcode_desc_id")
end if
QueryString = QueryString & "&ValuePairs=" & Left(ValuePairs, len(ValuePairs) - 2)
QueryString = QueryString & Credentials
'Response.Write "<BR>" & QueryString & "<BR>"
'Response.End

' Create the plates
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/createPlateFromMap.asp", "ChemInv", QueryString)
newPlateIDs = httpResponse
'Response.Write newPlateIDs & "=newPlateIDs<BR>"
'Response.End

'update the wells
QueryString = "PlateIDs=" & replace(newPlateIDs,"|",",")
QueryString = QueryString & "&ValuePairs=" & Left(WellValuePairs,len(WellValuePairs) - 2)
QueryString = QueryString & Credentials
'Response.Write "<BR>" & QueryString & "<BR>"
'Response.End
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/updateWell.asp", "ChemInv", QueryString)
out2 = httpResponse
if trim(cStr(out2)) <> "0" then bWellUpdateError = true

'Response.Write out2 & "=out2<BR>"
'Response.End
arrPlateIDs = split(newPlateIDs,"|")
if isNumeric(arrPlateIDs(0)) then
	numPlates = ubound(arrPlateIDs) + 1
	plateIDFocus = arrPlateIDs(0)
else
	numPlates = 0
end if
'Response.Write numPlates & "=num<BR>"
'Response.End

'increment plate table count
currCount = Application("inv_platesRecordCount" & Application("appkey"))
Application.Lock
	Application("inv_platesRecordCount" & Application("appkey")) = cInt(currCount) + cInt(NumCopies)
Application.UnLock

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create an Inventory Plate</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE>
			<% 
			If numPlates > 0 and not bWellUpdateError then
				If not bUpdateWells then
					Session("CurrentLocationID") = Request("iLocation_ID_FK")
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate has been updated</SPAN></center>"
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & plateIDFocus & ",1); opener.focus(); window.close();</SCRIPT>"
					'Response.Write "SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & plateIDFocus & ",1); opener.focus(); window.close();</SCRIPT>"  
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">Plate could not be updated</SPAN>"
					Response.Write "<P><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				If bWellUpdateError then
					Response.Write "<P><CODE>Oracle Error: " & out2 & "</code></p>" 
				else
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
				end if
				Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>