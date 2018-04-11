<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_Home" Codebehind="Home.aspx.cs" MasterPageFile="~/Forms/Master/MasterPage.master" Theme="Blue" %>
<%@ MasterType VirtualPath="~/Forms/Master/MasterPage.master" %>
<%@ Register Namespace="CambridgeSoft.COE.Framework.Controls.WebParts" assembly="CambridgeSoft.COE.Framework" TagPrefix="uc1"%>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >

<script language="javascript" type="text/javascript">
if (window.name != "" && self.opener != null)
{
    self.opener = this;
    self.close();
}
</script>

    <div id="wrapper" class="PublicContentContainer">
        <div id="content">
            <p class="info">Welcome to ChemBioOffice Enterprise.  To begin, select a task below.<br /><br />
            Each module in the suite offers its own contributions to the research and development cycle including unique compound registration, chemical substance procurement, and inventory management of containers and plates.
            </p>
            <img src="/coecommonresources/Utility_Images/BackGroundImages/PKI_FTB_Logo_RGB_small.jpg" alt="customer brand" class="custom_brand" />
            <div class="clear"></div>
            <asp:WebPartManager Personalization-Enabled="false" ID="WebPartManager1" runat="server"/>
            <center>
                <table>
                    <tr>
                        <td  valign="top">
                            <asp:WebPartZone  SkinID="WebPartZone" ID="Col0" runat="server">
                                <ZoneTemplate>
                                    <uc1:HomeWebPart SkinID="HomeWebPart" hidden="true" ID="HomeWebPart0_0" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_2" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_3" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_4" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_5" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_6" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_7" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_8" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_9" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_10" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_11" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart0_12" runat="server" />
                                </ZoneTemplate>
                            </asp:WebPartZone>
                        </td>
                        <td  valign="top">
                            <asp:WebPartZone  SkinID="WebPartZone" ID="Col1" runat="server">
                                <ZoneTemplate>
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_0" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_2" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_3" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_4" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_5" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart1_6" runat="server" />
                                </ZoneTemplate>
                            </asp:WebPartZone>
                        </td>
                        <td valign="top">
                            <asp:WebPartZone SkinID="WebPartZone" ID="Col2" runat="server">
                                <ZoneTemplate>
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_0" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_2" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_3" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_4" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_5" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart2_6" runat="server" />
                                </ZoneTemplate>
                            </asp:WebPartZone>
                        </td>
                        <td valign="top">
                            <asp:WebPartZone  SkinID="WebPartZone" ID="Col3" runat="server">
                                <ZoneTemplate>
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_0" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_2" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_3" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_4" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_5" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart3_6" runat="server" />
                                </ZoneTemplate>
                            </asp:WebPartZone>
                        </td>
                        <td valign="top">
                            <asp:WebPartZone SkinID="WebPartZone" ID="Col4" runat="server">
                                <ZoneTemplate>
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_0" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_2" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_3" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_4" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_5" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart4_6" runat="server" />
                                </ZoneTemplate>
                            </asp:WebPartZone>
                        </td>
                        <td valign="top">
                            <asp:WebPartZone  SkinID="WebPartZone" ID="Col5" runat="server" >
                                <ZoneTemplate>
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_0" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_1" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_2" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_3" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_4" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_5" runat="server" />
                                    <uc1:HomeWebPart SkinID="HomeWebPart"  hidden="true" ID="HomeWebPart5_6" runat="server" />
                                </ZoneTemplate>
                            </asp:WebPartZone>
                        </td>
                    </tr>
                </table>
            </center>
        </div>
     </div>
</asp:Content>
