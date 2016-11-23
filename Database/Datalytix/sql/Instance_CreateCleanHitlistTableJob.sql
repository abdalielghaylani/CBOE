-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

--Creation script for Instance_CreateCleanHitlistTableJob.sql

--#########################################################
--CREATE CLEAN HITLIST TABLE JOB
--#########################################################

PROMPT Starting Instance_CreateCleanHitlistTableJob.sql...

CONNECT &&globalSchemaName/&&globalSchemaPass@&&serverName

-- Create PartitionManagement package
@@Instance_PartitionManagement.sql

-- Create DBMS JOB
VARIABLE jobno number;
BEGIN
   -- Delete existed jobs	
    FOR existedJob IN (
      SELECT *
        FROM user_jobs
        WHERE UPPER(schema_user) = UPPER('&&globalSchemaName')
          AND UPPER(WHAT) LIKE UPPER('PartitionManagment.UpdateHitlistPartitions(SYSDATE%'))
    LOOP
      DBMS_JOB.REMOVE(existedJob.job);
    END LOOP;

    -- Create new job
    DBMS_JOB.SUBMIT(
      :jobno, 
      'PartitionManagment.UpdateHitlistPartitions(SYSDATE, ''' || UPPER ('&&globalSchemaName') || ''');',
      SYSDATE,
      'SYSDATE + .5');
    COMMIT;
END;
/
