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

    RETURN vReturn;

exception
WHEN invalid_table then
	RETURN 'Error: Invalid table specified';   
WHEN others THEN
	RETURN 'Error:' || SQLCODE || ':' || SQLERRM ;
END;
/
