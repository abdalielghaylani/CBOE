<%@ Control Language="C#" AutoEventWireup="true" Inherits="Forms_Master_UserControls_Footer" Codebehind="Footer.ascx.cs" %>
<div id="Footer" class="PublicFooter">
	<p class="Copyright"><%=Resources.Resource.CopyrightInfo%></p>
	<p class="Version">CBOE Framework Version: <%=CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetFrameworkFileVersion()%></p>
</div>