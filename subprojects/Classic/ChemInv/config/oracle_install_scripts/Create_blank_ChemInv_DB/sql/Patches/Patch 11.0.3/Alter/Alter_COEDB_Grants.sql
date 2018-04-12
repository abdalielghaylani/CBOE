PROMPT Starting Alter_COEDB_Grants.sql

-- Execute as SYSTEM/DBA
-- Grant privileges on COEDB needed by Inventory for Authority

GRANT SELECT,REFERENCES ON &securitySchemaName..COEPRINCIPAL TO &schemaname;
--GRANT SELECT ON &&securitySchemaName..COEAPP TO &schemaname;
GRANT SELECT ON &&securitySchemaName..COEGROUPORG TO &schemaname;
GRANT SELECT ON &&securitySchemaName..COEGROUP TO &schemaname;
GRANT SELECT ON &&securitySchemaName..PEOPLE TO &schemaname;
GRANT SELECT ON &&securitySchemaName..COEGROUPPEOPLE TO &schemaname;
 
--Grant Privleges to public on the coedb objects
GRANT SELECT ON &&securitySchemaName..PEOPLE TO public;
GRANT SELECT ON &&securitySchemaName..COEGROUP TO  public;
GRANT SELECT,REFERENCES ON &securitySchemaName..COEPRINCIPAL TO  public;
GRANT SELECT ON &&securitySchemaName..COEGROUPPEOPLE TO  public;

