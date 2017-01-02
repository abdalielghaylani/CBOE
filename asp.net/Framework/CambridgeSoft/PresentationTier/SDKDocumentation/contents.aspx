<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest ="false" CodeFile="contents.aspx.cs" Inherits="contents" %>

<%@ Register Assembly="Infragistics2.WebUI.WebHtmlEditor.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebHtmlEditor" TagPrefix="ighedit" %>
<%@ Register TagPrefix="igtab" Namespace="Infragistics.WebUI.UltraWebTab" Assembly="Infragistics2.WebUI.UltraWebTab.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Cambridgesoft SDK Documentation</title>
	</head>
	<body bottomMargin="2" leftMargin="5" topMargin="5" scroll="no" rightMargin="5">
		<form id="Form1" method="post" runat="server">
			<!-- 		<table width="100%">
				<tr bordercolor="#000000" border="1">
					<td background="dkblueGradient.gif" height="30"></td>
				</tr>
			</table> -->
            &nbsp;<igtab:UltraWebTab ID="UltraWebTab1" runat="server" BorderColor="#CCCCCC" BorderStyle="Solid"
                BorderWidth="1px" Font-Bold="True" Font-Names="Tahoma" Font-Size="8pt" Height="100%"
                ThreeDEffect="False" Width="100%" >
                <HoverTabStyle BackColor="Aqua">
                </HoverTabStyle>
                <DefaultTabStyle BackColor="#FEFCFD" BorderColor="Silver" Font-Names="Microsoft Sans Serif"
                    Font-Size="8pt" ForeColor="Black" Height="22px">
                    <Padding Top="2px" />
                </DefaultTabStyle>
                <Tabs>
                    <igtab:Tab Text="Running Sample">
                        <ContentTemplate>
                            <span style="font-size: 10pt; font-family: Arial">
                                <br />
                            </span>
                        </ContentTemplate>
                    </igtab:Tab>
                    <igtab:TabSeparator>
                    </igtab:TabSeparator>
                    <igtab:Tab Text="Code">
                        <ContentTemplate>
                            <asp:LinkButton ID="lnkaspxcode" runat="server" OnClick="lnkaspxcode_Click">.aspx Code</asp:LinkButton>&nbsp;
                            <asp:LinkButton ID="lnkcscode" runat="server" OnClick="lnkcscode_Click">.aspx.cs Code</asp:LinkButton><br />
                            <asp:TextBox ID="txtCode" runat="server" Height="92%" TextMode="MultiLine" Width="98%"></asp:TextBox>
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
           
        </form>
	</body>
</html>
