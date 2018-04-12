<%
bDebugPrint = false

Set DataConn=GetConnection(dbkey, formgroup, "Structures")
sql =	" SELECT" &_ 
		" Reg_Number," & _   
		" Reg_ID," & _  
		" Reg_Numbers.cpd_internal_id," & _ 
		" Registry_Date," & _
		" people.user_id," & _ 
		"(SELECT alt_ids.identifier FROM alt_ids WHERE alt_ids.identifier_type = 1 AND reg_internal_id = reg_numbers.reg_id AND ROWNUM = 1) AS CAS," & _
		"(SELECT alt_ids.identifier FROM alt_ids WHERE alt_ids.identifier_type = 0 AND reg_internal_id = reg_numbers.reg_id AND ROWNUM = 1) AS ChemicalName" & _
		" FROM Reg_Numbers, people" & _
		" WHERE reg_numbers.registrar_person_id = people.person_id(+)" & _
		" AND Reg_ID =" & BaseID		
Set BaseRS=DataConn.Execute(sql)
if Not (BaseRS.BOF=True and BaseRS.EOF=True) then
	BaseRS.MoveFirst
	cpdDBCounter = BaseRS("cpd_internal_id")
	reg_ID = BaseRS("reg_ID")
	regDate = BaseRS("Registry_Date")
	' Get batch_internal_id for use later...
	baseRegNumber = BaseRS("reg_number")
	CAS = BaseRS("CAS")
	SubstanceName = BaseRS("ChemicalName")
	UserID = BaseRS("user_id")
	sql2 = "Select Base64_cdx from Structures WHERE structures.cpd_internal_id = " & cpdDBCounter
	Set StructuresRS = DataConn.Execute(sql2)
end if
if bDebugPrint then
	Response.Write "SQL= " & sql & "<BR>"
	Response.Write "SQL2= " & sql2 & "<BR>"
	Response.Write "Reg_ID= " & Reg_ID & "<BR>"
	Response.Write "RegDate= " & RegDate & "<BR>"
	Response.Write "RegNumber= " & baseRegNumber & "<BR>"
	Response.Write "CAS= " & CAS & "<BR>"
	Response.Write "SubstanceName= " & SubstanceName & "<BR>"
	Response.Write "Chemist= " & UserID & "<BR>"
	Response.Write "Cpd_internal_id= " & cpdDBCounter & "<BR><BR>"
End if 
%>