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


--indexes for increasing performance of duplicate checking
create index index_compoundmol_strucid on &&schemaName..compound_molecule(structureid);
create index index_mixcomp_mixcompound on &&schemaName..mixture_component(mixtrureid, compoundid);
create index index_mixcomp_mixcompound2 on &&schemaName..MIXTURE_COMPONENT("COMPOUNDID");
create index index_comp_cpdstruc on &&schemaName..COMPOUND_MOLECULE("CPD_DATABASE_COUNTER","STRUCTUREID");
create index regdb.batch_identifier_batchid on &&schemaName..batch_identifier(batchID);
create index statusidtemp_indx on &&schemaName..mixtures(statusid);
create index statusidmix_indx on &&schemaName..temporary_batch(statusid);

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

set define on
@"sql\Patches\Patch 12.3.2\Packages\Pkg_Gui_Util_def.sql"
@"sql\Patches\Patch 12.3.2\Packages\Pkg_Gui_Util_body.sql"
Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 12.3.2\Packages\pkg_ConfigurationCompoundRegistry_body.sql"
@"sql\Patches\Patch 12.3.2\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 12.3.2\Packages\Pkg_RegistryDuplicateCheck_body.sql"
set define on
Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;


--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--DATA
--#########################################################
update  &&schemaName..FRAGMENTS set DESCRIPTION = trim(DESCRIPTION);
/
@"sql\Patches\Patch 12.3.2\UpdateConfigurations.sql"
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









