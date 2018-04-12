<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
	<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
	<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
	<SCRIPT LANGUAGE=javascript>
		function focusTextBox(){
			if (document.all.ContainerBarcode) document.all.ContainerBarcode.focus();
		}
		
		function clearTextBox(){
			if (document.all.ContainerBarcode) document.all.ContainerBarcode.value = "";
			focusTextBox();
		}
			
		function LookUpContainer(barcode){
			if (barcode =="") return;
			var strURL = serverType + serverName + "/cheminv/api/GetKeyContainerAttributes.asp?ContainerBarcode='" + barcode + "'";	
			var httpResponse = JsHTTPGet(strURL) 
			
			if (httpResponse.length == 0){
				alert("Could not a find a container for barcode= " + barcode);
			}
			else{
				var temp = httpResponse.split(",");
				    <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
				        var isOwner = GetContainerAuthority(barcode,document.form1.tempCsUserName.value);	
                         if (isOwner == "0")
                        {
                            alert("You are not owner of container having barcode " + barcode);
                        }
                        else
                        {
                            top.ListFrame.location.href = "multiscan_list.asp?Clear=0&AddContainerID=" + temp[0];
                        }
				    <% else  %>
				        top.ListFrame.location.href = "multiscan_list.asp?Clear=0&AddContainerID=" + temp[0];
				    <% end if %>    
			}
			clearTextBox();
		}
	</SCRIPT>
</HEAD>
<body onload="focusTextBox()">
<form name="form1" onsubmit="LookUpContainer(document.form1.ContainerBarcode.value); return false;">
<input TYPE="hidden" NAME="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" NAME="tempCsUserID" Value=<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp"))%>>
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>
			<a class="MenuLink" HREF="/cheminv/cheminv/BrowseInventory_frset.asp" target="_top" title="Browse chemical inventory by location">Browse</a>
			|
			<a class="MenuLink" HREF="/cheminv/inputtoggle.asp?dataaction=db&amp;dbname=cheminv" target="_top" title="Go to the main search screen">Search</a>
			|
			<a class="MenuLink" HREF="javascript:top.close();" target="_top" title="Return to the home page" >Home</a>
			|
			<a class="MenuLink" HREF="/cheminv/logoff.asp" target="_top">Log Off</a>
		</td>
	</tr>
</table>
<BR clear="all">
<center>
<table border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td align="right">
			Scan Container Barcode: <%=GetBarcodeIcon()%>&nbsp;
		</td>
		<td align="left">
			<input type="text" name="ContainerBarcode"  onchange="LookUpContainer(this.value); return false;">
		</td>
	</tr>
</table>
</center>
</form>
</body>
</html>
