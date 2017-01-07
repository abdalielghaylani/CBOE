<%@ Control Language="C#" AutoEventWireup="true" Inherits="Forms_Public_UserControls_ErrorArea" Codebehind="ErrorArea.ascx.cs" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<table width="50%">
    <tr class="ErrorMessagesRow" runat="server" id="ErrorMessagesRow">
            <td>
                <igmisc:WebPanel ID="ErrorsWebPanel" runat="server" Width="95%">
                    <PanelStyle Font-Size="8pt" Font-Names="Verdana">
                        <BorderDetails WidthLeft="1px" StyleBottom="Solid" ColorBottom="Gray" ColorRight="Gray" WidthRight="1px"
                             StyleRight="Solid" WidthBottom="1px" StyleLeft="Solid" ColorLeft="224, 224, 224">
                         </BorderDetails>
                    </PanelStyle>
                    <Header TextAlignment="Left">
                     <ExpandedAppearance Style-CssClass="ErrorMessageLabel" ></ExpandedAppearance>
                    </Header>
                    <Template>
                        <table>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="ErrorMessageLabel" runat="server" SkinID="ErrorMessageLabel"></asp:Label>
                                </td>
                                <td colspan="2" align="center">
                                    <asp:Label ID="GoBackLinkLabel" runat="server" SkinID="LinkLabel" Visible="false"></asp:Label>            
                               </td>
                            </tr>
                        </table>
                    </Template>
                </igmisc:WebPanel>
            </td>   
        </tr>
</table>