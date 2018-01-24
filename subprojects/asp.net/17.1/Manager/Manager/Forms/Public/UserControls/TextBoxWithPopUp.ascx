<%@ Control Language="C#" AutoEventWireup="true" Inherits="Forms_Public_UserControls_TextBoxWithPopUp" Codebehind="TextBoxWithPopUp.ascx.cs" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<style type="text/css">
    .TextBoxWithPopUpRO{font-size: 10px;font-family: Verdana;color: #000066;background-color:#F1F1F1;height: 15px;width: 150px;border-top: #C0C0C0 1px solid;border-left: #C0C0C0 1px solid;border-right: #C0C0C0 1px solid;border-bottom: #C0C0C0 1px solid;}
    .TextBoxWithPopUp{font-size: 10px;font-family:Verdana;color:#000066;background-color:#FFF;height:15px;width:150px;border:#C0C0C0 1px solid;}
    .TextBoxWithPopUpImage{cursor:pointer;}
    .TextWithPopUpError{font-size:14px;font-family:Verdana;color:#000066;}
</style>
<div class="TextBoxWithPop" runat="server" id="TextBoxWithPop">
    <igtxt:WebTextEdit ID="SummaryTextBox" runat="server" ReadOnly="true" AutoPostBack="true" ></igtxt:WebTextEdit>
    <asp:Image runat="server" ID="TextBoxImage" ImageAlign="Middle" SkinID="TooltipImage" CssClass="TextBoxWithPopUpImage" />   
</div>