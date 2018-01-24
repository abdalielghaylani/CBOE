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

        IF UPPER(AState)='Y' OR UPPER(AState)='TRUE' THEN
            AddPolicy('COMPOUND_MOLECULE');
            AddPolicy('REG_NUMBERS');
            AddPolicy('STRUCTURES');
            AddPolicy('BATCHES');
            AddPolicy('PROJECTS');
            COEDB.ConfigurationManager.UpdateParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ActiveRLS"]','<add name="ActiveRLS" value="True"></add>');
         ELSE
            DropPolicy('COMPOUND_MOLECULE');
            DropPolicy('REG_NUMBERS');
            DropPolicy('STRUCTURES');
            DropPolicy('BATCHES');
            DropPolicy('PROJECTS');
            COEDB.ConfigurationManager.UpdateParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ActiveRLS"]','<add name="ActiveRLS" value="False"></add>');
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
        IF gEnableRLS AND COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ActiveRLS"]/@value')='True' THEN
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

    FUNCTION PeopleProject_RLL_Function(p_schema IN varchar2, p_object IN varchar2) RETURN varchar2 AS
    BEGIN
     InsertLog('SetEnableRLS',sys_context('userenv','session_user'));
        IF ((NOT gEnableRLS) OR sys_context('userenv','session_user') = 'REGDB' OR sys_context('userenv','session_user') = 'SYSTEM') THEN
            RETURN '';
        ELSE
            IF  (p_object='COMPOUND_MOLECULE') THEN
                RETURN ' CPD_Database_Counter IN ( SELECT MC.CompoundID
                           FROM  VW_Mixture_Component MC,VW_Mixture M,VW_RegistryNumber_Project RNP, VW_Project P
                           WHERE MC.MixtureID=M.MixtureID AND M.RegID=RNP.RegID AND RNP.ProjectID = P.ProjectID )';
            ELSIF (p_object='BATCHES') THEN
                RETURN ' ( Batch_Internal_ID IN ( SELECT  BP.BatchID
                           FROM  VW_Batch_Project BP, VW_Project P
                           WHERE BP.ProjectID = P.ProjectID ))'||
                       ' OR
                         ( Batch_Internal_ID IN ( SELECT RNP.REGID
                          FROM  VW_RegistryNumber_Project RNP, VW_Project P
                          WHERE  RNP.ProjectID = P.ProjectID
                          AND (SELECT Count(1) FROM VW_Batch_Project BP WHERE BP.BatchID=BATCHES.Batch_Internal_ID)=0))';
            ELSIF (p_object='REG_NUMBERS') THEN
                RETURN ' Reg_ID IN ( SELECT C.RegID
                           FROM  VW_Compound C) '||
                       ' OR
                           Reg_ID IN ( SELECT RNP.RegId
                            FROM   VW_RegistryNumber_Project RNP, VW_PeopleProject PP,VW_Project P
                            WHERE RNP.ProjectID = P.ProjectID)';
            ELSIF (p_object='STRUCTURES') THEN
                RETURN '  CPD_Internal_ID IN ( SELECT StructureID
                           FROM  VW_Compound C)';
            ELSIF (p_object='PROJECTS') THEN
                RETURN ' Project_Internal_ID IN ( SELECT PP.ProjectID
                           FROM  VW_PeopleProject PP,People P
                           WHERE  PP.PersonID = P.Person_ID AND P.User_ID =''' || sys_context('userenv','session_user') || ''')';
            ELSE
                RETURN '';
            END IF;
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