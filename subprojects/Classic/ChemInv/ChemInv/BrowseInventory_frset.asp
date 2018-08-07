<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim RS
Dim cmd
'if Not Session("UserValidated" & "cheminv") = 1 then
'	response.redirect "../login.asp?dbname=cheminv&formgroup=base_form_group&perform_validate=0"
'end if
Qstr = Request.QueryString

GotoNode = Request.QueryString("GotoNode")
ClearNodes = Request.QueryString("ClearNodes")
FormPopup = Request.QueryString("FormPopup")
if ClearNodes = "" then ClearNodes = "1"
SelectContainer = Request.QueryString("SelectContainer")
SelectWell = Request.QueryString("SelectWell")
if Session("DefaultLocation") = "" then Session("DefaultLocation")= GetUserProperty(Session("UserNameCheminv"),"INVDefLoc")
If Session("DefaultLocation")="" OR IsNULL(Session("DefaultLocation"))  then Session("DefaultLocation")= 0	

if ClearNodes = "0" then
	OpenNodes = Session("TreeViewOpenNodes1")
Else
	OpenNodes = ""	
End if

If Len(GotoNode) = 0 then 
	GotoNode = Session("CurrentLocationID")	
	If Len(GotoNode) = 0 then GotoNode = Session("DefaultLocation")
End if
    GotoNode=GetGotoNode(GotoNode)
    
If Len(SelectContainer) = 0 then 
	SelectContainer = Session("CurrentContainerID")
	If Len(SelectContainer) = 0 then SelectContainer = 0
End if

'BrowseTreeQS = "ClearNodes=" & ClearNodes & "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" & GotoNode & "&node=0&sNode=" & GotoNode & "&SelectContainer=" & SelectContainer & "&SelectWell=" & SelectWell & OpenNodes & "&Exp=Y#" & GotoNode
BrowseTreeQS = "ClearNodes=" & ClearNodes & "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" & GotoNode & "&sNode=" & GotoNode & "&SelectContainer=" & SelectContainer & "&SelectWell=" & SelectWell & OpenNodes & "&Exp=Y#" & GotoNode

'Create Function to Fix CSBR-78419 and CSBR-78452
Function GetGotoNode(GotoNode)
    Dim GotoNodeTemp
    Dim bSelect
    bSelect=false
    if INSTR(1,GotoNode,",")>0 then
        GotoNodeTemp=Split(GotoNode,",")
        if Ubound(GotoNodeTemp) > 0 Then
            if isRackLocation(GotoNodeTemp(0)) then            
                SQL = "SELECT PARENT_ID FROM INV_LOCATIONS WHERE Location_id =?" 
                Set Cmd = GetCommand(Conn, SQL, adCmdText)
                Cmd.Parameters.Append Cmd.CreateParameter("Location_id", adNumeric, 1, 0, cInt(GotoNodeTemp(0)))
                Set RS = Cmd.Execute
                GetGotoNode = RS("PARENT_ID")
                bSelect=true
            end if        
        End if
    Else
        if isRackLocation(GotoNode) then            
            SQL = "SELECT PARENT_ID FROM INV_LOCATIONS WHERE Location_id =?" 
            Set Cmd = GetCommand(Conn, SQL, adCmdText)
            Cmd.Parameters.Append Cmd.CreateParameter("Location_id", adNumeric, 1, 0, cInt(GotoNode))
            Set RS = Cmd.Execute
            GetGotoNode = RS("PARENT_ID")
            bSelect=true
        end if 
    End if
    if bSelect=false then
        GetGotoNode=GotoNode
    End if
End Function
%>
<!DOCTYPE html>
<html>
<head>
<link rel="shortcut icon" href="/cheminv/graphics/favicon.ico" type="image/x-icon">	
<title><%=Application("appTitle")%></title>
</head>
<meta http-equiv="X-UA-Compatible" content="IE=IE11">
<frameset rows="97,*">
	<frameset cols="*,1">
		<frame name="bannerFrame" src="browsebannerFrame.asp?FormPopup=<%=FormPopup%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
		<frame name="HiddenFrame" src="about:blank" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="no">
	</frameset>
	<frameset cols="250,*">
	  <frame name="TreeFrame" target="TreeFrame" src="BrowseTree.asp?<%=BrowseTreeQS%>">
	  <frameset rows="225,*">
		<frame name="ListFrame" src="BuildList.asp?SelectContainerID=<%=SelectContainer%>" MARGINWIDTH="0" MARGINHEIGHT="0">
		<frame name="TabFrame" src="SelectContainerMsg.asp" MARGINWIDTH="0" MARGINHEIGHT="0">
	  </frameset>
</frameset>  
  <body>

  <p>This page uses frames, but your browser doesn't support them.</p>

  </body>
  </noframes>
</frameset>

</html>
