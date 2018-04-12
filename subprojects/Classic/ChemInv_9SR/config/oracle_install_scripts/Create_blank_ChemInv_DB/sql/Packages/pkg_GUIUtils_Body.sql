CREATE OR REPLACE PACKAGE BODY "GUIUTILS"      AS

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

	FUNCTION IS_NUMBER(p VARCHAR2) 
  RETURN NUMBER 
  IS
  	l_num NUMBER;
  BEGIN
  	l_num := p;
    RETURN 1;
  EXCEPTION
  WHEN OTHERS THEN 
  	RETURN 0;
  
  END IS_NUMBER;
  
	
	  PROCEDURE GetRootNodes
	    (p_selectedID IN NUMBER,
	     p_assetType IN VARCHAR2,
	     O_RS OUT CURSOR_TYPE) 
						IS
						l_parentCount inv_plate_parent.parent_plate_id_fk%TYPE;
						l_childCount inv_plate_parent.child_plate_id_fk%TYPE;
			BEGIN
  				IF p_assetType = 'plate' THEN
  								--' determine the number of parent plates
  							SELECT COUNT(*)  INTO l_parentCount FROM inv_plate_parent WHERE child_plate_id_fk = p_selectedID;
  							--' determine the number of child plates
  							SELECT COUNT(*)  INTO l_childCount FROM inv_plate_parent WHERE parent_plate_id_fk = p_selectedID;
									
										--' if the plate is a root plate (no parents) return this plate ID
										IF l_parentCount = 0 AND l_childCount  > 0 THEN
    				 				OPEN O_RS FOR 
            						SELECT plate_id AS id, plate_barcode AS displayText FROM inv_plates WHERE plate_id = p_selectedID;
									ELSE
    				 				OPEN O_RS FOR 
																		SELECT plate_id AS id, plate_barcode AS displayText
																									FROM inv_plates
																									WHERE plate_id IN
																										(SELECT DISTINCT parent_plate_id_fk
																																		FROM inv_plate_parent
																																		WHERE parent_plate_id_fk NOT IN
																																		(SELECT child_plate_id_fk FROM inv_plate_parent)
																																		START WITH child_plate_id_fk = p_selectedid
																																		CONNECT BY PRIOR parent_plate_id_fk = child_plate_id_fk);
							   END IF;																		
						 ELSIF p_assetType = 'container' THEN
										--' containers can only have 0 or 1 parent container
										SELECT COUNT(*) INTO l_parentCount FROM inv_containers WHERE parent_container_id_fk IS not NULL AND container_id = p_selectedID;
										--' determine the number of child plates
										SELECT COUNT(*) INTO l_childCount FROM inv_containers WHERE parent_container_id_fk = p_selectedID;
										
										IF		l_parentCount = 0 AND l_childCount > 0 THEN
														OPEN O_RS FOR
																			SELECT container_id AS id, barcode AS displayText FROM inv_containers WHERE container_id = p_selectedID;
									ELSE
													OPEN O_RS FOR
																		SELECT container_id AS id, barcode AS displayText 
																									FROM inv_containers
																									WHERE parent_container_id_fk IS NULL
																									AND container_id IN
																									(SELECT container_id FROM inv_containers 
																									START WITH container_id = p_selectedID
																									CONNECT BY PRIOR parent_container_id_fk = container_id);
								  END IF;
							END IF;	
			END GetRootNodes;
	
	
	  PROCEDURE GetLineage
	    (p_rootID IN NUMBER,
	     p_assetType IN VARCHAR2,
	     O_RS OUT CURSOR_TYPE) 
						IS
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
            CONNECT BY PRIOR child_plate_id_fk = parent_plate_id_fk				;
								ELSIF p_assetType = 'container' THEN
														OPEN O_RS FOR
																			SELECT container_id AS pk,
																										barcode AS barcode,
																										location_id_fk,
																										guiutils.GETLOCATIONPATH(location_id_fk) AS locationPath,
																										parent_container_id_fk AS parent_id,
																										container_id AS child_id,
																										sys_connect_by_path(container_id,'_') AS id
																			FROM inv_containers
																			START WITH container_id = p_rootID
																			CONNECT BY PRIOR container_id = parent_container_id_fk;
																										
														
								END IF;
			 END;

END GUIUTILS;
/
show errors;