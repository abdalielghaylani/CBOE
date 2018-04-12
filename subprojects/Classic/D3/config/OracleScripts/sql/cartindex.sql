Connect &&InstallUser/&&sysPass@&&serverName ;

ALTER USER CSCARTRIDGE QUOTA UNLIMITED ON &&cscartTableSpaceName;

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
								PARAMETERS(''TABLESPACE=&&cscartTableSpaceName'')';
		END createCartridgeIndex;
BEGIN
	createCartridgeIndex('mxd3', 'D3DATA.DRUGDEG_BASE64', 'base64_cdx');
END;
/



prompt 'Synchronizing the index'
ALTER INDEX MXD3 PARAMETERS('SYNCHRONIZE');

prompt 'Computing Index statistics'
ANALYZE INDEX MXD3 COMPUTE STATISTICS;