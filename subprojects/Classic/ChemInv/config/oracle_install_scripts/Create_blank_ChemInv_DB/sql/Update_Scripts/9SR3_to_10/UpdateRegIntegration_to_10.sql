-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Update file for Inventory Enterprise integration 
-- with Registration Enterprise for 10
--######################################################### 

spool ..\..\Logs\LOG_UpdateRegIntegration10.txt

-- Intialize variables
@@..\..\Parameters.sql
@@..\..\Prompts.sql

--' Update views in the Inventory schema
Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Updating Functions...'
prompt '#########################################################'

@@RegistrationIntegration\f_UpdateRegCompound.sql;
@@RegistrationIntegration\f_CreateRegCompound.sql;

prompt '#########################################################'
prompt 'Recompiling pl/sql...'
prompt '#########################################################'

@PLSQL\RecompilePLSQL.sql;

prompt '#########################################################'
prompt 'Updating existing container records...'
prompt '#########################################################'

-- This script should run prior to creating inv_vw_compounds to ensure
-- those records get inserted into the view
@@RegistrationIntegration\update_existing_containers.sql
commit;

prompt '#########################################################'
prompt 'Updating existing Wells ...'
prompt '#########################################################'
@@RegistrationIntegration\Update_Existing_Wells.sql
commit;

prompt '#########################################################'
prompt 'Creating views...'
prompt '#########################################################'

-- Use RegRLSScript to detect when Reg has RLS enabled
DEFINE RegRLSScript = Add_Reg_RLS_Policy
col rls_col for a30 new_value RegRLSScript

@@RegistrationIntegration\DetectRegRLS.sql;
@@RegistrationIntegration\Inv_VW_Compounds.sql;
@@RegistrationIntegration\&RegRLSScript..sql;
@@RegistrationIntegration\inv_vw_reg_batches.sql;

prompt '#########################################################'
prompt 'Altering CS_SECURITY for new privileges...'
prompt '#########################################################'

@@RegistrationIntegration\Alter_Cs_Security.sql

Connect &&schemaName/&&schemaPass@&&serverName

prompt #############################################################
prompt Logged session to: Logs\LOG_UpdateRegIntegration10.txt
prompt #############################################################


prompt 
spool off

exit
