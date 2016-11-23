<%@ Page Language="C#" AutoEventWireup="true" CodeFile="COEHitListServiceTest.aspx.cs" Inherits="COEHitListServiceTest" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebTab.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebTab" TagPrefix="igtab" %>
<%@ Register Assembly="Infragistics2.WebUI.WebCombo.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDateChooser.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebSchedule" TagPrefix="igsch" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>COEHitListService Sample Application</title>
</head>
<body>
    <form id="form1" runat="server">
        <igtab:ultrawebtab id="tbMainTab" runat="server" autopostback="True" bordercolor="#CCCCCC"
            borderstyle="Solid" borderwidth="1px" fixedlayout="True" font-bold="True" font-names="Tahoma"
            font-size="8pt" height="55%"
            threedeffect="False" width="71%">
<HoverTabStyle BackColor="Aqua"></HoverTabStyle>

<DefaultTabStyle ForeColor="Black" BorderColor="Silver" BackColor="#FEFCFD" Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif">
<Padding Top="2px"></Padding>
</DefaultTabStyle>
<Tabs>
<igtab:Tab Text="Manage"><ContentTemplate>
<SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial"><TABLE style="WIDTH: 488px; HEIGHT: 71px"><TBODY><TR><TD style="WIDTH: 90px; HEIGHT: 5px">Select Record</TD><TD style="WIDTH: 134px; HEIGHT: 5px"><igcmbo:WebCombo id="WebCombo1" runat="server" OnSelectedRowChanged="WebCombo1_SelectedRowChanged">
<ExpandEffects ShadowColor="LightGray"></ExpandEffects>

<DropDownLayout RowHeightDefault="20px" BorderCollapse="Separate" Version="4.00">
<HeaderStyle BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</HeaderStyle>

<FrameStyle BorderWidth="2px" BorderStyle="Ridge" Font-Size="10pt" Font-Names="Verdana" BackColor="Silver" Width="325px" Height="130px" Cursor="Default"></FrameStyle>

<RowStyle BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" BackColor="White">
<BorderDetails WidthLeft="0px" WidthTop="0px"></BorderDetails>
</RowStyle>

<SelectedRowStyle ForeColor="White" BackColor="DarkBlue"></SelectedRowStyle>
</DropDownLayout>
<Columns>
<igtbl:UltraGridColumn HeaderText="Column0">
<Header Caption="Column0"></Header>
</igtbl:UltraGridColumn>
</Columns>
</igcmbo:WebCombo></TD></TR><TR><TD colSpan=2 style="height: 34px"><asp:CheckBox id="chkManageCurrentRecord" runat="server" Width="199px" Text="Manage Current Record" AutoPostBack="True" OnCheckedChanged="chkManageCurrentRecord_CheckedChanged"></asp:CheckBox></TD></TR></TBODY></TABLE></SPAN>&nbsp;<igtab:ultrawebtab id="tbManageTab" runat="server" width="490px" height="221px" borderwidth="1px" bordercolor="#CCCCCC" borderstyle="Solid" font-bold="True" font-names="Tahoma" font-size="8pt" threedeffect="False" FixedLayout="True">
<HoverTabStyle BackColor="Aqua"></HoverTabStyle>

<DefaultTabStyle ForeColor="Black" BorderColor="Silver" BackColor="#FEFCFD" Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif">
<Padding Top="2px"></Padding>
</DefaultTabStyle>
<Tabs>
<igtab:Tab Text="Edit"><ContentTemplate>
<SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial"><BR /><TABLE style="WIDTH: 555px; HEIGHT: 157px"><TBODY><TR><TD style="WIDTH: 69px">Description</TD><TD style="WIDTH: 100px">
<asp:TextBox id="txtEditDescription" runat="server"></asp:TextBox></TD></TR><TR><TD style="WIDTH: 69px; height: 33px;">List Name</TD><TD style="WIDTH: 100px; height: 33px;"><asp:TextBox id="txtEditListName" runat="server"></asp:TextBox></TD></TR><TR><TD style="WIDTH: 69px">IsPublic</TD><TD style="WIDTH: 100px">
    <asp:CheckBox ID="chkEditIsPublic" runat="server" /></TD></TR><TR><TD colSpan=2>
