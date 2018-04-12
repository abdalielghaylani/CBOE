-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Drop cs_security privilege table
--######################################################### 

prompt '#########################################################'
prompt 'dropping cs_security privilege table...' 
prompt '#########################################################'

connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from user_tables where table_name = Upper('&&privTableName');
	if n = 1 then
		execute immediate '
		DROP TABLE &&privTableName CASCADE CONSTRAINTS';
	end if;
END;
/
show errors;