<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" ValidateRequest="false" CodeFile="COETableEditorServiceTest.aspx.cs" Inherits="TableEditorServiceTest" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.WebCombo.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebTab.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebTab" TagPrefix="igtab" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>COETableEditorService Sample Application</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<TABLE style="WIDTH: 596px"><TBODY><tr><td style="WIDTH: 118px; HEIGHT: 13px">Application Name</TD><TD style="WIDTH: 136px; HEIGHT: 13px">
            <igcmbo:WebCombo id="cmbAppName" runat="server" OnSelectedRowChanged="cmbAppName_SelectedRowChanged">
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
</igcmbo:WebCombo></TD></TR><TR><TD style="WIDTH: 118px; HEIGHT: 4px">Tables</TD><TD style="WIDTH: 136px; HEIGHT: 4px"><igcmbo:WebCombo id="cmbTables" runat="server" OnSelectedRowChanged="cmbTables_SelectedRowChanged">
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
</igcmbo:WebCombo></TD></TR><TR><TD style="WIDTH: 118px; HEIGHT: 4px"><asp:Label id="lblRecord" runat="server" Text="Record"></asp:Label></TD><TD style="WIDTH: 136px; HEIGHT: 4px"><igcmbo:WebCombo id="cmbRecord" runat="server" OnSelectedRowChanged="cmbRecord_SelectedRowChanged">
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
</igcmbo:WebCombo></TD></TR></TBODY></TABLE>
        <br />
            <igtab:ultrawebtab id="UltraWebTab1" runat="server" bordercolor="#CCCCCC" borderstyle="Solid" borderwidth="1px" font-bold="True" font-names="Tahoma" font-size="8pt" height="85%" threedeffect="False" width="73%" FixedLayout="True" OnTabClick="UltraWebTab1_TabClick" AutoPostBack="True">
<HoverTabStyle BackColor="Aqua"></HoverTabStyle>

<DefaultTabStyle ForeColor="Black" BorderColor="Silver" BackColor="#FEFCFD" Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif">
<Padding Top="2px"></Padding>
</DefaultTabStyle>
<Tabs>
<igtab:Tab Text="Add Record"><ContentTemplate>
                        <span style="font-size: 10pt; font-family: Arial">
                            <br   />
                        </span>
                    
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
<igtab:Tab Text="Update Record"><ContentTemplate>
&nbsp;&nbsp;<BR />
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
<igtab:Tab Text="Delete Record"><ContentTemplate>
<BR /><SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial"></SPAN><SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial">&nbsp; </SPAN>
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
    
    <igtxt:WebImageButton id="btnPerformAction" runat="server" Text="Update" OnClick="btnPerformAction_Click"></igtxt:WebImageButton>
    
    </div>
    </form>
</body>
</html>
