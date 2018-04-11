--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

--#########################################################
--TABLES
--######################################################### 





set define off
@"sql\Patches\Patch 12.1.3\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 12.1.3\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 12.1.3\Packages\pkg_RegistrationRLS_body.sql"
@"sql\Patches\Patch 12.1.3\Packages\pkg_RegistryDuplicateCheck_def.sql"
@"sql\Patches\Patch 12.1.3\Packages\pkg_RegistryDuplicateCheck_body.sql"
@"sql\Patches\Patch 12.1.3\Packages\Pkg_Gui_Util_body.sql"
set define on
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
--CONTEXTS
--#########################################################

--#########################################################
--TYPES
--#########################################################

set define off

set define on

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

--#########################################################
--GRANTS
--#########################################################

--#########################################################
--PACKAGES
--#########################################################

set define off

set define on

--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--DATA
--#########################################################

--#########################################################
--DATA
--#########################################################



--#####################################################################

UPDATE &&schemaName..Globals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';

UPDATE &&schemaName..Globals
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


