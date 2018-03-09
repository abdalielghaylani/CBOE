<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataviewManagerFooter.ascx.cs" Inherits="Manager.Forms.DataViewManager.UserControls.DataviewManagerFooter" %>
<div id="DataviewManagerFooter" class="DataviewManagerFooter">
    	<p class="Copyright"><%=Resources.Resource.CopyrightInfo%></p>
    	<p class="Version">CBOE Framework Version: <%=CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetFrameworkFileVersion()%></p>
    </div>