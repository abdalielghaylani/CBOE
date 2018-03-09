<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddTable.ascx.cs" Inherits="Manager.Forms.DataViewManager.UserControls.AddTable" %>
<style type="text/css">
#<%=TableTextBox.ClientID %> {
    width:130px;
    position:relative;
}

#<%= this.InstanceDropDownList.ClientID %> {
    width: 120px;
}

#<%= this.SchemaDropDownList.ClientID %> {
    width: 120px;
}
#AutoCompleteContainer {
    width:250px; /* set width here or else widget will expand to fit its container */
    position:relative;
    z-index:1001;
}

#AutoCompleteDiv {
    width:410px;
}

#SchemaDiv, #InstanceDiv {
    width:190px;
    margin-left: 5px;
}

#AutoCompleteDiv, #SchemaDiv, #InstanceDiv {
    padding-left: 6px;
    float:left;
    vertical-align:top;
    height:23px;
}

#PaginatorTop, #PaginatorBottom {
    text-align: center;
    height: 25px;
}

#PaginatorTop, #PaginatorBottom, #tbl {
    padding-left:6px;
    clear:both;
    width: 400px;
}

/* Class for marked rows */ 
tr.myRow { 
    height: 35px;
}
tr.myColorRow { 
    font-family:Verdana;
	color:Red;
	height:20px;
	margin: 15px;
	width: 50px;
}

