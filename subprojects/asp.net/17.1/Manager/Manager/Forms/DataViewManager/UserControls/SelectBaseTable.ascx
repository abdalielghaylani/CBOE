<%@ Control Language="C#" AutoEventWireup="true" Inherits="SelectBaseTable" Codebehind="SelectBaseTable.ascx.cs" %>

<script type="text/javascript">
    window.onload = function () {
        SchemasDropDownOnchange();
        var h = document.getElementById("<%=hfScrollPosition.ClientID%>");
        document.getElementById("<%=scrollArea.ClientID%>").scrollTop = h.value;
    }
    function SetDivPosition() {
        var intY = document.getElementById("<%=scrollArea.ClientID%>").scrollTop;
        var h = document.getElementById("<%=hfScrollPosition.ClientID%>");
        h.value = intY;
    }

    function afterpostback() {
        
        var h = document.getElementById("<%=hfScrollPosition.ClientID%>");
        document.getElementById("<%=scrollArea.ClientID%>").scrollTop = h.value;

    }
    function SchemasDropDownOnchange() {

        document.getElementById('<%= SchemasDropDownList.ClientID %>').onchange();

    }

    </script> 

     <asp:HiddenField ID="hfScrollPosition" runat="server" Value="0" />
<div class="BaseTable">
    <div style="margin-bottom:10px;">
        <asp:Label ID="SelectBaseTableLabel" runat="server" SkinID="DataViewTitle" />
    </div>
    <div>
        <asp:DropDownList ID="SchemasDropDownList" runat="server" SkinID="SchemaDropDownList" AutoPostBack="true"   />
    </div>
    <span><asp:RequiredFieldValidator ControlToValidate="RadioButtonList" runat="server" ID="RadioListrReqFieldValidator" SkinID="ReqFieldText"></asp:RequiredFieldValidator></span>   
    <div>      
        <asp:UpdatePanel ID="TablesUpdatePanel" runat="server" UpdateMode="Conditional">
         <ContentTemplate>          
         <div style="height:450px;overflow: auto;"  id="scrollArea" onscroll="SetDivPosition()"   runat="server"  >  
            <asp:RadioButtonList ID="RadioButtonList" runat="server" SkinID="TablesRadioItems"></asp:RadioButtonList>                 
            </div>
         </ContentTemplate>
         <Triggers>
                <asp:AsyncPostBackTrigger ControlID="SchemasDropDownList" EventName="SelectedIndexChanged" />
         </Triggers>
        </asp:UpdatePanel>
    </div>   
</div>
 