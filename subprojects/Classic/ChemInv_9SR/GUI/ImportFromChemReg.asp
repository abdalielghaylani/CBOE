<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<%

'-- store the aspID so core can kill the session
StoreASPSessionID()	

Response.Expires = -1
RegIDList = Request("RegIDList")
LocationID = Request("LocationID")
dbkey = Application("appkey")
'-- only allow users with the CREATE_CONTAINER privilege to proceed 
If not Session("INV_CREATE_CONTAINER" & dbkey) then
%>
<html>
<head>
<title>Create Inventory Containers from Registry Compounds</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>

</head>
<body>
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top">
			<img src="../graphics/cheminventory_banner.gif" border="0" WIDTH="286" HEIGHT="50">
		</td>
		<td align="right">
			<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
		</td>
	</tr>
</table>
<table border="0" width="100%">
<tr>
	<td align="center"><BR /><BR /><span class="GuiFeedback">You do not have the proper privileges to create a container.<BR> Please contact your system administrator.</span></td>
</tr>
<tr>
	<td align="center">
		<br> 
		<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
	</td>
</tr>	
</table>
<p>&nbsp;</p>

</body>
</html>
<%
else

Dim Conn
Dim RS
Dim ConnStr
Dim rsStatus

GetRegConnection()
sql= "SELECT r.reg_id, Max(r.last_batch_number) as last_batch_number, Max(a.identifier) AS ChemicalName FROM Reg_Numbers r, Alt_ids a WHERE r.Reg_id = a.Reg_internal_id(+) AND a.identifier_type(+) = 0 AND r.Reg_ID IN(" & RegIDList & ") GROUP BY Reg_ID ORDER BY Reg_ID"
'Response.Write sql
'Response.end
Set RS = Conn.Execute(sql)
' Response.Write RS.GetString(2,, " | ", "<BR>", "NULL")
GetInvConnection()
SQL = "SELECT container_status_id FROM inv_container_status WHERE container_status_name = 'In Use'"
Set rsStatus = Conn.Execute(SQL)
defaultStatusID = rsStatus("container_status_id")
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<html>
<head>
<title>Create Inventory Containers from Registry Compounds</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
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
		if (document.form1.checkedBatch.length)
		{
			// multiple batches
			for (i = 0; i < document.form1.checkedBatch.length; i++)
			{
				if (document.form1.checkedBatch[i].checked) { 
					if (i != 0 && BatchList.length != 0) {
						BatchList = BatchList + ",";
					} 
					BatchList = BatchList + document.form1.checkedBatch[i].value; 
				}
			}
		}
		else
		{
			//DJP: single batch
			if (document.form1.checkedBatch.checked)
				BatchList = document.form1.checkedBatch.value;
		}
		//DJP: you must check at least 1 batch
		if (BatchList == "")
		{
			errmsg = errmsg + "- You must select at least one batch.\r\r";
			bWriteError = true;
		}
		//BatchList = document.form1.checkedBatch.value;
		
		// don't validate any further if there is an issue with the BatchList
		if (bWriteError){
			BatchList = "";
			alert(errmsg);
			return false;
		}
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
			Barcode = eval("document.form1.Barcode_" + Batch_arr[i] + ".value");
			size = eval("document.form1.ContainerSize_" +  Batch_arr[i] + ".value");
			Initial = eval("document.form1.InitialAmount_" +  Batch_arr[i] + ".value");
			UOMID = eval("document.form1.UOMID_" +  Batch_arr[i] + ".value");
			errmsg = errmsg + "For Reg #" + RegNum + " / Batch #" + BatchNum + ":\r";
