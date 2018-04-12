<%
Response.ExpiresAbsolute = Now()
'****************************************************************************************
'*	PURPOSE: fetch all container attributes for a given container_id and store them  
'*			in session variables for use by container view tab interface			 	                            
'*	INPUT: ContainerId AS Long, GetDbData AS Boolean passed as QueryString parameters  	                    			
'*	OUTPUT: Populates session variables with container attributes										
'****************************************************************************************

ExpYears = 2

RushOrder = Request.Form("RushOrder")
If RushOrder <> "" then 
	Session("RushOrder") = RushOrder
Else	
	RushOrder = Session("RushOrder")
End if

' Receive Posted data
iDueDate = Request.Form("iDueDate")
iProject = Request.Form("iProject")
iJob = Request.Form("iJob")
iDeliveryLocationID = Request.Form("iDeliveryLocationID")
iOrderReason = Request.Form("iOrderReason")
iOrderReasonOther = Request.Form("iOrderReasonOther")

' Set session variables to store posted data
if Not IsEmpty(iDueDate) then
	Session("DueDate")= iDueDate
else
	tempDate = Month(Now() + 1) & "/" & Day(Now() + 2) & "/" & Year(Now() + 1)
	Session("DueDate") = ConvertDateToStr(Application("DATE_FORMAT"), tempDate)
end if
if Not IsEmpty(iProject) then 
	Session("Project")= iProject
else
	Session("Project") = GetUserProperty(Session("UserNameCheminv"), "Project")
end if
if Not IsEmpty(iJob) then 
	Session("Job")= iJob
else
	Session("Job") = GetUserProperty(Session("UserNameCheminv"), "Job")
end if
if Not IsEmpty(iDeliveryLocationID) then 
	Session("DeliveryLocationID")= iDeliveryLocationID
else
	Session("DeliveryLocationID") = GetUserProperty(Session("UserNameCheminv"), "INVContainerOrderDeliveryLoc")
	' Fall back to Default location user setting
	if Session("DeliveryLocationID") = "" then
	Session("DeliveryLocationID") = GetUserProperty(Session("UserNameCheminv"), "INVDefLoc")
	end if
	' Fall back to application default delivery location
	if Session("DeliveryLocationID") = "" then
		Session("DeliveryLocationID") = Application("DEFAULT_CONTAINER_ORDER_DELIVERY_LOCATION")
	end if
end if
if Not isEmpty(iOrderReason) Then Session("OrderReason") = iOrderReason
if Not isEmpty(iOrderReasonOther) Then Session("OrderReasonOther") = iOrderReasonOther

' Set local variables
DueDate = Session("DueDate")
Project = Session("Project")
Job = Session("Job")
DeliveryLocationID = Session("DeliveryLocationID")
OrderReason = Session("OrderReason")
OrderReasonOther = Session("OrderReasonOther")

%>
