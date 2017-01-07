<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablesTreeView.ascx.cs" Inherits="Manager.Forms.DataViewManager.UserControls.TablesTreeView" %>
<style type="text/css">
    #<%=TableTextBox.ClientID %> {
    width:130px;
    position:relative;
}

#<%= this.SchemaDropDownList.ClientID %> {
    width: 220px;
    margin-left: 43px;
}

#<%= this.InstanceDropDownList.ClientID %> {
    width: 220px;
    margin-left: 23px;
}

#AutoCompleteContainer {
    width:215px; /* set width here or else widget will expand to fit its container */
    position:relative;
}

#AutoCompleteDiv, #SchemaDiv, #InstanceDiv {
    width:400px;
}

#AutoCompleteDiv, #SchemaDiv, #InstanceDiv {
    padding-left: 6px;
    float:left;
    vertical-align:top;
    height:23px;
}

#LeftPanelPaginatorTop, #LeftPanelPaginatorBottom {
    text-align: center;
    height: 25px;
}

#LeftPanelPaginatorTop, #LeftPanelPaginatorBottom, #LeftPanelTbl {
    padding-left:6px;
    clear:both;
    width: 350px;
}
</style>
<script type="text/javascript" language="javascript">
function OpenAddTable(element)
{
    element.href = "AddTable.aspx?schemaSelected=" + document.getElementById('<%= this.InstanceDropDownList.ClientID %>').value + '.' + document.getElementById('<%= this.SchemaDropDownList.ClientID %>').value;
    SetProgressLayerVisibility(true);
    return true;
}
</script>
<table>
    <tr valign="top">
        <td colspan="2">
            <table style="width:400px;">
                <tr valign="top">
                    <td align="left">
                        <div class="markup">
                           <div id="InstanceDiv">
                                <label for="<%= this.InstanceDropDownList.ClientID %>"><%= Resources.Resource.Instance_Label_Text %>:</label> 
                                <asp:DropDownList ID="InstanceDropDownList" ClientIDMode="Static" runat="server" AutoPostBack="true" OnSelectedIndexChanged="InstanceDropDownList_SelectedIndexChanged"></asp:DropDownList> 
                            </div>
                            <div id="SchemaDiv">
                                <label for="<%= this.SchemaDropDownList.ClientID %>"><%= Resources.Resource.Schema_Label_Text %>:</label> 
                                <asp:DropDownList ID="SchemaDropDownList" ClientIDMode="Static" runat="server"></asp:DropDownList> 
                                <asp:LinkButton ID="RemoveSchema" runat="server" CssClass="ImageButton" OnClientClick="RemoveSchemaFromMaster();"><%= Resources.Resource.Remove_Label_Text %></asp:LinkButton>                                
                            </div>
                            <div id="AutoCompleteDiv">
                                <label for="<%= this.TableTextBox.ClientID %>"><%= Resources.Resource.FilterTable_Label_Text %>:</label> 
                                <asp:TextBox id="TableTextBox" runat="server" Width="215px" onkeydown ="return (event.keyCode!=13);"></asp:TextBox>
                                <div id="AutoCompleteContainer">
                                </div>
                            </div>
                            <div style="width:400px;">
                                <div id="TablesDiv" style="float:left;">
                                    <div style="position: absolute; margin-left: 320px; margin-top: 10px;">
                                        <asp:LinkButton id="AddTable" ClientIDMode="Static" runat="server" OnClick="AddTable_Click" CssClass="ImageButton"><%= Resources.Resource.AddTable_Page_Title %></asp:LinkButton>                                        
                                    </div>
                                    <div id="LeftPanelPaginatorTop"></div>
                                    <div id="LeftPanelTbl"></div>
                                    <div id="LeftPanelPaginatorBottom"></div>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </td>
     </tr>
</table>