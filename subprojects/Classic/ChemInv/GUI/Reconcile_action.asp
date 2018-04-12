<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bDebugPrint

bDebugPrint = false

showInList = Request("showInList")
if lcase(showInList) = "plates" then
	assetNameLcase = "plate"
	aseetNameUcase = "Plate"
	assetNameLcasePlural = "plates"
	Set myDict = plate_multiSelect_dict	
else
	assetNameLcase = "container"
	aseetNameUcase = "Container"
	assetNameLcasePlural = "containers"
	Set myDict = multiSelect_dict
end if

VerifiedContainerIDList = DictionaryToList(myDict)

if bDebugPrint then
	Response.Write "Verified containers: " & VerifiedContainerIDList & "<BR>"
	Response.Write "Missing containers: " & Request("MissingContainerIDList") & "<BR>"
	Response.Write "Awol containers: " & Request("awolContainerIDList") & "<BR>"
	Response.Write "ShowInList: "  & Request("showInList") & "<BR>"
	Response.end
End if
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = "VerifiedContainerIDList=" & VerifiedContainerIDList & "&" & Request.Form & Credentials
if showInList = "plates" then
	apiPage = "ReconcilePlate.asp"
else	
	apiPage = "ReconcileContainer.asp"
end if
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/" & apiPage, "ChemInv", FormData)
'response.Write httpResponse
'response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Reconcile Inventory Containers</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
	window.focus();

//this function is for refreshing from this opened frameset page	
function SelectLocationNodeFromReconcile(bClearNodes, LocationID, RemoveLocationID, openNodes, SelectContainer, IsPlateView, SelectWell){
	var url = "/cheminv/cheminv/BrowseTree.asp?" + "ClearNodes=" + bClearNodes + "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" + LocationID +  "&sNode=" + LocationID + "&SelectContainer=" + SelectContainer + "&SelectWell=" + SelectWell;
	if (RemoveLocationID){
		url += "&RemoveNode=" + RemoveLocationID;
	}
	
	if (openNodes){
		url += openNodes;
	}
	url += "&Exp=Y#" + LocationID;
	
	var thetop = top.opener.parent
	//alert(url);
	
	if (thetop.name == "main"){
		// Tree is in search results mode
		var theTreeFrame =  thetop.mainFrame;
		if (theTreeFrame) theTreeFrame.location.reload();
	}
	else if (thetop.TreeFrame){
		//Tree is in Browse mode
		var theTreeFrame = thetop.TreeFrame;
		tempURL = theTreeFrame.location.pathname + theTreeFrame.location.search + theTreeFrame.location.hash 
		theTreeFrame.location.href = url;
		if (tempURL == url){
			if (theTreeFrame) theTreeFrame.location.reload();
		}
	}
	else{
		opener.location.reload();
	}
}
	
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=1 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff Width=90%>
	<TR>
		<TD>
			<% 
			If IsNumeric(httpresponse) then 
				If CLng(httpResponse) > 0 then
					Session("bMultiSelect") = false
					multiSelect_dict.RemoveAll()
					Session("CurrentLocationID") = httpResponse
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					Response.Write "<center><SPAN class=""GuiFeedback"">Containers have been reconciled.</SPAN></center>"
					Response.Write "<SCRIPT language=JavaScript>SelectLocationNodeFromReconcile(0, " & Request("LocationID") & ", 0); top.window.close();</SCRIPT>"
				else				
					Response.Write "<P><CODE>ChemInv API Error: " & Application(httpResponse) & "</CODE></P>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Container could not be reconciled</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				if InStr(1, httpresponse, "VerifiedContainerIDList or AwolContainerIDList") > 0 then
					Response.Write "<center><SPAN class=""GuiFeedback"">You must check at least one container to reconcile.</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				else
					Response.Write httpresponse
					Response.end
				end if
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>