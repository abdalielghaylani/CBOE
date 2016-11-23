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

@"sql\Patches\Patch 12.3.0\Auditing_feature.sql"
@"sql\Patches\Patch 12.3.0\alterFullRegNum.sql"

--#########################################################
--TABLES
--#########################################################

@"sql\Patches\Patch 12.3.0\mods_statusid.sql"
@"sql\Patches\Patch 12.3.0\mods_submissioncomments.sql"
@"sql\Patches\Patch 12.3.0\addPersonApproved.sql"
--#########################################################
--PACKAGES
--#########################################################

set define off
@"sql\Patches\Patch 12.3.0\Packages\pkg_ConfigurationCompoundRegistry_body.sql"
set define on
@"sql\Patches\Patch 12.3.0\Packages\pkg_RegistryDuplicateCheck_def.sql"
@"sql\Patches\Patch 12.3.0\Packages\pkg_RegistryDuplicateCheck_body.sql"
@"sql\Patches\Patch 12.3.0\Packages\Pkg_Gui_Util_def.sql"
@"sql\Patches\Patch 12.3.0\Packages\Pkg_Gui_Util_body.sql"

Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 12.3.0\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 12.3.0\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 12.3.0\Packages\pkg_RegistrationRLS_def.sql"
@"sql\Patches\Patch 12.3.0\Packages\pkg_RegistrationRLS_body.sql"
set define on
Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

--#########################################################
--DATA
--#########################################################

@"sql\Patches\Patch 12.3.0\UpdateConfigurations.sql"

-- To ensure database change notification can be used by the caching mechanism
GRANT CHANGE NOTIFICATION TO &&schemaName;


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

@&&setNextPatch 


