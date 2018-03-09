<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_SecHome" Codebehind="Home.aspx.cs" MasterPageFile="~/Forms/Master/SecurityManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/SecurityManager.Master" %>
<%@ Register Namespace="CambridgeSoft.COE.Framework.Controls.WebParts" Assembly="CambridgeSoft.COE.Framework" TagPrefix="uc1"%>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
<!--[if lt IE 7]><style type="text/css" media="screen">@import url(/coemanager/app_themes/homeblue/ie6.css);</style><![endif]-->
<!--[if IE 7]><style type="text/css" media="screen">@import url(/coemanager/app_themes/homeblue/ie7.css);</style><![endif]-->
<asp:WebPartManager Personalization-Enabled="false" ID="WebPartManager1" runat="server"/>
<div id="wrapper" style="width:100%;">
    <div id="content" style="width:100%;">
        <img src="/COECommonResources/Utility_Images/BackGroundImages/PKI_FTB_Logo_RGB_small.jpg" alt="customer brand" class="custom_brand" style="margin-top:10px;margin-bottom:5px;float:left;display:inline"/>
        <p class="SecManagerInfo">Edit users and roles by selecting tasks below.  Available options are shown based on user permissions.</p>
        <div class="clear"></div>
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

