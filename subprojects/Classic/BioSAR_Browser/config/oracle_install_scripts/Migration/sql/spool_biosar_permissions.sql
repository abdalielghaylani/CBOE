set linesize 400
set heading off
set pagesize 0
set verify off
set feedback off
set echo off


Connect &&InstallUser/&&sysPass@&&serverName

spool ../exported_data/recreate_biosar_roles_and_grants.sql

--spool that you are creating formgrou roles
prompt --Create formgroup roles

select  'DECLARE ' from dual;
select 	' 	PROCEDURE CreateRole(roleName IN varchar2) IS ' from dual;
select	'		n NUMBER;' from dual;
select  '	BEGIN ' from dual;
select  '		select count(*) into n from dba_roles where Upper(role) = roleName; ' from dual;
select	'		if n > 0 then ' from dual;
select  '		   execute immediate ''DROP ROLE '' || roleName;' from dual;
select	'		end if;' from dual;
select	'		execute immediate ''CREATE ROLE '' || roleName|| '' NOT IDENTIFIED'';' from dual;
select	'	END CreateRole;' from dual;
select  'BEGIN ' from dual; 

select 'CreateRole('''||role_name||''');'
from &&securitySchemaName..security_roles rls
inner join &&securitySchemaName..BIOSAR_BROWSER_PRIVILEGES privs
	ON rls.ROLE_ID = privs.ROLE_INTERNAL_ID
where privs.IS_FORMGROUP_ROLE  = 1
group by role_name;
select 'null;' from dual;
select 'END; ' from dual;
select '/' from dual;

-- First create roles for only biosar formgrou roles

--  Now label that you are granting roles to roles
select chr(10)||'--Grant roles to roles' from dual
/

--Now do biosar to biosar grants
select 'grant ' || granted_role || ' to ' || grantee || ';' ||chr(39)||chr(10) col1
from SYS.DBA_ROLE_PRIVS
where granted_role IN (select upper(role_name) from &&securitySchemaName..security_roles rls
inner join &&securitySchemaName..BIOSAR_BROWSER_PRIVILEGES privs
	ON ROLE_ID = privs.ROLE_INTERNAL_ID) 
/


-- Grant object object privileges to roles
select chr(10)||'--Grant object privileges to roles' from dual
/

((SELECT 'GRANT '||s.privilege||' ON '||s.owner||'.'||s.table_name||' TO '||s.grantee||DECODE(s.grantable,'YES', ' WITH GRANT OPTION')||';' col1 FROM
	sys.dba_tab_privs s WHERE  s.grantee IN (select upper(role_name) from &&securitySchemaName..security_roles rls
inner join &&securitySchemaName..BIOSAR_BROWSER_PRIVILEGES privs
	ON rls.ROLE_ID = privs.ROLE_INTERNAL_ID)) UNION
	(SELECT 'GRANT '||s.privilege||' ON '||s.owner||'.'||s.table_name||'.'||s.column_name||' TO '||s.grantee||DECODE(s.grantable,'YES', ' WITH GRANT OPTION')||';' col1 FROM
	sys.dba_col_privs s WHERE  s.grantee IN (select upper(role_name) from &&securitySchemaName..security_roles rls
inner join &&securitySchemaName..BIOSAR_BROWSER_PRIVILEGES privs
	ON rls.ROLE_ID = privs.ROLE_INTERNAL_ID))) order by 1
/


spool off

       


