<%@ Page Language="C#" AutoEventWireup="true" CodeFile="COEDatabasePublishingServiceTest.aspx.cs" ValidateRequest = "false" Inherits="COEDatabasePublishingServiceTest" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.WebCombo.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebTab.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebTab" TagPrefix="igtab" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>COEDatabasePublishing Service Sample Application</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Height="505px"
            Width="509px">
            <igmisc:WebPanel ID="WebPanel1" runat="server" Height="357px" Width="491px">
                <Template>
            <table id="tblPublishDatabase" runat ="server" style="width: 477px; height: 151px">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblError" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 100px">
                    Select Owner:</td>
                    <td style="width: 100px">
                        <igcmbo:WebCombo ID="drpOwners" runat="server" Width="171px" OnSelectedRowChanged="drpOwners_SelectedRowChanged">
                        <ExpandEffects ShadowColor="LightGray" />
                        <DropDownLayout BorderCollapse="Separate" RowHeightDefault="20px" Version="4.00">
                            <HeaderStyle BackColor="LightGray" BorderStyle="Solid">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                            </HeaderStyle>
                            <FrameStyle BackColor="Silver" BorderStyle="Ridge" BorderWidth="2px" Cursor="Default"
                                Font-Names="Verdana" Font-Size="10pt" Height="130px" Width="325px">
                            </FrameStyle>
                            <RowStyle BackColor="White" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px">
                                <BorderDetails WidthLeft="0px" WidthTop="0px" />
                            </RowStyle>
                            <SelectedRowStyle BackColor="DarkBlue" ForeColor="White" />
                        </DropDownLayout>
                        <Columns>
                            <igtbl:UltraGridColumn HeaderText="Column0">
                                <header caption="Column0"></header>
                            </igtbl:UltraGridColumn>
                        </Columns>
                    </igcmbo:WebCombo>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">
                        Password</td>
                    <td style="width: 100px">
                    <igtxt:WebTextEdit ID="txtPassword" runat="server" 
                        PasswordMode="True" Width="167px">
                    </igtxt:WebTextEdit>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; height: 28px">
                        &nbsp;</td>
                    <td style="width: 100px; height: 28px">
                    <igtxt:WebImageButton ID="btnPublishDatabase" runat="server" Text="Publish Database"
                        Width="104px" OnClick="btnPublishDatabase_Click">
                    </igtxt:WebImageButton>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 329px">
                        <asp:CheckBoxList ID="usersListBox" runat="server" Height="23px" Width="102px">
                        </asp:CheckBoxList></td>
                </tr>
            </table>
                </Template>
            </igmisc:WebPanel>
            <br />
                    <br />
            <table id="tblDataView"  runat = "server" style="width: 474px; height: 59px">
                <tr>
                    <td style="width: 108px; height: 314px">
                        <igtab:UltraWebTab ID="UltraWebTab1" runat="server" Height="322px" Width="467px" Visible="False">
                            <Tabs>
                                <igtab:Tab>
                                </igtab:Tab>
                            </Tabs>
                        </igtab:UltraWebTab>
                    </td>
                </tr>
                <tr>
                    <td style="width: 108px; height: 21px">
                        <asp:LinkButton ID="lnkbtnBack" runat="server" OnClick="lnkbtnBack_Click" Visible="False">Back</asp:LinkButton></td>
                </tr>
            </table>
        </igmisc:WebAsyncRefreshPanel>
    
    </div>
    </form>
</body>
</html>