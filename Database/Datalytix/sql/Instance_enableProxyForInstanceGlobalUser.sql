-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.
-- This script enable proxy for instance global user.

prompt 
prompt Starting "Instance_enableProxyForInstanceGlobalUser.sql"...
prompt

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

VARIABLE proxy_script_name VARCHAR2(50)
COLUMN :proxy_script_name NEW_VALUE proxy_script_file NOPRINT; 

-- Check if COEDB was installed
DECLARE
  FlagSchema Number;
BEGIN
    SELECT count(1) 
        INTO FlagSchema
        FROM DBA_USERS
        WHERE UPPER(USERNAME)=UPPER('&schemaName');
    IF FlagSchema>0 THEN
		:proxy_script_name := 'Instance_doEnableProxyForInstanceGlobalUser.sql';
    ELSE
        :proxy_script_name := 'Instance_skipProxyForInstanceGlobalUser.sql';
    END IF;
END;
/

SELECT :proxy_script_name FROM DUAL;
@@&proxy_script_file
