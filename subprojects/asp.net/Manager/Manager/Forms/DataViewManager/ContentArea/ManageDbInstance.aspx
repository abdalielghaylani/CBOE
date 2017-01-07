<%@ Page Language="C#" AutoEventWireup="true" Inherits="Manager.Forms.DataViewManager.ContentArea.ManageDbInstance" CodeBehind="ManageDbInstance.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework, Version=12.1.0.0, Culture=neutral, PublicKeyToken=1e3754866626dfbf"
    Namespace="CambridgeSoft.COE.Framework.Controls" TagPrefix="cc2" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="server"  >
    <style type="text/css">
    .yui-skin-sam .yui-dt-body
    {
        cursor:pointer;
    }

    #single
    {
        margin-top:2em;
    }
    </style>
    <asp:ScriptManager ID="ScriptManager1" 
    EnablePageMethods="true" 
    EnablePartialRendering="true" runat="server" />
    <script language="javascript"  type="text/javascript">
        YAHOO.namespace("coemanager.dbInstance");
        function ConfirmPannel(title,message) {
            YAHOO.coemanager.dbInstance.ConfirmPanel = new YAHOO.widget.Panel("ConfirmPanel",
                {
                    close: true,
                    width: "400px",
                    visible: false,
                    draggable: false,
                    constraintoviewport: true,
                    modal: true,
                    fixedcenter: true,
                    zIndex: 17000
                }
            );
            document.getElementById('<%= this.ConfirmPanelText.ClientID %>').innerHTML = message;
            document.getElementById('<%= this.ConfirmPannelTitle.ClientID %>').innerHTML = title;
            YAHOO.coemanager.dbInstance.ConfirmPanel.render(document.forms[0]);
            YAHOO.coemanager.dbInstance.ConfirmPanel.show();
        }

        function InformationPannel(title,message) {
            YAHOO.coemanager.dbInstance.InformationPannel = new YAHOO.widget.Panel("InformationPannel",
                {
                    close: true,
                    width: "400px",
                    visible: false,
                    draggable: false,
                    constraintoviewport: true,
                    modal: true,
                    fixedcenter: true,
                    zIndex: 17000
                }
            );
            document.getElementById('<%= this.InformationPannelText.ClientID %>').innerHTML = message;
            document.getElementById('<%= this.InformationPannelTitle.ClientID %>').innerHTML = title;
            YAHOO.coemanager.dbInstance.InformationPannel.render(document.forms[0]);
            YAHOO.coemanager.dbInstance.InformationPannel.show();
        }

        function showModalFrame(url, title)
        {
            InitModalIframeControl_<%= this.MoldalIFrame.ClientID %>(url, title, true);
        }

        function showEditFrame(url,title)
        {
            var rows = instanceDataTable.getSelectedRows();
            var name;
            if(rows.length>0)
            {
                name=instanceDataTable.getRecordSet().getRecord(rows[0]).getData('Name');
                if(name)
                {
                    InitModalIframeControl_<%= this.MoldalIFrame.ClientID %>(url+'?InstanceName='+name, title, true);
                }
            }
        }

        var instanceDataTable;
        var selectRows;

        function deleteInstance()
        {
            selectRows = instanceDataTable.getSelectedRows();
            var name;
            if(selectRows.length>0)
            {
                name=instanceDataTable.getRecordSet().getRecord(selectRows[0]).getData('Name');
                if(name)
                {
                    PageMethods.IsDbInstanceExists(name, function(data){
                        if(data){
                            ConfirmPannel('Delete Data Source','Are you sure you want to delete this data source['+name+']?');    
                        } else {
                            InformationPannel("Delete Data Source", 'Data source ['+name+'] cannot be found on server, it may be deleted, please refresh page'); 
                        }
                    });                    
                }
            }

            return false;
        }

        function deleteInstanceProcess()
        {
            selectRows = instanceDataTable.getSelectedRows();
            var name;
            if(selectRows.length>0)
            {
                name=instanceDataTable.getRecordSet().getRecord(selectRows[0]).getData('Name');
                if(name)
                {
                    SetProgressLayerVisibility(true);
                    var deleteClientId='<%= this.DeleteInstanceBtn.ClientID %>'+'_ActionButton';
                    var deleteBtn=document.getElementById(deleteClientId);
                    deleteBtn.disabled=true;
                    PageMethods.DeleteInstance(name, onDelComplete, OnDelFailed);
                }
            }
        }

        function CloseModal(refresh)
        {
            if(typeof(CloseCOEModalIframe) == 'function')
                CloseCOEModalIframe();
        }

        function OnDelFailed(error, userContext, methodName) {
            if (error.get_message().toUpperCase()=='AUTHENTICATION FAILED.'){
                top.location.href="/COEManager/Forms/Public/ContentArea/Login.aspx";
            }else{
                InformationPannel("Delete Data Source", error.get_message());
            }
        }

        function onDelComplete(result, userContext, methodName)
        {
            SetProgressLayerVisibility(false);
            var deleteClientId='<%= this.DeleteInstanceBtn.ClientID %>'+'_ActionButton';
            var deleteBtn=document.getElementById(deleteClientId);
            deleteBtn.disabled=false;

            if(result.length>0)
            {                
                var r=/<html/ig;
                if(r.test(result)){
                    top.location.href="/COEManager/Forms/Public/ContentArea/Login.aspx";
                }else{
                    InformationPannel("Delete Data Source", result);
                }               
            }
            else
            {
                instanceDataTable.deleteRow(instanceDataTable.getRecordIndex(selectRows[0]));
                InformationPannel("Delete Data Source","The data source is deleted.");
            }
        }

        function SetProgressLayerVisibility(visible)
        {
            if (visible) 
            {
                document.getElementById('UpdateProgressDiv').style.display = 'block';
            }
            else 
            {
                document.getElementById('UpdateProgressDiv').style.display = 'none';
            }
        }

        function CheckAndEditInstance(){
            var rows = instanceDataTable.getSelectedRows();
            if(rows.length>0) {
               // Gets the data source name.
               var name=instanceDataTable.getRecordSet().getRecord(rows[0]).getData('Name');
               if(name) {
                    // Check if the data source exists or not.
                   PageMethods.IsDbInstanceExists(name, function(data){
                        // If exists, continue the edit process.
                        if(data){                            
                            showEditFrame('AddInstances.aspx', 'Edit Data Source');
                        } else {
                            // It maybe deleleted, show the warning message.
                            InformationPannel("Edit Data Source", 'Data source ['+name+'] cannot be found on server, it may be deleted, please refresh page');
                        }
                   });
               }
            }
        }

    </script>
    <div style="border:1px solid #0099ff;width:980px;padding: 0px;">
        <table class="PagesContentTable">
            <tr>
                <td>
                    <div class="BaseTable" style="border:1px solid #0099ff;width:979px;padding: 0px;">
                        <div style="text-align:center;margin:5px" runat="server">
                            <COEManager:ImageButton runat="server" ButtonText="Add Data Source" ButtonMode="ImgAndTxt" TypeOfButton="AddSchema" 
                                 CausesValidation="false" OnClientClick="showModalFrame('AddInstances.aspx', 'Add Data Source'); return false;" />
                            <COEManager:ImageButton Id="EditInstanceBtn" runat="server" ButtonText="Edit Data Source" ButtonMode="ImgAndTxt" TypeOfButton="Edit"
                                 CausesValidation="false" OnClientClick="CheckAndEditInstance(); return false;"/>
                            <COEManager:ImageButton ID="DeleteInstanceBtn" runat="server" ButtonText="Delete Data Source" ButtonMode="ImgAndTxt" TypeOfButton="Remove"
                                CausesValidation="false" OnClientClick="return deleteInstance();"/>
                        </div>
                    </div>
                    <script language="javascript" type="text/javascript">
                    YAHOO.util.Event.addListener(window, "load", function() {
                        InitDataTable = function() {
                            var myColumnDefs = [
                                { key: "ID",  resizeable: true },
                                { key: "Name",  resizeable: true},
                                { key: "Data Source Type",field:"TypeName",  resizeable: true},
                                { key: "Driver Type",field:"DriverName",  resizeable: true},
                                { key: "Host",   field:"HostName",resizeable: true },
                                { key: "Port",  resizeable: true},
                                { key: "SID",  resizeable: true }
                            ];

                            var myDataSource = new YAHOO.util.LocalDataSource(<%=InstanceJson %>);
                            myDataSource.responseSchema = {
                                fields: ["ID", "Name", "TypeName", "DriverName", "HostName", "Port", "SID"]
                            };
                            var oConfigs = { 
                                selectionMode:"single",
                                paginator: new YAHOO.widget.Paginator({ 
                                    rowsPerPage: 20 
                                })
                            }; 

                            instanceDataTable = new YAHOO.widget.DataTable("instanceList",
                                    myColumnDefs,
                                    myDataSource,
                                    oConfigs);

                            instanceDataTable.subscribe("rowMouseoverEvent", instanceDataTable.onEventHighlightRow);
                            instanceDataTable.subscribe("rowMouseoutEvent", instanceDataTable.onEventUnhighlightRow);
                            instanceDataTable.subscribe("rowClickEvent", function(oArgs){
                                // select the row.
                                instanceDataTable.onEventSelectRow(oArgs);

                                SetDeleteStatus();        
                            });
                            
                            this.instanceDataTable.selectRow(this.instanceDataTable.getRow(0));
                            SetDeleteStatus();

                            return {
                                oDS: myDataSource,
                                oDT: instanceDataTable
                            };
                        }();
                    });

                    function SetDeleteStatus(){
                        var selectedRow = this.instanceDataTable.getSelectedRows();
                        var instanceName=instanceDataTable.getRecordSet().getRecord(selectedRow[0]).getData('Name');
                        var deleteClientId='<%= this.DeleteInstanceBtn.ClientID %>'+'_ActionButton';
                        var editClientId='<%= this.EditInstanceBtn.ClientID %>'+'_ActionButton';
                        var deleteBtn=document.getElementById(deleteClientId);
                        var editBtn=document.getElementById(editClientId);
                        var mainDataSource= document.getElementById('<%= this.hdMainDataSource.ClientID %>').value;

                        if(document.getElementById('UpdateProgressDiv').style.display != 'none'){
                            if(instanceName == mainDataSource){
                                deleteBtn.disabled=true;
                                editBtn.disabled=true;
                                deleteBtn.setAttribute('title',"This is primary data source, can not be deleted.");
                                editBtn.setAttribute('title',"This is primary data source, can not be modified.");
                            }else{
                                deleteBtn.disabled=true;
                                editBtn.disabled=false;
                                deleteBtn.setAttribute('title','');
                                editBtn.setAttribute('title','');
                            }
                            return;
                        }

                        if(instanceName == mainDataSource){
                            deleteBtn.disabled=true;
                            editBtn.disabled=true;
                            deleteBtn.setAttribute('title',"This is primary data source, can not be deleted.");
                            editBtn.setAttribute('title',"This is primary data source, can not be modified.");
                        } else {
                            deleteBtn.disabled=false;
                            editBtn.disabled=false;
                            deleteBtn.setAttribute('title','');
                            editBtn.setAttribute('title','');
                        }
                    }
                    </script>
                    <div id="instanceList" align="center" ></div> 
                    <div id="UpdateProgressDiv" style="z-index: 340; display: none;">
                        <img id="ProgressImage" alt="Processing" src="../../../App_Themes/Common/Images/searching.gif"
                        style="position: absolute; top: 72px; left: 762px; z-index: 340;" />
                        <asp:HiddenField id="hdMainDataSource" Value=""  runat="server"/>
                    </div>
                    <div align="right" style="width:979px;padding: 0px;" >
                        <COEManager:ImageButton ID="DoneImageButton" runat="server" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Done.png" TypeOfButton="Done" />
                    </div>
                </td>
             </tr>
        </table>
    </div>
    <cc2:COEModalIFrame runat="server" BodyURL="" ID="MoldalIFrame" HeaderText="Add Data Source" 
        ModalPanelSettings="modal: true, fixedCenter: true, constraintoviewport: true" PaneInsideHeight="210px" PaneInsideWidth="400px" />
    <div id="ConfirmPanel" style="visibility:hidden;">
        <div class="hd" id="ConfirmPannelTitle" runat="server">Confirm</div>
        <div class="bd" style="text-align:center;">
            <img src="../../../App_Themes/Blue/Images/Warning.png" alt="Warning" style="float:left;" />
            <p id="ConfirmPanelText" runat="server"></p>
            <div style="clear:both;">
                <COEManager:ImageButton ID="AbortImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnClientClick="YAHOO.coemanager.dbInstance.ConfirmPanel.hide();return false;" />
                <COEManager:ImageButton ID="OkImageButton" runat="server" TypeOfButton="Save" ButtonText="Yes" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" OnClientClick="YAHOO.coemanager.dbInstance.ConfirmPanel.hide();deleteInstanceProcess();return false;"/>
            </div>
        </div>
    </div>
    <div id="InformationPannel" style="visibility:hidden;">
        <div class="hd" id="InformationPannelTitle" runat="server">Information</div>
        <div class="bd" style="text-align:center;">
            <p id="InformationPannelText" runat="server"></p>
            <div style="clear:both;">
                <COEManager:ImageButton runat="server" TypeOfButton="Done" ButtonText="OK" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" OnClientClick="YAHOO.coemanager.dbInstance.InformationPannel.hide();return false;"/>
            </div>
        </div>
    </div>
</asp:Content>
