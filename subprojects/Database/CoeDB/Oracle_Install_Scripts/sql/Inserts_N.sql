-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

PROMPT Inserting starting data for new coedb...

Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--CS-Security
--#########################################################

INSERT INTO privilege_tables (privilege_table_name, app_name, table_space)
     VALUES ('CS_SECURITY_PRIVILEGES', 'CS Security', '&&tableSpaceName');

-- CSS_ADMIN
INSERT INTO security_roles (privilege_table_int_id, role_name,coeidentifier)
     VALUES (privilege_tables_seq.CURRVAL, 'CSS_ADMIN', 'COE');

INSERT INTO cs_security_privileges
            (role_internal_id, css_login, css_create_user, css_edit_user,
             css_delete_user, css_change_password, css_create_role,
             css_edit_role, css_delete_role, css_create_workgrp,
             css_edit_workgrp, css_delete_workgrp)
     VALUES (security_roles_seq.CURRVAL, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1);

-- CSS_USER
INSERT INTO security_roles (privilege_table_int_id, role_name,coeidentifier)
     VALUES (privilege_tables_seq.CURRVAL, 'CSS_USER', 'COE');

INSERT INTO cs_security_privileges
            (role_internal_id, css_login, css_create_user, css_edit_user,
             css_delete_user, css_change_password, css_create_role,
             css_edit_role, css_delete_role, css_create_workgrp,
             css_edit_workgrp, css_delete_workgrp)
     VALUES (security_roles_seq.CURRVAL, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);

--SITES
INSERT INTO sites (site_id, site_code, site_name, active)
     VALUES ('1', '0', 'unspecified', '1');

INSERT INTO people (user_id, user_code, supervisor_internal_id, last_name, site_id, active)
     VALUES ('CSSUSER', 'CSSUSER', '1', 'CSSUSER', '1', '1');

INSERT INTO people (user_id, user_code, supervisor_internal_id, last_name, site_id, active)
     VALUES ('CSSADMIN', 'CSSADMIN', '1', 'CSSADMIN', '1', '1');

UPDATE cs_security_privileges
   SET css_change_password = 1
 WHERE role_internal_id = (SELECT role_id FROM security_roles WHERE role_name = 'CSS_ADMIN');

--PEOPLE
INSERT INTO people (user_id, user_code, supervisor_internal_id, last_name, site_id, active)
     VALUES ('login_disabled', 'C0', '1', 'unspecified', '1', '1');


-- This file provides the mapping between CS_SECURITY Privileges and Oracle
-- Object Privileges.  

-- CSS_LOGIN PRIVS
INSERT INTO object_privileges VALUES ('CSS_LOGIN', 'EXECUTE', 'CS_SECURITY', 'LOGIN');
INSERT INTO object_privileges VALUES ('CSS_LOGIN', 'EXECUTE', 'CS_SECURITY', 'MANAGE_ROLES');
INSERT INTO object_privileges VALUES ('CSS_LOGIN', 'EXECUTE', 'CS_SECURITY', 'MANAGE_USERS');
INSERT INTO object_privileges VALUES ('CSS_LOGIN', 'SELECT', 'CS_SECURITY', 'PEOPLE');
INSERT INTO object_privileges VALUES ('CSS_LOGIN', 'SELECT', 'CS_SECURITY', 'PRIVILEGE_TABLES');
INSERT INTO object_privileges VALUES ('CSS_LOGIN', 'SELECT', 'CS_SECURITY', 'SITES');
INSERT INTO object_privileges VALUES ('CSS_LOGIN', 'SELECT', 'CS_SECURITY', 'SECURITY_ROLES');

-- CSS_CREATE_USER PRIVS
INSERT INTO object_privileges VALUES ('CSS_CREATE_USER', 'EXECUTE', 'CS_SECURITY', 'CREATEUSER');

-- CSS_EDIT_USER PRIVS
INSERT INTO object_privileges VALUES ('CSS_EDIT_USER', 'EXECUTE', 'CS_SECURITY', 'UPDATEUSER');

-- CSS_DELETE_USER PRIVS
INSERT INTO object_privileges VALUES ('CSS_DELETE_USER', 'EXECUTE', 'CS_SECURITY', 'DELETEUSER');

-- CSS_CREATE_ROLE PRIVS
INSERT INTO object_privileges VALUES ('CSS_CREATE_ROLE', 'EXECUTE', 'CS_SECURITY', 'CREATEROLE');

-- CSS_EDIT_ROLE PRIVS
INSERT INTO object_privileges VALUES ('CSS_EDIT_ROLE', 'EXECUTE', 'CS_SECURITY', 'UPDATEROLE');

-- CSS_DELETE_ROLE PRIVS
INSERT INTO object_privileges VALUES ('CSS_DELETE_ROLE', 'EXECUTE', 'CS_SECURITY', 'DELETEROLE');

-- CSS_CHANGE_PASSWORD PRIVS
INSERT INTO object_privileges VALUES ('CSS_CHANGE_PASSWORD', 'EXECUTE', 'CS_SECURITY', 'CHANGEPWD');

COMMIT ;

-- CSS_CREATE_WORKGRP PRIVS

-- CSS_UPDATE_WORKGRP PRIVS

-- CSS_DELETE_WORKGRP PRIVS