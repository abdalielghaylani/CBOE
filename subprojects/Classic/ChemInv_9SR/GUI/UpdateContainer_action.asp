<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint

bDebugPrint = false

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
'FormData = Request.Form & Credentials
QueryString = ""
ValuePairs = ""
For each field in Request.Form
	if InStr("iContainerIDs,multiscan,multiSelect,fieldList", field) = 0 then
		Select case lcase(field)
			case "ilocation_id_fk","icompound_id_fk","iqty_max","iqty_initial","iqty_remaining","iqty_minstock","iqty_maxstock","icontainer_type_id_fk","ipurity","iconcentration","iunit_of_wght_id_fk","iweight","itare_weight","ireceived_by_id_fk","ifinal_wght","inet_wght","iqty_available","iphysical_state_id_fk","isupplier_id_fk","icontainer_status_id_fk","icontainer_cost","idef_location_id_fk","idensity","ipo_line_number","isolvent_id_fk","iunit_of_conc_id_fk","iunit_of_purity_id_fk","iunit_of_cost_id_fk","iunit_of_density_id_fk"
				ValuePairs = ValuePairs & mid(field,2) & "%3D" & Request(field) & "::"
			case "iowner_id_fk","iordered_by_id_fk","icurrent_user_id_fk","icontainer_name","igrade","icontainer_comments","ilot_num","isupplier_catnum","ipo_number","ireq_number","ifield_1","ifield_2","ifield_3","ifield_4","ifield_5","ifield_6","ifield_7","ifield_8","ifield_9","ifield_10"
				'ValuePairs = ValuePairs & mid(field,2) & "%3D'" & replace(Request(field),"'","''") & "'::"
				ValuePairs = ValuePairs & mid(field,2) & "%3D'" & server.URLEncode(replace(Request(field),"'","''")) & "'::"
			case "idate_expires","idate_ordered","idate_received","idate_produced","idate_1","idate_2","idate_3","idate_4","idate_5"
				ValuePairs = ValuePairs & mid(field,2) & "%3D TO_DATE('" & Request(field) & "','" & Application("DATE_FORMAT_STRING") & "')::"
		end select

		'Response.Write ValuePairs & "=vp<BR>"
	end if
next
QueryString = QueryString & "ContainerIDs=" & Request("iContainerIDs")
QueryString = QueryString & "&ValuePairs=" & Left(ValuePairs, len(ValuePairs) - 2)
'Response.Write "<BR>" & QueryString & "<BR>"
'Response.End
QueryString = QueryString & Credentials

'Update the containers
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
out = httpResponse

'Response.Write out & "=out<BR>"
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Update an Inventory Container</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
<%
			If isNumeric(out) then
				if Clng(out) > 0 then
					containerList = out
					theAction = "SelectContainer"
				Elseif Clng(out) = 0 then
					theAction = "Success"
				Else
					theAction = "WriteAPIError"
				End if
			Elseif InStr(Left(out,18),"|") then
				containerList = out
				out = left(out,InStr(out,"|")-1)
				theAction = "SelectContainer"	
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:"&theAction)
			Select Case theAction
				Case "SelectContainer"
					Session("bMultiSelect") = false
					multiSelect_dict.RemoveAll()
					Session("CurrentContainerID")= Request("ContainerID")
					Session("CurrentLocationID")= Request("LocationID") 
					Response.Write "<SPAN class=""GuiFeedback"">Container has been updated.</SPAN>"
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & ", " & Session("CurrentContainerID") & "');opener.focus();window.close()</SCRIPT>" 
				Case "Success"
					'-- select the first container in the list
					items = multiSelect_dict.Keys
					ContainerID = items(0)
					'Response.Write selected
					'Response.End
					Session("bMultiSelect") = false
					multiSelect_dict.RemoveAll()
					'ContainerID = Session("CurrentContainerID")
					If Session("CurrentLocationID") = "" then Session("CurrentLocationID") = 0
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & ContainerID & ");opener.focus();window.close()</SCRIPT>" 
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>Container(s) could not be updated.</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
