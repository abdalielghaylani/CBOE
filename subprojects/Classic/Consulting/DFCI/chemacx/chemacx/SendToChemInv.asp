<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
'SYAN modified on 3/31/2004 to support parameterized SQL
Dim Conn
Dim RS
Dim ConnStr
Dim oACXXML
Dim FormData
Dim ServerName
Dim bDebugPrint

bDebugPrint = false
Response.Expires = -1
PackageIDList = Request("PackageIDList")

ServerName = Request.ServerVariables("Server_Name")
FormData = "product=1&package=1&Synonym=1&structType=base64cdx&fieldName=PackageID&valueList=" & PackageIDList
' Use XMLHTTP to post against the ChemACX to get ACXXML
Set oXMLHTTP = GetXMLHTTP("POST", ServerName, "/chemacx/api/getXMLdata.asp", "ChemACX", FormData)
' Store the ACXXML returned by the server in a DOM object
Set oACXXML = oXMLHTTP.responseXML
oACXXML.setProperty "SelectionLanguage", "XPath"

' Create xsl transform tree
dim xslDoc
set xslDoc = Server.CreateObject("Msxml2.DOMDocument")
xslDoc.async = false
xslDoc.load(server.MapPath("ACXxml2ActionBatch.xsl"))

' Transform and return in response
xmlDoc.transformNodeToObject xslDoc, Response


'Response.ContentType = "text/xml"
'Response.Write oACXXML.xml
'Response.end

' Loop over the <package> nodes to produce
'For each substanceNode in oACXXML.documentElement.selectNodes("substance")
'	'Response.Write substanceNode.selectNodes("ancestor::substanceName").length & "<BR>"
'	Response.Write substanceNode.getAttribute("acxNum") & "<BR>"
'	Response.Write substanceNode.getAttribute("casNum") & "<BR>"
'	Response.Write Left(substanceNode.SelectSingleNode("structure/strucdata").text ,30) & "<BR>"
'	Response.Write
'	Response.Write "<BR>"
'Next

Response.End
GetRegConnection()
'sql= "SELECT r.reg_id, r.last_batch_number, a.identifier AS ChemicalName FROM Reg_Numbers r, Alt_ids a WHERE r.Reg_id = a.Reg_internal_id AND a.identifier_type = 0 AND r.Reg_ID IN(" & RegIDList & ") ORDER BY Reg_ID"
sql= "SELECT r.reg_id, r.last_batch_number, a.identifier AS ChemicalName FROM Reg_Numbers r, Alt_ids a WHERE r.Reg_id = a.Reg_internal_id AND a.identifier_type = 0 AND r.Reg_ID IN(" & BuildInClause(RegIDList) & ") ORDER BY Reg_ID"
sql_parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & RegIDList 'Name|Type|Direction|Size|Value
'Response.Write sql
'Response.end
'Set RS = Conn.Execute(sql)
Set RS = GetRecordSet(sql, sql_parameters)

