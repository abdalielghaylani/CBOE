<%@ LANGUAGE="VBScript" %>

<script language="JavaScript">
	if  (top.main.location.href.indexOf("chemacx_form_frset.asp") == -1){
		top.main.location.href = "chemacx_form_frset.asp?<%=request.QueryString%>";
	}
	top.bannerFrame.productsShown = true;
	productsShown = true;
	theDisplayFrame = top.main.DisplayFrame;
	theMainFrame = top.main.mainFrame;
	top.bannerFrame.theDisplayFrame = top.main.DisplayFrame;
	top.bannerFrame.theMainFrame = top.main.mainFrame;
	var ShowMSDSLinks = <%=lcase(Application("SHOW_MSDX_LOOKUP_LINK"))%>
	var msdxURL = "<%=Application("msdxURL")%>";
	var msdxLinkText ="<%=Application("msdxLinkText")%>";
	var msdxLinkTitle = "<%=Application("msdxLinkTitle")%>";
	var msdxACXIDKeywd = "<%=Application("msdxACXIDKeywd")%>";
	var msdxCASKeywd = "<%=Application("msdxCASKeywd")%>";
	var msdxSupplierIDKeywd = "<%=Application("msdxSupplierIDKeywd")%>";
	var msdxSupplierCatNumKeywd = "<%=Application("msdxSupplierCatNumKeywd")%>";
	
</script> 
<!DOCTYPE html public "-//IETF//DTD HTML//EN">
<html>
	<head>
		<title>ChemACX Results-Form View</title>
		<script LANGUAGE="javascript" src="/chemacx/Choosecss.js"></script>
		<script LANGUAGE="JavaScript" SRC="/chemacx/chemacx/wddx.js"></script>
		<script LANGUAGE="JavaScript" SRC="/chemacx/chemacx/wddxRsEx.js"></script>
		<script LANGUAGE="JavaScript" SRC="/chemacx/chemacx/wddxDes.js"></script>
		<script language="JavaScript" src="/chemacx/chemacx/acxprod_display.js"></script>
		
		<!--#INCLUDE VIRTUAL = "/chemacx/chemacx/reg_utils.asp"-->
		<!--#INCLUDE VIRTUAL = "/chemacx/chemacx/msdx_utils.asp"-->
		<!--#INCLUDE FILE="AdoToWddx.js"-->
		<!--#INCLUDE FILE="../source/app_js.js"-->
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
		<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
        <style>
            .copyOverlay {
                bottom: 55px;
                left: 80px;
            }
        </style>
	</head>
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<%
If isArray(Session("Base_RS" & dbkey & formgroup)) then 
%>

	<body bgcolor="#ffffff" onUnload="theDisplayFrame.document.write('&lt;html&gt;&lt;body&gt;&lt;/body&gt;&lt;/html&gt;')">
	<!--#INCLUDE FILE="BuildProductRecordsets.asp"-->
	
<%
if  plugin_value  then
	displayType = "cdx"
    zoomFunction = "ACX_getStrucZoomBtn('Substance.BASE64_CDX'," & BaseID & ", 'SubstanceBASE64_CDX_" & BaseID & "_orig')"
else
	displayType = "SizedGif"
    zoomFunction = "ACX_getStrucZoomBtn('Substance.BASE64_CDX'," & BaseID & ", 'SubstanceBASE64_CDX_" & BaseID & "_orig',300,300)"
end if

if detectModernBrowser = true or displayType = "SizedGif" then
    TempCdxPath =Application("TempFileDirectoryHTTP" & "chemacx")
