-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved
prompt 'Creating chemical structure index ...'


Connect &&schemaName/&&schemaPass@&&serverName

DECLARE
	PROCEDURE createCartridgeIndex(iName IN varchar2, tName IN varchar2, fName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from user_indexes where Upper(index_name) = iName AND Upper(table_owner)= '&&schemaName';
			if n = 1 then
				execute immediate 'DROP INDEX '||iName||' force';
			end if;
			execute immediate 'CREATE INDEX '||iName|| ' ON ' || tName || '('||fName||')
								indexType is cscartridge.moleculeindextype
								PARAMETERS(''SKIP_POPULATING=YES,TABLESPACE=&&cscartTableSpaceName'')';
		END createCartridgeIndex;
BEGIN
	createCartridgeIndex('MX', 'inv_compounds', 'base64_cdx');
END;
/

DECLARE
	Procedure populateCartridgeIndex(iName IN varchar2, tName IN varchar2, fName IN varchar2) IS
			vPKColumnName VARCHAR2(100);
    		vMinPKValue NUMBER;
			vMaxPKValue NUMBER;
			vLowerPKBound NUMBER;
			vUpperPKBound NUMBER;
			vPKChunkValue NUMBER := 1000;
		BEGIN
		    SELECT COLUMN_NAME INTO vPKColumnName FROM user_cons_columns cc, user_constraints c WHERE cc.table_name = c.table_name and cc.constraint_name = c.constraint_name and cc.table_name = upper(tName) and constraint_type = 'P';
		    EXECUTE IMMEDIATE 'SELECT min(' || vPKColumnName || ') FROM ' || tName INTO vMinPKValue;
		    EXECUTE IMMEDIATE 'SELECT max(' || vPKColumnName || ') FROM ' || tName INTO vMaxPKValue;
			vLowerPKBound := vMinPKValue;
			vUpperPKBound := vLowerPKBound + vPKChunkValue;
		    WHILE vLowerPKBound < vMaxPKValue
		    LOOP
				EXECUTE IMMEDIATE 'ALTER INDEX ' || iName || ' PARAMETERS(''SUSPEND'')';
				EXECUTE IMMEDIATE 'UPDATE ' || tName || ' SET ' || fName || ' = ' || fname || ' WHERE ' || vPKColumnName  || ' BETWEEN ' || vLowerPKBound || ' AND ' || vUpperPKBound;
				EXECUTE IMMEDIATE 'ALTER INDEX ' || iName || ' PARAMETERS(''RESUME'')';
				COMMIT;
				vLowerPKBound := vUpperPKBound + 1;
				vUpperPKBound := vLowerPKBound + vPKChunkValue;
		    END LOOP;
		END populateCartridgeIndex;

BEGIN
	populateCartridgeIndex('MX', 'inv_compounds', 'base64_cdx');
END;
/