' Response.Write RS.GetString(2,, " | ", "<BR>", "NULL")
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
	
	function ValidateBatch(){
		
		var RegNum;
		var size = "";
		var Initial = "";
		var UOMID = "";
		var errmsg = "Please fix the following problems:\r\r";
		var bWriteError = false;
		var blocalError;
		
		// Destination is required
		if (document.form1.LocationID.value.length == 0) {
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
		
		for (i=0; i< Container_arr.length; i++){
			blocalError =  false;
			RegNum = eval("document.all.RegNum_" + Container_arr[i] + ".innerHTML")
			size = eval("document.form1.ContainerSize_" +  Container_arr[i] + ".value");
			Initial = eval("document.form1.InitialAmount_" +  Container_arr[i] + ".value");
			UOMID = eval("document.form1.UOMID_" +  Container_arr[i] + ".value");
			errmsg = errmsg + "For Reg #" + RegNum + ":\r";
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
			
		
		if (bWriteError){
			alert(errmsg);
		}
		else{
			var xml = GetActionBatchXML();
			
			document.form1.ActionBatchXML.value = xml;
			//alert(document.form1.ActionBatchXML.value);
			document.form1.submit();
		}
	}
	
	function GetActionBatchXML(){
		var LocationID = document.form1.LocationID.value;
		var mydoc = new ActiveXObject("Msxml2.DOMDocument");
		// <CHEMINVACTIONBATCH>
		RootElm = mydoc.createElement("CHEMINVACTIONBATCH");
		mydoc.appendChild(RootElm);
		for (i=0; i< Container_arr.length; i++){
			BatchID = eval("document.form1.BatchNum_" + Container_arr[i] + ".value");
			RegID = Container_arr[i];
			BatchNumber = BatchID
			size = eval("document.form1.ContainerSize_" +  Container_arr[i] + ".value");
			Initial = eval("document.form1.InitialAmount_" +  Container_arr[i] + ".value");
			UOMID = eval("document.form1.UOMID_" +  Container_arr[i] + ".value");
			ContainerTypeID = eval("document.form1.ContainerTypeID_" +  Container_arr[i] + ".value");
			ChemicalName = document.all["ChemicalName_" + Container_arr[i]].title;
			//<CREATECONTAINER>
			ActionElm = mydoc.createElement("CREATECONTAINER");
			ActionElm.setAttribute("LocationID",LocationID);
			ActionElm.setAttribute("ContainerName",ChemicalName);
			ActionElm.setAttribute("MaxQty",size);
			ActionElm.setAttribute("InitialQty",Initial);
			ActionElm.setAttribute("UOMID",UOMID);
			ActionElm.setAttribute("ContainerTypeID",ContainerTypeID);
			ActionElm.setAttribute("ContainerStatusID",ContainerStatusID);
			//<OPTIONALPARAMS>
			OptParams= mydoc.createElement("OPTIONALPARAMS"); 
			//<REGBATCHID>
			OptElm = mydoc.createElement("REGID"); 
			OptElm.text = RegID;
			//<BATCHNUMBER>
			OptElm2 = mydoc.createElement("BATCHNUMBER"); 
			OptElm2.text = BatchNumber;
			// Asemble the tree
			ActionElm.appendChild(OptParams);
			OptParams.appendChild(OptElm);
			OptParams.appendChild(OptElm2);
			RootElm.appendChild(ActionElm);
		}
		return mydoc.xml;
	}
</script>
</head>
<body>
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top">
			<img src="../graphics/cheminventory_banner.gif" border="0" />
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
<table border="0">
	<tr>
		<th>Container</th>
		<th>Registry #</th>
		<th><span class="required">Batch #</span></th>
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
	RS.MoveFirst
	n = 0
	Do While NOT RS.EOF
		n = n + 1
		RegID = RS("Reg_ID")
		LastBatch =  RS("last_batch_number")
		Response.Write "<tr>"
		Response.Write "<td align=center>" & n & "</td>"
		Response.Write "<td><span id=""RegNum_" & RegID & """>&nbsp;</span></td>"
		Response.Write "<td align=right>" & GetNumSelector("BatchNum_" & RegID, CInt(LastBatch), 1, "onChange=""UpdateBatchInfo(" & RegID & ", this.value)""") & "</td>"
		Response.Write "<td align=right>" & truncateInSpan(RS("ChemicalName"),20, "ChemicalName_" & RegID) & "</td>"
		Response.Write "<td align=right><span id=""NoteBookName_" & RegID & """>&nbsp;</span></td>"
		Response.Write "<td align=right><span id=""NoteBookPage_" & RegID & """>&nbsp;</span></td>"
		Response.Write "<td align=right><span id=""Chemist_" & RegID & """>&nbsp;</span></td>"
		Response.Write "<td>" & ShowSelectBox("ContainerTypeID_" & RegID, 2,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>"
		Response.Write "<td><input type=text size=5 maxlength=5 name=""ContainerSize_" & RegID & """></td>"		
		Response.Write "<td>" & ShowSelectBox("UOMID_" & RegID, 6,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC") & "</td>"
		Response.Write "<td align=right><input type=text size=6 maxlength=5 name=""InitialAmount_" & RegID & """></td>"
		Response.Write "</tr>" 	
		Response.Write "<SCRIPT language=JavaScript>UpdateBatchInfo(" & RegID & ", " & LastBatch & ");</SCRIPT>"
		RS.MoveNext
	Loop
End if

%>
	<tr>
		<td colspan="10" align="right">
			<br> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0"></a><a HREF="#" onclick="ValidateBatch(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>
</form>


<p>&nbsp;</p>

</body>
</html>