%>
        <div style="display: none">
            <script language="JavaScript">cd_insertObject("chemical/x-cdx", "100", "100", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", "", "true", <%=ISISDraw%>)</script>
        </div>
<%
end if
%>
	<center>
	<table border="1" cellpadding="0" cellspacing="0" width="210">
			<tr>
				<td colspan="2">			
					<table border="0" cellpadding="0" cellspacing="0" width="205">
						<tr>
							<td><script language="JavaScript">var isPlugin = "<%=plugin_value%>"; <%=zoomFunction%></script></td>
							<td><a href="Synonyms" onClick="openSynWindow(100,200, <%=BaseID%>,<%=BaseRunningIndex%>);return false"><img src="<%=Application("NavButtonGifPath")%>names.gif" alt border="0"></a></td>
							<td><a href="Table%20View" onclick="getAction('list_view');return false;"><img border="0" SRC="<%=Application("NavButtonGifPath")%>list_view_btn2.gif"></a></td>
							<td><script language="JavaScript">getMarkBtn(<%=BaseID%>)</script></td>
						</tr>
					</table>	
				</td>
			</tr>
			<tr>
			<td colspan="2" align="center"><font size="1" face="arial">Record #<%=BaseRunningIndex%></font></td>
			</tr>
			<tr>
				<td colspan="2" align="center" bgcolor="white" valign="middle">
					<%ShowCFWChemResult dbkey, formgroup, "Structure", "Substance.Structure", BaseID, displayType, 185, 130%>	
				</td>
			</tr>
			<tr>
				<td colspan="2" align="center">
					<% listItemNumber = listItemNumber +1%>
					<font size="1" face="Arial">
					<%
					if CBool(Application("RegisterFromACX")) AND (Application("RegServer")<> "NULL") then
						ShowSendtoRegLink(BaseID)
						Response.Write " | "
					End if
					if len(Trim(CAS))> 0 AND NOT IsNull(CAS) then
 						response.write "<strong>CAS#: " & CAS & "</strong></font>"
 						if CBool(Application("SHOW_MSDX_LOOKUP_LINK")) then		
								Response.Write " | "
								ShowMSDXLink ACX_ID, CAS, "", "", bHasMSDX
						End if 
					else
						response.write "<br><strong>CAS#: n/a</strong></font>"
					end if
					%>
					<br></font>
				</td>
			</tr>
			<tr>
				<td align="left" bgcolor="#FFFFD6">
				<strong><small>Formula:</small></strong>
				</td>
				<td bgcolor="#FFFFD6" colspan="3">
				<small>&nbsp;<%ShowCFWChemResult dbkey, formgroup, "Formula", "Substance.Formula", BaseID, "raw", 20, 20%></small>
				</td>
			</tr>
			<tr>
				<td align="left" bgcolor="#FFFFFF">
				<strong><small>MW:</small></strong>
				</td>
				<td bgcolor="#FFFFFF">
				<small>&nbsp;<%ShowCFWChemResult dbkey, formgroup, "MolWeight", "Substance.MolWeight", BaseID, "raw", 20, 20%></small>
				</td>
			</tr>
			<tr>
				<td align="left" bgcolor="#FFFFD6">
				<strong><small>ACX Number:</small></strong>
				</td>
				<td bgcolor="#FFFFD6" colspan="3">
				<small>&nbsp;<% ShowResult dbkey, formgroup, BaseRS, "Substance.ACX_ID", "raw", 0, 0%></small>
				</td>
			</tr>
			<tr>
				<td colspan="4" align="center">
					<table cellspacing="0" cellpadding="0" border="0">
						<tr>
							<td height="2">
							</td>
						</tr>
						<tr>
							<td align="right">
							</td>							
						</tr> 
						<tr>
							<td height="2">
							</td>
						</tr>
						<tr>
							<td valign="bottom" nowrap>
								<a href="Vendors" onClick="toggleTabs(1);document.cows_input_form.selectedList.value = 1;FillSelectFromWDDXPacket(SupplierList1);  return false"><img name="tab1" width="70" height="30" alt border="0" align="absbottom" vspace="0" hspace="0"><nobr></a><%if bLimitSearch = "0" OR blimitSearch = "" then%><a href="Others" onClick="toggleTabs(2);document.cows_input_form.selectedList.value = 2;FillSelectFromWDDXPacket(SupplierList2);  return false"><img name="tab2" width="68" height="30" alt border="0" align="absbottom" vspace="0" hspace="0"><nobr></a><a href="Vendors" onClick="toggleTabs(3);document.cows_input_form.selectedList.value = 3;FillSelectFromWDDXPacket(SupplierList3);  return false"><img name="tab3" width="70" height="30" alt border="0" align="absbottom" vspace="0" hspace="0"><nobr></a><%else%><a href="#" onclick="alert('You searched over a limited number of suppliers.  There may be more hits from other suppliers you excluded from the search.  To get all hits start a new search with out limiting the suppliers to search.');return false" onmouseover="status='';return true"><img src="../graphics/get_more_warn.gif" width="140" height="20" alt border="0" align="absmiddle"></a><%end if%>
							</td>
						</tr>
						<tr>
							<td valign="bottom" cellspacing="3" cellpadding="3">
							<table cellspacing="0" cellpadding="0" border="0" bgcolor="#FF3333">
							<tr>
							<td valign="top" align="left">
								<img src="../graphics/pixel.gif" width="208" height="10" alt border="0"><br>
								<select multiple name="supplierList" onchange="eval(productViewFunction +'(ListFromSelect(supplierList))')" size="5">
									<option><font face="Courier" size="+6">Chose Suppliers from the list:</font>
								</select>
								<br><img src="../graphics/pixel.gif" width="208" height="10" alt border="0"><br>
							</td></tr></table>
							</td>
						</tr>
						<tr><td height="2"></td></tr>
					</table>
					
				</td>
			</tr>
			<tr>
				<td colspan="2" align="center">
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td><a href="#" onclick="productViewFunction = 'DisplayBySupplierID';DisplayBySupplierID(ListFromSelect(supplierList));return false"><img src="../graphics/cat_view.gif" alt border="0" WIDTH="100" HEIGHT="16"></a></td>
							<td><a href="#" onclick="productViewFunction = 'DisplayBySupplierID_compact';DisplayBySupplierID_compact(ListFromSelect(supplierList));return false"><img src="../graphics/compact_view.gif" alt border="0" WIDTH="100" HEIGHT="16"></a></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
		<input type="hidden" name="selectedList">
		<script language="JavaScript">
			var numProducts = "<%= NumProducts%>";
			var numSuppliers = "<%= NumSuppliers%>";
			if (numProducts == ""){numProducts = "0"}
			if (numSuppliers == ""){numSuppliers = "0"} 
			</script>
				
			<%
			CloseRS(BaseRS)
			CloseRS(SynRS)
			CloseRS(PSynRS)
			CloseRS(SupRS1)
			CloseRS(SupRs2)
			CloseRS(SupRS3)
			CloseRS(ProdRS)
			%>
<%end if%>	
			<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
<%
If isArray(Session("Base_RS" & dbkey & formgroup)) then 
%>			
			
			<%
			'if plugin_value then
				'WriteAppletCode()
			'end if
			%>
			</center>


			<!--#INCLUDE FILE = "product_functions_js.asp"-->

			<script language="JavaScript">
				// Global Variables
				var productViewFunction = "DisplayBySupplierID";
				var SupplierList1 = "<%=SupplierList1_WDDX %>";
				var CheckACXCDROM = "Check ChemACX CD-ROM";
				var noSuppliersFoundTxt = ""
				var noSuppliersFound = <%=noSuppliersFound%>; 
				var numselected; // number of vendors selected from list
			<%if NOT (bLimitSearch = "0" OR blimitSearch = "") then%>
				//search is limited to a subset of users show a single tab
				var tab1_off = new Image();
				var tab1_on = new Image();
				tab1_on.src = '<%=Application("NavButtonGifPath")%>vendors_on.gif';
				tab1_off.src = '<%=Application("NavButtonGifPath")%>vendors_off.gif';
				document.cows_input_form.selectedList.value = 1;
				FillSelectFromWDDXPacket(SupplierList1);
				toggleTabs(1);
			<%else%>
				var tab1_off = new Image();
				var tab1_on = new Image();
				tab1_on.src = '<%=Application("NavButtonGifPath") & tab1gifname & "_on.gif"%>';
				tab1_off.src = '<%=Application("NavButtonGifPath") & tab1gifname & "_off.gif"%>';
				var tab2_off = new Image();
				var tab2_on = new Image();
				tab2_off.src = '<%=Application("NavButtonGifPath") & tab2gifname & "_off.gif"%>';
				tab2_on.src = '<%=Application("NavButtonGifPath") & tab2gifname & "_on.gif"%>';
				var tab3_off = new Image();
				var tab3_on = new Image();
				tab3_off.src = '<%=Application("NavButtonGifPath") & tab3gifname & "_off.gif"%>';
				tab3_on.src = '<%=Application("NavButtonGifPath") & tab3gifname & "_on.gif"%>';
				var SupplierList2 = "<%=SupplierList2_WDDX %>";
				var SupplierList3 = "<%=SupplierList3_WDDX %>";
				
				<% if nolist1 = 0 then 'First tab is not empty%>
					document.cows_input_form.selectedList.value = 1;
					FillSelectFromWDDXPacket(SupplierList1);
					toggleTabs(1);
				<% elseif nolist2= 0 then 'Second tab is not empty%>
					document.cows_input_form.selectedList.value = 2;
					FillSelectFromWDDXPacket(SupplierList2);
					toggleTabs(2);
				<% elseif nolist3= 0 then 'Third tab is not empty%>
					document.cows_input_form.selectedList.value = 3;
					FillSelectFromWDDXPacket(SupplierList3);
					toggleTabs(3);
				<% elseif noSuppliersFound = 1 then %>
					document.cows_input_form.selectedList.value = "NS";
					var noSuppliersFound = 1;
					FillSelectFromWDDXPacket(noSuppliersFoundTxt);
					toggleTabs(0);
				<% else%>
					document.cows_input_form.selectedList.value = "CD";
					var noSuppliersFound = 0;
					FillSelectFromWDDXPacket(CheckACXCDROM);
					toggleTabs(0);
				<%end if %>
			<%end if%>
			
				var currentNumSuppliers = document.cows_input_form.supplierList.options.length;
				var supplierSelect = "<%=Session("supplierSelect")%>";
				
				if (supplierSelect != ""){
					for (var i= 1; i< currentNumSuppliers; i++){
						if (document.cows_input_form.supplierList.options[i].value == supplierSelect){
							document.cows_input_form.supplierList.options[i].selected = 1;
						}
					}
					DisplayBySupplierID(ListFromSelect(document.cows_input_form.supplierList));
				}
				else if (currentNumSuppliers < 4){
					for (var i= 1; i< currentNumSuppliers; i++){
						document.cows_input_form.supplierList.options[i].selected = 1;
					}
					DisplayBySupplierID(ListFromSelect(document.cows_input_form.supplierList));
				}
				else{
					SelectSuppliersNotice()
				}
		</script>
	</body>
<%end if%>	
</html>

