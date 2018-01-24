<%@ Page Language="C#" AutoEventWireup="true" CodeFile="COESearchServiceTest.aspx.cs" Inherits="COESearchServiceTest" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebTab.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebTab" TagPrefix="igtab" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>COESearchService Sample Application</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <igtab:ultrawebtab id="tbCOESearchService" runat="server" bordercolor="#CCCCCC"
            borderstyle="Solid" borderwidth="1px" fixedlayout="True" font-bold="True" font-names="Tahoma"
            font-size="8pt" height="106%" ontabclick="tbCOESearchService_TabClick" threedeffect="False"
            width="76%">
<HoverTabStyle BackColor="Aqua"></HoverTabStyle>

<DefaultTabStyle ForeColor="Black" BorderColor="Silver" BackColor="#FEFCFD" Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif">
<Padding Top="2px"></Padding>
</DefaultTabStyle>
<Tabs>
<igtab:Tab Text="Input Data"><ContentTemplate>
<span style="FONT-SIZE: 10pt; FONT-FAMILY: Arial">
    <br />
    <table style="width: 530px; height: 156px">
        <tr>
            <td style="width: 161px">
                Application:</td>
            <td style="width: 213px">
                <asp:TextBox id="txtappName" runat="server">ChemInv</asp:TextBox>
                </td>
        </tr>
        <tr>
            <td style="width: 161px">
                Upload SecurityInfo:</td>
            <td style="width: 213px">
                <asp:FileUpload ID="uploadSecurityInfo" runat="server" />
                </td>
        </tr>
        <tr>
            <td style="width: 161px">
                Upload DataView:</td>
            <td style="width: 213px">
                <asp:FileUpload ID="uploadDataView" runat="server" />
                </td>
        </tr>
        <tr>
            <td style="width: 161px">
                Upload SearchCriteria:</td>
            <td style="width: 213px">
                <asp:FileUpload ID="uploadSearchCriteria" runat="server" />
                </td>
        </tr>
        <tr>
            <td style="width: 161px">
                Upload ResultsCriteria:</td>
            <td style="width: 213px">
                <asp:FileUpload ID="uploadResultsCriteria" runat="server" />
                </td>
        </tr>
        <tr>
            <td style="width: 161px">
                Upload PagingInfo:</td>
            <td style="width: 213px">
                <asp:FileUpload ID="uploadPagingInfo" runat="server" />
                </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="btnSubmit" runat="server" Font-Bold="True" OnClick="btnSubmit_Click"
        Style="background-image: url(images/weblisttab.JPG)" Text="Submit" /><br />
    <br />
<igtab:ultrawebtab id="tbInputData" runat="server" height="463px" width="571px" bordercolor="#CCCCCC" borderstyle="Solid" borderwidth="1px" font-bold="True" font-names="Tahoma" font-size="8pt" threedeffect="False" FixedLayout="True" OnTabClick="tbInputData_TabClick">
<HoverTabStyle BackColor="Aqua"></HoverTabStyle>

<DefaultTabStyle ForeColor="Black" BorderColor="Silver" BackColor="#FEFCFD" Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif">
<Padding Top="2px"></Padding>
</DefaultTabStyle>
<Tabs>
<igtab:Tab Text="SecurityInfo"><ContentTemplate>
                        <span style="font-size: 10pt; font-family: Arial">
                            <br   />
                        </span>
                    
</ContentTemplate>
    <ContentPane TargetUrl="xml/ChemInv/SecurityInfo.xml">
    </ContentPane>
</igtab:Tab>
    <igtab:TabSeparator>
    </igtab:TabSeparator>
    <igtab:Tab Text="DataView">
        <ContentPane TargetUrl="xml/ChemInv/DataView.xml">
        </ContentPane>
    </igtab:Tab>
    <igtab:TabSeparator>
    </igtab:TabSeparator>
    <igtab:Tab Text="SearchCriteria">
        <ContentTemplate>
            <br   />
            <span style="font-size: 10pt; font-family: Arial"></span><span style="font-size: 10pt;
                            font-family: Arial">&nbsp; </span>
        </ContentTemplate>
        <ContentPane TargetUrl="xml/ChemInv/SearchCriteria.xml">
        </ContentPane>
    </igtab:Tab>
    <igtab:TabSeparator>
    </igtab:TabSeparator>
    <igtab:Tab Text="ResultsCriteria">
        <ContentPane TargetUrl="xml/ChemInv/ResultsCriteria.xml">
        </ContentPane>
    </igtab:Tab>
    <igtab:TabSeparator>
    </igtab:TabSeparator>
    <igtab:Tab Text="PagingInfo">
        <ContentPane TargetUrl="xml/ChemInv/PagingInfo.xml">
        </ContentPane>
    </igtab:Tab>
</Tabs>

<RoundedImage SelectedImage="./images/blueTab1.jpg" NormalImage="./images/ig_tab_winXP3.gif" FillStyle="LeftMergedWithCenter" ShiftOfImages="2" LeftSideWidth="7" RightSideWidth="6"></RoundedImage>

<SelectedTabStyle ForeColor="Black">
<Padding Bottom="2px"></Padding>
</SelectedTabStyle>

<DefaultTabSeparatorStyle BackColor="#33CC99"></DefaultTabSeparatorStyle>

