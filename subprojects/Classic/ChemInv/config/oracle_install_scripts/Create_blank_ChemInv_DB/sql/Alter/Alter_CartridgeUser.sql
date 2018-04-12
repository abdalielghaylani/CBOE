-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Alter cartridge user
--######################################################### 

Connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'Altering cartridge user...'
prompt '#########################################################'

alter user cscartridge quota unlimited on &&cscartTableSpaceName;  
/

show errors;