<% else %>
		Str_arr = Container_arr;
		for (i=0; i< Container_arr.length; i++){
			blocalError =  false;
			RegNum = eval("document.all.RegNum_" + Container_arr[i] + ".innerHTML")
			Barcode = eval("document.form1.Barcode_" + Container_arr[i] + ".value");
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
			
			//check for duplicate barcodes
			if (Barcode.length > 0 && !blocalError){
				var strURL = "http://" + serverName + "/cheminv/api/CheckDuplicateBarcode.asp?Barcodes=" + Barcode + "&BarcodeType=container";	
				var httpResponse = JsHTTPGet(strURL) 
				if (httpResponse.length > 0) {
					errmsg = errmsg + "- Barcode conflict for barcode(s): " + httpResponse + " .\r"
					bWriteError = true;
					blocalError = true;
				}
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
			Barcode = eval("document.form1.Barcode_" + Str_arr[i] + ".value");
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
			//<BARCODE>
			OptElm5 = mydoc.createElement("BARCODE")
			OptElm5.text = Barcode;
			
			// Asemble the tree
			ActionElm.appendChild(OptParams);
			OptParams.appendChild(OptElm);
			OptParams.appendChild(OptElm2);
			OptParams.appendChild(OptElm3);
			OptParams.appendChild(OptElm4);
			OptParams.appendChild(OptElm5);
			RootElm.appendChild(ActionElm);
		}
		return mydoc.xml;
	}
	
	function SetExpandListURL(element)
	{
		element.href = element.href + "&LocationID=" + document.form1.LocationID.value;
		//alert(element.href);
	}
	
</script>
</head>
<body>
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top">
			<img src="../graphics/cheminventory_banner.gif" border="0" WIDTH="286" HEIGHT="50">
		</td>
		<td align="right">
			<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
		</td>
	</tr>
</table>
<form name="form1" action="/Cheminv/api/DisplayActionBatch.asp" method="POST">
	<input type="hidden" name="ActionBatchXML">
<table border="0">
<tr>
	<td align="right" nowrap>
		<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
	</td>
	<td>
		<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 31, false%> 
	</td>
</tr>
</table>
<p><span class="GUIFeedback">Specify the type, size and initial amount of containers to be created</span></p>
<%
if request("expandList") <> "1" then
	Response.Write("&nbsp;&nbsp;<a class=""MenuLink"" href=""ImportFromChemReg.asp?RegIDList=" & RegIDList & "&expandList=1"" onclick=""SetExpandListURL(this);"" target=""_self"">Expand Batch List</a>")
else
	Response.Write("&nbsp;&nbsp;<a class=""MenuLink"" href=""ImportFromChemReg.asp?RegIDList=" & RegIDList & "&expandList=0"" onclick=""SetExpandListURL(this);"" target=""_self"">Collapse Batch List</a>")
