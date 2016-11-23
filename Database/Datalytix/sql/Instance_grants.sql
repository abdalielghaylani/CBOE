-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

PROMPT Starting Instance_grants.sql

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

--grants permissions for Global user

GRANT CREATE SESSION TO &&globalSchemaName;
GRANT SELECT ANY DICTIONARY TO &&globalSchemaName;
GRANT SELECT ANY TABLE TO &&globalSchemaName;
--GRANT SELECT ANY SEQUENCE TO &&globalSchemaName;
--GRANT ALTER USER TO &&globalSchemaName;
GRANT UNLIMITED TABLESPACE TO &&globalSchemaName;
--GRANT DELETE ANY TABLE TO &&globalSchemaName;
--GRANT DROP ANY ROLE TO &&globalSchemaName;
--GRANT DELETE ANY TABLE TO &&globalSchemaName;
-- Required when enable the direct Cartridge.
GRANT CREATE SYNONYM TO &&globalSchemaName;
-- Required when execute DBMS_JOB to clean up hitlist.
--GRANT EXECUTE ANY PROCEDURE TO &&globalSchemaName;

COMMIT;