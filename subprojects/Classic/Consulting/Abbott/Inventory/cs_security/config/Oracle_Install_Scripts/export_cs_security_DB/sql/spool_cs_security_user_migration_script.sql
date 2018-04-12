--
--  This will generate a script called cr_usr_new_instace.sql
--  that can be used to create the user ids on the new instance.
set heading off
set pagesize 0
set verify off
set feedback off 
set echo off

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the source Oracle service name:'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):'


Connect &&InstallUser/&&sysPass@&&serverName

spool migrate_cs_security_users.sql

select 'spool log_migrate_cs_security_users.txt'    
from dual
/

-- Create cs_security roles
select chr(10)||'--Create cs_security roles' from dual
/

select 'create role ' ||role_name|| chr(10)|| 'not identified;' 
from cs_security.security_roles
group by role_name
/


-- Grant roles to roles
select chr(10)||'--Grant roles to roles' from dual
/

select 'grant ' || granted_role || ' to ' || grantee || ';' ||chr(39)||chr(10)
from SYS.DBA_ROLE_PRIVS 
where granted_role IN (select upper(role_name) from cs_security.security_roles)
/


-- Grant system privileges to roles
select chr(10)||'--Grant system privileges to roles' from dual
/

select 'grant ' || privilege || ' to ' || grantee || ';' ||chr(39)||chr(10)
from SYS.DBA_SYS_PRIVS 
where grantee IN (select upper(role_name) from cs_security.security_roles)
/

-- Grant object object privileges to roles
select chr(10)||'--Grant object privileges to roles' from dual
/

((SELECT 'GRANT '||s.privilege||' ON '||s.owner||'.'||NVL(l.longname, s.table_name)||' TO '||s.grantee||DECODE(s.grantable,'YES', ' WITH GRANT OPTION')||';' FROM
	sys.dba_tab_privs s, javasnm l WHERE s.table_name = l.short(+) AND s.grantee IN (select upper(role_name) AS role_name from cs_security.security_roles)) UNION
	(SELECT 'GRANT '||s.privilege||' ON '||s.owner||'.'||s.table_name||'.'||s.column_name||' TO '||s.grantee||DECODE(s.grantable,'YES', ' WITH GRANT OPTION')||';' FROM
	sys.dba_col_privs s WHERE s.grantee IN (select upper(role_name) AS role_name from cs_security.security_roles))) order by 1
/


-- Create users
--select 'create user ' ||username|| chr(10)|| 'identified by values '
--       ||chr(39)||max(password)||chr(39)||chr(10)||' default tablespace '
--       ||max(default_tablespace)||chr(10)|| ' temporary tablespace '
--       ||max(temporary_tablespace)||';'
--from dba_users,
--     dba_role_privs
--where username = grantee 
--and granted_role  IN (select upper(role_name) from cs_security.security_roles)
--group by username-
--/


-- Create cs_security users
select chr(10)||'--Create cs_security users' from dual
/

select 'create user ' ||username|| chr(10)|| 'identified by values '
       ||chr(39)||password||chr(39)||chr(10)||' default tablespace '
       ||default_tablespace||chr(10)|| ' temporary tablespace '
       ||temporary_tablespace||';'
from dba_users u,
     cs_security.people p
where u.username = Upper(p.user_id) 
order by username
/

-- Grant system privileges to users
select chr(10)||'--Grant system privileges to users' from dual
/

select 'grant ' || privilege || ' to ' || grantee || ';' ||chr(39)||chr(10)
from SYS.DBA_SYS_PRIVS 
where grantee IN (select upper(user_id) from cs_security.people)
/


-- Grant object privileges to users
select chr(10)||'--Grant object privileges to users' from dual
/

((SELECT 'GRANT '||s.privilege||' ON '||s.owner||'.'||NVL(l.longname, s.table_name)||' TO '||s.grantee||DECODE(s.grantable,'YES', ' WITH GRANT OPTION')||';' FROM
	sys.dba_tab_privs s, javasnm l WHERE s.table_name = l.short(+) AND s.grantee IN (select upper(user_id) AS user_id from cs_security.people)) UNION
	(SELECT 'GRANT '||s.privilege||' ON '||s.owner||'.'||s.table_name||'.'||s.column_name||' TO '||s.grantee||DECODE(s.grantable,'YES', ' WITH GRANT OPTION')||';' FROM
	sys.dba_col_privs s WHERE s.grantee IN (select upper(user_id) AS user_id from cs_security.people))) order by 1
/


select 'spool off'
from dual
/

spool off
exit