<igtxt:WebImageButton id="btnEdit" runat="server" Text="Edit" OnClick="btnEdit_Click"></igtxt:WebImageButton></TD></TR></TBODY></TABLE></SPAN>
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
<igtab:Tab Text="Operations"><ContentTemplate>
<igcmbo:WebCombo id="WebCombo2" runat="server"></igcmbo:WebCombo><BR />
    <BR />
    <asp:RadioButtonList ID="rdOperations" runat="server">
        <asp:ListItem>Intersection With Current List</asp:ListItem>
        <asp:ListItem>Subraction With Current List</asp:ListItem>
        <asp:ListItem>Union with Current List</asp:ListItem>
    </asp:RadioButtonList><BR /><BR /><BR /><igtxt:WebImageButton id="btnPerformOperation" runat="server" Text="Perform Operation" OnClick="btnPerformOperation_Click"></igtxt:WebImageButton> 
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
<igtab:Tab Text="Delete"><ContentTemplate>
<SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial"> <TABLE style="WIDTH: 557px"><TBODY><TR><TD style="WIDTH: 61px">Description</TD><TD style="WIDTH: 100px"><asp:TextBox id="txtDeleteDesc" runat="server" ReadOnly="True"></asp:TextBox>
</TD></TR><TR><TD style="WIDTH: 61px">List Name</TD><TD style="WIDTH: 100px"><asp:TextBox id="txtDeleteListName" runat="server" ReadOnly="True"></asp:TextBox></TD></TR><TR><TD style="WIDTH: 61px">Is Public</TD><TD style="WIDTH: 100px"><asp:CheckBox id="chkDeleteIsPublic" runat="server"></asp:CheckBox></TD></TR><TR><TD style="HEIGHT: 52px" colSpan=2>
    <igtxt:WebImageButton ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click">
    </igtxt:WebImageButton>
</TD></TR></TBODY></TABLE></SPAN>
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
</Tabs>

<RoundedImage SelectedImage="./images/blueTab1.jpg" NormalImage="./images/ig_tab_winXP3.gif" FillStyle="LeftMergedWithCenter" ShiftOfImages="2" LeftSideWidth="7" RightSideWidth="6"></RoundedImage>

<SelectedTabStyle ForeColor="Black">
<Padding Bottom="2px"></Padding>
</SelectedTabStyle>

<DefaultTabSeparatorStyle BackColor="#33CC99"></DefaultTabSeparatorStyle>

<DisabledTabStyle BackColor="Silver"></DisabledTabStyle>
</igtab:ultrawebtab>
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
<igtab:Tab Text="Restore"><ContentTemplate>
&nbsp;&nbsp;<BR /><TABLE style="WIDTH: 471px; HEIGHT: 232px"><TBODY><TR><TD style="WIDTH: 122px"><asp:RadioButton id="rdSaved" runat="server" Text="Saved"></asp:RadioButton></TD><TD style="WIDTH: 90px">
    <igcmbo:WebCombo ID="WebCombo3" runat="server">
    </igcmbo:WebCombo>
</TD></TR><TR><TD style="WIDTH: 122px; height: 37px;"><asp:RadioButton id="rdLastSelected" runat="server" Text="Last Selected"></asp:RadioButton></TD><TD style="WIDTH: 90px; height: 37px;"></TD></TR><TR><TD style="WIDTH: 122px; HEIGHT: 70px">
<igtxt:WebImageButton id="btnRestore" runat="server" Text="Restore" OnClick="btnRestore_Click"></igtxt:WebImageButton></TD><TD style="WIDTH: 90px; HEIGHT: 70px"></TD></TR></TBODY></TABLE>
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
<igtab:Tab Text="Save"><ContentTemplate>
<BR /><SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial"></SPAN><SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial">&nbsp; <TABLE style="WIDTH: 508px; HEIGHT: 138px"><TBODY><TR><TD style="WIDTH: 68px">Description</TD><TD style="WIDTH: 43px"><asp:TextBox id="txtSaveDesc" runat="server" ReadOnly="True"></asp:TextBox></TD></TR><TR><TD style="WIDTH: 68px">Name</TD><TD style="WIDTH: 43px"><asp:TextBox id="txtSaveListName" runat="server" ReadOnly="True"></asp:TextBox></TD></TR><TR><TD style="WIDTH: 68px; HEIGHT: 31px">IsPublic</TD><TD style="WIDTH: 43px; HEIGHT: 31px"><asp:CheckBox id="chkSaveIsPublic" runat="server"></asp:CheckBox></TD></TR><TR><TD style="WIDTH: 68px">
<igtxt:WebImageButton id="btnSave" runat="server" Width="95px" Text="Save" OnClick="btnSave_Click"></igtxt:WebImageButton></TD><TD style="WIDTH: 43px"></TD></TR></TBODY></TABLE></SPAN>
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
</Tabs>

<RoundedImage SelectedImage="./images/blueTab1.jpg" NormalImage="./images/ig_tab_winXP3.gif" FillStyle="LeftMergedWithCenter" ShiftOfImages="2" LeftSideWidth="7" RightSideWidth="6"></RoundedImage>

<SelectedTabStyle ForeColor="Black">
<Padding Bottom="2px"></Padding>
</SelectedTabStyle>

<DefaultTabSeparatorStyle BackColor="#33CC99"></DefaultTabSeparatorStyle>

<DisabledTabStyle BackColor="Silver"></DisabledTabStyle>
</igtab:ultrawebtab>
    </form>
</body>
</html>
