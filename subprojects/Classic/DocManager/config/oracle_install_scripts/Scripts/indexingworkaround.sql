
prompt 'Creating chemical structure index...'

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
								PARAMETERS(''SKIP_POPULATING=YES'',''TABLESPACE=&&cscartTableSpaceName'')';
		END createCartridgeIndex;
BEGIN
	createCartridgeIndex('mx', 'structures', 'base64_cdx');
	createCartridgeIndex('mx2', 'temporary_structures', 'base64_cdx');
END;
/


ALTER INDEX mx2 PARAMETERS('SUSPEND');
prompt 'Updating temporary structure index.  This may take a very long time...'
UPDATE temporary_structures SET base64_cdx = base64_cdx; 
ALTER INDEX mx2 PARAMETERS('RESUME');
COMMIT;


ALTER INDEX mx PARAMETERS('SUSPEND');
prompt 'Updating registered structure index.  This may take a very long time...'
UPDATE structures SET base64_cdx = base64_cdx; 
ALTER INDEX mx PARAMETERS('RESUME');
COMMIT;

