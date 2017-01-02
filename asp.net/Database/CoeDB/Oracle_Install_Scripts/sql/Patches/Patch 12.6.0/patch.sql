--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 
Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--TABLES
--######################################################### 

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
--PROCEDURES AND FUNCTIONS
@"sql\Patches\Patch &&currentPatch\Procedure\ProcmapPrivStoRole.sql"
--#########################################################
@"sql\Patches\Patch 12.6.0\createCOERole.sql"
--#########################################################
--PACKAGES
--#########################################################
@"sql\Patches\Patch &&currentPatch\Packages\pkg_Roles.sql"
--#########################################################
--DATA
--#########################################################

--set define off
@"sql\Patches\Patch 12.6.0\UpdateDataViewConfigurations.sql"
@"sql\Patches\Patch 12.6.0\evolveSchema.sql"
--set define on


--COMMIT;



--#####################################################################


UPDATE &&schemaName..CoeGlobals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'SCHEMAVERSION';

UPDATE &&schemaName..CoeGlobals
	SET Value = '&&versionApp' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&currentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&currentPatch'
		THEN  'sql\Patches\stop.sql'
		ELSE  '"sql\Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;

@&&setNextPatch 









