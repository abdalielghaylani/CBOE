CREATE OR REPLACE PACKAGE BODY "GUIUTILS" AS

	PROCEDURE GETKEYCONTAINERATTRIBUTES(pContainerIDList IN VARCHAR2 := NULL,
								 pContainerBarcodeList IN VARCHAR2 := NULL,
								 O_RS OUT CURSOR_TYPE) IS
		my_sql VARCHAR2(2000);
	BEGIN
		my_sql := 'SELECT c.container_id,
	                  c.barcode,
	                  c.container_name,
	                  l.location_name,
	                  p.user_id,
	                  u.unit_abreviation,
	                  c.qty_remaining,
	                  GetLocationPath(l.location_id) AS Path,
				c.container_status_id_fk
	           FROM inv_containers c, inv_locations l, &&securitySchemaName..people p, inv_units u
	           WHERE c.location_id_fk = l.location_id
	           AND c.unit_of_meas_id_fk = u.unit_id
	           AND upper(c.current_user_id_fk) = p.user_id(+)';
		IF pContainerIDList IS NOT NULL THEN
			my_sql := my_sql || 'AND Container_ID IN(' || pContainerIDList || ')';
		ELSE
			my_sql := my_sql || 'AND Barcode IN(' || pContainerBarcodeList || ')';
		END IF;
		my_sql := my_sql || ' ORDER BY l.location_name ASC';
		OPEN O_RS FOR my_sql;
	END GETKEYCONTAINERATTRIBUTES;

	PROCEDURE GETCONTAINERATTRIBUTES(pContainerID IN inv_containers.container_id%TYPE := NULL,
								 pContainerBarcode IN inv_containers.barcode%TYPE := NULL,
								 pUseReg IN NUMBER,
								 O_RS OUT CURSOR_TYPE) IS
		lContainerID inv_containers.container_id%TYPE;
	BEGIN
		IF pContainerBarcode IS NOT NULL THEN
			BEGIN
				SELECT inv_containers.container_id into lContainerID
				FROM inv_containers
				WHERE inv_containers.barcode = pContainerBarcode;
			EXCEPTION
				WHEN NO_DATA_FOUND THEN lContainerID := 0;
			END;
		ELSE
			SELECT pContainerID into lContainerID FROM dual;
		END IF;

		IF pUseReg = 1 THEN
			OPEN O_RS FOR SELECT /*+ ORDERED */ 
			inv_Containers.Container_ID AS Container_ID,  
			inv_Containers.Barcode AS Barcode,
			inv_Containers.Container_Name AS Container_Name, 
			inv_Containers.Batch_ID_FK AS Batch_ID_FK, 
			DECODE(inv_Containers.Batch_ID_FK, NULL, ' ', replace(CHEMINVDB2.GUIUTILS.GETBATCHUOMAMOUNTSTRING(container_ID,NULL),':',' ')) as totalBatchAmt,  
			inv_Container_Types.Container_Type_Name AS Container_Type_Name,  
			inv_Container_Status.Container_Status_Name AS Container_Status_Name,  
			inv_Containers.Container_Description AS Container_Description, 
			inv_Containers.Container_Comments AS Container_Comments, 
			inv_Containers.Storage_Conditions AS Storage_Conditions, 
			inv_Containers.Handling_Procedures AS Handling_Procedures, 
			inv_Containers.Family AS Family, 
			inv_Locations.Location_Name AS Location_Name,  
			inv_Locations.Location_Type_ID_FK as Location_Type_ID_FK, 
			inv_Compounds.Compound_ID AS Compound_ID, 
			inv_Compounds.ACX_ID AS ACX_ID, 
			ALT_ID_1, 
			ALT_ID_2, 
			ALT_ID_3, 
			ALT_ID_4, 
			ALT_ID_5, 
			CHEMINVDB2.RESERVATIONS.GETTOTALQTYRESERVED(inv_Containers.Container_ID) as TotalQtyReserved, 
			TO_CHAR(TRUNC(inv_Containers.DATE_Expires),'MM/DD/YYYY') AS DATE_Expires,  
			TO_CHAR(TRUNC(inv_Containers.DATE_Created),'MM/DD/YYYY')  AS DATE_Created,  
			TO_CHAR(TRUNC(inv_Containers.DATE_Produced),'MM/DD/YYYY')  AS DATE_Produced, 
			TO_CHAR(TRUNC(inv_Containers.DATE_Ordered),'MM/DD/YYYY')  AS DATE_Ordered, 
			TO_CHAR(TRUNC(inv_Containers.DATE_Received),'MM/DD/YYYY')  AS DATE_Received, 
			inv_Containers.Parent_container_id_fk as Parent_container_id, 
			(SELECT c2.barcode from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk) as Parent_container_barcode, 
			case when (select count((select e.location_id_fk from inv_grid_element e, inv_grid_storage s, inv_locations p where e.grid_storage_id_fk = s.grid_storage_id and p.location_id = s.location_id_fk and p.collapse_child_nodes = 1  and e.location_id_fk = (SELECT c2.location_id_fk  from inv_Containers c2  where c2.container_id = inv_Containers.Parent_container_id_fk)))  from dual) = 0 then  (SELECT c2.location_id_fk from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk) else (select p.location_id from inv_grid_element e, inv_grid_storage s, inv_locations p where e.grid_storage_id_fk = s.grid_storage_id  and p.location_id = s.location_id_fk  and p.collapse_child_nodes = 1 and e.location_id_fk = (SELECT c2.location_id_fk from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk)) end as Parent_container_Location_ID, DECODE(inv_Containers.Qty_Max, NULL, ' ', inv_Containers.Qty_Max||' '||UOM.Unit_Abreviation) AS ContainerSize ,  
			DECODE(inv_Containers.Qty_Remaining, NULL, ' ', inv_Containers.Qty_Remaining||' '||UOM.Unit_Abreviation) AS AmountRemaining ,  
			DECODE(inv_Containers.Qty_Available, NULL, ' ', inv_Containers.Qty_Available||' '||UOM.Unit_Abreviation) AS AmountAvailable ,  
			DECODE(inv_Containers.Concentration, NULL, ' ', inv_Containers.Concentration||' '||UOC.Unit_Abreviation) AS Concentration_Text, 
			DECODE(inv_Containers.Density, NULL, ' ', inv_Containers.Density||' '||UOD.Unit_Abreviation) AS Density_Text, 
			DECODE(inv_Containers.Purity, NULL, ' ', inv_Containers.Purity||' '||UOP.Unit_Abreviation) AS Purity_Text, 
			DECODE(inv_Containers.Net_Wght, NULL, ' ', inv_Containers.Net_Wght||' '||UOW.Unit_Abreviation) AS Weight_Text, 
			DECODE(inv_Containers.Container_Cost, NULL, ' ', UOCost.Unit_Abreviation||TO_CHAR(inv_Containers.Container_Cost,'999,999.99')) AS ContainerCost,  
			inv_Containers.Concentration AS Concentration, 
			inv_Containers.Density AS Density, 
			inv_Containers.Purity AS Purity, 
			inv_Containers.Grade AS Grade, 
			inv_Containers.Solvent_ID_FK,
			DECODE(inv_Containers.Solvent_ID_FK, NULL, ' ', inv_Solvents.Solvent_Name) AS Solvent_Name,  
			inv_Containers.Lot_Num AS Lot_Num, 
			inv_Containers.Supplier_CATNUM AS Supplier_CATNUM, 
			inv_Suppliers.Supplier_Name AS Supplier_Name, 
			inv_Suppliers.Supplier_Short_Name AS Supplier_Short_Name, 
			inv_Containers.PO_Number AS PO_Number, 
			inv_Containers.PO_Line_Number AS PO_Line_Number, 
			inv_Containers.Req_Number AS Req_Number, 
			inv_Address.Contact_Name AS ContactName, 
			inv_Address.Address1 AS Address1, 
			inv_Address.Address2 AS Address2, 
			inv_Address.Address3 AS Address3, 
			inv_Address.Address4 AS Address4, 
			inv_Address.City AS , 
			inv_States.State_Name AS State_Name, 
			inv_Country.Country_Name AS Country_Name, 
			inv_Address.ZIP AS ZIP, 
			inv_Address.FAX AS FAX, 
			inv_Address.Phone AS Phone, 
			inv_Address.Email AS Email, 
			inv_Containers.Qty_MinStock AS Qty_MinStock, 
			inv_Containers.Qty_MaxStock AS Qty_MaxStock, 
			DECODE(inv_Containers.Qty_MinStock, NULL, ' ', inv_Containers.Qty_MinStock||' '||UOM.Unit_Abreviation) AS Qty_MinStock_String ,  
			DECODE(inv_Containers.Qty_MaxStock, NULL, ' ', inv_Containers.Qty_MaxStock||' '||UOM.Unit_Abreviation) AS Qty_MaxStock_String ,  
			inv_compounds.Reg_ID_FK,  
			inv_compounds.Batch_Number_FK,
			DECODE(inv_compounds.Reg_ID_FK,
				NULL,
				inv_Compounds.Substance_Name,
				(Select REGNAME from cheminvdb2.inv_vw_reg_batches Where regid = inv_compounds.reg_id_fk AND batchnumber= inv_compounds.Batch_Number_FK))
				AS Substance_Name,
			DECODE(inv_compounds.Reg_ID_FK,
				NULL,
				inv_Compounds.CAS,
				(Select REGCAS from cheminvdb2.inv_vw_reg_batches Where regid = inv_compounds.reg_id_fk AND batchnumber= inv_compounds.Batch_Number_FK))
				AS CAS,
			DECODE(inv_compounds.Reg_ID_FK,
				NULL, 
				NULL, 
				(Select regnumber from cheminvdb2.inv_vw_reg_batches Where regid = inv_compounds.reg_id_fk AND batchnumber= inv_compounds.Batch_Number_FK))
				AS RegNumber, 
			DECODE(inv_compounds.Reg_ID_FK, 
				NULL, 
				NULL, 
				(Select regbatchid from cheminvdb2.inv_vw_reg_batches Where regid = inv_compounds.reg_id_fk AND batchnumber= inv_compounds.Batch_Number_FK)) 
				AS Reg_Batch_ID, 
			DECODE(inv_compounds.Reg_ID_FK, 
				NULL, 
				inv_Compounds.BASE64_CDX, 
				(Select base64_cdx From cheminvdb2.inv_vw_reg_structures Where inv_vw_reg_structures.regid=inv_compounds.reg_id_fk))
				AS Base64_CDX,
			inv_Containers.Location_ID_FK,  
			inv_Containers.Compound_ID_FK,  
			inv_Containers.Container_Type_ID_FK,  
			inv_Containers.Container_Status_ID_FK,  
			inv_Containers.Supplier_ID_FK, 
			inv_Containers.Owner_ID_FK, 
			inv_Containers.Current_User_ID_FK, 
			inv_Containers.Qty_Initial AS Qty_Initial,  
			inv_Containers.Qty_Max AS Qty_Max,  
			inv_Containers.Qty_Remaining AS Qty_Remaining,  
			inv_Containers.Qty_Available AS Qty_Available,  
			UOM.Unit_ID AS UOMID,  
			UOP.Unit_ID AS UOPID,  
			UOC.Unit_ID AS UOCID,  
			UOD.Unit_ID AS UODID,  
			UOW.Unit_ID AS UOWID,  
			UOCost.Unit_ID AS UOCostID,  
			UOM.Unit_Name AS UOMName,  
			UOP.Unit_Name AS UOPName,  
			UOC.Unit_Name AS UOCName,  
			UOD.Unit_Name AS UODName,  
			UOW.Unit_Name AS UOWName,  
			UOCost.Unit_Name AS UOCostName,  
			UOM.Unit_Abreviation AS UOMAbv,  
			UOP.Unit_Abreviation AS UOPAbv,  
			UOC.Unit_Abreviation AS UOCAbv,  
			UOD.Unit_Abreviation AS UODAbv,  
			UOW.Unit_Abreviation AS UOWAbv,  
			UOCost.Unit_Abreviation AS UOCostAbv,  
			inv_Containers.Tare_Weight AS Tare_Weight,  
			inv_Containers.Final_Wght AS Final_Weight,  
			inv_Containers.Net_Wght AS Net_Weight,  
			TO_CHAR(inv_Containers.Container_Cost, '999,999.99') AS Container_Cost, 
			TO_CHAR(TRUNC(inv_containers.Date_Certified),'MM/DD/YYYY') AS Date_Certified, 
			TO_CHAR(TRUNC(inv_containers.Date_Approved),'MM/DD/YYYY') AS Date_Approved, 
			inv_containers.Field_1, 
			inv_containers.Field_2, 
			inv_containers.Field_3, 
			inv_containers.Field_4, 
			inv_containers.Field_5, 
			inv_containers.Field_6, 
			inv_containers.Field_7, 
			inv_containers.Field_8, 
			inv_containers.Field_9, 
			inv_containers.Field_10, 
			TO_CHAR(TRUNC(inv_containers.Date_1),'MM/DD/YYYY') AS Date_1, 
			TO_CHAR(TRUNC(inv_containers.Date_2),'MM/DD/YYYY') AS Date_2, 
			TO_CHAR(TRUNC(inv_containers.Date_3),'MM/DD/YYYY') AS Date_3, 
			TO_CHAR(TRUNC(inv_containers.Date_4),'MM/DD/YYYY') AS Date_4, 
			TO_CHAR(TRUNC(inv_containers.Date_5),'MM/DD/YYYY') AS Date_5, 
			inv_containers.Creator AS Creator FROM 
			inv_Containers, inv_Locations, inv_Compounds, inv_Suppliers, inv_Units UOM, inv_Units UOP, inv_Units UOC, inv_Units UOD, inv_Units UOW, inv_Container_Types, inv_container_status, inv_solvents, inv_address, inv_states, inv_country, inv_units UOCost, inv_vw_reg_batches ivrb, inv_vw_reg_structures ivrs 
			WHERE inv_containers.container_id = lContainerID
			AND inv_Containers.Compound_ID_FK = inv_Compounds.Compound_ID(+) 
			AND inv_compounds.reg_id_fk = ivrb.RegID(+) 		
			AND	inv_compounds.batch_number_fk = ivrb.BatchNumber(+) 		
			AND	ivrb.RegID = ivrs.RegID(+) 
			AND inv_Containers.Location_ID_FK = inv_Locations.Location_ID  
			AND inv_Containers.UNIT_OF_MEAS_ID_FK = UOM.Unit_ID 
			AND inv_Containers.Unit_of_Purity_ID_FK = UOP.Unit_ID(+) 
			AND inv_Containers.Unit_of_Conc_ID_FK = UOC.Unit_ID(+) 
			AND inv_Containers.Unit_of_Density_ID_FK = UOD.Unit_ID(+) 
			AND inv_Containers.Unit_of_wght_ID_FK = UOW.Unit_ID(+) 
			AND inv_Containers.Unit_of_Cost_ID_FK = UOCost.Unit_ID(+) 
			AND inv_Suppliers.Supplier_Address_ID_FK = inv_Address.Address_ID(+) 
			AND inv_Address.State_ID_FK = inv_states.State_ID(+) 
			AND inv_Address.Country_ID_FK = inv_country.Country_ID(+) 
			AND inv_Containers.Container_Status_ID_FK = inv_Container_Status.Container_Status_ID(+) 
			AND inv_Containers.Container_Type_ID_FK = inv_Container_Types.Container_Type_ID 			
			AND inv_Containers.Supplier_ID_FK = inv_Suppliers.Supplier_ID(+) 
			AND inv_Containers.Solvent_ID_FK = inv_Solvents.Solvent_ID(+);
		ELSE
			OPEN O_RS FOR SELECT /*+ ORDERED */ 
			inv_Containers.Container_ID AS Container_ID,  
			inv_Containers.Barcode AS Barcode,
			inv_Containers.Container_Name AS Container_Name, 
			inv_Containers.Batch_ID_FK AS Batch_ID_FK, 
			DECODE(inv_Containers.Batch_ID_FK, NULL, ' ', replace(CHEMINVDB2.GUIUTILS.GETBATCHUOMAMOUNTSTRING(container_ID,NULL),':',' ')) as totalBatchAmt,  
			inv_Container_Types.Container_Type_Name AS Container_Type_Name,  
			inv_Container_Status.Container_Status_Name AS Container_Status_Name,  
			inv_Containers.Container_Description AS Container_Description, 
			inv_Containers.Container_Comments AS Container_Comments, 
			inv_Containers.Storage_Conditions AS Storage_Conditions, 
			inv_Containers.Handling_Procedures AS Handling_Procedures, 
			inv_Containers.Family AS Family, 
			inv_Locations.Location_Name AS Location_Name,  
			inv_Locations.Location_Type_ID_FK as Location_Type_ID_FK, 
			inv_Compounds.Compound_ID AS Compound_ID, 
			inv_Compounds.ACX_ID AS ACX_ID, 
			ALT_ID_1, 
			ALT_ID_2, 
			ALT_ID_3, 
			ALT_ID_4, 
			ALT_ID_5, 
			CHEMINVDB2.RESERVATIONS.GETTOTALQTYRESERVED(inv_Containers.Container_ID) as TotalQtyReserved, 
			TO_CHAR(TRUNC(inv_Containers.DATE_Expires),'MM/DD/YYYY') AS DATE_Expires,  
			TO_CHAR(TRUNC(inv_Containers.DATE_Created),'MM/DD/YYYY')  AS DATE_Created,  
			TO_CHAR(TRUNC(inv_Containers.DATE_Produced),'MM/DD/YYYY')  AS DATE_Produced, 
			TO_CHAR(TRUNC(inv_Containers.DATE_Ordered),'MM/DD/YYYY')  AS DATE_Ordered, 
			TO_CHAR(TRUNC(inv_Containers.DATE_Received),'MM/DD/YYYY')  AS DATE_Received, 
			inv_Containers.Parent_container_id_fk as Parent_container_id, 
			(SELECT c2.barcode from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk) as Parent_container_barcode, 
			case when (select count((select e.location_id_fk from inv_grid_element e, inv_grid_storage s, inv_locations p where e.grid_storage_id_fk = s.grid_storage_id and p.location_id = s.location_id_fk and p.collapse_child_nodes = 1  and e.location_id_fk = (SELECT c2.location_id_fk  from inv_Containers c2  where c2.container_id = inv_Containers.Parent_container_id_fk)))  from dual) = 0 then  (SELECT c2.location_id_fk from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk) else (select p.location_id from inv_grid_element e, inv_grid_storage s, inv_locations p where e.grid_storage_id_fk = s.grid_storage_id  and p.location_id = s.location_id_fk  and p.collapse_child_nodes = 1 and e.location_id_fk = (SELECT c2.location_id_fk from inv_Containers c2 where c2.container_id = inv_Containers.Parent_container_id_fk)) end as Parent_container_Location_ID, DECODE(inv_Containers.Qty_Max, NULL, ' ', inv_Containers.Qty_Max||' '||UOM.Unit_Abreviation) AS ContainerSize ,  
			DECODE(inv_Containers.Qty_Remaining, NULL, ' ', inv_Containers.Qty_Remaining||' '||UOM.Unit_Abreviation) AS AmountRemaining ,  
			DECODE(inv_Containers.Qty_Available, NULL, ' ', inv_Containers.Qty_Available||' '||UOM.Unit_Abreviation) AS AmountAvailable ,  
			DECODE(inv_Containers.Concentration, NULL, ' ', inv_Containers.Concentration||' '||UOC.Unit_Abreviation) AS Concentration_Text, 
			DECODE(inv_Containers.Density, NULL, ' ', inv_Containers.Density||' '||UOD.Unit_Abreviation) AS Density_Text, 
			DECODE(inv_Containers.Purity, NULL, ' ', inv_Containers.Purity||' '||UOP.Unit_Abreviation) AS Purity_Text, 
			DECODE(inv_Containers.Net_Wght, NULL, ' ', inv_Containers.Net_Wght||' '||UOW.Unit_Abreviation) AS Weight_Text, 
			DECODE(inv_Containers.Container_Cost, NULL, ' ', UOCost.Unit_Abreviation||TO_CHAR(inv_Containers.Container_Cost,'999,999.99')) AS ContainerCost,  
			inv_Containers.Concentration AS Concentration, 
			inv_Containers.Density AS Density, 
			inv_Containers.Purity AS Purity, 
			inv_Containers.Grade AS Grade, 
			inv_Containers.Solvent_ID_FK,
			DECODE(inv_Containers.Solvent_ID_FK, NULL, ' ', inv_Solvents.Solvent_Name) AS Solvent_Name,  
			inv_Containers.Lot_Num AS Lot_Num, 
			inv_Containers.Supplier_CATNUM AS Supplier_CATNUM, 
			inv_Suppliers.Supplier_Name AS Supplier_Name, 
			inv_Suppliers.Supplier_Short_Name AS Supplier_Short_Name, 
			inv_Containers.PO_Number AS PO_Number, 
			inv_Containers.PO_Line_Number AS PO_Line_Number, 
			inv_Containers.Req_Number AS Req_Number, 
			inv_Address.Contact_Name AS ContactName, 
			inv_Address.Address1 AS Address1, 
			inv_Address.Address2 AS Address2, 
			inv_Address.Address3 AS Address3, 
			inv_Address.Address4 AS Address4, 
			inv_Address.City AS , 
			inv_States.State_Name AS State_Name, 
			inv_Country.Country_Name AS Country_Name, 
			inv_Address.ZIP AS ZIP, 
			inv_Address.FAX AS FAX, 
			inv_Address.Phone AS Phone, 
			inv_Address.Email AS Email, 
			inv_Containers.Qty_MinStock AS Qty_MinStock, 
			inv_Containers.Qty_MaxStock AS Qty_MaxStock, 
			DECODE(inv_Containers.Qty_MinStock, NULL, ' ', inv_Containers.Qty_MinStock||' '||UOM.Unit_Abreviation) AS Qty_MinStock_String ,  
			DECODE(inv_Containers.Qty_MaxStock, NULL, ' ', inv_Containers.Qty_MaxStock||' '||UOM.Unit_Abreviation) AS Qty_MaxStock_String ,  
			inv_Compounds.Reg_ID_FK,  
			inv_Compounds.Batch_Number_FK,
			inv_Compounds.Substance_Name AS Substance_Name,
			inv_Compounds.CAS AS CAS,
			inv_Compounds.BASE64_CDX AS BASE64_CDX,
			null as RegNumber,			
			null as Reg_Batch_ID,			
			inv_Containers.Location_ID_FK,  
			inv_Containers.Compound_ID_FK,  
			inv_Containers.Container_Type_ID_FK,  
			inv_Containers.Container_Status_ID_FK,  
			inv_Containers.Supplier_ID_FK, 
			inv_Containers.Owner_ID_FK, 
			inv_Containers.Current_User_ID_FK, 
			inv_Containers.Qty_Initial AS Qty_Initial,  
			inv_Containers.Qty_Max AS Qty_Max,  
			inv_Containers.Qty_Remaining AS Qty_Remaining,  
			inv_Containers.Qty_Available AS Qty_Available,  
			UOM.Unit_ID AS UOMID,  
			UOP.Unit_ID AS UOPID,  
			UOC.Unit_ID AS UOCID,  
			UOD.Unit_ID AS UODID,  
			UOW.Unit_ID AS UOWID,  
			UOCost.Unit_ID AS UOCostID,  
			UOM.Unit_Name AS UOMName,  
			UOP.Unit_Name AS UOPName,  
			UOC.Unit_Name AS UOCName,  
			UOD.Unit_Name AS UODName,  
			UOW.Unit_Name AS UOWName,  
			UOCost.Unit_Name AS UOCostName,  
			UOM.Unit_Abreviation AS UOMAbv,  
			UOP.Unit_Abreviation AS UOPAbv,  
			UOC.Unit_Abreviation AS UOCAbv,  
			UOD.Unit_Abreviation AS UODAbv,  
			UOW.Unit_Abreviation AS UOWAbv,  
			UOCost.Unit_Abreviation AS UOCostAbv,  
			inv_Containers.Tare_Weight AS Tare_Weight,  
			inv_Containers.Final_Wght AS Final_Weight,  
			inv_Containers.Net_Wght AS Net_Weight,  
			TO_CHAR(inv_Containers.Container_Cost, '999,999.99') AS Container_Cost, 
			TO_CHAR(TRUNC(inv_containers.Date_Certified),'MM/DD/YYYY') AS Date_Certified, 
			TO_CHAR(TRUNC(inv_containers.Date_Approved),'MM/DD/YYYY') AS Date_Approved, 
			inv_containers.Field_1, 
			inv_containers.Field_2, 
			inv_containers.Field_3, 
			inv_containers.Field_4, 
			inv_containers.Field_5, 
			inv_containers.Field_6, 
			inv_containers.Field_7, 
			inv_containers.Field_8, 
			inv_containers.Field_9, 
			inv_containers.Field_10, 
			TO_CHAR(TRUNC(inv_containers.Date_1),'MM/DD/YYYY') AS Date_1, 
			TO_CHAR(TRUNC(inv_containers.Date_2),'MM/DD/YYYY') AS Date_2, 
			TO_CHAR(TRUNC(inv_containers.Date_3),'MM/DD/YYYY') AS Date_3, 
			TO_CHAR(TRUNC(inv_containers.Date_4),'MM/DD/YYYY') AS Date_4, 
			TO_CHAR(TRUNC(inv_containers.Date_5),'MM/DD/YYYY') AS Date_5, 
			inv_containers.Creator AS Creator FROM 
			inv_Containers, inv_Locations, inv_Compounds, inv_Suppliers, inv_Units UOM, inv_Units UOP, inv_Units UOC, inv_Units UOD, inv_Units UOW, inv_Container_Types, inv_container_status, inv_solvents, inv_address, inv_states, inv_country, inv_units UOCost
			WHERE inv_containers.container_id = lContainerID			
			AND inv_Containers.Location_ID_FK = inv_Locations.Location_ID  
			AND inv_Containers.UNIT_OF_MEAS_ID_FK = UOM.Unit_ID 
			AND inv_Containers.Unit_of_Purity_ID_FK = UOP.Unit_ID(+) 
			AND inv_Containers.Unit_of_Conc_ID_FK = UOC.Unit_ID(+) 
			AND inv_Containers.Unit_of_Density_ID_FK = UOD.Unit_ID(+) 
			AND inv_Containers.Unit_of_wght_ID_FK = UOW.Unit_ID(+) 
			AND inv_Containers.Unit_of_Cost_ID_FK = UOCost.Unit_ID(+) 
			AND inv_Suppliers.Supplier_Address_ID_FK = inv_Address.Address_ID(+) 
			AND inv_Address.State_ID_FK = inv_states.State_ID(+) 
			AND inv_Address.Country_ID_FK = inv_country.Country_ID(+) 
			AND inv_Containers.Container_Status_ID_FK = inv_Container_Status.Container_Status_ID(+) 
			AND inv_Containers.Container_Type_ID_FK = inv_Container_Types.Container_Type_ID 
			AND inv_Containers.Compound_ID_FK = inv_Compounds.Compound_ID(+) 
			AND inv_Containers.Supplier_ID_FK = inv_Suppliers.Supplier_ID(+) 
			AND inv_Containers.Solvent_ID_FK = inv_Solvents.Solvent_ID(+);		
		END IF;
	END GETCONTAINERATTRIBUTES;

	PROCEDURE GETCONTAINERSUBSTANCE(pContainerIDList IN VARCHAR2 := NULL,
								 pContainerBarcodeList IN VARCHAR2 := NULL,
								 O_RS OUT CURSOR_TYPE) IS
		my_sql VARCHAR2(2000);
		vContainerBarcodeList_t       STRINGUTILS.t_char;
	BEGIN
		my_sql := 'SELECT c.container_id,
	                  c.barcode,
	                  c.container_name,
					  c.container_status_id_fk,
					  l.location_name,
					  GetLocationPath(l.location_id) AS Path,					  
					  u.unit_abreviation,
	                  c.qty_remaining,
					  ic.compound_id,
					  ic.cas,
					  ic.acx_id,
					  ic.substance_name,
					  ic.molecular_weight,
					  ic.density,
					  ic.clogp,
					  ic.rotatable_bonds,
					  ic.tot_pol_surf_area,
					  ic.hbond_acceptors,
					  ic.hbond_donors,
					  ic.alt_id_1,
					  ic.alt_id_2,
					  ic.alt_id_3,
					  ic.alt_id_4,
					  ic.alt_id_5,
					  ic.conflicting_fields,
					  ic.rid,
					  ic.creator,
					  ic.timestamp,
					  ic.base64_cdx
					FROM inv_containers c, inv_locations l, inv_units u, inv_compounds ic
					WHERE c.location_id_fk = l.location_id
					AND c.unit_of_meas_id_fk = u.unit_id
					AND c.compound_id_fk = ic.compound_id(+) ';
		
		IF pContainerIDList IS NOT NULL THEN
			my_sql := my_sql || 'AND Container_ID IN(' || pContainerIDList || ')';
		ELSE
			my_sql := my_sql || 'AND Barcode IN(';
			vContainerBarcodeList_t := STRINGUTILS.split(pContainerBarcodeList, ',');

			FOR i IN vContainerBarcodeList_t.FIRST .. vContainerBarcodeList_t.LAST
			LOOP
				my_sql := my_sql || '''' || vContainerBarcodeList_t(i) || '''';
				if i != vContainerBarcodeList_t.LAST then
					my_sql := my_sql || ',';
				end if;
			END LOOP;
			my_sql := my_sql || ')';
		END IF;
		my_sql := my_sql || ' ORDER BY c.container_id ASC';
		OPEN O_RS FOR my_sql;
	END GETCONTAINERSUBSTANCE;

	PROCEDURE SETCONTAINERSUBSTANCE(pContainerIDList IN VARCHAR2 := NULL,
								 pContainerBarcodeList IN VARCHAR2 := NULL,
								 pCompoundID IN inv_containers.compound_id_fk%TYPE) IS		
		vSQL varchar2(1000);
		vContainerBarcodeList_t       STRINGUTILS.t_char;
	BEGIN
		vSQL := 'UPDATE inv_containers SET compound_id_fk = ' || pCompoundID || ' ';
		IF pContainerIDList IS NOT NULL THEN
			vSQL := vSQL || 'WHERE Container_ID IN(' || pContainerIDList || ')';
		ELSE
			vSQL := vSQL || 'WHERE Barcode IN(';			
			vContainerBarcodeList_t := STRINGUTILS.split(pContainerBarcodeList, ',');

			FOR i IN vContainerBarcodeList_t.FIRST .. vContainerBarcodeList_t.LAST
			LOOP
				vSQL := vSQL || '''' || vContainerBarcodeList_t(i) || '''';
				if i != vContainerBarcodeList_t.LAST then
					vSQL := vSQL || ',';
				end if;
			END LOOP;
			vSQL := vSQL || ')';
		END IF;
		EXECUTE IMMEDIATE vSQL;
	END SETCONTAINERSUBSTANCE;

	PROCEDURE GETKEYPLATEATTRIBUTES(pPlateIDList IN VARCHAR2 := NULL,
							  pPlateBarcodeList IN VARCHAR2 := NULL,
							  O_RS OUT CURSOR_TYPE) IS
		my_sql VARCHAR2(2000);
	BEGIN
		my_sql := 'SELECT p.plate_id,
	                  p.plate_barcode,
	                  p.plate_name,
	                  l.location_name,
				pf.plate_format_name,
				pt.plate_type_name,
	                  u.unit_abreviation,
	                  p.qty_remaining,
	                  GetLocationPath(l.location_id) AS Path,
				e.enum_value as plate_status_name
	           FROM inv_plates p, inv_locations l, inv_plate_format pf, inv_plate_types pt, inv_enumeration e, inv_units u
	           WHERE p.location_id_fk = l.location_id
		   AND p.plate_type_id_fk = pt.plate_type_id
		   AND p.plate_format_id_fk = pf.plate_format_id
		   AND p.status_id_fk = e.enum_id
	           AND p.qty_unit_fk = u.unit_id(+)';
		IF pPlateIDList IS NOT NULL THEN
			my_sql := my_sql || 'AND plate_ID IN(' || pPlateIDList || ')';
		ELSE
			my_sql := my_sql || 'AND plate_barcode IN(' || pPlateBarcodeList || ')';
		END IF;
		my_sql := my_sql || ' ORDER BY l.location_name ASC';
		OPEN O_RS FOR my_sql;
	END GETKEYPLATEATTRIBUTES;

	PROCEDURE DISPLAYCREATEDCONTAINERS(p_ContainerList IN VARCHAR2 := NULL,
								O_RS OUT CURSOR_TYPE) IS
		my_sql VARCHAR2(5000);
	BEGIN
		my_sql := 'Select
		  c.container_id
		  , c.barcode
		  , c.container_name
		  , guiutils.GETLOCATIONPATH(c.location_id_fk) as Location_Path
		  , UOM.Unit_ID AS UOMID
		  , UOP.Unit_ID AS UOPID
		  , UOC.Unit_ID AS UOCID
		  , UOD.Unit_ID AS UODID
		  , UOW.Unit_ID AS UOWID
		  , UOM.Unit_Name AS UOMName
		  , UOP.Unit_Name AS UOPName
		  , UOC.Unit_Name AS UOCName
		  , UOD.Unit_Name AS UODName
		  , UOW.Unit_Name AS UOWName
		  , UOM.Unit_Abreviation AS UOMAbv
		  , UOP.Unit_Abreviation AS UOPAbv
		  , UOC.Unit_Abreviation AS UOCAbv
		  , UOD.Unit_Abreviation AS UODAbv
		  , UOW.Unit_Abreviation AS UOWAbv
		  , DECODE(c.Qty_Max, NULL, '' '', c.Qty_Max||'' ''||UOM.Unit_Abreviation) AS ContainerSize
		  , DECODE(c.Qty_Remaining, NULL, '' '', c.Qty_Remaining||'' ''||UOM.Unit_Abreviation) AS AmountRemaining
		  , DECODE(c.Qty_Available, NULL, '' '', c.Qty_Available||'' ''||UOM.Unit_Abreviation) AS AmountAvailable
		  , DECODE(c.Concentration, NULL, '' '', c.Concentration||'' ''||UOC.Unit_Abreviation) AS Concentration_Text
		  , DECODE(c.Density, NULL, '' '', c.Density||'' ''||UOD.Unit_Abreviation) AS Density_Text
		  , DECODE(c.Purity, NULL, '' '', c.Purity||'' ''||UOP.Unit_Abreviation) AS Purity_Text
		  , DECODE(c.Net_Wght, NULL, '' '', c.Net_Wght||'' ''||UOW.Unit_Abreviation) AS Weight_Text
	   From inv_containers c
		  , inv_Units UOM
		  , inv_Units UOP
		  , inv_Units UOC
		  , inv_Units UOD
		  , inv_Units UOW
		  , inv_Container_Types ct
	   Where c.container_id in (' || p_ContainerList || ')
	   And c.UNIT_OF_MEAS_ID_FK = UOM.Unit_ID
	   And c.Unit_of_Purity_ID_FK = UOP.Unit_ID(+)
	   And c.Unit_of_Conc_ID_FK = UOC.Unit_ID(+)
	   And c.Unit_of_Density_ID_FK = UOD.Unit_ID(+)
	   And c.Unit_of_wght_ID_FK = UOW.Unit_ID(+)
	   And c.Container_Type_ID_FK = ct.Container_Type_ID';
		OPEN O_RS FOR my_sql;
	END DISPLAYCREATEDCONTAINERS;
	FUNCTION GETLOCATIONPATH(pLocationID IN inv_locations.location_id%TYPE)
		RETURN VARCHAR2 IS

		CURSOR LocationNodes_cur(LocationID_in IN inv_locations.location_id%TYPE) IS
			SELECT Location_Name
			FROM inv_Locations
			CONNECT BY Location_id = PRIOR Parent_id
			START WITH Location_id = LocationID_in
			ORDER BY LEVEL DESC;
		path_str     VARCHAR2(2000);
		locationName VARCHAR2(200);
	BEGIN
		OPEN LocationNodes_cur(pLocationID);
		LOOP
			FETCH LocationNodes_cur
				INTO locationName;
			EXIT WHEN LocationNodes_cur%NOTFOUND;
			path_str := path_str || locationName || '\';
		END LOOP;
		--dbms_output.put_line(path_str);
		RETURN path_str;
	END GETLOCATIONPATH;
	FUNCTION GETRACKLOCATIONPATH(pLocationID IN inv_locations.location_id%TYPE)
		RETURN VARCHAR2 IS
		CURSOR LocationNodes_cur(LocationID_in IN inv_locations.location_id%TYPE) IS
			SELECT Location_Name, collapse_child_nodes
			FROM inv_Locations
			CONNECT BY Location_id = PRIOR Parent_id
			START WITH Location_id = LocationID_in
			ORDER BY LEVEL DESC;
		path_str           VARCHAR2(2000);
		locationName       VARCHAR2(200);
		collapseChildNodes inv_locations.collapse_child_nodes%TYPE;
	BEGIN
		OPEN LocationNodes_cur(pLocationID);
		LOOP
			FETCH LocationNodes_cur
				INTO locationName, collapseChildNodes;
			EXIT WHEN LocationNodes_cur%NOTFOUND;
			IF collapseChildNodes IS NOT NULL THEN
				path_str := path_str || locationName || '\';
			END IF;
		END LOOP;
		--dbms_output.put_line(path_str);
		RETURN path_str;
	END GETRACKLOCATIONPATH;
	FUNCTION GETFULLRACKLOCATIONPATH(pLocationID IN inv_locations.location_id%TYPE)
		RETURN VARCHAR2 IS
		CURSOR LocationNodes_cur(LocationID_in IN inv_locations.location_id%TYPE) IS
			SELECT Location_Name, collapse_child_nodes, Location_ID
			FROM inv_Locations
			CONNECT BY Location_id = PRIOR Parent_id
			START WITH Location_id = LocationID_in
			ORDER BY LEVEL DESC;
		path_str           VARCHAR2(2000);
		locationName       VARCHAR2(200);
		collapseChildNodes inv_locations.collapse_child_nodes%TYPE;
		locationID         inv_locations.location_id%TYPE;
	BEGIN
		OPEN LocationNodes_cur(pLocationID);
		LOOP
			FETCH LocationNodes_cur
				INTO locationName, collapseChildNodes, locationID;
			EXIT WHEN LocationNodes_cur%NOTFOUND;
			IF locationID > 0 THEN
				path_str := path_str || locationName || '\';
			END IF;
		END LOOP;
		--dbms_output.put_line(path_str);
		RETURN path_str;
	END GETFULLRACKLOCATIONPATH;

	PROCEDURE GETRECENTLOCATIONS(pContainerID IN inv_containers.container_id%TYPE,
						    pNumRows IN INTEGER := 5,
						    O_RS OUT CURSOR_TYPE) IS
		mySQL VARCHAR2(2000);
	BEGIN

		mySQL := 'SELECT c.raid, rid,
					getLocationPath(new_value) AS LocationPath, TimeStamp,
					(SELECT location_name from inv_locations WHERE location_id = new_value) AS LocationName,
				  	(SELECT Upper(old_value) from audit_column where column_name= ''CURRENT_USER_ID_FK'' AND RAID = c.raid) AS fromUser,
					(SELECT Upper(new_value) from audit_column where column_name= ''CURRENT_USER_ID_FK'' AND RAID = c.raid) AS toUser
			FROM audit_column c, audit_row r
			WHERE c.raid = r.raid
			AND column_Name = ''LOCATION_ID_FK''
			AND rid = (SELECT rid from inv_containers where container_ID = ' ||
			    pContainerID || ')
			AND rowNum < ' || pNumRows || '
			Order by TimeStamp desc';
		OPEN O_RS FOR mySQL;
	END GETRECENTLOCATIONS;

	FUNCTION GETPARENTPLATEIDS(pPlateID inv_plates.plate_id%TYPE) RETURN VARCHAR2 IS
		vCount          NUMBER;
		vParentPlateIDs VARCHAR2(1000);
	BEGIN
		SELECT COUNT(*)
		INTO vCount
		FROM inv_plate_parent
		WHERE child_plate_id_fk = pPlateID;
		IF vCount > 0 THEN
			FOR parentPlateID_rec IN (SELECT parent_plate_id_fk
								 FROM inv_plate_parent
								 WHERE child_plate_id_fk = pPlateID)
			LOOP
				vParentPlateIDs := vParentPlateIDs ||
							    parentPlateID_rec.parent_plate_id_fk || ',';
			END LOOP;
			vParentPlateIDs := rTrim(vParentPlateIDs, ',');
			--ELSE
			--    SELECT parent_plate_id_fk INTO vParentPlateIDs FROM inv_plates WHERE plate_id = pPlateID;
		END IF;

		--return '1';
		RETURN vParentPlateIDs;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATEIDS;

	FUNCTION GETPARENTPLATEBARCODES(pPlateID inv_plates.plate_id%TYPE)
		RETURN VARCHAR2 IS
		vParentPlateIDs      VARCHAR2(1000);
		vPlateIDList_t       STRINGUTILS.t_char;
		vTempBarcode         inv_plates.plate_barcode%TYPE;
		vParentPlateBarcodes VARCHAR2(1000);
	BEGIN
		vParentPlateIDs := GetParentPlateIDs(pPlateID);

		IF vParentPlateIDs IS NULL THEN
			vParentPlateBarcodes := NULL;
		ELSE
			vPlateIDList_t := STRINGUTILS.split(vParentPlateIDs, ',');
			FOR i IN vPlateIDList_t.FIRST .. vPlateIDList_t.LAST
			LOOP
				SELECT plate_barcode
				INTO vTempBarcode
				FROM inv_plates
				WHERE plate_id = vPlateIDList_t(i);
				vParentPlateBarcodes := vParentPlateBarcodes ||
								    vTempBarcode || ',';
			END LOOP;
			vParentPlateBarcodes := rTrim(vParentPlateBarcodes, ',');
		END IF;

		RETURN vParentPlateBarcodes;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;
	END GETPARENTPLATEBARCODES;

	FUNCTION GETPARENTPLATELOCATIONIDS(pPlateID inv_plates.plate_id%TYPE)
		RETURN VARCHAR2 IS
		vParentPlateIDs         VARCHAR2(1000);
		vPlateIDList_t          STRINGUTILS.t_char;
		vTempLocationID         inv_locations.location_id%TYPE;
		vParentPlateLocationIDs VARCHAR2(1000);
	BEGIN
		vParentPlateIDs := GetParentPlateIDs(pPlateID);
		IF vParentPlateIDs IS NULL THEN
			vParentPlateLocationIDs := NULL;
		ELSE
			vPlateIDList_t := STRINGUTILS.split(vParentPlateIDs, ',');
			FOR i IN vPlateIDList_t.FIRST .. vPlateIDList_t.LAST
			LOOP
				SELECT location_id
				INTO vTempLocationID
				FROM inv_locations, inv_plates
				WHERE location_id = location_id_fk
					 AND plate_id = vPlateIDList_t(i);
				vParentPlateLocationIDs := vParentPlateLocationIDs ||
									  vTempLocationID || ',';
			END LOOP;
			vParentPlateLocationIDs := rTrim(vParentPlateLocationIDs, ',');
		END IF;

		RETURN vParentPlateLocationIDs;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATELOCATIONIDS;

	FUNCTION GETPARENTWELLIDS(pWellID inv_wells.well_id%TYPE) RETURN VARCHAR2 IS
		vCount         NUMBER;
		vParentWellIDs VARCHAR2(1000);
	BEGIN
		SELECT COUNT(*)
		INTO vCount
		FROM inv_well_parent
		WHERE child_well_id_fk = pWellID;
		IF vCount > 0 THEN
			FOR parentWellID_rec IN (SELECT parent_well_id_fk
								FROM inv_well_parent
								WHERE child_well_id_fk = pWellID)
			LOOP
				vParentWellIDs := vParentWellIDs ||
							   parentWellID_rec.parent_well_id_fk || ',';
			END LOOP;
			vParentWellIDs := rTrim(vParentWellIDs, ',');
			--ELSE
			--    SELECT parent_well_id_fk INTO vParentWellIDs FROM inv_wells WHERE well_id = pWellID;
		END IF;

		--return '1';
		RETURN vParentWellIDs;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTWELLIDS;

	FUNCTION GETPARENTWELLLABELS(pWellID inv_wells.well_id%TYPE) RETURN VARCHAR2 IS
		vParentWellIDs    VARCHAR2(1000);
		vWellIDList_t     STRINGUTILS.t_char;
		vTempLabel        inv_plates.plate_barcode%TYPE;
		vParentWellLabels VARCHAR2(1000);
	BEGIN
		vParentWellIDs := GetParentWellIDs(pWellID);

		IF vParentWellIDs IS NULL THEN
			vParentWellLabels := NULL;
		ELSE
			vWellIDList_t := STRINGUTILS.split(vParentWellIDs, ',');
			FOR i IN vWellIDList_t.FIRST .. vWellIDList_t.LAST
			LOOP
				SELECT plate_barcode || '-' || NAME
				INTO vTempLabel
				FROM inv_plates, inv_vw_well
				WHERE plate_id = plate_id_fk
					 AND well_id = vWellIDList_t(i);
				vParentWellLabels := vParentWellLabels || vTempLabel || ',';
			END LOOP;
			vParentWellLabels := rTrim(vParentWellLabels, ',');
		END IF;

		RETURN vParentWellLabels;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;
	END GETPARENTWELLLABELS;

	FUNCTION GETPARENTPLATEIDS2(pWellID inv_wells.well_id%TYPE) RETURN VARCHAR2 IS
		vParentWellIDs  VARCHAR2(1000);
		vWellIDList_t   STRINGUTILS.t_char;
		vTempPlateID    inv_plates.plate_id%TYPE;
		vParentPlateIDs VARCHAR2(1000);
	BEGIN
		vParentWellIDs := GETPARENTWELLIDS(pWellID);
		IF vParentWellIDs IS NULL THEN
			vParentPlateIDs := NULL;
		ELSE
			vWellIDList_t := STRINGUTILS.split(vParentWellIDs, ',');
			FOR i IN vWellIDList_t.FIRST .. vWellIDList_t.LAST
			LOOP
				SELECT plate_id_fk
				INTO vTempPlateID
				FROM inv_wells
				WHERE well_id = vWellIDList_t(i);
				vParentPlateIDs := vParentPlateIDs || vTempPlateID || ',';
			END LOOP;
			vParentPlateIDs := rTrim(vParentPlateIDs, ',');

		END IF;

		RETURN vParentPlateIDs;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATEIDS2;

	FUNCTION GETPARENTPLATELOCATIONIDS2(pWellID inv_wells.well_id%TYPE)
		RETURN VARCHAR2 IS
		vParentWellIDs          VARCHAR2(1000);
		vWellIDList_t           STRINGUTILS.t_char;
		vTempLocationID         inv_locations.location_id%TYPE;
		vParentPlateLocationIDs VARCHAR2(1000);
	BEGIN
		vParentWellIDs := GETPARENTWELLIDS(pWellID);
		IF vParentWellIDs IS NULL THEN
			vParentPlateLocationIDs := NULL;
		ELSE
			vWellIDList_t := STRINGUTILS.split(vParentWellIDs, ',');
			FOR i IN vWellIDList_t.FIRST .. vWellIDList_t.LAST
			LOOP
				SELECT location_id
				INTO vTempLocationID
				FROM inv_locations, inv_plates, inv_wells
				WHERE location_id = location_id_fk
					 AND plate_id = plate_id_fk
					 AND well_id = vWellIDList_t(i);
				vParentPlateLocationIDs := vParentPlateLocationIDs ||
									  vTempLocationID || ',';
			END LOOP;
			vParentPlateLocationIDs := rTrim(vParentPlateLocationIDs, ',');
		END IF;

		RETURN vParentPlateLocationIDs;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATELOCATIONIDS2;

	FUNCTION GETPARENTWELLNAMES(pWellID inv_wells.well_id%TYPE) RETURN VARCHAR2 IS
		vParentWellIDs    VARCHAR2(1000);
		vWellIDList_t     STRINGUTILS.t_char;
		vName             inv_plates.plate_barcode%TYPE;
		vParentWellLabels VARCHAR2(1000);
	BEGIN
		vParentWellIDs := GetParentWellIDs(pWellID);

		IF vParentWellIDs IS NULL THEN
			vParentWellLabels := NULL;
		ELSE
			vWellIDList_t := STRINGUTILS.split(vParentWellIDs, ',');
			FOR i IN vWellIDList_t.FIRST .. vWellIDList_t.LAST
			LOOP
				SELECT NAME
				INTO vName
				FROM inv_plates, inv_vw_well
				WHERE plate_id = plate_id_fk
					 AND well_id = vWellIDList_t(i);
				vParentWellLabels := vParentWellLabels || vName || ',';
			END LOOP;
			vParentWellLabels := rTrim(vParentWellLabels, ',');
		END IF;

		RETURN vParentWellLabels;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;
	END GETPARENTWELLNAMES;

	FUNCTION GETPARENTPLATEBARCODES2(pWellID inv_wells.well_id%TYPE)
		RETURN VARCHAR2 IS
		vParentPlateIDs      VARCHAR2(1000);
		vPlateIDList_t       STRINGUTILS.t_char;
		vTempBarcode         inv_plates.plate_barcode%TYPE;
		vParentPlateBarcodes VARCHAR2(1000);
	BEGIN
		vParentPlateIDs := GETPARENTPLATEIDS2(pWellID);
		IF vParentPlateIDs IS NULL THEN
			vParentPlateBarcodes := NULL;
		ELSE
			vPlateIDList_t := STRINGUTILS.split(vParentPlateIDs, ',');
			FOR i IN vPlateIDList_t.FIRST .. vPlateIDList_t.LAST
			LOOP
				SELECT plate_barcode
				INTO vTempBarcode
				FROM inv_plates
				WHERE plate_id = vPlateIDList_t(i);
				vParentPlateBarcodes := vParentPlateBarcodes ||
								    vTempBarcode || ',';
			END LOOP;
			vParentPlateBarcodes := rTrim(vParentPlateBarcodes, ',');
		END IF;

		RETURN vParentPlateBarcodes;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATEBARCODES2;

	FUNCTION GETBATCHAMOUNTSTRING(pContainerID inv_containers.container_id%TYPE,
							p_DryWeightUOM VARCHAR2) RETURN VARCHAR2 IS
		vBatchAmountString VARCHAR2(500) := '';
		vNoConvert         VARCHAR2(500) := '';
		vCurrUnitID        inv_units.unit_id%TYPE;
		vCurrUnitAbbrev    inv_units.unit_abreviation%TYPE;
		vTotal             inv_containers.qty_available%TYPE;
		vConvertedQty      inv_containers.qty_available%TYPE;
		vIsFirst           BOOLEAN := TRUE;
		l_batchID          inv_container_batches.batch_id%TYPE := NULL;
	BEGIN
		--' get batch id
		SELECT batch_id_fk
		INTO l_batchID
		FROM inv_containers
		WHERE container_id = pContainerID;
		IF l_batchID IS NOT NULL THEN
			FOR batch_rec IN (SELECT SUM(qty_available * concentration) qty_available,
								unit_of_meas_id_fk AS UOMID,
								unit_abreviation
						   FROM inv_containers, inv_units
						   WHERE unit_id = unit_of_meas_id_fk
							    AND batch_id_fk = l_batchID
						   GROUP BY unit_of_meas_id_fk, unit_abreviation
						   ORDER BY qty_available DESC)
			LOOP
				IF vIsFirst THEN
					vCurrUnitID := batch_rec.UOMID;
					IF p_DryWeightUOM IS NOT NULL THEN
						vCurrUnitAbbrev := p_DryWeightUOM;
					ELSE
						vCurrUnitAbbrev := batch_rec.unit_abreviation;
					END IF;
					vTotal   := batch_rec.qty_available;
					vIsFirst := FALSE;
				ELSE
					--try to convert
					vConvertedQty := chemcalcs.convert(batch_rec.qty_available,
												batch_rec.UOMID,
												vCurrUnitID);
					IF vConvertedQty < 0 THEN
						--couldn't convert
						vNoConvert := vNoConvert ||
								    batch_rec.qty_available || ' ' ||
								    batch_rec.unit_abreviation || ',';
					ELSE
						vTotal := vTotal + vConvertedQty;
					END IF;
				END IF;
			END LOOP;
		END IF;
		IF vBatchAmountString IS NULL AND (NOT vIsFirst) THEN
			vBatchAmountString := vTotal || ' ' || vCurrUnitAbbrev;
		END IF;
		-- append unconvertable quantities
		IF vNoConvert IS NOT NULL THEN
			vBatchAmountString := vBatchAmountString || ',' ||
							  TRIM(',' FROM vNoConvert);
		END IF;
		RETURN vBatchAmountString;
		--RETURN vTotal;
	END GETBATCHAMOUNTSTRING;
	FUNCTION GETBATCHAMOUNTSTRING(pContainerID inv_containers.container_id%TYPE)
		RETURN VARCHAR2 IS
		vBatchAmountString VARCHAR2(500) := '';
		vNoConvert         VARCHAR2(500) := '';
		vCurrUnitID        inv_units.unit_id%TYPE;
		vCurrUnitAbbrev    inv_units.unit_abreviation%TYPE;
		vTotal             inv_containers.qty_available%TYPE;
		vConvertedQty      inv_containers.qty_available%TYPE;
		vIsFirst           BOOLEAN := TRUE;
	BEGIN

		FOR sibling_rec IN (SELECT SUM(qty_available) qty_available,
							  unit_of_meas_id_fk AS UOMID,
							  unit_abreviation
						FROM inv_containers, inv_units
						WHERE unit_id = unit_of_meas_id_fk
							 AND parent_container_id_fk = pContainerID
						GROUP BY unit_of_meas_id_fk, unit_abreviation
						ORDER BY qty_available DESC)
		LOOP
			IF vIsFirst THEN
				vCurrUnitID     := sibling_rec.UOMID;
				vCurrUnitAbbrev := sibling_rec.unit_abreviation;
				vTotal          := sibling_rec.qty_available;
				vIsFirst        := FALSE;
			ELSE
				--try to convert
				vConvertedQty := chemcalcs.convert(sibling_rec.qty_available,
											sibling_rec.UOMID,
											vCurrUnitID);
				IF vConvertedQty < 0 THEN
					--couldn't convert
					vNoConvert := vNoConvert || sibling_rec.qty_available || ' ' ||
							    sibling_rec.unit_abreviation || ',';
				ELSE
					vTotal := vTotal + vConvertedQty;
				END IF;

			END IF;
		END LOOP;

		IF vBatchAmountString IS NULL AND (NOT vIsFirst) THEN
			vBatchAmountString := vTotal || ' ' || vCurrUnitAbbrev;
		END IF;
		-- append unconvertable quantities
		IF vNoConvert IS NOT NULL THEN
			vBatchAmountString := vBatchAmountString || ',' ||
							  TRIM(',' FROM vNoConvert);
		END IF;
		RETURN vBatchAmountString;
		--RETURN vTotal;

	END GETBATCHAMOUNTSTRING;
	
	FUNCTION GETQTYAVAILABLE(pContainerID inv_containers.container_id%TYPE)
	RETURN NUMBER IS
		vQtyAvailable inv_containers.qty_available%TYPE;
	BEGIN
		SELECT ic.qty_available * DECODE(ic.concentration, NULL, 1, ic.concentration)
		INTO vQtyAvailable	
		FROM inv_containers ic
		WHERE ic.container_id = pContainerID;
		RETURN vQtyAvailable;
	END GETQTYAVAILABLE;
	
	FUNCTION GETBATCHUOMAMOUNTSTRING(pContainerID inv_containers.container_id%TYPE,
			pBatchID inv_container_batches.batch_id%TYPE)
	RETURN VARCHAR2 IS
		vBatchAmountString VARCHAR2(500) := '';		
		vIsFirst           BOOLEAN := TRUE;
		l_batchID          inv_container_batches.batch_id%TYPE := NULL;
	BEGIN
		IF pContainerID IS NOT NULL THEN
			--' get batch id
			SELECT batch_id_fk
			INTO l_batchID
			FROM inv_containers
			WHERE container_id = pContainerID;
		ELSE
			IF pBatchID IS NOT NULL THEN
				l_batchID := pBatchID;			
			END IF;
		END IF;
		IF l_batchID IS NOT NULL THEN
			FOR batch_rec IN (SELECT SUM(GUIUTILS.GETQTYAVAILABLE(ic.container_id)) qty_available,
								ic.unit_of_meas_id_fk,
								iu.unit_abreviation
						   FROM inv_containers ic, inv_units iu
						   WHERE ic.batch_id_fk = l_batchID
						   AND GUIUTILS.IS_CONTAINER_AVAILABLE(ic.container_id) = 1
						   AND iu.unit_id = ic.unit_of_meas_id_fk						   
						   GROUP BY ic.unit_of_meas_id_fk, iu.unit_abreviation
						   ORDER BY iu.unit_abreviation DESC)
			LOOP
				IF NOT vIsFirst THEN
					vBatchAmountString := vBatchAmountString || ',';
				ELSE
					vIsFirst := FALSE;					
				END IF;
				vBatchAmountString := vBatchAmountString || batch_rec.qty_available || ':' || batch_rec.unit_abreviation;
			END LOOP;
		END IF;
		RETURN vBatchAmountString;

	END GETBATCHUOMAMOUNTSTRING;
	
	FUNCTION IS_CONTAINER_AVAILABLE(pContainerID inv_containers.container_id%TYPE) 
	RETURN NUMBER IS
		lReturn NUMBER;
	BEGIN
		SELECT COUNT(*) INTO lReturn
		FROM inv_containers ic
		WHERE ic.container_id = pContainerID
		AND ic.location_id_fk NOT IN(1,2,3,4);
		-- 1 = On Order
		-- 2 = Disposed
		-- 3 = Trash Can
		-- 4 = Missing
		RETURN lReturn;
	END IS_CONTAINER_AVAILABLE;

	FUNCTION IS_NUMBER(p VARCHAR2) RETURN NUMBER IS
		l_num NUMBER;
	BEGIN
		l_num := p;
		RETURN 1;
	EXCEPTION
		WHEN OTHERS THEN
			RETURN 0;

	END IS_NUMBER;

	PROCEDURE GetRootNodes(p_selectedID IN NUMBER,
					   p_assetType IN VARCHAR2,
					   O_RS OUT CURSOR_TYPE) IS
		l_parentCount inv_plate_parent.parent_plate_id_fk%TYPE;
		l_childCount  inv_plate_parent.child_plate_id_fk%TYPE;
	BEGIN
		IF p_assetType = 'plate' THEN
			--' determine the number of parent plates
			SELECT COUNT(*)
			INTO l_parentCount
			FROM inv_plate_parent
			WHERE child_plate_id_fk = p_selectedID;
			--' determine the number of child plates
			SELECT COUNT(*)
			INTO l_childCount
			FROM inv_plate_parent
			WHERE parent_plate_id_fk = p_selectedID;

			--' if the plate is a root plate (no parents) return this plate ID
			IF l_parentCount = 0 AND l_childCount > 0 THEN
				OPEN O_RS FOR
					SELECT plate_id AS id, plate_barcode AS displayText
					FROM inv_plates
					WHERE plate_id = p_selectedID;
			ELSE
				OPEN O_RS FOR
					SELECT plate_id AS id, plate_barcode AS displayText
					FROM inv_plates
					WHERE plate_id IN
						 (SELECT DISTINCT parent_plate_id_fk
						  FROM inv_plate_parent
						  WHERE parent_plate_id_fk NOT IN
							   (SELECT child_plate_id_fk
							    FROM inv_plate_parent)
						  START WITH child_plate_id_fk = p_selectedid
						  CONNECT BY PRIOR parent_plate_id_fk =
								    child_plate_id_fk);
			END IF;
		ELSIF p_assetType = 'container' THEN
			--' containers can only have 0 or 1 parent container
			SELECT COUNT(*)
			INTO l_parentCount
			FROM inv_containers
			WHERE parent_container_id_fk IS NOT NULL
				 AND container_id = p_selectedID;
			--' determine the number of child plates
			SELECT COUNT(*)
			INTO l_childCount
			FROM inv_containers
			WHERE parent_container_id_fk = p_selectedID;

			IF l_parentCount = 0 AND l_childCount > 0 THEN
				OPEN O_RS FOR
					SELECT container_id AS id, barcode AS displayText
					FROM inv_containers
					WHERE container_id = p_selectedID;
			ELSE
				OPEN O_RS FOR
					SELECT container_id AS id, barcode AS displayText
					FROM inv_containers
					WHERE parent_container_id_fk IS NULL
						 AND container_id IN
						 (SELECT container_id
							 FROM inv_containers
							 START WITH container_id = p_selectedID
							 CONNECT BY PRIOR parent_container_id_fk =
									   container_id);
			END IF;
		END IF;
	END GetRootNodes;

	PROCEDURE GetLineage(p_rootID IN NUMBER,
					 p_assetType IN VARCHAR2,
					 O_RS OUT CURSOR_TYPE) IS
	BEGIN
		/*
                        --' find the root parent(s) of this plate
                        SELECT  DISTINCT
                                        parent_plate_id_fk
                                                 FROM inv_plate_parent
                                                  WHERE parent_plate_id_fk NOT IN (SELECT child_plate_id_fk FROM inv_plate_parent)
                                                       START WITH CHILD_PLATE_ID_FK = p_selectedID
                                                           CONNECT BY PRIOR PARENT_PLATE_ID_FK = CHILD_PLATE_ID_FK
          */
		IF p_assetType = 'plate' THEN

			OPEN O_RS FOR
				SELECT plate_id AS pk,
					  plate_barcode AS barcode,
					  location_id_fk,
					  GuiUtils.GETLOCATIONPATH(location_id_fk) AS locationPath,
					  NULL AS parent_id,
					  NULL AS child_id,
					  '_' || plate_id AS id
				FROM inv_plates
				WHERE plate_id = p_rootID
				UNION
				SELECT plate_id AS pk,
					  plate_barcode AS barcode,
					  location_id_fk,
					  GuiUtils.GETLOCATIONPATH(location_id_fk) AS locationPath,
					  parent_plate_id_fk AS parent_id,
					  child_plate_id_fk AS child_id,
					  sys_connect_by_path(plate_id, '_') AS id
				FROM inv_plates, inv_plate_parent
				WHERE plate_id = child_plate_id_fk(+)
				START WITH parent_plate_id_fk = p_rootID
				CONNECT BY PRIOR child_plate_id_fk = parent_plate_id_fk;
		ELSIF p_assetType = 'container' THEN
			OPEN O_RS FOR
				SELECT container_id AS pk,
					  barcode AS barcode,
					  location_id_fk,
					  guiutils.GETLOCATIONPATH(location_id_fk) AS locationPath,
					  parent_container_id_fk AS parent_id,
					  container_id AS child_id,
					  sys_connect_by_path(container_id, '_') AS id
				FROM inv_containers
				START WITH container_id = p_rootID
				CONNECT BY PRIOR container_id = parent_container_id_fk;

		END IF;
	END;

	/*
  given a comma delimited list of locations (regular loactions, racks, or rack positions)
  return the appropriate location_id either the regular location_id or the next open position
  */


	FUNCTION GetLocationId(p_locationIds VARCHAR2, p_containerId inv_containers.container_id%TYPE, p_plateId inv_plates.plate_id%TYPE, p_currLocationId inv_locations.location_id%TYPE)
		RETURN inv_locations.location_id%TYPE IS
  	l_locationIds_t Stringutils.t_char;
    l_count INT;
    l_rackPos inv_locations.location_id%TYPE;
    l_nextOpen inv_locations.location_id%TYPE;

		l_locationId inv_locations.location_id%TYPE;
	BEGIN
  		l_locationIds_t := STRINGUTILS.split(p_locationIds,',');

      FOR i IN l_locationIds_t.FIRST..l_locationIds_t.LAST
      LOOP
      	--' check to see if this is a rack
        IF (racks.isRack(l_locationIds_t(i)) = 1) THEN
					l_nextOpen := racks.first_open_position(l_locationIds_t(i), p_containerId, p_plateId, p_currLocationId);
				--' check to see if this is a rack position
				ELSIF (racks.isRackLocation(l_locationIds_t(i)) = 1) THEN
          l_nextOpen := racks.getNextOpenPosition(l_locationIds_t(i), p_containerId, p_plateId, p_currLocationId);
				--' otherwise it is a regular location
        ELSE
        	l_nextOpen := l_locationIds_t(i);
        END IF;
        IF l_nextOpen <> -1 THEN
        	RETURN l_nextOpen;
        END IF;
      END LOOP;
	  RETURN 4;	-- Missing location, at least we'll have the object show up in case a valid location cannot be obtained
	END GetLocationId;

	/*
     Purpose: In the GUI, rack locations aren't show in the tree.  This checks
     		if this is a rack position and returns the rack location if it is.
     */
	FUNCTION GetRefreshLocationID(p_locationId inv_locations.location_id%TYPE)
     RETURN inv_locations.location_id%TYPE IS
     	l_parentId inv_locations.parent_id%TYPE;
     BEGIN
     	IF racks.isRackLocation(p_locationId) = 1 THEN
     		SELECT parent_id INTO l_parentId FROM inv_locations WHERE location_id = p_locationId;
               RETURN l_parentId;
          ELSE
          	RETURN p_locationId;
          END IF;

     END GetRefreshLocationID;

	FUNCTION UseGetLocation(p_locationIds VARCHAR2)
  RETURN NUMBER IS
  	l_locationIds_t Stringutils.t_char;
    l_useGetLocation NUMBER(4) := 0;
  BEGIN
  		l_locationIds_t := STRINGUTILS.split(p_locationIds,',');

      FOR i IN l_locationIds_t.FIRST..l_locationIds_t.LAST
      LOOP
      	--' check to see if this is a rack
        IF (racks.isRack(l_locationIds_t(i)) = 1) THEN
					l_useGetLocation := 1;
				--' check to see if this is a rack position
				ELSIF (racks.isRackLocation(l_locationIds_t(i)) = 1) THEN
					l_useGetLocation := 1;
        END IF;
      END LOOP;

      RETURN l_useGetLocation;
  END UseGetLocation;

	PROCEDURE GETLABELPRINTER(	pLabelPrinterID IN INV_LABEL_PRINTERS.LABEL_PRINTER_ID%TYPE,	
						O_RS OUT CURSOR_TYPE)
	IS
	BEGIN	
		OPEN O_RS FOR 
		SELECT inv_label_printers.server_name, inv_label_printers.queue_name, inv_label_printers.display_name, inv_label_printers.lpr_definition_id_fk, 
		inv_label_printers.reporttype_id_fk, inv_lpr_definition.header_filename, inv_lpr_definition.label_filename, inv_lpr_definition.footer_filename
		FROM cheminvdb2.inv_label_printers, cheminvdb2.inv_lpr_definition
		WHERE cheminvdb2.inv_label_printers.label_printer_id = pLabelPrinterID
		AND cheminvdb2.inv_lpr_definition.lpr_definition_id = cheminvdb2.inv_label_printers.lpr_definition_id_fk;
	END GETLABELPRINTER;

	PROCEDURE GETLABELPRINTERS(	pReportTypeID IN INV_LABEL_PRINTERS.REPORTTYPE_ID_FK%TYPE, O_RS OUT CURSOR_TYPE )
	IS
	BEGIN	
		OPEN O_RS FOR 
		SELECT inv_label_printers.label_printer_id, inv_label_printers.display_name
		FROM cheminvdb2.inv_label_printers
		WHERE cheminvdb2.inv_label_printers.reporttype_id_fk = pReportTypeID
		AND cheminvdb2.inv_label_printers.is_available = 1;
	END GETLABELPRINTERS;

PROCEDURE GETFKReference(   pColumnName IN  user_cons_columns.Column_name%TYPE, pTableName IN  user_cons_columns.Table_name%TYPE, pOwner IN  user_cons_columns.Owner%TYPE,  O_RS OUT CURSOR_TYPE  )
   IS
	vPerentTablename varchar2(100);
	vPerentcolumnname varchar2(100);
	vSql varchar(1000);
  BEGIN
  select table_name into vPerentTablename from user_constraints where constraint_name=(
  select r_constraint_name from user_constraints where constraint_name=(
  select constraint_name from user_cons_columns where Column_name=pColumnName and Table_name=pTableName and Owner=pOwner));
  
  select a.column_name into vPerentcolumnname from 
  user_cons_columns a, user_constraints b
  where a.constraint_name = b.constraint_name 
  and a.table_name = b.table_name and b.table_name=vPerentTablename
  and b.constraint_type = 'P'; 
 
  vSql:='select '|| vPerentcolumnname ||' AS DisplayText,'|| vPerentcolumnname ||' AS Value from '|| vPerentTablename ;
 
  OPEN O_RS FOR vSql;
END GETFKReference ;

END GUIUTILS;
/
show errors;