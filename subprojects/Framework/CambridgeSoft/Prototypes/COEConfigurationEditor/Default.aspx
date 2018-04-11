<%@ Page  validateRequest="false" Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="Altova.Authentic.WebControls" Namespace="Altova.Authentic.WebControls"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="igtbar" Namespace="Infragistics.WebUI.UltraWebToolbar" Assembly="Infragistics2.WebUI.UltraWebToolbar.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="igtab" Namespace="Infragistics.WebUI.UltraWebTab" Assembly="Infragistics2.WebUI.UltraWebTab.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="igtxt" Namespace="Infragistics.WebUI.WebDataInput" Assembly="Infragistics2.WebUI.WebDataInput.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="igtxt" Namespace="Infragistics.WebUI.WebDataInput" Assembly="Infragistics2.WebUI.WebDataInput.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="igmisc" Namespace="Infragistics.WebUI.Misc" Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="ignav" Namespace="Infragistics.WebUI.UltraWebNavigator" Assembly="Infragistics2.WebUI.UltraWebNavigator.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="iglbar" Namespace="Infragistics.WebUI.UltraWebListbar" Assembly="Infragistics2.WebUI.UltraWebListbar.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Image ID="MyCompanyLogo" runat="server" ImageUrl="~/cs_logo_en.jpg" />&nbsp;
        <igtbar:UltraWebToolbar ID="UltraWebToolbar1" runat="server" BackColor="#7288AC"
            BackgroundImage="" Font-Names="Microsoft Sans Serif" Font-Size="8pt" ForeColor="White"
            Height="22px" ImageDirectory="/ig_common/images/" ItemWidthDefault="" MovableImage="ig_tb_move00.gif"
            Width="950">
            <HoverStyle BackColor="#64799C" BorderColor="Blue" BorderStyle="Solid" ForeColor="White">
                <BorderDetails WidthBottom="1px" WidthLeft="1px" WidthRight="1px" WidthTop="1px" />
            </HoverStyle>
            <Items>
                <igtbar:TBLabel Text="">
                </igtbar:TBLabel>
                <igtbar:TBLabel Text="">
                </igtbar:TBLabel>
                <igtbar:TBLabel Text="">
                </igtbar:TBLabel>
                <igtbar:TBLabel Text="">
                </igtbar:TBLabel>
                <igtbar:TBLabel Text="">
                </igtbar:TBLabel>
                <igtbar:TBLabel Text="">
                </igtbar:TBLabel>
                <igtbar:TBarButton DisabledImage="" HoverImage="" Image="" SelectedImage="" Text="Home">
                    <DefaultStyle Width="15%">
                    </DefaultStyle>
                </igtbar:TBarButton>
                <igtbar:TBarButton DisabledImage="" HoverImage="" Image="" SelectedImage="" Text="Help">
                </igtbar:TBarButton>
                <igtbar:TBarButton DisabledImage="" HoverImage="" Image="" SelectedImage="" Text="Log off">
                </igtbar:TBarButton>
            </Items>
            <DefaultStyle BackColor="#7288AC" BorderStyle="Solid" ForeColor="White">
                <BorderDetails ColorBottom="114, 136, 172" ColorLeft="114, 136, 172" ColorRight="114, 136, 172"
                    ColorTop="114, 136, 172" WidthBottom="1px" WidthLeft="1px" WidthRight="1px" WidthTop="1px" />
            </DefaultStyle>
        </igtbar:UltraWebToolbar>
        &nbsp;&nbsp;
        <table style="table-layout: fixed; overflow: visible" width="950">
            <tr>
            <td style="overflow-y: visible; overflow-x: hidden; overflow: visible; width: 250px; vertical-align: top; text-align: left; font-size: 13px; font-family: Tahoma; font-weight: bold; height: 1139px;">
                    Navigation Pane<br />
                    <br />
                    <asp:Panel ID="Panel" runat="server" Height="100%" Width="100%">
                        </asp:Panel>
                &nbsp;&nbsp;
                </td>
                <td style="width: 734px; height: 1139px; vertical-align: top; text-align: center;">
                    <strong style="font-weight: bold; font-size: 10pt; font-family: Tahoma; text-align: center;">Editor Pane<br />
                    </strong><br />
                    <igmisc:WebPanel ID="EditPane" runat="server" Width="650px" EnableTheming="True" ExpandEffect="None" BorderColor="Blue" BorderStyle="Solid" BorderWidth="1px" Visible="False">
                        <Template>
                            &nbsp;
                            &nbsp;<br />
                            <cc1:AuthenticDocumentView ID="advSNE" runat="server"
                                Height="180px" ToolbarsEnabled="False" Visible="False" Width="99%" BorderColor="White" EntryHelperShowAttributes="False" SubmitOnSave="False" ToolbarTooltipsEnabled="True" HidePluginSaveButton="False" />
                                        &nbsp;<igtab:UltraWebTab ID="uwtNLE" runat="server" AutoPostBack="True" Height="380px" Visible="False" Width="95%" BorderColor="#0056D7" BorderStyle="Solid" BorderWidth="1px" ThreeDEffect="False">
                                        <BorderDetails ColorBottom="90, 120, 160" ColorRight="90, 120, 160" />
                                        <DefaultTabStyle BackColor="#E1EDFF" Height="20px">
                                        </DefaultTabStyle>
                                        <Tabs>
                                            <igtab:Tab Text="New Tab">
                                                <HoverStyle BackColor="AliceBlue">
                                                </HoverStyle>
                                            </igtab:Tab>
                                        </Tabs>
                                        <RoundedImage FillStyle="LeftMergedWithCenter" NormalImage="ig_tab_blueb1.gif" SelectedImage="ig_tab_blueb2.gif" />
                                    </igtab:UltraWebTab>
                            &nbsp; &nbsp;&nbsp;
                            <igtab:UltraWebTab ID="uwtCNLE" runat="server" AutoPostBack="True" Height="380px"
                                OnTabClick="uwtCNLE_TabClick" Visible="False" Width="95%" BorderColor="#0056D7" BorderStyle="Solid" BorderWidth="1px" ThreeDEffect="False">
                                <BorderDetails ColorBottom="90, 120, 160" ColorRight="90, 120, 160" />
                                <DefaultTabStyle BackColor="#E1EDFF" Height="20px">
                                </DefaultTabStyle>
                                <Tabs>
                                    <igtab:Tab Text="New Tab">
                                        <HoverStyle BackColor="AliceBlue">
                                        </HoverStyle>
                                        <DisabledStyle BackColor="AliceBlue">
                                        </DisabledStyle>
                                        <SelectedStyle BackColor="AliceBlue">
                                        </SelectedStyle>
                                                    <Style BackColor="AliceBlue"></Style>
                                                </igtab:Tab>
                                            </Tabs>
                                            <RoundedImage FillStyle="LeftMergedWithCenter" NormalImage="ig_tab_blueb1.gif" SelectedImage="ig_tab_blueb2.gif" />
                                        </igtab:UltraWebTab><igtab:UltraWebTab ID="uwtGNLE" runat="server" AutoPostBack="True" Height="200px"
                                OnTabClick="uwtGNLE_TabClick" Visible="False" Width="95%" BorderColor="#0056D7" BorderStyle="Solid" BorderWidth="1px" ThreeDEffect="False">
                                            <BorderDetails ColorBottom="90, 120, 160" ColorRight="90, 120, 160" />
                                            <DefaultTabStyle BackColor="#E1EDFF" Height="20px">
                                            </DefaultTabStyle>
                                            <Tabs>
                                                <igtab:Tab Text="New Tab">
                                                    <Style BackColor="Azure"></Style>
                                                </igtab:Tab>
                                            </Tabs>
                                            <RoundedImage FillStyle="LeftMergedWithCenter" NormalImage="ig_tab_blueb1.gif" SelectedImage="ig_tab_blueb2.gif" />
                                        </igtab:UltraWebTab>
                            <br />
                            <cc1:AuthenticDocumentView ID="advSNEofNLE" runat="server" Height="180px"
                                ToolbarsEnabled="False" Visible="False" Width="99%" SubmitOnSave="False" HidePluginSaveButton="False" EntryHelperShowAttributes="False" />
                            <br /><cc1:AuthenticDocumentView ID="advSNEofCNLE" runat="server" Height="180px"
                                ToolbarsEnabled="False" Visible="False" Width="99%" HidePluginSaveButton="True" SubmitOnSave="False" />
                            <br />
                            <cc1:AuthenticDocumentView ID="advSNEofGNLE" runat="server" Height="180px"
                                ToolbarsEnabled="False" Visible="False" Width="99%" HidePluginSaveButton="True" SubmitOnSave="False" />
                            <br />
                            &nbsp;
                        </Template>
                        <Header Visible="False">
                        </Header>
                    
                    </igmisc:WebPanel>
                    &nbsp;&nbsp;
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
