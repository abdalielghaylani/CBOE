<%@ Page Language="C#" MasterPageFile="~/Forms/Master/DataViewManager.Master" AutoEventWireup="true"
    CodeBehind="DataviewBoard.aspx.cs" Inherits="Manager.Forms.DataViewManager.ContentArea.DataviewBoard"
    Title="Dataview Board" %>

<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/TablesTreeView.ascx" TagName="TablesTreeView"
    TagPrefix="cm" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/TableSummary.ascx" TagName="TableSummary"
    TagPrefix="cm" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/DuplicateTable.ascx" TagName="DuplicateTable"
    TagPrefix="cm" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/SchemaSummary.ascx" TagName="SchemaSummary"
    TagPrefix="cm" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/ToolBar.ascx" TagName="Toolbar"
    TagPrefix="cm" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework, Version=17.1.0.0, Culture=neutral, PublicKeyToken=1e3754866626dfbf"
    Namespace="CambridgeSoft.COE.Framework.Controls" TagPrefix="cc2" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <iframe id="ParentDiv" class="BackgroundHidden" frameborder="0" scrolling="no"></iframe>
    <script language="javascript" type="text/javascript">
    function SetProgressLayerVisibility(visible)
    {
        if(visible)
        {
            document.getElementById('UpdateProgressDiv').style.display = 'block';
            document.getElementById('ParentDiv').className = 'BackgroundVisible';
        }
        else
        {
            document.getElementById('UpdateProgressDiv').style.display = 'none';
            document.getElementById('ParentDiv').className ='BackgroundHidden';
        }
    }       
    

    function delete_Confirm()
    {
           
        var x = confirm('<%= Resources.Resource.ConfirmTableDeletion_Alert_Text %>','Remove Table Confirmation');
          if (x)
             document.getElementById('<%= this.hdnRemoveTable.ClientID %>').value='1';
          else
            document.getElementById('<%= this.hdnRemoveTable.ClientID %>').value='0';

    }
    function ToggleVisibility(elementid)
    {
        var element = document.getElementById(elementid);
        if(element.style.display == 'none')
        {
            element.style.display = 'block';
            element.previousSibling.className = 'CollapsablePanelExpanded';
        }
        else
        {
            element.style.display = 'none';
            element.previousSibling.className = 'CollapsablePanelCollapsed';
        }
    }
    
    function ShowModalFrame(url, headerText)
    {
        if(typeof(InitModalIframeControl_<%= this.MoldalIFrame.ClientID %>) == 'function')
            InitModalIframeControl_<%= this.MoldalIFrame.ClientID %>(url, headerText, true);
    }
    
    function CloseModal(refresh)
    {
        if(typeof(CloseCOEModalIframe) == 'function')
            CloseCOEModalIframe();
    }   
    function ShowConfirmationArea(message)
    {
        if(message!='')
        { 
            document.getElementById('<%=this.ConfirmationAreaUserControl.ConfirmationMessageLabelClientID %>').innerHTML = message;
            document.getElementById('ConfirmationAreaDiv').style.display = 'block';
        }
        else
        {
            document.getElementById('ConfirmationAreaDiv').style.display = 'none';
        }
    }
    //Show or hide Duplicate table user control
    function ShowDuplicateTable(visible)
    {
        if(visible)
        {
            var tableId = document.getElementById('<%= this.TableSummary.SelectedTableIDHiddenClientID %>').value;
            YAHOO.DataviewBoardNS.callServer('DuplicateTable: ' + tableId,"");
        }
        else
        {
            var tableId = document.getElementById('<%= this.DuplicateTable.SelectedTableIDHiddenClientID %>').value;
            var recordId = YAHOO.DataviewBoardNS.LeftPanel.DataTable.getLastSelectedRecord();   
            var record = YAHOO.DataviewBoardNS.LeftPanel.DataTable.getRecord(recordId);
            YAHOO.DataviewBoardNS.callServer('SelectTable: ' + tableId,record);
        }
    }
    //Call server method to create duplicate table and add to DataViewManager
    function DoDuplicateTable()
    {
        var tableId = document.getElementById('<%= this.DuplicateTable.SelectedTableIDHiddenClientID %>').value;
        var aliastextbox = document.getElementById('<%= this.DuplicateTable.AliasTextBoxClientId %>');
        YAHOO.DataviewBoardNS.callServer('TableDuplicated: ' + tableId + "," + aliastextbox.value,"");
    }
    //Validating controls in DuplicateTable user control
    function ValidateDuplicateTable() 
    {

        if (typeof (Page_ClientValidate) == 'function') 
        {
            Page_ClientValidate('DuplicateTable');
        }

        if (Page_IsValid) 
        {
            return 1;                
        }
        else 

        {
            return 0;
        }
    }
    //Enable or disable validation controls of DuplicateTable user control
    function DuplicateTableValidation(enable)
    {
        var aliasrequiredfield = document.getElementById('<%= this.DuplicateTable.AliasRequiredFieldClientID %>');
        var AliasRegExpValidator = document.getElementById('<%= this.DuplicateTable.AliasRegExpValidatorClientID %>');
        ValidatorEnable(aliasrequiredfield,enable);
        ValidatorEnable(AliasRegExpValidator,enable);
        ValidatorValidate(aliasrequiredfield);
        ValidatorValidate(AliasRegExpValidator);
    }
    function RemoveSchemaFromMaster()
    {
        document.getElementById('<%= this.hdRemoveSchemaConfirmation.ClientID %>').value=0;
        if (!this.disabled) 
        {
            if(confirm('Are you sure you want to delete all the tables from this schema?','Remove Table Confirmation')) 
            { 
                document.getElementById('<%= this.hdRemoveSchemaConfirmation.ClientID %>').value=1;
            } 
        } 
    }
    </script>
    <table class="PagesContentTable">
        <tr>
            <td align="center" colspan="2">
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr valign="top">
            <td align="left">
                <div class="BaseTable" style="width: 400px">
                <asp:HiddenField id="hdRemoveSchemaConfirmation" Value="0"  runat="server"/>
                 <asp:HiddenField id="hdnRemoveTable" Value="0"  runat="server"/>
                <asp:HiddenField id="hdnTableAlias" Value="0" runat="server"/>
                    <cm:TablesTreeView ID="TablesTreeViewUserControl" runat="server"></cm:TablesTreeView>
                </div>
            </td>
            <td align="right" style="width: 600px">
                <div id="ConfirmationAreaDiv" style="display: none;">
                    <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" />
                </div>
                <div class="BaseTable" style="width: 558px">
                <asp:HiddenField id="hdnIsBaseTable" Value="-1"  runat="server"/>
                    <cm:Toolbar ID="Toolbar" runat="server" />
                </div>
                <div class="BaseTable" style="width: 558px; text-align: left;">          
                    <div id="TableSummaryDiv" style="display: none;">
                        <cm:TableSummary ID="TableSummary" runat="server" />
                    </div>
                    <div id="SchemaSummaryDiv" style="display: block;">
                        <cm:SchemaSummary ID="SchemaSummary" runat="server" />
                    </div>
                    <div id="DuplicateTableDiv" style="display: none;">
                        <cm:DuplicateTable ID="DuplicateTable" runat="server" />
                    </div>
                </div>
                <div class="BaseTable" style="width: 558px">
                    <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel"
                        ButtonMode="ImgAndTxt" OnClientClick="return ConfirmCancel();" CausesValidation="false"
                        ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
                    <COEManager:ImageButton ID="DoneImageButton" runat="server" TypeOfButton="Submit"
                        ButtonMode="ImgAndTxt" OnClientClick="SetProgressLayerVisibility(true); return true;" />
                </div>
            </td>
            <td>
                <div id="UpdateProgressDiv" style="z-index: 340; display: none;">
                    <img id="ProgressImage" alt="Processing" src="../../../App_Themes/Common/Images/searching.gif"
                        style="position: absolute; top: 72px; left: 762px; z-index: 340;" />
                </div>
            </td>
        </tr>
    </table>
    <script language="javascript" type="text/javascript">
    YAHOO.util.Event.addListener(window, "load", function() {
        YAHOO.namespace('DataviewBoardNS');
                
        YAHOO.DataviewBoardNS.myRowFormatter = function(elTr, oRecord) {
            Dom.addClass(elTr, 'myRow');
            return true;
        };
        
        YAHOO.DataviewBoardNS.selectableRowFormatter = function(elTr, oRecord) {
            Dom.addClass(elTr, 'selectableRow');
            return true;
        };
        
       YAHOO.DataviewBoardNS.removeTableButtonFormatter = function(elLiner, oRecord, oColumn, oData) {
            if(oRecord.getData('isbasetable') == 'False')
            {
                var obj = document.getElementById('<% =this.hdnIsBaseTable.ClientID %>').value;
                elLiner.innerHTML = '<input type="button" value="<%= Resources.Resource.Remove_Label_Text %>" onclick="delete_Confirm();DisableBaseTableLink('+ obj+ ');"></input>';
                return true;
            }
        };

        
        YAHOO.DataviewBoardNS.showRightPanelDiv = function(divId) {
            var divids = ['TableSummaryDiv', 'SchemaSummaryDiv', 'DuplicateTableDiv'];
            for(i = 0; i < divids.length; i++)
            {
                document.getElementById(divids[i]).style.display = (divId == divids[i]? "block" : "none");
            }                           
            if(divId == 'DuplicateTableDiv')
            {
                DuplicateTableValidation(true);
            }
            else
            {
                DuplicateTableValidation(false);
            }
        
        };
        
        
        //---------------------------- SchemaSummary ----------------------------//
        YAHOO.namespace('DataviewBoardNS.SchemaSummary');
        
        YAHOO.DataviewBoardNS.SchemaSummary.PkDataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.SchemaSummary.PkDataSource.responseSchema = { 
            fields : ['fieldid', 'tablealias', 'fieldalias', 'fieldtype']
        };
        <%= this.SchemaSummary.GetPkDataSource(string.Empty) %>
        
        YAHOO.DataviewBoardNS.SchemaSummary.PkDataTableCols = [
            { key: 'fieldid', sortable: true, width: 35, label: '<%= Resources.Resource.ID_ColHeader_Caption %>' },
            { key: 'tablealias', sortable: true, width: 150, label: '<%= Resources.Resource.Table_Label_Text %>' },
            { key: 'fieldalias', sortable: true, width: 145, label: '<%= Resources.Resource.PrimaryKey_ColHeader_Caption %>' },
            { key: 'fieldtype', sortable: true, width: 45, label: '<%= Resources.Resource.Type_Label_Text %>' }
        ];

        YAHOO.DataviewBoardNS.SchemaSummary.PkPaginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            alwaysVisible : false
        });

        YAHOO.DataviewBoardNS.SchemaSummary.PkDataTableConf = {
            paginator : YAHOO.DataviewBoardNS.SchemaSummary.PkPaginator,
            formatRow: YAHOO.DataviewBoardNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.NoPkInSchema_Label_Text %>'
        };
        
        YAHOO.DataviewBoardNS.SchemaSummary.PkDataTable = new YAHOO.widget.DataTable('SchemaSummaryPKTbl',YAHOO.DataviewBoardNS.SchemaSummary.PkDataTableCols,YAHOO.DataviewBoardNS.SchemaSummary.PkDataSource,YAHOO.DataviewBoardNS.SchemaSummary.PkDataTableConf);
        
        YAHOO.DataviewBoardNS.SchemaSummary.PkFilterTimeout = null;
        YAHOO.DataviewBoardNS.SchemaSummary.PkUpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.SchemaSummary.PkFilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.SchemaSummary.PkDataTable.getState();            

            // Get filtered data
            YAHOO.DataviewBoardNS.SchemaSummary.PkDataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.SchemaSummary.PkDataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.SchemaSummary.PkDataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.SchemaSummary.PkDataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.SchemaSummary.PkRefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.SchemaSummary.PkFilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.SchemaSummary.PkUpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.SchemaSummary.PkUpdateFilter();
                return true;
            };
            
        YAHOO.DataviewBoardNS.SchemaSummary.RelDataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.SchemaSummary.RelDataSource.responseSchema = { 
            fields : ['parenttable', 'childtable', 'reltype']
        };
        <%= this.SchemaSummary.GetRelDataSource(string.Empty) %>
        
        YAHOO.DataviewBoardNS.SchemaSummary.RelDataTableCols = [
            { key: 'parenttable', sortable: true, width: 178, label: '<%= Resources.Resource.ParentTable_ColHeader_Caption %>' },
            { key: 'childtable', sortable: true, width: 178, label: '<%= Resources.Resource.ChildTable_ColHeader_Caption %>' },
            { key: 'reltype', sortable: true, width
            : 40, label: '<%= Resources.Resource.Type_Label_Text %>' }
        ];

        YAHOO.DataviewBoardNS.SchemaSummary.RelPaginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            alwaysVisible : false
        });

        YAHOO.DataviewBoardNS.SchemaSummary.RelDataTableConf = {
            paginator : YAHOO.DataviewBoardNS.SchemaSummary.RelPaginator,
            formatRow: YAHOO.DataviewBoardNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.NoRelInSchema_Label_Text %>'
        };
        
        YAHOO.DataviewBoardNS.SchemaSummary.RelDataTable = new YAHOO.widget.DataTable('SchemaSummaryRelTbl',YAHOO.DataviewBoardNS.SchemaSummary.RelDataTableCols,YAHOO.DataviewBoardNS.SchemaSummary.RelDataSource,YAHOO.DataviewBoardNS.SchemaSummary.RelDataTableConf);
        
        YAHOO.DataviewBoardNS.SchemaSummary.RelFilterTimeout = null;
        YAHOO.DataviewBoardNS.SchemaSummary.RelUpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.SchemaSummary.RelFilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.SchemaSummary.RelDataTable.getState();
            state.sortedBy = {key:'parenttable', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.DataviewBoardNS.SchemaSummary.RelDataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.SchemaSummary.RelDataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.SchemaSummary.RelDataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.SchemaSummary.RelDataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.SchemaSummary.RelRefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.SchemaSummary.RelFilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.SchemaSummary.RelUpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.SchemaSummary.RelUpdateFilter();
                return true;
            };

        YAHOO.DataviewBoardNS.SchemaSummary.LookupDataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.SchemaSummary.LookupDataSource.responseSchema = { 
            fields : ['table', 'lookup', 'type']
        };
        <%= this.SchemaSummary.GetLookupDataSource(string.Empty) %>
        
        YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTableCols = [
            { key: 'table', sortable: true, width: 178, label: '<%= Resources.Resource.Table_Label_Text %>' },
            { key: 'lookup', sortable: true, width: 178, label: '<%= Resources.Resource.Lookup_ColHeader_Caption %>' },
            { key: 'type', sortable: true, width: 40, label: '<%= Resources.Resource.Type_Label_Text %>' }
        ];

        YAHOO.DataviewBoardNS.SchemaSummary.LookupPaginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            alwaysVisible : false
        });

        YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTableConf = {
            paginator : YAHOO.DataviewBoardNS.SchemaSummary.LookupPaginator,
            formatRow: YAHOO.DataviewBoardNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.NoLookupInSchema_Label_Text %>'
        };
        
        YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTable = new YAHOO.widget.DataTable('SchemaSummaryLookupTbl',YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTableCols,YAHOO.DataviewBoardNS.SchemaSummary.LookupDataSource,YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTableConf);
        
        YAHOO.DataviewBoardNS.SchemaSummary.LookupFilterTimeout = null;
        YAHOO.DataviewBoardNS.SchemaSummary.LookupUpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.SchemaSummary.LookupFilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTable.getState();
            state.sortedBy = {key:'table', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.DataviewBoardNS.SchemaSummary.LookupDataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.SchemaSummary.LookupRefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.SchemaSummary.LookupFilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.SchemaSummary.LookupUpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.SchemaSummary.LookupUpdateFilter();
                return true;
            };
            
        //---------------------------- SchemaSummary ----------------------------//


        //---------------------------- TableSummary ----------------------------//
        YAHOO.namespace('DataviewBoardNS.TableSummary');
               
        YAHOO.DataviewBoardNS.TableSummary.PkDataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.TableSummary.PkDataSource.responseSchema = { 
            fields : ['fieldid', 'fieldalias', 'fieldtype']
        };
        <%= this.TableSummary.GetPkDataSource(this.TableSummary.SelectedTableId) %>
        
        YAHOO.DataviewBoardNS.TableSummary.PkDataTableCols = [
            { key: 'fieldid', sortable: true, width: 48, label: '<%= Resources.Resource.ID_ColHeader_Caption %>' },
            { key: 'fieldalias', sortable: true, width: 300, label: '<%= Resources.Resource.PrimaryKey_ColHeader_Caption %>' },
            { key: 'fieldtype', sortable: true, width: 48, label: '<%= Resources.Resource.Type_Label_Text %>' }
        ];

        YAHOO.DataviewBoardNS.TableSummary.PkPaginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            alwaysVisible : false
        });

        YAHOO.DataviewBoardNS.TableSummary.PkDataTableConf = {
            paginator : YAHOO.DataviewBoardNS.TableSummary.PkPaginator,
            formatRow: YAHOO.DataviewBoardNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.NoPkInTable_Label_Text %>'
        };
        
        YAHOO.DataviewBoardNS.TableSummary.PkDataTable = new YAHOO.widget.DataTable('TableSummaryPKTbl',YAHOO.DataviewBoardNS.TableSummary.PkDataTableCols,YAHOO.DataviewBoardNS.TableSummary.PkDataSource,YAHOO.DataviewBoardNS.TableSummary.PkDataTableConf);
        
        YAHOO.DataviewBoardNS.TableSummary.PkFilterTimeout = null;
        YAHOO.DataviewBoardNS.TableSummary.PkUpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.TableSummary.PkFilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.TableSummary.PkDataTable.getState();
            state.sortedBy = {key:'fieldalias', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.DataviewBoardNS.TableSummary.PkDataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.TableSummary.PkDataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.TableSummary.PkDataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.TableSummary.PkDataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.TableSummary.PkRefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.TableSummary.PkFilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.TableSummary.PkUpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.TableSummary.PkUpdateFilter();
                return true;
            };
            
        YAHOO.DataviewBoardNS.TableSummary.RelDataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.TableSummary.RelDataSource.responseSchema = { 
            fields : ['parenttable', 'childtable', 'reltype']
        };
        <%= this.TableSummary.GetRelDataSource(this.TableSummary.SelectedTableId) %>
        
        YAHOO.DataviewBoardNS.TableSummary.RelDataTableCols = [
            { key: 'parenttable', sortable: true, width: 178, label: '<%= Resources.Resource.ParentTable_ColHeader_Caption %>' },
            { key: 'childtable', sortable: true, width: 178, label: '<%= Resources.Resource.ChildTable_ColHeader_Caption %>' },
            { key: 'reltype', sortable: true, width: 41, label: '<%= Resources.Resource.Type_Label_Text %>' }
        ];

        YAHOO.DataviewBoardNS.TableSummary.RelPaginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            alwaysVisible : false
        });

        YAHOO.DataviewBoardNS.TableSummary.RelDataTableConf = {
            paginator : YAHOO.DataviewBoardNS.TableSummary.RelPaginator,
            formatRow: YAHOO.DataviewBoardNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.NoRelInTable_Label_Text %>'
        };
        
        YAHOO.DataviewBoardNS.TableSummary.RelDataTable = new YAHOO.widget.DataTable('TableSummaryRelTbl',YAHOO.DataviewBoardNS.TableSummary.RelDataTableCols,YAHOO.DataviewBoardNS.TableSummary.RelDataSource,YAHOO.DataviewBoardNS.TableSummary.RelDataTableConf);
        
        YAHOO.DataviewBoardNS.TableSummary.RelFilterTimeout = null;
        YAHOO.DataviewBoardNS.TableSummary.RelUpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.TableSummary.RelFilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.TableSummary.RelDataTable.getState();
            state.sortedBy = {key:'parenttable', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.DataviewBoardNS.TableSummary.RelDataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.TableSummary.RelDataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.TableSummary.RelDataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.TableSummary.RelDataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.TableSummary.RelRefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.TableSummary.RelFilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.TableSummary.RelUpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.TableSummary.RelUpdateFilter();
                return true;
            };

        YAHOO.DataviewBoardNS.TableSummary.LookupDataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.TableSummary.LookupDataSource.responseSchema = { 
            fields : ['lookup', 'type']
        };
        <%= this.TableSummary.GetLookupDataSource(this.TableSummary.SelectedTableId) %>
        
        YAHOO.DataviewBoardNS.TableSummary.LookupDataTableCols = [
            { key: 'lookup', sortable: true, width: 378, label: '<%= Resources.Resource.Lookup_ColHeader_Caption %>' },
            { key: 'type', sortable: true, width: 40, label: '<%= Resources.Resource.Type_Label_Text %>' }
        ];

        YAHOO.DataviewBoardNS.TableSummary.LookupPaginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            alwaysVisible : false
        });

        YAHOO.DataviewBoardNS.TableSummary.LookupDataTableConf = {
            paginator : YAHOO.DataviewBoardNS.TableSummary.LookupPaginator,
            formatRow: YAHOO.DataviewBoardNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.NoLookupInTable_Label_Text %>'
        };
        
        YAHOO.DataviewBoardNS.TableSummary.LookupDataTable = new YAHOO.widget.DataTable('TableSummaryLookupTbl',YAHOO.DataviewBoardNS.TableSummary.LookupDataTableCols,YAHOO.DataviewBoardNS.TableSummary.LookupDataSource,YAHOO.DataviewBoardNS.TableSummary.LookupDataTableConf);
        
        YAHOO.DataviewBoardNS.TableSummary.LookupFilterTimeout = null;
        YAHOO.DataviewBoardNS.TableSummary.LookupUpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.TableSummary.LookupFilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.TableSummary.LookupDataTable.getState();
            state.sortedBy = {key:'lookup', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.DataviewBoardNS.TableSummary.LookupDataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.TableSummary.LookupDataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.TableSummary.LookupDataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.TableSummary.LookupDataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.TableSummary.LookupRefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.TableSummary.LookupFilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.TableSummary.LookupUpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.TableSummary.LookupUpdateFilter();
                return true;
            };
            
        YAHOO.DataviewBoardNS.TableSummary.TagsDataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.TableSummary.TagsDataSource.responseSchema = { 
            fields : ['tag']
        };
        <%= this.TableSummary.GetTagsDataSource(this.TableSummary.SelectedTableId) %>
        
        YAHOO.DataviewBoardNS.TableSummary.TagsDataTableCols = [
            { key: 'tag', sortable: true, width: 440, label: '<%= Resources.Resource.Tags_Label_Text %>' }
        ];

        YAHOO.DataviewBoardNS.TableSummary.TagsPaginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            alwaysVisible : false
        });

        YAHOO.DataviewBoardNS.TableSummary.TagsDataTableConf = {
            paginator : YAHOO.DataviewBoardNS.TableSummary.TagsPaginator,
            formatRow: YAHOO.DataviewBoardNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.NoTagsInTable_Label_Text %>'
        };
        
        YAHOO.DataviewBoardNS.TableSummary.TagsDataTable = new YAHOO.widget.DataTable('TableSummaryTagsTbl',YAHOO.DataviewBoardNS.TableSummary.TagsDataTableCols,YAHOO.DataviewBoardNS.TableSummary.TagsDataSource,YAHOO.DataviewBoardNS.TableSummary.TagsDataTableConf);
        
        YAHOO.DataviewBoardNS.TableSummary.TagsFilterTimeout = null;
        YAHOO.DataviewBoardNS.TableSummary.TagsUpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.TableSummary.TagsFilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.TableSummary.TagsDataTable.getState();
            state.sortedBy = {key:'tag', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.DataviewBoardNS.TableSummary.TagsDataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.TableSummary.TagsDataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.TableSummary.TagsDataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.TableSummary.TagsDataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.TableSummary.TagsRefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.TableSummary.TagsFilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.TableSummary.TagsUpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.TableSummary.TagsUpdateFilter();
                return true;
            };


        //---------------------------- Index summary ---------------------------//
        YAHOO.DataviewBoardNS.TableSummary.IndexDataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.TableSummary.IndexDataSource.responseSchema = { 
            fields : ['fieldid','indexname']
        };
        <%= this.TableSummary.GetIndexesDataSource(this.TableSummary.SelectedTableId) %>
        
        YAHOO.DataviewBoardNS.TableSummary.IndexDataTableCols = [
            { key: 'fieldid', sortable: true, width: 208, label: '<%= Resources.Resource.Name_ColHeader_Caption %>' },
            { key: 'indexname', sortable: true, width: 210, label: '<%= Resources.Resource.Indexname_ColHeader_Caption %>' }
        ];

        YAHOO.DataviewBoardNS.TableSummary.IndexPaginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            alwaysVisible : false
        });

        YAHOO.DataviewBoardNS.TableSummary.IndexDataTableConf = {
            paginator : YAHOO.DataviewBoardNS.TableSummary.IndexPaginator,
            formatRow: YAHOO.DataviewBoardNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.NoIndexInTable_Label_Text %>'
        };
        
        YAHOO.DataviewBoardNS.TableSummary.IndexDataTable = new YAHOO.widget.DataTable('TableSummaryIndexesTbl',YAHOO.DataviewBoardNS.TableSummary.IndexDataTableCols,YAHOO.DataviewBoardNS.TableSummary.IndexDataSource,YAHOO.DataviewBoardNS.TableSummary.IndexDataTableConf);
        
        YAHOO.DataviewBoardNS.TableSummary.IndexFilterTimeout = null;
        YAHOO.DataviewBoardNS.TableSummary.IndexUpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.TableSummary.IndexFilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.TableSummary.IndexDataTable.getState();
            state.sortedBy = {key:'fieldid', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.DataviewBoardNS.TableSummary.IndexDataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.TableSummary.IndexDataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.TableSummary.IndexDataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.TableSummary.IndexDataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.TableSummary.IndexRefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.TableSummary.IndexFilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.TableSummary.IndexUpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.TableSummary.IndexUpdateFilter();
                return true;
            };
        
        //---------------------------- Index summary ---------------------------//


        //---------------------------- TableSummary ----------------------------//
        
        //---------------------------- LeftPanel ----------------------------//
        YAHOO.namespace('DataviewBoardNS.LeftPanel');
         
        YAHOO.DataviewBoardNS.LeftPanel.DataSource = new YAHOO.util.LocalDataSource();
        YAHOO.DataviewBoardNS.LeftPanel.DataSource.responseSchema = { 
            fields : ['tablealias', 'tableschema', 'tableid', 'tablename', 'isbasetable']
        };
        <%= this.TablesTreeViewUserControl.GetTablesDataSource(string.Empty) %>
        YAHOO.DataviewBoardNS.LeftPanel.DataSource.doBeforeCallback = function (req,raw,res,cb) {
            // This is the filter function
            var data     = res.results || [],
                filtered = [],
                i,l;

            if (req) {
                if(req == '*')
                    return res;

                req = req.toLowerCase();
                for (i = 0, l = data.length; i < l; ++i) {
                    if (data[i].tablealias.toLowerCase().indexOf(req) != -1) {
                        filtered.push(data[i]);
                    }
                }
                res.results = filtered;
            }

            return res;
        };
        
        YAHOO.DataviewBoardNS.LeftPanel.DataTableCols = [
            { key: 'tablealias', sortable: true, width: 280, label: '<%= Resources.Resource.Table_Label_Text %>' },
            { key: 'button', label: '<%= Resources.Resource.Remove_Label_Text %>', formatter: YAHOO.DataviewBoardNS.removeTableButtonFormatter }
        ];

        YAHOO.DataviewBoardNS.LeftPanel.Paginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            containers    : ["LeftPanelPaginatorTop", "LeftPanelPaginatorBottom"]
        });

        YAHOO.DataviewBoardNS.LeftPanel.DataTableConf = {
            paginator : YAHOO.DataviewBoardNS.LeftPanel.Paginator,
            formatRow: YAHOO.DataviewBoardNS.selectableRowFormatter
        };
        
        YAHOO.DataviewBoardNS.LeftPanel.DataTable = new YAHOO.widget.DataTable('LeftPanelTbl',YAHOO.DataviewBoardNS.LeftPanel.DataTableCols,YAHOO.DataviewBoardNS.LeftPanel.DataSource,YAHOO.DataviewBoardNS.LeftPanel.DataTableConf);
        YAHOO.DataviewBoardNS.LeftPanel.DataTable.set("selectionMode", "single");
        //tooltip for table
        var tt = new YAHOO.widget.Tooltip("myTooltip");
        YAHOO.DataviewBoardNS.LeftPanel.DataTable.on('cellMouseoverEvent', function (oArgs) {				
				var target = oArgs.target;
				var column = this.getColumn(target);
				if (column.key == 'tablealias') { 
					var record = this.getRecord(target);
					var description = 'ID=' + record.getData('tableid') + ' | Name=' + record.getData('tablename') + ' (' + record.getData('tablealias') + ') ' + '| Database=' + record.getData('tableschema') ;
					var xy = [parseInt(oArgs.event.clientX,10) + 10 ,parseInt(oArgs.event.clientY,10) + 10 ];
					showTimer = window.setTimeout(function() {
						tt.setBody(description);
						tt.cfg.setProperty('xy',xy);
						tt.show();
						hideTimer = window.setTimeout(function() {
							tt.hide();
						},5000);
					},500);
				}
				if (showtimer) {
					window.clearTimeout(showTimer);
					showTimer = 0;
				}
			});
			YAHOO.DataviewBoardNS.LeftPanel.DataTable.on('cellMouseoutEvent', function (oArgs) {
				if (showTimer) {
					window.clearTimeout(showTimer);
					showTimer = 0;
				}
				if (hideTimer) {
					window.clearTimeout(hideTimer);
					hideTimer = 0;
				}
				tt.hide();
			});
        YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp = new YAHOO.widget.AutoComplete('<%=this.TablesTreeViewUserControl.TableTextBoxClientID %>','AutoCompleteContainer', YAHOO.DataviewBoardNS.LeftPanel.DataSource);
        YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.useIFrame = true; // Enable an iFrame shim under the container element
        YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.typeAhead = true; // Enable type ahead
        YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.resultTypeList = false;
        YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.applyLocalFilter = false;
        
        YAHOO.DataviewBoardNS.LeftPanel.FilterTimeout = null;
        YAHOO.DataviewBoardNS.LeftPanel.UpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.DataviewBoardNS.LeftPanel.FilterTimeout = null;
            // Reset sort
            var state = YAHOO.DataviewBoardNS.LeftPanel.DataTable.getState();

            // Get filtered data
            YAHOO.DataviewBoardNS.LeftPanel.DataSource.sendRequest(query,{
                success : YAHOO.DataviewBoardNS.LeftPanel.DataTable.onDataReturnInitializeTable,
                failure : YAHOO.DataviewBoardNS.LeftPanel.DataTable.onDataReturnInitializeTable,
                scope   : YAHOO.DataviewBoardNS.LeftPanel.DataTable,
                argument: state
            });
        };
        
        YAHOO.DataviewBoardNS.LeftPanel.RefreshTable = function (query) {
                clearTimeout(YAHOO.DataviewBoardNS.LeftPanel.FilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.DataviewBoardNS.LeftPanel.UpdateFilter(query);
                else
                    YAHOO.DataviewBoardNS.LeftPanel.UpdateFilter();
                return true;
            };
        //---------------------------- LeftPanel ----------------------------//
        
        
        YAHOO.DataviewBoardNS.initializeEmptyMessages = function()
        {
            YAHOO.DataviewBoardNS.SchemaSummary.PkDataTable.set('MSG_EMPTY', '<%= Resources.Resource.NoPkInSchema_Label_Text %>');
            YAHOO.DataviewBoardNS.SchemaSummary.RelDataTable.set('MSG_EMPTY', '<%= Resources.Resource.NoRelInSchema_Label_Text %>');
            YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTable.set('MSG_EMPTY', '<%= Resources.Resource.NoLookupInSchema_Label_Text %>');
            YAHOO.DataviewBoardNS.LeftPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.EmptySchema_Label_Text %>');
        }
        
        YAHOO.DataviewBoardNS.initializeEmptyMessages();
        
        YAHOO.DataviewBoardNS.schemaFiltered = function(schema) {
                YAHOO.DataviewBoardNS.showRightPanelDiv('SchemaSummaryDiv');
                
                YAHOO.DataviewBoardNS.initializeEmptyMessages();
                
                YAHOO.DataviewBoardNS.LeftPanel.RefreshTable('');
                YAHOO.DataviewBoardNS.SchemaSummary.PkRefreshTable('');
                YAHOO.DataviewBoardNS.SchemaSummary.RelRefreshTable('');
                YAHOO.DataviewBoardNS.SchemaSummary.LookupRefreshTable('');
                
                document.getElementById('<%= this.SchemaSummary.SchemaNameTextBoxClientID %>').setAttribute('value', schema);
                    
                if(document.getElementById('<%= this.Toolbar.CurrentSchemaHiddenClientID %>') != null)
                    document.getElementById('<%= this.Toolbar.CurrentSchemaHiddenClientID %>').setAttribute('value', schema);
                return true;
            };

        YAHOO.DataviewBoardNS.tableSelected = function(tableRecord) {
            var tableId = tableRecord.getData('tableid');
            var aliasValue=document.getElementById('<%= this.hdnTableAlias.ClientID %>').value;
            YAHOO.DataviewBoardNS.showRightPanelDiv('TableSummaryDiv');
            document.getElementById('<%= this.TableSummary.NameTextBoxClientID %>').setAttribute('value', tableRecord.getData('tablename'));
            document.getElementById('<%= this.TableSummary.AliasTextBoxClientID %>').setAttribute('value', aliasValue);            
            document.getElementById('<%= this.TableSummary.SelectedTableIDHiddenClientID %>').value = tableId;
            YAHOO.DataviewBoardNS.TableSummary.PkRefreshTable('');
            YAHOO.DataviewBoardNS.TableSummary.RelRefreshTable('');
            YAHOO.DataviewBoardNS.TableSummary.LookupRefreshTable('');
            YAHOO.DataviewBoardNS.TableSummary.TagsRefreshTable('');
            YAHOO.DataviewBoardNS.TableSummary.IndexRefreshTable('');
            return true;
        };
        
        YAHOO.DataviewBoardNS.tableDeleted = function(oRecord) {
            var tableId = oRecord.getData('tableid');
            var database = oRecord.getData('tableschema');
            if( document.getElementById('SchemaSummaryDiv').style.display == 'block' ||
                document.getElementById('<%= this.TableSummary.SelectedTableIDHiddenClientID %>').value == tableId)
                YAHOO.DataviewBoardNS.schemaFiltered(database);
            else
                YAHOO.DataviewBoardNS.LeftPanel.RefreshTable('');
                
            return true;
        };
        

        YAHOO.DataviewBoardNS.tableDuplicated = function(tableId) {
            YAHOO.DataviewBoardNS.LeftPanel.RefreshTable('');
            var data = YAHOO.DataviewBoardNS.LeftPanel.DataTable.getRecordSet().getRecords();
            for(i = 0 ; i < data.length; i++)
            {
                if(data[i].getData('tableid') == tableId)
                {
                    var record = data[i];
                    YAHOO.DataviewBoardNS.LeftPanel.DataTable.selectRow(record);
                    YAHOO.DataviewBoardNS.tableSelected(record);
                    break;
                }
            }     
        }
        
        YAHOO.DataviewBoardNS.callServerCompleted = function(arg, context){ 
            if(arg.indexOf('FilterSchema: ') != -1) 
            {
                eval(arg.substring(14));
                YAHOO.DataviewBoardNS.schemaFiltered(context);                
            }
            else if(arg.indexOf('SelectTable: ') != -1) 
            {   
                var messageIndex = arg.indexOf("Message: "); 
                var AliasIndex = arg.indexOf("AliasValue:"); 
                eval(arg.substring(13,messageIndex));       
                document.getElementById('<%= this.hdnTableAlias.ClientID %>').value=arg.substring(AliasIndex + 11);
                YAHOO.DataviewBoardNS.tableSelected(context);
                ShowConfirmationArea(arg.substring(messageIndex + 9,AliasIndex));   
                 
            }
            else if(arg.indexOf('DeleteTable: ') != -1)
            {
                var messageIndex = arg.indexOf("Message: ");                
                eval(arg.substring(13,messageIndex));
                YAHOO.DataviewBoardNS.tableDeleted(context);
                ShowConfirmationArea(arg.substring(messageIndex + 9));              
            }
            else if(arg.indexOf('TableDuplicated: ') != -1) 
            {                         
                var messageIndex = arg.indexOf("Message: ");                
                eval(arg.substring(17,messageIndex));               
                var message = arg.substring(messageIndex + 9); 
                
                if(message.length == 0)                                                  
                {                                
                    var newTableId = document.getElementById('<%= this.TableSummary.SelectedTableIDHiddenClientID %>').value;
                    YAHOO.DataviewBoardNS.tableDuplicated(newTableId);                
                }                
                ShowConfirmationArea(arg.substring(messageIndex + 9));       
            }
            else if(arg.indexOf('Message: ') != -1) 
            {                
            }
            else if(arg.indexOf('DuplicateTable: ') != -1) 
            {                
                eval(arg.substring(16));
                ValidateDuplicateTable();
            }
           
            SetProgressLayerVisibility(false);
            return true;
        };
        
        YAHOO.DataviewBoardNS.callServer = function(arg, context) {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "arg", "YAHOO.DataviewBoardNS.callServerCompleted", "context") %>;
            SetProgressLayerVisibility(true);
        }
        
        YAHOO.DataviewBoardNS.subscribeToEvents = function () {
            YAHOO.util.Event.addListener('<%= this.TablesTreeViewUserControl.SchemaDropDownListClientID %>', "change", function(args) {
                YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.getInputEl().value = '';
                YAHOO.DataviewBoardNS.LeftPanel.DataSource.liveData = '';
                YAHOO.DataviewBoardNS.LeftPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.Searching_Label_Text %>');
                YAHOO.DataviewBoardNS.SchemaSummary.PkDataSource.liveData = '';
                YAHOO.DataviewBoardNS.SchemaSummary.PkDataTable.set('MSG_EMPTY', '<%= Resources.Resource.Searching_Label_Text %>');
                YAHOO.DataviewBoardNS.SchemaSummary.RelDataSource.liveData = '';
                YAHOO.DataviewBoardNS.SchemaSummary.RelDataTable.set('MSG_EMPTY', '<%= Resources.Resource.Searching_Label_Text %>');
                YAHOO.DataviewBoardNS.SchemaSummary.LookupDataSource.liveData = '';
                YAHOO.DataviewBoardNS.SchemaSummary.LookupDataTable.set('MSG_EMPTY', '<%= Resources.Resource.Searching_Label_Text %>');
                YAHOO.DataviewBoardNS.LeftPanel.RefreshTable('');
                YAHOO.DataviewBoardNS.SchemaSummary.PkRefreshTable('');
                YAHOO.DataviewBoardNS.SchemaSummary.RelRefreshTable('');
                YAHOO.DataviewBoardNS.SchemaSummary.LookupRefreshTable('');
                YAHOO.DataviewBoardNS.callServer('FilterSchema: ' + document.getElementById('<%= this.TablesTreeViewUserControl.SchemaDropDownListClientID %>').value, document.getElementById('<%= this.TablesTreeViewUserControl.SchemaDropDownListClientID %>').value);
            });

            
           YAHOO.DataviewBoardNS.LeftPanel.DataTable.subscribe("cellClickEvent", function(oArgs)
            {
                var column = this.getColumn(oArgs.target);
               
                switch (column.key) {
                    case 'button':
                        var record = this.getRecord(oArgs.target);
                        var isbasetable = record.getData("isbasetable")
                        if(isbasetable == 'False')
                        {
                         
                            var confirmDelete=document.getElementById('<%= this.hdnRemoveTable.ClientID %>').value;
                           if(confirmDelete !=null && confirmDelete == '1')
                           {
                                document.getElementById('<% =this.hdnIsBaseTable.ClientID %>').value = YAHOO.DataviewBoardNS.LeftPanel.DataTable.getRecordSet().getRecords().length -1;
                                YAHOO.DataviewBoardNS.callServer('DeleteTable: ' + this.getRecord(oArgs.target).getData('tableid'), this.getRecord(oArgs.target));                            
                           }
                           document.getElementById('<%= this.hdnRemoveTable.ClientID %>').value='0';
                         }
                        break;
                    default:
                        YAHOO.DataviewBoardNS.LeftPanel.DataTable.onEventSelectRow(oArgs);
                        YAHOO.DataviewBoardNS.callServer('SelectTable: ' + this.getRecord(oArgs.target).getData('tableid'), this.getRecord(oArgs.target));
                        break;
                }
            });
            
            YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.textboxChangeEvent.subscribe(function(eventName, args) { if(args[0].getInputEl().value == '') YAHOO.DataviewBoardNS.LeftPanel.RefreshTable('');});
            YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.itemSelectEvent.subscribe(function(eventName, args) { YAHOO.DataviewBoardNS.LeftPanel.RefreshTable(args[1].innerHTML);});
            YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.dataRequestEvent.subscribe(function(eventName, args) { YAHOO.DataviewBoardNS.LeftPanel.RefreshTable(args[0].getInputEl().value);});
            //Bug fixing JiraID:: CBOE-712
            YAHOO.DataviewBoardNS.LeftPanel.TagAutoComp.textboxKeyEvent.subscribe(function(eventName,args){ if (parseInt(args[0].getInputEl().value.length)==0) YAHOO.DataviewBoardNS.LeftPanel.RefreshTable(''); });
        };

        YAHOO.DataviewBoardNS.subscribeToEvents();
        
        <%= GetSelectedDivJS() %>
        
        var param = '<%= this.Request["ParamCaller"] %>';
        if(param != '')
        {
            var data = YAHOO.DataviewBoardNS.LeftPanel.DataTable.getRecordSet().getRecords();
            for(i = 0 ; i < data.length; i++)
            {
                if(data[i].getData('tableid') == param)
                {
                    var index = YAHOO.DataviewBoardNS.LeftPanel.DataTable.getRecordIndex(data[i]);
                    var page = Math.floor((index/YAHOO.DataviewBoardNS.LeftPanel.Paginator.getRowsPerPage())) + 1;
                    YAHOO.DataviewBoardNS.LeftPanel.Paginator.setPage(page, false);
                    YAHOO.DataviewBoardNS.LeftPanel.DataTable.selectRow(data[i]);
                    break;
                }
            }
        }
        
    });
    </script>
    <cc2:COEModalIFrame runat="server" BodyURL="" ID="MoldalIFrame" HeaderText="Add Schema"
        ModalPanelSettings="modal: true, fixedCenter: true, constraintoviewport: true "
        Width="500px"></cc2:COEModalIFrame>
</asp:Content>
