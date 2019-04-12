--Copyright 1998-2018 PerkinElmer Informatics, Inc. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--Drop Invalid Indexes
--#########################################################

--#########################################################
--Recreate the Indexes for the schema user
--#########################################################

--#########################################################
--TABLES
--######################################################### 


update &&schemaName..coeobjectconfig
set xml = replace(xml, 'Version=18.1.1.0','Version=19.1.0.0')
where id = 2;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

update &&securitySchemaName..COECONFIGURATION set
classname = replace(classname, 'Version=18.1.1.0','Version=19.1.0.0')
where description = 'Registration';
 

Connect &&schemaName/&&schemaPass@&&serverName

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
--PROCEDURES AND FUNCTIONS
--#########################################################

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
--GRANTS
--#########################################################

--#########################################################
--SYNONYM
--#########################################################

--#########################################################
--PACKAGES
--#########################################################
Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 19.1.0\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 19.1.0\Packages\pkg_CompoundRegistry_body.sql"
set define on
--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--JOBS
--#########################################################

--#########################################################
--ORACLE PARAMETERS
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

