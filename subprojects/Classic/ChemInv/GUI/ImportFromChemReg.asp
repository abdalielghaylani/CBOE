<%@ Language=VBScript %>
<%
Response.Expires = 3
RegIDList = Request("RegIDList")

Dim Conn
Dim RS
Dim ConnStr
Dim rsStatus
Dim sOpenAsModalFrame

sOpenAsModalFrame = Request("OpenAsModalFrame")
if trim(sOpenAsModalFrame) = "" then sOpenAsModalFrame = "false"

'-- CSBR ID:132599
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Set the default location as per the setting/selection
'-- Date: 11/02/2010
LocationID = Request.Form("LocationID")
if Not IsEmpty(LocationID) then 
	Session("DeliveryLocationID")= LocationID
else
	Session("DeliveryLocationID") = GetUserProperty(Session("UserNameCheminv"), "INVContainerOrderDeliveryLoc")
	' Fall back to Default location user setting
	if Session("DeliveryLocationID") = "" then
	Session("DeliveryLocationID") = GetUserProperty(Session("UserNameCheminv"), "INVDefLoc")
	end if
	' Fall back to application default delivery location
	if Session("DeliveryLocationID") = "" then
		Session("DeliveryLocationID") = Application("DEFAULT_CONTAINER_ORDER_DELIVERY_LOCATION")
	end if
