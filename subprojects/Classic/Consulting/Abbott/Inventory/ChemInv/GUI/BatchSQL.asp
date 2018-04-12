<%
schemaName = Application("ORASCHEMANAME")
dateFormatString = Application("DATE_FORMAT_STRING")
strBatchFields = ""
For each Key in reg_fields_dict
	if cStr(lcase(Key)) <> "base64_cdx" then
		strBatchFields = strBatchFields & ", INV_VW_REG_BATCHES." & cStr(uCase(Key)) & " " & vbcrlf
	end if
Next	


if Application("DEFAULT_SAMPLE_REQUEST_CONC") <> "" then
	tmpConc = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
	strConcAbbrv = "'" & tmpConc(1) & "'"
else
	strConcAbbrv = "(select unit_abreviation FROM CHEMINVDB2.inv_containers c, inv_units u where batch_id_fk=batch_id AND c.unit_of_meas_id_fk = u.unit_id(+) And rownum = 1)"	
end if

SQL = "SELECT /*+ ORDERED */ distinct BATCH_ID AS BATCHID" & vbcrlf & _
	"	, RegName as Substance" & vbcrlf & _
	"	, INV_CONTAINER_BATCHES.Container_Status_ID_FK as BatchStatus" & vbcrlf & _
	"	, case when (Select count(Qty_Available) From CHEMINVDB2.inv_containers Where batch_id_fk = batch_id AND container_status_id_fk not in (6,7)) = 0 then 0 else (Select Sum(Qty_Available*concentration) From CHEMINVDB2.inv_containers Where batch_id_fk = batch_id AND container_status_id_fk not in (6,7)) end ||' ' || " & strConcAbbrv & " as AmountRemaining 	" & _
	"	, case when (Select count(Qty_Available) From CHEMINVDB2.inv_containers Where batch_id_fk = batch_id AND container_status_id_fk not in (6,7)) = 0 then 0 else (Select Sum(Qty_Available*concentration) From CHEMINVDB2.inv_containers Where batch_id_fk = batch_id AND container_status_id_fk not in (6,7))-(case when (select Count(qty_required) from inv_requests where batch_id_fk=batch_id and request_status_id_fk=9) = 0 then 0 else (select sum(qty_required) from inv_requests where batch_id_fk=batch_id and request_status_id_fk=9) end) end ||' ' || " & strConcAbbrv & " as AmountAvailable 	" & _
	"	, case when (Select count(Qty_Available) From CHEMINVDB2.inv_containers Where batch_id_fk = batch_id AND container_status_id_fk not in (6,7)) = 0 then 0 else (Select Sum(Qty_Available*concentration) From CHEMINVDB2.inv_containers Where batch_id_fk = batch_id AND container_status_id_fk not in (6,7))-(case when (select Count(qty_required) from inv_requests where batch_id_fk=batch_id and request_status_id_fk=9) = 0 then 0 else (select sum(qty_required) from inv_requests where batch_id_fk=batch_id and request_status_id_fk=9) end) end as AmountAvailableNumber 	" & _
	"   , case when (select Count(qty_required) from inv_requests where batch_id_fk=batch_id and request_status_id_fk=9) = 0 then 0 else (select sum(qty_required) from inv_requests where batch_id_fk=batch_id and request_status_id_fk=9) end ||' ' || " & strConcAbbrv & " as AmountReserved" & _
	"   , (select count(distinct Org_Unit_ID) from inv_org_unit orgunit, inv_org_users orguser where orgunit.org_unit_id = orguser.org_unit_id_fk and orguser.user_id_fk='" & uCase(Session("UserName" & "cheminv")) & "') as NumberOfOrganizations" & _
	"   , (select unit_abreviation FROM CHEMINVDB2.inv_containers c, inv_units u where batch_id_fk=batch_id AND c.unit_of_meas_id_fk = u.unit_id(+) And rownum = 1) as UOMAbbrv " & _
	"	, (select unit_abreviation FROM CHEMINVDB2.inv_containers c, inv_units u where batch_id_fk=batch_id AND c.unit_of_meas_id_fk = u.unit_id(+) And rownum = 1) as UOMAbbrv " & _
	"   , inv_container_batches.field_1" & _
	"   , inv_container_batches.field_2" & _
	"   , inv_container_batches.field_3" & _
	"   , inv_container_batches.field_4" & _
	"   , inv_container_batches.field_5" & _
	"   , inv_container_batches.date_1" & _
	"   , inv_container_batches.date_2" & _
	strBatchFields & _
	"   , INV_CONTAINER_BATCHES.batch_field_1, INV_CONTAINER_BATCHES.batch_field_2, INV_CONTAINER_BATCHES.batch_field_3 " & vbcrlf & _
	"FROM CHEMINVDB2.CSDOHitList " & vbcrlf & _
	"	, CHEMINVDB2.INV_CONTAINER_BATCHES " & vbcrlf & _
	"	, CHEMINVDB2.INV_CONTAINERS " & vbcrlf & _
	"	, CHEMINVDB2.INV_VW_REG_BATCHES " & vbcrlf & _
	"WHERE " & vbcrlf & _
	"batch_id = inv_containers.batch_id_fk " & vbcrlf & _
	"AND inv_containers.reg_id_fk = inv_vw_reg_batches.RegID(+) " & vbcrlf & _
	"AND inv_containers.batch_number_fk = inv_vw_reg_batches.BatchNumber(+) "

%>

