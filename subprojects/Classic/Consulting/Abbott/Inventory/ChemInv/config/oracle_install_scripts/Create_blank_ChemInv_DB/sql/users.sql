-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

prompt starting users.sql

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = Upper('&&schemaName');
	if n > 0 then
		execute immediate 'DROP USER &&schemaName CASCADE';
	end if;
END;
/

CREATE USER &&schemaName
	IDENTIFIED BY ORACLE
	DEFAULT TABLESPACE &&tableSpaceName
	TEMPORARY TABLESPACE &&tempTableSpaceName;

GRANT CONNECT, RESOURCE TO  &&schemaName;
GRANT CREATE SEQUENCE TO &&SchemaName;


connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
grant select on people to &&SchemaName;
GRANT EXECUTE ON GrantOnCoreTableToAllRoles to &&SchemaName;


-- the inv_schema owner needs to get to reg tables during Access pass-through queries
DECLARE
	PROCEDURE grantOnTable(tOwner in varchar2, tName in varchar2, priv in varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_tables where Upper(owner) = Upper(tOwner) AND Upper(table_name)= Upper(tName);
			if n = 1 then
				execute immediate 'GRANT '||priv||' ON '||tOwner||'.'||tName||' TO &&SchemaName';
			end if;
		END grantOnTable;
BEGIN
	grantOnTable('REGDB', 'reg_numbers', 'select');
	grantOnTable('REGDB', 'batches', 'select');
	grantOnTable('REGDB', 'alt_ids', 'select');
	grantOnTable('REGDB', 'compound_molecule', 'select');  
END;
/



 



