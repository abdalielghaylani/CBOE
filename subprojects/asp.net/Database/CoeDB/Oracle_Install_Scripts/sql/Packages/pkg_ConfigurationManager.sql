CREATE OR REPLACE PACKAGE COEDB.ConfigurationManager
AS
    PROCEDURE RetrieveConfiguration(ADescription IN VARCHAR2, AClassName OUT VARCHAR2, AConfiguration OUT CLOB);
    FUNCTION RetrieveParameter(ADescription IN VARCHAR2, AParameter IN VARCHAR2) RETURN CLOB;
    PROCEDURE UpdateParameter(ADescription IN VARCHAR2, AParameter IN VARCHAR2, AValue VARCHAR2);
    PROCEDURE InsertConfiguration(ADescription IN VARCHAR2, AClassName IN VARCHAR2, AConfiguration IN CLOB);
    PROCEDURE UpdateConfiguration(ADescription IN VARCHAR2, AClassName IN VARCHAR2, AConfiguration IN CLOB);
    PROCEDURE DeleteConfiguration(ADescription IN VARCHAR2);

    Debuging Constant boolean:=False;

    eGenericException Constant Number:=-20000;
    eNoRowsReturned Constant Number:=-20020;
    eParameterNonexistent Constant Number:=-20021;

    eRetrieveConfiguration Constant Number:=-20001;
    eInsertConfiguration Constant Number:=-20002;
    eUpdateConfiguration Constant Number:=-20003;
    eDeleteConfiguration Constant Number:=-20004;
    
    eRetrieveParameter Constant Number:=-20005;
    eUpdateParameter Constant Number:=-20006;
    
    

END ConfigurationManager;
/

CREATE OR REPLACE PACKAGE BODY COEDB.ConfigurationManager
IS
    PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO LOG(LogProcedure,LogComment) VALUES($$plsql_unit||'.'||ALogProcedure,ALogComment);
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN NULL; --If logs don't work then don't stop
    END;

    PROCEDURE RetrieveConfiguration(ADescription IN VARCHAR2, AClassName OUT VARCHAR2, AConfiguration OUT CLOB) IS
    BEGIN
        SELECT CC.ClassName, CC.ConfigurationXML.getClobVal()
            INTO AClassName,AConfiguration
            FROM CoeConfiguration  CC
            WHERE UPPER(CC.Description)=UPPER(ADescription);

    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationManager.Debuging $then InsertLog('RetrieveConfiguration','Error eetrieving a configuration. '||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eRetrieveConfiguration, 'Error retrieving a configuration.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    --Example: RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="MISC"]/settings/add[@name="SameBatchesIdentity"]/@value');
    FUNCTION RetrieveParameter(ADescription IN VARCHAR2, AParameter IN VARCHAR2) RETURN CLOB IS
        LValue CLOB;
    BEGIN
        SELECT Extract(CC.ConfigurationXML,AParameter).GetClobVal()
            INTO LValue
            FROM CoeConfiguration CC
            WHERE UPPER(CC.Description)=UPPER(ADescription);

        IF LValue IS NULL THEN
            RAISE_APPLICATION_ERROR(eParameterNonexistent, 'Parameter non-existent.');
        END IF;

        RETURN LValue;

    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationManager.Debuging $then InsertLog('RetrieveParameter','Error retrieving a configuration parameter.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eRetrieveParameter, 'Error retrieving a configuration parameter.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    --Example: COEDB.ConfigurationManager.UpdateParameter('Registration','Registration/applicationSettings/groups/add[@name="MISC"]/settings/add[@name="SameBatchesIdentity"]','<add name="SameBatchesIdentity" value="False"></add>');
    PROCEDURE UpdateParameter(ADescription IN VARCHAR2, AParameter IN VARCHAR2, AValue VARCHAR2) IS
    BEGIN
        UPDATE COECONFIGURATION
            SET CONFIGURATIONXML=UpdateXML(CONFIGURATIONXML,AParameter,XmlType(AValue))
            WHERE DESCRIPTION='Registration';
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationManager.Debuging $then InsertLog('UpdateParameter','Error updating a configuration parameter.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eUpdateParameter, 'Error updating a configuration parameter.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    PROCEDURE InsertConfiguration(ADescription IN VARCHAR2, AClassName IN VARCHAR2, AConfiguration IN CLOB) IS
    BEGIN
        INSERT INTO CoeConfiguration(Description,ClassName,ConfigurationXML) VALUES(ADescription,AClassName,XmlType(AConfiguration));
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationManager.Debuging $then InsertLog('InsertConfiguration','Error inserting a configuration.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eRetrieveParameter, 'Error inserting a configuration.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    PROCEDURE UpdateConfiguration(ADescription IN VARCHAR2, AClassName IN VARCHAR2, AConfiguration IN CLOB) IS
    BEGIN
        IF AClassName IS NOT NULL THEN
            UPDATE CoeConfiguration SET ClassName=AClassName WHERE UPPER(Description)=UPPER(ADescription);
        END IF;
        IF AConfiguration IS NOT NULL THEN
            UPDATE CoeConfiguration SET ConfigurationXML=XmlType(AConfiguration) WHERE UPPER(Description)=UPPER(ADescription);
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationManager.Debuging $then InsertLog('UpdateConfiguration','Error updating a configuration.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eUpdateConfiguration, 'Error updating a configuration.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    PROCEDURE DeleteConfiguration(ADescription IN VARCHAR2) IS
    BEGIN
        DELETE CoeConfiguration WHERE UPPER(Description)=UPPER(ADescription);
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationManager.Debuging $then InsertLog('DeleteConfiguration','Error brief description.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eDeleteConfiguration, 'Error deleting a configuration.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

END;
/