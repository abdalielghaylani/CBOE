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
'Response.Write plFieldList & "<BR>"

arrFields = split(plFieldList, ",")
For i = 0 to ubound(arrFields)
	field = arrFields(i)
	if len(Session(field)) > 0 then
		if InStr("iContainerIDs,multiscan,multiSelect,fieldList,AutoGen", field) = 0 then
			Select case lcase(field)
				'number fields
				case "pllocation_id_fk","plplate_pype_id_fk","plqty_unit_fk","plqty_initial","plqty_remaining","plstatus_id_fk","plsupplier_shipment_number","pllibrary_id_fk","plft_cycles","plweight_unit_fk","plweight","plconc_unit_fk","plconcentration","plsolvent_id_fk","plsolvent_volume","plsolvent_volume_unit_id_fk","plplate_type_id_fk"'"plnumcopies",
					ValuePairs = ValuePairs & mid(field,3) & "%3D" & Session(field) & "::"
				'text fields
				case "plplate_name","plsupplier_barcode","plsupplier_shipment_code","plfield_1","plfield_2","plfield_3","plfield_4","plfield_5","plgroup_name","plplate_barcode"
					ValuePairs = ValuePairs & mid(field,3) & "%3D'" & Server.URLEncode(Replace(Session(field),"'","''")) & "'::"
				'date fields
				case "plsupplier_shipment_date","pldate_1","pldate_2"
					ValuePairs = ValuePairs & mid(field,3) & "%3D TO_DATE('" & Session(field) & "','" & Application("DATE_FORMAT_STRING") & "')::"
				'well number field
				case "plwqty_remaining","plwqty_unit_fk","plwweight","plwweight_unit_fk","plwconcentration","plwconc_unit_fk","plwmolar_amount","plwmolar_unit_fk","plwsolvent_id_fk","plwsolvent_volume","plwsolvent_volume_unit_id_fk"
					WellValuePairs = WellValuePairs & mid(field,4) & "%3D" & Session(field) & "::"			
				'well text field
				'case "plwsolvent"
				'	WellValuePairs = WellValuePairs & mid(field,4) & "%3D" & Server.URLEncode(Replace(Session(field),"'","''")) & "'::"
				'don't update the field
				case else
			end select
			fieldList = fieldList & field & ","
		end if
	else
		'don't update well fields if values aren't specified
		if InStr("iContainerIDs,multiscan,multiSelect,fieldList,AutoGen", field) = 0 then
			Select case lcase(field)
				'number fields
				case "pllocation_id_fk","plplate_pype_id_fk","plqty_unit_fk","plqty_initial","plqty_remaining","plstatus_id_fk","plsupplier_shipment_number","pllibrary_id_fk","plft_cycles","plweight_unit_fk","plweight","plconc_unit_fk","plconcentration","plsolvent_id_fk","plsolvent_volume","plsolvent_volume_unit_id_fk","plplate_type_id_fk"'"plnumcopies",
					ValuePairs = ValuePairs & mid(field,3) & "%3DNULL::"
				'text fields
				case "plplate_name","plsupplier_shipment_code","plfield_1","plfield_2","plfield_3","plfield_4","plfield_5","plgroup_name","plplate_barcode"
					ValuePairs = ValuePairs & mid(field,3) & "%3DNULL::"
				'date fields
				case "plsupplier_shipment_date","pldate_1","pldate_2"
					ValuePairs = ValuePairs & mid(field,3) & "%3DNULL::"
				'well number field
				'case "plwqty_remaining","plwqty_unit_fk","plwweight","plwweight_unit_fk","plwconcentration","plwconc_unit_fk","plwmolar_amount","plwmolar_unit_fk","plwsolvent_id_fk"
				'	WellValuePairs = WellValuePairs & mid(field,4) & "%3DNULL::"			
				'well text field
				'case "plwsolvent"
				'	WellValuePairs = WellValuePairs & mid(field,4) & "%3DNULL::"
				'don't update the field
				case else
			end select
			fieldList = fieldList & field & ","
		end if
	end if
next

'For each field in Request.Form
'	Response.Write field & "=" & Request(field) & "<BR>"
'next
'Response.Write "<BR>fieldList=" & fieldList & "<BR>"
'Response.Write "<BR>valuePairs=" & ValuePairs & "<BR>"
'Response.Write "<BR>WellValuePairs=" & WellValuePairs & "<BR>"
'Response.End

QueryString = QueryString & "PlateIDs=" & Request("iPlateIDs")
QueryString = QueryString & "&ValuePairs=" & Left(ValuePairs, len(ValuePairs) - 2)
QueryString = QueryString & Credentials
'Response.Write "<BR>" & QueryString & "<BR>"
'Response.End

'Update the plate
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/updatePlate.asp", "ChemInv", QueryString)
out = httpResponse

'Response.Write out & "=out<BR>"
'Response.End
'If necessary, update the wells
if len(Request("iwQty_Remaining") & Request("iwWeight") & Request("iwConcentration") & Request("iwMolar_amount") & Request("iwSolvent") & Request("iwSolvent_Volume")) > 0 then
	QueryString = "PlateIDs=" & Request("iPlateIDs") 
	QueryString = QueryString & "&ValuePairs=" & Left(WellValuePairs,len(WellValuePairs) - 2)
	QueryString = QueryString & Credentials
	'Response.Write "<BR>" & QueryString & "<BR>"
	'Response.End
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/updateWell.asp", "ChemInv", QueryString)
	out2 = httpResponse
end if

'Response.Write out2 & "=out2<BR>"
'Response.End

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Update an Inventory Plate</title>
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
			If isNumeric(httpResponse) then
				If CLng(httpResponse) = 0 then
					CurrentLocationID = Session("pllocation_id_fk")
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate has been updated</SPAN></center>"
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & CurrentLocationID & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & Session("plPlate_ID") & ",1);  opener.parent.focus(); window.close();</SCRIPT>" 
					'Response.Write "<SCRIPT language=JavaScript>SelectLocationNode(0, " & httpResponse & ", 0, '" & Session("TreeViewOpenNodes1") & "',0,1); opener.parent.focus(); window.close();</SCRIPT>"

				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">Plate could not be updated</SPAN>"
					Response.Write "<P><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
					Response.Write "<P><CODE>Oracle Error: " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>