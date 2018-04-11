<%@ Control Language="C#" AutoEventWireup="true" Inherits="EditTableAndFields" CodeBehind="EditTableAndFields.ascx.cs" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<script type="text/javascript">
        YAHOO.util.Event.addListener(window, 'load',
        function init(){
        YAHOO.namespace('dvconfirmationBox');
        
        YAHOO.dvconfirmationBox.ShowConfirmationBox = new YAHOO.widget.Panel('showconfirmationbox', 
        {width:'400px', fixedcenter:true, visible:false, modal:true, draggable:true, iframe:true, close:true, constraintoviewport:true});
        YAHOO.dvconfirmationBox.ShowConfirmationBox.render();
        //JIRA ID : CBOE-483
        YAHOO.dvconfirmationBox.ShowProgressBar = new YAHOO.widget.Panel('showprogressbar', 
        {width:'300px', fixedcenter:true, visible:false, modal:true, draggable:false, iframe:true, close:false, constraintoviewport:true});
        YAHOO.dvconfirmationBox.ShowProgressBar.render();
        //END CBOE-483
        YAHOO.dvconfirmationBox.callServerCompleted = function(arg, context){
            if(arg.indexOf("Index not created") >-1){
               alert("<%= Resources.Resource.Apply_Index_Error_Message %>");
            }
            else{
                var rowIndex = document.getElementById('<%= SelectedRowIndex.ClientID %>').value;

                if(rowIndex && rowIndex != 'undefined'){
                    var grid = igtbl_getGridById('<%= TableFieldsUltraWebGrid.ClientID %>');
                    var row = grid.Rows.getRow(rowIndex);
                    var IsDefaultQuery = row.getCellFromKey('IsDefaultQuery');
                    //JIRA ID : CBOE-482 Changed color from red to green and added tooltip to indexed column ASV 19032013
                    if(IsDefaultQuery != null)
                    {
                        IsDefaultQuery.Element.style.backgroundColor = "GreenYellow";
                        IsDefaultQuery.Element.title = "<%= Resources.Resource.ToolTip_IsDafaultQuery_Indexed_Field %>";
                    }//end CBOE-482
                }
            }
            YAHOO.dvconfirmationBox.ShowProgressBar.hide();//JIRA ID : CBOE-483
            return true;
        };
        

        YAHOO.dvconfirmationBox.callServer = function (arg, context) {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "arg", "YAHOO.dvconfirmationBox.callServerCompleted", "context") %>;
            YAHOO.dvconfirmationBox.ShowProgressBar.show();//JIRA ID : CBOE-483
        };

        YAHOO.dvconfirmationBox.SubscribeEvents = function(){
            YAHOO.util.Event.addListener('btnYes', 'click', function(args){
                YAHOO.dvconfirmationBox.callServer('createindex:'+document.getElementById('<%= IndexFieldID.ClientID %>').value, 'createindex');
                YAHOO.dvconfirmationBox.ShowConfirmationBox.hide();
            });
        };

        YAHOO.dvconfirmationBox.SubscribeEvents();
    });       
    
    function CheckPkUkConstraint(ColumnName,FieldID) {    
    var grid = igtbl_getGridById('<%= TableFieldsUltraWebGrid.ClientID %>');     
    var PkUkList=  document.getElementById('<%=PkUkList.ClientID %>').value;
    var stringArray=new Array();
    stringArray = PkUkList.split(","); 
     
     //Bug Fixing : CBOE-242
    for (_Index = 0; _Index < grid.Rows.length; _Index++) 
        {
        var flag= false;
         var IdCell = grid.Rows.getRow(_Index).getCellFromKey('ID');
            if(IdCell.getValue() == FieldID)
            {
                var IsUnique = grid.Rows.getRow(_Index).getCellFromKey('IsUniqueKey');  
                var chkIsUnique = IsUnique.Element.getElementsByTagName("input")[0];
                if (chkIsUnique != null && chkIsUnique.type == "checkbox" && chkIsUnique.checked == true) 
                {
                    for (i = 0; i < stringArray.length; i++)
                        { 
                            if(stringArray[i] == ColumnName) 
                            {        flag= true; break;}
                            else  flag= false;
                        } 
                        
                        if (flag == true)
                            return true;
                            else {
                                alert("Unique constraint cannot be applied on this field : "+ ColumnName);     
                                chkIsUnique.checked = false;
                                return false;     
                                } 
                }
            }
        }
  }
        
    function ShowConfirmationBox(ColumnName, FieldID) {
        var grid = igtbl_getGridById('<%= TableFieldsUltraWebGrid.ClientID %>');
        document.getElementById('<%= IndexFieldID.ClientID %>').value = null;
        document.getElementById('<%= SelectedRowIndex.ClientID %>').value = null;
        
        //iterating through the row and getting the cell with the key 
        for (_Index = 0; _Index < grid.Rows.length; _Index++) 
        {
            var IdCell = grid.Rows.getRow(_Index).getCellFromKey('ID');
            if(IdCell.getValue() == FieldID)
            {
                var IsDefaultQuery = grid.Rows.getRow(_Index).getCellFromKey('IsDefaultQuery');  
                var chkIsDefaultQuery = IsDefaultQuery.Element.getElementsByTagName("input")[0];
                if (chkIsDefaultQuery != null && chkIsDefaultQuery.type == "checkbox" && chkIsDefaultQuery.checked == true) 
                {
                    var lblColName = document.getElementById('<%= lblColName.ClientID %>');
                    if (lblColName != null) 
                    {

                        document.getElementById('<%= IndexFieldID.ClientID %>').value = FieldID;
                        document.getElementById('<%= SelectedRowIndex.ClientID %>').value = _Index;

                        lblColName.innerHTML = '<%= Resources.Resource.Apply_Index_Confirmation_Message %> ' + ColumnName +'?';
                        YAHOO.dvconfirmationBox.ShowConfirmationBox.show();
                    }
                }
                return;
            }
    }
    }

    function Cancleconfirmationbox_panel() {
        document.getElementById('<%= IndexFieldID.ClientID %>').value = null;
        YAHOO.dvconfirmationBox.ShowConfirmationBox.hide();
    }

     function CheckHeaderCheckBoxItem(HeaderChkObj, ItemCheckBoxKey) {
        var GridName = '<%=TableFieldsUltraWebGrid.ClientID%>';        
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

            //changing the status, Visible field value for primary key will be alway true
            var fieldID = document.getElementById('<%= PrevPkFieldIdHidden.ClientID %>').value;
            var IdCell = _grid.Rows.getRow(_Index).getCellFromKey('ID');
            if(_KeyValue != 'IsDefault')
            {
            if(IdCell.getValue() != fieldID) 
                _CheckBoxElement.checked = _CheckStatus;          
                }
                else
                {
                _CheckBoxElement.checked = _CheckStatus;
                }
        }       
    }
       
    function BeforeExitEditModeHandlerMethod(gridName,cellID)
    {
        cell=igtbl_getCellById(cellID);
        if(cell!=null && cell.Column.Key=="Alias")
        {
            var object=cell.getValue();
            while(object.indexOf("&amp;") !=-1)
            {
                object=object.replace("&amp;","&")
            }
            while(object.indexOf("&lt;") !=-1)
            {
                object= object.replace("&lt;","<")
            }
            while(object.indexOf("&gt;") !=-1)
            {
                object=object.replace("&gt;",">")
            }
           
            while(object.indexOf("&#37;") !=-1)
            {
                object=object.replace("&#37;","%")
            }
            cell.setValue(object);
            
         }
    }
    
    function TableFieldsUltraWebGrid_AfterExitEditModeHandler(gridName,cellID)
    {
        cell=igtbl_getCellById(cellID);
        if(cell!=null && cell.Column.Key=="Alias")
        {
           var object=cell.getValue();
           
            
            if(object ==null )
            {
                cell.setValue(cell._oldValue);
            }
            else
            {
                  var trimmed = object.toString().replace(/^\s+|\s+$/g, '') ;
                  if(trimmed == "" )
                  {
                    var name =cell.Row.getCellFromKey("Name").getValue();
                    if(name ==null)
                    {
                    cell.setValue(cell._oldValue);
                    }
                    else
                    {
                    cell.setValue(name);
                    }
                  }
                  else if( trimmed.indexOf('\"') > -1 )
                  {
                     cell.setValue(cell._oldValue);
                  }
            }

           var cnt=-1;
           var newObj='';
           var obj1=cell.getValue();
           var temp=new Array();
           temp = obj1.split(""); 
           for(i=0;i<temp.length;i++)
           {
                    if(temp[i] == '&')
                    {
                        cnt =cnt+5;
                        if(cnt< 30)
                        {
                            newObj =newObj+temp[i];
                        }
                        else
                        break;
                
                    }
                    else if(temp[i] == '>')
                    {
                    cnt =cnt+4;
                        if(cnt< 30)
                        {
                            newObj =newObj+temp[i];
                        }
                        else
                        break;
                    }
                    else if(temp[i] == '<')
                    {
                     cnt =cnt+4;
                        if(cnt< 30)
                        {
                            newObj =newObj+temp[i];
                        }
                        else
                        break;
                    }
                    else if(temp[i] == '%')
                    {
                     cnt =cnt+5;
                        if(cnt< 30)
                        {
                            newObj =newObj+temp[i];
                        } 
                        else
                        break;  
                    }
                    else
                    {
                        cnt= cnt+1;
                        if(cnt< 30)
                        {
                            newObj =newObj+temp[i];
                        }
                        else
                        break;
               
                    }
           
           }
            cell.setValue(newObj);

        }
    }

    function CheckedChanged(_KeyValue) {        
        var _GridName = '<%=TableFieldsUltraWebGrid.ClientID%>';    
        //get the object of the grid 
        var _grid = igtbl_getGridById(_GridName);
        // get the field item value excluding primary key
        var chkFirstItem = false;
        var checkItem = false;

        var flag = true;

        //iterating through the row and getting the cell with the key 
        for (_Index = 0; _Index < _grid.Rows.length; _Index++) {
            //getting the cell object
            var _CellObj = _grid.Rows.getRow(_Index).getCellFromKey(_KeyValue);
          
            //getting the checkbox with the object or tag name.  
            var _CheckBoxElement = _CellObj.Element.getElementsByTagName("input")[0];
                        
           // get Visible value of first item in grid (excluding primary key)
            if (checkItem != true && _CheckBoxElement.disabled == false) {               
                chkFirstItem = _CheckBoxElement.checked;
                checkItem = true;
            }

            // if all the items in grid are not true/false then checkbox vlaue for the Visbile/Default header will be false
            if (checkItem == true && chkFirstItem != _CheckBoxElement.checked) {
                flag = false;
                break;
            }
        }

        //getting the grid bands 
        var oBands = _grid.Bands;
        var oBand = oBands[0];  
        //getting the selected column in the row by passing the key
        var _col = oBand.getColumnFromKey(_KeyValue);
        
        // getting the checkbox present in the header
        var _CheckBoxElement = _col.Element.getElementsByTagName("input")[0];
        // if all the items in grid are true/false then checkbox value for the Visbile header will be set to true/false 
        if (flag == true) {            
            _CheckBoxElement.checked = chkFirstItem;           
        }
        else {            
            _CheckBoxElement.checked = false;        
        }        
    }
    
