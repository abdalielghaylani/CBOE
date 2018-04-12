--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

prompt *******************************
prompt **** Applying Patch for RegistrationIntegration ****
prompt *******************************

Connect &&InstallUser/&&sysPass@&&serverName
COL oracleEdition new_value oracleEdition NOPRINT
SELECT	CASE
		WHEN  (select 1 from v$version where lower(banner) like '%enterprise%')=1
		THEN  ''
		ELSE  '_Standard'
	END	AS oracleEdition
FROM	DUAL;

Connect &&regSchemaName/&&regSchemaPass@&&serverName

GRANT SELECT on VW_BATCH to &&schemaName with grant option;
GRANT SELECT on VW_REGISTRYNUMBER to &&schemaName with grant option;
GRANT SELECT on VW_COMPOUND_IDENTIFIER to &&schemaName with grant option;
GRANT SELECT on VW_MIXTURE to &&schemaName with grant option;
GRANT SELECT on VW_PICKLIST to &&schemaName with grant option;
GRANT SELECT on PICKLIST to &&schemaName with grant option;
GRANT SELECT on VW_MIXTURE_STRUCTURE to &&schemaName with grant option;
GRANT SELECT on BATCHES to &&schemaName with grant option;
GRANT SELECT on VW_IDENTIFIERTYPE to &&schemaName with grant option;

-- Need access to underlying tables to create fast refresh materialized view
-- Note:  ALL privilege may be excessive
GRANT ALL ON &&regSchemaName..REG_NUMBERS TO &&schemaName;
GRANT ALL ON &&regSchemaName..MIXTURES TO &&schemaName;

grant select on &&regSchemaName..compound_fragment to inv_browser; 
grant select on &&regSchemaName..compound_molecule to inv_browser;
grant select on &&regSchemaName..vw_compound_fragment to inv_browser;
grant select on &&regSchemaName..vw_compound_fragment to inv_browser;
grant select on &&regSchemaName..vw_fragment to inv_browser;
grant select on &&regSchemaName..vw_compound to inv_browser;
grant select on &&regSchemaName..compound_fragment to &&schemaName; 
grant select on &&regSchemaName..compound_molecule to &&schemaName;
grant select on &&regSchemaName..vw_compound_fragment to &&schemaName;
grant select on &&regSchemaName..vw_fragment to &&schemaName;
grant select on &&regSchemaName..vw_compound to &&schemaName;
grant select on &&regSchemaName..vw_mixture_component to &&schemaName;
grant select on &&regschemaName..vw_batchcomponentfragment to &&schemaName;
grant select on &&regschemaName..vw_batchcomponent to &&schemaName;
grant select on &&regschemaName..structures to &&schemaName;

--REGDB should have EXECUTE privilege on the package &&schemaName..COMPOUNDS 
Connect &&InstallUser/&&sysPass@&&serverName
GRANT EXECUTE on &&schemaName..COMPOUNDS to &&regSchemaName;
GRANT GLOBAL QUERY REWRITE to &&schemaName;

grant alter any materialized view to &&schemaName;


prompt 
-- changed refresh fast on commit to refresh fast on demand
declare
  no_primary_key exception;
  PRAGMA EXCEPTION_INIT (no_primary_key, -12014);
  no_primary_key1 exception;
  PRAGMA EXCEPTION_INIT (no_primary_key1, -12024);
  no_primary_key2 exception;
  PRAGMA EXCEPTION_INIT (no_primary_key2, -12016);
cnt number;
begin
select count(1) into cnt from dba_objects o where object_name = 'INV_VW_COMPOUNDS' 
and owner = '&&schemaName' and object_type = 'MATERIALIZED VIEW';
if cnt >0 then 
begin
/* Oracle does not make it easy to reliably determine if a materialized view is 
created with the PRIMARY KEY clause or not.  So we're going to 
assume it is created with PRIMARY KEY, but if that fails, we'll try again without it. */
  execute immediate 'ALTER MATERIALIZED VIEW &&schemaName..INV_VW_COMPOUNDS REFRESH FAST ON DEMAND START WITH sysdate NEXT sysdate + ((1/24/3600)*120) WITH PRIMARY KEY';
exception
  when others
 -- no_primary_key or no_primary_key1 or no_primary_key2 
  then
    execute immediate 'ALTER MATERIALIZED VIEW &&schemaName..INV_VW_COMPOUNDS REFRESH FAST ON DEMAND START WITH sysdate NEXT sysdate + ((1/24/3600)*120)';
end;
end if;
end;
/

-- changed refresh fast on commit to refresh fast on demand
declare
  no_primary_key exception;
  PRAGMA EXCEPTION_INIT (no_primary_key, -12014);
  no_primary_key1 exception;
  PRAGMA EXCEPTION_INIT (no_primary_key1, -12024);
  no_primary_key2 exception;
  PRAGMA EXCEPTION_INIT (no_primary_key2, -12016);
