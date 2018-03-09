-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

--Creation script for Instance_PartitionManagment.sql

CREATE OR REPLACE PACKAGE PartitionManagment AS
    /******************************************************************************
       NAME:       PartitionManagment
       PURPOSE:

       REVISIONS:
       Ver        Date        Author           Description
       ---------  ----------  ---------------  ------------------------------------
       1.0        04/15/2008   Fari               Created this package.
       1.0        07/31/2009   Fari               Partitioning by id instead of date.    
       2.0        10/15/2014   Brian              Manage partition on multipule database instance
    ******************************************************************************/

    PROCEDURE UpdateHitlistPartitions   (ACurrentDate IN DATE, ASchemaName VARCHAR2);
    PROCEDURE ValidateAndCreatePartitions (ATableName IN VARCHAR2, ACurrentDate IN DATE, ASchemaName VARCHAR2);
    PROCEDURE ValidateAndDeletePartitions (ATableName IN VARCHAR2, ACurrentDate IN DATE, ASchemaName VARCHAR2);
    PROCEDURE UpdatePartitions   (ATableName IN VARCHAR2, ACurrentDate IN DATE, ASchemaName VARCHAR2);
    PROCEDURE UpdatePartitionJob (ATableName IN VARCHAR2, ACurrentDate IN DATE, ASchemaName VARCHAR2);
    PROCEDURE ValidatePartitionSupport;

    Debuging Constant boolean:=False;

    eGenericException Constant Number:=-20000;

    eCreatePartitions    Constant Number:=-20001;
    eDeletePartitions    Constant Number:=-20002;
    eUpdatePartitions    Constant Number:=-20003;
    eUpdatePartitionJob  Constant Number:=-20004;

    eOraclePartitionsnotsupported Constant Number:=-20010;
    ePartitionParamManagmentError Constant Number:=-20011;
    eValidatePartitionSupport     Constant Number:=-20012;
    
    /* Partition range by CHILDHITLISTID column. Each partition
       will store all the hits for 1000 child queries. */
    CountHitListIDByPartition Constant Number:= 1000;
    LiveDays Constant Number:= 0;
    LiveWeeks Constant Number:= 0;
    LiveMonths Constant Number:= 6;
    HoursBetweenUpdate Constant Number:= 12;

END PartitionManagment;
/

CREATE OR REPLACE PACKAGE BODY PartitionManagment IS

/*PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
    INSERT INTO LOG(LogProcedure,LogComment) VALUES($$plsql_unit||'.'||ALogProcedure,ALogComment);
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN NULL; --If logs don't work then don't stop
END;*/

PROCEDURE ValidatePartitionSupport IS
    LExist Integer;
