<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest = "false" CodeFile="COEDataViewServiceTest.aspx.cs" Inherits="DataViewService" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebTab.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebTab" TagPrefix="igtab" %>
<%@ Register Assembly="Infragistics2.WebUI.WebCombo.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDateChooser.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebSchedule" TagPrefix="igsch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>COEDataViewService Sample Application</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width: 665px; height: 16px;">
            <tr>
                <td style="width: 104px">
                    Select Database</td>
                <td style="width: 140px">
                    <asp:DropDownList ID="drpAppList" runat="server" Width="204px" AutoPostBack="True" OnSelectedIndexChanged="drpAppList_SelectedIndexChanged">
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td style="width: 104px; height: 11px;">
                    Select Record</td>
                <td style="width: 140px; height: 11px;">
                    <igcmbo:WebCombo ID="drpSelectRecord" runat="server" OnSelectedRowChanged="drpSelectRecord_SelectedRowChanged">
                    </igcmbo:WebCombo>
                </td>
            </tr>
        </table>
        <br />
        <igtab:UltraWebTab ID="UltraWebTab1" runat="server" AutoPostBack="True" BorderColor="White"
            BorderStyle="None" FixedLayout="True" Font-Bold="True" Font-Names="Tahoma"
            Font-Size="8pt" Height="1%" OnTabClick="UltraWebTab1_TabClick" ThreeDEffect="False"
            Width="90%" SelectedTab="1">
            <HoverTabStyle BackColor="Aqua">
            </HoverTabStyle>
            <DefaultTabStyle BackColor="#FEFCFD" BorderColor="Silver" Font-Names="Microsoft Sans Serif"
                Font-Size="8pt" ForeColor="Black" Height="22px">
                <Padding Top="2px" />
            </DefaultTabStyle>
            <Tabs>
                <igtab:Tab Text="Add Record">
                    <ContentTemplate>
                        <span style="font-size: 10pt; font-family: Arial"></span>
                    </ContentTemplate>
                </igtab:Tab>
                <igtab:TabSeparator>
                </igtab:TabSeparator>
                <igtab:Tab Text="Update Record">
                    <ContentTemplate>
                        &nbsp;&nbsp;<br />
                    </ContentTemplate>
                </igtab:Tab>
                <igtab:TabSeparator>
                </igtab:TabSeparator>
                <igtab:Tab Text="Delete Record">
                    <ContentTemplate>
                        <br />
                        <span style="font-size: 10pt; font-family: Arial"></span><span style="font-size: 10pt;
                            font-family: Arial">&nbsp; </span>
                    </ContentTemplate>
                </igtab:Tab>
                <igtab:TabSeparator>
                </igtab:TabSeparator>
            </Tabs>
            <RoundedImage FillStyle="LeftMergedWithCenter" LeftSideWidth="7" NormalImage="./images/ig_tab_winXP3.gif"
                RightSideWidth="6" SelectedImage="./images/blueTab1.jpg" ShiftOfImages="2" />
            <SelectedTabStyle ForeColor="Black">
                <Padding Bottom="2px" />
            </SelectedTabStyle>
            <DefaultTabSeparatorStyle BackColor="#33CC99">
            </DefaultTabSeparatorStyle>
            <DisabledTabStyle BackColor="Silver">
            </DisabledTabStyle>
        </igtab:UltraWebTab>
    
    </div>
        <table style="width: 669px; height: 352px">
            <tr>
                <td style="width: 90px">
                    Form Group</td>
                <td style="width: 127px">
                    <asp:TextBox ID="txtFormGroup" runat="server" Width="189px" ReadOnly="True"></asp:TextBox>
                    <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtFormGroup"
                        ErrorMessage="Must be an Integer" MaximumValue="999999999999" MinimumValue="0"
                        Width="144px"></asp:RangeValidator></td>
            </tr>
            <tr>
                <td style="width: 90px">
                    IsPublic</td>
                <td style="width: 127px">
                    <asp:CheckBox ID="chkIsPublic" runat="server" Text="Is Public" Width="194px" /></td>
            </tr>
            <tr>
                <td style="width: 90px">
                    Description</td>
                <td style="width: 127px">
                    <asp:TextBox ID="txtDesc" runat="server" Width="191px"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 90px; height: 26px">
                    Name</td>
                <td style="width: 127px; height: 26px">
                    <asp:TextBox ID="txtName" runat="server" Width="192px"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 90px; height: 16px">
                    User Name</td>
                <td style="width: 127px; height: 16px">
                    <asp:TextBox ID="txtUserName" runat="server" Width="193px" ReadOnly="True"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 90px; height: 66px;">
                    COEDataView</td>
                <td style="width: 127px; height: 66px;">
                    <asp:TextBox ID="txtCOEDataView" runat="server" Height="48px" TextMode="MultiLine"
                        Width="477px"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 90px; height: 10px">
                    Date Created</td>
                <td style="width: 127px; height: 10px">
                    <igsch:WebDateChooser ID="dtDateCreated" runat="server" Width="201px" ReadOnly="True">
                    </igsch:WebDateChooser>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="height: 1px">
                    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" /></td>
            </tr>
        </table>
    </form>
</body>
</html>
