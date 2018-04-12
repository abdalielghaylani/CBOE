<%
schemaName = Application("ORASCHEMANAME")
dateFormatString = Application("DATE_FORMAT_STRING")
Ownership=""
if Application("ENABLE_OWNERSHIP")="TRUE" then
Ownership= "Decode(CHEMINVDB2.Authority.PlateIsAuthorized(p.PLATE_ID),NULL,0,CHEMINVDB2.Authority.PlateIsAuthorized(p.PLATE_ID)) as isAuthorised, " &_
		"CHEMINVDB2.AUTHORITY.GETOWNERSHIP(p.PRINCIPAL_ID_FK) AS Ownership_Name, "
end if
' SQL to get all attributes of a container
SQL = "SELECT /*+ ORDERED */ " & vblf & _
    "inv_Containers.Container_ID AS Container_ID,  " & vblf & _
    "inv_Containers.Barcode AS Barcode," & vblf & _
    "inv_Containers.Container_Name AS Container_Name, " & vblf & _
    "inv_Containers.Batch_ID_FK AS Batch_ID_FK, " & vblf & _
    "inv_Containers.BATCH_ID2_FK AS BATCH_ID2_FK, " & vblf & _
    "inv_Containers.BATCH_ID3_FK AS BATCH_ID3_FK, " & vblf & _ 
     "DECODE(inv_Containers.Batch_ID_FK, NULL, ' ', replace(" & schemaName & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING(NULL,Batch_ID_FK),':',' ')) as totalBatchAmt,  " & _
     "DECODE(inv_Containers.BATCH_ID2_FK, NULL, ' ', replace(" & schemaName & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING(NULL,BATCH_ID2_FK),':',' ')) as totalBatch2Amt,  " & _
     "DECODE(inv_Containers.BATCH_ID3_FK, NULL, ' ', replace(" & schemaName & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING(NULL,BATCH_ID3_FK),':',' ')) as totalBatch3Amt,  " & _
    "inv_Container_Types.Container_Type_Name AS Container_Type_Name,  " & vblf & _
    "inv_Container_Status.Container_Status_Name AS Container_Status_Name,  " & vblf & _
    "inv_Containers.Container_Description AS Container_Description, " & vblf & _
    "inv_Containers.Container_Comments AS Container_Comments, " & vblf & _
    "inv_Containers.Storage_Conditions AS Storage_Conditions, " & vblf & _
    "inv_Containers.Handling_Procedures AS Handling_Procedures, " & vblf & _
    "inv_Containers.Family AS Family, " & vblf & _
    "inv_Locations.Location_Name AS Location_Name,  " & vblf & _
    "inv_Locations.Location_Type_ID_FK as Location_Type_ID_FK, " & vblf & _
    "inv_Compounds.Compound_ID AS Compound_ID, " & vblf & _
    "inv_Compounds.ACX_ID AS ACX_ID, " & vblf
    for n = 1 to 5
        strVar = "ALT_ID_" & cstr(n)
        key = "inv_compounds." & strVar
        if alt_ids_dict.exists(key) then
	        if left(alt_ids_dict.item(key),1) = "*" then
                SQL = SQL & "(select picklist_code from inv_picklists where picklist_id = " & strVar & ") as " & strVar & ", " & vblf
            else
                SQL = SQL & strVar & ", " & vblf   
            end if
        else
            SQL = SQL & strVar & ", " & vblf   
        end if
    next
    SQL = SQL & _
    Application("CHEMINV_USERNAME") & ".RESERVATIONS.GETTOTALQTYRESERVED(inv_Containers.Container_ID) as TotalQtyReserved, " & vblf & _
    "TO_CHAR(TRUNC(inv_Containers.DATE_Expires),'" & dateFormatString & "') AS DATE_Expires,  " & vblf & _
    "TO_CHAR(TRUNC(inv_Containers.DATE_Created),'" & dateFormatString & "')  AS DATE_Created,  " & vblf & _
    "TO_CHAR(TRUNC(inv_Containers.DATE_Produced),'" & dateFormatString & "')  AS DATE_Produced, " & vblf & _
    "TO_CHAR(TRUNC(inv_Containers.DATE_Ordered),'" & dateFormatString & "')  AS DATE_Ordered, " & vblf & _
    "TO_CHAR(TRUNC(inv_Containers.DATE_Received),'" & dateFormatString & "')  AS DATE_Received, " & vblf & _
    "inv_Containers.Parent_container_id_fk as Parent_container_id, " & vblf & _
    "(SELECT c2.barcode from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk) as Parent_container_barcode, " & vblf & _
    "case when (select count((select e.location_id_fk from inv_grid_element e, inv_grid_storage s, inv_locations p where e.grid_storage_id_fk = s.grid_storage_id and p.location_id = s.location_id_fk and p.collapse_child_nodes = 1  and e.location_id_fk = (SELECT c2.location_id_fk  from inv_Containers c2  where c2.container_id = inv_Containers.Parent_container_id_fk)))  from dual) = 0 then  (SELECT c2.location_id_fk from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk) else (select p.location_id from inv_grid_element e, inv_grid_storage s, inv_locations p where e.grid_storage_id_fk = s.grid_storage_id  and p.location_id = s.location_id_fk  and p.collapse_child_nodes = 1 and e.location_id_fk = (SELECT c2.location_id_fk from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk)) end as Parent_container_Location_ID, " &_

    "DECODE(inv_Containers.Qty_Max, NULL, ' ', inv_Containers.Qty_Max||' '||UOM.Unit_Abreviation) AS ContainerSize ,  " & vblf & _
    "DECODE(inv_Containers.Qty_Remaining, NULL, ' ', inv_Containers.Qty_Remaining||' '||UOM.Unit_Abreviation) AS AmountRemaining ,  " & vblf & _
    "DECODE(inv_Containers.Qty_Available, NULL, ' ', inv_Containers.Qty_Available||' '||UOM.Unit_Abreviation) AS AmountAvailable ,  " & vblf & _
    "DECODE(inv_Containers.Concentration, NULL, ' ', inv_Containers.Concentration||' '||UOC.Unit_Abreviation) AS Concentration_Text, " & vblf & _
    "DECODE(inv_Containers.Density, NULL, ' ', inv_Containers.Density||' '||UOD.Unit_Abreviation) AS Density_Text, " & vblf & _
    "DECODE(inv_Containers.Purity, NULL, ' ', inv_Containers.Purity||' '||UOP.Unit_Abreviation) AS Purity_Text, " & vblf & _
	"DECODE(inv_Containers.Net_Wght, NULL, ' ', inv_Containers.Net_Wght||' '||UOW.Unit_Abreviation) AS Weight_Text, " & vblf & _
	"DECODE(inv_Containers.Container_Cost, NULL, ' ', UOCost.Unit_Abreviation||TO_CHAR(inv_Containers.Container_Cost,'999,999.99')) AS ContainerCost,  " & vblf & _

    "inv_Containers.Concentration AS Concentration, " & vblf & _
    "inv_Containers.Density AS Density, " & vblf & _
    "inv_Containers.Purity AS Purity, " & vblf & _
    "inv_Containers.Grade AS Grade, " & vblf & _
    "inv_Containers.Solvent_ID_FK," & vblf & _
	"DECODE(inv_Containers.Solvent_ID_FK, NULL, ' ', inv_Solvents.Solvent_Name) AS Solvent_Name,  " & vblf & _
    "inv_Containers.Lot_Num AS Lot_Num, " & vblf & _
    "inv_Containers.Supplier_CATNUM AS Supplier_CATNUM, " & vblf & _
    "inv_Suppliers.Supplier_Name AS Supplier_Name, " & vblf & _
    "inv_Suppliers.Supplier_Short_Name AS Supplier_Short_Name, " & vblf & _
    "inv_Containers.PO_Number AS PO_Number, " & vblf & _
    "inv_Containers.PO_Line_Number AS PO_Line_Number, " & vblf & _
    "inv_Containers.Req_Number AS Req_Number, " & vblf & _
    "inv_Address.Contact_Name AS ContactName, " & vblf & _
    "inv_Address.Address1 AS Address1, " & vblf & _
    "inv_Address.Address2 AS Address2, " & vblf & _
    "inv_Address.Address3 AS Address3, " & vblf & _
    "inv_Address.Address4 AS Address4, " & vblf & _
    "inv_Address.City AS , " & vblf & _
    "inv_States.State_Name AS State_Name, " & vblf & _
    "inv_Country.Country_Name AS Country_Name, " & vblf & _
    "inv_Address.ZIP AS ZIP, " & vblf & _
    "inv_Address.FAX AS FAX, " & vblf & _
    "inv_Address.Phone AS Phone, " & vblf & _
    "inv_Address.Email AS Email, " & vblf & _
    "inv_Containers.Qty_MinStock AS Qty_MinStock, " & vblf & _
    "inv_Containers.Qty_MaxStock AS Qty_MaxStock, " & vblf & _
    "DECODE(inv_Containers.Qty_MinStock, NULL, ' ', inv_Containers.Qty_MinStock||' '||UOM.Unit_Abreviation) AS Qty_MinStock_String ,  " & vblf & _
    "DECODE(inv_Containers.Qty_MaxStock, NULL, ' ', inv_Containers.Qty_MaxStock||' '||UOM.Unit_Abreviation) AS Qty_MaxStock_String ,  " & vblf & _
    "inv_compounds.reg_id_fk,  " & vblf & _
    "inv_compounds.batch_number_fk,  " 

    if Application("RegServerName") <> "NULL" then        
        SQL = SQL & "DECODE(" &_
            "inv_compounds.reg_id_fk, " &_
            "NULL, " &_
            "inv_compounds.Substance_Name, " &_
            "(select REGNAME from cheminvdb2.inv_vw_reg_batches Where regid = inv_compounds.reg_id_fk AND batchnumber = inv_compounds.batch_number_fk)) " &_
            "AS Substance_Name,"
        
        SQL = SQL & "DECODE(" &_
            "inv_compounds.reg_id_fk, " &_
            "NULL, " &_
            "inv_compounds.CAS, " &_
            "(select REGCAS from cheminvdb2.inv_vw_reg_batches Where regid = inv_compounds.reg_id_fk AND batchnumber = inv_compounds.batch_number_fk)) " &_
            "AS CAS,"
                
		SQL = SQL & "DECODE(" &_
		    "inv_compounds.reg_id_fk, " &_
		    "NULL, " &_
		    "NULL, " &_
		    "(select regnumber from cheminvdb2.inv_vw_reg_batches Where regid = inv_compounds.reg_id_fk AND batchnumber = inv_compounds.batch_number_fk)) " &_ 
		    "AS RegNumber, "
		
		SQL = SQL & "DECODE(" &_
		    "inv_compounds.reg_id_fk, " &_
		    "NULL, " &_
		    "NULL, " &_
		    "(select regbatchid from cheminvdb2.inv_vw_reg_batches Where regid = inv_compounds.reg_id_fk AND batchnumber = inv_compounds.batch_number_fk)) " &_
		    "AS Reg_Batch_ID, "
		    
		SQL = SQL & "DECODE(" &_
		    "inv_compounds.reg_id_fk, " &_
		    "NULL, " &_
		    "inv_compounds.BASE64_CDX, " &_
		    "(select base64_cdx From cheminvdb2.inv_vw_reg_structures Where inv_vw_reg_structures.regid = inv_compounds.reg_id_fk)) " &_
		    "AS Base64_CDX, "
		
		'-- get MW and Formula from the cartridge
		if Application("GetCartridgeMwFormula") = "TRUE" then
			SQL = SQL & "cscartridge.molweight(DECODE(" &_
			    "inv_compounds.reg_id_fk, " &_
			    "NULL, " &_
			    "inv_compounds.base64_cdx, " &_
			    "(select base64_cdx From cheminvdb2.inv_vw_reg_structures Where inv_vw_reg_structures.regid = inv_compounds.reg_id_fk))) " &_
			    "as MW, "
			
			SQL = SQL & "cscartridge.formula(DECODE(" &_
			    "inv_compounds.reg_id_fk, " &_
			    "NULL, " &_
			    "inv_compounds.base64_cdx, " &_
			    "(select base64_cdx From cheminvdb2.inv_vw_reg_structures Where inv_vw_reg_structures.regid = inv_compounds.reg_id_fk)), '') " &_
			    "as formula, "
		end if
	else	    
	    SQL = SQL & "inv_compounds.Substance_Name AS Substance_Name, " & vblf 
	    SQL = SQL & "inv_compounds.CAS AS CAS, " & vblf
		SQL = SQL & "inv_compounds.BASE64_CDX AS BASE64_CDX, " & vblf
		
		'-- get MW and Formula from the cartridge
		if Application("GetCartridgeMwFormula") = "TRUE" then
			SQL = SQL & "cscartridge.molweight(inv_compounds.base64_cdx) as MW, "
			SQL = SQL & "cscartridge.formula(inv_compounds.base64_cdx, '') as formula, "
		end if
    End if

    SQL = SQL & "inv_Containers.Location_ID_FK,  " & vblf & _
    "inv_Containers.Compound_ID_FK,  " & vblf & _
    "inv_Containers.Container_Type_ID_FK,  " & vblf & _
    "inv_Containers.Container_Status_ID_FK,  " & vblf & _
    "inv_Containers.Supplier_ID_FK, " & vblf & _
    "inv_Containers.Owner_ID_FK, " & vblf & _
    "inv_Containers.Current_User_ID_FK, " & vblf & _

    "inv_Containers.Qty_Initial AS Qty_Initial,  " & vblf & _
    "inv_Containers.Qty_Max AS Qty_Max,  " & vblf & _
    "inv_Containers.Qty_Remaining AS Qty_Remaining,  " & vblf & _
    "inv_Containers.Qty_Available AS Qty_Available,  " & vblf & _

    "UOM.Unit_ID AS UOMID,  " & vblf & _
    "UOP.Unit_ID AS UOPID,  " & vblf & _
    "UOC.Unit_ID AS UOCID,  " & vblf & _
    "UOD.Unit_ID AS UODID,  " & vblf & _
    "UOW.Unit_ID AS UOWID,  " & vblf & _
    "UOCost.Unit_ID AS UOCostID,  " & vblf & _
    "UOM.Unit_Name AS UOMName,  " & vblf & _
    "UOP.Unit_Name AS UOPName,  " & vblf & _
    "UOC.Unit_Name AS UOCName,  " & vblf & _
    "UOD.Unit_Name AS UODName,  " & vblf & _
    "UOW.Unit_Name AS UOWName,  " & vblf & _
    "UOCost.Unit_Name AS UOCostName,  " & vblf & _
    "UOM.Unit_Abreviation AS UOMAbv,  " & vblf & _
    "UOP.Unit_Abreviation AS UOPAbv,  " & vblf & _
    "UOC.Unit_Abreviation AS UOCAbv,  " & vblf & _
    "UOD.Unit_Abreviation AS UODAbv,  " & vblf & _
    "UOW.Unit_Abreviation AS UOWAbv,  " & vblf & _
    "UOCost.Unit_Abreviation AS UOCostAbv,  " & vblf & _

    "inv_Containers.Tare_Weight AS Tare_Weight,  " & vblf & _
    "inv_Containers.Final_Wght AS Final_Weight,  " & vblf & _
    "inv_Containers.Net_Wght AS Net_Weight,  " & vblf & _
    "TO_CHAR(inv_Containers.Container_Cost, '999,999.99') AS Container_Cost, " & vblf & _
	"TO_CHAR(TRUNC(inv_containers.Date_Certified),'" & dateFormatString & "') AS Date_Certified, " &_
	"TO_CHAR(TRUNC(inv_containers.Date_Approved),'" & dateFormatString & "') AS Date_Approved, " &_
	"inv_containers.Field_1, " &_
	"inv_containers.Field_2, " &_
	"inv_containers.Field_3, " &_
	"inv_containers.Field_4, " &_
	"inv_containers.Field_5, " &_
	"inv_containers.Field_6, " &_
	"inv_containers.Field_7, " &_
	"inv_containers.Field_8, " &_
	"inv_containers.Field_9, " &_
	"inv_containers.Field_10, " &_
	"TO_CHAR(TRUNC(inv_containers.Date_1),'" & dateFormatString & "') AS Date_1, " &_
	"TO_CHAR(TRUNC(inv_containers.Date_2),'" & dateFormatString & "') AS Date_2, " &_
	"TO_CHAR(TRUNC(inv_containers.Date_3),'" & dateFormatString & "') AS Date_3, " &_
	"TO_CHAR(TRUNC(inv_containers.Date_4),'" & dateFormatString & "') AS Date_4, " &_
	"TO_CHAR(TRUNC(inv_containers.Date_5),'" & dateFormatString & "') AS Date_5, " &_
	"inv_containers.Creator AS Creator, "
	if Application("ENABLE_OWNERSHIP")="TRUE" then
    SQL = SQL & "Decode(CHEMINVDB2.Authority.ContainerIsAuthorized(inv_Containers.Container_ID),NULL,0,CHEMINVDB2.Authority.ContainerIsAuthorized(inv_Containers.Container_ID)) as isAuthorised," &_
	"inv_Containers.PRINCIPAL_ID_FK AS OwnershipType, " &_
	"inv_Containers.LOCATION_TYPE_ID_FK AS LocationType, " &_
	"CHEMINVDB2.AUTHORITY.GetLocationtype(inv_Containers.LOCATION_TYPE_ID_FK) AS Location_Description, " &_
	"CHEMINVDB2.AUTHORITY.GETOWNERSHIP(inv_Containers.PRINCIPAL_ID_FK) AS Ownership_Name, "
	end if
	SQL = SQL & "CHEMINVDB2.GETLOCATIONPATH(inv_Locations.location_id) AS Location_Path " &_
