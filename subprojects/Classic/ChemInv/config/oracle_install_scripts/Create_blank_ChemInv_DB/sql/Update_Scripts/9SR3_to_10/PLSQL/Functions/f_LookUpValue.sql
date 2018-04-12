CREATE OR REPLACE
FUNCTION "LOOKUPVALUE"
  (pTableName varchar2,
  pTableValue varchar2,
  pInsertIfNotFound varchar2)
RETURN varchar2
IS
/*
	Purpose:  to look up fields in specifc tables and insert that field if specified.  It is used mainly in import processes.
*/

vSQL varchar2(500);
vCount int;
vID number;
vReturn varchar2(100);
invalid_table exception;

BEGIN

	--validate the table
	SELECT count(*) INTO vCount FROM user_tables WHERE table_name = upper(pTableName) ;
	IF vCount = 0 THEN
		 RAISE invalid_table;
	END IF;       
	vCount := 0;
	vReturn := 'NOT FOUND';    
	
	--lookup the value
	IF lower(pTableName) = 'inv_solvents' THEN
			SELECT count(*) INTO vCount FROM inv_solvents WHERE lower(solvent_name) = lower(pTableValue);
			IF vCount > 0 THEN
				SELECT solvent_id INTO vReturn FROM inv_solvents WHERE lower(solvent_name) = lower(pTableValue);
			ELSIF vCount = 0 AND lower(pInsertIfNotFound) = 'true' THEN
				SELECT count(*) INTO vCount FROM inv_compounds WHERE lower(substance_name) = lower(pTableValue);	
				IF vCount > 0 THEN
				    SELECT compound_id INTO vID FROM inv_compounds WHERE lower(substance_name) = lower(pTableValue);	
				ELSIF vCount = 0 THEN                                 
					vID := null;					
				END IF;
			    INSERT INTO inv_solvents VALUES (NULL,pTableValue,vID) RETURNING solvent_ID into vReturn;
			END IF;		
	END IF;   

	IF lower(pTableName) = 'inv_units' THEN
			SELECT count(*) INTO vCount FROM inv_units WHERE unit_abreviation = pTableValue;
			IF vCount > 0 THEN
				SELECT unit_id INTO vReturn FROM inv_units WHERE unit_abreviation = pTableValue;
			END IF;
			-- pInsertIfNotFound does not apply to this table
	END IF;   

    IF lower(pTableName) = 'inv_container_types' THEN
			SELECT count(*) INTO vCount FROM inv_container_types WHERE lower(container_type_name) = lower(pTableValue);
			IF vCount > 0 THEN
				SELECT container_type_id INTO vReturn FROM inv_container_types WHERE lower(container_type_name) = lower(pTableValue);			
			END IF;
			-- pInsertIfNotFound does not apply to this table
	END IF;
	IF lower(pTableName) = 'inv_container_status' THEN
			SELECT count(*) INTO vCount FROM inv_container_status WHERE lower(container_status_name) = lower(pTableValue);
			IF vCount > 0 THEN
				SELECT container_status_id INTO vReturn FROM inv_container_status WHERE lower(container_status_name) = lower(pTableValue);			
			END IF;
			-- pInsertIfNotFound does not apply to this table
	END IF;
	IF lower(pTableName) = 'inv_suppliers' THEN
			SELECT count(*) INTO vCount FROM inv_suppliers WHERE lower(supplier_short_name) = lower(pTableValue);
			IF vCount > 0 THEN
				SELECT supplier_id INTO vReturn FROM inv_suppliers WHERE lower(supplier_short_name) = lower(pTableValue);			
			END IF;
			-- pInsertIfNotFound does not apply to this table
	END IF;
	IF lower(pTableName) = 'inv_owners' THEN
			SELECT count(*) INTO vCount FROM inv_owners WHERE lower(description) = lower(pTableValue);
			IF vCount > 0 THEN
				SELECT owner_id INTO vReturn FROM inv_owners WHERE lower(description) = lower(pTableValue);			
			END IF;
			-- pInsertIfNotFound does not apply to this table
	END IF;
	IF lower(pTableName) = 'inv_locations' THEN
			SELECT count(*) INTO vCount FROM inv_locations WHERE lower(location_barcode) = lower(pTableValue);
			IF vCount > 0 THEN
				SELECT location_id INTO vReturn FROM inv_locations WHERE lower(location_barcode) = lower(pTableValue);
			END IF;
			-- pInsertIfNotFound does not apply to this table
	END IF;
  IF lower(pTableName) = 'inv_reporttypes' THEN
			SELECT count(*) INTO vCount FROM inv_reporttypes WHERE lower(reporttypedesc) = lower(pTableValue);
			IF vCount > 0 THEN
				SELECT reporttype_id INTO vReturn FROM inv_reporttypes WHERE lower(reporttypedesc) = lower(pTableValue);
			END IF;
			-- pInsertIfNotFound does not apply to this table
	END IF;
    RETURN vReturn;

exception
WHEN invalid_table then
	RETURN 'Error: Invalid table specified';   
WHEN others THEN
	RETURN 'Error:' || SQLCODE || ':' || SQLERRM ;
END;
/
