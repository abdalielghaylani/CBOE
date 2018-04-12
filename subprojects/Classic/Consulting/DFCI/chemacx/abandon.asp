<%
response.write application("banana") & "<BR>"
response.write application("banana2")
response.end
CSWebUsersDBConnStr = "Provider= SQLOLEDB; Data Source=172.17.1.18; Initial Catalog=csWebUsers; User Id=cows2000; Password=cambridgesoft; network=dbmssocn"

gServiceID = 10  '  ServiceID 10 is ChemACX Pro

response.expires = -1
'session.abandon

 result =  ChangeConcurrentUserCounter(CSWebUsersDBConnStr, Request.Cookies("SubscriptionID"),gServiceID, -1)
response.write "<BR>"& result

'**********************************************************************************************
'This function decrements the concurrent user counter for a given subscriptionID/serviceID
function ChangeConcurrentUserCounter(ConnStr, SubscriptionID, ServiceID, value)
response.write "servid= " & serviceID & " subid= " & subscriptionID & " value= " & value
	Set cmd = Server.CreateObject("ADODB.Command")
	With cmd
		.ActiveConnection = ConnStr
		.CommandText = "CS_ChangeConcurrentUserCounter"
		
		AdCmdStoredProc = 4
		.CommandType = AdCmdStoredProc
		
		'ReplaceParameters
		adInteger = 3
		adVarChar = 200
		adParamInput = 1
		adParamOutput = 2
		adParamReturnValue = 4
		
		.Parameters.Append .CreateParameter ("RETURN_VALUE", adInteger, adParamReturnValue)
		.Parameters.Append .CreateParameter ("@SubscriptionID", adInteger, adParamInput)	
		.Parameters.Append .CreateParameter ("@ServiceID", adInteger, adParamInput)
		.Parameters.Append .CreateParameter ("@value", adInteger, adParamInput)
		.Parameters.Append .CreateParameter ("@CurrentUsers", adInteger, adParamOutput)
		
		.Parameters("@SubscriptionID") = SubscriptionID 
		.Parameters("@ServiceID") = ServiceID
		.Parameters("@value") = value
		response.write "<BR>" & value & "<br>"
		adExecuteNoRecords = 128
		.Execute lngRecs, adExecuteNoRecords
		
		ChangeConcurrentUserCounter =  .Parameters("@CurrentUsers")
	End With
End function

%>  