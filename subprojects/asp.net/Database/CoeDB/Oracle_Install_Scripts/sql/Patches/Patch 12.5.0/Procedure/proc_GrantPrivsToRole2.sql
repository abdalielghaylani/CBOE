create or replace procedure GrantPrivsToRole2(p_roleName security_roles.role_name%TYPE)
is
l_privilegeTable privilege_tables.privilege_table_name%TYPE;
l_roleId security_roles.role_id%TYPE;
l_privValue NUMBER;
BEGIN

					--' get priv table name and role_id
					SELECT role_id, privilege_table_name INTO l_roleId, l_privilegeTable FROM security_roles, privilege_tables WHERE privilege_table_int_id = privilege_table_id AND role_name = p_roleName;


					--' To avoid role dependencies it is best to process first the revokes and then the grants.
					-- That will lead to a consistent grant state
					--' loop over the privileges
					FOR priv_rec IN (SELECT  * FROM user_tab_columns WHERE column_name <> 'ROLE_INTERNAL_ID' AND column_name <> 'RID' AND column_name <> 'CREATOR' AND column_name <> 'TIMESTAMP' AND table_name = l_privilegeTable)
					LOOP
										--' revoke the grant for privileges that are not set.
										EXECUTE IMMEDIATE 'SELECT ' || priv_rec.column_name || ' FROM ' || l_privilegeTable || ' WHERE role_internal_id = ' || l_roleId  INTO l_privValue;
										IF l_privValue <> 1 THEN
													mapprivstorole(p_roleName, priv_rec.column_name, 'REVOKE');
										END IF;
					END LOOP;

					--' loop over the privileges
					FOR priv_rec IN (SELECT  * FROM user_tab_columns WHERE column_name <> 'ROLE_INTERNAL_ID' AND column_name <> 'RID' AND column_name <> 'CREATOR' AND column_name <> 'TIMESTAMP'  AND table_name = l_privilegeTable)
					LOOP
										--' issue the grant if the role has the privilege
										EXECUTE IMMEDIATE 'SELECT ' || priv_rec.column_name || ' FROM ' || l_privilegeTable || ' WHERE role_internal_id = ' || l_roleId  INTO l_privValue;
										IF l_privValue = 1 THEN
													mapprivstorole(p_roleName, priv_rec.column_name, 'GRANT');
										END IF;
					END LOOP;

END GrantPrivsToRole2;
/