<%
'Set alt_IDDict = Session("Alt_IDDict")
'Set RequiredAlt_IDDict = Session("RequiredAlt_IDDict")
%>
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript">
<!--
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
	window.focus();
	function getStrucData(){
		document.form1.Structure.value = cd_getData("mycdx", "chemical/x-cdx");;
		//alert(document.form1.Structure.value)
		document.form1.action = "/cheminv/gui/CreateSubstance2.asp";
		ValidateSubstance();
	}
	
	function ValidateSubstance(){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// SubstanceName is required
		if (document.form1.SubstanceName.value.length == 0) {
			errmsg = errmsg + "- SubstanceName is required.\r";
			bWriteError = true;
		}
		
		<%For each key in req_alt_ids_dict%>
			if (document.form1["<%=key%>"].value.length == 0) {
				errmsg = errmsg + "- <%=req_alt_ids_dict.item(Key)%> is required.\r";
				bWriteError = true;
			}			
		<%Next%>
		
		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
//-->
</script>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<title>Create a new substance in <%=Application("appTitle")%></title>
</head>
<body>
<br><br><br>
<center>
<form name="form1" action method="POST">
	<table border="0" width="500" cellpadding="0" cellspacing="0">
		  <tr>
			<td>
				&nbsp;
			</td>
			<td colspan="2">
				<span class="GuiFeedback">Enter substance data below and click the &quot;Create Substance&quot; button.</span><br><br>
			</td>
		  </tr>
		  <tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td width="300" valign="top">
				<table border="1" cellpadding="0" cellspacing="0">
					<tr>
						<td valign="top">
<%
						if session("DrawPref") = "ISIS" then
							ISISDraw = " isisdraw=" & "'" & "True" &  "'"
						else
							ISISDraw = " isisdraw=" & "'" & "False" &  "'"
						end if	
%>						
							<script language="JavaScript">cd_insertObjectStr("<embed src='/CFWTEMP/cheminv/cheminvTemp/mt.cdx' width='280' height='200' id='5' name='mycdx' type='chemical/x-cdx'<%=ISISDraw%>>");</script>
						</td>
					</tr>
				</table>		
			</td>
			<td valign="top">
				<table border="0">
					<tr>
			        	<td>
							<span title="(eg. Acetonitrile)" class="required">Substance Name:</span><br>
							<input type="text" name="SubstanceName" SIZE="30" value maxlength="255">  
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(eg. 50-50-0)">CAS Registry#:</span><br>
							<input type="text" name="CAS" SIZE="30" value maxlength="15"> 
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<input type="text" name="ACX_ID" SIZE="30" value maxlength="15">
						</td>
					</tr>
					<%
					
					For each key in alt_ids_dict
						Response.Write "<TR><td>" & vblf
						if req_alt_ids_dict.Exists(Key) then
							requiredFields =  requiredFields & "," & Key & ";0:" & alt_ids_dict.Item(Key)
							Response.Write "<span class=""required"">"
						Else
							Response.Write "<span title="""">"
						end if
						Response.Write alt_ids_dict.Item(Key) & "</span><br>"
						Response.write "<input type=text name=""" & key & """ size=30>"
						Response.Write "</td></TR>" & vblf 
					Next 
					%>
		    	</table>
			</td>
		  </tr>
		  <tr>
			<td align="right" colspan="2">
				<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a><a HREF="Create%20a%20new%20substance" onclick="getStrucData(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt="Create a new substance for this container" WIDTH="61" HEIGHT="21"></a>
			</td>
		</tr>
		</table>
	<input type="hidden" name="add_order" value="inv_compounds">
	<input type="hidden" name="add_record_action" value="Duplicate_Search_No_Add">	
	<input type="hidden" name="commit_type" value="full_commit">	
	<input type="hidden" name="return_location" value>
	<input type="hidden" name="inv_compounds.Structure.sstype" value="1">
	<input type="hidden" name="ExactSearchFields" value="inv_compounds.Structure">
	<input type="hidden" name="RelationalSearchFields" value="inv_compounds.CAS;0,inv_compounds.ACX_ID;0,inv_compounds_ALT_ID_1;0">
	<input type="hidden" name="Structure" value>
</form>
</center>
</body>
</html>

