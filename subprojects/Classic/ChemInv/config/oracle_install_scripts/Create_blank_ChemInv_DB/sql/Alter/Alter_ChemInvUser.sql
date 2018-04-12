-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Alter Schema user
--######################################################### 

Connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'Altering &&schemaName user for supporting Oracle 12c'
prompt '#########################################################'

ALTER USER &&schemaName QUOTA UNLIMITED ON &&tableSpaceName;
ALTER USER &&schemaName QUOTA UNLIMITED ON &&indexTableSpaceName;
ALTER USER &&schemaName QUOTA UNLIMITED ON &&lobsTableSpaceName;
ALTER USER &&schemaName QUOTA UNLIMITED ON &&auditTableSpaceName;
grant execute on wm_concat to &&schemaName;
