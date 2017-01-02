<%@ Page Language="C#" AutoEventWireup="true" CodeFile="leftPanel.aspx.cs" Inherits="leftPane" %>

<%@ Register TagPrefix="igmisc" Namespace="Infragistics.WebUI.Misc" Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="iglbar" Namespace="Infragistics.WebUI.UltraWebListbar" Assembly="Infragistics2.WebUI.UltraWebListbar.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="ignav" Namespace="Infragistics.WebUI.UltraWebNavigator" Assembly="Infragistics2.WebUI.UltraWebNavigator.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>


<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Cambridgesoft SDK Documentation</title>
		<script type="text/javascript">
	
		//Used to determine if the "Dock Panel" is pinned or not
//		var _pinned = true;
//		
//		//Toggles the "Dock Panel" to either Pinned or Docked State
//		function toggleDockPanel(){
//			var theMainMenuText = document.getElementById('theMainMenuText');
//	//		var theListBar = iglbar_getListbarById('wb');
//			var theImage = document.getElementById('theDockPanelImage');
//			var theFrameSet = top.frameset?top.frameset:top.document.getElementById("frameSet2");
//			var theHeader = document.getElementById('Mainbg');
//			var thedotdot = document.getElementById('dotdot');

//			if (_pinned){  
//				theFrameSet.setAttribute("cols", "28, *");
//	//			theListBar.Element.style.display = 'none';
//				theMainMenuText.style.display = 'none';
//				theImage.src='./images/pushPinClose.gif';
//				theHeader.style.backgroundImage = "url(./images/toggle_bg_hor.gif)";
//				theHeader.style.backgroundRepeat = "no-repeat";
//				thedotdot.style.backgroundImage = "url(./images/dotdot_bg.gif)";
//				
//				
//				//document.getElementById("theMainMenuText").style.backgroundImage = "url(./images/greenBar_Hover.gif)";
//				
//	
//			}else{
//				
//				theFrameSet.setAttribute("cols", "30%, 70%");
//				theListBar.Element.style.display = '';
//				theMainMenuText.style.display = '';
//				theImage.src='./images/pushPin.gif';
//				theHeader.style.backgroundImage = "url(./images/onyxBar.gif)";
//				thedotdot.style.backgroundImage = "url(./images/onyxBar.gif)";
//				
//			}
//			_pinned = !_pinned;
//		}
		
		
		
		</script>

		

        <script id="Infragistics" type="text/javascript">
<!--

//function wb_HeaderClick(oListbar, oGroup, oEvent){
//}
// -->
</script>

		
	</head>
	<body style="height:100%" bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" XMLNS:iglbar="http://schemas.infragistics.com/ASPNET/WebControls/UltraWebListbar" XMLNS:ignav="http://schemas.infragistics.com/ASPNET/WebControls/WebTree">
		<form id="Form1" method="post" runat="server" style="height:100%">
		<table id="Table1" style="HEIGHT:65%;width:59%; BACKGROUND-IMAGE: url(./images/toggle_bg.gif); background-repeat:no-repeat; background-position:right; " cellSpacing="0" cellPadding="0" bgcolor="#6B6B6B">
			<tr style="HEIGHT:25px;width:100%;">
				<td style="height:25px;width:26%;">
					<table id="dotdot" style="BACKGROUND-IMAGE:url(./images/onyxBar.gif);BACKGROUND-REPEAT:repeat-x;HEIGHT:25px; border-right:1px solid #6B6B6B;" cellSpacing="0" cellPadding="0" width="100%" border="0">
						<tr>
							<td style="width: 8px"><IMG src="./images/move.gif"></td>
							<td id="theMainMenuText" style="FONT-WEIGHT: bold; FONT-SIZE: 9pt; WIDTH: 100%; COLOR: white; BACKGROUND-REPEAT: repeat-x; FONT-FAMILY: Lucida Sans" background="./images/onyxBar.gif">&nbsp;&nbsp; Contents</td>
							<TD id="Mainbg" style="BACKGROUND-IMAGE: url(./images/onyxBar.gif); width: 22px;" align="right">
                                &nbsp;</TD>
						</tr>
							</table></td></tr>
							<tr style="height:100%;width:100%"><td style="width:26%;height:111%">
                                <ignav:ultrawebtree id="wb" runat="server" backcolor="White" bordercolor="White" cursor="Default" defaultimage="" defaultselectedimage="" font-names="Microsoft Sans Serif"
                                    font-size="9pt" forecolor="Navy" height="1000px" indentation="20" javascriptfilename=""
                                    javascriptfilenamecommon="" onnodeselectionchanged="wb_NodeSelectionChanged" webtreetarget="ClassicTree"
                                    width="445px" onnodeclicked="wb_NodeClicked" cssclass="Style1">
                                    <styles>
