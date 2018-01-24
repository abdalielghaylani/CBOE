--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

prompt 
prompt Starting "GrantsUpgrade.sql"...
prompt 

--#########################################################
--GRANTS AND USERS 11.0.1
--######################################################### 

GRANT EXECUTE ON COEDB.CONFIGURATIONMANAGER TO &&schemaName;
GRANT CREATE VIEW TO &&schemaName;

GRANT CONNECT, RESOURCE TO  &&schemaName;
GRANT CREATE VIEW TO &&schemaName;
GRANT EXECUTE ON &&securitySchemaName..CONFIGURATIONMANAGER TO &&schemaName;
GRANT SELECT ANY TABLE TO &&schemaName;


ALTER USER &&schemaName GRANT CONNECT THROUGH COEUSER;
GRANT CREATE ANY INDEX TO &&schemaName;
GRANT CREATE ANY SNAPSHOT TO &&schemaName;
GRANT CREATE ANY VIEW TO &&schemaName;


ALTER USER &&cartSchemaName QUOTA UNLIMITED ON &&cscartTableSpaceName;

UPDATE COEDB.PRIVILEGE_TABLES SET TABLE_SPACE='T_COEDB' WHERE TABLE_SPACE='T_CS_SECURITY';
DELETE COEDB.PRIVILEGE_TABLES where PRIVILEGE_TABLE_NAME='OTHER_PRIVILEGES';
COMMIT;
