-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved
Connect &&InstallUser/&&sysPass@&&serverName

-- point users to new centralized temporary tablespace  

alter user &&schemName temporary tablespace &&tempTableSpaceName;
alter user invadmin temporary tablespace &&tempTableSpaceName;
alter user invbrowser temporary tablespace &&tempTableSpaceName;
alter user invfinance temporary tablespace &&tempTableSpaceName;
alter user invregistrar temporary tablespace &&tempTableSpaceName;
alter user invreceiving temporary tablespace &&tempTableSpaceName;

-- drop old temp tablespace
BEGIN
	if &&OraVersionNumber =  8 then 
		execute immediate 'drop tablespace t_cheminv2_temp including contents cascade constraints';
	else 
		execute immediate 'drop tablespace t_cheminv2_temp including contents and datafiles cascade constraints';	
	end if;
END;
/