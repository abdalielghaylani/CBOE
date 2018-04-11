prompt 
prompt Starting "pkg_RegistrationRLS_body.sql"...
prompt 

CREATE OR REPLACE PACKAGE BODY REGDB.RegistrationRLS AS
/******************************************************************************
   NAME:       RegistrationRLS
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        6/13/2008             1. Created this package body.
******************************************************************************/

    PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO REGDB.LOG(LogProcedure,LogComment) VALUES($$plsql_unit||'.'||ALogProcedure,ALogComment);
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN NULL; --If logs don't work then don't stop
    END;

    PROCEDURE ActivateRLS(AState Varchar2) IS

        LStatement   Varchar2(4000);

        PROCEDURE AddPolicy(AObjectName Varchar2) IS
            LExist Integer;
        BEGIN
            LStatement:='SELECT count(1) FROM DBA_Policies WHERE PF_Owner=''REGDB'' AND Policy_Name='''||AObjectName||'POLICY''';
            EXECUTE IMMEDIATE LStatement INTO LExist;
            $if RegistrationRLS.Debuging $then RegistrationRLS.InsertLog('ActivateRLS AddPolicy','LExist->'||LExist); $end null;

            IF LExist=1 then
                LStatement:='BEGIN DBMS_RLS.drop_policy(''REGDB'','''||AObjectName||''','''||AObjectName||'POLICY''); END;';
                $if RegistrationRLS.Debuging $then RegistrationRLS.InsertLog('ActivateRLS AddPolicy','LStatement->'||LStatement); $end null;
                EXECUTE IMMEDIATE LStatement;
            END IF;
            LStatement:='BEGIN dbms_rls.add_policy(''REGDB'','''||AObjectName||''','''||AObjectName||'POLICY'',''REGDB'',''RegistrationRLS.PeopleProject_RLL_Function'',''select''); END;';
            $if RegistrationRLS.Debuging $then RegistrationRLS.InsertLog('ActivateRLS AddPolicy','LStatement->'||LStatement); $end null;

            EXECUTE IMMEDIATE LStatement;
        END;

        PROCEDURE DropPolicy(AObjectName Varchar2) IS
            LExist Integer;
        BEGIN
            LStatement:='SELECT count(1) FROM DBA_Policies WHERE PF_Owner=''REGDB'' AND Policy_Name='''||AObjectName||'POLICY''';

            EXECUTE IMMEDIATE LStatement INTO LExist;
            $if RegistrationRLS.Debuging $then RegistrationRLS.InsertLog('DropPolicy','LExist->'||LExist); $end null;

            IF LExist=1 then
                LStatement:='BEGIN dbms_rls.drop_policy(''REGDB'','''||AObjectName||''','''||AObjectName||'POLICY''); END;';
                EXECUTE IMMEDIATE LStatement;
            END IF;
        END;

    BEGIN
        $if RegistrationRLS.Debuging $then RegistrationRLS.InsertLog('ActivateRLS AddPolicy','uSER->'||uSER); $end null;
        $if RegistrationRLS.Debuging $then RegistrationRLS.InsertLog('ActivateRLS AddPolicy','sys_context(''userenv'',''session_user'')->'||sys_context('userenv','session_user')); $end null;

        IF UPPER(AState)='Y' OR UPPER(AState)='TRUE' OR UPPER(AState)='REGISTRY LEVEL PROJECTS' OR UPPER(AState)='R' OR UPPER(AState)='BATCH LEVEL PROJECTS' OR UPPER(AState)='B' THEN
            AddPolicy('COMPOUND_MOLECULE');
            AddPolicy('REG_NUMBERS');
            AddPolicy('STRUCTURES');
            AddPolicy('BATCHES');
            AddPolicy('PROJECTS');
            AddPolicy('MIXTURES');
            AddPolicy('TEMPORARY_BATCH');
            LStatement:='ALTER TRIGGER RegDB.TRG_Project_Owner ENABLE';
            EXECUTE IMMEDIATE LStatement;
            IF UPPER(AState)='BATCH LEVEL PROJECTS' OR UPPER(AState)='B' THEN
                COEDB.ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','ActiveRLS','Batch Level Projects' ,NULL,NULL ,NULL , NULL);
            ELSE
                COEDB.ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','ActiveRLS','Registry Level Projects' ,NULL,NULL ,NULL , NULL);
            END IF;
         ELSE
            DropPolicy('COMPOUND_MOLECULE');
            DropPolicy('REG_NUMBERS');
            DropPolicy('STRUCTURES');
            DropPolicy('BATCHES');
            DropPolicy('PROJECTS');
            DropPolicy('MIXTURES');
            DropPolicy('TEMPORARY_BATCH');
            LStatement:='ALTER TRIGGER RegDB.TRG_Project_Owner DISABLE';
			IF(AState <> '') THEN
				COEDB.ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','ActiveRLS','Off' ,NULL,NULL ,NULL , NULL);
			END IF;
            EXECUTE IMMEDIATE LStatement;
         END IF;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if RegistrationRLS.Debuging $then RegistrationRLS.InsertLog('ActivateRLS',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(RegistrationRLS.eGenericException, 'ActivateRLS Error: '|| DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    FUNCTION  GetStateRLS RETURN BOOLEAN IS
    BEGIN
        IF gEnableRLS AND GetConfigStateRLS='T' THEN
            RETURN True;
        ELSE
            RETURN False;
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if RegistrationRLS.Debuging $then InsertLog('GetStateRLS',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'GetStateRLS Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    FUNCTION  GetConfigStateRLS RETURN Varchar2 IS
         LConfigParameterValue   Varchar2(50);
    BEGIN
        LConfigParameterValue:=UPPER(COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ActiveRLS"]/@value'));
        IF LConfigParameterValue='REGISTRY LEVEL PROJECTS' OR LConfigParameterValue='BATCH LEVEL PROJECTS' OR LConfigParameterValue='TRUE' THEN
            RETURN 'T';
        ELSE
            RETURN 'F';
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if RegistrationRLS.Debuging $then InsertLog('GetStateRLS',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'GetStateRLS Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;


    FUNCTION  GetLevelRLS RETURN Varchar2 IS
         LConfigParameterValue   Varchar2(50);
    BEGIN

       RETURN UPPER(COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ActiveRLS"]/@value'));

    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if RegistrationRLS.Debuging $then InsertLog('GetLevelRLS',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'GetLevelRLS Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    PROCEDURE SetEnableRLS(AState IN BOOLEAN) IS
    BEGIN
        gEnableRLS:=AState;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if RegistrationRLS.Debuging $then InsertLog('SetEnableRLS',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'SetEnableRLS error: '||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    PROCEDURE SetExemptRLS(AExemptRLS  Varchar2,AClientID  Varchar2) IS
    BEGIN
        dbms_session.set_context('RegistrationUsersCtx','ExemptRLS',AExemptRLS,null,AClientID);
     EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if RegistrationRLS.Debuging $then InsertLog('SetExemptRLS',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'SetExemptRLS error: '||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;


    FUNCTION PeopleProject_RLL_Function(p_schema IN Varchar2, p_object IN Varchar2) RETURN Varchar2 AS
        LClientID    Varchar2(30);
        LExemptRLS   Varchar2(1);
        LSessionUser Varchar2(30);
    BEGIN

        LSessionUser:=UPPER(SYS_Context('userenv', 'session_user'));
        
        LClientID:=SYS_Context('userenv', 'client_identifier');
        
        IF LClientID IS NULL THEN
            DBMS_Session.Set_Identifier(LSessionUser);
            LClientID:=SYS_Context('userenv', 'client_identifier');
        END IF;

        LExemptRLS:=SYS_Context('RegistrationUsersCtx', 'ExemptRLS');

        $if RegistrationRLS.Debuging $then InsertLog('PeopleProject_RLL_Function',' LClientID='||LClientID||' LExemptRLS='||LExemptRLS||' LSessionUser:'||LSessionUser);  $end null;

        IF LExemptRLS IS NULL THEN

            LExemptRLS:=CoeDB.GetExemptRLSFromPrivileges(LClientID);

            SetExemptRLS(LExemptRLS, LClientID);

            LExemptRLS:=SYS_Context('RegistrationUsersCtx', 'ExemptRLS');

        END IF;

        $if RegistrationRLS.Debuging $then
            IF gEnableRLS
                THEN InsertLog('PeopleProject_RLL_Function','LExemptRLS='||LExemptRLS||' client_identifier='||LClientID||' session_user='||LSessionUser||' gEnableRLS=True');
                ELSE InsertLog('PeopleProject_RLL_Function','LExemptRLS='||LExemptRLS||' client_identifier='||LClientID||' session_user='||LSessionUser||' gEnableRLS=False');
            END IF;
        $end null;

        IF ((NOT gEnableRLS) OR LSessionUser = 'SYS' OR LSessionUser = 'SYSTEM' OR LClientID='REGDB' OR LExemptRLS='1' )   THEN

            RETURN '';

        ELSE
            CASE p_object

                WHEN 'PROJECTS' THEN

                    RETURN 'Project_Internal_ID IN
                             (
                              SELECT PP.ProjectID
                                FROM  VW_PeopleProject PP,People P
                                WHERE  PP.PersonID = P.Person_ID AND P.User_ID =''' || UPPER(sys_context('userenv','client_identifier')) || '''
                             )

                             OR

                             Is_Public = ''T''

                             ';

                WHEN 'BATCHES' THEN

                    RETURN 'Batch_Internal_ID IN
                             (
                              SELECT  BP.BatchID
                                FROM  VW_Batch_Project BP, VW_Project P
                                WHERE BP.ProjectID = P.ProjectID
                             )

                            OR

                            Reg_Internal_ID IN
                             (
                              SELECT RNP.REGID
                                FROM  VW_RegistryNumber_Project RNP, VW_Project P
                                WHERE  RNP.ProjectID = P.ProjectID AND
                                       ( SELECT Count(1) FROM VW_Batch_Project BP WHERE BP.BatchID=BATCHES.Batch_Internal_ID )=0
                             )
                           ';

                WHEN 'MIXTURES' THEN

                    RETURN 'RegID IN
                             (
                              SELECT RNP.RegID
                                FROM   VW_RegistryNumber_Project RNP, VW_PeopleProject PP,VW_Project P
                                WHERE RNP.ProjectID = P.ProjectID
                             )

                             OR

                             RegID IN
                             (
                              SELECT B.RegID
                                FROM VW_Batch B
                             )';

                WHEN 'COMPOUND_MOLECULE' THEN

                    RETURN 'CPD_Database_Counter IN
                            (
                             SELECT MC.CompoundID
                                FROM  VW_Mixture_Component MC,VW_Mixture M
                                WHERE MC.MixtureID=M.MixtureID
                            )';

                WHEN 'REG_NUMBERS' THEN

                    RETURN 'Reg_ID IN
                             (
                              SELECT C.RegID
                                FROM  VW_Compound C
                             )

                            OR

                            Reg_ID IN
                             (
                              SELECT M.RegID
                                FROM  VW_Mixture M
                             )';

                WHEN 'STRUCTURES' THEN

                    RETURN 'CPD_Internal_ID IN
                             (
                              SELECT StructureID
                                FROM  VW_Compound C
                             )

                            OR

                            CPD_Internal_ID < 0';


                 WHEN 'TEMPORARY_BATCH' THEN

                     RETURN 'TempBatchID IN
                              (
                               SELECT TBP.TempBatchID
                                 FROM VW_TemporaryBatchProject TBP, VW_Project P
                                 WHERE TBP.ProjectID = P.ProjectID
                              )
                             OR
                             TempBatchID IN
                              (
                               SELECT TRNP.TempBatchID
                                 FROM VW_TemporaryRegNumbersProject TRNP, VW_Project P
                                 WHERE TRNP.ProjectID = P.ProjectID
                              )
                            ';
                ELSE RETURN '';

            END CASE;
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if RegistrationRLS.Debuging $then InsertLog('PeopleProject_RLL_Function',DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'PeopleProject_RLL_Function Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;


END RegistrationRLS;
/