</script>
<asp:HiddenField ID="SelectedPKIndexHidden" runat="server" />
<asp:HiddenField ID="PrevPkFieldIdHidden" runat="server" />
<asp:HiddenField ID="PrevLookupDisabledHidden" runat="server" />
<asp:HiddenField ID="PrevVisibleFieldHidden" runat="server" />
<asp:HiddenField ID="IndexFieldID" runat="server" />
<asp:HiddenField ID="SelectedRowIndex" runat="server" />
<asp:HiddenField ID="PkUkList" runat="server" Value="" />
<asp:HiddenField ID="PkUkNotNullList" runat="server" Value="" />
<asp:ValidationSummary ID="ValidationSummary" runat="server" EnableClientScript="true"
    ShowSummary="true" DisplayMode="List" CssClass="NameDescUCTable" />
<table class="EditTable" onkeydown="return (event.keyCode!=13);">
    <tr valign="top">
        <td align="center" style="width: 800px;">
            <table id="ShowDetailsTable" runat="server" class="SmallTableDetails" style="padding: 3px;
                margin: 5px;">
                <tr valign="top">
                    <td colspan="2" style="padding: 3px; margin: 5px;">
                        <asp:Label runat="server" ID="TableDetailTitle" SkinID="Title"></asp:Label>
                    </td>
                </tr>
                <tr valign="top">
                    <td style="padding: 3px; margin: 5px;">
                        <asp:Label runat="server" ID="NameTitleLabel" SkinID="Text"></asp:Label>
                    </td>
                    <td align="left" style="padding: 3px; margin: 5px;">
                        <COEManager:TextBoxWithPopUp ID="NameTextBoxWithPopUp" runat="server" SummaryTextCssClass="EditTableTextBoxRO" />
                    </td>
                </tr>
                <tr valign="top">
                    <td style="padding: 3px; margin: 5px;">
                        <asp:Label runat="server" ID="AliasTitleLabel" SkinID="Text"></asp:Label>
                    </td>
                    <td align="left" style="padding: 3px; margin: 5px;">
                        <COEManager:TextBoxWithPopUp ID="AliasTextBoxWithPopUp" runat="server" ReadOnly="false"
                            SummaryTextCssClass="EditTableTextBox" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center" style="padding: 3px; margin: 5px;">
                    </td>
                </tr>
            </table>
        </td>
        <td style="width: 80px;" align="right">
            <COEManager:ImageButton ID="CancelImageButton" runat="server" ButtonMode="ImgAndTxt"
                TypeOfButton="Cancel" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png"
                HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnClientClick="SetProgressLayerVisibility(true);" />
        </td>
        <td style="width: 80px;" align="right">
            <COEManager:ImageButton ID="OkImageButton" runat="server" ButtonMode="ImgAndTxt"
                TypeOfButton="Save" CausesValidation="true" OnClientClick="SetProgressLayerVisibility(true);" />
        </td>
    </tr>
    <tr>
        <td align="right" colspan="3">
            <COEManager:ImageButton ID="CreateAliasFieldImageButton" runat="server" ButtonMode="ImgAndTxt"
                TypeOfButton="CreateAlias" />
        </td>
    </tr>
    <tr>
        <td colspan="3" align="center">
            <asp:Button runat="server" ID="ToggleButton" SkinID="ButtonLikeLink" />
            <asp:HiddenField runat="server" ID="ShowMasterSchemaFieldsHidden" />
        </td>
    </tr>
    <tr>
        <td colspan="3" align="right">
            <asp:Label runat="server" ID="lblVewSelectedError" Style="color: Red;"></asp:Label>
        </td>
    </tr>
    <tr>
    <td colspan="3" align="left" >
    <COEManager:ConfirmationArea ID="WarningControl" runat="server" Visible="false" />
    </td>
    </tr>
    <tr>
        <td colspan="3" align="center">
            <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
        </td>
    </tr>
    <tr valign="top">
        <td align="center" colspan="3">
            <igtbl:UltraWebGrid ID="TableFieldsUltraWebGrid" runat="server" Browser="Xml">
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
                            <igtbl:UltraGridColumn Key="SortOrder" BaseColumnName="SortOrder" DataType="System.Int32"
                                Hidden="true" IsBound="true" AllowUpdate="No">
                                <Header Fixed="true">
                                </Header>
                            </igtbl:UltraGridColumn>
                            <igtbl:UltraGridColumn Key="Name" IsBound="true" DataType="System.String" BaseColumnName="Name"
                                AllowUpdate="No" Width="135px">
                                <Header Fixed="true">
                                </Header>
                                <CellStyle HorizontalAlign="Left" CssClass="DisplayNameOnGrid" TextOverflow="Ellipsis">
                                </CellStyle>
                            </igtbl:UltraGridColumn>
                            <igtbl:TemplatedColumn BaseColumnName="Visible" Key="Visible" AllowUpdate="No" Width="75px">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="IsAllVisibleKeyCheckBox" runat="server" onclick="CheckHeaderCheckBoxItem(this,'Visible')" />
                                </HeaderTemplate>
                                <Header Fixed="true">
                                </Header>
                                <CellTemplate>
                                    <asp:CheckBox runat="server" ID="VisibleCheckBox" onclick="CheckedChanged('Visible')" />
                                </CellTemplate>
                                <CellStyle HorizontalAlign="center" />
                            </igtbl:TemplatedColumn>
                            <igtbl:TemplatedColumn BaseColumnName="IsUniqueKey" Key="IsUniqueKey" AllowUpdate="No"
                                Width="50px">
                                <Header Fixed="true">
                                </Header>
                                <CellTemplate>
                                    <asp:CheckBox runat="server" ID="IsUniqueKeyCheckBox" />
                                </CellTemplate>
                                <CellStyle HorizontalAlign="center" />
                            </igtbl:TemplatedColumn>
                            <igtbl:TemplatedColumn BaseColumnName="IsDefaultQuery" Key="IsDefaultQuery" AllowUpdate="No"
                                Width="40px">
                                <Header Fixed="true">
                                </Header>
                                <CellTemplate>
                                    <asp:CheckBox runat="server" ID="IsDefaultQueryCheckBox" />
                                </CellTemplate>
                                <CellStyle HorizontalAlign="center" />
                            </igtbl:TemplatedColumn>
                            <igtbl:TemplatedColumn BaseColumnName="IsDefault" Key="IsDefault" AllowUpdate="No"
                                Width="75px">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="IsAllDefaultKeyCheckBox" runat="server" onclick="CheckHeaderCheckBoxItem(this,'IsDefault')" />
                                </HeaderTemplate>
                                <Header Fixed="true">
                                </Header>
                                <CellTemplate>
                                    <asp:CheckBox runat="server" ID="IsDefaultCheckBox" onclick="CheckedChanged('IsDefault')" />
                                </CellTemplate>
                                <CellStyle HorizontalAlign="center" />
                            </igtbl:TemplatedColumn>
                            <igtbl:UltraGridColumn Key="ID" IsBound="true" DataType="System.Int32" BaseColumnName="ID"
                                Hidden="true" AllowUpdate="No">
                                <Header Fixed="true">
                                </Header>
                            </igtbl:UltraGridColumn>
                            <igtbl:UltraGridColumn Key="Alias" IsBound="true" FieldLen="30" DataType="System.String"
                                BaseColumnName="Alias" AllowUpdate="Yes" Width="135px">
                                <Header Fixed="true">
                                </Header>
                                <CellStyle HorizontalAlign="Left" CssClass="DisplayNameOnGrid" BackColor="White"
                                    BorderStyle="Inset" BorderColor="Gray" BorderWidth="1px" TextOverflow="Ellipsis">
                                </CellStyle>
                            </igtbl:UltraGridColumn>
                            <igtbl:TemplatedColumn BaseColumnName="LookUpColumn" Key="LookUpTable" AllowUpdate="Yes"
                                IsBound="false" Width="135px">
                                <Header Fixed="false">
                                </Header>
                            </igtbl:TemplatedColumn>
                            <igtbl:TemplatedColumn BaseColumnName="ParentColumn" Key="ParentColumn" AllowUpdate="No"
                                IsBound="false" Width="135px">
                                <Header Fixed="false">
                                </Header>
                            </igtbl:TemplatedColumn>
                            <igtbl:UltraGridColumn BaseColumnName="DataType" Key="DataType" AllowUpdate="No"
                                IsBound="true" Width="80px">
                                <Header Fixed="false">
                                </Header>
                                <CellStyle HorizontalAlign="Left" CssClass="DisplayNameOnGrid" TextOverflow="Ellipsis">
                                </CellStyle>
                            </igtbl:UltraGridColumn>
                            <igtbl:UltraGridColumn Key="IndexType" BaseColumnName="IndexType" IsBound="true"
                                AllowUpdate="Yes" Type="DropDownList" CellButtonDisplay="Always">
                                <Header Fixed="false">
                                </Header>
                                <CellStyle ForeColor="#000099">
                                </CellStyle>
                            </igtbl:UltraGridColumn>
                            <igtbl:UltraGridColumn Key="MimeType" BaseColumnName="MimeType" IsBound="true" AllowUpdate="Yes"
                                Width="165px" Type="DropDownList" CellButtonDisplay="Always">
                                <Header Fixed="false">
                                </Header>
                                <CellStyle ForeColor="#000099">
                                </CellStyle>
                            </igtbl:UltraGridColumn>
                            <igtbl:TemplatedColumn Key="Action" BaseColumnName="Action" IsBound="true" AllowUpdate="Yes"
                                Width="50px">
                                <Header Fixed="false">
                                </Header>
                            </igtbl:TemplatedColumn>
                        </Columns>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout RowHeightDefault="18px" Version="3.00" Name="TableFieldsUltraWebGrid"
                    ViewType="Hierarchical" BorderCollapseDefault="Separate" AllowUpdateDefault="Yes"
                    CellClickActionDefault="Edit" ActivationObject-AllowActivation="true" FilterOptionsDefault-RowFilterMode="AllRowsInBand"
                    UseFixedHeaders="true" StationaryMargins="Header">
                    <AddNewBox>
                        <BoxStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                            <BorderDetails ColorTop="White" WidthLeft="0px" WidthTop="1px" ColorLeft="White">
                            </BorderDetails>
                        </BoxStyle>
                    </AddNewBox>
                    <Pager>
                        <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                            <BorderDetails ColorTop="White" WidthLeft="0px" WidthTop="1px" ColorLeft="White">
                            </BorderDetails>
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
                    <FrameStyle Width="100%" BorderWidth="1px" Font-Size="8pt" Font-Names="Verdana" BorderStyle="Solid"
                        BackColor="#EBF3FB" Height="350px">
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
                    <ClientSideEvents ClickCellButtonHandler="CellButtonClick" CellClickHandler="CellButtonClick"
                        BeforeEnterEditModeHandler="BeforeExitEditModeHandlerMethod" AfterExitEditModeHandler="TableFieldsUltraWebGrid_AfterExitEditModeHandler"
                        InitializeLayoutHandler="InitializeGrid"></ClientSideEvents>
                </DisplayLayout>
            </igtbl:UltraWebGrid>
        </td>
    </tr>
</table>
<div id="container_confirmationbox">
    <div id="showconfirmationbox">
        <div class="hd">
            <%= Resources.Resource.Apply_Index_ConfirmationBox_Title %>
        </div>
        <div class="bd">
            <b>
                <asp:Label ID='lblColName' runat='server'></asp:Label></b>
            <br />
            <br />
            <input id="btnYes" type="button" value="Yes" style="width: 40px;" />
            &nbsp;&nbsp;
            <input id="btnNo" type="button" value="No" style="width: 40px;" onclick="Cancleconfirmationbox_panel()" />
        </div>
    </div>
</div>
<!-- JIRA ID : CBOE-483 -->
<div id="container_progressbar">
    <div id="showprogressbar">
        <div class="bd">
            <%= Resources.Resource.Apply_Index_showprogressbar_Title%>
            <br />
            <img id="ProgressImage" alt="Processing" src="../../../App_Themes/Common/Images/searching.gif" />
        </div>
    </div>
</div>
