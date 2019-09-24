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
update &&schemaName..coeobjectconfig
set xml = replace(xml, 'Version=11.0.1.0','Version=12.1.0.0')
where id = 2;

update &&schemaName..coeobjectconfig
set xml = replace(xml, 'ChemOfficeEnterprise11.0.1.0','ChemOfficeEnterprise12.1.0.0')
where id = 2; 

update &&securitySchemaName..COECONFIGURATION set
classname = replace(classname, 'Version=11.0.1.0','Version=12.1.0.0')
where description = 'Registration';

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


--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--DATA
--#########################################################


@"sql\Patches\Patch 12.1.0\UpdateConfigurations.sql"
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









