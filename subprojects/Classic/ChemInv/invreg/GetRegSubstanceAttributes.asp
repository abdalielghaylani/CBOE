<%
bDebugPrint = false
' basearray is set in /cfserverasp/source/recordset_vbs.asp
' e.g. BaseID = basearray(0,r)
RSBatchNumber = basearray(1,r)

Set DataConn=GetConnection(dbkey, formgroup, "Structures")
sql =	" Select distinct " &_
	" INV_VW_REG_BATCHES.RegNumber as Reg_Number, " & _
	" INV_VW_REG_BATCHES.BatchNumber as Batch_Number, " &_
	" INV_VW_REG_BATCHES.RegBatchID as RegBatchID, " &_
	" INV_VW_REG_BATCHES.RegID as Reg_ID, " &_
	" INV_VW_REG_BATCHES.RegID as cpd_internal_id, " &_
	" INV_VW_REG_BATCHES.root_reg_date as Registry_Date," &_
	" RegScientist as user_id," &_
	" RegCas as CAS," &_
	" RegName as ChemicalName" &_
	" FROM INV_VW_REG_BATCHES, INV_VW_REG_STRUCTURES" &_
	" WHERE INV_VW_REG_BATCHES.RegID = " & BaseID &_
	" AND INV_VW_REG_BATCHES.BatchNumber = " & RSBatchNumber &_
	" AND INV_VW_REG_BATCHES.REGID=INV_VW_REG_STRUCTURES.REGID"
'Response.Write(sql)		
Set BaseRS=DataConn.Execute(sql)
if Not (BaseRS.BOF=True and BaseRS.EOF=True) then
	BaseRS.MoveFirst
	cpdDBCounter = BaseRS("cpd_internal_id")
	reg_ID = BaseRS("reg_ID")
	regDate = BaseRS("Registry_Date")
	' Get batch_internal_id for use later...	
	baseRegNumber = BaseRS("reg_number")
	batchNumber = BaseRS("Batch_Number")
	RegBatchID = BaseRS("RegBatchID")
	CAS = BaseRS("CAS")
	SubstanceName = BaseRS("ChemicalName")
	UserID = BaseRS("user_id")
	sql2 = "Select BASE64_CDX as Base64_cdx from INV_VW_REG_STRUCTURES WHERE RegID = " & cpdDBCounter 
	Set StructuresRS = DataConn.Execute(sql2)
end if
if bDebugPrint then
	Response.Write "SQL= " & sql & "<br>"
	Response.Write "SQL2= " & sql2 & "<br>"
	Response.Write "Reg_ID= " & Reg_ID & "<br>"
	Response.Write "RegDate= " & RegDate & "<br>"
	Response.Write "RegNumber= " & baseRegNumber & "<br>"
	Response.Write "CAS= " & CAS & "<br>"
	Response.Write "SubstanceName= " & SubstanceName & "<br>"
	Response.Write "Chemist= " & UserID & "<br>"
	Response.Write "Cpd_internal_id= " & cpdDBCounter & "<br><br>"
End if 
%>