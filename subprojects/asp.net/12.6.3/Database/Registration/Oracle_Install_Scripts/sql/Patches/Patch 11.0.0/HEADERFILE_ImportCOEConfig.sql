--Copyright 1999-2011 CambridgeSoft Corporation. All rights reserved




@@Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName

update regdb.sequence set active = null;
alter table  regdb.sequence modify (active nchar(1));
update regdb.sequence set active = 'T';
update regdb.sequence set type = 'R' where prefix = 'CID';
update regdb.sequence set type = 'C' where prefix = 'COC';
commit;

truncate table coedb.coeform;
delete from coedataview;
commit;
---truncate table coedb.coeconfiguration;
truncate table coedb.coepagecontrol;

host IMP &&InstallUser/&&sysPass@&&serverName parfile=coeconfig.imp




exit


	