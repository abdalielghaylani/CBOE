CREATE OR REPLACE FUNCTION CS_SECURITY.DELETEUSER(
	pUserName IN people.user_id%type) RETURN varchar2 AS
    cannot_drop_connected_user exception;
	pragma exception_init (cannot_drop_connected_user, -1940);
BEGIN
	EXECUTE IMMEDIATE 'DROP USER ' || pUserName || ' CASCADE';
	update people set active = 0 where user_id = pUserName;
	RETURN '1';
EXCEPTION
	WHEN cannot_drop_connected_user then
		RETURN 'Cannot drop user ' || pUserName || ' because it is currently connected to Oracle';
END DELETEUSER;
