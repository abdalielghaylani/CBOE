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

Connect &&InstallUser/&&sysPass@&&serverName;

drop index system.index_compoundmol_strucid;
drop index system.index_mixcomp_mixcompound;
drop index system.index_mixcomp_mixcompound2;
drop index system.index_comp_cpdstruc;
drop index system.statusidtemp_indx;
drop index system.statusidmix_indx;

--#########################################################
--Recreate the Indexes for the schema user
--#########################################################

Connect &&schemaName/&&schemaPass@&&serverName

create index &&schemaName..index_compoundmol_strucid on &&schemaName..compound_molecule(structureid);
create index &&schemaName..index_mixcomp_mixcompound on &&schemaName..mixture_component(mixtrureid, compoundid);
create index &&schemaName..index_mixcomp_mixcompound2 on &&schemaName..MIXTURE_COMPONENT("COMPOUNDID");
create index &&schemaName..index_comp_cpdstruc on &&schemaName..COMPOUND_MOLECULE("CPD_DATABASE_COUNTER","STRUCTUREID");
create index &&schemaName..statusidtemp_indx on &&schemaName..mixtures(statusid);
create index &&schemaName..statusidmix_indx on &&schemaName..temporary_batch(statusid);

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
@"sql\Patches\Patch 12.5.3\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 12.5.3\Packages\pkg_RegistryDuplicateCheck_body.sql"
set define on
--#########################################################
--TRIGGERS
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










