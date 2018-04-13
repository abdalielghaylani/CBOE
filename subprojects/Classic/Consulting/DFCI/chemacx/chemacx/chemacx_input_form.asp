<%@ LANGUAGE=VBScript %>
<!--#INCLUDE FILE = "../AnonymousSearchCounterFunctions.asp"-->
<%
' CSWebUser Authentication
Session("ShowContent") = Session("okOnService_1")

' Allows anonymous searches before forcing CSWebUsers authentication
NumSearchesAllowed =  5 ' Number of anonymous searches allowed
if Not Session("ShowContent") then
	Session("ShowContent") = AllowAnonymousSearch(ServiceID, NumSearchesAllowed)	
	if bdebugPrint then Response.Write "ShowContent= " & Session("ShowContent") &"<BR>"
End if

if lcase(Request("formmode"))= "refine" then 
	buttonName="apply_btn.gif"
else
	buttonName="search_btn.gif"
end if
%>
<%
'Copyright CambridgeSoft Corporation 1998-2001 all rights reserved	
Response.expires= -1
%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<script language="JavaScript">
			var needToAuthenticate = <%=lcase(Not Session("ShowContent"))%>
			DoAfterOnLoad = true
			var serverName = "http://<%=Request.ServerVariables("HTTP_HOST")%>";
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
			function DoAfterOnLoad(){top.bannerFrame.reload()}
		</script>
		<script LANGUAGE="javascript" src="/chemacx/Choosecss.js"></script>
		<title>ChemACX Structure Query Form</title>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<!--#INCLUDE FILE="../source/secure_nav.asp"-->
		<!--#INCLUDE FILE="../source/app_js.js"-->
		<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	</head>
	<body bgcolor="#FFFFFF">
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
	
		<table border="0" width="500" cellpadding="0" cellspacing="0">
		  <%
		  'SYAN changed on 4/7/2004 for global search form
		  'if Application("DBMS")= "ORACLE"  then
		  if Application("DBMS" & dbkey)= "ORACLE"  then
				SynTableName ="ACX_Synonym.Name" 
		  else
				SynTableName ="Synonym.Name"
		  End if
		  %>
		  <% if Ucase(Application("NP_search")) = "TRUE" then%>
		  <tr>
		  <td colspan="3" align="right">
		  <%if Instr(Application("DBNames"), ",") > 0 then%>
			<%if request("formgroup") = "gs_form_group" then%>
				<a href="<%=Application("AppPathHTTP")%>/default.asp?formgroup=base_form_group&dataaction=db&dbname=chemacx" " target="_top">Single DB Search</a> 
			<%else%>
				<a href="<%=Application("AppPathHTTP")%>/default.asp?formgroup=gs_form_group&dbname=chemacx" " target="_top">Global Search</a> 
			<%end if%>
			&nbsp;&nbsp;&nbsp;&nbsp;
		  
		  <%end if%>	
		  <a href="<%=Application("AppPathHTTP")%>/<%=Application("NPinputForm")%>" " target="_top"><img src="../graphics/for_non_plugin.gif" alt="Simple text search" border="0" WIDTH="174" HEIGHT="16"></a> 
		  </td>
		  </tr>
		  <% end if%>
		  <tr>
		  	<td rowspan="3">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td>
				<table border="0">
					<tr>
						<td>
							<%'pjd=getGetSearchOptions(dbkey, formgroup, "Substance.Structure", "AllOptions", "SelectList")%>
						</td>
						<td valign="bottom" align="right">
							<input type="image" src="/chemacx/graphics/<%=buttonName%>" border="0">&nbsp;
						</td>
					</tr>
					<tr>
		<!--pjd moved	        	<td valign="top" align="left" colspan="2">
							<%'pjd ShowStrucInputField  dbkey, formgroup, "Substance.Structure", "1",330, 250, "AllOptions", ""
							ShowStrucInputField  dbkey, formgroup, "Substance.Structure", "1",0, 0, "AllOptions", ""
							%>
						</td>  pjd-->
		      		</tr>
				</table>			
			</td>
			<td valign="bottom">
				<table border="0">
					<tr>
			        	<td nowrap>
							<span title="(e.g. Acetonitrile)"><!--pjd Substance pjd--> Drug Name:</span><br>
							<%'SYAN changed on 4/7/2004 for global search form%>
							<%'ShowInputField dbkey, formgroup, SynTableName, "0","30"%>    
							<%if LCase(formgroup) = "gs_form_group" then%>
								<%ShowInputField dbkey, formgroup, "ACX_Synonym.Name", "0","30"%>    
							<%else%>
								<%ShowInputField dbkey, formgroup, SynTableName, "0","30"%>    
							<%end if%>    
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(e.g. 50-50-0)"><!--pjd CAS Registry#: pjd-->NDC</span><br>
							<%ShowInputField dbkey, formgroup, "Substance.CAS", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)"><!--pjd ACX Number: pjd-->Catalog Number<br>
							<%ShowInputField dbkey, formgroup, "Substance.ACX_ID", "0","30"%>
						</td>
					</tr>
