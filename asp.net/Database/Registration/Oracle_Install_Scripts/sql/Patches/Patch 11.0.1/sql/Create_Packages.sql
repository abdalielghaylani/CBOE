--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

prompt 
prompt Starting "CREATE_Packages.sql...
prompt 

--#########################################################
--CREATE PACKAGES
--#########################################################

set define off

@"sql\Patches\Patch 11.0.1\plsql\pkg_RegistrationRLS_def.sql"
@"sql\Patches\Patch 11.0.1\plsql\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 11.0.1\plsql\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 11.0.1\plsql\pkg_RegistrationRLS_body.sql"
@"sql\Patches\Patch 11.0.1\plsql\pkg_ConfigurationCompoundRegistry_def.sql"
@"sql\Patches\Patch 11.0.1\plsql\pkg_ConfigurationCompoundRegistry_body.sql"
@"sql\Patches\Patch 11.0.1\plsql\pkg_Gui_Util_def.sql"
@"sql\Patches\Patch 11.0.1\plsql\pkg_Gui_Util_body.sql"

set define on