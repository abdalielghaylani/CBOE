<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessagesBox.ascx.cs" Inherits="DocManagerWeb.Forms.Public.UserControls.MessagesBox" %>

<script type="text/javascript">
var popupMaskId = "<%=popupMask.ClientID %>";
var popupContainerId = "<%=popupContainer.ClientID %>"; 
</script>

<script src="/DocManager/Forms/Public/JScripts/MessagesBox.js" type="text/javascript">
</script>

<div id="popupMask" visible="false"  runat="server"  style="position: absolute;
	z-index: 200;
	top: 0px;
	left: 0px;
	width: 100%;
	border: red 2px solid;
	height: 100%;
	opacity: .4;
	filter: alpha(opacity=0);

	background-color:transparent !important;
	background-color: #333333;
	background-image:none;
	background-repeat: repeat;
	display:none;
">
    
</div>
<table id="Designtable" runat="server"  bgcolor="#E0E9F8">
<tr>
<td>MessageBox</td>
</tr>
</table>
<div runat="server" id="popupContainer"  visible="false" style="  width:35; height:15;	position: absolute;
	z-index: 201;
	top: 0px;
	left: 0px;
	display:none;
	padding: 0px;">
	<div id="popupInner" runat="server">
		<div id="popupTitleBar" runat="server" style="	
	font-weight: bold;
	height: 1.3em;
	padding: 5px;
	position: relative;
	z-index: 1203; 
">
			<div runat="server" id="popupTitle" style="	float:left;
	font-size: 1.1em;"><asp:Label ID="lblTitle" runat="server"></asp:Label></div>
			<div runat="server" id="popupControls" style="	float: right;
	cursor: pointer;
	cursor: hand;">
			</div>
		</div>
		
			
			<table   runat="server" id="Maintable" name="Maintable" style="" width="100%" height="100%" border="0">
				<tr>
					<td style="WIDTH: 62px; HEIGHT: 96px" valign="middle" align="center"><asp:image id="iIcon" runat="server"></asp:image></td>
					<td style="HEIGHT: 96px" valign="middle" align="left" colspan="3"><asp:label id="lblMessage" runat="server" Width="100%" Height="100%" ForeColor="#1F336B" Font-Names="Arial"
							Font-Size="Smaller"></asp:label></td>
				</tr>
				<tr align="center">
					<td style="WIDTH: 62px; height: 26px;"></td>
					<td valign="bottom" align="right" style="height: 26px"><asp:button id="btn1" runat="server" Text="Button"  CssClass="button" 
							 OnClick="btn1_Click" Width="82px" OnClientClick="hidePopWin();" CausesValidation="false"></asp:button><asp:button id="btn2" runat="server" Text="Button"  CssClass="button" 
							OnClick="btn2_Click" Width="82px" OnClientClick="hidePopWin();" CausesValidation="false"></asp:button><asp:button id="btn3" runat="server" Text="Button" 
							 OnClick="btn3_Click" Width="82px" OnClientClick="hidePopWin();" CssClass="button" CausesValidation="false"></asp:button></td>
				</tr>
			</table>
		
		
	</div>
	
	</div>