BEGIN
    SELECT Count(1) INTO LExist
        FROM PRODUCT_COMPONENT_VERSION
        WHERE Product like('%Enterprise%');
    IF LExist=0 THEN
        Raise_Application_Error(eOraclePartitionsnotsupported, 'Error: Oracle Database Partitions isn''t supported '||'-'||DBMS_UTILITY.Format_Error_Stack);
    END IF;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      --$if PartitionManagment.Debuging $then InsertLog('ValidatePartitionSupport',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
      RAISE_APPLICATION_ERROR(eValidatePartitionSupport, DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE ValidateAndCreatePartitions(ATableName IN VARCHAR2,ACurrentDate IN DATE, ASchemaName VARCHAR2) IS
  
  LLastID NUMBER:=0;
  LNextHighValueMax NUMBER:=0;
  LLastNextHighValueMax NUMBER:=0;

  LSQLDDL          VARCHAR2(1000);
  LExist           INTEGER;
  LCount           INTEGER;
  LCountPartition  INTEGER;

  LHighValueMax DBA_Tab_Partitions.high_value%TYPE;
  LHighValueMaxStr VARCHAR2(100);
  LHighValueMaxNum NUMBER:=0;
  
  LSysDate DATE;
  LIndex   Number:=0;

  LColumn_Name DBA_Part_Key_Columns.Column_Name%TYPE;

BEGIN
    --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','BEGIN'); $end null;
     ValidatePartitionSupport;

    --Create partitions

    --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','ATableName='||ATableName); $end null;

    SELECT COUNT(1) INTO LExist FROM DBA_TAB_PARTITIONS WHERE Partition_Name=ATableName||'MAXVALUE' AND Table_Owner=ASchemaName;

    -- If exist partition
    IF LExist > 0 THEN

       --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','Before select'); $end null;

        SELECT COUNT(1) INTO LCountPartition FROM DBA_TAB_PARTITIONS WHERE Table_Owner=ASchemaName AND UPPER(Table_Name)=UPPER(ATableName);

            IF LCountPartition=1 THEN
                LHighValueMaxNum:=1;
                --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions',' LExist=1  LHighValueMaxNum='||LHighValueMaxNum); $end null;
            ELSE
                SELECT HIGH_VALUE
                    INTO  LHighValueMax
                    FROM DBA_TAB_PARTITIONS
                    WHERE UPPER(Table_Name)=UPPER(ATableName) AND Table_Owner=ASchemaName AND Partition_Position=
                        (SELECT MAX(partition_position)-1
                            FROM DBA_TAB_PARTITIONS
                            WHERE UPPER(Table_Name)=UPPER(ATableName) AND Table_Owner=ASchemaName);
                EXECUTE IMMEDIATE 'SELECT '||LHighValueMax||' FROM DUAL ' INTO LHighValueMaxStr;
                --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','LHighValueMaxStr='||LHighValueMaxStr); $end null;
                LHighValueMaxNum:=TO_NUMBER(LHighValueMaxStr);
                --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','LHighValueMaxNum='||LHighValueMaxNum); $end null;
            END IF;

            SELECT  Column_Name
                INTO LColumn_Name
                FROM DBA_Part_Key_Columns
                WHERE NAME = ATableName;

            EXECUTE IMMEDIATE 'SELECT NVL(MAX('||LColumn_Name||'),0) FROM '||ATableName INTO LLastID;
            --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','LLastID='||LLastID); $end null;

            LLastNextHighValueMax:=LHighValueMaxNum;
            LNextHighValueMax:=LHighValueMaxNum;
            LSysDate:=SYSDATE;
            WHILE (LLastID>=LNextHighValueMax) OR (LCountPartition=1) LOOP
                LNextHighValueMax:=LNextHighValueMax+CountHitListIDByPartition;
                LSQLDDL:= 'SELECT MAX(1) FROM '||ATableName||' WHERE '||LColumn_Name||'<'||LNextHighValueMax||' AND '||LColumn_Name||'>='||LLastNextHighValueMax;
                --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','SELECT LSQLDDL='||LSQLDDL); $end null;
                EXECUTE IMMEDIATE LSQLDDL INTO LExist;
                --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','LLastID='||LLastID||' LNextHighValueMax='||LNextHighValueMax||' LExist='||LExist); $end null;
                IF (LLastID<=LNextHighValueMax) OR (LExist=1)THEN
                    LSQLDDL:= 'ALTER TABLE '||ASchemaName||'.'||ATableName||' SPLIT PARTITION '||ATableName||'MAXVALUE AT ('||LNextHighValueMax||') INTO (PARTITION '||ATableName||TO_CHAR(LSysDate+LIndex,'YYYYMMDDHHMISS')||',PARTITION  '||ATableName||'MAXVALUE'||') UPDATE GLOBAL INDEXES';
                    --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions','LSQLDDL='||LSQLDDL); $end null;
                    EXECUTE IMMEDIATE LSQLDDL;
                    LIndex:=LIndex+(1/86400);
                    LLastNextHighValueMax:=LNextHighValueMax;
                END IF;
                LCountPartition:=-1;
            END LOOP;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      --$if PartitionManagment.Debuging $then InsertLog('CreatePartitions',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
      RAISE_APPLICATION_ERROR(eCreatePartitions, DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE ValidateAndDeletePartitions(ATableName IN VARCHAR2,ACurrentDate IN DATE,ASchemaName VARCHAR2) IS

  LLastID NUMBER:=0;
  LSQLDDL VARCHAR2(1000);
  LCount  INTEGER;

  LHighValueStr   VARCHAR2(100);
  LHighValueNum   NUMBER:=0;
  LPartitionStr   VARCHAR2(100);
  LPartitionDate  DATE;
  LDeleteDate     DATE;
  
  LColumn_Name DBA_Part_Key_Columns.Column_Name%TYPE;

  CURSOR C_User_Tab_Partitions(LTableName VARCHAR2)  IS
      SELECT High_Value, Partition_Name
        FROM DBA_TAB_PARTITIONS
        WHERE Table_Name=ATableName AND Table_Owner=ASchemaName;

BEGIN
    --$if PartitionManagment.Debuging $then InsertLog('ValidateAndDeletePartitions','BEGIN'); $end null;

    ValidatePartitionSupport;

    LDeleteDate:=ACurrentDate-(LiveDays+(LiveWeekS*7)+(ADD_MONTHS(ACurrentDate,LiveMonths)-ACurrentDate));

    --$if PartitionManagment.Debuging $then InsertLog('ValidateAndDeletePartitions','LDeleteDate='||LDeleteDate); $end null;
            
    -- Get PARTITION BY column name
    SELECT  Column_Name INTO LColumn_Name  FROM DBA_Part_Key_Columns WHERE NAME = ATableName;
    
    -- Get the max value of the partition by column
    EXECUTE IMMEDIATE 'SELECT NVL(MAX('||LColumn_Name||'),0) FROM '||ATableName INTO LLastID;

    -- Delete expired partitions
    FOR  R_User_Tab_Partitions IN C_User_Tab_Partitions(ATableName) LOOP        
        
        -- Never delete MAX_VALUE partition
        IF R_User_Tab_Partitions.HIGH_VALUE<>'MAXVALUE' THEN
        
            -- Get HIGH_VALUE number for current partition. HIGH_VALUE is LONG() type.
            EXECUTE IMMEDIATE 'SELECT '||R_User_Tab_Partitions.HIGH_VALUE||' FROM DUAL ' INTO LHighValueStr;
            LHighValueNum:=TO_NUMBER(LHighValueStr);
            
            -- If the partition is full
            IF (LHighValueNum < LLastID) THEN

                LPartitionStr:=SUBSTR(R_User_Tab_Partitions.Partition_Name,LENGTH(ATableName)+1,LENGTH(R_User_Tab_Partitions.Partition_Name));
                
                --$if PartitionManagment.Debuging $then InsertLog('ValidateAndDeletePartitions','LPartitionStr='||LPartitionStr); $end null;
                
                LPartitionDate:=TO_DATE(LPartitionStr,'YYYYMMDDHHMISS');
                
                --$if PartitionManagment.Debuging $then InsertLog('ValidateAndDeletePartitions','LPartitionDate='||LPartitionDate); $end null;
                
                -- Delete current partition if it was expired
                IF LPartitionDate < LDeleteDate THEN
                    SELECT COUNT(1) INTO LCount FROM DBA_TAB_PARTITIONS WHERE UPPER(Table_Name)=UPPER(ATableName) AND Table_Owner=ASchemaName;
                    IF LCount>2 THEN
                        LSQLDDL:='ALTER TABLE '||ASchemaName||'.'||ATableName||' DROP PARTITION '||R_User_Tab_Partitions.Partition_Name||' UPDATE GLOBAL INDEXES';
                        --$if PartitionManagment.Debuging $then InsertLog('ValidateAndDeletePartitions','LSQLDDL='||LSQLDDL); $end null;
                
                        EXECUTE IMMEDIATE LSQLDDL;
                    END IF;
                END IF;
            END IF;
        END IF;
    END LOOP;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      --$if PartitionManagment.Debuging $then InsertLog('DeletePartitions',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
      RAISE_APPLICATION_ERROR(eDeletePartitions, DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE UpdateHitListPartitions(ACurrentDate IN DATE,ASchemaName VARCHAR2) IS
BEGIN
    UpdatePartitions('TEMPCHITLIST',ACurrentDate,ASchemaName);
    UpdatePartitions('TEMPCHITLISTID',ACurrentDate,ASchemaName);
END;

PROCEDURE UpdatePartitions(ATableName IN VARCHAR2,ACurrentDate IN DATE,ASchemaName VARCHAR2) IS
BEGIN
    ValidateAndDeletePartitions(ATableName,ACurrentDate,ASchemaName);
    ValidateAndCreatePartitions(ATableName,ACurrentDate,ASchemaName);
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      --$if PartitionManagment.Debuging $then InsertLog('UpdatePartitions',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
      RAISE_APPLICATION_ERROR(eUpdatePartitions, DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE UpdatePartitionJob(ATableName IN VARCHAR2,ACurrentDate IN DATE,ASchemaName VARCHAR2) IS

    LJobWhat  VARCHAR2(2000);
    LJob      Number;
    LInterval DATE;
    LIntervalNum NUMBER;

    CURSOR C_Jobs(AWhat VARCHAR2)  IS
      SELECT Job
        FROM USER_JOBS
        WHERE UPPER(What)=UPPER(AWhat);
BEGIN
    ValidatePartitionSupport;

    --Calculate interval
    LInterval:=SYSDATE+(HoursBetweenUpdate/24);
    LIntervalNum:=HoursBetweenUpdate/24;

    --Create job
    LJobWhat:='PartitionManagment.UpdatePartitions('''||ATableName||''',SYSDATE,'''||ASchemaName||''');';
    FOR  R_Jobs IN C_Jobs(LJobWhat) LOOP
        DBMS_JOB.REMOVE(R_Jobs.Job);
    END LOOP;

    DBMS_JOB.SUBMIT
       (
         job        => LJob
        ,what       => LJobWhat
        ,next_date  => SYSDATE
        ,interval   => 'SYSDATE + '|| LIntervalNum
        ,no_parse   => FALSE
       );

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      --$if PartitionManagment.Debuging $then InsertLog('UpdatePartitionJob',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
      RAISE_APPLICATION_ERROR(eUpdatePartitionJob, DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

END;
/