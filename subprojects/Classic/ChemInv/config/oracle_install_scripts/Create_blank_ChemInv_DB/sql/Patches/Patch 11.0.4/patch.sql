--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt
--#########################################################
--Updating COEDB
--######################################################### 
Connect &&securitySchemaName/&&securitySchemaPass @&&serverName
@"Patches\Patch &&currentpatch\Alter\Alter_PrivilegeTable.sql"

--#######################################################################
-- Headerfile for updating the cheminvdb2 schema to enable ownership
--####################################################################### 


Connect &&InstallUser/&&sysPass@&&serverName
create public synonym racks for &&schemaName..racks;
grant execute on &&schemaName..racks to inv_browser;
create public synonym getnumberofcompoundwells for  &&schemaName..getnumberofcompoundwells;
grant execute on &&schemaName..getnumberofcompoundwells to inv_browser;
Connect &&schemaName/&&schemaPass@&&serverName
--@"Patches\Patch &&currentpatch\Alter\xxxxxxx"

prompt *******************************
Connect &&schemaName/&&schemaPass@&&serverName
-- Updating Authority
prompt '#########################################################'
prompt 'Updating Tables for Authority...'
prompt '#########################################################'

--@"Patches\Patch &&currentpatch\Alter\xxxxxxxxx"

-- Updating other objects
prompt '#########################################################'
prompt 'Alter Tables...'
prompt '#########################################################'
alter table inv_Reservations add (Request_ID_FK NUMBER(16));
--@"Patches\Patch &&currentpatch\PLSQL\Packages\xxxxxxxxx"
--@"Patches\Patch &&currentpatch\Alter\xxxxxxxxx"


prompt '#########################################################'
prompt 'Updating Functions and Packages...'
prompt '#########################################################'

--@"Patches\Patch &&currentpatch\PLSQL\Functions\xxxxxxxxx"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Requests_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Compounds_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Compounds_Body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Reservations_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Reservations_Body.sql"

prompt '#########################################################'
prompt 'Inserting Table Data...'
prompt '#########################################################'

--@"Patches\Patch &&currentpatch\Data\xxxxxxxxx"		

prompt '#########################################################'
prompt 'Inserting data...'
prompt '#########################################################'

  
@"Patches\Patch &&currentpatch\PLSQL\RecompilePLSQL.sql"


--#########################################################
--FUNCTIONS
--#########################################################
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateAllContainerFields.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateLocation.sql"
prompt 


set define on

UPDATE &&schemaName..Globals
	SET Value = '&&CurrentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';
UPDATE &&schemaName..Globals
	SET Value = '&&CurrentPatch' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&CurrentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&CurrentPatch'
		THEN  'Patches\stop.sql'
		ELSE  '"Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;
 
prompt ****&&setNextPatch ***
@&&setNextPatch 






