-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

--Creation script for Instance_CreateJob.sql

--#########################################################
--CREATE JOB
--#########################################################

PROMPT Starting Instance_CreateJob.sql...

-- Connect and assign privillege
CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
GRANT CREATE PROCEDURE TO &&globalSchemaName;
GRANT CREATE JOB TO &&globalSchemaName;

-- Create clean hitlist table job
COL nextScript NEW_VALUE nextScript NOPRINT
SELECT	CASE
		WHEN  (SELECT COUNT(1) FROM v$option where Upper(parameter) = 'PARTITIONING' and Upper(value) = 'TRUE') > 0
		THEN  'Instance_CreateCleanHitlistTableJob.sql'
		ELSE  'continue.sql'
	END	AS nextScript 
FROM	DUAL;
@@&&nextScript 


-- Create other jobs if have
-- ...

-- Connect and revoke privillege
CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
REVOKE CREATE PROCEDURE FROM &&globalSchemaName;
REVOKE CREATE JOB FROM &&globalSchemaName;