<DisabledTabStyle BackColor="Silver"></DisabledTabStyle>
</igtab:ultrawebtab></SPAN>
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
<igtab:Tab Text="Methods"><ContentTemplate>
&nbsp;&nbsp;<br />
    <igmisc:webgroupbox id="WebGroupBox1" runat="server" height="96px" text="GetHitList"
        width="528px"><Template>
<br /><SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial">Performs a search based on the input DataView and SearchCriteria. Resulting hits are saved to the database. A hitlist object is returned.<br /></SPAN><br />
            &nbsp;<asp:Button ID="btnGetHitlist" runat="server" OnClick="btnGetHitList_Click" Text="Test" UseSubmitBehavior="False" />
</Template>
</igmisc:webgroupbox>
    <igmisc:webgroupbox id="WebGroupBox2" runat="server" height="205px" text="GetData"
        width="532px"><Template>
<br /><SPAN style="FONT-SIZE: 10pt"><SPAN style="FONT-FAMILY: Arial">This method can be used to retrive a page of data either from an exiting hitlist or from the base table of the current dataview. Its behavior is driven by the presence or absence of a hitlistID value in the current pagingInfo object: <br /><br />1.To browse the base table, simply modify the PagingInfo object to set HitlistID to zero <br />2.To get data from an exisitng hitlist, simply modify the PagingInfo object to set HitlistID to desired value </SPAN><br /></SPAN><br /><asp:Button id="btnTestGetData" runat="server" Text="Test" OnClick="btnTestGetData_Click"></asp:Button> 
</Template>
</igmisc:webgroupbox>
    <igmisc:webgroupbox id="WebGroupBox3" runat="server" height="147px" text="DoSearch"
        width="534px"><Template>
<br /><SPAN style="FONT-SIZE: 10pt; FONT-FAMILY: Arial">This method is a convenience method that combines a call to GetHitlist with a call to GetData <br />Performs a search using the provided DataView and SearchCriteria, and returns a Dataset with as specified by the ResultsCriteria</SPAN> 
            <br />
            <br />
            <asp:CheckBox ID="chkSendSearchCritera" runat="server" Text="Do not Send Search Criteria" /><br /><br /><asp:Button id="btnTestDoSearch" runat="server" Text="Test" OnClick="btnTestDoSearch_Click"></asp:Button> 
</Template>
</igmisc:webgroupbox>
</ContentTemplate>
</igtab:Tab>
<igtab:TabSeparator></igtab:TabSeparator>
<igtab:Tab Text="Results"><ContentTemplate>
                        <span style="font-size: 10pt;
                            font-family: Arial">&nbsp;<asp:Button ID="btnGridView" runat="server" BackColor="SkyBlue" Font-Bold="True" Text="View in Grid"
                                Width="100px" OnClick="btnGridView_Click" /><asp:Button ID="btnXML" runat="server" BackColor="SkyBlue" Font-Bold="True" Text="View as XML"
        Visible="False" OnClick="btnXML_Click" /><br />
                            <br />
                            <igtab:ultrawebtab id="tbResults" runat="server" height="463px" width="571px" bordercolor="#CCCCCC" borderstyle="Solid" borderwidth="1px" font-bold="True" font-names="Tahoma" font-size="8pt" threedeffect="False" FixedLayout="True" OnTabClick="tbResults_TabClick">
                                <HoverTabStyle BackColor="Aqua">
                                </HoverTabStyle>
                                <DefaultTabStyle ForeColor="Black" BorderColor="Silver" BackColor="#FEFCFD" Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif">
                                    <Padding Top="2px" />
                                </DefaultTabStyle>
                                <Tabs>
                                    <igtab:Tab Text="HitListInfo">
                                        <ContentTemplate>
                                            <span style="font-size: 10pt; font-family: Arial">
                                                <br   />
                                                &nbsp;<asp:GridView ID="gvHitListInfo" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                                    Height="1px" Width="231px">
                                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                    <RowStyle BackColor="#EFF3FB" />
                                                    <EditRowStyle BackColor="#2461BF" />
                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                    <AlternatingRowStyle BackColor="White" />
                                                </asp:GridView>
                                            </span>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                    <igtab:TabSeparator>
                                    </igtab:TabSeparator>
                                    <igtab:Tab Text="PagingInfo">
                                        <ContentTemplate>
                                            <asp:GridView ID="gvPagingInfo" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                                Height="41px" Visible="False" Width="203px">
                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <RowStyle BackColor="#EFF3FB" />
                                                <EditRowStyle BackColor="#2461BF" />
                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <AlternatingRowStyle BackColor="White" />
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </igtab:Tab>
                                    <igtab:TabSeparator>
                                    </igtab:TabSeparator>
                                    <igtab:Tab Text="DataSet">
                                        <ContentTemplate>
                                            <br   />
                                            <span style="font-size: 10pt; font-family: Arial"></span><span style="font-size: 10pt;
                            font-family: Arial">&nbsp; 
                                                <asp:GridView ID="gvDataSet" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                                    Visible="False" Height="1px" Width="176px">
                                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                    <RowStyle BackColor="#EFF3FB" />
                                                    <EditRowStyle BackColor="#2461BF" />
                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                    <AlternatingRowStyle BackColor="White" />
                                                </asp:GridView>
                                            </span>
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
                            &nbsp;</span>
                    
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
        &nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp; &nbsp;
    </div>
    </form>
</body>
</html>
