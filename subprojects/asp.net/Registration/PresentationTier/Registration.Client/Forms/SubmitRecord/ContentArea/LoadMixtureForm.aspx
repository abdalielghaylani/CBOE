<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" EnableViewState="true" AutoEventWireup="true" CodeBehind="LoadMixtureForm.aspx.cs" Inherits="RegistrationWebApp2.Forms.SubmitRecord.ContentArea.LoadMixtureForm" Title="Untitled Page" %>

<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.ChemDraw"
    TagPrefix="cc2" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30" Namespace="Csla.Web" TagPrefix="cc1" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<asp:HiddenField ID="SelectedItemsCountHiddenField" runat="server" Value="0" />
<asp:HiddenField ID="TotalItemsHiddenField" runat="server"/>
    <table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
            <td class="ColumnContentHeader"></td>
            <td align="left">
                <asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" />
                <asp:Button ID="DeleteButton" runat="server" OnClick="DeleteButton_Click" />
            </td> 
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel"></asp:Label>
            </td>   
        </tr>
        <tr>
            <td colspan="3" align="center">
               <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server"/>
            </td>   
        </tr>
        <tr>
                <td colspan="3" style="height:10px"></td>
        </tr> 
        <tr>
            <td colspan="3">
                <asp:Label ID="SavedCompoundsLabel" CssClass="COELabelTitle2" runat="server" ></asp:Label>
             </td>   
        </tr>     
        <tr>
                <td colspan="3" style="height:5px"></td>
        </tr>
        <tr> 
             <td colspan="3">
                <igtbl:UltraWebGrid ID="SavedCompoundsUltraWebGrid" runat="server" DataSourceID="SavedCompoundsCslaDataSource" 
                DataMember="Default" CaptionAlign="Top" OnClick="SavedCompoundsUltraWebGrid_Click" Width="100%" 
                EnableViewState="true" DisplayLayout-AllowColSizingDefault="NotSet" DisplayLayout-RowHeightDefault="20px" OnInitializeRow="UltraWebGrid_InitializeRow">
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer"
                        AllowSortingDefault="OnClient" BorderCollapseDefault="Separate"
                        HeaderClickActionDefault="SortSingle" Name="SavedCompoundsUltraWebGrid" RowHeightDefault="20px"
                        RowSelectorsDefault="No" SelectTypeRowDefault="Extended" TableLayout="Fixed" Version="4.00" 
                        ViewType="OutlookGroupBy"  SelectTypeColDefault="Single" 
                        AutoGenerateColumns="False"> 
                 <GroupByBox Hidden="true"></GroupByBox>
                </DisplayLayout>
                        
                    <Bands>
                        <igtbl:UltraGridBand AllowSorting="Yes" Key="MainBand" GridLines="Both" CellClickAction="CellSelect">

                            <AddNewRow View="NotSet" Visible="NotSet">
                            </AddNewRow>
                            <Columns>
                                <igtbl:UltraGridColumn AllowUpdate="Yes" DataType="System.Boolean" 
                                    HeaderText="Delete" Key="DeleteColumn" Type="CheckBox" Width="50px">
                                    <Header Caption="Delete" ClickAction="Select" FixedHeaderIndicator="Button">
                                    </Header>
                                    <Footer Caption="DeleteAll" Title="DeleteAll">
                                    </Footer>
                                    <CellStyle Wrap="false" HorizontalAlign="Center">
                                    </CellStyle>
                                    <SelectedCellStyle BackColor="#D7D7E3" Height="20px" HorizontalAlign="Left" Wrap="false">
                                    </SelectedCellStyle>
                                </igtbl:UltraGridColumn>
                                <igtbl:TemplatedColumn AllowUpdate="No" BaseColumnName="Visible" HeaderText="Structure"
                                    IsBound="False" Key="StructureColumn" Width="200px">
                                    <Header Caption="Structure" ClickAction="Select">
                                        <RowLayoutColumnInfo OriginX="1" />
                                    </Header>                                    
                                    <CellTemplate>
                                        <cc2:COEChemDrawEmbed ID="AggregatedStructureChemDraw" runat="server" Height="100px"
                                            ViewOnly="true" Width="100px">
                                        </cc2:COEChemDrawEmbed>
                                    </CellTemplate>
                                    <CellStyle HorizontalAlign="center">
                                    </CellStyle>
                                </igtbl:TemplatedColumn>
                                <igtbl:UltraGridColumn AllowUpdate="No" BaseColumnName="Name" CellButtonDisplay="Always"
                                    HeaderText="Compound" IsBound="True" Key="CompoundColumn" Width="200px">
                                    <Header Caption="Name">
                                    <RowLayoutColumnInfo OriginX="2" />
                                    </Header>
                                    <Footer>
                                    <RowLayoutColumnInfo OriginX="2" />
                                    </Footer>
                                    <CellStyle Wrap="true"></CellStyle>
                                    <SelectedCellStyle BackColor="#D7D7E3" Wrap="true" Height="20px" HorizontalAlign="Left"></SelectedCellStyle>
                                </igtbl:UltraGridColumn>
                                
                                <igtbl:UltraGridColumn BaseColumnName="Description" HeaderText="Description" IsBound="True"
                                    Key="DescriptionColumn" Width="230px">
                                    <Header Caption="Description">
                                    <RowLayoutColumnInfo OriginX="3" />
                                    </Header>
                                    <Footer>
                                    <RowLayoutColumnInfo OriginX="3" />
                                    </Footer>
                                    <CellStyle Wrap="true"></CellStyle>
                                    <SelectedCellStyle BackColor="#D7D7E3" Wrap="true" Height="20px" HorizontalAlign="Left"></SelectedCellStyle>
                                </igtbl:UltraGridColumn>
                               
                                <igtbl:UltraGridColumn BaseColumnName="DateCreated" HeaderText="Date Created" IsBound="True"
                                    Key="DateCreatedColumn" DataType="System.DateTime" Width="153px">
                                    <Header Caption="Date Created">
                                    <RowLayoutColumnInfo OriginX="4" />
                                    </Header>
                                    <Footer>
                                    <RowLayoutColumnInfo OriginX="4" />
                                    </Footer>
                                    <CellStyle Wrap="true"></CellStyle>
                                    <SelectedCellStyle BackColor="#D7D7E3" Wrap="true" Height="20px" HorizontalAlign="Left"></SelectedCellStyle>
                                </igtbl:UltraGridColumn>
                                
                                <igtbl:UltraGridColumn BaseColumnName="ID" Hidden="True" IsBound="True" Key="IDColumn" DataType="System.Int32">
                                    <Header>
                                        <RowLayoutColumnInfo OriginX="5" />
                                    </Header>
                                    <Footer>
                                        <RowLayoutColumnInfo OriginX="5" />
                                    </Footer>
                                    <CellStyle Wrap="true"></CellStyle>
                                </igtbl:UltraGridColumn>
                            </Columns>
                        </igtbl:UltraGridBand>
                    </Bands>    
                </igtbl:UltraWebGrid>
                <a runat="server" id="SelectAllItemsLink" class="SelectAllText"></a> 
                </td>
             </tr>
            <tr>
                <td colspan="3" style="height:15px"></td>
            </tr>
             <tr>
                <td colspan="3"> 
                <asp:Label ID="OtherSavedCompoundsLabel" CssClass="COELabelTitle2" runat="server"></asp:Label>
                </td>   
            </tr>     
            <tr>
                <td colspan="3" style="height:5px"></td>
            </tr>
            <tr> 
             <td colspan="3">
                <igtbl:UltraWebGrid ID="OtherCompoundsUltraWebGrid" runat="server" DataSourceID="OtherCompoundsCslaDataSource"
                 DataMember="Default" CaptionAlign="Top" OnClick="SavedCompoundsUltraWebGrid_Click" Width="100%" 
                 EnableViewState="true" DisplayLayout-AllowColSizingDefault="NotSet" DisplayLayout-RowHeightDefault="20px" OnInitializeRow="UltraWebGrid_InitializeRow">
                 <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer"
                        AllowSortingDefault="OnClient" BorderCollapseDefault="Separate"
                        HeaderClickActionDefault="SortSingle" Name="SavedCompoundsUltraWebGrid" RowHeightDefault="20px"
                        RowSelectorsDefault="No" SelectTypeRowDefault="Extended" TableLayout="Fixed" Version="4.00" 
                        ViewType="OutlookGroupBy" CellClickActionDefault="RowSelect" SelectTypeColDefault="Single" 
                        AutoGenerateColumns="False"> <GroupByBox Hidden="true"></GroupByBox></DisplayLayout>
                    <Bands>
                        <igtbl:UltraGridBand AllowSorting="Yes" Key="MainBand" GridLines="Both" CellClickAction="RowSelect">
                            <AddNewRow View="NotSet" Visible="NotSet"></AddNewRow>
                            <Columns>
                                <igtbl:TemplatedColumn AllowUpdate="No" BaseColumnName="Visible" HeaderText="Structure"
                                    IsBound="False" Key="StructureColumn" Width="200px">
                                    <Header Caption="Structure" ClickAction="Select">
                                        <RowLayoutColumnInfo OriginX="1" />
                                    </Header>
                                    <CellTemplate>
                                        <cc2:COEChemDrawEmbed ID="AggregatedStructureChemDraw" runat="server" Height="100px" Width="100px"
                                            ViewOnly="true">
                                        </cc2:COEChemDrawEmbed>
                                    </CellTemplate>
                                    <CellStyle HorizontalAlign="center">
                                    </CellStyle>
                                </igtbl:TemplatedColumn>
                                <igtbl:UltraGridColumn AllowUpdate="No" BaseColumnName="Name" CellButtonDisplay="Always"
                                    HeaderText="Compound" IsBound="True" Key="CompoundColumn" Width="200px">
                                    <Header Caption="Name">
                                    <RowLayoutColumnInfo OriginX="1" />
                                    </Header>
                                    <Footer>
                                    <RowLayoutColumnInfo OriginX="1" />
                                    </Footer>
                                    <CellStyle Wrap="true"></CellStyle>
                                    <SelectedCellStyle BackColor="#D7D7E3" Height="20px" Wrap="true" HorizontalAlign="Left"></SelectedCellStyle>
                                </igtbl:UltraGridColumn>
                                <igtbl:UltraGridColumn BaseColumnName="Description" HeaderText="Description" IsBound="True"
                                    Key="DescriptionColumn" Width="230px">
                                    <Header Caption="Description">
                                    <RowLayoutColumnInfo OriginX="2" />
                                    </Header>
                                    <Footer>
                                    <RowLayoutColumnInfo OriginX="2" />
                                    </Footer>
                                    <CellStyle Wrap="true"></CellStyle>
                                    <SelectedCellStyle BackColor="#D7D7E3" Height="20px" Wrap="true" HorizontalAlign="Left"></SelectedCellStyle>
                                </igtbl:UltraGridColumn>
                               
                                <igtbl:UltraGridColumn BaseColumnName="DateCreated" HeaderText="Date Created" IsBound="True"
                                    Key="DateCreatedColumn" DataType="System.DateTime" Width="153px">
                                    <Header Caption="Date Created">
                                    <RowLayoutColumnInfo OriginX="3" />
                                    </Header>
                                    <Footer>
                                    <RowLayoutColumnInfo OriginX="3" />
                                    </Footer>
                                    <CellStyle Wrap="true"></CellStyle>
                                    <SelectedCellStyle BackColor="#D7D7E3" Wrap="true" Height="20px" HorizontalAlign="Left"></SelectedCellStyle>
                                </igtbl:UltraGridColumn>
                                <igtbl:UltraGridColumn BaseColumnName="ID" Hidden="True" IsBound="True" Key="IDColumn" DataType="System.Int32">
                                    <Header>
                                        <RowLayoutColumnInfo OriginX="3" />
                                    </Header>
                                    <Footer>
                                        <RowLayoutColumnInfo OriginX="3" />
                                    </Footer>
                                    <CellStyle Wrap="true"></CellStyle>
                                </igtbl:UltraGridColumn>
                            </Columns>
                            
                        </igtbl:UltraGridBand>
                    </Bands>
                    
                    
                    
                </igtbl:UltraWebGrid><br />
                <cc1:CslaDataSource  ID="SavedCompoundsCslaDataSource" runat="server" OnSelectObject="SavedCompoundsCslaDataSource_SelectObject" TypeAssemblyName="CambridgeSoft.COE.Framework" TypeName="CambridgeSoft.COE.Framework.COEGenericObjectStorageService.COEGenericObjectStorageBO">
                </cc1:CslaDataSource><cc1:CslaDataSource ID="OtherCompoundsCslaDataSource" runat="server" OnSelectObject="OtherCompoundsCslaDataSource_SelectObject" TypeAssemblyName="CambridgeSoft.COE.Framework" TypeName="CambridgeSoft.COE.Framework.COEGenericObjectStorageService.COEGenericObjectStorageBO">
                </cc1:CslaDataSource>
                
            </td>
        </tr>
    </table>
</asp:Content>
