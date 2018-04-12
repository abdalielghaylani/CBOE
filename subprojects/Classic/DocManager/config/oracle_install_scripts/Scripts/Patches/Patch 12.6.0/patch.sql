--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

connect &&securitySchemaName/&&securitySchemaPass@&&serverName
--#########################################################
--TABLES
--######################################################### 
@"Patches\Patch &&currentpatch\Alter\alter_CS_Security.sql"

Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--INDEXES
--#########################################################

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--VIEWS
--#########################################################

--#########################################################
--PROCEDURES
--#########################################################


--#########################################################
--PACKAGES
--#########################################################

set define off

set define on

UPDATE &&schemaName..Globals
	SET Value = '&&CurrentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';
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






