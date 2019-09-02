--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

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
@"sql\Patches\Patch 12.6.2\alterfragments.sql"

--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--INDEXES
--#########################################################
CREATE INDEX BATCHCOMPONENT_BATCHID_IX ON BATCHCOMPONENT (BATCHID);

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
Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 12.6.2\Packages\pkg_RegistrationRLS_body.sql"
@"sql\Patches\Patch 12.6.2\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 12.6.2\Packages\pkg_CompoundRegistry_body.sql"
set define on

ALTER PACKAGE REGDB.RegistrationRLS COMPILE PACKAGE;
ALTER PACKAGE REGDB.RegistrationRLS COMPILE BODY;

--#########################################################
--TRIGGERS
--#########################################################
@"sql\Patches\Patch 12.6.2\trg_fragments_bu.sql"

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
--Added the check for CurrentPatch = 12.6.2 to avoid the oracle error SP2-0309: SQL*Plus command procedures may only be nested to a depth of 20
--This is caused due to the nested script execution has reached the depth limit of 20 starting from 11.0.1
--When 12.6.2 is reached, we end the script execution and go back to the header file and continue the remaining patch scripts as a fresh nested script.
COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&currentPatch'
		THEN  'sql\Patches\stop.sql'
		ELSE  '"sql\Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;