end if
LocationID = Session("DeliveryLocationID")
'End of Change #132599#
GetRegConnection()
'JHS 8/3/2009 - Notes for Reg 11.0.1
'change to use inv views
'sql= "SELECT r.reg_id, Max(r.last_batch_number) as last_batch_number, Max(a.identifier) AS ChemicalName FROM Reg_Numbers r, Alt_ids a WHERE r.Reg_id = a.Reg_internal_id(+) AND a.identifier_type(+) = 0 AND r.Reg_ID IN(" & RegIDList & ") GROUP BY Reg_ID ORDER BY Reg_ID"
sql= "SELECT r.regid as REG_ID, Max(r.batchnumber) as last_batch_number, Max(REGNAME) AS ChemicalName FROM INV_VW_REG_BATCHES r WHERE  r.RegID IN(" & RegIDList & ") GROUP BY r.RegID ORDER BY r.RegID"
'Response.Write sql
'Response.end
Set RS = Conn.Execute(sql)
' Response.Write RS.GetString(2,, " | ", "<BR>", "NULL")
'GetInvConnection()
'SQL = "SELECT container_status_id FROM inv_container_status WHERE container_status_name = 'In Use'"
'Set rsStatus = Conn.Execute(SQL)
'defaultStatusID = rsStatus("container_status_id")
'This was running a query even though it was hardcoded.  'Lets just hardcode the id then use the app variable if it is there
defaultStatusID = Application("DefaultRegContainerStatus") ' Hardcode it to the application variable where the default will be 9 in the global.asa but 1 in the ini
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
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
	var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
	var tempCSUserID= "<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>";
	
	function CloseModal()
    {
	if((navigator.userAgent.indexOf("MSIE") != -1 ) || (!!document.documentMode == true )){
	window.parent.CloseModal();}
	else
	window.close();
    }

	function ValidateBatch(){

		/*
		CSBR ID : 59253 
        Date : 04-Feb-2010
        Changed by :Soorya Anwar        
        */
        var strContainerName = ""; //Adding new variable for storing Container Name
        //end of Change for CSBR# 59253		        
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
		<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		 !document.form1.Ownershiplst ? document.form1.PrincipalID.value="" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
	     if(GetAuthorizedLocation(document.form1.LocationID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not authorized for this location.\r";
                bWriteError = true;
            }
		//Container Admin is required
		if (document.form1.PrincipalID.value.length == 0) {
			errmsg = errmsg + "- Container Admin is required.\r";
			alert(errmsg);
			return false;
		}
		<% end if %>
		
		for (i=0; i< Container_arr.length; i++){
			blocalError =  false;
			RegNum = eval("document.all.RegNum_" + Container_arr[i] + ".innerHTML")
			size = eval("document.form1.ContainerSize_" +  Container_arr[i] + ".value");
			Initial = eval("document.form1.InitialAmount_" +  Container_arr[i] + ".value");
			UOMID = eval("document.form1.UOMID_" +  Container_arr[i] + ".value");
			/*
		    CSBR ID : 59253 
            Date : 04-Feb-2010
            Changed by :Soorya Anwar            
            */
            strContainerName = eval("document.form1.ContainerName_" +  Container_arr[i] + ".value");//Setting Container Name
            //end of Change for CSBR# 59253		
			errmsg = errmsg + "For Reg #" + RegNum + ":\r";
			if (size.length == 0) {
				errmsg = errmsg + "   - Size is required.\r";
				bWriteError = true;
				blocalError = true
			}
			else{
				 //size must be a number
				if (!isPositiveNumber(size)){
					errmsg = errmsg + "   - Size must be a positive number.\r";
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
			/*
		    CSBR ID : 59253 
            Date : 04-Feb-2010
            Changed by :Soorya Anwar
            Purpose : Adding ContainerName variable for validating ContainerName field
            */     
            //Container Name should not be blank                   
			if (strContainerName.length == 0){
			    errmsg = errmsg + "   - Container Name is required.\r";
			    bWriteError = true;
			    blocalError = true;
			}
			//end of Change for CSBR# 59253		
			if (!blocalError) errmsg +=  "   -No errors.\r"
		}
        <%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		 if (!bWriteError && document.form1.LocationAdmin.value!='' && document.form1.LocationAdmin.value !=document.form1.PrincipalID.value)
            {
                if (confirm("- The Location Admin and Container Admin are not the same,\r Do you really want to continue?")!=true)
                    {     
                        return;
                     }
            }	
		<%end if%>
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
		var mydoc;
		if (window.DOMParser) {
            mydoc = document.implementation.createDocument("", "", null);
        }
        else // Internet Explorer
        {
            mydoc = new ActiveXObject("MSXML2.DOMDocument");
        }
		
		// <CHEMINVACTIONBATCH>
		var RootElm = mydoc.createElement("CHEMINVACTIONBATCH");
		RootElm.setAttribute("FromReg","true");
		mydoc.appendChild(RootElm);
		for (i=0; i< Container_arr.length; i++){
			BatchID = eval("document.form1.BatchNum_" + Container_arr[i] + ".value");
			RegID = Container_arr[i];
			BatchNumber = BatchID
			size = eval("document.form1.ContainerSize_" +  Container_arr[i] + ".value");
			Initial = eval("document.form1.InitialAmount_" +  Container_arr[i] + ".value");
			UOMID = eval("document.form1.UOMID_" +  Container_arr[i] + ".value");
			ContainerTypeID = eval("document.form1.ContainerTypeID_" +  Container_arr[i] + ".value");
			ChemicalName = eval("document.all.ChemicalName_" + Container_arr[i] + ".innerHTML");			
			ChemicalName = ChemicalName.replace("&nbsp;", "");
			/*' CSBR ID : 59253
	        ' Date : 04-Feb-2010
	        ' Changed by :Soorya Anwar	        
	        */
			strContainerName = eval("document.all.ContainerName_" + Container_arr[i] + ".value");						
			//end of Change for CSBR# 59253		
			UOMName = eval("document.form1.UOMID_" +  Container_arr[i] + "[document.form1.UOMID_" +  Container_arr[i] + ".selectedIndex].text");
			ContainerTypeName = eval("document.form1.ContainerTypeID_" +  Container_arr[i] + "[document.form1.ContainerTypeID_" +  Container_arr[i] + ".selectedIndex].text");		
			//<CREATECONTAINER>
			ActionElm = mydoc.createElement("CREATECONTAINER");
			ActionElm.setAttribute("UOMName",UOMName);
            ActionElm.setAttribute("ContainerTypeName",ContainerTypeName);
			ActionElm.setAttribute("LocationID",LocationID);
			ActionElm.setAttribute("MaxQty",size);
			ActionElm.setAttribute("InitialQty",Initial);
			ActionElm.setAttribute("UOMID",UOMID);
			ActionElm.setAttribute("ContainerTypeID",ContainerTypeID);
			ActionElm.setAttribute("ContainerStatusID",ContainerStatusID);
			<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
			ActionElm.setAttribute("PrincipalID",document.form1.PrincipalID.value);
			<% end if %>
			ActionElm.setAttribute("OpenAsModalFrame",document.form1.OpenAsModalFrame.value);
			//<OPTIONALPARAMS>
			OptParams= mydoc.createElement("OPTIONALPARAMS"); 
			//<REGBATCHID>
			OptElm = mydoc.createElement("REGID"); 
			OptElm.appendChild(mydoc.createTextNode(RegID));

			//<BATCHNUMBER>
			OptElm2 = mydoc.createElement("BATCHNUMBER"); 
			OptElm2.appendChild(mydoc.createTextNode(BatchNumber));

			//<CONTAINERNAME>
			OptElm3 = mydoc.createElement("CONTAINERNAME"); 
			OptElm3.appendChild(mydoc.createTextNode(strContainerName));
			
			//<CONTAINERSTATUSID>
			OptElm4 = mydoc.createElement("CONTAINERSTATUSID");
			OptElm4.appendChild(mydoc.createTextNode('<%=defaultStatusID%>'));
			
			// Asemble the tree
			ActionElm.appendChild(OptParams);
			OptParams.appendChild(OptElm);
			OptParams.appendChild(OptElm2);
			OptParams.appendChild(OptElm3);
			OptParams.appendChild(OptElm4);
			RootElm.appendChild(ActionElm);
		}
		if ( window.ActiveXObject )
		{
			return mydoc.xml;
		}
		else if ( window.XMLSerializer )
		{
			return (new XMLSerializer()).serializeToString(mydoc);
		}
	}
</script>
</head>
<body>
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top">
			<img src="../graphics/cheminventory_banner.gif" border="0" / WIDTH="250" HEIGHT="48">
		</td>
		<td align="right">
			<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
		</td>
	</tr>
</table>
<%If not Session("INV_CREATE_CONTAINER" & "cheminv")  then%>
<br><br><br><br>
<table ALIGN="CENTER" BORDER="1" CELLPADDING="0" CELLSPACING="0" BGCOLOR="#ffffff" Width="90%">
	<tr>
		<td>
			<center><span class="GuiFeedback">This action requires Create Container privilege.<br>Please contact your system administrator.</span></center>
			<p><center><a HREF="3" onclick="window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a></center>
		</td>
	</tr>
</table>
<%else%>
<form name="form1" action="/Cheminv/api/DisplayActionBatch.asp" method="POST">
	<input type="hidden" name="ActionBatchXML">
	<input type="hidden" name="OpenAsModalFrame" value='<%=sOpenAsModalFrame%>'/>
	<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
	<input type="hidden" name="OwnerShipGroupList" id="OwnerShipGroupList"  value='<%=GetOwnerShipGroupList()%>'/>
    <input type="hidden" name="OwnerShipUserList" id="OwnerShipUserList"  value='<%=GetOwnerShipUserList()%>'/>
    <input type="hidden" name="PrincipalID" value/>
    <input type="hidden" name="OwnershipType" value />
    <input TYPE="hidden" NAME="LocationAdmin" id="LocationAdmin" Value="<%=LocationAdmin%>">
    <% end if %>
    <input type="hidden" name="tempCsUserName" id="tempCsUserName" value="<%=Session("UserName" & "cheminv")%>" />
    <input type="hidden" name="tempCsUserID" id="tempCsUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>" />
<table border="0">
<tr>
	<td align="right" nowrap>
		<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
	</td>
	<td>
		<%
            if Application("ENABLE_OWNERSHIP")="TRUE" then
                authorityFunction = "SetOwnerInfo('location');"
           else
                authorityFunction= ""
            end if
'-- CSBR ID:132599
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Should use version 2 of ShowLocationPicker (The same was used in ImportFromChemACX also)
'-- Date: 11/02/2010
		ShowLocationPicker9 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 49, false, LocationID, authorityFunction
'End of Change #132599#
		%> 
	</td>
</tr>
<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
<tr>
		<td  align=right> <span class="required" title="Pick an option from the list">Container Admin:</span></td>
		<td  align=left>
		<table border=.5>
		<tr><td><input type="Radio" name="Ownership" name="User_cb" id="User_cb" onclick="SetOwnerInfo('user');"/>by user</td>
		<td><input type="Radio" name="Ownership" name="Group_cb" id="Group_cb" onclick="SetOwnerInfo('group');" />by Group</td></tr>
		<tr><td colspan="2"><SELECT id="Ownershiplst" ><OPTION></OPTION></SELECT></td></tr></table></td>
	</tr>	
<% end if %>	
</table>
<p><span class="GUIFeedback">Specify the type, size and initial amount of containers to be created</span></p>
<table border="0">
	<tr>
		<th>Container</th>
		<th>Registry #</th>
		<th><span class="required">Batch #</span></th>
		<th>Chemical Name</th>
		<!-- CSBR ID :59253
		     Date : 03-Feb-2010
		     Changed by : Soorya Anwar
		     Purpose of Change : To Add a new Container Name as Header row
		-->
		<th><span class="required">Container Name</span></th>
		<!-- end of Change for CSBR # 59253-->
		<th>Notebook</th>
		<th>Page</th>
		<th>Chemist</th>
		<th><span class="required">Type</span></th>
		<th><span class="required">Size</span></th>		
		<th><span class="required">Amount</span></th>
		<th><span class="required">UOM</span></th>
	</tr>
<%
If RS.BOF AND RS.EOF then
	Response.Write "Error: No rows returned"
Else
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
	    ' CSBR ID : 59253
	    ' Date : 03-Feb-2010
	    ' Changed by :Soorya Anwar
	    ' Purpose : To add an editable text field, to display the Container Name    
	    Response.Write "<td align=right><input type=text size=20 maxlength=255 name=""ContainerName_" & RegID & """ value = """ & RS("ChemicalName") & """></td>"
	    ' end of Change for CSBR # 59253
		Response.Write "<td align=right><span id=""NoteBookName_" & RegID & """>&nbsp;</span></td>"
		Response.Write "<td align=right><span id=""NoteBookPage_" & RegID & """>&nbsp;</span></td>"
		Response.Write "<td align=right><span id=""Chemist_" & RegID & """>&nbsp;</span></td>"
		Response.Write "<td>" & ShowSelectBox("ContainerTypeID_" & RegID, 2,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & "</td>"
		Response.Write "<td><input type=text size=5 maxlength=5 name=""ContainerSize_" & RegID & """></td>"				
		Response.Write "<td align=right><input type=text size=6 maxlength=5 name=""InitialAmount_" & RegID & """></td>"
        Response.Write "<td>" & ShowSelectBox("UOMID_" & RegID, 6,"SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC") & "</td>"
		Response.Write "</tr>" 	
		Response.Write "<SCRIPT language=JavaScript>UpdateBatchInfo(" & RegID & ", " & LastBatch & ");</SCRIPT>"
		RS.MoveNext
	Loop
End if

%>
	<tr>
		<td colspan="10" align="right">
			<br> 
			<a HREF="#" onclick="CloseModal(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateBatch(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>
</form>
<% end if%>
<% if Application("ENABLE_OWNERSHIP")="TRUE" and PrincipalID="" then %>
<script language="javascript">
    //set the inital location group
    SetOwnerInfo('location');
 </script>
<%end if %>
<p>&nbsp;</p>

</body>
</html>