<ignav:Style BackgroundImage="./D:/SDK Documentation/Sept17/SDKDocumentation/images/background/background4.jpg" CssClass="Style1"></ignav:Style>
</styles>
                                    <parentnodestyle backcolor="White" />
                                    <nodes>
<ignav:Node TargetUrl="HTMLFiles/COEFramework.htm" SelectedImageUrl="" Text="Exploring COEFramework" ImageUrl="" Expanded="True" TargetFrame="main"><Nodes>
<ignav:Node TargetUrl="HTMLFiles/FrameworkServices.htm" SelectedImageUrl="" Text="Framework Services" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node TargetUrl="HTMLFiles/COEDataViewService/DataViewService.htm" SelectedImageUrl="" Text="COEDataViewService" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node SelectedImageUrl="" Text="How To" ImageUrl=""><Nodes>
<ignav:Node TargetUrl="HTMLFiles/COEDataViewService/fetch.htm" SelectedImageUrl="" Text="Fetch the Records for the COEDataViewBO objects" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEDataViewService/add.htm" SelectedImageUrl="" Text="Add a New COEDataViewBO object" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEDataViewService/Update.htm" SelectedImageUrl="" Text="Update the COEDataView Object" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEDataViewService/Delete.htm" SelectedImageUrl="" Text="Delete the COEDataViewBO object" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TagString="COEDataViewServiceTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="COEDataViewService Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEFormService/FormService.htm" SelectedImageUrl="" Text="COEFormService" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node SelectedImageUrl="" Text="How To" ImageUrl=""><Nodes>
<ignav:Node TargetUrl="HTMLFiles/COEFormService/fetch.htm" SelectedImageUrl="" Text="Fetch the Records for the COEFormBO objects" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEFormService/add.htm" SelectedImageUrl="" Text="Add a New COEFormBO object" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEFormService/update.htm" SelectedImageUrl="" Text="Update a COEFormBO object" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEFormService/delete.htm" SelectedImageUrl="" Text="Delete a COEFormBO object" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TagString="COEFormServiceTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="COEFormService Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TagString="COESearchServiceTest" TargetUrl="HTMLFiles\COESearchService\SearchService.htm" SelectedImageUrl="" Text="COESearchService" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node SelectedImageUrl="" Text="How To" ImageUrl=""><Nodes>
<ignav:Node TargetUrl="HTMLFiles\COESearchService\GetHitlist.htm" SelectedImageUrl="" Text="Get Hitlist" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\COESearchService\GetData.htm" SelectedImageUrl="" Text="Get Data" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\COESearchService\PerformSearch.htm" SelectedImageUrl="" Text="Perform Search" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TagString="COESearchServiceTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="COESearchService Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\DatabasePublishingService\DatabasePublishingService.htm" SelectedImageUrl="" Text="COEDatabasePublishingService" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node SelectedImageUrl="" Text="How To" ImageUrl=""><Nodes>
<ignav:Node TargetUrl="HTMLFiles\DatabasePublishingService\GetDatabase.htm" SelectedImageUrl="" Text="Get Database" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\DatabasePublishingService\GetUserList.htm" SelectedImageUrl="" Text="Get the List of Users" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\DatabasePublishingService\PublishDatabase.htm" SelectedImageUrl="" Text="Publish Database" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TagString="COEDatabasePublishingServiceTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="COEDatabasePublishingService Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COETableEditorService/TableEditorService.htm" SelectedImageUrl="" Text="COETableEditorService" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node SelectedImageUrl="" Text="How To" ImageUrl=""><Nodes>
<ignav:Node TargetUrl="HTMLFiles/COETableEditorService/add.htm" SelectedImageUrl="" Text="Add Record" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COETableEditorService/update.htm" SelectedImageUrl="" Text="Update Record" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COETableEditorService/delete.htm" SelectedImageUrl="" Text="Delete Record" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COETableEditorService/getlistoftables.htm" SelectedImageUrl="" Text="Get the List of Tables" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TagString="COETableEditorServiceTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="COETableEditorService Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/COEHitListService.htm" SelectedImageUrl="" Text="COEHitListService" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node SelectedImageUrl="" Text="How To" ImageUrl=""><Nodes>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/add.htm" SelectedImageUrl="" Text="Add a new COEHitListBO object" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/update.htm" SelectedImageUrl="" Text="Update the records of a COEHitListBO object" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/delete.htm" SelectedImageUrl="" Text="Delete the records of a COEHitListBO object" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/fetch.htm" SelectedImageUrl="" Text="Fetch the records of a COEHitListBO objects" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/saveCOEHitlistBO.htm" SelectedImageUrl="" Text="Create a SAVE COEHitlistBO object from an existing TEMP COEHitlistBO" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/subtraction.htm" SelectedImageUrl="" Text="Perform Subtraction Operation on the two COEHitlistBO objects" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/intersection.htm" SelectedImageUrl="" Text="Perform Intersection Operation on the two COEHitlistBO objects" ImageUrl="" TargetFrame="main"></ignav:Node>
<ignav:Node TargetUrl="HTMLFiles/COEHitListService/union.htm" SelectedImageUrl="" Text="Perform Union Operation on the two COEHitlistBO objects" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TagString="COEHitListServiceTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="COEHitListService Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\ServerControls\ServerControls.htm" SelectedImageUrl="" Text="Server Controls" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node TargetUrl="HTMLFiles\ServerControls\COEDatabasePublishManager.htm" SelectedImageUrl="" Text="Use COEDatabasePublishManager Server Control" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node TagString="COEDatabasePublishManagerTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\ServerControls\DataViewManager.htm" SelectedImageUrl="" Text="Use COEDataViewManager Server Control" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node TagString="COEDataViewManagerTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\ServerControls\TableEditor.htm" SelectedImageUrl="" Text="Use COETableManager Server Control" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node TagString="COETableManagerTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\ServerControls\FormManager.htm" SelectedImageUrl="" Text="Use COEFormManager Server Control" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node TagString="COEFormManagerTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\ServerControls\COEWebGrid.htm" SelectedImageUrl="" Text="Use COEWebGrid Server Control" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node TagString="COEWebGridTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
<ignav:Node TargetUrl="HTMLFiles\ServerControls\COEHitListManager.htm" SelectedImageUrl="" Text="Use COEHitListManager Control" ImageUrl="" TargetFrame="main"><Nodes>
<ignav:Node TagString="COEHitListManagerTest" TargetUrl="contents.aspx" SelectedImageUrl="" Text="Sample Application" ImageUrl="" TargetFrame="main"></ignav:Node>
</Nodes>
</ignav:Node>
</Nodes>
</ignav:Node>
</Nodes>
</ignav:Node>
</nodes>
                                    <levels>
<ignav:Level Index="0" __designer:dtid="562954248388643"></ignav:Level>
<ignav:Level Index="1" __designer:dtid="562954248388644"></ignav:Level>
<ignav:Level Index="2" __designer:dtid="562954248388645"></ignav:Level>
<ignav:Level Index="3" __designer:dtid="562954248388646"></ignav:Level>
<ignav:Level Index="4" __designer:dtid="562954248388647"></ignav:Level>
</levels>
                                    <nodestyle backcolor="White" />
                                    <selectednodestyle backcolor="LightBlue" forecolor="Black" />
                                    <hovernodestyle backcolor="AliceBlue" forecolor="Black" />
                                </ignav:ultrawebtree></td></tr></table>
			
			
		</form>
	</body>

</html>