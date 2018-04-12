-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

-- Create ChemInv database.
-- This script creates the table space and database owner for the ChemInv database.
-- It should be run prior to importing a ChemInv samle data dump file or a dump file
-- from another ChemInv database.


spool ON
spool Logs\LOG_Create_Test_ChemInvDB.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql                      
alter user cscartridge quota unlimited on &&cscartTableSpaceName;  
Connect &&schemaName/&&schemaPass@&&serverName
@@packages\pkg_Constants_def.sql;
--Inv user needs privileges to add foreign key constraints to the people table
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
GRANT REFERENCES ON people TO &&schemaName;
Connect &&schemaName/&&schemaPass@&&serverName
@@Create_ChemInvDB.sql;
@@Create_Audit_Tables.sql;
@@Create_ChemInvDB_views.sql;
@@Create_ChemInvDB_Packages_Procedures.sql
--@@inserts.sql
@@Alter_CS_security_for_ChemInv.sql;
@@Create_ChemInvDB_test_users.sql;
--@@packages\pkg_FastIndexAccess.sql --this connects as cscartridge
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql
Connect &&schemaName/&&schemaPass@&&serverName
ALTER TABLE INV_LOCATIONS DISABLE CONSTRAINT INV_LOC_LOC_FK ;
ALTER TABLE INV_WELLS DISABLE CONSTRAINT WELLS_PLATES_FK;  
--ALTER TABLE INV_CONTAINERS DISABLE CONSTRAINT INV_CONT_CURRUSERID_FK;
ALTER TRIGGER TRG_MOLAR_CALCS DISABLE;
ALTER TRIGGER TRG_MOLAR_CONC DISABLE;                 
host imp &&schemaName/&&schemaPass@&&serverName parfile=Dump_Files\Test_ChemInvDB.inp

Connect &&schemaName/&&schemaPass@&&serverName
ALTER TRIGGER TRG_MOLAR_CALCS ENABLE;
ALTER TRIGGER TRG_MOLAR_CONC ENABLE;                 
ALTER TABLE INV_WELLS ENABLE CONSTRAINT WELLS_PLATES_FK;
ALTER TABLE INV_LOCATIONS ENABLE CONSTRAINT INV_LOC_LOC_FK ; 
@@incrementSequences.sql
@@indexClobFields&&OraVersionNumber
@@RecompilePLSQL.sql
--this is to add correct test data, has to be after the index is in place
update inv_wells set qty_remaining = 10, qty_initial = 10, qty_unit_fk = 8 where plate_id_fk is not null;

prompt logged session to: Logs\LOG_Create_Test_ChemInvDB.txt
spool off

exit
