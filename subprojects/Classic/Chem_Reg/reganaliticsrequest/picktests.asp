<%@ Language=VBScript %>

<%
dbkey="reg"
formgroup="base_form_group"
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->

<%
Response.Expires = -1
RegIDList = Request("RegIDList")
LocationID = Request("LocationID")

Dim Conn
Dim RS, ExperimentRS
Dim ConnStr
Dim rsStatus

'stop
Set Conn = GetNewConnection("reg", "base_form_group", "base_connection")

sql = "SELECT EXPERIMENT_TYPE_ID, EXPERIMENT_TYPE_NAME FROM EXPERIMENTTYPE"
Set ExperimentRS = Conn.Execute(sql)

sql= "SELECT r.reg_id, Max(r.last_batch_number) as last_batch_number, Max(a.identifier) AS ChemicalName FROM Reg_Numbers r, Alt_ids a WHERE r.Reg_id = a.Reg_internal_id(+) AND a.identifier_type(+) = 0 AND r.Reg_ID IN(" & RegIDList & ") GROUP BY Reg_ID ORDER BY Reg_ID"
'Response.Write sql
'Response.end
Set RS = Conn.Execute(sql)
' Response.Write RS.GetString(2,, " | ", "<BR>", "NULL")
'GetInvConnection()
'SQL = "SELECT container_status_id FROM inv_container_status WHERE container_status_name = 'In Use'"
'Set rsStatus = Conn.Execute(SQL)
'defaultStatusID = rsStatus("container_status_id")
%>
<!--#INCLUDE VIRTUAL = "/chem_reg/RegAnaliticsRequest/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/chem_reg/RegAnaliticsRequest/guiUtils.asp"-->
<html>
<head>
<title>Create Test Request from Registry Compounds</title>
<script LANGUAGE="javascript" src="/chem_reg/RegAnaliticsRequest/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/chem_reg/RegAnaliticsRequest/utils.js"></script>
<script LANGUAGE="javascript" src="/chem_reg/RegAnaliticsRequest/gui/validation.js"></script>
<script LANGUAGE="javascript">
	// default container status = 1 (available)
	var ContainerStatusID = 1;
	var RegIDList = "<%=RegIDList%>";
	var Container_arr = RegIDList.split(",");
	var Str_arr = "";
	var BatchList = "";
	
	function ValidateBatch(){
		
		var RegNum;
		var size = "";
		var Initial = "";
		var UOMID = "";
		var errmsg = "Please fix the following problems:\r\r";
		var bWriteError = false;
		var blocalError;
		
<% if Request("expandList") = 1 then %>
		for (i = 0; i < document.form1.checkedBatch.length; i++)
		{
			if (document.form1.checkedBatch[i].checked) { 
				if (i != 0 && BatchList.length != 0) {
					BatchList = BatchList + ",";
				} 
				BatchList = BatchList + document.form1.checkedBatch[i].value; 
			}
		}
		// DJP: fix for CSBR-59949
		if (BatchList == "")
			BatchList = document.form1.checkedBatch.value;
		var Batch_arr = BatchList.split(",");
<% end if%>
		// Destination is required
		if (document.form1.LocationID.value.length == 0) {
			BatchList = "";
			errmsg = errmsg + "- Destination is required.\r\r";
			alert(errmsg);
			return false;
		}
		else{
			// Destination must be a number
			if (!isNumber(document.form1.LocationID.value)){
			errmsg = errmsg + "- Destination must be a number.\r\r";
			bWriteError = true;
			}
		}
		
<% if Request("expandList") = 1 then %>
		Str_arr = Batch_arr;
		for (i=0; i< Batch_arr.length; i++){
			blocalError =  false;
			RegNum = eval("document.all.RegNum_" + Batch_arr[i] + ".innerHTML")
			BatchNum = eval("document.form1.BatchNum_" +  Batch_arr[i] + ".value");
			size = eval("document.form1.ContainerSize_" +  Batch_arr[i] + ".value");
			Initial = eval("document.form1.InitialAmount_" +  Batch_arr[i] + ".value");
			UOMID = eval("document.form1.UOMID_" +  Batch_arr[i] + ".value");
			errmsg = errmsg + "For Reg #" + RegNum + " / Batch #" + BatchNum + ":\r";
<% else %>
		Str_arr = Container_arr;
		for (i=0; i< Container_arr.length; i++){
			blocalError =  false;
			RegNum = eval("document.all.RegNum_" + Container_arr[i] + ".innerHTML")
			size = eval("document.form1.ContainerSize_" +  Container_arr[i] + ".value");
			Initial = eval("document.form1.InitialAmount_" +  Container_arr[i] + ".value");
			UOMID = eval("document.form1.UOMID_" +  Container_arr[i] + ".value");
			errmsg = errmsg + "For Reg #" + RegNum + ":\r";
<% end if %>			
			if (size.length == 0) {
				errmsg = errmsg + "   - Size is required.\r";
				bWriteError = true;
				blocalError = true
			}
			else{
				 //size must be a number
				if (!isNumber(size)){
					errmsg = errmsg + "   - Size must be a number.\r";
					bWriteError = true;
					blocalError = true;
				}
			}
			if (Initial.length == 0) {
				errmsg = errmsg + "   - Initial Amount is required.\r";
				bWriteError = true;
				blocalError = true
			}
			else{
				 //size must be a number
				if (!isNumber(Initial)){
					errmsg = errmsg + "   - Initial Amount must be a number.\r";
					bWriteError = true;
					blocalError = true;
				}
			}
			if (UOMID.length == 0) {
				errmsg = errmsg + "   - Unit of measure is required.\r";
				bWriteError = true;
				blocalError = true
			}
			if (!blocalError) errmsg +=  "   -No errors.\r"
		}
			
		//bWriteError = true;
		if (bWriteError){
			BatchList = "";
			alert(errmsg);
		}
		else{
			var xml = GetActionBatchXML(Str_arr);
			
			document.form1.ActionBatchXML.value = xml;
			//alert(document.form1.ActionBatchXML.value);
			document.form1.submit();
		}
	}
	
	function GetActionBatchXML(Str_arr){
		var LocationID = document.form1.LocationID.value;
		var mydoc = new ActiveXObject("Msxml2.DOMDocument");
		// <CHEMINVACTIONBATCH>
		RootElm = mydoc.createElement("CHEMINVACTIONBATCH");
		RootElm.setAttribute("FromReg","true");
		mydoc.appendChild(RootElm);
		for (i=0; i< Str_arr.length; i++){
			BatchID = eval("document.form1.BatchNum_" + Str_arr[i] + ".value");
<% if Request("expandList") = 1 then %>
			RegID = Str_arr[i].indexOf("_");
			RegID = Str_arr[i].substring(0, RegID);
<% else %>
			RegID = Str_arr[i];
<% end if %>
			BatchNumber = BatchID
			size = eval("document.form1.ContainerSize_" +  Str_arr[i] + ".value");
			Initial = eval("document.form1.InitialAmount_" +  Str_arr[i] + ".value");
			UOMID = eval("document.form1.UOMID_" +  Str_arr[i] + ".value");
			UOMName = eval("document.form1.UOMID_" +  Str_arr[i] + ".options[document.form1.UOMID_" +  Str_arr[i] + ".selectedIndex].id");
			ContainerTypeID = eval("document.form1.ContainerTypeID_" +  Str_arr[i] + ".value");
			ContainerTypeName = eval("document.form1.ContainerTypeID_" +  Str_arr[i] + ".options[document.form1.ContainerTypeID_" +  Str_arr[i] + ".selectedIndex].id");
			ChemicalName = eval("document.all.ChemicalName_" + Str_arr[i] + ".innerHTML");
			ChemicalName = ChemicalName.replace("&nbsp;", " ");
			//<CREATECONTAINER>
			ActionElm = mydoc.createElement("CREATECONTAINER");
			ActionElm.setAttribute("LocationID",LocationID);
			ActionElm.setAttribute("MaxQty",size);
			ActionElm.setAttribute("InitialQty",Initial);
			ActionElm.setAttribute("UOMID",UOMID);
			ActionElm.setAttribute("UOMName",UOMName);
			ActionElm.setAttribute("ContainerTypeID",ContainerTypeID);
			ActionElm.setAttribute("ContainerTypeName",ContainerTypeName);
			ActionElm.setAttribute("ContainerStatusID",ContainerStatusID);
			//<OPTIONALPARAMS>
			OptParams= mydoc.createElement("OPTIONALPARAMS"); 
			//<REGBATCHID>
			OptElm = mydoc.createElement("REGID"); 
			OptElm.text = RegID;
			//<BATCHNUMBER>
			OptElm2 = mydoc.createElement("BATCHNUMBER"); 
			OptElm2.text = BatchNumber;
			//<CONTAINERNAME>
			OptElm3 = mydoc.createElement("CONTAINERNAME"); 
			OptElm3.text = ChemicalName;
			//<CONTAINERSTATUSID>
			OptElm4 = mydoc.createElement("CONTAINERSTATUSID")
			OptElm4.text = '<%=defaultStatusID%>';
			// Asemble the tree
			ActionElm.appendChild(OptParams);
			OptParams.appendChild(OptElm);
			OptParams.appendChild(OptElm2);
			OptParams.appendChild(OptElm3);
			OptParams.appendChild(OptElm4);
			RootElm.appendChild(ActionElm);
		}
		return mydoc.xml;
	}
	
	function SetExpandListURL(element)
	{
		element.href = element.href + "&LocationID=" + document.form1.LocationID.value;
	}
	
</script>
</head>
<body onload=window.focus()>
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top">
			<img src="../graphics/logo_regsys_250.gif" border="0" WIDTH="286" HEIGHT="50">
		</td>
		<td align="right">
			<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameReg"))%></b></font>
		</td>
	</tr>
</table>
<form name="form1" action="CreateTestRequest.asp?RegIDList=<%=RegIDList%>" method="POST">
	<input type="hidden" name="ActionBatchXML">
<table border="0">
<!--tr>
	<td align="right" nowrap>
		<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
	</td>
	<td>
		<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 31, false%> 
	</td>
</tr-->
</table>
<p><span class="GUIFeedback">Select the experiments for the compounds:</span></p>
Plate Size: <input type="text" name="plateSize" value="80" size=3><br>
Perform following experiments
<br>
<%
if not (ExperimentRS.BOF and ExperimentRS.EOF) then
	ExperimentRS.MoveFirst
	
	While Not ExperimentRS.EOF %>
		<input type="checkbox" name="checkedExperiment" value="<%=ExperimentRS("EXPERIMENT_TYPE_NAME")%>" checked><%=ExperimentRS("EXPERIMENT_TYPE_NAME")%><br>
		<%ExperimentRS.MoveNext
	wend
end if 

%>

on following batches

<%
if request("expandList") <> "1" then
	Response.Write("&nbsp;&nbsp;<a class=""MenuLink"" href=""pickTests.asp?RegIDList=" & RegIDList & "&expandList=1"" onclick=""SetExpandListURL(this);"" target=""_self"">Expand Batch List</a>")
else
	Response.Write("&nbsp;&nbsp;<a class=""MenuLink"" href=""pickTests.asp?RegIDList=" & RegIDList & "&expandList=0"" onclick=""SetExpandListURL(this);"" target=""_self"">Collapse Batch List</a>")
end if
%>
<table border="0">
	<tr>
		<th>Container</th>
		<% if Request("expandList") = 1 then Response.Write("<th></th>") end if %>
		<th>Registry #</th>
		<th><span class="required">Batch #</span></th>
		<th>Chemical Name</th>
		<!--th>Notebook</th-->
		<!--th>Page</th-->
		<th>Chemist</th>
		<!--th><span class="required">Type</span></th-->
		<!--th><span class="required">Size</span></th-->
		<!--th><span class="required">UOM</span></th-->
		<!--th><span class="required">Amount</span></th-->
	</tr>
<%
If RS.BOF AND RS.EOF then
	Response.Write "Error: No rows returned"
Else
	n = 0
	Do While NOT RS.EOF
		RegID = RS("Reg_ID")
		LastBatch =  RS("last_batch_number")

		'SYAN modified this section on 11/21/2005 to fix CSBR-61357
		sql = "select batch_number from batches where reg_internal_id=" & RegID & " order by batch_number DESC"
		Set RSBatchNumber = Conn.Execute(sql)
		
		if not (RSBatchNumber.BOF and RSBatchNumber.EOF) then
			if Request("expandList") = 1 then
				if cInt(LastBatch) > 1 then
					Do while Not RSBatchNumber.EOF
						n = n + 1
						i = CLng(RSBatchNumber("batch_number"))
						
						if i < 10 then
							i = "0" & i
						end if
						Response.Write "<tr>" & vbcrlf
						Response.Write vbtab & "<td align=center>" & n & "</td>" & vbcrlf
						Response.Write vbtab & "<td align=center><input type=""checkbox"" name=""checkedBatch"" value=""" & RegID & "_" & i & """ checked></td>" & vbcrlf
						Response.Write vbtab & "<td><span id=""RegNum_" & RegID & "_" & i & """>&nbsp;</span></td>" & vbcrlf
						Response.Write vbtab & "<td align=""right""><input type=""hidden"" name=""BatchNum_" & RegID & "_" & i & """ value=""" & i & """>" & i & "</td>" & vbcrlf
						Response.Write vbtab & "<td align=right>" & truncateInSpan(RS("ChemicalName"),20, "ChemicalName_" & RegID & "_" & i) & "</td>" & vbcrlf
						'Response.Write vbtab & "<td align=right><span id=""NoteBookName_" & RegID & "_" & i & """>&nbsp;</span></td>" & vbcrlf
						'Response.Write vbtab & "<td align=right><span id=""NoteBookPage_" & RegID & "_" & i & """>&nbsp;</span></td>" & vbcrlf
						Response.Write vbtab & "<td align=right><span id=""Chemist_" & RegID & "_" & i & """>&nbsp;</span></td>" & vbcrlf
						'Response.Write vbtab & "<td>" & ShowSelectBox("ContainerTypeID_" & RegID & "_" & i, 2,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>" & vbcrlf
						'Response.Write vbtab & "<td><input type=text size=5 maxlength=5 name=""ContainerSize_" & RegID & "_" & i & """></td>"		 & vbcrlf
						'Response.Write vbtab & "<td>" & ShowSelectBox("UOMID_" & RegID & "_" & i, 6,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC") & "</td>" & vbcrlf
						'Response.Write vbtab & "<td align=right><input type=text size=6 maxlength=5 name=""InitialAmount_" & RegID & "_" & i & """></td>" & vbcrlf
						Response.Write "</tr>" & vbcrlf & vbcrlf
						Response.Write "<SCRIPT language=JavaScript>UpdateBatchInfo2(" & RegID & ", """ & i & """);</SCRIPT>" & vbcrlf
						RSBatchNumber.MoveNext
					Loop
				else
					n = n + 1
					if cInt(LastBatch) < 10 then
						LastBatch = "0" & cStr(LastBatch)
					end if
					Response.Write "<tr>" & vbcrlf
					Response.Write vbtab & "<td align=center>" & n & "</td>" & vbcrlf
					Response.Write vbtab & "<td align=center><input type=""checkbox"" name=""checkedBatch"" value=""" & RegID & "_" & LastBatch & """ checked></td>" & vbcrlf
					Response.Write vbtab & "<td><span id=""RegNum_" & RegID & "_" & LastBatch & """>&nbsp;</span></td>" & vbcrlf
					Response.Write vbtab & "<td align=""right""><input type=""hidden"" name=""BatchNum_" & RegID & "_" & LastBatch & """ value=""" & LastBatch & """>" & LastBatch & "</td>" & vbcrlf
					Response.Write vbtab & "<td align=right>" & truncateInSpan(RS("ChemicalName"),20, "ChemicalName_" & RegID & "_" & LastBatch) & "</td>" & vbcrlf
					'Response.Write vbtab & "<td align=right><span id=""NoteBookName_" & RegID & "_" & LastBatch & """>&nbsp;</span></td>" & vbcrlf
					'Response.Write vbtab & "<td align=right><span id=""NoteBookPage_" & RegID & "_" & LastBatch & """>&nbsp;</span></td>" & vbcrlf
					Response.Write vbtab & "<td align=right><span id=""Chemist_" & RegID & "_" & LastBatch & """>&nbsp;</span></td>" & vbcrlf
					'Response.Write vbtab & "<td>" & ShowSelectBox("ContainerTypeID_" & RegID & "_" & LastBatch, 2,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>" & vbcrlf
					'Response.Write vbtab & "<td><input type=text size=5 maxlength=5 name=""ContainerSize_" & RegID & "_" & LastBatch & """></td>"		 & vbcrlf
					'Response.Write vbtab & "<td>" & ShowSelectBox("UOMID_" & RegID & "_" & LastBatch, 6,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC") & "</td>" & vbcrlf
					'Response.Write vbtab & "<td align=right><input type=text size=6 maxlength=5 name=""InitialAmount_" & RegID & "_" & LastBatch & """></td>" & vbcrlf
					Response.Write "</tr>" & vbcrlf & vbcrlf
					Response.Write "<SCRIPT language=JavaScript>UpdateBatchInfo2(" & RegID & ", """ & LastBatch & """);</SCRIPT>" & vbcrlf
				end if
			else
				n = n + 1
				RegID = RS("Reg_ID")
				LastBatch =  RS("last_batch_number")
				Response.Write "<tr>"
				Response.Write "<td align=center>" & n & "</td>"
				Response.Write "<td><span id=""RegNum_" & RegID & """>&nbsp;</span></td>"
				'Response.Write "<td align=right>" & GetNumSelector("BatchNum_" & RegID, CInt(LastBatch), 1, "onChange=""UpdateBatchInfo(" & RegID & ", this.value)""") & "</td>"
				batchNumberList = ""
				Do While NOT RSBatchNumber.EOF
					batchNumberList = batchNumberList & "," & RSBatchNumber("batch_number")
					RSBatchNumber.MoveNext
				Loop
				'stop
				batchNumberList = right(batchNumberList, len(batchNumberList) - 1)
				Response.Write "<td align=right>" & GetNumSelector2("BatchNum_" & RegID, batchNumberList, "onChange=""UpdateBatchInfo(" & RegID & ", this.value)""") & "</td>"
				Response.Write "<td align=right>" & truncateInSpan(RS("ChemicalName"),20, "ChemicalName_" & RegID) & "</td>"
				'Response.Write "<td align=right><span id=""NoteBookName_" & RegID & """>&nbsp;</span></td>"
				'Response.Write "<td align=right><span id=""NoteBookPage_" & RegID & """>&nbsp;</span></td>"
				Response.Write "<td align=right><span id=""Chemist_" & RegID & """>&nbsp;</span></td>"
				'Response.Write "<td>" & ShowSelectBox("ContainerTypeID_" & RegID, 2,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>"
				'Response.Write "<td><input type=text size=5 maxlength=5 name=""ContainerSize_" & RegID & """></td>"		
				'Response.Write "<td>" & ShowSelectBox("UOMID_" & RegID, 6,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC") & "</td>"
				'Response.Write "<td align=right><input type=text size=6 maxlength=5 name=""InitialAmount_" & RegID & """></td>"
				Response.Write "</tr>" 	
				Response.Write "<SCRIPT language=JavaScript>UpdateBatchInfo(" & RegID & ", " & LastBatch & ");</SCRIPT>"
			end if				
			RS.MoveNext
		Else
			RS.MoveNext
		end if
		RSBatchNumber.Close
	Loop
End if
'End of SYAN modification

'SYAN added on 5/19/2005 to fix CSBR-52680
Function GetNumSelector2 (name, numberList, dhtml)
	Dim out
	Dim arr
	if  numstart > numstop then s = -1
	out = "<Select name=""" & name & """ size=""1""" & dhtml & ">"
	arr = Split(numberList, ",")
	For i = 1 to Ubound(arr) + 1 
		if CInt(arr(i - 1)) < 10 then pad = "0"
		out = out & "<option value=""" & pad & CInt(arr(i - 1)) & """>" & pad & CInt(arr(i - 1))
	Next
	out = out & "</select>"
	GetNumSelector2 = out
End function
'End of SYAN modification
%>
	<tr>
		<td colspan="10" align="right">
			<br> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="document.form1.submit(); "><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>
</form>


<p>&nbsp;</p>

</body>
</html>