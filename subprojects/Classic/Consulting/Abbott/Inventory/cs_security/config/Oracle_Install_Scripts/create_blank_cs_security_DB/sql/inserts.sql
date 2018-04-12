--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

prompt 'Inserting Data...'
prompt '------------------------'

--PEOPLE
INSERT INTO PEOPLE (USER_ID,USER_CODE,SUPERVISOR_INTERNAL_ID,LAST_NAME,SITE_ID,ACTIVE)values('login_disabled','C0','1','unspecified','1','1');

--SITES
INSERT INTO SITES(SITE_ID,SITE_CODE,SITE_NAME,ACTIVE)values('1','0','unspecified','1');

-- PRIVILEGES
GRANT EXECUTE ON AUDIT_TRAIL TO CSS_USER;
GRANT EXECUTE ON AUDIT_TRAIL TO CSS_ADMIN;


commit;



