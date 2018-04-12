CREATE OR REPLACE PACKAGE BODY "GUIUTILS"AS

  PROCEDURE GETKEYCONTAINERATTRIBUTES
	    (pContainerIDList IN  varchar2:=NULL,
	     pContainerBarcodeList IN varchar2:=NULL,
	     O_RS OUT CURSOR_TYPE) IS
   my_sql varchar2(2000);
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
	           FROM inv_containers c, inv_locations l, cs_security.people p, inv_units u
	           WHERE c.location_id_fk = l.location_id
	           AND c.unit_of_meas_id_fk = u.unit_id
	           AND upper(c.current_user_id_fk) = p.user_id(+)';
	  if pContainerIDList is NOT NULL then
	     my_sql := my_sql || 'AND Container_ID IN(' || pContainerIDList || ')';
	  else
	     my_sql := my_sql || 'AND Barcode IN(' || pContainerBarcodeList || ')';
    end if;
       my_sql := my_sql || ' ORDER BY l.location_name ASC';
    OPEN O_RS FOR
      my_sql;
	END GETKEYCONTAINERATTRIBUTES;

    PROCEDURE GETKEYPLATEATTRIBUTES
  	    (pPlateIDList IN  varchar2:=NULL,
  	     pPlateBarcodeList IN varchar2:=NULL,
  	     O_RS OUT CURSOR_TYPE) IS
     my_sql varchar2(2000);
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
  	  if pPlateIDList is NOT NULL then
  	     my_sql := my_sql || 'AND plate_ID IN(' || pPlateIDList || ')';
  	  else
  	     my_sql := my_sql || 'AND plate_barcode IN(' || pPlateBarcodeList || ')';
      end if;
         my_sql := my_sql || ' ORDER BY l.location_name ASC';
      OPEN O_RS FOR
        my_sql;
  	END GETKEYPLATEATTRIBUTES;


    PROCEDURE DISPLAYCREATEDCONTAINERS
      (p_ContainerList IN  varchar2:=NULL,
      O_RS OUT CURSOR_TYPE) IS
      my_sql varchar2(3000);
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

        OPEN O_RS FOR
            my_sql;
    END DISPLAYCREATEDCONTAINERS;
    
	 FUNCTION GETLOCATIONPATH
	     (pLocationID IN inv_locations.location_id%type) return varchar2 IS

	 CURSOR LocationNodes_cur(LocationID_in in inv_locations.location_id%type) IS
    SELECT Location_Name
    FROM inv_Locations
    CONNECT BY Location_id = prior Parent_id
    START WITH Location_id = LocationID_in
    ORDER BY Level DESC;
    path_str varchar2(2000);
    locationName varchar2(200);
  BEGIN
    OPEN LocationNodes_cur(pLocationID);
    LOOP
      FETCH LocationNodes_cur INTO locationName;
      EXIT WHEN LocationNodes_cur%NOTFOUND;
      path_str := path_str || locationName || '\';
    END LOOP;
    --dbms_output.put_line(path_str);
    RETURN path_str;
  END GETLOCATIONPATH;

  FUNCTION GETRACKLOCATIONPATH
	     (pLocationID IN inv_locations.location_id%type) return varchar2 IS

	 CURSOR LocationNodes_cur(LocationID_in in inv_locations.location_id%type) IS
    SELECT Location_Name, collapse_child_nodes
    FROM inv_Locations
    CONNECT BY Location_id = prior Parent_id
    START WITH Location_id = LocationID_in
    ORDER BY Level DESC;
    path_str varchar2(2000);
    locationName varchar2(200);
    collapseChildNodes inv_locations.collapse_child_nodes%type;
  BEGIN
    OPEN LocationNodes_cur(pLocationID);
    LOOP
      FETCH LocationNodes_cur INTO locationName, collapseChildNodes;
      EXIT WHEN LocationNodes_cur%NOTFOUND;
      If collapseChildNodes is not null then
        path_str := path_str || locationName || '\';
      end if;
    END LOOP;
    --dbms_output.put_line(path_str);
    RETURN path_str;
  END GETRACKLOCATIONPATH;

  FUNCTION GETFULLRACKLOCATIONPATH
	     (pLocationID IN inv_locations.location_id%type) return varchar2 IS

    CURSOR LocationNodes_cur(LocationID_in in inv_locations.location_id%type) IS
    SELECT Location_Name, collapse_child_nodes, Location_ID
    FROM inv_Locations
    CONNECT BY Location_id = prior Parent_id
    START WITH Location_id = LocationID_in
    ORDER BY Level DESC;
    path_str varchar2(2000);
    locationName varchar2(200);
    collapseChildNodes inv_locations.collapse_child_nodes%type;
    locationID inv_locations.location_id%type;
  BEGIN
    OPEN LocationNodes_cur(pLocationID);
    LOOP
      FETCH LocationNodes_cur INTO locationName, collapseChildNodes, locationID;
      EXIT WHEN LocationNodes_cur%NOTFOUND;
      If locationID > 0 then
        path_str := path_str || locationName || '\';
      end if;
    END LOOP;
    --dbms_output.put_line(path_str);
    RETURN path_str;
  END GETFULLRACKLOCATIONPATH;

  PROCEDURE GETRECENTLOCATIONS(pContainerID IN inv_containers.container_id%type,
  				pNumRows in integer:=5,
				O_RS OUT CURSOR_TYPE) IS
	  mySQL varchar2(2000);
   BEGIN

       		mySQL:=	'SELECT c.raid, rid,
					getLocationPath(new_value) AS LocationPath, TimeStamp,
					(SELECT location_name from inv_locations WHERE location_id = new_value) AS LocationName,
				  	(SELECT Upper(old_value) from audit_column where column_name= ''CURRENT_USER_ID_FK'' AND RAID = c.raid) AS fromUser,
					(SELECT Upper(new_value) from audit_column where column_name= ''CURRENT_USER_ID_FK'' AND RAID = c.raid) AS toUser
			FROM audit_column c, audit_row r
			WHERE c.raid = r.raid
			AND column_Name = ''LOCATION_ID_FK''
			AND rid = (SELECT rid from inv_containers where container_ID = ' || pContainerID || ')
			AND rowNum < ' || pNumRows ||'
			Order by TimeStamp desc';
		Open O_RS for
			mySQL;
   END GETRECENTLOCATIONS;

	FUNCTION GETPARENTPLATEIDS (pPlateID inv_plates.plate_id%TYPE)
	RETURN varchar2
	IS
		vCount number;
		vParentPlateIDs varchar2(1000);
	BEGIN
		SELECT count(*) INTO vCount FROM inv_plate_parent WHERE child_plate_id_fk = pPlateID;
		IF vCount  > 0 THEN
			FOR parentPlateID_rec IN (SELECT parent_plate_id_fk FROM inv_plate_parent WHERE child_plate_id_fk = pPlateID)
			LOOP
				vParentPlateIDs := vParentPlateIDs || parentPlateID_rec.parent_plate_id_fk || ',';
			END LOOP;
			vParentPlateIDs := rTrim(vParentPlateIDs,',');
		--ELSE
		--	SELECT parent_plate_id_fk INTO vParentPlateIDs FROM inv_plates WHERE plate_id = pPlateID;
		END IF;

		--return '1';
		RETURN vParentPlateIDs;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATEIDS;


	FUNCTION GETPARENTPLATEBARCODES (pPlateID inv_plates.plate_id%TYPE)
	RETURN varchar2
	IS
		vParentPlateIDs varchar2(1000);
		vPlateIDList_t STRINGUTILS.t_char;
		vTempBarcode inv_plates.plate_barcode%TYPE;
		vParentPlateBarcodes varchar2(1000);
	BEGIN
		vParentPlateIDs := GetParentPlateIDs(pPlateID);

		IF vParentPlateIDs is null THEN
			vParentPlateBarcodes := null;
		ELSE
			vPlateIDList_t := STRINGUTILS.split(vParentPlateIDs, ',');
			FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
			LOOP
				SELECT plate_barcode INTO vTempBarcode FROM inv_plates WHERE plate_id = vPlateIDList_t(i);
				vParentPlateBarcodes := vParentPlateBarcodes || vTempBarcode || ',';
			END LOOP;
			vParentPlateBarcodes := rTrim(vParentPlateBarcodes,',');
		END IF;

		RETURN vParentPlateBarcodes;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;
	END GETPARENTPLATEBARCODES;

	FUNCTION GETPARENTPLATELOCATIONIDS(pPlateID inv_plates.plate_id%TYPE)
	RETURN varchar2
	IS
		vParentPlateIDs varchar2(1000);
		vPlateIDList_t STRINGUTILS.t_char;
		vTempLocationID inv_locations.location_id%TYPE;
		vParentPlateLocationIDs varchar2(1000);
	BEGIN
		vParentPlateIDs := GetParentPlateIDs(pPlateID);
		IF vParentPlateIDs is null THEN
			vParentPlateLocationIDs := null;
		ELSE
			vPlateIDList_t := STRINGUTILS.split(vParentPlateIDs, ',');
			FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
			LOOP
				SELECT location_id  INTO vTempLocationID FROM inv_locations, inv_plates WHERE location_id = location_id_fk AND plate_id = vPlateIDList_t(i);
				vParentPlateLocationIDs := vParentPlateLocationIDs || vTempLocationID || ',';
			END LOOP;
			vParentPlateLocationIDs := rTrim(vParentPlateLocationIDs,',');
		END IF;

		RETURN vParentPlateLocationIDs;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATELOCATIONIDS;


	FUNCTION GETPARENTWELLIDS (pWellID inv_wells.well_id%TYPE)
	RETURN varchar2
	IS
		vCount number;
		vParentWellIDs varchar2(1000);
	BEGIN
		SELECT count(*) INTO vCount FROM inv_well_parent WHERE child_well_id_fk = pWellID;
		IF vCount  > 0 THEN
			FOR parentWellID_rec IN (SELECT parent_well_id_fk FROM inv_well_parent WHERE child_well_id_fk = pWellID)
			LOOP
				vParentWellIDs := vParentWellIDs || parentWellID_rec.parent_well_id_fk || ',';
			END LOOP;
			vParentWellIDs := rTrim(vParentWellIDs,',');
		--ELSE
		--	SELECT parent_well_id_fk INTO vParentWellIDs FROM inv_wells WHERE well_id = pWellID;
		END IF;

		--return '1';
		RETURN vParentWellIDs;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTWELLIDS;

	FUNCTION GETPARENTWELLLABELS (pWellID inv_wells.well_id%TYPE)
	RETURN varchar2
	IS
		vParentWellIDs varchar2(1000);
		vWellIDList_t STRINGUTILS.t_char;
		vTempLabel inv_plates.plate_barcode%TYPE;
		vParentWellLabels varchar2(1000);
	BEGIN
		vParentWellIDs := GetParentWellIDs(pWellID);

		IF vParentWellIDs is null THEN
			vParentWellLabels := null;
		ELSE
			vWellIDList_t := STRINGUTILS.split(vParentWellIDs, ',');
			FOR i in vWellIDList_t.First..vWellIDList_t.Last
			LOOP
				SELECT plate_barcode || '-' || name  INTO vTempLabel FROM inv_plates, inv_vw_well WHERE plate_id = plate_id_fk AND well_id =  vWellIDList_t(i);
				vParentWellLabels := vParentWellLabels || vTempLabel || ',';
			END LOOP;
			vParentWellLabels := rTrim(vParentWellLabels,',');
		END IF;

		RETURN vParentWellLabels;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;
	END GETPARENTWELLLABELS;

	FUNCTION GETPARENTPLATEIDS2 (pWellID inv_wells.well_id%TYPE)
	RETURN varchar2
	IS
		vParentWellIDs varchar2(1000);
		vWellIDList_t STRINGUTILS.t_char;
		vTempPlateID inv_plates.plate_id%TYPE;
		vParentPlateIDs varchar2(1000);
	BEGIN
		vParentWellIDs := GETPARENTWELLIDS(pWellID);
		IF vParentWellIDs is null THEN
			vParentPlateIDs := null;
		ELSE
			vWellIDList_t := STRINGUTILS.split(vParentWellIDs, ',');
			FOR i in vWellIDList_t.First..vWellIDList_t.Last
			LOOP
				SELECT plate_id_fk INTO vTempPlateID FROM  inv_wells WHERE  well_id =  vWellIDList_t(i);
				vParentPlateIDs := vParentPlateIDs || vTempPlateID || ',';
			END LOOP;
			vParentPlateIDs := rTrim(vParentPlateIDs,',');

		END IF;

		RETURN vParentPlateIDs;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATEIDS2;

	FUNCTION GETPARENTPLATELOCATIONIDS2(pWellID inv_wells.well_id%TYPE)
	RETURN varchar2
	IS
		vParentWellIDs varchar2(1000);
		vWellIDList_t STRINGUTILS.t_char;
		vTempLocationID inv_locations.location_id%TYPE;
		vParentPlateLocationIDs varchar2(1000);
	BEGIN
		vParentWellIDs := GETPARENTWELLIDS(pWellID);
		IF vParentWellIDs is null THEN
			vParentPlateLocationIDs := null;
		ELSE
			vWellIDList_t := STRINGUTILS.split(vParentWellIDs, ',');
			FOR i in vWellIDList_t.First..vWellIDList_t.Last
			LOOP
				SELECT location_id  INTO vTempLocationID FROM inv_locations, inv_plates, inv_wells WHERE location_id = location_id_fk AND plate_id = plate_id_fk AND well_id = vWellIDList_t(i);
				vParentPlateLocationIDs := vParentPlateLocationIDs || vTempLocationID || ',';
			END LOOP;
			vParentPlateLocationIDs := rTrim(vParentPlateLocationIDs,',');
		END IF;

		RETURN vParentPlateLocationIDs;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATELOCATIONIDS2;

	FUNCTION GETPARENTWELLNAMES (pWellID inv_wells.well_id%TYPE)
	RETURN varchar2
	IS
		vParentWellIDs varchar2(1000);
		vWellIDList_t STRINGUTILS.t_char;
		vName inv_plates.plate_barcode%TYPE;
		vParentWellLabels varchar2(1000);
	BEGIN
		vParentWellIDs := GetParentWellIDs(pWellID);

		IF vParentWellIDs is null THEN
			vParentWellLabels := null;
		ELSE
			vWellIDList_t := STRINGUTILS.split(vParentWellIDs, ',');
			FOR i in vWellIDList_t.First..vWellIDList_t.Last
			LOOP
				SELECT name INTO vName FROM inv_plates, inv_vw_well WHERE plate_id = plate_id_fk AND well_id =  vWellIDList_t(i);
				vParentWellLabels := vParentWellLabels || vName || ',';
			END LOOP;
			vParentWellLabels := rTrim(vParentWellLabels,',');
		END IF;

		RETURN vParentWellLabels;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;
	END GETPARENTWELLNAMES;

	FUNCTION GETPARENTPLATEBARCODES2 (pWellID inv_wells.well_id%TYPE)
	RETURN varchar2
	IS
		vParentPlateIDs varchar2(1000);
		vPlateIDList_t STRINGUTILS.t_char;
		vTempBarcode inv_plates.plate_barcode%TYPE;
		vParentPlateBarcodes varchar2(1000);
	BEGIN
		vParentPlateIDs := GETPARENTPLATEIDS2(pWellID);
		IF vParentPlateIDs is null THEN
			vParentPlateBarcodes := null;
		ELSE
			vPlateIDList_t := STRINGUTILS.split(vParentPlateIDs, ',');
			FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
			LOOP
				SELECT plate_barcode INTO vTempBarcode FROM inv_plates WHERE plate_id = vPlateIDList_t(i);
				vParentPlateBarcodes := vParentPlateBarcodes || vTempBarcode || ',';
			END LOOP;
			vParentPlateBarcodes := rTrim(vParentPlateBarcodes,',');
		END IF;

		RETURN vParentPlateBarcodes;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'Error: ' || SQLCODE || ':' || SQLERRM;

	END GETPARENTPLATEBARCODES2;

    
	FUNCTION GETBATCHAMOUNTSTRING (
             pContainerID inv_containers.container_id%TYPE
             ,p_DryWeightUOM VARCHAR2)
	RETURN varchar2
	IS
		vBatchAmountString varchar2(500) := '';
		vNoConvert varchar2(500) :='';
		vCurrUnitID inv_units.unit_id%TYPE;
		vCurrUnitAbbrev inv_units.unit_abreviation%TYPE;
		vTotal inv_containers.qty_available%TYPE;
		vConvertedQty inv_containers.qty_available%TYPE;
		vIsFirst boolean := true;
    l_batchID inv_container_batches.batch_id%TYPE := NULL;
	BEGIN
		--' get batch id
    SELECT batch_id_fk INTO l_batchID FROM inv_containers WHERE container_id = pContainerID;
    
    IF l_batchID IS NOT NULL THEN
   		FOR batch_rec IN (SELECT sum(qty_available*concentration) qty_available,  unit_of_meas_id_fk as UOMID, unit_abreviation FROM inv_containers, inv_units WHERE unit_id = unit_of_meas_id_fk AND batch_id_fk = l_batchID GROUP BY unit_of_meas_id_fk, unit_abreviation ORDER BY qty_available desc)
  		LOOP
  			IF vIsFirst THEN
  				vCurrUnitID := batch_rec.UOMID;
                IF p_DryWeightUOM is not null THEN
                   vCurrUnitAbbrev := p_DryWeightUOM;
                ELSE
                   vCurrUnitAbbrev := batch_rec.unit_abreviation;
                END IF;
  				vTotal := batch_rec.qty_available;
  				vIsFirst := false;
  			ELSE
  				--try to convert
  				vConvertedQty := chemcalcs.convert(batch_rec.qty_available, batch_rec.UOMID, vCurrUnitID);
  				IF vConvertedQty < 0 THEN
  					--couldn't convert
  					vNoConvert := vNoConvert || batch_rec.qty_available || ' ' || batch_rec.unit_abreviation || ',' ;
  				ELSE
  					vTotal := vTotal + vConvertedQty;
  				END IF;
  
  			END IF;
  		END LOOP;
		END IF;
      
		IF vBatchAmountString is null  AND (not vIsFirst) THEN
			vBatchAmountString := vTotal || ' ' || vCurrUnitAbbrev;
		END IF;
		-- append unconvertable quantities
		IF vNoConvert is not null THEN
			vBatchAmountString := vBatchAmountString || ',' || TRIM(',' FROM vNoConvert);
		END IF;
		RETURN vBatchAmountString;
		--RETURN vTotal;

	END GETBATCHAMOUNTSTRING;
    
