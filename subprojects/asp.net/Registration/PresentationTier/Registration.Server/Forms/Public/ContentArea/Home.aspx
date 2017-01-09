<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="Home.aspx.cs" Inherits="PerkinElmer.CBOE.Registration.Client.Forms.Public.ContentArea.Home" %>
<%@Register   Namespace="CambridgeSoft.COE.Framework.Controls.WebParts" Assembly="CambridgeSoft.COE.Framework" TagPrefix="uc1"%>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="Server" >

<asp:webpartmanager personalization-enabled="false" id="WebPartManager1" runat="server"/>
<div id="wrapper">

	<div id="content">
      <p class="info"><asp:Literal id="MainTextLiteral" runat="server" ></asp:Literal> </p>
      
      <asp:Image ID="PE_Logo" runat="server" SkinID="CustomBrand" />
      
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
        <asp:webpartzone  SkinID="WebPartZone" id="Webpartzone1" runat="server">
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
    
     <div id="footer" class="Footer">
    	<p class="Copyright"><%=Resources.Resource.CopyrightInfo%></p>
    	<p class="Version"><asp:Literal runat="server" ID="FrameworkVersionLiteral" ></asp:Literal></p>
    	<p class="Version"><asp:Literal runat="server" ID="RegVersionLiteral" ></asp:Literal></p>
    </div>
</div>
</asp:Content>

