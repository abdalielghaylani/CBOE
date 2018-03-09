

--#########################################################
--CREATE PACKAGES
--#########################################################

set define off

@sql\pkg_RegistrationRLS_def.sql
@sql\pkg_CompoundRegistry_def.sql
@sql\pkg_CompoundRegistry_body.sql
@sql\pkg_RegistrationRLS_body.sql
@sql\pkg_ConfigurationCompoundRegistry_def.sql
@sql\pkg_ConfigurationCompoundRegistry_body.sql
@sql\pkg_Gui_Util_def.sql
@sql\pkg_Gui_Util_body.sql


set define on