/*
-- OLD FUNCTION

	FUNCTION GETBATCHAMOUNTSTRING (pContainerID inv_containers.container_id%TYPE)
	RETURN varchar2
	IS
		vBatchAmountString varchar2(500) := '';
		vNoConvert varchar2(500) :='';
		vCurrUnitID inv_units.unit_id%TYPE;
		vCurrUnitAbbrev inv_units.unit_abreviation%TYPE;
		vTotal inv_containers.qty_available%TYPE;
		vConvertedQty inv_containers.qty_available%TYPE;
		vIsFirst boolean := true;
	BEGIN

		FOR sibling_rec IN (SELECT sum(qty_available) qty_available,  unit_of_meas_id_fk as UOMID, unit_abreviation FROM inv_containers, inv_units WHERE unit_id = unit_of_meas_id_fk AND parent_container_id_fk = pContainerID GROUP BY unit_of_meas_id_fk, unit_abreviation ORDER BY qty_available desc)
		LOOP
			IF vIsFirst THEN
				vCurrUnitID := sibling_rec.UOMID;
				vCurrUnitAbbrev := sibling_rec.unit_abreviation;
				vTotal := sibling_rec.qty_available;
				vIsFirst := false;
			ELSE
				--try to convert
				vConvertedQty := chemcalcs.convert(sibling_rec.qty_available, sibling_rec.UOMID, vCurrUnitID);
				IF vConvertedQty < 0 THEN
					--couldn't convert
					vNoConvert := vNoConvert || sibling_rec.qty_available || ' ' || sibling_rec.unit_abreviation || ',' ;
				ELSE
					vTotal := vTotal + vConvertedQty;
				END IF;

			END IF;
		END LOOP;

		IF vBatchAmountString is null  AND (not vIsFirst) THEN
			vBatchAmountString := vTotal || ' ' || vCurrUnitAbbrev;
		END IF;
		-- append unconvertable quantities
		IF vNoConvert is not null THEN
			vBatchAmountString := vBatchAmountString || ',' || TRIM(',' FROM vNoConvert);
		END IF;
		RETURN vBatchAmountString;
		--RETURN vTotal;

	END GETBATCHAMOUNTSTRING;
*/

END GUIUTILS;




/
show errors;