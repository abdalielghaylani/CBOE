<%
	Response.Expires = -1
	Set Cmd = Nothing
	Set Conn = Nothing

	Dim strError
	Dim PrintDebug
	Dim spName
	Dim theID
	
	
	bDebugPrint = false
	bWriteError = False
	strError = "Error:ShowEHS:<BR>"
	
	spName = Application("CHEMINV_USERNAME") & "." 
	
	
	if ContainerID <> "" then
		spName = spName & "SELECTHAZMATDATA"
		theID = ContainerID
	elseif compoundID <> ""  then
		spName = spName & "SELECTSUBSTANCEHAZMATDATA"
		theID = CompoundID
		CAS = Request("CAS")
		SubstanceName = Request("SN")
	else
		Response.Write stError & "Either CompoundID or ContainerID is required." 
		Response.End
	End if
	
	' Set up and ADO command
	Call GetInvConnection
	Call GetInvCommand(spName, adCmdStoredProc)

	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 131, 4, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("theID", 131, 1, 0, theID)
	Cmd.Parameters.Append Cmd.CreateParameter("PEHSGROUP1", 200, 2, 10, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PEHSGROUP2", 200, 2, 10, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PEHSGROUP3", 200, 2, 10, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PHEALTH", 131, 2, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PFLAMMABILITY", 131, 2, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PREACTIVITY", 131, 2, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PISSENSITIZER", 131, 2, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PISREFRIGERATED", 131, 2, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PPACKINGGROUP", 131, 2, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PUNNUMBER", 131, 2, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PISOSHACARCINOGEN", 131, 2, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PACGIHCARCINOGENCATEGORY", 200, 2, 2, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PIARCCARCINOGEN", 200, 2, 10, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PEUCARCINOGEN", 200, 2, 50, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PISDEFAULTSOURCE", 131, 2, 0, NULL)

	if bDebugPrint then
		Response.Write spName & "<BR>"
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
		Response.End	
	Else
		Call ExecuteCmd(spName)
	End if
	
	Session("EHSFoundData") = Cmd.Parameters("RETURN_VALUE")
	Session("EHSGroup1") = LTrim(RTrim(Cmd.Parameters("pEHSGroup1")))
	Session("EHSGroup2") = LTrim(RTrim(Cmd.Parameters("pEHSGroup2")))
	Session("EHSGroup3") = LTrim(RTrim(Cmd.Parameters("pEHSGroup3")))
	Session("EHSHealth") = Cmd.Parameters("pHealth")
	Session("EHSFlammability") = Cmd.Parameters("pFlammability")
	Session("EHSReactivity") = Cmd.Parameters("pReactivity")
	Session("EHSIsSensitizer") = Cmd.Parameters("pIsSensitizer")
	If IsNull(Session("EHSIsSensitizer")) Then
		Session("EHSIsSensitizer") = "0"
	End If
	Session("EHSIsRefrigerated") = Cmd.Parameters("pIsRefrigerated")
	If IsNull(Session("EHSIsRefrigerated")) Then
		Session("EHSIsRefrigerated") = "0"
	End If
	Session("EHSPackingGroup") = Cmd.Parameters("pPackingGroup")
	Session("EHSUNNumber") = Cmd.Parameters("pUNNumber")
	Session("EHSIsOSHACarcinogen") = Cmd.Parameters("pIsOSHACarcinogen")
	If IsNull(Session("EHSIsOSHACarcinogen")) Then
		Session("EHSIsOSHACarcinogen") = "0"
	End If
	Session("EHSACGIHCarcinogenCategory") = LTrim(RTrim(Cmd.Parameters("pACGIHCarcinogenCategory")))
	Session("EHSIARCCarcinogen") = LTrim(RTrim(Cmd.Parameters("pIARCCarcinogen")))
	Session("EHSEUCarcinogen") = LTrim(RTrim(Cmd.Parameters("pEUCarcinogen")))
	Session("EHSIsDefaultSource") = Cmd.Parameters("pIsDefaultSource")
	Conn.Close
	Set Cmd = Nothing
	Set Conn = Nothing

	EHSFoundData = Session("EHSFoundData")
	EHSGroup1 = Session("EHSGroup1")
	EHSGroup2 = Session("EHSGroup2")
	EHSGroup3 = Session("EHSGroup3")
	EHSHealth = Session("EHSHealth")
	EHSFlammability = Session("EHSFlammability")
	EHSReactivity = Session("EHSReactivity")
	EHSIsSensitizer = Session("EHSIsSensitizer")
	EHSIsRefrigerated = Session("EHSIsRefrigerated")
	EHSPackingGroup = Session("EHSPackingGroup")
	EHSUNNumber = Session("EHSUNNumber")
	EHSIsOSHACarcinogen = Session("EHSIsOSHACarcinogen")
	EHSACGIHCarcinogenCategory = Session("EHSACGIHCarcinogenCategory")
	EHSIARCCarcinogen = Session("EHSIARCCarcinogen")
	EHSEUCarcinogen = Session("EHSEUCarcinogen")
	EHSIsDefaultSource = Session("EHSIsDefaultSource")
%>

