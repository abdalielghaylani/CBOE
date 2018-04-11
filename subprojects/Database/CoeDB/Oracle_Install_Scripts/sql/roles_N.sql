--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

PROMPT Starting roles.sql new coedb...

Connect &&schemaName/&&schemaPass@&&serverName

CREATE  ROLE css_user NOT IDENTIFIED;
GRANT  "CONNECT" TO "CSS_USER";
CREATE USER cssuser IDENTIFIED BY cssuser DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&temptableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT css_user TO cssuser;
ALTER USER cssuser DEFAULT ROLE ALL;
CREATE  ROLE css_admin NOT IDENTIFIED;
GRANT  "CONNECT" TO "CSS_ADMIN";
CREATE USER cssadmin IDENTIFIED BY cssadmin DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&temptableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT css_admin TO cssadmin;
ALTER USER cssadmin DEFAULT ROLE ALL;


