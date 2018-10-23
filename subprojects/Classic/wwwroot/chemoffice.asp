<%@ LANGUAGE=VBScript %>

<!-- #include virtual="/cs_security/variables.asp" -->
<!-- #include virtual="/cs_security/functions.asp" -->

<%
' Redirect ultra users to cs_Security
'if bCS_SECURITY then Response.Redirect "/cs_security/home.asp"

PAGE_URL = "CambridgeSoft ChemOffice Enterprise"
PAGE_COLOR = color_blue
PAGE_APP = "header_" & color_blue & "_coent.gif"
TOP_NAV = "<a href=""http://www.cambridgesoft.com/services/topics.cfm?FID=6"" target=""_blank""><b>Technical Notes</b></a>"


Response.Expires = -1
Dim COXml
Dim oSections, oSection
Dim fpath, sectionName, sectionCaption, sectionPrivilege

' Cache the xml representation of the ChemOffice entry page in Applcation scope
if isObject(Application("COXml")) then 
	Set COXml = Application("COXml")
else	
	Set COXml = Server.CreateObject("Microsoft.FreeThreadedXMLDOM")
	COXml.async = false
	on error resume next
	fpath = server.MapPath("/chemoffice/config/chemofficeASP.xml")
	COXml.load(fpath)
	'Check for parser errors
	If COXml.parseError.errorCode <> 0 Then 
		Application("COXml") = ""
		Response.Write reportParseError(COXml.parseError)
	else  
		Set Application("COXml") = COXml
	end if
End if
%>

<!-- #include virtual="/cs_security/header.asp" -->

<form name="form1">
<table border="0" width="100%" style="border-collapse: collapse">
	<tr>
	<%
	Set oSections = COXml.documentElement.selectNodes("section")
	i=0
	For each oSection in oSections
		i= i + 1
		isEven = false
		sectionName =  oSection.getAttribute("name")
		sectionCaption = oSection.getAttribute("caption")
		sectionPrivilege = oSection.getAttribute("privilege")
		
		bSectionPriv = CheckPrivilege(sectionPrivilege)
		
		Response.Write "<td valign=""top"" width=""50%"">"
		Response.Write renderBoxBegin (sectionCaption,"")
		
		if bSectionPriv then 
			Set oLines = oSection.selectNodes("line")			
			For each oLine in oLines
				Response.write renderLineCaption(oLine.getAttribute("caption"),"SmallCaption")
				Set oLinks = oLine.selectNodes("link")
				linkCount = oLinks.length
				j = 0 
				For each oLink in oLinks
					
					addPipe = false
					linkDisplay = oLink.getAttribute("display")
					linkUrl = oLink.getAttribute("url")
					linkTip = oLink.getAttribute("tip")
					linkPopup = oLink.getAttribute("popup")
					linkPrivilege = oLink.getAttribute("privilege")
					
					bLinkPrivilege = CheckPrivilege(linkPrivilege)
					
					if bLinkPrivilege then
						j = j + 1
						Response.write RenderLink(linkDisplay, linkTip, linkUrl, linkPopUp, j > 1, "HomeLink")
					End if		
				Next
				Response.Write "<BR/>"  
			Next
			Set oCOiniApps = oSection.selectNodes("chemofficeiniApps")
			For each oCOiniAppType in oCOiniApps
				appType = oCOiniAppType.getAttribute("apptype")
				linkListCaption = oCOiniAppType.getAttribute("caption")
				useDropdownThreshold = oCOiniAppType.getAttribute("useDropdownThreshold")
				if isNull(useDropdownThreshold) then useDropdownThreshold = 0
				GetApps appType, linkListCaption, useDropdownThreshold
			Next
		end if	
		Response.Write renderBoxEnd 
		if (i mod 2) = 0 then isEven = true
		
		if isEven then 
			Response.Write "</td></tr><tr><td colspan=""2"">&nbsp;</td></tr>"
		else
			Response.Write "</td><td width=""20"">&nbsp;&nbsp;&nbsp;&nbsp;</td>"
		end if
	Next
		if not isEven then Response.Write "<td valign=""top"" width=""50%"">&nbsp;</td></tr><tr><td colspan=""2"">&nbsp;</td></tr>"
	%>
</table>
<br/>
</form>
<!-- #include virtual="/cs_security/footer.asp" -->
