<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManager_ContentArea_DefineLookup"
    CodeBehind="DefineLookup.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>

<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <style type="text/css">
#<%=TableTextBox.ClientID %> {
    width:130px;
    position:relative;
}

#AutoCompleteContainer {
    width:205px; /* set width here or else widget will expand to fit its container */
    position:relative;
}
</style>
    <iframe id="ParentDiv" class="BackgroundHidden" frameborder="0" scrolling="no"></iframe>
    <script language="javascript" type="text/javascript">
        function SetProgressLayerVisibility(visible) {
            if (visible) {
                document.getElementById('UpdateProgressDiv').style.display = 'block';
                document.getElementById('ParentDiv').className = 'BackgroundVisible';
            }
            else {
                document.getElementById('UpdateProgressDiv').style.display = 'none';
                document.getElementById('ParentDiv').className = 'BackgroundHidden';
            }
        }
    </script>
    <table width="100%" class="PagesContentTable">
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField runat="server" ID="SelectedFieldIDHidden" Value="-1" />
                <asp:HiddenField runat="server" ID="SelectedDisplayFieldIDHidden" Value="-1" />
                <asp:HiddenField runat="server" ID="SelectedTableIDHidden" Value="-1" />
                <div runat="server" id="InvalidLookupDiv" style="display: none">
                    <asp:Label runat="server" ID="InvalidFieldsLabel"></asp:Label>
                </div>
                <table class="NewLookup">
                    <tr style="height: 30px;">
                        <td colspan="2">
                            <asp:Label runat="server" ID="SelectLookupLabel" SkinID="Title"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <COEManager:ImageButton ID="TopCancelImageButton" runat="server" TypeOfButton="Cancel"
                                ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png"
                                HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
                            <COEManager:ImageButton ID="TopOKImageButton" runat="server" TypeOfButton="Submit"
                                ButtonMode="ImgAndTxt" CausesValidation="true" />
                            <COEManager:ImageButton ID="TopDeleteImageButton" runat="server" TypeOfButton="Cancel"
                                ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Delete.png"
                                HoverImageURL="../../../App_Themes/Common/Images/Delete.png" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" valign="top" colspan="2">
                            <asp:Label ID="SelectedFromFieldLabel" runat="server" SkinID="RelationShipText"></asp:Label>
                            <span>--------------> </span>
                            <asp:Label ID="SelectedToFieldLabel" runat="server" SkinID="RelationShipText"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="width: 315px" align="left">
                            <div class="markup">
                                <div style="height: 65px;" class="GroupingDiv">
                                    <div class="GroupingHeader">
                                        <%= Resources.Resource.SelectLookupSchema_Label_Text %></div>
                                    <div id="SchemaDiv" class="ItemDiv">
                                        <label for="<%= this.SchemaDropDownList.ClientID %>">
                                            <%= Resources.Resource.Schema_Label_Text %>:</label>
                                        <asp:DropDownList ID="SchemaDropDownList" runat="server" CssClass="DropDownListsClass">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div style="height: 650px;" class="GroupingDiv">
                                    <div class="GroupingHeader">
                                        <%= Resources.Resource.SelectLookupTable_Label_Text %></div>
                                    <div id="AutoCompleteDiv" class="ItemDiv">
                                        <label for="<%= this.TableTextBox.ClientID %>">
                                            <%= Resources.Resource.FilterTable_Label_Text %>:</label>
                                        <asp:TextBox ID="TableTextBox" runat="server" Width="215px"></asp:TextBox>
                                        <div id="AutoCompleteContainer">
                                        </div>
                                    </div>
                                    <div>
                                        <div id="TablesDiv" style="float: left;">
                                            <div id="LeftPanelPaginatorTop" class="Paginator">
                                            </div>
                                            <div id="LeftPanelTbl" class="TableHolder">
                                            </div>
                                            <div id="LeftPanelPaginatorBottom" class="Paginator">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td valign="top" align="left">
                            <div class="markup">
                                <div style="height: 65px;" class="GroupingDivBig">
                                    <div class="GroupingHeader">
                                        <%= Resources.Resource.LookupSortOrder_Label_Text %></div>
                                    <div id="RightPanelTopWrapper" class="WideItemDiv">
                                        <label for="<%= this.SortingDropDownList.ClientID %>">
                                            <%= Resources.Resource.SortOrder_ColHeader_Caption%></label>
                                        <asp:DropDownList runat="server" ID="SortingDropDownList" CssClass="DropDownListsClass" onChange="return OnSelectedIndexChange();">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div style="height: 650px;" class="GroupingDivBig">
                                    <div class="GroupingHeader">
                                        <%= Resources.Resource.SelectLookupField_Label_Text %></div>
                                    <div id="RightTableDiv" class="WideItemDiv">
                                        <div id="RightPanelTopPaginatorTop" class="Paginator">
                                        </div>
                                        <div id="RightPanelTopTbl" class="TableHolder">
                                        </div>
                                        <div id="RightPanelTopPaginatorBottom" class="Paginator">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <COEManager:ImageButton ID="BottomCancelImageButton" runat="server" TypeOfButton="Cancel"
                                ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png"
                                HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
                            <COEManager:ImageButton ID="BottomOKImageButton" runat="server" TypeOfButton="Submit"
                                ButtonMode="ImgAndTxt" CausesValidation="true" />
                            <COEManager:ImageButton ID="BottomDeleteImageButton" runat="server" TypeOfButton="Cancel"
                                ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Delete.png"
                                HoverImageURL="../../../App_Themes/Common/Images/Delete.png" />
                        </td>
                    </tr>
                </table>
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
        
        function OnSelectedIndexChange()
        {          
                       
            var data = YAHOO.LookupsNS.LeftPanel.DataTable.getRecordSet().getRecords();
            var fieldData = YAHOO.LookupsNS.RightPanelTop.DataTable.getRecordSet().getRecords();
            var fieldId =document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value;
            var displayId =document.getElementById('<%= this.SelectedDisplayFieldIDHidden.ClientID %>').value;

             for(i = 0 ; i < data.length; i++)
            {
                if(data[i].getData('tableid') == document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value)
                {                    
                    YAHOO.LookupsNS.callServer('SelectTable: ' + document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value +',FieldID:' + fieldId + ',DisplayID:' + displayId + ',TotalRows:' + YAHOO.LookupsNS.RightPanelTop.Paginator.getRowsPerPage() + ',SortOrder:' + document.getElementById('<%= this.SortingDropDownList.ClientID %>').value, YAHOO.LookupsNS.LeftPanel.DataTable.getRecord(i));       
                    break;
                }
            }                                 
        }

        YAHOO.util.Event.addListener(window, "load", function() {
        YAHOO.namespace('LookupsNS');
                
        YAHOO.LookupsNS.myRowFormatter = function(elTr, oRecord) {
            Dom.addClass(elTr, 'myRow');
            return true;
        };
        
        YAHOO.LookupsNS.selectableRowFormatter = function(elTr, oRecord) {
            Dom.addClass(elTr, 'selectableRow');
            return true;
        };
        
        //---------------------------- LeftPanel ----------------------------//
        YAHOO.namespace('LookupsNS.LeftPanel');
         
        YAHOO.LookupsNS.LeftPanel.DataSource = new YAHOO.util.LocalDataSource();
        YAHOO.LookupsNS.LeftPanel.DataSource.responseSchema = { 
            fields : ['tablealias', 'tableschema', 'tableid', 'tablename', 'isbasetable']
        };
        <%= this.GetTablesDataSource(string.Empty) %>
        YAHOO.LookupsNS.LeftPanel.DataSource.doBeforeCallback = function (req,raw,res,cb) {
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
        
        YAHOO.LookupsNS.LeftPanel.DataTableCols = [
            { key: 'tablealias', sortable: true, width: 280, label: '<%= Resources.Resource.Table_Label_Text %>' }
        ];

        YAHOO.LookupsNS.LeftPanel.Paginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            containers    : ["LeftPanelPaginatorTop", "LeftPanelPaginatorBottom"]
        });

        YAHOO.LookupsNS.LeftPanel.DataTableConf = {
            paginator : YAHOO.LookupsNS.LeftPanel.Paginator,
            sortedBy: {key:'tablealias', dir:YAHOO.widget.DataTable.CLASS_ASC},
            formatRow: YAHOO.LookupsNS.selectableRowFormatter
        };
        
        YAHOO.LookupsNS.LeftPanel.DataTable = new YAHOO.widget.DataTable('LeftPanelTbl',YAHOO.LookupsNS.LeftPanel.DataTableCols,YAHOO.LookupsNS.LeftPanel.DataSource,YAHOO.LookupsNS.LeftPanel.DataTableConf);
        YAHOO.LookupsNS.LeftPanel.DataTable.set("selectionMode", "single");
        YAHOO.LookupsNS.LeftPanel.TagAutoComp = new YAHOO.widget.AutoComplete('<%=this.TableTextBox.ClientID %>','AutoCompleteContainer', YAHOO.LookupsNS.LeftPanel.DataSource);
        YAHOO.LookupsNS.LeftPanel.TagAutoComp.useIFrame = true; // Enable an iFrame shim under the container element
        YAHOO.LookupsNS.LeftPanel.TagAutoComp.typeAhead = true; // Enable type ahead
        YAHOO.LookupsNS.LeftPanel.TagAutoComp.resultTypeList = false;
        YAHOO.LookupsNS.LeftPanel.TagAutoComp.applyLocalFilter = false;
        
        YAHOO.LookupsNS.LeftPanel.FilterTimeout = null;
        YAHOO.LookupsNS.LeftPanel.UpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.LookupsNS.LeftPanel.FilterTimeout = null;
            // Reset sort
            var state = YAHOO.LookupsNS.LeftPanel.DataTable.getState();
            state.sortedBy = {key:'tablealias', dir:YAHOO.widget.DataTable.CLASS_ASC};
           
            // Get filtered data
            YAHOO.LookupsNS.LeftPanel.DataSource.sendRequest(query,{
                success : YAHOO.LookupsNS.LeftPanel.DataTable.onDataReturnInitializeTable,
                failure : YAHOO.LookupsNS.LeftPanel.DataTable.onDataReturnInitializeTable,
                scope   : YAHOO.LookupsNS.LeftPanel.DataTable,
                argument: state
            });
        };
        
        YAHOO.LookupsNS.LeftPanel.RefreshTable = function (query) {
                clearTimeout(YAHOO.LookupsNS.LeftPanel.FilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.LookupsNS.LeftPanel.UpdateFilter(query);
                else
                    YAHOO.LookupsNS.LeftPanel.UpdateFilter();
                return true;
            };
        //---------------------------- LeftPanel ----------------------------//
        
        
        //---------------------------- RightPanelTop ----------------------------//
        YAHOO.namespace('LookupsNS.RightPanelTop');
         
        YAHOO.LookupsNS.RightPanelTop.DataSource = new YAHOO.util.LocalDataSource();
        YAHOO.LookupsNS.RightPanelTop.DataSource.responseSchema = { 
            fields : ['fieldalias', 'fieldid', 'fieldtype', 'islookup', 'isdisplay']
        };
        <%= this.GetFieldsDataSource(-1,20,-1) %>
        
        YAHOO.LookupsNS.RightPanelTop.DataTableCols = [
            { key: 'fieldalias', width: 250, label: '<%= Resources.Resource.Field_Label_Text %>' },
            { key: 'islookup', label: '<%= Resources.Resource.Join_ColHeader_Caption %>', formatter: YAHOO.widget.DataTable.formatRadio },
            { key: 'isdisplay', label: '<%= Resources.Resource.Display_Label_Text %>', formatter: YAHOO.widget.DataTable.formatRadio }
        ];

        YAHOO.LookupsNS.RightPanelTop.Paginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            containers    : ["RightPanelTopPaginatorTop", "RightPanelTopPaginatorBottom"]
        });

        YAHOO.LookupsNS.RightPanelTop.DataTableConf = {
            paginator : YAHOO.LookupsNS.RightPanelTop.Paginator,
            sortedBy: {key:'fieldalias', dir:YAHOO.widget.DataTable.CLASS_ASC},
            formatRow: YAHOO.LookupsNS.myRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.SelectTable_Label_Text %>'
        };
        
        YAHOO.LookupsNS.RightPanelTop.DataTable = new YAHOO.widget.DataTable('RightPanelTopTbl',YAHOO.LookupsNS.RightPanelTop.DataTableCols,YAHOO.LookupsNS.RightPanelTop.DataSource,YAHOO.LookupsNS.RightPanelTop.DataTableConf);
        YAHOO.LookupsNS.RightPanelTop.FilterTimeout = null;
        YAHOO.LookupsNS.RightPanelTop.UpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.LookupsNS.RightPanelTop.FilterTimeout = null;
            // Reset sort
            var state = YAHOO.LookupsNS.RightPanelTop.DataTable.getState();
            state.sortedBy = {key:'fieldalias', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.LookupsNS.RightPanelTop.DataSource.sendRequest(query,{
                success : YAHOO.LookupsNS.RightPanelTop.DataTable.onDataReturnInitializeTable,
                failure : YAHOO.LookupsNS.RightPanelTop.DataTable.onDataReturnInitializeTable,
                scope   : YAHOO.LookupsNS.RightPanelTop.DataTable,
                argument: state
            });
        };
        
        YAHOO.LookupsNS.RightPanelTop.RefreshTable = function (query) {
                clearTimeout(YAHOO.LookupsNS.RightPanelTop.FilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.LookupsNS.RightPanelTop.UpdateFilter(query);
                else
                    YAHOO.LookupsNS.RightPanelTop.UpdateFilter();
                return true;
            };


        //---------------------------- RightPanelTop ----------------------------//
        
        YAHOO.LookupsNS.initializeEmptyMessages = function()
        {
            YAHOO.LookupsNS.LeftPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.EmptySchema_Label_Text %>');
            YAHOO.LookupsNS.LeftPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.NoFieldsInTable_Label_Text %>');
        }
        
        YAHOO.LookupsNS.initializeEmptyMessages();
        
        YAHOO.LookupsNS.schemaFiltered = function(schema) {
                YAHOO.LookupsNS.initializeEmptyMessages();
                YAHOO.LookupsNS.LeftPanel.RefreshTable('');
                return true;
            };

        YAHOO.LookupsNS.tableSelected = function(tableRecord) {
            var tableId = tableRecord.getData('tableid');
            // DAT-1590, select a existent lookup/set lookup and display, then change sort method, 
            // fieldId and displayId will set to -1, but not the true value.
            // add fieldId and display Id setting in table Record.
            // If change sort method more then twice, fieldId and DisplayId will not in tableRecord.
            // If fieldId and DisplayId is undefined, do not change the value of element.
            var fieldId = tableRecord.getData('FieldID');
            var displayId = tableRecord.getData('DisplayID');

            document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value = tableId;
            if (fieldId) {
                document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value = fieldId;
            }
            if (displayId) {
                document.getElementById('<%= this.SelectedDisplayFieldIDHidden.ClientID %>').value = displayId;
            }
            YAHOO.LookupsNS.initializeEmptyMessages();
            YAHOO.LookupsNS.RightPanelTop.RefreshTable('');
            return true;
        };
        
        YAHOO.LookupsNS.lookupSelected = function(fieldRecord) {
            var fieldId = fieldRecord.getData('fieldid');
            document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value = fieldId;
            return true;
        };
        
        YAHOO.LookupsNS.displaySelected = function(fieldRecord) {
            var fieldId = fieldRecord.getData('fieldid');
            var database = document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value;
            var tableRecord = YAHOO.LookupsNS.LeftPanel.DataTable.getRecord(YAHOO.LookupsNS.LeftPanel.DataTable.getSelectedRows()[0]);
            var field = fieldRecord.getData('fieldalias');
            document.getElementById('<%= this.SelectedToFieldLabel.ClientID %>').innerHTML = tableRecord.getData('tableschema') + '.' + tableRecord.getData('tablealias') + '.' + field;
            document.getElementById('<%= this.SelectedDisplayFieldIDHidden.ClientID %>').value = fieldId;
            return true;
        };
        
        YAHOO.LookupsNS.callServerCompleted = function(arg, context){ 
            document.getElementById('<%= InvalidLookupDiv.ClientID %>').style.display = 'none';
            if(arg.indexOf('FilterSchema: ') != -1) 
            {
                eval(arg.substring(14));
                YAHOO.LookupsNS.schemaFiltered(context);
            }
            else if(arg.indexOf('SelectTable: ') != -1) 
            {
                eval(arg.substring(13));
                YAHOO.LookupsNS.tableSelected(context);
            }
            else if(arg.indexOf('SelectLookup: ') != -1) 
            {
            YAHOO.LookupsNS.lookupSelected(context);
            }
            else if(arg.indexOf('SelectDisplay: ') != -1) 
            {
                YAHOO.LookupsNS.displaySelected(context);
            }
            else if(arg.indexOf('Message: ') != -1) 
            {
                document.getElementById('<%= InvalidLookupDiv.ClientID %>').style.display = 'block';
                document.getElementById('<%= InvalidFieldsLabel.ClientID %>').innerHTML = arg.substring(9);
            }
            
            SetProgressLayerVisibility(false);
            return true;
        };
        
        YAHOO.LookupsNS.callServer = function(arg, context) {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "arg", "YAHOO.LookupsNS.callServerCompleted", "context") %>;
            SetProgressLayerVisibility(true);
        }
        
        YAHOO.LookupsNS.subscribeToEvents = function () {
            YAHOO.util.Event.addListener('<%= this.SchemaDropDownList.ClientID %>', "change", function(args) {
                YAHOO.LookupsNS.LeftPanel.TagAutoComp.getInputEl().value = '';
                YAHOO.LookupsNS.LeftPanel.DataSource.liveData = '';
                YAHOO.LookupsNS.LeftPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.Searching_Label_Text %>');
                YAHOO.LookupsNS.LeftPanel.RefreshTable('');
                
                YAHOO.LookupsNS.RightPanelTop.DataSource.liveData = '';
                YAHOO.LookupsNS.RightPanelTop.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.SelectTable_Label_Text %>');
                YAHOO.LookupsNS.RightPanelTop.RefreshTable('');
                
                YAHOO.LookupsNS.callServer('FilterSchema: ' + document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value, document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value);
            });
            
            YAHOO.LookupsNS.LeftPanel.DataTable.subscribe("cellClickEvent", function(oArgs)
            {
                var fieldId =document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value;
                var displayId =document.getElementById('<%= this.SelectedDisplayFieldIDHidden.ClientID %>').value;
                YAHOO.LookupsNS.LeftPanel.DataTable.onEventSelectRow(oArgs);                
                YAHOO.LookupsNS.callServer('SelectTable: ' + this.getRecord(oArgs.target).getData('tableid') +',FieldID:' + fieldId +',DisplayID:' + displayId +',TotalRows:' + YAHOO.LookupsNS.RightPanelTop.Paginator.getRowsPerPage() + ',SortOrder:' + document.getElementById('<%= this.SortingDropDownList.ClientID %>').value, this.getRecord(oArgs.target));
            });
            
            YAHOO.LookupsNS.RightPanelTop.DataTable.subscribe("radioClickEvent", function(oArgs)
            {
                var column = this.getColumn(oArgs.target);
                 var data = YAHOO.LookupsNS.RightPanelTop.DataTable.getRecordSet().getRecords();
                switch (column.key) {
                    case 'islookup':
                     for(i = 0 ; i < data.length; i++)
                        {
                            if(data[i].getData('islookup') == true)
                            {
                               data[i].setData('islookup',false);
                               
                            }
                        }
                        YAHOO.LookupsNS.callServer('SelectLookup: ' + this.getRecord(oArgs.target).getData('fieldid'), this.getRecord(oArgs.target));
                        break;
                    case 'isdisplay':
                     for(i = 0 ; i < data.length; i++)
                        {
                            if(data[i].getData('isdisplay') == true)
                            {
                               data[i].setData('isdisplay',false);
                             
                            }
                        }
                        YAHOO.LookupsNS.callServer('SelectDisplay: ' + this.getRecord(oArgs.target).getData('fieldid'), this.getRecord(oArgs.target));
                        break;
                }
            });

            YAHOO.LookupsNS.RightPanelTop.Paginator.subscribe("changeRequest", function (){

                if(document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value != -1)
                {
                    var data = YAHOO.LookupsNS.RightPanelTop.DataTable.getRecordSet().getRecords();
                     for(i = 0 ; i < data.length; i++)
                        {
                            if(data[i].getData('fieldid') == document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value)
                            {
                               data[i].setData('islookup',true);
                             
                            }
                            if(data[i].getData('fieldid') == document.getElementById('<%= this.SelectedDisplayFieldIDHidden.ClientID %>').value)
                            {
                               data[i].setData('isdisplay',true);
                           
                            }
                        }
                }
            });

            

            YAHOO.LookupsNS.LeftPanel.TagAutoComp.textboxChangeEvent.subscribe(function(eventName, args) { if(args[0].getInputEl().value == '') YAHOO.LookupsNS.LeftPanel.RefreshTable('');});
            YAHOO.LookupsNS.LeftPanel.TagAutoComp.itemSelectEvent.subscribe(function(eventName, args) { YAHOO.LookupsNS.LeftPanel.RefreshTable(args[1].innerHTML);});
            YAHOO.LookupsNS.LeftPanel.TagAutoComp.dataRequestEvent.subscribe(function(eventName, args) { YAHOO.LookupsNS.LeftPanel.RefreshTable(args[0].getInputEl().value);});

            //Bug fixing JiraID:: CBOE-712
            YAHOO.LookupsNS.LeftPanel.TagAutoComp.textboxKeyEvent.subscribe(function(eventName,args){ if (parseInt(args[0].getInputEl().value.length)==0) YAHOO.LookupsNS.LeftPanel.RefreshTable(''); });

        };
        
        YAHOO.LookupsNS.subscribeToEvents();
        
        
            
        if(document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value != -1)
        {
            var data = YAHOO.LookupsNS.LeftPanel.DataTable.getRecordSet().getRecords();
            for(i = 0 ; i < data.length; i++)
            {
                if(data[i].getData('tableid') == document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value)
                {
                    var index = YAHOO.LookupsNS.LeftPanel.DataTable.getRecordIndex(data[i]);
                    var page = Math.floor((index/YAHOO.LookupsNS.LeftPanel.Paginator.getRowsPerPage())) + 1;
                    YAHOO.LookupsNS.LeftPanel.Paginator.setPage(page, false);
                    YAHOO.LookupsNS.LeftPanel.DataTable.selectRow(data[i]);
                    break;
                }
            }
        }
        
        if(document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value != -1)
        {
            var data = YAHOO.LookupsNS.RightPanelTop.DataTable.getRecordSet().getRecords();
            for(i = 0 ; i < data.length; i++)
            {
                if(data[i].getData('fieldid') == document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value)
                {
                    var index = YAHOO.LookupsNS.RightPanelTop.DataTable.getRecordIndex(data[i]);
                    var page = Math.floor((index/YAHOO.LookupsNS.RightPanelTop.Paginator.getRowsPerPage())) + 1;
                    YAHOO.LookupsNS.RightPanelTop.Paginator.setPage(page, false);
                    break;
                }
            }
        }
    });
    </script>
</asp:Content>
