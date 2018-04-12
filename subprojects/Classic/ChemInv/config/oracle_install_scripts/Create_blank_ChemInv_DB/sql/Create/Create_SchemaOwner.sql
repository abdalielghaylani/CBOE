-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Create schema owner
--######################################################### 

Connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'Creating schema owner...'
prompt '#########################################################'


CREATE USER &&schemaName
	IDENTIFIED BY &&schemaPass
	DEFAULT TABLESPACE &&tableSpaceName
	TEMPORARY TABLESPACE &&tempTableSpaceName;

GRANT CONNECT, RESOURCE TO  &&schemaName;
GRANT CREATE SEQUENCE TO &&SchemaName;
GRANT CREATE ANY VIEW TO &&SchemaName;
GRANT CREATE ANY INDEX TO &&schemaName;
GRANT CREATE ANY SNAPSHOT TO &&schemaName;

--' Grant core privileges to schema owner
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

Grant select on people to &&SchemaName;
GRANT EXECUTE ON GrantOnCoreTableToAllRoles to &&SchemaName;


DECLARE
	PROCEDURE grantOnTable(tOwner in varchar2, tName in varchar2, priv in varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_tables where Upper(owner) = Upper(tOwner) AND Upper(table_name)= Upper(tName);
			if n = 1 then
				execute immediate 'GRANT '||priv||' ON '||tOwner||'.'||tName||' TO &&SchemaName ';
			end if;
		END grantOnTable;
BEGIN
	grantOnTable('REGDB', 'reg_numbers', 'select');
	grantOnTable('REGDB', 'batches', 'select');
	grantOnTable('REGDB', 'alt_ids', 'select');
	grantOnTable('REGDB', 'compound_molecule', 'select');  
	grantOnTable('REGDB', 'notebooks', 'select');  
END;
/

show errors;


 



