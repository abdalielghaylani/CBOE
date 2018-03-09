<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManager_ContentArea_DefineRelationships" Codebehind="DefineRelationships.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>

<asp:Content Id="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="Server" >
<style type="text/css">
#<%=TableTextBox.ClientID %> {
    width:205px;
    position:relative;
}

#AutoCompleteContainer {
    width:215px; /* set width here or else widget will expand to fit its container */
    position:relative;
}

#<%= this.SchemaDropDownList.ClientID %> {
    margin-left: 22px;
}

#<%= this.InstanceDropDownList.ClientID %> {
    margin-left: 2px;
}

</style>
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

    function ConfirmCircularDependancy() {
        var val = document.getElementById('<%= this.CircularDependancyHidden.ClientID %>').value;
        if (val == "TRUE") {
            alert('<%= Resources.Resource.ConfirmCircualrDependancyRelationship %>');
            return false;
        } else {
            return true;
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
                <asp:HiddenField runat="server" ID="SelectedTableIDHidden" Value="-1" />
                <asp:HiddenField runat="server" ID="CircularDependancyHidden" Value="false" />
                <div runat="server" id="InvalidRelationshipsText" style="display:none">
                    <asp:Label runat="server" ID="InvalidRelationshipsTextLabel" SkinID="InvalidRelationShipText"></asp:Label>
                </div>
                <table class="NewRelationShips">
                    <tr style="height:30px;">
                        <td colspan="2">
                            <asp:Label runat="server" ID="SelectParentRelationshipLabel" SkinID="Title"></asp:Label>
                        </td>
                    </tr>
                    <td colspan="2" align="right">
                            <COEManager:ImageButton ID="TopCancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png"/>
                            <COEManager:ImageButton ID="TopOKImageButton" runat="server" TypeOfButton="Submit" ButtonMode="ImgAndTxt" CausesValidation="true" OnClientClick="return ConfirmCircularDependancy();"/>
                            <COEManager:ImageButton ID="TopDeleteImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Delete.png" HoverImageURL="../../../App_Themes/Common/Images/Delete.png"/>
                        </td>
                    <tr>
                        <td align="center" valign="top" colspan="2">
                            <asp:Label ID="SelectedFromFieldLabel" runat="server"  SkinID="RelationShipText"></asp:Label>
                            <asp:Label ID="SelectionJoinTypeLabel" runat="server" SkinID="RelationShipText"></asp:Label>
                            <asp:Label ID="SelectedToFieldLabel" runat="server" SkinID="RelationShipText"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="width:315px" align="left">
                            <div class="markup">
                                <div style="height:75px;" class="GroupingDiv">
                                    <div class="GroupingHeader"><%= Resources.Resource.SelectParentSchema_Label_Text %></div>
                                    <div id="SchemaDiv" class="ItemDiv">
                                        <label for="<%= this.InstanceDropDownList.ClientID %>"><%= Resources.Resource.Instance_Label_Text %>:</label> 
                                        <asp:DropDownList ID="InstanceDropDownList" runat="server" AutoPostBack="true" CssClass="DropDownListsClass" onselectedindexchanged="InstanceDropDownList_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                    <div id="SchemaDiv" class="ItemDiv">
                                        <label for="<%= this.SchemaDropDownList.ClientID %>"><%= Resources.Resource.Schema_Label_Text %>:</label> 
                                        <asp:DropDownList ID="SchemaDropDownList" runat="server" CssClass="DropDownListsClass"></asp:DropDownList>
                                    </div>
                                </div>
                                <div style="height:650px;" class="GroupingDiv">
                                    <div class="GroupingHeader"><%= Resources.Resource.SelectParentTable_Label_Text %></div>
                                    <div id="AutoCompleteDiv" class="ItemDiv">
                                        <label for="<%= this.TableTextBox.ClientID %>"><%= Resources.Resource.FilterTable_Label_Text %>:</label> 
                                        <asp:TextBox id="TableTextBox" runat="server"></asp:TextBox>
                                        <div id="AutoCompleteContainer">
                                        </div>
                                    </div>
                                    <div id="TablesDiv" style="float:left;">
                                        <div id="LeftPanelPaginatorTop" class="Paginator"></div>
                                        <div id="LeftPanelTbl" class="TableHolder"></div>
                                        <div id="LeftPanelPaginatorBottom" class="Paginator"></div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td valign="top" align="left">
                            <div class="markup">
                                <div style="height:65px;" class="GroupingDiv">
                                    <div class="GroupingHeader"><%= Resources.Resource.SelectJoinType_Label_Text %></div>
                                    <div id="SelectedTableWrapper" class="ItemDiv">
                                        <label for="<%= this.JoinTypesDropDown.ClientID %>"><%= Resources.Resource.JoinType_Label_Text %></label>
                                        <asp:DropDownList runat="server" ID="JoinTypesDropDown" CssClass="DropDownListsClass"></asp:DropDownList> 
                                    </div>
                                </div>
                                <div style="height:650px;" class="GroupingDiv">
                                    <div class="GroupingHeader"><%= Resources.Resource.SelectJoinField_Label_Text %></div>
                                    <div id="RightTableDiv" style="float:left;">
                                        <div id="RightPanelPaginatorTop" class="Paginator"></div>
                                        <div id="RightPanelTbl" class="TableHolder"></div>
                                        <div id="RightPanelPaginatorBottom" class="Paginator"></div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <COEManager:ImageButton ID="BottomCancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png"/>
                            <COEManager:ImageButton ID="BottomOKImageButton" runat="server" TypeOfButton="Submit" ButtonMode="ImgAndTxt" CausesValidation="true" OnClientClick="return ConfirmCircularDependancy();"/>
                            <COEManager:ImageButton ID="BottomDeleteImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Delete.png" HoverImageURL="../../../App_Themes/Common/Images/Delete.png"/>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <div id="UpdateProgressDiv" style="z-index:340;display:none;">
                    <img id="ProgressImage" alt="Processing" src="../../../App_Themes/Common/Images/searching.gif" style="position:absolute;top:72px;left:762px;z-index:340;"/>
                </div>
            </td>
        </tr>
    </table>
    <script language="javascript" type="text/javascript">
        YAHOO.util.Event.addListener(window, "load", function() {
        YAHOO.namespace('RelationshipsNS');
                
        YAHOO.RelationshipsNS.myRowFormatter = function(elTr, oRecord) {
            Dom.addClass(elTr, 'myRow');
            return true;
        };
        
        YAHOO.RelationshipsNS.selectableRowFormatter = function(elTr, oRecord) {
            Dom.addClass(elTr, 'selectableRow');
            return true;
        };
        
        //---------------------------- LeftPanel ----------------------------//
        YAHOO.namespace('RelationshipsNS.LeftPanel');
         
        YAHOO.RelationshipsNS.LeftPanel.DataSource = new YAHOO.util.LocalDataSource();
        YAHOO.RelationshipsNS.LeftPanel.DataSource.responseSchema = { 
            fields : ['tablealias', 'tableschema', 'tableid', 'tablename', 'isbasetable']
        };
        <%= this.GetTablesDataSource(this.InstanceDropDownList.SelectedValue + "." + this.SchemaDropDownList.SelectedValue) %>
        YAHOO.RelationshipsNS.LeftPanel.DataSource.doBeforeCallback = function (req,raw,res,cb) {
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
        
        YAHOO.RelationshipsNS.LeftPanel.DataTableCols = [
            { key: 'tablealias', sortable: true, width: 280, label: '<%= Resources.Resource.Table_Label_Text %>' }
        ];

        YAHOO.RelationshipsNS.LeftPanel.Paginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            containers    : ["LeftPanelPaginatorTop", "LeftPanelPaginatorBottom"]
        });

        YAHOO.RelationshipsNS.LeftPanel.DataTableConf = {
            paginator : YAHOO.RelationshipsNS.LeftPanel.Paginator,
            sortedBy: {key:'tablealias', dir:YAHOO.widget.DataTable.CLASS_ASC},
            formatRow: YAHOO.RelationshipsNS.selectableRowFormatter
        };
        
        YAHOO.RelationshipsNS.LeftPanel.DataTable = new YAHOO.widget.DataTable('LeftPanelTbl',YAHOO.RelationshipsNS.LeftPanel.DataTableCols,YAHOO.RelationshipsNS.LeftPanel.DataSource,YAHOO.RelationshipsNS.LeftPanel.DataTableConf);
        YAHOO.RelationshipsNS.LeftPanel.DataTable.set("selectionMode", "single");
        YAHOO.RelationshipsNS.LeftPanel.TagAutoComp = new YAHOO.widget.AutoComplete('<%=this.TableTextBox.ClientID %>','AutoCompleteContainer', YAHOO.RelationshipsNS.LeftPanel.DataSource);
        YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.useIFrame = true; // Enable an iFrame shim under the container element
        YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.typeAhead = true; // Enable type ahead
        YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.resultTypeList = false;
        YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.applyLocalFilter = false;
        
        YAHOO.RelationshipsNS.LeftPanel.FilterTimeout = null;
        YAHOO.RelationshipsNS.LeftPanel.UpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.RelationshipsNS.LeftPanel.FilterTimeout = null;
            // Reset sort
            var state = YAHOO.RelationshipsNS.LeftPanel.DataTable.getState();
            state.sortedBy = {key:'tablealias', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.RelationshipsNS.LeftPanel.DataSource.sendRequest(query,{
                success : YAHOO.RelationshipsNS.LeftPanel.DataTable.onDataReturnInitializeTable,
                failure : YAHOO.RelationshipsNS.LeftPanel.DataTable.onDataReturnInitializeTable,
                scope   : YAHOO.RelationshipsNS.LeftPanel.DataTable,
                argument: state
            });
        };
        
        YAHOO.RelationshipsNS.LeftPanel.RefreshTable = function (query) {
                clearTimeout(YAHOO.RelationshipsNS.LeftPanel.FilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.RelationshipsNS.LeftPanel.UpdateFilter(query);
                else
                    YAHOO.RelationshipsNS.LeftPanel.UpdateFilter();
                return true;
            };
        //---------------------------- LeftPanel ----------------------------//
        
        
        //---------------------------- RightPanel ----------------------------//
        YAHOO.namespace('RelationshipsNS.RightPanel');
         
        YAHOO.RelationshipsNS.RightPanel.DataSource = new YAHOO.util.LocalDataSource();
        YAHOO.RelationshipsNS.RightPanel.DataSource.responseSchema = { 
            fields : ['fieldalias', 'fieldid', 'fieldtype']
        };
        <%= this.GetFieldsDataSource(-1) %>
        
        YAHOO.RelationshipsNS.RightPanel.DataTableCols = [
            { key: 'fieldalias', sortable: true, width: 280, label: '<%= Resources.Resource.Field_Label_Text %>' }
        ];

        YAHOO.RelationshipsNS.RightPanel.Paginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            containers    : ["RightPanelPaginatorTop", "RightPanelPaginatorBottom"]
        });

        YAHOO.RelationshipsNS.RightPanel.DataTableConf = {
            paginator : YAHOO.RelationshipsNS.RightPanel.Paginator,
            sortedBy: {key:'fieldalias', dir:YAHOO.widget.DataTable.CLASS_ASC},
            formatRow: YAHOO.RelationshipsNS.selectableRowFormatter,
            MSG_EMPTY: '<%= Resources.Resource.SelectTable_Label_Text %>'
        };
        
        YAHOO.RelationshipsNS.RightPanel.DataTable = new YAHOO.widget.DataTable('RightPanelTbl',YAHOO.RelationshipsNS.RightPanel.DataTableCols,YAHOO.RelationshipsNS.RightPanel.DataSource,YAHOO.RelationshipsNS.RightPanel.DataTableConf);
        YAHOO.RelationshipsNS.RightPanel.DataTable.set("selectionMode", "single");
        YAHOO.RelationshipsNS.RightPanel.FilterTimeout = null;
        YAHOO.RelationshipsNS.RightPanel.UpdateFilter  = function (query) {
            // Reset timeout
            YAHOO.RelationshipsNS.RightPanel.FilterTimeout = null;
            // Reset sort
            var state = YAHOO.RelationshipsNS.RightPanel.DataTable.getState();
            state.sortedBy = {key:'fieldalias', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            YAHOO.RelationshipsNS.RightPanel.DataSource.sendRequest(query,{
                success : YAHOO.RelationshipsNS.RightPanel.DataTable.onDataReturnInitializeTable,
                failure : YAHOO.RelationshipsNS.RightPanel.DataTable.onDataReturnInitializeTable,
                scope   : YAHOO.RelationshipsNS.RightPanel.DataTable,
                argument: state
            });
        };
        
        YAHOO.RelationshipsNS.RightPanel.RefreshTable = function (query) {
                clearTimeout(YAHOO.RelationshipsNS.RightPanel.FilterTimeout);
                if(typeof(query) != 'undefined')
                    YAHOO.RelationshipsNS.RightPanel.UpdateFilter(query);
                else
                    YAHOO.RelationshipsNS.RightPanel.UpdateFilter();
                return true;
            };
        //---------------------------- RightPanel ----------------------------//
        
        YAHOO.RelationshipsNS.initializeEmptyMessages = function()
        {
            YAHOO.RelationshipsNS.LeftPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.EmptySchema_Label_Text %>');
            YAHOO.RelationshipsNS.LeftPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.NoFieldsInTable_Label_Text %>');
        }
        
        YAHOO.RelationshipsNS.initializeEmptyMessages();
        
        YAHOO.RelationshipsNS.schemaFiltered = function(schema) {
                YAHOO.RelationshipsNS.initializeEmptyMessages();
                YAHOO.RelationshipsNS.LeftPanel.RefreshTable('');
                return true;
            };

        YAHOO.RelationshipsNS.tableSelected = function(tableRecord) {
            var tableId = tableRecord.getData('tableid');
            YAHOO.RelationshipsNS.initializeEmptyMessages();
            YAHOO.RelationshipsNS.RightPanel.RefreshTable('');
            
            return true;
        };
        
        YAHOO.RelationshipsNS.fieldSelected = function(fieldRecord) {
            var fieldId = fieldRecord.getData('fieldid');
            var database = document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value;
            var tableRecord = YAHOO.RelationshipsNS.LeftPanel.DataTable.getRecord(YAHOO.RelationshipsNS.LeftPanel.DataTable.getSelectedRows()[0]);
            var field = fieldRecord.getData('fieldalias');
            document.getElementById('<%= this.SelectedFromFieldLabel.ClientID %>').innerHTML = database + '.' + tableRecord.getData('tablealias') + '.' + field;
            document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value = fieldId;
            document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value = tableRecord.getData('tableid');
            return true;
        };
        
           YAHOO.RelationshipsNS.MessageDone = function(fieldRecord,message) {
            var fieldId = fieldRecord.getData('fieldid');
            var database = document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value;
            var tableRecord = YAHOO.RelationshipsNS.LeftPanel.DataTable.getRecord(YAHOO.RelationshipsNS.LeftPanel.DataTable.getSelectedRows()[0]);
            var field = fieldRecord.getData('fieldalias');
            document.getElementById('<%= this.SelectedFromFieldLabel.ClientID %>').innerHTML = database + '.' + tableRecord.getData('tablealias') + '.' + field;
            document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value = fieldId;
            document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value = tableRecord.getData('tableid');
            document.getElementById('<%= InvalidRelationshipsText.ClientID %>').style.display = 'block';
            document.getElementById('<%= InvalidRelationshipsTextLabel.ClientID %>').innerHTML = message;
            return true;
        };

        YAHOO.RelationshipsNS.callServerCompleted = function(arg, context){ 
            document.getElementById('<%= InvalidRelationshipsText.ClientID %>').style.display = 'none';
            // Clean the last check result.
            document.getElementById('<%= this.CircularDependancyHidden.ClientID %>').value="";

            if(arg.indexOf('FilterSchema: ') != -1) 
            {
                eval(arg.substring(14));
                YAHOO.RelationshipsNS.schemaFiltered(context);
            }
            else if(arg.indexOf('SelectTable: ') != -1) 
            {
                eval(arg.substring(13));
                YAHOO.RelationshipsNS.tableSelected(context);
            }
            else if(arg.indexOf('SelectField: ') != -1) 
            {
                YAHOO.RelationshipsNS.fieldSelected(context);
            }
            else if(arg.indexOf('Message: ') != -1) 
            {      
              var val= arg.substring(9);
               if(val=='CIRCULARDEPENDENCY')
               {              
                   document.getElementById('<%= this.CircularDependancyHidden.ClientID %>').value="TRUE";                
                   arg='Message: ';
               }           
              YAHOO.RelationshipsNS.MessageDone(context,arg.substring(9));                
            }
           
           

            SetProgressLayerVisibility(false);
            return true;
        };
        
        YAHOO.RelationshipsNS.callServer = function(arg, context) {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "arg", "YAHOO.RelationshipsNS.callServerCompleted", "context") %>;
            SetProgressLayerVisibility(true);
        }
        
        YAHOO.RelationshipsNS.subscribeToEvents = function () {
            YAHOO.util.Event.addListener('<%= this.SchemaDropDownList.ClientID %>', "change", function (args) {
                YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.getInputEl().value = '';
                YAHOO.RelationshipsNS.LeftPanel.DataSource.liveData = '';
                YAHOO.RelationshipsNS.LeftPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.Searching_Label_Text %>');
                YAHOO.RelationshipsNS.LeftPanel.RefreshTable('');

                YAHOO.RelationshipsNS.RightPanel.DataSource.liveData = '';
                YAHOO.RelationshipsNS.RightPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.SelectTable_Label_Text %>');
                YAHOO.RelationshipsNS.RightPanel.RefreshTable('');
                var instanceName = document.getElementById('<%= this.InstanceDropDownList.ClientID %>').value;
                var schemaName = document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value;

                YAHOO.RelationshipsNS.callServer('FilterSchema: ' + instanceName + '.' + schemaName, document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value);
            });

            YAHOO.util.Event.addListener('<%= this.InstanceDropDownList.ClientID %>', "change", function (args) {
                YAHOO.RelationshipsNS.RightPanel.DataSource.liveData = '';
                YAHOO.RelationshipsNS.RightPanel.DataTable.set('MSG_EMPTY', '<%= Resources.Resource.SelectTable_Label_Text %>');
                YAHOO.RelationshipsNS.RightPanel.RefreshTable('');

                document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value = -1;
                document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value = -1;
            });

            YAHOO.util.Event.addListener('<%= this.JoinTypesDropDown.ClientID %>', "change", function(args) {
                document.getElementById('<%= this.SelectionJoinTypeLabel.ClientID %>').innerHTML = document.getElementById('<%= this.JoinTypesDropDown.ClientID %>').value;
            });
            
            YAHOO.RelationshipsNS.LeftPanel.DataTable.subscribe("cellClickEvent", function(oArgs)
            {
                YAHOO.RelationshipsNS.LeftPanel.DataTable.onEventSelectRow(oArgs);
                YAHOO.RelationshipsNS.callServer('SelectTable: ' + this.getRecord(oArgs.target).getData('tableid'), this.getRecord(oArgs.target));
            });
            
            YAHOO.RelationshipsNS.RightPanel.DataTable.subscribe("cellClickEvent", function(oArgs)
            {
                YAHOO.RelationshipsNS.RightPanel.DataTable.onEventSelectRow(oArgs);
                YAHOO.RelationshipsNS.callServer('SelectField: ' + this.getRecord(oArgs.target).getData('fieldid'), this.getRecord(oArgs.target));                            
            });
            
            YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.textboxChangeEvent.subscribe(function(eventName, args) { if(args[0].getInputEl().value == '') YAHOO.RelationshipsNS.LeftPanel.RefreshTable('');});
            YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.itemSelectEvent.subscribe(function(eventName, args) { YAHOO.RelationshipsNS.LeftPanel.RefreshTable(args[1].innerHTML);});
            YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.dataRequestEvent.subscribe(function(eventName, args) { YAHOO.RelationshipsNS.LeftPanel.RefreshTable(args[0].getInputEl().value);});

            //Bug fixing JiraID:: CBOE-712
            YAHOO.RelationshipsNS.LeftPanel.TagAutoComp.textboxKeyEvent.subscribe(function(eventName,args){ if (parseInt(args[0].getInputEl().value.length)==0) YAHOO.RelationshipsNS.LeftPanel.RefreshTable(''); });            

        };
        
        YAHOO.RelationshipsNS.subscribeToEvents();
        
        
            
        if(document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value != -1)
        {
            var data = YAHOO.RelationshipsNS.LeftPanel.DataTable.getRecordSet().getRecords();
            for(i = 0 ; i < data.length; i++)
            {
                if(data[i].getData('tableid') == document.getElementById('<%= this.SelectedTableIDHidden.ClientID %>').value)
                {
                    var index = YAHOO.RelationshipsNS.LeftPanel.DataTable.getRecordIndex(data[i]);
                    var page = Math.floor((index/YAHOO.RelationshipsNS.LeftPanel.Paginator.getRowsPerPage())) + 1;
                    YAHOO.RelationshipsNS.LeftPanel.Paginator.setPage(page, false);
                    YAHOO.RelationshipsNS.LeftPanel.DataTable.selectRow(data[i]);
                    break;
                }
            }
        }
        
        if(document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value != -1)
        {
            var data = YAHOO.RelationshipsNS.RightPanel.DataTable.getRecordSet().getRecords();
            for(i = 0 ; i < data.length; i++)
            {
                if(data[i].getData('fieldid') == document.getElementById('<%= this.SelectedFieldIDHidden.ClientID %>').value)
                {
                    var index = YAHOO.RelationshipsNS.RightPanel.DataTable.getRecordIndex(data[i]);
                    var page = Math.floor((index/YAHOO.RelationshipsNS.RightPanel.Paginator.getRowsPerPage())) + 1;
                    YAHOO.RelationshipsNS.RightPanel.Paginator.setPage(page, false);
                    YAHOO.RelationshipsNS.RightPanel.DataTable.selectRow(data[i]);
                    break;
                }
            }
        }
    });
    </script>
</asp:Content>

