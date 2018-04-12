CREATE OR REPLACE
FUNCTION "GETPRIMARYKEYIDS"
  (pTableName varchar2,
  pTableValues varchar2)
RETURN varchar2
IS
/*
	Purpose: To look up fields in specifc tables and return a list of primary key values for those that
	match.  The table values must be of a unique constraint type.  pTableName must exist within the database
	and pTableValues should be a comma-delimited sequence of unique values.
*/

vCount int;
vReturn varchar2(32000) := '';
vValueIDs_t STRINGUTILS.t_char;
invalid_table exception;
vPlateID NUMBER(9,0);
vContainerID NUMBER(9,0);
vLocationID NUMBER(9,0);

BEGIN
	-- Validate the table
	SELECT count(*) INTO vCount FROM user_tables WHERE table_name = upper(pTableName);
	IF vCount = 0 THEN
		 RAISE invalid_table;
	END IF;

	vValueIDs_t := STRINGUTILS.split(pTableValues, ',');	
	
	-- Lookup the values	
	IF lower(pTableName) = 'inv_plates' THEN
		FOR i in vValueIDs_t.First..vValueIDs_t.Last
		LOOP
			BEGIN
				SELECT plate_id INTO vPlateID FROM inv_plates WHERE plate_barcode = vValueIDs_t(i);
				vReturn := vReturn  || vValueIDs_t(i) || '=' || vPlateID || ',';				
			EXCEPTION
				WHEN NO_DATA_FOUND THEN vPlateID := 0;
			END;
		END LOOP;
	END IF;
	IF lower(pTableName) = 'inv_containers' THEN
		FOR i in vValueIDs_t.First..vValueIDs_t.Last
		LOOP
			BEGIN
				SELECT CONTAINER_ID INTO vContainerID FROM inv_containers WHERE barcode = vValueIDs_t(i);
				vReturn := vReturn  || vValueIDs_t(i) || '=' || vContainerID || ',';
			EXCEPTION
				WHEN NO_DATA_FOUND THEN vContainerID := 0;
			END;
		END LOOP;
	END IF;
	IF lower(pTableName) = 'inv_locations' THEN
		FOR i in vValueIDs_t.First..vValueIDs_t.Last
		LOOP
			BEGIN
				SELECT LOCATION_ID INTO vLocationID FROM inv_locations WHERE location_barcode = vValueIDs_t(i);
				vReturn := vReturn  || vValueIDs_t(i) || '=' || vLocationID || ',';
			EXCEPTION
				WHEN NO_DATA_FOUND THEN vLocationID := 0;
			END;
		END LOOP;
	END IF;
	IF LENGTH(vReturn) IS NULL THEN
		vReturn := 'NOT FOUND';
	END IF;
	RETURN TRIM(',' FROM vReturn);

EXCEPTION
WHEN invalid_table THEN
	RETURN 'Error: Invalid table specified';   
WHEN others THEN
	RETURN 'Error:' || SQLCODE || ':' || SQLERRM ;
END;
/
