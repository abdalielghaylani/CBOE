
Connect &&InstallUser/&&sysPass@&&serverName

-- point users to new centralized temporary tablespace  

alter user &&schemaName  default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T1_84 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T2_84 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T3_84 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T4_84 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T5_84 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T6_84 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T1_85 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T2_85 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T3_85 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T4_85 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T5_85 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
alter user T6_85 default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;

-- drop old temp tablespace
BEGIN
	if &&OraVersionNumber =  8 then 
		execute immediate 'drop tablespace t_regdb_temp including contents cascade constraints';
	else 
		execute immediate 'drop tablespace t_regdb_temp including contents and datafiles cascade constraints';	
	end if;
END;
/