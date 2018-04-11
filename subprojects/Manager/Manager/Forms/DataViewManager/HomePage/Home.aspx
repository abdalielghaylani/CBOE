<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_DVHome" Codebehind="Home.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.Master" %>
<%@ Register Namespace="CambridgeSoft.COE.Framework.Controls.WebParts" Assembly="CambridgeSoft.COE.Framework" TagPrefix="uc1" %>

<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
<!--[if lt IE 7]><style type="text/css" media="screen">@import url(/coemanager/app_themes/homeblue/ie6.css);</style><![endif]-->
<!--[if IE 7]><style type="text/css" media="screen">@import url(/coemanager/app_themes/homeblue/ie7.css);</style><![endif]-->
<asp:WebPartManager Personalization-Enabled="false" ID="WebPartManager1" runat="server"  />
<div id="wrapper">
    <div id="content">
      <p class="info">Create or edit a dataview using the options below.   The available options are based on your user permissions.</p>
      <img src="/COECommonResources/Utility_Images/BackGroundImages/PKI_FTB_Logo_RGB_small.jpg" alt="customer brand" class="custom_brand" />
        <asp:webpartzone  SkinID="WebPartZone" id="Panel" runat="server">
            <zonetemplate>
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="PanelWebPart0_0" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="PanelWebPart0_1" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="PanelWebPart0_2" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="PanelWebPart0_3" runat="server" />
            </zonetemplate>
        </asp:webpartzone>
        <div class="clear"></div>

    </div>
    
    <div id="dashboard">
        <asp:webpartzone  SkinID="WebPartZone" id="DashBoard" runat="server">
            <zonetemplate>
               <uc1:HomeWebPart  SkinID="HomeWebPart" hidden="true" ID="DashWebPart0_1" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_2" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_3" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_4" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_5" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_6" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_7" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_8" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_9" runat="server" />
               <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="DashWebPart0_10" runat="server" />
            </zonetemplate>
        </asp:webpartzone>
         <div class="clear"></div>
    </div>
</div>
</asp:Content>

