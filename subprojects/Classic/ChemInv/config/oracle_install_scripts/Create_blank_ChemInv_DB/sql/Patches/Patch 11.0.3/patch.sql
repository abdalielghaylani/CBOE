--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

--#######################################################################
-- Headerfile for updating the cheminvdb2 schema to enable ownership
--####################################################################### 


Connect &&InstallUser/&&sysPass@&&serverName
@"Patches\Patch &&currentpatch\Alter\Alter_COEDB_Grants.sql"
prompt *******************************
Connect &&schemaName/&&schemaPass@&&serverName
-- Updating Authority
prompt '#########################################################'
prompt 'Updating Tables for Authority...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\Alter\Alter_Add_Authority_Columns.sql"
@"Patches\Patch &&currentpatch\Alter\Alter_Add_Location_Type_Columns.sql"

prompt '#########################################################'
prompt 'Updating Functions and Packages for Authority...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Authority.sql"

-- Updating other objects
prompt '#########################################################'
prompt 'Alter Tables...'
prompt '#########################################################'

--@"Patches\Patch &&currentpatch\Alter\Alter_Inv_Compounds.sql"
--@"Patches\Patch &&currentpatch\Alter\Alter_Inv_Containers.sql"
--@"Patches\Patch &&currentpatch\Alter\Alter_Inv_Plates.sql"

prompt '#########################################################'
prompt 'Updating Functions and Packages...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CreateContainer.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateAllContainerFields.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_OrderContainer.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CreateLocation.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateLocation.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CopyContainer.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_EnableGridForLocation.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Compounds_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Compounds_Body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Reformat_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Reformat_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_GUIUtils_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_GUIUtils_Body.sql"

prompt '#########################################################'
prompt 'Inserting Table Data...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\Data\IncrementSequences.sql"		
@"Patches\Patch &&currentpatch\Data\Inv_Graphics.sql"
@"Patches\Patch &&currentpatch\Data\Inv_Location_Types.sql"

prompt '#########################################################'
prompt 'Inserting data...'
prompt '#########################################################'


--updating permissions on authority 
Grant execute on cheminvdb2.authority to public;
  
@"Patches\Patch &&currentpatch\PLSQL\RecompilePLSQL.sql"

prompt ####################################################################
prompt Logged session to: Logs\LOG_Enable_ownership.txt
prompt ####################################################################

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