"FROM " & vblf & _
    "inv_Containers , inv_Locations, inv_Compounds, inv_Suppliers, inv_Units UOM, inv_Units UOP, inv_Units UOC, inv_Units UOD, inv_Units UOW, inv_Container_Types, inv_container_status, inv_solvents, inv_address, inv_states, inv_country, inv_units UOCost " & vblf
if Application("RegServerName") <> "NULL" then
	SQL = SQL & ", inv_vw_reg_batches ivrb, inv_vw_reg_structures ivrs " & _
		"WHERE inv_compounds.reg_id_fk = ivrb.RegID(+) " & _
		"		AND	inv_compounds.batch_number_fk = ivrb.BatchNumber(+) "  & _
		"		AND	ivrb.RegID = ivrs.RegID(+) AND "
else
	SQL = SQL & "WHERE  " & vblf
end if
SQL = SQL & "inv_Containers.Location_ID_FK = inv_Locations.Location_ID  " & vblf & _
"AND " & vblf & _
  "inv_Containers.UNIT_OF_MEAS_ID_FK = UOM.Unit_ID " & vblf & _
"AND " & vblf & _
  "inv_Containers.Unit_of_Purity_ID_FK = UOP.Unit_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Containers.Unit_of_Conc_ID_FK = UOC.Unit_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Containers.Unit_of_Density_ID_FK = UOD.Unit_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Containers.Unit_of_wght_ID_FK = UOW.Unit_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Containers.Unit_of_Cost_ID_FK = UOCost.Unit_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Suppliers.Supplier_Address_ID_FK = inv_Address.Address_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Address.State_ID_FK = inv_states.State_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Address.Country_ID_FK = inv_country.Country_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Containers.Container_Status_ID_FK = inv_Container_Status.Container_Status_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Containers.Container_Type_ID_FK = inv_Container_Types.Container_Type_ID " & vblf & _
"AND " & vblf & _
  "inv_Containers.Compound_ID_FK = inv_Compounds.Compound_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Containers.Supplier_ID_FK = inv_Suppliers.Supplier_ID(+) " & vblf & _
"AND " & vblf & _
  "inv_Containers.Solvent_ID_FK = inv_Solvents.Solvent_ID(+) " &vblf

'"DECODE(inv_requests.date_delivered, NULL, inv_requests.request_id, NULL) AS request_id, " &_
'"AND " & vblf & _
'  "inv_Containers.Container_ID = inv_requests.container_ID_fk(+) "

%>
