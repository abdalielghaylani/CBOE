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
Connect &&schemaName/&&schemaPass@&&serverName

drop index &&schemaName..INDEX_SEQ_PREFIX;
create unique index &&schemaName..UNQ_INDEX_SEQ_PREFIX on &&schemaName..SEQUENCE (PREFIX ASC) TABLESPACE &&indexTableSpaceName;

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

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

--#########################################################
--GRANTS
--#########################################################
@"sql\Patches\Patch 12.5.2\Grants.sql"
--#########################################################
--PACKAGES
--#########################################################

Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 12.5.2\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 12.5.2\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 12.5.2\Packages\pkg_RegistryDuplicateCheck_def.sql"
@"sql\Patches\Patch 12.5.2\Packages\pkg_RegistryDuplicateCheck_body.sql"
@"sql\Patches\Patch 12.5.2\Packages\pkg_ConfigurationCompoundRegistry_body.sql"
@"sql\Patches\Patch 12.5.2\Packages\pkg_RegistrationRLS_body.sql"
@"sql\Patches\Patch 12.5.2\Packages\Pkg_Gui_Util_def.sql"
@"sql\Patches\Patch 12.5.2\Packages\Pkg_Gui_Util_body.sql"
set define on


--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--DATA
--#########################################################
Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;
@"sql\Patches\Patch 12.5.2\UpdateRegRoles.sql"
@"sql\Patches\Patch 12.5.2\LoadCoepagecontrols_reg_custom.sql"
@"sql\Patches\Patch 12.5.2\PickListDomainFeature.sql"
@"sql\Patches\Patch 12.5.2\AddColumnToSequences.sql"
@"sql\Patches\Patch 12.5.2\UpdateConfigurations.sql"
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









