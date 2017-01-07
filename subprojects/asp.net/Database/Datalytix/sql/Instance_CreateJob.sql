-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

--Creation script for Instance_CreateJob.sql

--#########################################################
--CREATE JOB
--#########################################################

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
GRANT CREATE PROCEDURE TO &&globalSchemaName;
GRANT CREATE JOB TO &&globalSchemaName;

CONNECT &&globalSchemaName/&&globalSchemaPass@&&serverName

-- Create PartitionManagement package
@@Instance_PartitionManagement.sql

-- Create DBMS JOB
VARIABLE jobno number;
BEGIN
   -- Delete existed jobs	
    FOR existedJob IN (
      SELECT *
        FROM dba_jobs
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

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
REVOKE CREATE PROCEDURE FROM &&globalSchemaName;
REVOKE CREATE JOB FROM &&globalSchemaName;


