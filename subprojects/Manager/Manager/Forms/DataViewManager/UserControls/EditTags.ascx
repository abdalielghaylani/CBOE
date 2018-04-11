<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditTags.ascx.cs" Inherits="Manager.Forms.DataViewManager.UserControls.EditTags" %>
<style type="text/css">
#<%=TagTextBox.ClientID %> {
    width:130px;
    position:relative;
}

#AutoCompleteContainer {
    width:130px; /* set width here or else widget will expand to fit its container */
    position:relative;
    padding-left: 10px;
}
#<%=AddTagButton.ClientID %>, #<%=DeleteTagButton.ClientID %> {
    width:60px;
    position:relative;
    vertical-align: top;
}
#AutoCompleteDiv {
    width:200px;
}
</style>
<table>
    <tr>
        <td align="left">
            <div id="AutoCompleteDiv">
                <asp:TextBox ID="TagTextBox" runat="server" MaxLength="50" onkeyup="ValidateTag(this);" onblur="ValidateTag(this);" ></asp:TextBox><asp:Button
                    ID="AddTagButton" runat="server" Text="Add" />
                <div id="AutoCompleteContainer">
                </div>
            </div>
        </td>
    </tr> 
    <tr>
        <td>
            <div style="margin-top: 5px;">
                <asp:ListBox ID="TagsListBox" runat="server" Height="250px" Width="200px" SelectionMode="Multiple">
                </asp:ListBox><asp:Button ID="DeleteTagButton" runat="server" Text="Delete" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
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

<script type="text/javascript" language="javascript">
    var dsLocalArray = new YAHOO.util.LocalDataSource();
    <%= this.GetAllTags() %>
    var tagAutoComp = new YAHOO.widget.AutoComplete('<%= this.TagTextBox.ClientID %>','AutoCompleteContainer', dsLocalArray);
    tagAutoComp.useIFrame = true; // Enable an iFrame shim under the container element
    tagAutoComp.typeAhead = true; // Enable type ahead
    tagAutoComp.queryMatchCase = false; // Case insensitive matching
    tagAutoComp.queryMatchContains = true;
</script>

<script type="text/javascript"> // Fix for CSBR-153934. Blank Tags are not allowed to add.
function ValidateTag(t)
{
    if (t.value.match(/\s/g))
    {
     if (t.value == 0) 
       {
            var l = 0; var r = t.length - 1;
            t.value="";
       }
    }
}
</script>

