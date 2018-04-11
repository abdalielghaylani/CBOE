--Copyright 1998-2017 PerkinElmer Informatics, Inc. All rights reserved

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
create global temporary table duplicate_check (id number,compound_id number,regid number,structureid number);


update &&schemaName..coeobjectconfig
set xml = replace(xml, 'C:\Program Files (x86)\CambridgeSoft\ChemOfficeEnterprise12.1.0.0\Registration\PythonScripts\parentscript.py','C:\Program Files (x86)\PerkinElmer\ChemOfficeEnterprise\Registration\PythonScripts\parentscript.py')
where id = 2;

update &&schemaName..coeobjectconfig
set xml = replace(xml, 'Version=12.1.0.0','Version=17.1.0.0')
where id = 2;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

update &&securitySchemaName..COECONFIGURATION set
classname = replace(classname, 'Version=12.1.0.0','Version=17.1.0.0')
where description = 'Registration';


--#########################################################
--SEQUENCES
--#########################################################

Connect &&schemaName/&&schemaPass@&&serverName

create sequence sq_duplicate_check;

--#########################################################
--INDEXES
--#########################################################

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--VIEWS
--#########################################################
--CBOE-5908. remove "order by"
Connect &&schemaName/&&schemaPass@&&serverName
CREATE OR REPLACE VIEW VW_MIXTURE_STRUCTURE AS
	SELECT M.MIXTUREID, MC.MIXTURECOMPONENTID, C.*, S.STRUCTURE, R.REGNUMBER, CR.REGNUMBER AS COMPONENTID
        FROM   VW_MIXTURE M, VW_MIXTURE_COMPONENT MC, VW_COMPOUND C, VW_STRUCTURE S, VW_REGISTRYNUMBER R, VW_REGISTRYNUMBER CR
        WHERE  M.MIXTUREID = MC.MIXTUREID AND MC.COMPOUNDID = C.COMPOUNDID AND C.STRUCTUREID = S.STRUCTUREID AND M.REGID = R.REGID AND
	       C.REGID = CR.REGID;
--#########################################################
--CONTEXTS
--#########################################################

--#########################################################
--TYPES
--#########################################################

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################
Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 17.1.0\Function\fn_refresh_User_regnum_count.sql"
set define on

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
@"sql\Patches\Patch 17.1.0\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 17.1.0\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 17.1.0\Packages\pkg_ConfigurationCompoundRegistry_body.sql"

@"sql\Patches\Patch 17.1.0\Packages\pkg_RegistryDuplicateCheck.sql"
@"sql\Patches\Patch 17.1.0\Packages\pkg_RegistryDuplicateCheck_body.sql"
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

@&&setNextPatch 