</style>
<table>
    <tr>
        <td align="left">
            <div style="margin-top: 5px; vertical-align: top; width: 880px; padding-left: 100px;">
                <div class="markup">
                    <div id="InstanceDiv">
                        <label for="<%= this.InstanceDropDownList.ClientID %>">
                            <%= Resources.Resource.Instance_Label_Text %>:</label>
                        <asp:DropDownList ID="InstanceDropDownList" runat="server" OnSelectedIndexChanged="InstanceDropDownList_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                    <div id="SchemaDiv">
                        <label for="<%= this.SchemaDropDownList.ClientID %>">
                            <%= Resources.Resource.Schema_Label_Text %>:</label>
                        <asp:DropDownList ID="SchemaDropDownList" runat="server">
                        </asp:DropDownList>
                    </div>
                    <div id="AutoCompleteDiv">
                        <label for="<%= this.TableTextBox.ClientID %>">
                            <%= Resources.Resource.FilterTable_Label_Text %>:</label>
                        <asp:TextBox ID="TableTextBox" runat="server" Width="250px"></asp:TextBox>
                        <div id="AutoCompleteContainer">
                        </div>
                        <div style="left: 35px; top: 35px; width: 400px; position: absolute;">
                            <asp:Label ID="SelectTableLabel" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                    <div style="width: 740px;">
                        <div id="MasterTablesDiv" style="float: left; margin-left: 5px;">
                            <div id="PaginatorTop">
                            </div>
                            <div id="tbl">
                            </div>
                            <div id="PaginatorBottom">
                            </div>
                        </div>
                        <div style="float: right; margin-top: 37px;">
                            <div id="PendingTables" style="width: 303px;">
                            </div>
                        </div>
                    </div>
                </div>
                <script type="text/javascript" language="javascript">
                
                YAHOO.util.Event.addListener(window, "load", function() {
        var AddTableNS = YAHOO.namespace('AddTable');
        AddTableNS.myRowFormatter = function(elTr, oRecord) {
        if(oRecord.getData('IsAdded') =='True')
        {
         Dom.addClass(elTr, 'myColorRow')
        }

        else
        {
        Dom.addClass(elTr, 'myRow')
        }
            return true;
        };
         
        
        AddTableNS.pendingTablesDataSource = new YAHOO.util.LocalDataSource();
        AddTableNS.pendingTablesDataSource.responseSchema = { 
            fields : ['table', 'removebutton']
        };
        AddTableNS.dataSource = new YAHOO.util.LocalDataSource();
        AddTableNS.dataSource.responseSchema = { 
            fields : ['tablealias', 'tableschema', 'addbutton','IsAdded']
        };
        <%= this.GetMasterTablesDataSource(this.InstanceDropDownList.SelectedItem==null?string.Empty:this.InstanceDropDownList.SelectedValue,
                this.SchemaDropDownList.SelectedItem == null? string.Empty : this.SchemaDropDownList.SelectedItem.Text) %>
        
        <%= this.GetPendingTablesDataSource() %>
        AddTableNS.dataSource.doBeforeCallback = function (req,raw,res,cb) {
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
        
        AddTableNS.dataTableCols = [
            { key: 'tablealias', sortable: true, width: 310, label: '<%= Resources.Resource.Alias_ColHeader_Caption %>' },
            { key: 'addbutton', sortable: false, width: 35, formatter:YAHOO.widget.DataTable.formatButton, label: '<%= Resources.Resource.Add_Button_Text %>' 

}
        ];

        AddTableNS.paginator = new YAHOO.widget.Paginator({
            rowsPerPage   : 20,
            pageLinks     : 5,
            containers    : ["PaginatorTop", "PaginatorBottom"]
        });

        AddTableNS.dataTableConf = {
            paginator : AddTableNS.paginator,
            sortedBy: {key:'tablealias', dir:YAHOO.widget.DataTable.CLASS_ASC},
            formatRow: AddTableNS.myRowFormatter
        };
        
        AddTableNS.pendingTablesCols = [
            { key: 'table', label: '<%= Resources.Resource.TablesToAdd_Column_Text %>', width: 280 },
            { key: 'removebutton', sortable: false, width: 55, formatter:YAHOO.widget.DataTable.formatButton, label: '<%= Resources.Resource.Remove_Label_Text %>' }
        ];
        
        AddTableNS.pendingTablesConf = {
            formatRow: AddTableNS.myRowFormatter, 
            MSG_EMPTY: '<%= Resources.Resource.ChooseTableToAdd_Label_Text %>'
        };
        
        AddTableNS.dataTable = new YAHOO.widget.DataTable('tbl',AddTableNS.dataTableCols,AddTableNS.dataSource,AddTableNS.dataTableConf);
        AddTableNS.pendingTables = new YAHOO.widget.DataTable('PendingTables',AddTableNS.pendingTablesCols,AddTableNS.pendingTablesDataSource, 

AddTableNS.pendingTablesConf);
        AddTableNS.tagAutoComp = new YAHOO.widget.AutoComplete('<%=this.TableTextBox.ClientID %>','AutoCompleteContainer', AddTableNS.dataSource);
        AddTableNS.tagAutoComp.useIFrame = true; // Enable an iFrame shim under the container element
        AddTableNS.tagAutoComp.typeAhead = false; // Enable type ahead
        AddTableNS.tagAutoComp.resultTypeList = false;
        AddTableNS.tagAutoComp.applyLocalFilter = false;
        AddTableNS.tagAutoComp.autoHighlight = false;
        
        AddTableNS.filterTimeout = null;
        AddTableNS.updateFilter  = function (query) {
            // Reset timeout
            AddTableNS.filterTimeout = null;
            // Reset sort
            var state = AddTableNS.dataTable.getState();
            state.sortedBy = {key:'tablealias', dir:YAHOO.widget.DataTable.CLASS_ASC};

            // Get filtered data
            AddTableNS.dataSource.sendRequest(query,{
                success : AddTableNS.dataTable.onDataReturnInitializeTable,
                failure : AddTableNS.dataTable.onDataReturnInitializeTable,
                scope   : AddTableNS.dataTable,
                argument: state
            });
        };
        
        AddTableNS.refreshTable = function (query) {
                clearTimeout(AddTableNS.filterTimeout);
                if(typeof(query) != 'undefined')
                    AddTableNS.updateFilter(query);
                else
                    AddTableNS.updateFilter();
                return true;
            };
        
        AddTableNS.callServerCompleted = function(arg, context){ 
                    if(arg.indexOf('AddTable: ') != -1)
                    {
                        var rowData = {table: arg.substring(10), removebutton: '<%= Resources.Resource.Remove_Label_Text %>'};
                        AddTableNS.pendingTables.addRow(rowData);
                    }
                    else if(arg.indexOf('RemoveTable: ') != -1)
                    {
                        if(arg.substring(13) == 'True')
                        {
                            AddTableNS.pendingTables.deleteRow(context.target);
                        }
                    }
                    else if(arg.indexOf('FilterSchema: ') != -1) 
                    {
                        eval(arg.substring(14));
                        // Get filtered data
                        AddTableNS.tagAutoComp.getInputEl().value = '';
                        AddTableNS.refreshTable();
                        AddTableNS.dataTable.set('MSG_EMPTY', '<%= Resources.Resource.EmptySchema_Label_Text %>')
                    }
                    return true;
                };
                
        AddTableNS.callServer = function(arg, context){ <%= this.Page.ClientScript.GetCallbackEventReference(this, "arg", "AddTableNS.callServerCompleted", "context") %>; }
        
        AddTableNS.tagAutoComp.textboxChangeEvent.subscribe(function(eventName, args) { if(args[0].getInputEl().value == '') AddTableNS.refreshTable('');});
        AddTableNS.tagAutoComp.itemSelectEvent.subscribe(function(eventName, args) { AddTableNS.refreshTable(args[1].innerHTML);});
        AddTableNS.tagAutoComp.dataRequestEvent.subscribe(function(eventName, args) { AddTableNS.refreshTable(args[0].getInputEl().value);});

        //Bug fixing JiraID:: CBOE-712
        AddTableNS.tagAutoComp.textboxKeyEvent.subscribe(function(eventName,args){ if (parseInt(args[0].getInputEl().value.length)==0) AddTableNS.refreshTable(''); });            


        AddTableNS.dataTable.subscribe("buttonClickEvent", function(oArgs){
            AddTableNS.callServer('AddTable: ' + this.getRecord(oArgs.target).getData('tableschema') + '.' + this.getRecord(oArgs.target).getData('tablealias'), oArgs);
        });
        AddTableNS.pendingTables.subscribe("buttonClickEvent", function(oArgs){
            AddTableNS.callServer('RemoveTable: ' + this.getRecord(oArgs.target).getData('table'), oArgs);
        });
        YAHOO.util.Event.addListener('<%= this.SchemaDropDownList.ClientID %>', "change", function(args) {
            AddTableNS.callServer('FilterSchema: ' + document.getElementById('<%= this.InstanceDropDownList.ClientID %>').value + '.' + document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value, args);
            AddTableNS.dataSource.liveData = '';
            AddTableNS.dataTable.set('MSG_EMPTY', '<%= Resources.Resource.Searching_Label_Text %>');
            AddTableNS.refreshTable();
        });

        function stopEnterKey(evt) {
          var evt = (evt) ? evt : ((event) ? event : null);
          var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
          if ((evt.keyCode == 13) && (node.type=="text"))  {return false;}
        }

        document.onkeypress = stopEnterKey; 
    });
                </script>
            </div>
        </td>
    </tr>
    <tr>
        <td align="right">
            <div>
                <COEManager:ImageButton ID="CancelImageButton" runat="server" ButtonMode="ImgAndTxt"
                    TypeOfButton="Cancel" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png"
                    HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
                <COEManager:ImageButton ID="OkImageButton" runat="server" ButtonMode="ImgAndTxt"
                    TypeOfButton="Save" CausesValidation="true" />
            </div>
        </td>
    </tr>
</table>
