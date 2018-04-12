<%
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
'****************************************************************************************
'*	PURPOSE: fetch all container attributes for a given container_id and store them  
'*			in session variables for use by container view tab interface			 	                            
'*	INPUT: ContainerId AS Long, GetDbData AS Boolean passed as QueryString parameters  	                    			
'*	OUTPUT: Populates session variables with container attributes										
'****************************************************************************************
Dim Conn
Dim RS

' Receive Posted data
ContainerID = Request("ContainerID")
GetData = Request.QueryString("GetData")

if NOT ContainerID = "" then 
	Session("ContainerID") = ContainerID
End if

if ContainerID = "" and LCase(GetData)= "db" then
	Response.Redirect "/cheminv/cheminv/SelectContainerMsg.asp"
End if

RushOrder = Request.Form("RushOrder")
If RushOrder <> "" then 
	Session("RushOrder") = RushOrder
Else	
	RushOrder = Session("RushOrder")
End if

Select Case GetData
	Case "db"
		Call SetContainerSessionVarsFromDb(ContainerID)
	Case Else
		Call SetContainerSessionVarsFromPostedData()
End Select



Sub SetContainerSessionVarsFromDb(pContainerID)
	Call GetInvConnection()
	
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/ReorderContainerSQL.asp"-->
<%
	SQL = SQL & "AND inv_Containers.Container_ID ="  & ContainerID
	
	'Response.Write SQL
	'Response.end
	Set RS= Conn.Execute(SQL)
	'Response.write RS.GetString(,,"&nbsp;","<BR>")
	'Response.end
	
	if RS.BOF AND RS.EOF then
		Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
		Response.Write "<P><CODE>Error:SetContainerSessionVarsFromDb</CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve container data for Container_ID " & ContainerID & "</SPAN>"
		Response.Write "</td></tr></table>"
		Response.end
	end if
	
	'set the session variables
	Session("ContainerID") = RS("Container_ID").value
	Session("DeliveryLocationID") = RS("Location_ID_FK").value
	Session("ContainerName") = RS("Container_Name").value
	Session("Comments") = RS("Container_Comments")
	Session("OwnerID") = RS("Owner_ID_FK").value
	Session("CurrentUserID") = Ucase(Session("USERNAMECHEMINV"))
	Session("Project") = RS("Project_No").value
	Session("Job") = RS("Job_No").value	
	'Session("DueDate") = Month(Now() + 1) & "/" & Day(Now() + 2) & "/" & Year(Now() + 1)
	tempDate = Month(Now() + 1) & "/" & Day(Now() + 2) & "/" & Year(Now() + 1)
	Session("DueDate") = ConvertDateToStr(Application("DATE_FORMAT"), tempDate)
	Session("RegID") = iif(Not isNULL(RS("Reg_ID_FK").value),RS("Reg_ID_FK").value,"")
	
	Project = GetUserProperty(Session("UserNameCheminv"), "Project")
	If Project <> "" Then
		Session("Project") = Project
		Session("Job") = GetUserProperty(Session("UserNameCheminv"), "Job")
	End If
	
	'Check for an 'Available' container with the same compound
	'Response.Write SQL2
	'Response.end
	if not (Session("SupplierID")="" and Session("SupplierCatNum")="") then 
		Set RS2 = Conn.Execute(SQL2)
		if RS2.BOF AND RS2.EOF then
			'No matching 'Available' container, don't hassle the customer
			Session("Hassle") = 0
		else
			Session("Hassle") = 1
		end if
	else
		Session("Hassle") = 0
	end if 
	Session("ReorderReason") = ""
	Session("ReorderReasonOther") = ""

	Session("NumCopies") = "1"
End sub


Sub SetContainerSessionVarsFromPostedData()
	' Receive Posted data
	iNumCopies = Request.Form("iNumCopies")
	DeliveryLocationID = Request.Form("iDeliveryLocationID")
	DeliveryLocationName = Request.Form("iDeliveryLocationName")
	iContainerID = Request.Form("iContainerID")
	iContainerName = Request.Form("iContainerName")
	iComments = Request.Form("iComments")
	iOwnerID = Request.Form("iOwnerID")
	iCurrentUserID = Request.Form("iCurrentUserID")
	iProject = Request.Form("iProject")
	iJob = Request.Form("iJob")	
    iDueDate = Request.Form("iDueDate")
	iReorderReason = Request.Form("iReorderReason")
	iReorderReasonOther = Request.Form("iReorderReasonOther")
	
	' Set session variables to store posted data
	if Not IsEmpty(DeliveryLocationID) then Session("DeliveryLocationID")= DeliveryLocationID
	if Not IsEmpty(DeliveryLocationName) then Session("DeliveryLocationName")= DeliveryLocationName
	if Not IsEmpty(iContainerID)then  Session("ContainerID")= iContainerID
	if Not IsEmpty(iNumCopies)then Session("NumCopies")= iNumCopies
	if Not IsEmpty(iContainerName) then Session("ContainerName")= iContainerName
	if Not IsEmpty(iComments) then Session("Comments")= iComments
	if Not IsEmpty(iOwnerID) then Session("OwnerID")= iOwnerID
	if Not IsEmpty(iCurrentUserID) then Session("CurrentUserID")= iCurrentUserID
	if Not isEmpty(iProject) Then Session("Project") = iProject
	if Not isEmpty(iJob) Then Session("Job") = iJob
	if Not IsEmpty(iDueDate) then Session("DueDate")= iDueDate
	if Not isEmpty(iReorderReason) Then Session("ReorderReason") = iReorderReason
	if Not isEmpty(iReorderReasonOther) Then Session("ReorderReasonOther") = iReorderReasonOther
End Sub


' Set local variables
	ContainerID = Session("ContainerID")
	DeliveryLocationName = Session("DeliveryLocationName")
	DeliveryLocationID = Session("DeliveryLocationID")
	ContainerName = Session("ContainerName")
	NumCopies = Session("NumCopies")
	Comments = Session("Comments")
	OwnerID = Session("OwnerID")
	CurrentUserID = Session("CurrentUserID")
	Project = Session("Project")
	Job = Session("Job")	
	DueDate = Session("DueDate")
	ReorderReason = Session("ReorderReason")
	ReorderReasonOther = Session("ReorderReasonOther")
%>
