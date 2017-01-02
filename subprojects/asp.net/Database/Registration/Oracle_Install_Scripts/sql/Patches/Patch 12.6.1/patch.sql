--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

--#########################################################
--Drop Invalid Indexes
--#########################################################

--#########################################################
--Recreate the Indexes for the schema user
--#########################################################

Connect &&schemaName/&&schemaPass@&&serverName

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

create index REG_NUMBERS_PROJECT_REGID_IX on REG_NUMBERS_PROJECT(REGID);
create index REG_NUMBERS_PROJECT_PROJID_IX on REG_NUMBERS_PROJECT(PROJECTID);
create index BATCH_PROJECT_BATCHID_IX on BATCH_PROJECT(BATCHID);
create index BATCH_PROJECT_PROJID_IX on BATCH_PROJECT(PROJECTID);
create index PEOPLE_PROJECT_PERSONID_IX on PEOPLE_PROJECT(PERSON_ID);
create index PEOPLE_PROJECT_PROJID_IX on PEOPLE_PROJECT(PROJECT_ID);

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--VIEWS
--#########################################################
Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;
@"sql\Patches\Patch 12.6.1\UpdatePickListDomainFeature.sql"

--#########################################################
--CONTEXTS
--#########################################################

--#########################################################
--TYPES
--#########################################################

--#########################################################
--GRANTS
--#########################################################
Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;
GRANT create type to &&schemaName;

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################
Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 12.6.1\Packages\pkg_ConfigurationCompoundRegistry_def.sql"
@"sql\Patches\Patch 12.6.1\Packages\pkg_ConfigurationCompoundRegistry_body.sql"
@"sql\Patches\Patch 12.6.1\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 12.6.1\Packages\pkg_CompoundRegistry_body.sql"
set define on

--#########################################################
--PACKAGES
--#########################################################

--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--DATA
--#########################################################

--#####################################################################
Connect &&schemaName/&&schemaPass@&&serverName

prompt executing "ConfigurationCompoundRegistry.CreateOrReplaceTypes"

BEGIN
ConfigurationCompoundRegistry.CreateOrReplaceTypes;
END;
/

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









