--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

PROMPT Starting roles.sql
Connect &&schemaName/&&schemaPass@&&serverName





--#########################################################
--CREATE ROLES
--#########################################################

CREATE ROLE COE_DV_ADMIN NOT IDENTIFIED;
	GRANT "CONNECT" TO "COE_DV_ADMIN";

GRANT COE_DV_ADMIN TO CSSADMIN;

CREATE ROLE COE_SEC_ADMIN NOT IDENTIFIED;
	GRANT "CONNECT" TO "COE_SEC_ADMIN";

GRANT COE_SEC_ADMIN TO CSSADMIN;