<!--pjd		      		<tr>
		        		<td>
							<span title="(e.g. C8H3Cl3O, C1-5O&lt;3F0-1)">Molecular Formula:<br>
							<%ShowInputField dbkey, formgroup, "Substance.Formula", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="&lt;120, 120-130, &gt;250">MolWeight Range:</span><br>
							<%ShowInputField dbkey, formgroup, "Substance.MolWeight", "0","30"%>
						</td>
		      		</tr>
			  		<tr>
			  			<td>
							<span title="e.g. ARC-1134">Catalog Number</span><br>
							<%ShowInputField dbkey, formgroup, "Product.CatalogNum", "0","30"%>
						</td>
			  		</tr>
		    	</table>
			</td>
		  </tr> pjd-->
		  <tr>
		  	<td align="left">
		  		<%Session("suppliersTosearch") = Replace(request.cookies("acxprefsuplist"), ":",",")%>
				<input type="hidden" name="Product.SupplierID" Value="<%=Session("suppliersTosearch")%>">
				<input type="hidden" name="Substance.HasProducts" Value="-1">
				<!--pjd add -->
				<input type="hidden" name="limitSearch" Value="0">
				<!--pjd end -->
				<!--pjd	<input type="radio" name="limitSearch" value="0" onclick="writeLimitCookie('0',false)">Search all vendors.<br>
				<input type="radio" name="limitSearch" value="1" onclick="writeLimitCookie('1',true)">Search your favorite vendors.<br> pjd-->
			</td>
			<td align="right" valign="bottom"><!--pjd	
				<a href="Select%20favorite%20vendors" onclick="editFavoriteVendorsList();return false"><img src="../graphics/edit_fav_vendors.gif" alt="View or edit your favorite vendors list" border="0" align="absmiddle" WIDTH="174" HEIGHT="16"></a>
			pjd--></td>
		  </tr>
		  <tr>
		  <td colspan="2" align="left">
		<!--pjd	<input type="checkbox" name="ShowAllSubstances" onclick="(this.checked) ? document.cows_input_form[&quot;Substance.HasProducts&quot;].value = &quot;&quot;:  document.cows_input_form[&quot;Substance.HasProducts&quot;].value = &quot;-1&quot;;">Display all substances even if no vendors are found. pjd--> 
			<input type="hidden" name="ShowAllSubstances" value="1">
		</td>
		  </tr>
			        <tr>	<td valign="top" align="left" colspan="2">
							<%'pjd ShowStrucInputField  dbkey, formgroup, "Substance.Structure", "1",330, 250, "AllOptions", ""
							ShowStrucInputField  dbkey, formgroup, "Substance.Structure", "1",0, 0, "AllOptions", ""
							%>
						</td>
		  </tr>
					</table>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
		<script language="JavaScript">
			var limitflag = false;
			var limit = ReadCookie("acxlimitsearch");
			
			if (limit == "1"){
				document.cows_input_form.limitSearch[1].checked=true;
				writeLimitCookie('1',true)
			}
			else{
				document.cows_input_form.limitSearch[0].checked=true;
				writeLimitCookie('0',false)
			}
		</script>
	</body>
</html>