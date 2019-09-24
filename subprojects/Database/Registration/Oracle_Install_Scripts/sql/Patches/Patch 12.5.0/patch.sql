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

--#########################################################
--SEQUENCES
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
@"sql\Patches\Patch 12.5.0\addMissingViews.sql"
--#########################################################
--CONTEXTS
--#########################################################

--#########################################################
--TYPES
--#########################################################

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

--#########################################################
--GRANTS
--#########################################################

--#########################################################
--PACKAGES
--#########################################################

Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 12.5.0\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 12.5.0\Packages\Pkg_RegistryDuplicateCheck_body.sql"
@"sql\Patches\Patch 12.5.0\Packages\Pkg_Gui_Util_def.sql"
@"sql\Patches\Patch 12.5.0\Packages\Pkg_Gui_Util_body.sql"
@"sql\Patches\Patch 12.5.0\Packages\pkg_Auditing_body.sql"
set define on

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;


--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--DATA
--#########################################################

@"sql\Patches\Patch 12.5.0\UpdateConfigurations.sql"

COMMIT;

@"sql\Patches\Patch 12.5.0\LoadCoepagecontrols_reg_master.sql"

COMMIT;

@"sql\Patches\Patch 12.5.0\LoadCoepagecontrols_reg_custom.sql"

COMMIT;

@"sql\Patches\Patch 12.5.0\UpdateRegRoles.sql"

COMMIT;

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