end if
%>
<table border="0">
	<tr>
		<th>Container</th>
		<% if Request("expandList") = 1 then Response.Write("<th></th>") end if %>
		<th>Registry #</th>
		<th><span class="required">Batch #</span></th>
		<th>Barcode</th>
		<th>Chemical Name</th>
		<th>Notebook</th>
		<th>Page</th>
		<th>Chemist</th>
		<th><span class="required">Type</span></th>
		<th><span class="required">Size</span></th>
		<th><span class="required">UOM</span></th>
		<th><span class="required">Amount</span></th>
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
					Response.Write vbtab & "<td align=""right""><input type=""text"" size=""10"" name=""Barcode_" & RegID & "_" & i & """>&nbsp;</td>" & vbcrlf
						Response.Write vbtab & "<td align=right>" & truncateInSpan(RS("ChemicalName"),20, "ChemicalName_" & RegID & "_" & i) & "</td>" & vbcrlf
						Response.Write vbtab & "<td align=right><span id=""NoteBookName_" & RegID & "_" & i & """>&nbsp;</span></td>" & vbcrlf
						Response.Write vbtab & "<td align=right><span id=""NoteBookPage_" & RegID & "_" & i & """>&nbsp;</span></td>" & vbcrlf
						Response.Write vbtab & "<td align=right><span id=""Chemist_" & RegID & "_" & i & """>&nbsp;</span></td>" & vbcrlf
						Response.Write vbtab & "<td>" & ShowSelectBox("ContainerTypeID_" & RegID & "_" & i, 2,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>" & vbcrlf
						Response.Write vbtab & "<td><input type=text size=5 maxlength=5 name=""ContainerSize_" & RegID & "_" & i & """></td>"		 & vbcrlf
						Response.Write vbtab & "<td>" & ShowSelectBox("UOMID_" & RegID & "_" & i, 6,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC") & "</td>" & vbcrlf
						Response.Write vbtab & "<td align=right><input type=text size=6 maxlength=5 name=""InitialAmount_" & RegID & "_" & i & """></td>" & vbcrlf
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
					Response.Write vbtab & "<td align=""right""><input type=""text"" size=""10"" name=""Barcode_" & RegID & "_" & LastBatch & """>&nbsp;</td>" & vbcrlf
					Response.Write vbtab & "<td align=right>" & truncateInSpan(RS("ChemicalName"),20, "ChemicalName_" & RegID & "_" & LastBatch) & "</td>" & vbcrlf
					Response.Write vbtab & "<td align=right><span id=""NoteBookName_" & RegID & "_" & LastBatch & """>&nbsp;</span></td>" & vbcrlf
					Response.Write vbtab & "<td align=right><span id=""NoteBookPage_" & RegID & "_" & LastBatch & """>&nbsp;</span></td>" & vbcrlf
					Response.Write vbtab & "<td align=right><span id=""Chemist_" & RegID & "_" & LastBatch & """>&nbsp;</span></td>" & vbcrlf
					Response.Write vbtab & "<td>" & ShowSelectBox("ContainerTypeID_" & RegID & "_" & LastBatch, 2,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>" & vbcrlf
					Response.Write vbtab & "<td><input type=text size=5 maxlength=5 name=""ContainerSize_" & RegID & "_" & LastBatch & """></td>"		 & vbcrlf
					Response.Write vbtab & "<td>" & ShowSelectBox("UOMID_" & RegID & "_" & LastBatch, 6,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC") & "</td>" & vbcrlf
					Response.Write vbtab & "<td align=right><input type=text size=6 maxlength=5 name=""InitialAmount_" & RegID & "_" & LastBatch & """></td>" & vbcrlf
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
				batchNumberList = right(batchNumberList, len(batchNumberList) - 1)
				Response.Write "<td align=right>" & GetNumSelector2("BatchNum_" & RegID, batchNumberList, "onChange=""UpdateBatchInfo(" & RegID & ", this.value)""") & "</td>"
				Response.Write "<td align=""right""><input type=""text"" size=""10"" name=""Barcode_" & RegID & """></td>"
				Response.Write "<td align=right>" & truncateInSpan(RS("ChemicalName"),20, "ChemicalName_" & RegID) & "</td>"
				Response.Write "<td align=right><span id=""NoteBookName_" & RegID & """>&nbsp;</span></td>"
				Response.Write "<td align=right><span id=""NoteBookPage_" & RegID & """>&nbsp;</span></td>"
				Response.Write "<td align=right><span id=""Chemist_" & RegID & """>&nbsp;</span></td>"
				Response.Write "<td>" & ShowSelectBox("ContainerTypeID_" & RegID, 2,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>"
				Response.Write "<td><input type=text size=5 maxlength=5 name=""ContainerSize_" & RegID & """></td>"		
				Response.Write "<td>" & ShowSelectBox("UOMID_" & RegID, 6,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC") & "</td>"
				Response.Write "<td align=right><input type=text size=6 maxlength=5 name=""InitialAmount_" & RegID & """></td>"
				Response.Write "</tr>" 	
				Response.Write "<SCRIPT language=JavaScript>UpdateBatchInfo(" & RegID & ", " & LastBatch & ");</SCRIPT>"
			end if				
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
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateBatch(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>
</form>


<p>&nbsp;</p>

</body>
</html>

<%end if%>