cnt number;
begin
select count(1) into cnt from dba_objects o where object_name = 'INV_VW_REG_BATCHES' 
and owner = '&&schemaName' and object_type = 'MATERIALIZED VIEW';
if cnt >0 then 
begin
/* Oracle does not make it easy to reliably determine if a materialized view is 
created with the PRIMARY KEY clause or not.  So we're going to 
assume it is created with PRIMARY KEY, but if that fails, we'll try again without it. */
  execute immediate 'ALTER MATERIALIZED VIEW &&schemaName..INV_VW_REG_BATCHES 
 REFRESH FAST ON DEMAND START WITH sysdate NEXT sysdate + ((1/24/3600)*120)
 WITH PRIMARY KEY';
exception
  when others
  --no_primary_key or no_primary_key1 or no_primary_key2 
  then
    execute immediate 'ALTER MATERIALIZED VIEW &&schemaName..INV_VW_REG_BATCHES 
 REFRESH FAST ON DEMAND START WITH sysdate NEXT sysdate + ((1/24/3600)*120)';
end;
end if;
end;
/


Connect &&schemaName/&&schemaPass@&&serverName
prompt '#########################################################'
prompt 'Create Functions...'
prompt '#########################################################'

@Patches\RegistrationIntegration\f_GetSaltName.sql;
@Patches\RegistrationIntegration\f_GetSolvateName.sql;
@Patches\RegistrationIntegration\f_GetPickListValue.sql;
@Patches\RegistrationIntegration\f_GetStructureProperty.sql;
@Patches\RegistrationIntegration\f_GetRegComponentIdentifier.sql;
@Patches\RegistrationIntegration\f_GetRegMixtureIdentifier.sql;
@Patches\RegistrationIntegration\f_GetRegAltID.sql;
@Patches\RegistrationIntegration\f_GetUserIDFromPersonID.sql;
@Patches\RegistrationIntegration\f_GetRegName.sql;

prompt '#########################################################'
prompt 'Create Views...'
prompt '#########################################################'

@@Alter_INV_VW_COMPOUNDS&&oracleEdition..sql;

Connect &&InstallUser/&&sysPass@&&serverName
@@Alter_INV_VW_REG_BATCHES&&oracleEdition..sql;

Connect &&schemaName/&&schemaPass@&&serverName
grant select on &&schemaName..inv_vw_reg_batches to inv_browser;

--#######################################################################
-- Adding permission in COEDB for VW_PICKLIST
--####################################################################### 

prompt *******************************
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

insert into OBJECT_PRIVILEGES(PRIVILEGE_NAME, PRIVILEGE, SCHEMA, OBJECT_NAME)
values ('INV_BROWSE_ALL', 'SELECT', '&&regSchemaName', 'VW_PICKLIST');


insert into OBJECT_PRIVILEGES(PRIVILEGE_NAME, PRIVILEGE, SCHEMA, OBJECT_NAME)
values ('INV_BROWSE_ALL', 'EXECUTE', '&&SchemaName', 'GETREGNAME');
prompt *******************************
Connect &&regSchemaName/&&regSchemaPass@&&serverName

--#######################################################################
-- Adding permission in COEDB for VW_PICKLIST
--####################################################################### 

GRANT SELECT ON &&regSchemaName..VW_PICKLIST TO INV_BROWSER;
GRANT SELECT ON &&regSchemaName..VW_PICKLIST TO INV_CHEMIST;
GRANT SELECT ON &&regSchemaName..VW_PICKLIST TO INV_RECEIVING;
GRANT SELECT ON &&regSchemaName..VW_PICKLIST TO INV_FINANCE;
GRANT SELECT ON &&regSchemaName..VW_PICKLIST TO INV_REGISTRAR;
GRANT SELECT ON &&regSchemaName..VW_PICKLIST TO INV_ADMIN;

--#######################################################################
-- Update Reg views
--####################################################################### 
@Patches\RegistrationIntegration\Alter_VW_UNIT.sql;

prompt '#########################################################'
prompt 'Modify Reg Configurations...'
prompt '#########################################################'
BEGIN
	&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','ChemInvSchemaName','&&schemaName', 'Configures ChemInv schema name', 'TEXT', NULL, 'False');
END;
/

connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

GRANT SELECT ON PEOPLE TO &&schemaName WITH GRANT OPTION;
GRANT SELECT ON &&regSchemaName..VW_PICKLIST TO &&schemaName WITH GRANT OPTION;
@@GrantPrivs.sql;

prompt '#########################################################'
prompt 'Fixing ELN/BA Views...'
prompt '#########################################################'
@@Alter_ELNBAViews.sql

