<%@ Control Language="C#" AutoEventWireup="true" Inherits="Forms_UserControls_Footer" Codebehind="Footer.ascx.cs" %>
<div id="CBVFooter" class="CBVFooter">
    	<p class="Copyright"><%=Resources.Resource.CopyrightInfo%></p>
    	<p class="Version">CBOE Framework Version: <%=CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetFrameworkFileVersion()%></p>
    </div>