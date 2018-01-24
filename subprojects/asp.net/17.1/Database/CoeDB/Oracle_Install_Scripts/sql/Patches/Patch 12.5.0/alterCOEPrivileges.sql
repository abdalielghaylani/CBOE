Alter table CS_SECURITY_PRIVILEGES
add(
CSS_MANAGE_GROUPS NUMBER(1)
);

update CS_SECURITY_PRIVILEGES
set
CSS_MANAGE_GROUPS=0;

update CS_SECURITY_PRIVILEGES
set
CSS_MANAGE_GROUPS=1
where role_internal_id=(select role_id from security_roles where role_name='CSS_ADMIN');

/