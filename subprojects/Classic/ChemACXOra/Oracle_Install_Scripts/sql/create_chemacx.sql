--Copyright Cambridgesoft corp 1999-2005 all rights reserved

spool sql\log_create_chemacx.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql

@@alter_system&&alter_qre
alter user cscartridge quota unlimited on &&cscartTableSpaceName;    

Connect &&schemaName/&&schemaPass@&&serverName
@@globals.sql
@@tables.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql

--Create ACX_ roles/users without cs_security schema
@@Create_ACX_roles.sql


Connect &&InstallUser/&&sysPass@&&serverName
@@importdata&&importdata

--Update CS_SECURITY schema if needed
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName
@@alter_cs_security_for_chemacx_ora&&alter_sec
@@create_chemacx_test_users&&alter_sec

--create CS_Cartridge index
Connect &&schemaName/&&schemaPass@&&serverName

PROMPT =========================================================
PROMPT = Next three statements will create Cartridge Index.    =
PROMPT = It can take up to several hours depending on a server = 
PROMPT = Do not close this window.                             =  
PROMPT =========================================================

create index mx on chemacxdb.substance(base64_cdx) INDEXTYPE IS CsCartridge.MoleculeIndexType PARAMETERS('TABLESPACE=T_chemacxdb_CSCART');

PROMPT =========================================================
PROMPT = Errors displayed by Cartridge Index can be reviewed in=
PROMPT = CHEMACXDB_MX_E table in CSCARTRIDGE schema.           = 
PROMPT = These errors are likely due to data problems.         = 
PROMPT = Script is still running……                             =
PROMPT =========================================================

ALTER INDEX mx PARAMETERS('SYNCHRONIZE');

ANALYZE INDEX mx COMPUTE STATISTICS;

--Check results
@@check_import_chemacx.sql

spool off
exit

	