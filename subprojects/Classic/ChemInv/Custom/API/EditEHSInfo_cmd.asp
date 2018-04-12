<%
Sub GetEditEHSInfo_cmd()
	
	' Check for required parameters
	If IsEmpty(ContainerID) then
		strError = strError & "ContainerID is a required parameter<BR>"
		bWriteError = True
	End if

	' Optional parameters
	if EHSGroup1 = "" then EHSGroup1 = NULL
	if EHSGroup2 = "" then EHSGroup2 = NULL
	if EHSGroup3 = "" then EHSGroup3 = NULL
	if EHSHealth = "" then EHSHealth = NULL
	if EHSFlammability = "" then EHSFlammability = NULL
	if EHSReactivity = "" then EHSReactivity = NULL
	if EHSPackingGroup = "" then EHSPackingGroup = NULL
	if EHSUNNumber = "" then EHSUNNumber = NULL
	if EHSACGIHCarcinogenCategory = "" then EHSACGIHCarcinogenCategory = NULL
	if EHSIARCCarcinogen = "" then EHSIARCCarcinogen = NULL
	if EHSEUCarcinogen = "" then EHSEUCarcinogen = NULL
	if EHSIsOSHACarcinogen = "" then EHSIsOSHACarcinogen = 0
	if EHSIsSensitizer = "" then EHSIsSensitizer = 0
	if EHSIsRefrigerated = "" then EHSIsRefrigerated = 0

	If bWriteError then
		' Respond with Error
		Response.Write strError
		Response.end
	End if

	cmd.Parameters.Append Cmd.CreateParameter("theID", 131, 1, , CLng(theID))
	Cmd.Parameters.Append Cmd.CreateParameter("pEHSGroup1",200, 1, 10, Left(EHSGroup1, 10))
	Cmd.Parameters.Append Cmd.CreateParameter("pEHSGroup2",200, 1, 10, Left(EHSGroup2, 10))
	Cmd.Parameters.Append Cmd.CreateParameter("pEHSGroup3",200, 1, 10, Left(EHSGroup3, 10))
	Cmd.Parameters.Append Cmd.CreateParameter("pHealth",131, 1, , EHSHealth)
	Cmd.Parameters.Append Cmd.CreateParameter("pFlammability",131, 1, , EHSFlammability)
	Cmd.Parameters.Append Cmd.CreateParameter("pReactivity",131, 1, , EHSReactivity)
	Cmd.Parameters.Append Cmd.CreateParameter("pIsSensitizer",131, 1, , EHSIsSensitizer)
	Cmd.Parameters.Append Cmd.CreateParameter("pIsRefrigerated",131, 1, , EHSIsRefrigerated)
	Cmd.Parameters.Append Cmd.CreateParameter("pPackingGroup",131, 1, , EHSPackingGroup)
	Cmd.Parameters.Append Cmd.CreateParameter("pUNNumber",131, 1, , EHSUNNumber)
	Cmd.Parameters.Append Cmd.CreateParameter("pIsOSHACarcinogen", 131, 1, , EHSIsOSHACarcinogen)
	Cmd.Parameters.Append Cmd.CreateParameter("pACGIHCarcinogenCategory",200, 1, 2, Left(EHSACGIHCarcinogenCategory, 2))
	Cmd.Parameters.Append Cmd.CreateParameter("pIARCCarcinogen",200, 1, 10, Left(EHSIARCCarcinogen, 10))
	Cmd.Parameters.Append Cmd.CreateParameter("pEUCarcinogen",200, 1, 50, Left(EHSEUCarcinogen, 50))

End Sub

%>