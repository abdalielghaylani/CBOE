-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Drop all tablespaces
--######################################################### 

Connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'dropping tablespaces...'
prompt '#########################################################'


DECLARE
	n NUMBER;
	dataFileClause varchar2(20);
BEGIN
	--if OraVersionNumber =  8 then 
	--	dataFileClause := '';
	--else 
		dataFileClause := 'AND DATAFILES';	
	--end if;
	select count(*) into n from dba_tablespaces where tablespace_name = '&&tableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&tableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
			
	select count(*) into n from dba_tablespaces where tablespace_name = '&&indexTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&indexTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
	
	select count(*) into n from dba_tablespaces where tablespace_name = '&&lobsTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&lobsTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
	
	select count(*) into n from dba_tablespaces where tablespace_name = '&&auditTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&auditTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
end;
/
show errors;
