<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_Home2" Codebehind="Home2.aspx.cs" %>
<asp:Content width="900"  ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
<%@ register 
             Namespace="CambridgeSoft.COE.Framework.Controls.WebParts" assembly="CambridgeSoft.COE.Framework"
             TagPrefix="uc1"%>



    <table class="PagesContentTable">
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
            </td>
        </tr>
       <tr>
        <td><asp:Panel SkinID="HomePanel" wrap="true" runat="server" >  
        <asp:webpartmanager personalization-enabled="false" id="WebPartManager1" runat="server"/>
            <table>
                <tr>
                    <td  valign=top>
                            <asp:webpartzone  SkinID="WebPartZone" id="Col0" runat="server">
                                <zonetemplate>
                                   <uc1:HomeWebPart SkinID="HomeWebPart" hidden="true" ID="HomeWebPart0_0" runat="server" />
                                   <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_1" runat="server" />
                                    <uc1:HomeWebPart   SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_2" runat="server" />
                                     <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_3" runat="server" />
                                      <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_4" runat="server" />
                                       <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_5" runat="server" />
                                        <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_6" runat="server" />
                                        <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_7" runat="server" />
                                        <uc1:HomeWebPart  SkinID="HomeWebPart" hidden="true" ID="HomeWebPart0_8" runat="server" />
                                        <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_9" runat="server" />
                                        <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_10" runat="server" />
                                        <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_11" runat="server" />
                                        <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_12" runat="server" />
                                </zonetemplate>
                            </asp:webpartzone>
                   </td>
                   <td  valign=top>
                           <asp:webpartzone  SkinID="WebPartZone" id="Col1" runat="server">
                                <zonetemplate>
                                  <uc1:HomeWebPart  SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_0" runat="server" />
                                   <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_2" runat="server" />
                                     <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_3" runat="server" />
                                      <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_4" runat="server" />
                                       <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_5" runat="server" />
                                        <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_6" runat="server" />
                              </zonetemplate>
                            </asp:webpartzone>
                   </td>
                   <td  valign=top>
                            <asp:webpartzone SkinID="WebPartZone" id="Col2" runat="server">
                                <zonetemplate>
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_0" runat="server" />
                                   <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_2" runat="server" />
                                     <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_3" runat="server" />
                                      <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_4" runat="server" />
                                       <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_5" runat="server" />
                                        <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_6" runat="server" />
                                </zonetemplate>
                            </asp:webpartzone>
                    </td>
                   <td valign=top>
                            <asp:webpartzone  SkinID="WebPartZone" id="Col3" runat="server">
                                <zonetemplate>
                                     <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_0" runat="server" />
                                   <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_2" runat="server" />
                                     <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_3" runat="server" />
                                      <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_4" runat="server" />
                                       <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_5" runat="server" />
                                        <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_6" runat="server" />
                                </zonetemplate>
                            </asp:webpartzone>
                         
                    </td>
                    
                    <td  valign=top>
                            <asp:webpartzone SkinID="WebPartZone" id="Col4" runat="server">
                                <zonetemplate>
                                     <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_0" runat="server" />
                                   <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_2" runat="server" />
                                     <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_3" runat="server" />
                                      <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_4" runat="server" />
                                       <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_5" runat="server" />
                                        <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_6" runat="server" />
                                </zonetemplate>
                            </asp:webpartzone>
                         
                    </td>
                    
                    <td  valign=top>
                            <asp:webpartzone  SkinID="WebPartZone" id="Col5" runat="server" >
                                <zonetemplate >
                                        <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_0" runat="server" />
                                   <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_2" runat="server" />
                                     <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_3" runat="server" />
                                      <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_4" runat="server" />
                                       <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_5" runat="server" />
                                        <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_6" runat="server" />
                                </zonetemplate>
                            </asp:webpartzone>
                          </asp:panel>
                    </td>
                </tr>
            </table>
        </td>
       </tr>
    </table>
</asp:Content>

