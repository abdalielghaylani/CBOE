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

prompt *******************************
Connect &&schemaName/&&schemaPass@&&serverName
-- Updating Authority
prompt '#########################################################'
prompt 'Updating Tables for Authority...'
prompt '#########################################################'


-- Updating other objects
prompt '#########################################################'
prompt 'Alter Tables...'
prompt '#########################################################'


prompt '#########################################################'
prompt 'Updating Functions and Packages...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Requests_body.sql"

prompt '#########################################################'
prompt 'Inserting Table Data...'
prompt '#########################################################'


prompt '#########################################################'
prompt 'Inserting data...'
prompt '#########################################################'


--#########################################################
--FUNCTIONS
--#########################################################


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




