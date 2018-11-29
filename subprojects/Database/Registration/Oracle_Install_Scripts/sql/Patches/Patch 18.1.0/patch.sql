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
set xml = replace(xml, 'Version=17.1.1.0','Version=18.1.0.0')
where id = 2;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

update &&securitySchemaName..COECONFIGURATION set
classname = replace(classname, 'Version=17.1.1.0','Version=18.1.0.0')
where description = 'Registration';

--CBOE-8264
update coedataview set coedataview =xmltype(
replace((coedataview).getclobval(),'<relationship parentkey="212" childkey="201" parent="209" child="1" jointype="OUTER"/>'
,'<relationship parentkey="212" childkey="201" parent="209" child="2" jointype="OUTER"/>')) where id=4004;
--CBOE-8264. End

--CBOE-8306

update COEFORM a set a.COEFORM = (updatexml( xmltype(COEFORM),
'formGroup/detailsForms/detailsForm/coeForms/coeForm[@id="1002"]/addMode/formElement[@name="SCIENTIST_ID"]/COE:configInfo',
'<COE:configInfo xmlns:COE="COE.FormGroup">
    <COE:fieldConfig>
         <COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
         <COE:CSSClass>FEDropDownListViewMode</COE:CSSClass>
         <COE:dropDownItemsSelect />
         <PickListDomain>3</PickListDomain>
         <COE:Enable>False</COE:Enable>
         <COE:ID>SCIENTIST_IDProperty</COE:ID>
         <COE:AutoPostBack>False</COE:AutoPostBack>
    </COE:fieldConfig>
</COE:configInfo>'
,'xmlns="COE.FormGroup" xmlns:COE="COE.FormGroup"')).getclobval()
 where id = 4010;
 
update COEFORM a set a.COEFORM = (updatexml( xmltype(COEFORM),
'formGroup/detailsForms/detailsForm/coeForms/coeForm[@id="1002"]/editMode/formElement[@name="SCIENTIST_ID"]/COE:configInfo',
'<COE:configInfo xmlns:COE="COE.FormGroup">
    <COE:fieldConfig>
         <COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
         <COE:CSSClass>FEDropDownListViewMode</COE:CSSClass>
         <COE:dropDownItemsSelect />
         <PickListDomain>3</PickListDomain>
         <COE:Enable>True</COE:Enable>
         <COE:ID>SCIENTIST_IDProperty</COE:ID>
         <COE:AutoPostBack>False</COE:AutoPostBack>
    </COE:fieldConfig>
</COE:configInfo>'
,'xmlns="COE.FormGroup" xmlns:COE="COE.FormGroup"')).getclobval()
 where id = 4010;
 
 update COEFORM a set a.COEFORM = (updatexml( xmltype(COEFORM),
'formGroup/detailsForms/detailsForm/coeForms/coeForm[@id="1002"]/viewMode/formElement[@name="SCIENTIST_ID"]/COE:configInfo',
'<COE:configInfo xmlns:COE="COE.FormGroup">
    <COE:fieldConfig>
         <COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
         <COE:CSSClass>FEDropDownListViewMode</COE:CSSClass>
         <COE:dropDownItemsSelect />
         <PickListDomain>3</PickListDomain>
         <COE:Enable>True</COE:Enable>
         <COE:ID>SCIENTIST_IDProperty</COE:ID>
         <COE:AutoPostBack>False</COE:AutoPostBack>
    </COE:fieldConfig>
</COE:configInfo>'
,'xmlns="COE.FormGroup" xmlns:COE="COE.FormGroup"')).getclobval()
 where id = 4010;

--CBOE-8306 End

commit;
 

Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--INDEXES
--#########################################################

create index structure_identifier_str_id on structure_identifier(structureid);


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
set define off
@"sql\Patches\Patch 18.1.0\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 18.1.0\Packages\pkg_RegistryDuplicateCheck_body.sql"

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

