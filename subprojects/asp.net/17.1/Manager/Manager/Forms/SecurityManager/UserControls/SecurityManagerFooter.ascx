<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityManagerFooter.ascx.cs" Inherits="Manager.Forms.SecurityManager.UserControls.SecurityManagerFooter" %>
<div id="SecurityManagerFooter" class="SecurityManagerFooter">
	<p class="Copyright"><%=Resources.Resource.CopyrightInfo%></p>
	<p class="Version">CBOE Framework Version: <%=CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetFrameworkFileVersion()%></p>
</div>