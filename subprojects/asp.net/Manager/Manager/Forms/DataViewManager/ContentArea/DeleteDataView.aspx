<%@ Page Language="C#" MasterPageFile="~/Forms/Master/DataViewManager.Master" AutoEventWireup="true" CodeBehind="DeleteDataView.aspx.cs" Inherits="DeleteDataView" Title="Delete a dataview" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.Master" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7DD5C3163F2CD0CB"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

<style type="text/css">
    
.dropDownStyle {
    margin-left: 10px;
    height:23px;
    width:200px;
}
</style>

    <script language="javascript" type="text/javascript">
        function ConfirmDelete() {
            var dataviewsChecked = getAllCheckedDataviews();

            if (dataviewsChecked.length == 0) {
                alert("Please select the dataviews you want to delete");

                return false;
            }

            // Bind all the dataviews user selected to hidden fields, so server can get value when post back for deletion.
            var hiddenFieldClientId = '<%= dataviewIdsHiddenField.ClientID %>';
            var hiddenField = document.getElementById(hiddenFieldClientId);
            hiddenField.value = dataviewsChecked.join();

            YAHOO.coemanager.dvManager.confirmationPanel.show();
            return false;
        }

        YAHOO.namespace("coemanager.dvManager");
        function initConfirmationPanel() {
            YAHOO.coemanager.dvManager.confirmationPanel = new YAHOO.widget.Panel("confirmationPanel",
                {
                    close: true,
                    width: 300,
                    visible: false,
                    draggable: false,
                    constraintoviewport: true,
                    modal: true,
                    fixedcenter: true,
                    zIndex: 17000
                }
            );

            YAHOO.coemanager.dvManager.confirmationPanel.render(document.forms[0]);
        }
        YAHOO.util.Event.addListener(window, "load", initConfirmationPanel);

        function CheckHeaderCheckBoxItem(HeaderChkObj, ItemCheckBoxKey) {
            var GridName = '<%=InstanceDataviewsWebGrid.ClientID%>';
            CheckAllCheckBoxInCell(HeaderChkObj.checked, GridName, ItemCheckBoxKey);
        }

        //this function will check/uncheck all the check box in the selected column
        function CheckAllCheckBoxInCell(_CheckStatus, _GridName, _KeyValue) {
            //get the object of the grid 
            var _grid = igtbl_getGridById(_GridName);

            //iteratin through the row and getting the cell with the key 
            for (_Index = 0; _Index < _grid.Rows.length; _Index++) {
                //getting the cell object
                var _CellObj = _grid.Rows.getRow(_Index).getCellFromKey(_KeyValue);

                //getting the checkbox with the object or tag name.  
                var _CheckBoxElement = _CellObj.Element.getElementsByTagName("input")[0];

                _CheckBoxElement.checked = _CheckStatus;
            }
        }

        function getAllCheckedDataviews() {
            var _GridName = '<%=InstanceDataviewsWebGrid.ClientID%>';
            var checkDataviews = new Array();
            
            var _grid = igtbl_getGridById(_GridName);

            for (_Index = 0; _Index < _grid.Rows.length; _Index++) {
                var _CellObj = _grid.Rows.getRow(_Index);
                var _CheckBoxElement = _CellObj.getCellFromKey('Remove').Element.getElementsByTagName("input")[0];

                if (_CheckBoxElement.checked) {
                    var dataviewId = _CellObj.getCell(1).getValue();
                    checkDataviews.push(dataviewId);
                }
            }

            return checkDataviews;
        }

        function WebRefreshPanel2_InitializePanel(oPanel) {
            oPanel.getProgressIndicator().setImageUrl("../../../App_Themes/Common/Images/searching.gif");
            oPanel.getProgressIndicator().setLocation(5);
        }

    </script>
    <table class="PagesContentTable" style="width:100%">
        <tr>
            <td align="center" colspan="2">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                &nbsp;&nbsp;
            </td>
        </tr>
        <tr id ="container" >
            <td align="right" style="width:23.2%">
                <asp:Label runat="server" ID="InstanceTitleLabel" SkinID="Title" Text="Data source"></asp:Label>
            </td>
            <td align="left">
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Height="25px" Width="70%" InitializePanel="WebRefreshPanel2_InitializePanel" LinkedRefreshControlID="WebAsyncRefreshPanel1">
                    <asp:DropDownList ID="InstanceDropDownList" runat="server" AutoPostBack="true" CssClass="dropDownStyle" AppendDataBoundItems="true" OnSelectedIndexChanged="InstanceDropDownList_SelectedIndexChanged" />
                </igmisc:WebAsyncRefreshPanel>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                &nbsp;&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Height="280px" Width="100%">
                <span runat="server" id="spGrid" style='display:block;'>
                    <igtbl:UltraWebGrid ID="InstanceDataviewsWebGrid" runat="server" Browser="Xml" OnInitializeDataSource="InstanceDataviewsWebGrid_InitializeDataSource">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                                <RowStyle CssClass="GridText" />
                                <SelectedRowStyle CssClass="SelectedGridText" />
                                <RowAlternateStyle CssClass="GridText" />
                                <RowSelectorStyle CssClass="GridText" />
                                <HeaderStyle CssClass="HeaderColumnsText" />
                                <RowTemplateStyle BorderColor="White" BorderStyle="None" BackColor="White">
                                    <BorderDetails WidthLeft="0px" WidthTop="3px" WidthRight="0px" WidthBottom="3px">
                                    </BorderDetails>
                                </RowTemplateStyle>
                                <Columns>
                                    <igtbl:TemplatedColumn BaseColumnName="Remove" Key="Remove" AllowUpdate="No" Width="30">
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="IsAllRemoveKeyCheckBox" runat="server" onclick="CheckHeaderCheckBoxItem(this,'Remove')" />
                                        </HeaderTemplate>
                                        <Header Fixed="true">
                                        </Header>
                                        <CellTemplate>
                                            <asp:CheckBox runat="server" ID="RemoveCheckBox" onclick="CheckedChanged('Remove')" />
                                        </CellTemplate>
                                    </igtbl:TemplatedColumn>
                                    <igtbl:UltraGridColumn Key="ID" BaseColumnName="ID" DataType="System.Int32"
                                        Hidden="false"  IsBound="true" AllowUpdate="No">
                                        <Header Fixed="true" Caption="Id">
                                        </Header>
                                        <CellStyle HorizontalAlign="Left" CssClass="DisplayNameOnGrid" TextOverflow="Ellipsis">
                                        </CellStyle>
                                    </igtbl:UltraGridColumn>
                                    <igtbl:UltraGridColumn Key="Name" IsBound="true" DataType="System.String" BaseColumnName="Name"
                                        AllowUpdate="No" Width="200px">
                                        <Header Fixed="true" Caption="Dataview Name">
                                        </Header>
                                        <CellStyle HorizontalAlign="Left" CssClass="DisplayNameOnGrid" TextOverflow="Ellipsis">
                                        </CellStyle>
                                    </igtbl:UltraGridColumn>
                                    <igtbl:UltraGridColumn Key="Description" IsBound="true" DataType="System.String" BaseColumnName="Description"
                                        AllowUpdate="No" Width="317px">
                                        <Header Fixed="true" Caption="Description">
                                        </Header>
                                        <CellStyle HorizontalAlign="Left" CssClass="DisplayNameOnGrid" TextOverflow="Ellipsis">
                                        </CellStyle>
                                    </igtbl:UltraGridColumn>
                                </Columns>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout RowHeightDefault="18px" Version="3.00" Name="InstanceDataviewsWebGrid"
                            ViewType="Hierarchical" BorderCollapseDefault="Separate" AllowUpdateDefault="Yes" NoDataMessage="<span style='background-color:red'>No matches found.</span>"
                            CellClickActionDefault="Edit" ActivationObject-AllowActivation="true" FilterOptionsDefault-RowFilterMode="AllRowsInBand"
                            UseFixedHeaders="true" StationaryMargins="Header">
                            <AddNewBox>
                                <BoxStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorTop="White" WidthLeft="0px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </BoxStyle>
                            </AddNewBox>
                        <Pager AllowPaging="False" ChangeLinksColor="True" MinimumPagesForDisplay="2" PageSize="5"
                            Pattern="Page [default] of [pagecount]" StyleMode="ComboBox">
                            <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                            </PagerStyle>
                        </Pager>
                            <HeaderStyleDefault VerticalAlign="Middle" BorderStyle="Solid" HorizontalAlign="Left"
                                ForeColor="#000099" Font-Bold="true" BackColor="#EBF3FB" Height="18px">
                                <Padding Left="3px" Right="3px"></Padding>
                                <BorderDetails ColorTop="#000099" WidthLeft="0px" WidthTop="1px" ColorLeft="#000099">
                                </BorderDetails>
                            </HeaderStyleDefault>
                            <RowSelectorStyleDefault BackColor="White">
                            </RowSelectorStyleDefault>
                            <FrameStyle Width="70%" BorderWidth="1px" Font-Size="8pt" Font-Names="Verdana" BorderStyle="Solid"
                                BackColor="#EBF3FB" Height="250px">
                            </FrameStyle>
                            <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                <BorderDetails ColorTop="White" WidthLeft="0px" WidthTop="1px" ColorLeft="White">
                                </BorderDetails>
                            </FooterStyleDefault>
                            <ActivationObject BorderColor="170, 184, 131">
                            </ActivationObject>
                            <RowExpAreaStyleDefault BackColor="White">
                            </RowExpAreaStyleDefault>
                            <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                            </EditCellStyleDefault>
                            <SelectedRowStyleDefault BackColor="#BECA98">
                            </SelectedRowStyleDefault>
                            <RowAlternateStyleDefault BackColor="#F1F1F1">
                            </RowAlternateStyleDefault>
                            <RowStyleDefault BorderWidth="1px" Font-Size="8pt" Font-Names="Verdana" BorderColor="#FFFFFF"
                                BorderStyle="Solid" Height="18px">
                                <Padding Left="3px" Right="3px"></Padding>
                                <BorderDetails WidthLeft="0px" WidthTop="0px"></BorderDetails>
                            </RowStyleDefault>
                        </DisplayLayout>
                    </igtbl:UltraWebGrid>
                    </span>
                    <span runat="server" id="spNoRecords" style='display:none;'>No matches found.</span>
                </igmisc:WebAsyncRefreshPanel>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt"  ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png"/>
                <COEManager:ImageButton ID="DeleteImageButton" runat="server" TypeOfButton="Delete" ButtonMode="ImgAndTxt" OnClientClick="javascript:return ConfirmDelete();" />
            </td>
        </tr>
    </table>
    <div id="confirmationPanel" style="visibility:hidden;"> 
      <div class="hd" runat="server" id="HeaderDiv"></div>  
      <div class="bd" style="text-align:center;">
        <img src="../../../App_Themes/Blue/Images/Warning.png" alt="Warning" style="float:left;" />
        <p runat="server" id="WarningMessage" class="DeleteDataviewWarning"></p>
        <div style="clear:both;">
            <COEManager:ImageButton ID="AbortImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnClientClick="YAHOO.coemanager.dvManager.confirmationPanel.hide();return false;" />
            <COEManager:ImageButton ID="OkImageButton" runat="server" TypeOfButton="Save" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" />
        </div>
      </div>
    </div>

    <!--The hidden div contains the hidden field to store the dataview ids user selected to be deleted.-->
    <div id="hiddenDiv" style="display:none">
        <asp:HiddenField ID="dataviewIdsHiddenField" runat="server"></asp:HiddenField>
    </div>
</asp:Content>
