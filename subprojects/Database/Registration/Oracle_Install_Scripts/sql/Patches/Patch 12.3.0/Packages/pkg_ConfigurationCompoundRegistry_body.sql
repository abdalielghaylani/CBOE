prompt 
prompt Starting "pkg_ConfigurationCompoundRegistry_body.sql"...
prompt

CREATE OR REPLACE PACKAGE BODY ConfigurationCompoundRegistry AS

    ErrorsLog Clob;

    -- Forward declarations
    FUNCTION GetDefaultMultiCompound RETURN XmlType;
    FUNCTION GetDefaultConfigXml RETURN XmlType;
    
    VW_MIXTURE_REGNUMBER VARCHAR2(1000):='CREATE OR REPLACE VIEW &&schemaName..VW_MIXTURE_REGNUMBER AS
        SELECT    M.*, R.REGNUMBER, R.SequenceNumber
        FROM     VW_MIXTURE M, VW_REGISTRYNUMBER R
        WHERE     M.REGID = R.REGID';

    VW_MIXTURE_BATCH VARCHAR2(1000):='CREATE OR REPLACE VIEW &&schemaName..VW_MIXTURE_BATCH AS
        SELECT     M.MIXTUREID, M.STRUCTUREAGGREGATION, R.REGNUMBER, B.*
        FROM     &&schemaName..VW_MIXTURE M, &&schemaName..VW_REGISTRYNUMBER R, &&schemaName..VW_BATCH B
        WHERE     M.REGID = R.REGID AND R.REGID = B.REGID';

   VW_MIXTURE_BATCHCOMPONENT VARCHAR2(1000):='CREATE OR REPLACE VIEW &&schemaName..VW_MIXTURE_BATCHCOMPONENT AS
        SELECT    MC.MIXTUREID, BC.*
        FROM     VW_MIXTURE_COMPONENT MC, VW_BATCHCOMPONENT BC
        WHERE     MC.MIXTURECOMPONENTID = BC.MIXTURECOMPONENTID';

    PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO LOG(LogProcedure,LogComment) VALUES($$plsql_unit||'.'||ALogProcedure,ALogComment);
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN NULL; --If logs don't work then don't stop
    END;

    PROCEDURE SetSessionParameter IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        DBMS_SESSION.set_nls('NLS_DATE_FORMAT','''YYYY-MM-DD HH:Mi:SS''');
        DBMS_SESSION.set_nls('NLS_NUMERIC_CHARACTERS', '''.,''');
        COMMIT; --It is necesary to finished the Autonomou-Transaction
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetSessionParameter'||' Line:'||$$plsql_line,DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'SetSessionParameter'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
     END;

     PROCEDURE CompileView(AViewName varchar2) is
        LStatement     Varchar2(4000);    --view definition
        LSubQuery    Varchar2(4000);    --subquery in view definition
        LViewName    Varchar2(50) := AViewName;     --view to be recompiled
        LViewAlias    Varchar2(800);    --field alias of the view
        CURSOR LCViewFields(ViewName VARCHAR2) IS
            SELECT Column_Name
                FROM User_Tab_Columns c
                WHERE Table_Name = ViewName
                ORDER BY Column_ID;
     begin
        LViewAlias:='';
        FOR  RViewFields IN LCViewFields(LViewName) LOOP
                     LViewAlias:=LViewAlias||RViewFields.Column_Name||',';
        END LOOP;
        LViewAlias := SUBSTR(LViewAlias,1,LENGTH(LViewAlias)-1); --remove last ','
        SELECT Text
            INTO LSubQuery
            FROM USER_VIEWS
            WHERE VIEW_NAME=LViewName;

        LStatement := 'CREATE OR REPLACE FORCE VIEW '||LViewName||' ('|| LViewAlias || ') as '||LSubQuery;
        Execute Immediate LStatement;
     end;

     PROCEDURE GetDBFieldName(AField IN VARCHAR2, AViewField OUT VARCHAR2, ATableField OUT VARCHAR2, ASection IN VARCHAR2, AType in VARCHAR2) IS
     BEGIN
         AViewField:=UPPER(AField);
         ATableField:=UPPER(AField);
         CASE UPPER(ASection)
             WHEN 'MIXTURE' THEN
                 BEGIN
                     NULL;
                 END;
             WHEN 'BATCH' THEN
                 BEGIN
                     CASE UPPER(AType)
                         WHEN 'DEFINITIVE' THEN
                             BEGIN
                                 IF ATableField='OPTICALROTATION' THEN
                                     ATableField:='OPTICAL_ROTATION';
                                 END IF;
                                 IF ATableField='REFRACTIONINDEX' THEN
                                     ATableField:='REFRACTIVE_INDEX';
                                 END IF;
                             END;
                         WHEN 'TEMPORARY' THEN
                             BEGIN
                                 NULL;
                             END;
                     END CASE;
                 END;
             WHEN 'COMPOUND' THEN
                 BEGIN
                     NULL;
                 END;
             WHEN 'BATCHCOMPONENT' THEN
                 BEGIN
                      CASE UPPER(AType)
                         WHEN 'DEFINITIVE' THEN
                             BEGIN
                                 NULL;
                             END;
                         WHEN 'TEMPORARY' THEN
                             BEGIN
                                 IF ATableField='COMMENTS' THEN
                                     ATableField:='BATCHCOMPONENT_COMMENTS';
                                 END IF;
                             END;
                     END CASE;
                 END;
             WHEN 'STRUCTURE' THEN
                 BEGIN
                     NULL;
                 END;
         END CASE;
     END;

     PROCEDURE AddErrorLog(AOperation IN VARCHAR2,AField IN VARCHAR2, AObject IN VARCHAR2, AFieldType IN VARCHAR2,ASection IN VARCHAR2) IS
     BEGIN
        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddErrorLog'||' Line:'||$$plsql_line,'Error '||AOperation||' the field "'||AField||'" in "'||ASection||'"-> '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
        ErrorsLog:=ErrorsLog||
        '     <Error>'||CHR(10)||
        '          <Operation>'||AOperation||'</Operation>'||CHR(10)||
        '          <Field>'||AField||'</Field>'||CHR(10)||
        '          <Object>'||AObject||'</Object>'||CHR(10)||
        '          <Fieldtype>'||AFieldType||'</Fieldtype>'||CHR(10)||
        '          <Section>'||ASection||'</Section>'||CHR(10)||
        '          <Message>'||SUBSTR(DBMS_UTILITY.FORMAT_ERROR_STACK,1,LENGTH(DBMS_UTILITY.FORMAT_ERROR_STACK)-1)||'</Message>'||CHR(10)||
        '          <ErrorCode>'||SQLCODE||'</ErrorCode>'||CHR(10)||
        '     </Error>'||CHR(10);
     END;

     /* Make sure all records in ATable does not have value (that is , null) in AField.
      * It will raise an exception if there are any records have a nonenull value in AField.
      * */
     PROCEDURE VerifyEmptyField(AField IN VARCHAR2, ATable IN VARCHAR2) IS
         LCount      Number;
         LStatement  Varchar2(4000);
     BEGIN
         LStatement:='SELECT COUNT(1) FROM '||ATable||' WHERE ROWNUM<2 AND '||AField||' IS NOT NULL';
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEmptyField'||' Line:'||$$plsql_line,'LStatement='||LStatement); $end null;
         BEGIN
             EXECUTE IMMEDIATE LStatement INTO LCount;
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'LCount='||LCount); $end null;
             IF LCount>0 THEN
                 RAISE_APPLICATION_ERROR (eFieldNotEmpty,'Field not empty.');
             END IF;
         END;
     END;

     PROCEDURE AddField(
       AField IN VARCHAR2
       , ASection IN VARCHAR2
       , AFieldType IN VARCHAR2
       , LNewColumnDefaultValue IN CLOB
     ) IS
       LStatement   Varchar2(4000);
       LViewField   Varchar2(100);
       LTableField  Varchar2(100);
       LObject      Varchar2(100);

         /* Construct a sql statement, which can be used to recreat view AViewName
      * this sql statement will be as it is if LTableField already exists in AViewName, or with a new field LViewField if it does not.
      * */
         FUNCTION AddFieldToView(AViewName VARCHAR2,APrimaryTable in VARCHAR2:='') RETURN VARCHAR2 IS
             LViewSelect VARCHAR2(10000);
             LViewFields VARCHAR2(10000);
             LFieldExist INTEGER;
             CURSOR LCViewFields(ViewName VARCHAR2) IS
                 SELECT Column_Name
                   FROM User_Tab_Columns c
                  WHERE Table_Name = ViewName
               ORDER BY Column_ID;
         BEGIN

             LViewFields:='';
             FOR  RViewFields IN LCViewFields(AViewName) LOOP
                         LViewFields:=LViewFields||RViewFields.Column_Name||',';
             END LOOP;
             LViewFields:=SUBSTR(LViewFields,1,LENGTH(LViewFields)-1);
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'LViewFields='||LViewFields); $end null;

             SELECT Text
               INTO LViewSelect
               FROM USER_VIEWS
              WHERE VIEW_NAME=AViewName;

             SELECT Count(1)
               INTO LFieldExist
               FROM USER_TAB_COLUMNS
               where UPPER(Table_name)=UPPER(AViewName) AND Column_Name=UPPER(LTableField);

             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'LViewSelect=1'||LViewSelect); $end null;
             IF LFieldExist=0 THEN
                 LViewSelect:=SUBSTR(LViewSelect,1,INSTR(LViewSelect,'FROM ', -1)-2)||','||LTableField||' '||SUBSTR(LViewSelect,INSTR(LViewSelect,'FROM ', -1),LENGTH(LViewSelect));
                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'LViewSelect=2'||LViewSelect); $end null;
                 LStatement:='CREATE OR REPLACE VIEW '||AViewName||'('||LViewFields||','||LViewField||') AS '||LViewSelect;
                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'LStatement='||LStatement); $end null;
                 RETURN LStatement;
              ELSE
                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'Field Exist'); $end null;
                 LStatement:='ALTER VIEW '||AViewName||' COMPILE';
                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'LStatement='||LStatement); $end null;
                 RETURN LStatement;
              END IF;
         END;

         PROCEDURE ExecuteImmediate IS
         BEGIN
            EXECUTE IMMEDIATE LStatement;
            --InsertLog('ExecuteImmediate', LStatement);
         EXCEPTION
             WHEN OTHERS THEN
                 BEGIN
                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField'||' Line:'||$$plsql_line,'ExecuteImmediate - '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
                    AddErrorLog('adding',AField,LObject,AFieldType,ASection);
                 END;
         END;

        /* Identify whether we can add a new field LTableField to ATable1, or edit an existing one.
     * That is, whether LTableField (defined in PROCEDURE AddField) exists in ATable1
     * Return true if LTableField does not exists, otherwise, return false.
     * */
        FUNCTION VerifyEnableAdd(ATable1 IN VARCHAR2) RETURN BOOLEAN IS
            LExist NUMBER;
        BEGIN
            SELECT Count(1)
                INTO LExist
                FROM USER_TAB_COLUMNS
                WHERE TABLE_NAME=ATable1 AND COLUMN_NAME=LTableField;
            IF LExist=0 THEN
                RETURN True;
            ELSE
                RETURN False;
            END IF;
        END;

        PROCEDURE ModifyColumnSetDefault(
          p_table_name varchar2, p_column_name varchar2, p_default_value CLOB
        )
        IS
          v_statement varchar2(4000);
        BEGIN
          if (p_default_value is not null) then
            v_statement := 'ALTER TABLE [TABLE] MODIFY ([COLUMN] DEFAULT ''[DEFAULT]'')';
            v_statement := replace(v_statement, '[TABLE]', p_table_name);
            v_statement := replace(v_statement, '[COLUMN]', p_column_name);
            v_statement := replace(v_statement, '[DEFAULT]', p_default_value);

            EXECUTE IMMEDIATE v_statement;
          end if;
        END;

    /* Make sure filed LTableField (defined in PROCEDURE AddField) in ATable1 and ATable2 does not
     * contain any data. Otherwise, raise an exception
     * */
        PROCEDURE VerifyEnableModify(ATable1 IN VARCHAR2,ATable2 IN VARCHAR2) IS
            LData_Length    USER_TAB_COLUMNS.DATA_LENGTH%Type;
            LFieldType      VARCHAR2(30);
            LFieldLong      NUMBER;
            LPos            NUMBER;
            LResult         BOOLEAN;
            CURSOR CFieldsExitent IS
                SELECT TABLE_NAME,DATA_TYPE, DATA_LENGTH, DATA_PRECISION
                    FROM USER_TAB_COLUMNS
                    WHERE (TABLE_NAME=ATable1 OR TABLE_NAME=ATable2) AND COLUMN_NAME=LTableField;
        BEGIN
            FOR RFieldsExitent IN CFieldsExitent LOOP
                BEGIN
                    LObject:=RFieldsExitent.Table_Name;

                    LPos:=INSTR(UPPER(AFieldType),'(');
                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LPos: '||LPos||' AFieldType:'||AFieldType); $end null;
                    IF LPos=0 THEN
                        LFieldType:=AFieldType;
                    ELSE
                        LFieldType:=SUBSTR(AFieldType,1,LPos-1);
                    END IF;

                    VerifyEmptyField(LTableField,RFieldsExitent.Table_Name);

                    IF (LFieldType='CLOB' AND RFieldsExitent.Data_Type='CLOB') THEN
                         NULL; --Continue
                    ELSIF (LFieldType='CLOB' OR RFieldsExitent.Data_Type='CLOB') THEN
                        RAISE_APPLICATION_ERROR(eFieldNotEmptyInvalidType,'The field can''t be added neither modified. The field exists, not is empty and the type is invalid. (The type is CLOB'||')');
                    ELSE
                         NULL; --Continue
                    END IF;

                EXCEPTION
                    WHEN OTHERS THEN
                    IF INSTR(DBMS_UTILITY.FORMAT_ERROR_STACK,eFieldNotEmpty)<>0 THEN
                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LFieldType: '||LFieldType); $end null;
                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','RFieldsExitent.Data_Type: '||RFieldsExitent.Data_Type); $end null;
                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LPos: '||LPos); $end null;
                        IF RFieldsExitent.Data_Type = LFieldType THEN
                            IF LPos>0 THEN
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LPos: '||LPos); $end null;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','AFieldType: '||AFieldType); $end null;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LFieldLong: '||SUBSTR(AFieldType,LPos+1,INSTR(AFieldType,')')-LPos-1)); $end null;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LFieldLong2: '||SUBSTR(AFieldType,LPos+1,INSTR(AFieldType,',')-LPos-1)); $end null;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LFieldLong3: '||SUBSTR(AFieldType,INSTR(AFieldType,',')+1,INSTR(AFieldType,')')-INSTR(AFieldType,',')-1)); $end null;
                                IF INSTR(AFieldType,',')>0 THEN
                                    LFieldLong:=SUBSTR(AFieldType,LPos+1,INSTR(AFieldType,',')-LPos-1)-SUBSTR(AFieldType,INSTR(AFieldType,',')+1,INSTR(AFieldType,')')-INSTR(AFieldType,',')-1);--SUBSTR(AFieldType,INSTR(AFieldType,')')-INSTR(AFieldType,',')
                                ELSE
                                    LFieldLong:=SUBSTR(AFieldType,LPos+1,INSTR(AFieldType,')')-LPos-1);
                                END IF;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LFieldLong: '||LFieldLong); $end null;
                            ELSE
                                IF LFieldType='NUMBER' THEN
                                    LFieldLong:=22;
                                ELSE
                                    LFieldLong:=0;
                                END IF;
                            END IF;
                            IF RFieldsExitent.Data_Type = 'VARCHAR2' THEN
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','is VARCHAR2'); $end null;
                                LStatement:='SELECT NVL(MAX(Length('||LTableField||')),0)
                                                FROM '||RFieldsExitent.Table_Name;
                                EXECUTE IMMEDIATE LStatement INTO LData_Length;

                                IF LData_Length>LFieldLong THEN
                                     RAISE_APPLICATION_ERROR(eFieldNotEmptyLengthBig,'The field can''t be added neither modified. The field exists, it isn''t empty and the data length is more big. (The data length more big is '||LData_Length||')');
                                ELSE
                                     NULL; --Continue
                                END IF;
                            ELSIF RFieldsExitent.Data_Type IN ('CHAR','NCHAR') THEN
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','is CHAR o NCHAR'); $end null;
                                IF RFieldsExitent.Data_Length>LFieldLong THEN
                                    RAISE_APPLICATION_ERROR(eFieldNotEmptyLengthBig,'The field can''t be added neither modified. The field exists, it isn''t empty and the length is more big. (The length is '||RFieldsExitent.Data_Length||')');
                                ELSE
                                     NULL; --Continue
                                END IF;
                            ELSIF RFieldsExitent.Data_Type = 'NUMBER' THEN
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','is number'); $end null;
                                IF RFieldsExitent.Data_Precision>LFieldLong THEN
                                    RAISE_APPLICATION_ERROR(eFieldNotEmptyLengthBig,'The field can''t be added neither modified. The field exists, it isn''t empty and the precision is more big. (The precision is '||RFieldsExitent.Data_Precision||')');
                                ELSE
                                     NULL; --Continue
                                END IF;
                            ELSIF RFieldsExitent.Data_Type IN ('DATE','CLOB') THEN
                                 NULL; --Continue
                            ELSE
                                RAISE_APPLICATION_ERROR(eGenericException,'Field Type isn''t recognized');
                            END IF;
                        ELSIF RFieldsExitent.Data_Type='CLOB' AND LFieldType='CLOB' THEN
                             NULL; --Continue
                        ELSE
                            RAISE_APPLICATION_ERROR(eFieldNotEmptyNotCompatible,'The field can''t be added or modified. The field exists, it isn''t empty and the type isn''t the same. (The type is '||RFieldsExitent.Data_Type||')');
                        END IF;
                    ELSE
                        RAISE_APPLICATION_ERROR(eGenericException, DBMS_UTILITY.FORMAT_ERROR_STACK);
                    END IF;
                END;
            END LOOP;
        END;
     BEGIN
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField'||' Line:'||$$plsql_line,'AField='||AField||' ASection='||ASection||' AFieldType='||AFieldType); $end null;

         IF (INSTR(UPPER(AFieldType),'NUMBER')<>0) OR (INSTR(UPPER(AFieldType),'VARCHAR2')<>0) OR (INSTR(UPPER(AFieldType),'DATE')<>0) OR (INSTR(UPPER(AFieldType),'CLOB')<>0) OR (INSTR(UPPER(AFieldType),'NCHAR')<>0) THEN
             CASE UPPER(ASection)
              WHEN 'MIXTURE' THEN
                     BEGIN
                        GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                        VerifyEnableModify('MIXTURES','TEMPORARY_BATCH');

                        LObject:='MIXTURES';
                         IF VerifyEnableAdd(LObject) THEN
                             LStatement := 
                               'ALTER TABLE ' || LObject || ' ADD ' || UPPER(LTableField) || ' ' || AFieldType;
                             ExecuteImmediate;
                        ELSE
                             LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField','LStatement: '||LStatement); $end null;
                             ExecuteImmediate;
                        END IF;
                        ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                        LObject:='VW_MIXTURE';
                        LStatement:=AddFieldToView(LObject);
                        ExecuteImmediate;

                        EXECUTE IMMEDIATE VW_MIXTURE_REGNUMBER;

                        LObject:='TEMPORARY_BATCH';
                        IF VerifyEnableAdd(LObject) THEN
                             LStatement := 
                               'ALTER TABLE ' || LObject || ' ADD ' || UPPER(LTableField) || ' ' || AFieldType;
                             ExecuteImmediate;
                        ELSE
                             LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                             ExecuteImmediate;
                        END IF;
                        ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                        LObject:='VW_TEMPORARYBATCH';
                        LStatement:=AddFieldToView(LObject,'TEMPORARY_BATCH');
                        ExecuteImmediate;
                        
                        Auditing.UpdateTrigger('TRG_MIXTURES_AI');
                        Auditing.UpdateTrigger('TRG_MIXTURES_AU');
                        Auditing.UpdateTrigger('TRG_MIXTURES_AD');
                    END;
                WHEN 'COMPOUND' THEN
                     BEGIN
                        GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                        VerifyEnableModify('COMPOUND_MOLECULE','TEMPORARY_COMPOUND');

                        LObject:='COMPOUND_MOLECULE';
                        IF VerifyEnableAdd(LObject) THEN
                             LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                             ExecuteImmediate;
                        ELSE
                             LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                             ExecuteImmediate;
                        END IF;
                        ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                        LObject:='VW_COMPOUND';
                        LStatement:=AddFieldToView(LObject);
                        ExecuteImmediate;

                        LObject:='VW_MIXTURE_STRUCTURE';
                        LStatement:=AddFieldToView(LObject);
                        ExecuteImmediate;

                        --CompileView('VW_MIXTURE_STRUCTURE');

                        LObject:='TEMPORARY_COMPOUND';
                        IF VerifyEnableAdd(LObject) THEN
                             LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                             ExecuteImmediate;
                        ELSE
                             LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                             ExecuteImmediate;
                        END IF;
                        ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                        LObject:='VW_TEMPORARYCOMPOUND';
                        LStatement:=AddFieldToView(LObject);
                        ExecuteImmediate;

                        Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AI');
                        Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AU');
                        Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AD');
                    END;
                WHEN 'BATCH' THEN
                    BEGIN
                       GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                       VerifyEnableModify('BATCHES','TEMPORARY_BATCH');

                       LObject:='BATCHES';
                        IF VerifyEnableAdd(LObject) THEN
                            LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       ELSE
                            LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField','LStatement: '||LStatement); $end null;
                            ExecuteImmediate;
                       END IF;
                       ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                       LObject:='VW_BATCH';
                       LStatement:=AddFieldToView(LObject);
                       ExecuteImmediate;

                       EXECUTE IMMEDIATE VW_MIXTURE_BATCH;

                       LObject:='TEMPORARY_BATCH';
                       IF VerifyEnableAdd(LObject) THEN
                            LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       ELSE
                            LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       END IF;
                       ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                       LObject:='VW_TEMPORARYBATCH';
                       LStatement:=AddFieldToView(LObject,'TEMPORARY_BATCH');
                       ExecuteImmediate;

                        Auditing.UpdateTrigger('TRG_BATCHES_AI');
                        Auditing.UpdateTrigger('TRG_BATCHES_AU');
                        Auditing.UpdateTrigger('TRG_BATCHES_AD');
                    END;
                WHEN 'BATCHCOMPONENT' THEN
                    BEGIN
                       GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                       VerifyEnableModify('BATCHCOMPONENT','TEMPORARY_COMPOUND');

                       LObject:='BATCHCOMPONENT';
                        IF VerifyEnableAdd(LObject) THEN
                            LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       ELSE
                            LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField','LStatement: '||LStatement); $end null;
                            ExecuteImmediate;
                       END IF;
                       ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                       LObject:='VW_BATCHCOMPONENT';
                       LStatement:=AddFieldToView(LObject);
                       ExecuteImmediate;

                       EXECUTE IMMEDIATE VW_MIXTURE_BATCHCOMPONENT;

                       LObject:='TEMPORARY_COMPOUND';
                       IF VerifyEnableAdd(LObject) THEN
                            LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       ELSE
                            LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       END IF;
                       ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                       LObject:='VW_TEMPORARYCOMPOUND';
                       LStatement:=AddFieldToView(LObject,'TEMPORARY_COMPOUND');
                       ExecuteImmediate;
                    END;
                WHEN 'STRUCTURE' THEN
                    BEGIN
                       GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                       /* Make sure the field (LTableField) does not contain any data in both STRUCTURES and TEMPORARY_COMPOUND table (if LTableField already exists)
                       * This method call will raise an exceptioin if there are some data in LTableField
                       * */
                       VerifyEnableModify('STRUCTURES','TEMPORARY_COMPOUND');

                       /* Once no data exists, we can add or modify the field in both table and view
                       * */

                       LObject:='STRUCTURES';
                        IF VerifyEnableAdd(LObject) THEN
                            LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       ELSE
                            LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField','LStatement: '||LStatement); $end null;
                            ExecuteImmediate;
                       END IF;
                       ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                       LObject:='VW_STRUCTURE';
                       LStatement:=AddFieldToView(LObject);
                       ExecuteImmediate;

                       LObject:='VW_MIXTURE_STRUCTURE';
                       LStatement:=AddFieldToView(LObject);
                       ExecuteImmediate;

                       LObject:='TEMPORARY_COMPOUND';
                       IF VerifyEnableAdd(LObject) THEN
                            LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       ELSE
                            LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                            ExecuteImmediate;
                       END IF;
                       ModifyColumnSetDefault(LObject, UPPER(LTableField), LNewColumnDefaultValue);

                       LObject:='VW_TEMPORARYCOMPOUND';
                       LStatement:=AddFieldToView(LObject,'TEMPORARY_COMPOUND');
                       ExecuteImmediate;

                       LObject:='VW_STRUCTURE_DRAWING';
                       LStatement:=AddFieldToView(LObject);
                       ExecuteImmediate;

                        Auditing.UpdateTrigger('TRG_STRUCTURES_AI');
                        Auditing.UpdateTrigger('TRG_STRUCTURES_AU');
                        Auditing.UpdateTrigger('TRG_STRUCTURES_AD');
                    END;
                END CASE;
             ELSE
                 RAISE_APPLICATION_ERROR(eGenericException, 'Field Type isn''t recognized');
             END IF;
     EXCEPTION
         WHEN OTHERS THEN
         BEGIN
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField'||' Line:'||$$plsql_line,'AddField - '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            AddErrorLog('adding',AField,LObject,AFieldType,ASection);
         END;
     END;

     PROCEDURE DropField(AField IN VARCHAR2, ASection IN VARCHAR2) IS
         LStatement     Varchar2(4000);
         LViewField     Varchar2(100);
         LTableField    Varchar2(100);
         LObject        Varchar2(100);

         /* Construct a sql statement, which can be used to recreat view AViewName
      * This statement is different from original definitioin of AViewName by eliminating LTableField (if such one exists).
      * The name of this function is confusing because it try to eliminate a field from a view, rather than add a new field.
      * */
         FUNCTION AddFieldsToView(AViewName VARCHAR2) RETURN VARCHAR2 IS
             LViewSelect VARCHAR2(10000);
             LViewFields VARCHAR2(10000);
             LTable      VARCHAR2(1000); -- This variable actually stores the View's create statement starting
             -- from 'From', so it can easily exceed 100 characters, which is the previous length.
             CURSOR LCViewFields(ViewName VARCHAR2) IS
                 SELECT Column_Name
                   FROM User_Tab_Columns c
                  WHERE Table_Name = ViewName
               ORDER BY Column_ID;
         BEGIN
             LViewFields:='';
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'Begin AViewName'||AViewName); $end null;
             FOR  RViewFields IN LCViewFields(AViewName) LOOP
                 IF UPPER(RViewFields.Column_Name)<>LViewField THEN
                     LViewFields:=LViewFields||RViewFields.Column_Name||',';
                 END IF;
             END LOOP;
             LViewFields:=SUBSTR(LViewFields,1,LENGTH(LViewFields)-1);
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LViewFields='||LViewFields); $end null;
             SELECT Text
               INTO LViewSelect
               FROM USER_VIEWS
              WHERE VIEW_NAME=AViewName;

             LViewSelect:=REPLACE(' '||SUBSTR(UPPER(LViewSelect),8,LENGTH(LViewSelect)),CHR(10),''); --remove all character return after 'SELECT '
             LTable:=SUBSTR(LViewSelect,INSTR(LViewSelect,'FROM ',-1),LENGTH(LViewSelect)); --get substring start with 'FROM '
             LViewSelect:=REPLACE(LViewSelect,LTable,' '); -- get substring after 'SELECT ' but before 'FROM', that is, all fields

             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LTableField 1='||LTableField); $end null;

             --LTableField can appears in view definition in several styles, the following try to eliminate that field.
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LViewSelect 1='||LViewSelect); $end null;
             LViewSelect:=REPLACE(LViewSelect,','||LTableField||',',',');
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LViewSelect 2='||LViewSelect); $end null;
             LViewSelect:=REPLACE(LViewSelect,','||LTableField||' ',' ');
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LViewSelect 3='||LViewSelect); $end null;
             LViewSelect:=REPLACE(LViewSelect,' '||LTableField||',',' ');
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LViewSelect 4='||LViewSelect); $end null;
             LViewSelect:=REPLACE(LViewSelect,', '||LTableField,' ');
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LViewSelect 4='||LViewSelect); $end null;


             LViewSelect:='SELECT'||LViewSelect||LTable;
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LViewSelect 5='||LViewSelect); $end null;

             LStatement:='CREATE OR REPLACE VIEW '||AViewName||'('||LViewFields||') AS '||LViewSelect;
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LStatement='||LStatement); $end null;
             RETURN LStatement;
         END;
     BEGIN
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'AField='||AField||' ASection='||ASection); $end null;
         CASE UPPER(ASection)
             WHEN 'MIXTURE' THEN
                 BEGIN

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='MIXTURES';
                     VerifyEmptyField(LTableField,LObject);
                     LObject:='TEMPORARY_BATCH';
                     VerifyEmptyField(LTableField,LObject);

                     LObject:='MIXTURES';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'1 LStatement='||LStatement); $end null;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_MIXTURE';
                     LStatement:=AddFieldsToView(LObject);
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'2 LStatement='||LStatement); $end null;
                     EXECUTE IMMEDIATE LStatement;

                     EXECUTE IMMEDIATE VW_MIXTURE_REGNUMBER;

                     LObject:='TEMPORARY_BATCH';
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'VerifyEmptyField LTableField='||LTableField||' LObject='||LObject); $end null;
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'3 LStatement='||LStatement); $end null;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYBATCH';
                     LStatement:=AddFieldsToView(LObject);
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'4 LStatement='||LStatement); $end null;
                     EXECUTE IMMEDIATE LStatement;

                        Auditing.UpdateTrigger('TRG_MIXTURES_AI');
                        Auditing.UpdateTrigger('TRG_MIXTURES_AU');
                        Auditing.UpdateTrigger('TRG_MIXTURES_AD');
                 END;
             WHEN 'COMPOUND' THEN
                 BEGIN

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='COMPOUND_MOLECULE';
                     VerifyEmptyField(LTableField,LObject);
                     LObject:='TEMPORARY_COMPOUND';
                     VerifyEmptyField(LTableField,LObject);

                     LObject:='COMPOUND_MOLECULE';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_COMPOUND';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_MIXTURE_STRUCTURE';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;
                     --CompileView('VW_MIXTURE_STRUCTURE');

                     LObject:='TEMPORARY_COMPOUND';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYCOMPOUND';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                        Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AI');
                        Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AU');
                        Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AD');
                 END;
             WHEN 'BATCH' THEN
                 BEGIN

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='BATCHES';
                     VerifyEmptyField(LTableField,LObject);
                     LObject:='TEMPORARY_BATCH';
                     VerifyEmptyField(LTableField,LObject);

                     LObject:='BATCHES';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_BATCH';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     EXECUTE IMMEDIATE VW_MIXTURE_BATCH;

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'TEMPORARY');

                     LObject:='TEMPORARY_BATCH';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYBATCH';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                        Auditing.UpdateTrigger('TRG_BATCHES_AI');
                        Auditing.UpdateTrigger('TRG_BATCHES_AU');
                        Auditing.UpdateTrigger('TRG_BATCHES_AD');
                 END;
             WHEN 'BATCHCOMPONENT' THEN
                 BEGIN

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='BATCHCOMPONENT';
                     VerifyEmptyField(LTableField,LObject);
                     LObject:='TEMPORARY_COMPOUND';
                     VerifyEmptyField(LTableField,LObject);

                     LObject:='BATCHCOMPONENT';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_BATCHCOMPONENT';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     EXECUTE IMMEDIATE VW_MIXTURE_BATCHCOMPONENT;

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'TEMPORARY');

                     LObject:='TEMPORARY_COMPOUND';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYCOMPOUND';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                 END;
             WHEN 'STRUCTURE' THEN
                 BEGIN

                     -- Update permanent table and its view
                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='STRUCTURES';
                     VerifyEmptyField(LTableField,LObject);
                     LObject:='TEMPORARY_COMPOUND';
                     VerifyEmptyField(LTableField,LObject);

                     LObject:='STRUCTURES';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_STRUCTURE';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_MIXTURE_STRUCTURE';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     -- Update temporary table and its view
                     GetDBFieldName(AField,LViewField,LTableField,ASection,'TEMPORARY');

                     LObject:='TEMPORARY_COMPOUND';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYCOMPOUND';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_STRUCTURE_DRAWING';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                        Auditing.UpdateTrigger('TRG_STRUCTURES_AI');
                        Auditing.UpdateTrigger('TRG_STRUCTURES_AU');
                        Auditing.UpdateTrigger('TRG_STRUCTURES_AD');
                 END;
         END CASE;
     EXCEPTION
         WHEN OTHERS THEN
         BEGIN
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'DropField - '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
              AddErrorLog('dropping',AField,LObject,'',ASection);
         END;
     END;

     PROCEDURE SetCoeObjectConfig(ACoeObjectConfigXML IN  CLOB,AErrorsLog OUT CLOB) IS
         LCoeObjectConfigField     XmlType;
         LFiled               Varchar2(100);
         LFiledType           Varchar2(200);
         LFieldType           Varchar2(100);

         LDOMDocument         DBMS_XMLDom.DOMDocument;
         LDocumentNode        DBMS_XMLDom.DOMNode;
         LNodeList            DBMS_XMLDom.DOMNodelist;
         LNode                DBMS_XMLDom.DOMNode;
         LNodesCount          Number;
         LAttrs               DBMS_XMLDom.DOMNamedNodeMap;
         LAttr                DBMS_XMLDom.DOMNode;
         LIndexAttr           Number;
         LAttrName            Varchar2(300);

         ValidationRuleNode  DBMS_XMLDom.DOMNode;
         LValidationRuleList DBMS_XMLDom.DOMNodelist;
         LAttributeName      Varchar2(300);
         LLong               Varchar2(50);

         LCountInvalid       Integer;

         PROCEDURE DBProcessDelete(ANodeList DBMS_XMLDom.DOMNodelist,ASection in VARCHAR2) IS
         BEGIN
             LNodesCount:=DBMS_XMLDom.GetLength(ANodeList);
             FOR LIndex IN 0..LNodesCount-1 LOOP
                 LNode := DBMS_XMLDom.Item(ANodeList, LIndex);
                 LAttrs := dbms_xmldom.Getattributes(LNode);
                 LAttr := dbms_xmldom.GetNamedItem(LAttrs,'name');
                 LFiled :=dbms_xmldom.GetNodeValue(LAttr);

                 DropField(LFiled,ASection);
             END LOOP;
         END;

         PROCEDURE DBProcessInsert(ANodeList DBMS_XMLDom.DOMNodelist,ASection in VARCHAR2) IS
           LNewColumnDefaultValue CLOB := NULL;
         BEGIN
             LNodesCount:=DBMS_XMLDom.GetLength(ANodeList);
             FOR LIndex IN 0..LNodesCount-1 LOOP
                 LNewColumnDefaultValue := NULL;
                 LNode := DBMS_XMLDom.Item(ANodeList, LIndex);
                 LAttrs := dbms_xmldom.Getattributes(LNode);
                 LAttr := dbms_xmldom.GetNamedItem(LAttrs,'name');
                 LFiled := dbms_xmldom.GetNodeValue(LAttr);

                 LAttrs := dbms_xmldom.Getattributes(LNode);
                 LAttr := dbms_xmldom.GetNamedItem(LAttrs,'type');
                 LAttributeName :=UPPER(dbms_xmldom.GetNodeValue(LAttr));

                  IF LAttributeName IN ('TEXT','NUMBER') THEN
                    LAttr := dbms_xmldom.GetNamedItem(LAttrs,'precision');
                    LLong := dbms_xmldom.GetNodeValue(LAttr);

                      IF LAttributeName='TEXT' THEN
                      IF LLong<=4000 THEN
                        LFiledType:='VARCHAR2('||LLong||')';
                      ELSE
                        LFiledType:='CLOB';
                      END IF;
                    ELSIF LAttributeName='NUMBER' THEN
                      IF NVL(LLong,0)<>0 THEN
                        LFiledType:='NUMBER('||Replace(LLong,'.',',')||')';
                      ELSE
                        LFiledType:='NUMBER';
                      END IF;
                    END IF;

                  ELSIF LAttributeName='DATE' THEN
                    LFiledType:='DATE';

                  ELSIF LAttributeName='BOOLEAN' THEN
                    LFiledType:='NCHAR(1)';
                    LNewColumnDefaultValue:='F';

                  ELSIF LAttributeName='PICKLISTDOMAIN' THEN
                    LFiledType:='VARCHAR2(10)';

                  ELSE
                    LFiledType:=LAttributeName;
                  END IF;

                  AddField(LFiled,ASection,LFiledType, LNewColumnDefaultValue);
             END LOOP;
         END;

        PROCEDURE DBProcessUpdate(ANodeList DBMS_XMLDom.DOMNodelist,ASection in VARCHAR2) IS
        BEGIN
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessUpdate'||' Line:'||$$plsql_line,'DBMS_XMLDom.GetLength(ANodeList)->'||DBMS_XMLDom.GetLength(ANodeList)); $end null;
            DBProcessDelete(ANodeList,ASection);
            DBProcessInsert(ANodeList,ASection);
        END;

        PROCEDURE DBProcessUpdateAttribute(ANodeList DBMS_XMLDom.DOMNodelist,ASection in VARCHAR2) IS
            PROCEDURE UpdateField(AField IN VARCHAR2, ASection IN VARCHAR2, AFieldType IN VARCHAR2) IS
                LStatement   Varchar2(4000);
                LViewField   Varchar2(100);
                LTableField  Varchar2(100);
                LObject      Varchar2(100);

                PROCEDURE ExecuteImmediate IS
                BEGIN
                   EXECUTE IMMEDIATE LStatement;
                EXCEPTION
                    WHEN OTHERS THEN
                        BEGIN
                           $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField'||' Line:'||$$plsql_line,'ExecuteImmediate - '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
                           AddErrorLog('adding',AField,LObject,AFieldType,ASection);
                        END;
                END;

                PROCEDURE RefreshView(AViewName VARCHAR2) IS
                BEGIN
                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('RefreshView'||' Line:'||$$plsql_line,'AViewName='||AViewName); $end null;
                    LStatement:='ALTER VIEW '||AViewName||' COMPILE';
                    ExecuteImmediate;
                END;

        /* Make sure field LTableField (defined in PROCEDURE UpdateField) in ATable1 and ATable2 does not
        * contain any data. Otherwise, raise an exception
        * */
                PROCEDURE VerifyEnableModify(ATable1 IN VARCHAR2,ATable2 IN VARCHAR2) IS
                    LData_Length    USER_TAB_COLUMNS.DATA_LENGTH%Type;
                    LFieldType      VARCHAR2(30);
                    LFieldLong      NUMBER;
                    LPos            NUMBER;
                    LResult         BOOLEAN;
                    CURSOR CFieldsExitent IS
                    SELECT TABLE_NAME,DATA_TYPE, DATA_LENGTH, DATA_PRECISION
                        FROM USER_TAB_COLUMNS
                        WHERE (TABLE_NAME=ATable1 OR TABLE_NAME=ATable2) AND COLUMN_NAME=LTableField;
                BEGIN
                    FOR RFieldsExitent IN CFieldsExitent LOOP
                        BEGIN
                            LObject:=RFieldsExitent.Table_Name;

                            LPos:=INSTR(UPPER(AFieldType),'(');
                            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LPos: '||LPos||' AFieldType:'||AFieldType); $end null;
                            IF LPos=0 THEN
                                LFieldType:=AFieldType;
                            ELSE
                                LFieldType:=SUBSTR(AFieldType,1,LPos-1);
                            END IF;

                            VerifyEmptyField(LTableField,RFieldsExitent.Table_Name);

                            IF (LFieldType='CLOB' AND RFieldsExitent.Data_Type='CLOB') THEN
                                 NULL; --Continue
                            ELSIF (LFieldType='CLOB' OR RFieldsExitent.Data_Type='CLOB') THEN
                                RAISE_APPLICATION_ERROR(eFieldNotEmptyInvalidType,'The field can''t be  modified. The field exists, not is empty and the type is invalid. (The type is CLOB'||')');
                            ELSE
                                 NULL; --Continue
                            END IF;

                        EXCEPTION
                            WHEN OTHERS THEN
                            IF INSTR(DBMS_UTILITY.FORMAT_ERROR_STACK,eFieldNotEmpty)<>0 THEN
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LFieldType: '||LFieldType); $end null;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','RFieldsExitent.Data_Type: '||RFieldsExitent.Data_Type); $end null;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LPos: '||LPos); $end null;
                                IF RFieldsExitent.Data_Type = LFieldType THEN
                                    IF LPos>0 THEN
                                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LPos: '||LPos); $end null;

                                        LFieldLong:=SUBSTR(AFieldType,LPos+1,INSTR(AFieldType,')')-LPos-1);
                                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','LFieldLong: '||LFieldLong); $end null;
                                    ELSE
                                        IF LFieldType='NUMBER' THEN
                                            LFieldLong:=22;
                                        ELSE
                                            LFieldLong:=0;
                                        END IF;
                                    END IF;
                                    IF RFieldsExitent.Data_Type = 'VARCHAR2' THEN
                                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','is VARCHAR2'); $end null;
                                        LStatement:='SELECT NVL(MAX(Length('||LTableField||')),0)
                                                        FROM '||RFieldsExitent.Table_Name;
                                        EXECUTE IMMEDIATE LStatement INTO LData_Length;

                                        IF LData_Length>LFieldLong THEN
                                             RAISE_APPLICATION_ERROR(eFieldNotEmptyLengthBig,'The field can''t be modified. The field exists, it isn''t empty and the data length is more big. (The data length more big is '||LData_Length||')');
                                        ELSE
                                             NULL; --Continue
                                        END IF;
                                    ELSIF RFieldsExitent.Data_Type IN ('CHAR','NCHAR') THEN
                                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','is CHAR o NCHAR'); $end null;
                                        IF RFieldsExitent.Data_Length>LFieldLong THEN
                                            RAISE_APPLICATION_ERROR(eFieldNotEmptyLengthBig,'The field can''t be modified. The field exists, it isn''t empty and the length is more big. (The length is '||RFieldsExitent.Data_Length||')');
                                        ELSE
                                             NULL; --Continue
                                        END IF;
                                    ELSIF RFieldsExitent.Data_Type = 'NUMBER' THEN
                                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEnableModify','is number'); $end null;
                                        IF RFieldsExitent.Data_Precision>LFieldLong THEN
                                            RAISE_APPLICATION_ERROR(eFieldNotEmptyLengthBig,'The field can''t be modified. The field exists, it isn''t empty and the precision is more big. (The precision is '||RFieldsExitent.Data_Precision||')');
                                        ELSE
                                             NULL; --Continue
                                        END IF;
                                    ELSIF RFieldsExitent.Data_Type IN ('DATE','CLOB') THEN
                                         NULL; --Continue
                                    ELSE
                                        RAISE_APPLICATION_ERROR(eGenericException,'Field Type isn''t recognized');
                                    END IF;
                                ELSIF RFieldsExitent.Data_Type='CLOB' AND LFieldType='CLOB' THEN
                                     NULL; --Continue
                                ELSE
                                    RAISE_APPLICATION_ERROR(eFieldNotEmptyNotCompatible,'The field can''t be modified. The field exists, it isn''t empty and the type isn''t the same. (The type is '||RFieldsExitent.Data_Type||')');
                                END IF;
                            ELSE
                                RAISE_APPLICATION_ERROR(eGenericException, DBMS_UTILITY.FORMAT_ERROR_STACK);
                            END IF;
                        END;
                    END LOOP;
                END;

            BEGIN
                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('UpdateField'||' Line:'||$$plsql_line,'AField='||AField||' ASection='||ASection||' AFieldType='||AFieldType); $end null;

                IF (INSTR(UPPER(AFieldType),'NUMBER')<>0) OR (INSTR(UPPER(AFieldType),'VARCHAR2')<>0) OR (INSTR(UPPER(AFieldType),'DATE')<>0) OR (INSTR(UPPER(AFieldType),'CLOB')<>0) OR (INSTR(UPPER(AFieldType),'NCHAR')<>0) THEN
                    CASE UPPER(ASection)
                        WHEN 'MIXTURE' THEN
                            BEGIN
                                GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                                VerifyEnableModify('MIXTURES','TEMPORARY_BATCH');

                                LObject:='MIXTURES';

                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField','LStatement: '||LStatement); $end null;
                                ExecuteImmediate;

                                LObject:='VW_MIXTURE';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;

                                EXECUTE IMMEDIATE VW_MIXTURE_REGNUMBER;

                                LObject:='TEMPORARY_BATCH';

                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                ExecuteImmediate;

                                LObject:='VW_TEMPORARYBATCH';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;
                           END;
                        WHEN 'COMPOUND' THEN
                            BEGIN
                                GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                                VerifyEnableModify('COMPOUND_MOLECULE','TEMPORARY_COMPOUND');

                                LObject:='COMPOUND_MOLECULE';
                                                                    LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField','LStatement: '||LStatement); $end null;
                                ExecuteImmediate;

                                LObject:='VW_COMPOUND';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;

                                CompileView('VW_MIXTURE_STRUCTURE');

                                LObject:='TEMPORARY_COMPOUND';

                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                ExecuteImmediate;

                                LObject:='VW_TEMPORARYCOMPOUND';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;
                            END;
                        WHEN 'STRUCTURE' THEN
                            BEGIN
                                GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                                VerifyEnableModify('STRUCTURES','TEMPORARY_COMPOUND');

                                LObject:='STRUCTURES';
                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('UpdateField','LStatement: '||LStatement); $end null;
                                ExecuteImmediate;

                                LObject:='VW_STRUCTURE';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;

                                LObject:='VW_STRUCTURE_DRAWING';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;

                                LObject:='VW_MIXTURE_STRUCTURE';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;

                                LObject:='TEMPORARY_COMPOUND';

                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                ExecuteImmediate;

                                LObject:='VW_TEMPORARYCOMPOUND';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;
                            END;
                       WHEN 'BATCH' THEN
                            BEGIN
                                GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                                VerifyEnableModify('BATCHES','TEMPORARY_BATCH');

                                LObject:='BATCHES';
                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField','LStatement: '||LStatement); $end null;
                                ExecuteImmediate;

                                LObject:='VW_BATCH';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;

                                EXECUTE IMMEDIATE VW_MIXTURE_BATCH;

                                LObject:='TEMPORARY_BATCH';
                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                ExecuteImmediate;

                                LObject:='VW_TEMPORARYBATCH';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;
                            END;
                       WHEN 'BATCHCOMPONENT' THEN
                            BEGIN
                                GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                                VerifyEnableModify('BATCHCOMPONENT','TEMPORARY_COMPOUND');

                                LObject:='BATCHCOMPONENT';
                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField','LStatement: '||LStatement); $end null;
                                ExecuteImmediate;

                                LObject:='VW_BATCHCOMPONENT';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;

                                EXECUTE IMMEDIATE VW_MIXTURE_BATCHCOMPONENT;

                                LObject:='TEMPORARY_COMPOUND';
                                LStatement:='ALTER TABLE '||LObject||' MODIFY '||UPPER(LTableField)||' '||AFieldType;
                                ExecuteImmediate;

                                LObject:='VW_TEMPORARYCOMPOUND';
                                LStatement:='ALTER VIEW '||LObject||' COMPILE';
                                ExecuteImmediate;
                            END;
                       END CASE;
                    ELSE
                        RAISE_APPLICATION_ERROR(eGenericException, 'Field Type isn''t recognized');
                    END IF;

            EXCEPTION
                WHEN OTHERS THEN
                BEGIN
                   $if ConfigurationCompoundRegistry.Debuging $then InsertLog('UpdateField'||' Line:'||$$plsql_line,'UpdateField - '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
                   AddErrorLog('updating',AField,LObject,AFieldType,ASection);
                END;
            END;

        BEGIN
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessUpdateAttribute'||' Line:'||$$plsql_line,'DBMS_XMLDom.GetLength(ANodeList)->'||DBMS_XMLDom.GetLength(ANodeList)); $end null;

             LNodesCount:=DBMS_XMLDom.GetLength(ANodeList);
             FOR LIndex IN 0..LNodesCount-1 LOOP
                 LNode := DBMS_XMLDom.Item(ANodeList, LIndex);
                 LAttrs := dbms_xmldom.Getattributes(LNode);
                 LAttr := dbms_xmldom.GetNamedItem(LAttrs,'name');
                 LFiled := dbms_xmldom.GetNodeValue(LAttr);

                 LAttrs := dbms_xmldom.Getattributes(LNode);
                 LAttr := dbms_xmldom.GetNamedItem(LAttrs,'type');
                 LAttributeName :=UPPER(dbms_xmldom.GetNodeValue(LAttr));

                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessUpdateAttribute'||' Line:'||$$plsql_line,'LFiled->'||LFiled||' LAttributeName->'||LAttributeName); $end null;

                 IF LAttributeName IN ('TEXT','NUMBER') THEN
                     LAttr := dbms_xmldom.GetNamedItem(LAttrs,'precision');
                     LLong := dbms_xmldom.GetNodeValue(LAttr);

                     IF LAttributeName='TEXT' THEN
                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessUpdateAttribute'||' Line:'||$$plsql_line,'Text LFiled->'||LFiled||' LLong->'||LLong||' LAttributeName->'||LAttributeName); $end null;
                         IF LLong<=4000 THEN
                             LFiledType:='VARCHAR2('||LLong||')';
                         ELSE
                             LFiledType:='CLOB';
                         END IF;
                     ELSIF LAttributeName='NUMBER' THEN
                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessUpdateAttribute'||' Line:'||$$plsql_line,'Number LFiled->'||LFiled||' LLong->'||LLong||' LAttributeName->'||LAttributeName); $end null;
                         IF NVL(LLong,0)<>0 THEN
                             LFiledType:='NUMBER('||Replace(LLong,'.',',')||')';
                         ELSE
                             LFiledType:='NUMBER';
                         END IF;
                     END IF;
                 ELSIF LAttributeName='DATE' THEN
                     LFiledType:='DATE';
                 ELSIF LAttributeName='BOOLEAN' THEN
                     LFiledType:='NCHAR(1)';
                 ELSIF LAttributeName='PICKLISTDOMAIN' THEN
                    LFiledType:='VARCHAR2(10)';
                 ELSE
                     LFiledType:=LAttributeName;
                 END IF;

                 UpdateField(LFiled,ASection,LFiledType);
             END LOOP;
         END;

         PROCEDURE DBProcessSection(ANodeList DBMS_XMLDom.DOMNodelist,ASection in VARCHAR2) IS
             LNodeList DBMS_XMLDom.DOMNodelist;
             LValidationRuleList DBMS_XMLDom.DOMNodelist;
         BEGIN
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessSection'||' Line:'||$$plsql_line,'DBMS_XMLDom.GetLength(ANodeList)->'||DBMS_XMLDom.GetLength(ANodeList)); $end null;
             LNodeList:=dbms_xslprocessor.selectNodes(DBMS_XMLDOM.ITEM(ANodeList,0),'Property[@delete="yes"]');
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessSection'||' Line:'||$$plsql_line,'Property[@delete="yes"] LNodeList->'||DBMS_XMLDom.GetLength(LNodeList)); $end null;
             DBProcessDelete(LNodeList,ASection);
             LNodeList:=dbms_xslprocessor.selectNodes(DBMS_XMLDOM.ITEM(ANodeList,0),'Property[@insert="yes"]');
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessSection'||' Line:'||$$plsql_line,'Property[@insert="yes"] LNodeList->'||DBMS_XMLDom.GetLength(LNodeList)); $end null;
             DBProcessInsert(LNodeList,ASection);
             LNodeList:=dbms_xslprocessor.selectNodes(DBMS_XMLDOM.ITEM(ANodeList,0),'Property[@update="yes"]');
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessSection'||' Line:'||$$plsql_line,'Property[@update="yes"] LNodeList->'||DBMS_XMLDom.GetLength(LNodeList)); $end null;
             DBProcessUpdate(LNodeList,ASection);
             LNodeList:=dbms_xslprocessor.selectNodes(DBMS_XMLDOM.ITEM(ANodeList,0),'Property[@update!="yes" and @update!=""]');
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessSection'||' Line:'||$$plsql_line,'Property[@update!="yes" and @update!=""] LNodeList->'||DBMS_XMLDom.GetLength(LNodeList)); $end null;
             DBProcessUpdateAttribute(LNodeList,ASection);
         END;

         PROCEDURE ProcessAndSaveXMLInDB(ADocumentNode DBMS_XMLDom.DOMNode) IS
             LCoeObjectConfigField XMLType;
             VExists number(1);
             LDOMDocument DBMS_XMLDOM.DOMDOCUMENT;
             LDocumentNode DBMS_XMLDOM.DOMNODE;

             PROCEDURE XMLNodeInsert(AxPathTarget VARCHAR2,AxPathSource VARCHAR2,ASection IN VARCHAR2) IS
                 LNodeListSource       DBMS_XMLDom.DOMNodelist;
                 LNodeListTarget       DBMS_XMLDom.DOMNodelist;
                 LNodeSourse           DBMS_XMLDom.DOMNode;
                 LNodeTarget           DBMS_XMLDom.DOMNode;
                 LNodeTargetAux        DBMS_XMLDom.DOMNode;
                 LIndex                Integer;
                 LIndexParent          Integer;
                 LIndexAttr            Integer;

                 LAttrs                DBMS_XMLDom.DOMNamedNodeMap;
                 LAttr                 DBMS_XMLDom.DOMNode;
                 LxPathTarget          VARCHAR2(4000);
                 LxPathTargetParent    VARCHAR2(4000);
                 LNodeListSourceParent DBMS_XMLDom.DOMNodelist;
                 LNodeSourseParent     DBMS_XMLDom.DOMNode;

                 LNodeListTargetParent DBMS_XMLDom.DOMNodelist;
                 LNodeTargetParent     DBMS_XMLDom.DOMNode;

                 LName                 VARCHAR2(200);
                 Lvalue                VARCHAR2(4000);
                 LNodeListAux          DBMS_XMLDom.DOMNodelist;
             BEGIN
                 CASE UPPER(ASection)
                     WHEN 'DEFAULT' THEN
                     BEGIN
                         LNodeListSource:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);
                         LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,AxPathTarget);

                         LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);

                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                         FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                             LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);

                             LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);

                             LAttrs := dbms_xmldom.Getattributes(LNodeSourse);

                             LAttr := dbms_xmldom.GetnamedItem(LAttrs,'name');
                             Lvalue := dbms_xmldom.GetNodevalue(LAttr);

                             $if ConfigurationCompoundRegistry.Debuging $then if Lvalue IS NULL then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'**ERROR** The tag will not be added, "values" is null: '||LxPathTarget); end if; $end null;
                             IF Lvalue IS NOT NULL THEN
                                 LNodeListAux:=dbms_xslprocessor.selectNodes(LDocumentNode,AxPathTarget||'/Property[@name="'||Lvalue||'"]');

                                 IF DBMS_XMLDom.GetLength(LNodeListAux)=0 THEN
                                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'Node will be added:'||AxPathTarget||'/Property[@name="'||Lvalue||'"]'); $end null;
                                    LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');
                                    --NOTE: DO not eliminate these attributes:
                                    --      They help the applciation code validate PropertyList Property values.
                                    -- LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'precision');
                                    -- LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'type');
                                    LNodeTargetAux:=dbms_xmldom.appendChild(LNodeTarget,LNodeSourse);
                                 END IF;
                             END IF;
                         END LOOP;
                     END;
                     WHEN 'EVENT' THEN
                     BEGIN

                          LNodeListSourceParent:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                         FOR LIndexParent IN 0..DBMS_XMLDom.GetLength(LNodeListSourceParent)-1 LOOP
                             LNodeSourseParent:=DBMS_XMLDom.Item(LNodeListSourceParent,LIndexParent);
                             LNodeListSource:=dbms_xslprocessor.selectNodes(LNodeSourseParent,'Event[@insert="yes"]');
                             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                             FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,' LCoeObjectConfigField.GetClobVal()'||LCoeObjectConfigField.GetClobVal()); $end null;
                                 LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);


                                 LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);
                                 LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');

                                 --LNodeSourseParent :=dbms_xmldom.GetParentNode(LNodeSourse);
                                 LAttrs := dbms_xmldom.Getattributes(LNodeSourseParent);
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'update');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');


                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;


                                 LxPathTarget:=AxPathTarget||'[';
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                 FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                      LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                      LName :=dbms_xmldom.GetNodeName(LAttr);
                                      LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                      LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                 END LOOP;
                                 IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                     LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                                 ELSE
                                     LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                                 END IF;
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'2-LxPathTarget='||LxPathTarget); $end null;

                                 LNodeListTargetParent:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                 LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTargetParent, 0);
                                 LNodeTargetParent:=dbms_xmldom.appendChild(LNodeTargetParent,LNodeSourse);
                             END LOOP;
                         END LOOP;
                     END;
                     WHEN 'ADDIN' THEN
                     BEGIN
                         LNodeListSource:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);
                         LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,AxPathTarget);

                         LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);

                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                         FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                             LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);

                             LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);

                             LAttrs := dbms_xmldom.Getattributes(LNodeSourse);

                             LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');

                             LNodeTargetAux:=dbms_xmldom.appendChild(LNodeTarget,LNodeSourse);
                         END LOOP;
                     END;
                     WHEN 'VALIDATION' THEN
                     BEGIN

                          LNodeListSourceParent:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                         FOR LIndexParent IN 0..DBMS_XMLDom.GetLength(LNodeListSourceParent)-1 LOOP
                             LNodeSourseParent:=DBMS_XMLDom.Item(LNodeListSourceParent,LIndexParent);
                             LNodeListSource:=dbms_xslprocessor.selectNodes(LNodeSourseParent,'validationRuleList/validationRule[@insert="yes"]');
                             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'VALIDATION DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                             FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'VALIDATION LCoeObjectConfigField.GetClobVal()'||LCoeObjectConfigField.GetClobVal()); $end null;
                                 LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);


                                 LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);
                                 LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');


                                 --LNodeSourseParent :=dbms_xmldom.GetParentNode(LNodeSourse);
                                 LAttrs := dbms_xmldom.Getattributes(LNodeSourseParent);
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'update');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'precision');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'type');

                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'VALIDATION DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'VALIDATION DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;


                                 LxPathTarget:=AxPathTarget||'[';
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'VALIDATION 1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                 FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                      LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                      LName :=dbms_xmldom.GetNodeName(LAttr);
                                      LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                      LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                 END LOOP;
                                 IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                     LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1)||'/validationRuleList';
                                 ELSE
                                     LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']/validationRuleList';
                                 END IF;
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert'||' Line:'||$$plsql_line,'VALIDATION 2-LxPathTarget='||LxPathTarget); $end null;

                                 LNodeListTargetParent:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                 LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTargetParent, 0);
                                 LNodeTargetParent:=dbms_xmldom.appendChild(LNodeTargetParent,LNodeSourse);
                             END LOOP;
                         END LOOP;
                     END;
                     WHEN 'PARAM' THEN
                     BEGIN

                          LNodeListSourceParent:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                         FOR LIndexParent IN 0..DBMS_XMLDom.GetLength(LNodeListSourceParent)-1 LOOP
                             LNodeSourseParent:=DBMS_XMLDom.Item(LNodeListSourceParent,LIndexParent);
                             LNodeListSource:=dbms_xslprocessor.selectNodes(LNodeSourseParent,'params/param[@insert="yes"]');
                             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                             FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                                $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION LCoeObjectConfigField.GetClobVal()'||LCoeObjectConfigField.GetClobVal()); $end null;
                                 LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);


                                 LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);
                                 LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');

                                 LAttrs := dbms_xmldom.Getattributes(LNodeSourseParent);
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'update');


                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;

                                 LxPathTarget:='/validationRuleList/validationRule[';
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION 1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                 FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                      LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                      LName :=dbms_xmldom.GetNodeName(LAttr);
                                      LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                      LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                 END LOOP;
                                 IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                     LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1)||'/params';
                                 ELSE
                                     LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']/params';
                                 END IF;
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION 2-LxPathTarget='||LxPathTarget); $end null;

                                 LNodeSourseParent :=dbms_xmldom.GetParentNode(LNodeSourseParent);
                                 LNodeSourseParent :=dbms_xmldom.GetParentNode(LNodeSourseParent);
                                 LAttrs := dbms_xmldom.Getattributes(LNodeSourseParent);
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'update');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'precision');
                                 LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'type');

                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;

                                 LxPathTargetParent:=AxPathTarget||'[';
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION 1-LxPathTargetParent='||LxPathTargetParent||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                 FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                      LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                      LName :=dbms_xmldom.GetNodeName(LAttr);
                                      LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                      LxPathTargetParent:=LxPathTargetParent||'@'||LName||'="'||LValue||'" and ';
                                 END LOOP;
                                 IF SUBSTR(LxPathTarget,LENGTH(LxPathTargetParent),1)='[' THEN
                                     LxPathTargetParent:=SUBSTR(LxPathTargetParent,1,LENGTH(LxPathTargetParent)-1);
                                 ELSE
                                     LxPathTargetParent:=SUBSTR(LxPathTargetParent,1,LENGTH(LxPathTargetParent)-5)||']';
                                 END IF;
                                 LxPathTarget:=LxPathTargetParent||LxPathTarget;
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeInsert PARAM'||' Line:'||$$plsql_line,'VALIDATION 2-LxPathTarget='||LxPathTarget); $end null;

                                 LNodeListTargetParent:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                 LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTargetParent, 0);
                                 LNodeTargetParent:=dbms_xmldom.appendChild(LNodeTargetParent,LNodeSourse);
                             END LOOP;
                         END LOOP;
                     END;
                 END CASE;
             END;

             PROCEDURE XMLNodeDelete(AxPathTarget VARCHAR2,AxPathSource VARCHAR2,ASection IN VARCHAR2) IS
                 LNodeListSource       DBMS_XMLDom.DOMNodelist;
                 LNodeListTarget       DBMS_XMLDom.DOMNodelist;
                 LNodeSourse           DBMS_XMLDom.DOMNode;
                 LNodeTarget           DBMS_XMLDom.DOMNode;
                 LIndex                Integer;
                 LIndexTarget          Integer;
                 LIndexAttr            Integer;

                 LNodeListSourceParent DBMS_XMLDom.DOMNodelist;
                 LNodeSourseParent     DBMS_XMLDom.DOMNode;
                 LNodeSourseGranParent DBMS_XMLDom.DOMNode;

                 LAttrs                DBMS_XMLDom.DOMNamedNodeMap;
                 LAttr                 DBMS_XMLDom.DOMNode;
                 LxPathTarget          VARCHAR2(4000);
                 LNodeListTargetParent DBMS_XMLDom.DOMNodelist;
                 LNodeTargetParent     DBMS_XMLDom.DOMNode;

                 LName                 VARCHAR2(200);
                 Lvalue                VARCHAR2(4000);

                 FUNCTION ValidateChangesInDB(AField VARCHAR2, ASection IN VARCHAR2) RETURN BOOLEAN IS
                    XmlTypeErrorsLog XmlType;
                    LExistError VARCHAR2(10);
                 BEGIN
                    XmlTypeErrorsLog:=XmlType.CreateXml(ErrorsLog);
                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ValidateChangesInDB'||' Line:'||$$plsql_line,'AField:'||AField||' ASection:'||ASection||' ErrorsLog:'||ErrorsLog); $end null;
                    BEGIN
                        SELECT 'TRUE'
                            INTO LExistError
                            FROM DUAL
                            WHERE ExistsNode(XmlTypeErrorsLog,'/ErrorList/Error[((Field="'||AField||'") and (Section="'||ASection||'"))]')=1;
                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ValidateChangesInDB'||' Line:'||$$plsql_line,'LExistError:'||LExistError); $end null;
                        RETURN FALSE;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ValidateChangesInDB'||' Line:'||$$plsql_line,'LExistError:FALSE'); $end null;
                            RETURN TRUE;
                    END;

                 END;
             BEGIN
                IF ASection='MIXTURE' OR ASection='COMPOUND' OR ASection='BATCH' OR ASection='BATCHCOMPONENT' OR ASection='STRUCTURE' OR ASection='ADDIN' THEN
                     LNodeListSource:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                     LNodeListTargetParent:=dbms_xslprocessor.selectNodes(LDocumentNode,AxPathTarget||'/..');
                     LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTargetParent, 0);

                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                     FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                         LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);

                         LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);

                         LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                         LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');
                         LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'precision');
                         LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'type');
                         LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'sortOrder');

                         LxPathTarget:=AxPathTarget||'[';
                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                         FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                              LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                              LName :=dbms_xmldom.GetNodeName(LAttr);
                              LValue :=dbms_xmldom.GetNodeValue(LAttr);

                              IF LName='name' THEN
                                IF NOT ValidateChangesInDB(LValue,ASection) THEN
                                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'ValidateChangesInDB=FaLse'); $end null;
                                    RETURN;
                                END IF;
                              END IF;

                              LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                         END LOOP;
                         IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                             LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                         ELSE
                             LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                         END IF;
                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'2-LxPathTarget='||LxPathTarget); $end null;
                         LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);

                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'DBMS_XMLDom.GetLength(LNodeListTarget)='||DBMS_XMLDom.GetLength(LNodeListTarget)); $end null;
                         $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'**ERROR1** The tag will not be deleted: '||LxPathTarget); end if; $end null;
                         IF DBMS_XMLDom.GetLength(LNodeListTarget)>0 THEN
                              $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'Node will be deleted:'||LxPathTarget); $end null;
                              LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                              LNodeTargetParent:=DBMS_XMLDom.REMOVECHILD(LNodeTargetParent,LNodeTarget);
                         END IF;

                     END LOOP;
                ELSE
                    CASE UPPER(ASection)
                         WHEN 'EVENT' THEN
                         BEGIN

                              LNodeListSourceParent:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                             FOR LIndexParent IN 0..DBMS_XMLDom.GetLength(LNodeListSourceParent)-1 LOOP
                                 LNodeSourseParent:=DBMS_XMLDom.Item(LNodeListSourceParent,LIndexParent);
                                 LNodeListSource:=dbms_xslprocessor.selectNodes(LNodeSourseParent,'Event[@delete="yes"]');
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                                 FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' LCoeObjectConfigField.GetClobVal()'||LCoeObjectConfigField.GetClobVal()); $end null;
                                     LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);


                                     LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);
                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');

                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourseParent);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'update');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');

                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;

                                     LxPathTarget:=AxPathTarget||'[';
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' Parent 1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                     FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                          LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                          LName :=dbms_xmldom.GetNodeName(LAttr);
                                          LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                          LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                     END LOOP;
                                     IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                                     ELSE
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                                     END IF;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' Parent 2-LxPathTarget='||LxPathTarget); $end null;

                                     LNodeListTargetParent:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                     LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTargetParent, 0);

                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'insert');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'update');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');

                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;

                                     LxPathTarget:=LxPathTarget||'/Event[';
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                     FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                          LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                          LName :=dbms_xmldom.GetNodeName(LAttr);
                                          LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                          LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                     END LOOP;
                                     IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                                     ELSE
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                                     END IF;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'2-LxPathTarget='||LxPathTarget); $end null;

                                     LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                     $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'**ERROR2** The tag will not be deleted: '||LxPathTarget); end if; $end null;
                                     IF DBMS_XMLDom.GetLength(LNodeListTarget)>0 THEN
                                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'Node will be deleted:'||LxPathTarget); $end null;
                                         LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                         LNodeTargetParent:=DBMS_XMLDom.RemoveChild(LNodeTargetParent,LNodeTarget);
                                      END IF;

                                 END LOOP;
                             END LOOP;
                         END;
                         WHEN 'VALIDATION' THEN
                         BEGIN

                             LNodeListSourceParent:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                             FOR LIndexParent IN 0..DBMS_XMLDom.GetLength(LNodeListSourceParent)-1 LOOP
                                 LNodeSourseParent:=DBMS_XMLDom.Item(LNodeListSourceParent,LIndexParent);
                                 LNodeListSource:=dbms_xslprocessor.selectNodes(LNodeSourseParent,'validationRuleList/validationRule[@delete="yes"]');
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                                 FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' LCoeObjectConfigField.GetClobVal()'||LCoeObjectConfigField.GetClobVal()); $end null;
                                     LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);


                                     LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);
                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');

                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourseParent);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'precision');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'type');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'sortOrder');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'update');


                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;

                                     LxPathTarget:=AxPathTarget||'[';
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' Parent 1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                     FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                          LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                          LName :=dbms_xmldom.GetNodeName(LAttr);
                                          LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                          LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                     END LOOP;
                                     IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                                     ELSE
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                                     END IF;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' Parent 2-LxPathTarget='||LxPathTarget); $end null;

                                     LNodeListTargetParent:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                     LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTargetParent, 0);

                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');

                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;

                                     LxPathTarget:=LxPathTarget||'/validationRuleList/validationRule[';
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                     FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                          LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                          LName :=dbms_xmldom.GetNodeName(LAttr);
                                          LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                          LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                     END LOOP;

                                     IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                                     ELSE
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                                     END IF;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'2-LxPathTarget='||LxPathTarget); $end null;

                                     LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);

                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListTarget)'||DBMS_XMLDom.GetLength(LNodeListTarget)); $end null;
                                     $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'**ERROR3** The tag will not be deleted: '||LxPathTarget); end if; $end null;
                                     IF DBMS_XMLDom.GetLength(LNodeListTarget)>0 THEN
                                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'Node will be deleted:'||LxPathTarget); $end null;
                                         LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                         LNodeTargetParent:=DBMS_XMLDom.RemoveChild(LNodeTargetParent,LNodeTarget);
                                     END IF;

                                 END LOOP;
                             END LOOP;
                         END;
                         WHEN 'PARAM' THEN
                         BEGIN
                             LNodeListSourceParent:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                             FOR LIndexParent IN 0..DBMS_XMLDom.GetLength(LNodeListSourceParent)-1 LOOP
                                 LNodeSourseParent:=DBMS_XMLDom.Item(LNodeListSourceParent,LIndexParent);
                                 LNodeListSource:=dbms_xslprocessor.selectNodes(LNodeSourseParent,'params/param[@delete="yes"]');
                                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListSource)'||DBMS_XMLDom.GetLength(LNodeListSource)); $end null;

                                 FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' LCoeObjectConfigField.GetClobVal()'||LCoeObjectConfigField.GetClobVal()); $end null;
                                     LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);


                                     LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);
                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');

                                     LNodeSourseGranParent :=dbms_xmldom.GetParentNode(LNodeSourseParent);
                                     LNodeSourseGranParent :=dbms_xmldom.GetParentNode(LNodeSourseGranParent);
                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourseGranParent);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'precision');
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'type');

                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;

                                     LxPathTarget:=AxPathTarget||'[';
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' Parent 1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                     FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                          LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                          LName :=dbms_xmldom.GetNodeName(LAttr);
                                          LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                          LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                     END LOOP;
                                     IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                                     ELSE
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                                     END IF;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' Parent 2-LxPathTarget='||LxPathTarget); $end null;

                                     LNodeListTargetParent:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                     LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTargetParent, 0);

                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourseParent);

                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourse: '||DBMS_XMLDOM.GETNODENAME(LNodeSourse)); $end null;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDOM.GETNODENAME LNodeSourseParent: '||DBMS_XMLDOM.GETNODENAME(LNodeSourseParent)); $end null;

                                     LxPathTarget:=LxPathTarget||'/validationRuleList/validationRule[';
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                     FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                          LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                          LName :=dbms_xmldom.GetNodeName(LAttr);
                                          LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                          LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                     END LOOP;
                                     IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                                     ELSE
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                                     END IF;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'2-LxPathTarget='||LxPathTarget); $end null;

                                     LNodeListTargetParent:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                     LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTargetParent, 0);

                                     LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                                     LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'delete');
                                     LxPathTarget:=LxPathTarget||'/params/param[';
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'1-LxPathTarget='||LxPathTarget||' Length: '||DBMS_XMLDom.GetLength(LAttrs)); $end null;
                                     FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrs)-1 LOOP
                                          LAttr := DBMS_XMLDom.Item(LAttrs,LIndexAttr);
                                          LName :=dbms_xmldom.GetNodeName(LAttr);
                                          LValue :=dbms_xmldom.GetNodeValue(LAttr);
                                          LxPathTarget:=LxPathTarget||'@'||LName||'="'||LValue||'" and ';
                                     END LOOP;
                                     IF SUBSTR(LxPathTarget,LENGTH(LxPathTarget),1)='[' THEN
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-1);
                                     ELSE
                                         LxPathTarget:=SUBSTR(LxPathTarget,1,LENGTH(LxPathTarget)-5)||']';
                                     END IF;
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'2-LxPathTarget='||LxPathTarget); $end null;


                                     LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,LxPathTarget);
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,' DBMS_XMLDom.GetLength(LNodeListTarget)'||DBMS_XMLDom.GetLength(LNodeListTarget)); $end null;
                                     $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'**ERROR4** The tag will not be deleted: '||LxPathTarget); end if; $end null;
                                     IF DBMS_XMLDom.GetLength(LNodeListTarget)>0 THEN
                                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'Node will be deleted:'||LxPathTarget); $end null;
                                         LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                         LNodeTargetParent:=DBMS_XMLDom.RemoveChild(LNodeTargetParent,LNodeTarget);
                                     END IF;
                                 END LOOP;
                             END LOOP;
                         END;
                     END CASE;
                 END IF;
             END;

            PROCEDURE XMLNodeUpdate(AxPathTarget VARCHAR2,AxPathSource VARCHAR2,ASection IN VARCHAR2) IS
                LNodeListSource       DBMS_XMLDom.DOMNodelist;
                LNodeListTarget       DBMS_XMLDom.DOMNodelist;
                LNodeSourse           DBMS_XMLDom.DOMNode;
                LNodeSourseParent     DBMS_XMLDom.DOMNode;
                LNodeTarget           DBMS_XMLDom.DOMNode;
                LNodeTargetParent     DBMS_XMLDom.DOMNode;
                LIndex                Integer;
                LIndexAttr            Integer;

                LAttrs                DBMS_XMLDom.DOMNamedNodeMap;
                LAttr                 DBMS_XMLDom.DOMNode;
                LAttrsSourse          DBMS_XMLDom.DOMNamedNodeMap;
                LAttrSourse           DBMS_XMLDom.DOMNode;
                LValue                VARCHAR2(4000);

                LAttrsParent          DBMS_XMLDom.DOMNamedNodeMap;
                LAttrParent           DBMS_XMLDom.DOMNode;
                LAttrsSourseParent    DBMS_XMLDom.DOMNamedNodeMap;
                LAttrSourseParent     DBMS_XMLDom.DOMNode;

                LAttrNameToUpdate     VARCHAR2(200);
                LAttrToUpdate         DBMS_XMLDom.DOMNode;
                LValueToUpdate        VARCHAR2(200);
            BEGIN
                CASE UPPER(ASection)
                    WHEN 'DEFAULT' THEN
                    BEGIN
                        LNodeListSource:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                        FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                            LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);

                            LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);

                            LAttrs := dbms_xmldom.Getattributes(LNodeSourse);

                            LAttr := dbms_xmldom.GetnamedItem(LAttrs,'update');
                            LAttrNameToUpdate := dbms_xmldom.GetNodevalue(LAttr);

                            LAttrToUpdate := dbms_xmldom.GetnamedItem(LAttrs,LAttrNameToUpdate);
                            LValueToUpdate := dbms_xmldom.GetNodevalue(LAttrToUpdate);

                            LAttr := dbms_xmldom.GetnamedItem(LAttrs,'name');
                            LValue := dbms_xmldom.GetNodevalue(LAttr);

                            IF Lvalue IS NOT NULL THEN
                                LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,AxPathTarget||'/Property[@name="'||Lvalue||'"]');
                                $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeUpdate'||' Line:'||$$plsql_line,'**ERROR** The tag will not be updated: '||AxPathTarget||'/Property[@name="'||Lvalue||'"]'); end if; $end null;
                                IF DBMS_XMLDom.GetLength(LNodeListTarget)<>0 THEN
                                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeUpdate'||' Line:'||$$plsql_line,'Node will be updated:'||AxPathTarget||'/Property[@name="'||Lvalue||'"]'); $end null;
                                    LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                    LAttrs := dbms_xmldom.Getattributes(LNodeTarget);
                                    LAttr := dbms_xmldom.GetnamedItem(LAttrs,LAttrNameToUpdate);
                                    dbms_xmldom.SetNodeValue(LAttr,LValueToUpdate);
                                END IF;
                            END IF;
                        END LOOP;
                    END;
                    WHEN 'ADDIN' THEN
                    BEGIN
                        LNodeListSource:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                        FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                            LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);

                            LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);

                            LAttrsSourse := dbms_xmldom.Getattributes(LNodeSourse);

                            LAttrSourse := dbms_xmldom.GetnamedItem(LAttrsSourse,'friendlyName');
                            LValue := dbms_xmldom.GetNodevalue(LAttrSourse);

                            IF Lvalue IS NOT NULL THEN
                                LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]');
                                $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeUpdate'||' Line:'||$$plsql_line,'**ERROR** The tag will not be updated: '||AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]'); end if; $end null;
                                IF DBMS_XMLDom.GetLength(LNodeListTarget)<>0 THEN
                                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeUpdate'||' Line:'||$$plsql_line,'Node will be updated:'||AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]'); $end null;
                                    FOR LIndexAttr IN 0..DBMS_XMLDom.GetLength(LAttrsSourse)-1  LOOP

                                        LAttrSourse := dbms_xmldom.Item(LAttrsSourse,LIndexAttr);
                                        LAttrNameToUpdate := dbms_xmldom.GetNodeName(LAttrSourse);

                                        LAttrToUpdate := dbms_xmldom.GetnamedItem(LAttrsSourse,LAttrNameToUpdate);
                                        LValueToUpdate := dbms_xmldom.GetNodevalue(LAttrToUpdate);

                                        LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                        LAttrs := dbms_xmldom.Getattributes(LNodeTarget);
                                        LAttr := dbms_xmldom.GetnamedItem(LAttrs,LAttrNameToUpdate);

                                        dbms_xmldom.SetNodeValue(LAttr,LValueToUpdate);
                                    END LOOP;
                                END IF;
                            END IF;
                        END LOOP;
                    END;
                    WHEN 'ADDINCOFIGURATION' THEN
                    BEGIN
                        LNodeListSource:=dbms_xslprocessor.selectNodes(ADocumentNode,AxPathSource);

                        FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                            LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);
                            LNodeSourseParent := dbms_xmldom.GetParentNode(LNodeSourse);
                            LNodeSourseParent := dbms_xmldom.importNode(LDOMDocument,LNodeSourseParent,TRUE);

                            LNodeSourse := dbms_xmldom.importNode(LDOMDocument,LNodeSourse,TRUE);
                            LAttrs := dbms_xmldom.Getattributes(LNodeSourse);
                            LAttr := dbms_xmldom.GetnamedItem(LAttrs,'update');
                            LValue := dbms_xmldom.GetNodevalue(LAttr);
                            IF LValue='yes' THEN
                                LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'update');

                                LAttrsSourseParent := dbms_xmldom.Getattributes(LNodeSourseParent);
                                LAttrSourseParent := dbms_xmldom.GetnamedItem(LAttrsSourseParent,'friendlyName');
                                LValue := dbms_xmldom.GetNodevalue(LAttrSourseParent);
                                LAttrNameToUpdate := dbms_xmldom.GetNodeName(LAttrSourseParent);

                                IF Lvalue IS NOT NULL THEN
                                    LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]/AddInConfiguration');
                                    $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeUpdate'||' Line:'||$$plsql_line,'**ERROR** The tag will not be taked off before inserted: '||AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]/AddInConfiguration'); end if; $end null;
                                    IF DBMS_XMLDom.GetLength(LNodeListTarget)<>0 THEN
                                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeUpdate'||' Line:'||$$plsql_line,'Node will be taked off before inserted:'||AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]/AddInConfiguration'); $end null;
                                        LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                        LNodeTargetParent:=DBMS_XMLDom.GetParentNode(LNodeTarget);
                                        LNodeTarget:=DBMS_XMLDom.RemoveChild(LNodeTargetParent,LNodeTarget);
                                    END IF;
                                    LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNode,AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]');
                                    $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeUpdate'||' Line:'||$$plsql_line,'**ERROR** The tag will not be updated: '||AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]'); end if; $end null;
                                    IF DBMS_XMLDom.GetLength(LNodeListTarget)<>0 THEN
                                        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeUpdate'||' Line:'||$$plsql_line,'Node will be updated:'||AxPathTarget||'/AddIn[@friendlyName="'||Lvalue||'"]'); $end null;
                                        LNodeTargetParent:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                        LNodeTargetParent:=dbms_xmldom.appendChild(LNodeTargetParent,LNodeSourse);
                                    END IF;
                                END IF;
                            END IF;
                        END LOOP;
                    END;
                END CASE;
            END;

        BEGIN
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Begin'); $end null;
            SELECT COUNT(1) INTO VExists FROM COEOBJECTCONFIG WHERE ID = 2;
            IF VExists > 0 THEN
                SELECT XmlType.CreateXml(Replace(XML,'&quot;','_quot;'))
                    INTO LCoeObjectConfigField
                    FROM COEOBJECTCONFIG
                    WHERE ID=2;
            ELSE
                LCoeObjectConfigField := GetDefaultMultiCompound;
            END IF;

            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'LCoeObjectConfigField:'||LCoeObjectConfigField.GetClobVal()); $end null;

            LDOMDocument  := DBMS_XMLDom.NewDOMDocument(LCoeObjectConfigField);
            LDocumentNode := DBMS_XMLDom.MakeNode(LDOMDocument);

            --Delete
            --  Defaut Sections
            --    Mixture Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Mixture Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/PropertyList/Property','/ConfigurationRegistryRecord/PropertyList/Property[@delete="yes"]','MIXTURE');
            --    Compound Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Compound Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList/Property','/ConfigurationRegistryRecord/Compound/PropertyList/Property[@delete="yes"]','COMPOUND');
            --    Structure Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Structure Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList/Property','/ConfigurationRegistryRecord/Structure/PropertyList/Property[@delete="yes"]','STRUCTURE');
            --    Batch Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Batch Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property','/ConfigurationRegistryRecord/Batch/PropertyList/Property[@delete="yes"]','BATCH');
            --  BatchComponent Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'BatchComponent Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property[@delete="yes"]','BATCHCOMPONENT');
            --    AddIn Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'AddIn Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/AddIns/AddIn','/ConfigurationRegistryRecord/AddIns/AddIn[@delete="yes"]','ADDIN');
            --  Events Sections
            --    Event Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Event Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/AddIns/AddIn','/ConfigurationRegistryRecord/AddIns/AddIn','EVENT');
            --  Validation Sections
            --    Mixture ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Mixture ValidationRule Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/PropertyList/Property','/ConfigurationRegistryRecord/PropertyList/Property','VALIDATION');
            --    Compound ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Compound ValidationRule Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList/Property','/ConfigurationRegistryRecord/Compound/PropertyList/Property','VALIDATION');
            --    Structure ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Structure ValidationRule Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList/Property','/ConfigurationRegistryRecord/Structure/PropertyList/Property','VALIDATION');
            --    Batch ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Batch ValidationRule Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property','/ConfigurationRegistryRecord/Batch/PropertyList/Property','VALIDATION');
            --    BatchComponent ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'BatchComponent ValidationRule Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property','VALIDATION');

            --  Parms Sections
            --    Mixture ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Mixture Param Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/PropertyList/Property','/ConfigurationRegistryRecord/PropertyList/Property/validationRuleList/validationRule','PARAM');
            --    Compound ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Compound Param Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList/Property','/ConfigurationRegistryRecord/Compound/PropertyList/Property/validationRuleList/validationRule','PARAM');
            --    Structure Params Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Structure Param Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList/Property','/ConfigurationRegistryRecord/Structure/PropertyList/Property/validationRuleList/validationRule','PARAM');
            --    Batch ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Batch Param Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property','/ConfigurationRegistryRecord/Batch/PropertyList/Property/validationRuleList/validationRule','PARAM');
            --    BatchComponent ValidationRule Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'BatchComponent Param Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property/validationRuleList/validationRule','PARAM');


            --Insert
            --  DefaultSections
            --    Mixture Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Mixture Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/PropertyList','/ConfigurationRegistryRecord/PropertyList/Property[@insert="yes"]','DEFAULT');
            --    Compound Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Compound Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList','/ConfigurationRegistryRecord/Compound/PropertyList/Property[@insert="yes"]','DEFAULT');
            --    Structure Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Structure Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList','/ConfigurationRegistryRecord/Structure/PropertyList/Property[@insert="yes"]','DEFAULT');
            --    Batch Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Batch Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList','/ConfigurationRegistryRecord/Batch/PropertyList/Property[@insert="yes"]','DEFAULT');
            --    BatchComponent Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'BatchComponent Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property[@insert="yes"]','DEFAULT');
            --  AddIns Sections
            --    AddIn Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'AddIn Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/AddIns','/ConfigurationRegistryRecord/AddIns/AddIn[@insert="yes"]','ADDIN');

            --  Events Sections
            --    Event Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Event Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/AddIns/AddIn','/ConfigurationRegistryRecord/AddIns/AddIn','EVENT');

            --  Validations Sections
            --    Mixture ValidationRule Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Mixture ValidationRule Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/PropertyList/Property','/ConfigurationRegistryRecord/PropertyList/Property','VALIDATION');
            --    Compound ValidationRule Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Compound ValidationRule Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList/Property','/ConfigurationRegistryRecord/Compound/PropertyList/Property','VALIDATION');
            --    Structure ValidationRule Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Structure ValidationRule Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList/Property','/ConfigurationRegistryRecord/Structure/PropertyList/Property','VALIDATION');
            --    Batch ValidationRule Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Batch ValidationRule Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property','/ConfigurationRegistryRecord/Batch/PropertyList/Property','VALIDATION');
            --    BatchComponent ValidationRule Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'BatchComponent ValidationRule Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property','VALIDATION');

            --  Params Sections
            --    Mixture Param Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Mixture Param Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/PropertyList/Property','/ConfigurationRegistryRecord/PropertyList/Property/validationRuleList/validationRule','PARAM');
            --    Compound Param Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Compound Param Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList/Property','/ConfigurationRegistryRecord/Compound/PropertyList/Property/validationRuleList/validationRule','PARAM');
            --    Structure Param Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Structure Param Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList/Property','/ConfigurationRegistryRecord/Structure/PropertyList/Property/validationRuleList/validationRule','PARAM');
            --    Batch Param Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Batch Param Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property','/ConfigurationRegistryRecord/Batch/PropertyList/Property/validationRuleList/validationRule','PARAM');
            --    BatchComponent Param Insert
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'BatchComponent Param Insert'); $end null;
            XMLNodeInsert('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property/validationRuleList/validationRule','PARAM');


            --Update
            --  Defaut Sections
            --    Mixture Update
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Mixture Update'); $end null;
            XMLNodeUpdate('/MultiCompoundRegistryRecord/PropertyList','/ConfigurationRegistryRecord/PropertyList/Property[@update!=""]','DEFAULT');
            XMLNodeUpdate('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList','/ConfigurationRegistryRecord/Compound/PropertyList/Property[@update!=""]','DEFAULT');
            XMLNodeUpdate('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList','/ConfigurationRegistryRecord/Structure/PropertyList/Property[@update!=""]','DEFAULT');
            XMLNodeUpdate('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList','/ConfigurationRegistryRecord/Batch/PropertyList/Property[@update!=""]','DEFAULT');
            XMLNodeUpdate('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property[@update!=""]','DEFAULT');
            --  AddIns Sections
            --    AddIn Update
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'AddIns Update'); $end null;
            XMLNodeUpdate('/MultiCompoundRegistryRecord/AddIns','/ConfigurationRegistryRecord/AddIns/AddIn[@update="yes"]','ADDIN');
            XMLNodeUpdate('/MultiCompoundRegistryRecord/AddIns','/ConfigurationRegistryRecord/AddIns/AddIn/AddInConfiguration[@update="yes"]','ADDINCOFIGURATION');


            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'LCoeObjectConfigField.GetClobVal()='||LCoeObjectConfigField.GetClobVal()); $end null;
            IF VExists > 0 THEN
                UPDATE COEOBJECTCONFIG
                   SET XML = Replace(LCoeObjectConfigField.GetClobVal(),'_quot;','&quot;')
                 WHERE ID = 2;
            ELSE
                INSERT INTO COEOBJECTCONFIG(ID,OBJECTTYPEID,XML) VALUES(2,1,LCoeObjectConfigField.GetClobVal());
            END IF;

            COMMIT;
        END;

     BEGIN
         ErrorsLog:='';

         SetSessionParameter;

         --Changes in DB

         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'Begin ACoeObjectConfigXML:'||ACoeObjectConfigXML); $end null;

         LDOMDocument := DBMS_XMLDom.NewDOMDocument(replace(ACoeObjectConfigXML,'&quot;','_quot;'));
         LDocumentNode := DBMS_XMLDom.MakeNode(LDOMDocument);

         LNodeList:=dbms_xslprocessor.selectNodes(LDocumentNode,'/ConfigurationRegistryRecord/PropertyList');
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'ProcessSection MIXTURE'); $end null;
         DBProcessSection(LNodeList,'MIXTURE');

         LNodeList:=dbms_xslprocessor.selectNodes(LDocumentNode,'/ConfigurationRegistryRecord/Compound/PropertyList');
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'ProcessSection COMPOUND'); $end null;
         DBProcessSection(LNodeList,'COMPOUND');

         LNodeList:=dbms_xslprocessor.selectNodes(LDocumentNode,'/ConfigurationRegistryRecord/Batch/PropertyList');
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'ProcessSection BATCH'); $end null;
         DBProcessSection(LNodeList,'BATCH');

         LNodeList:=dbms_xslprocessor.selectNodes(LDocumentNode,'/ConfigurationRegistryRecord/BatchComponent/PropertyList');
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'ProcessSection BATCH COMPONENT'); $end null;
         DBProcessSection(LNodeList,'BATCHCOMPONENT');

         LNodeList:=dbms_xslprocessor.selectNodes(LDocumentNode,'/ConfigurationRegistryRecord/Structure/PropertyList');
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'ProcessSection BASE FRAGMENT'); $end null;
         DBProcessSection(LNodeList,'STRUCTURE');

         ErrorsLog:='<ErrorList>'||CHR(13)||CHR(10)||ErrorsLog||CHR(13)||CHR(10)||'</ErrorList>';

         --Changes in XML
         ProcessAndSaveXMLInDB(LDocumentNode);

         SELECT Count(1) INTO LCountInvalid FROM User_Objects WHERE Object_Name='COMPOUNDREGISTRY' AND Status='INVALID';
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'LCountInvalid'||LCountInvalid); $end null;
         IF LCountInvalid>0 THEN
             EXECUTE IMMEDIATE 'ALTER PACKAGE COMPOUNDREGISTRY COMPILE';
         END IF;

         AErrorsLog:=ErrorsLog;

         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'End AErrorsLog='||AErrorsLog); $end null;

/*     EXCEPTION
         WHEN OTHERS THEN
         BEGIN
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
             RAISE_APPLICATION_ERROR(eGenericException, 'SetCoeObjectConfig'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
         END;*/
     END;

     PROCEDURE ProcessFieldsfromDB IS
         LCoeObjectConfigField     XmlType;
         LErrorLogs CLOB;
     BEGIN
         SELECT XmlType.CreateXml(XML)
           INTO LCoeObjectConfigField
           FROM COEOBJECTCONFIG
          WHERE ID=2;
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessFieldsfromDB'||' Line:'||$$plsql_line,LCoeObjectConfigField.GetClobval()); $end null;
         SetCoeObjectConfig(LCoeObjectConfigField.GetClobval(),LErrorLogs);
     EXCEPTION
         WHEN OTHERS THEN
         BEGIN
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessFieldsfromDB'||' Line:'||$$plsql_line,DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
             RAISE_APPLICATION_ERROR(eGenericException, 'ProcessFieldsfromDB'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
         END;
     END;

     PROCEDURE AddFieldToView(
       ATableFieldName VARCHAR2
       , AViewFieldName VARCHAR2
       , AViewName VARCHAR2
       , APrimaryTable in VARCHAR2 := NULL
     ) IS
        LStatement VARCHAR2(4000);
        LViewSelect VARCHAR2(10000);
        LViewFields VARCHAR2(10000) := NULL;
        LFieldExist INTEGER;
        CURSOR LCViewFields(ViewName VARCHAR2) IS
          SELECT Column_Name
          FROM User_Tab_Columns c
          WHERE Table_Name = ViewName
          ORDER BY Column_ID;
     BEGIN
      FOR RViewFields IN LCViewFields(AViewName) LOOP
        LViewFields := LViewFields || RViewFields.Column_Name || ',';
      END LOOP;
      LViewFields:=SUBSTR(LViewFields,1,LENGTH(LViewFields)-1);

      $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView_LViewFields_'||' Line:'||$$plsql_line, LViewFields); $end null;

      SELECT Text INTO LViewSelect
      FROM USER_VIEWS
      WHERE VIEW_NAME=AViewName;

      $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView_LViewSelect_'||' Line:'||$$plsql_line, LViewSelect); $end null;

      SELECT Count(1) INTO LFieldExist
      FROM USER_TAB_COLUMNS
      where UPPER(Table_name)=UPPER(AViewName) AND Column_Name=UPPER(AViewFieldName);

      $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView_LFieldExist_'||' Line:'||$$plsql_line, to_char(LFieldExist)); $end null;

      IF LFieldExist = 0 THEN
        LViewSelect :=
          SUBSTR(LViewSelect,1,INSTR(LViewSelect,'FROM ', -1)-2)||','||ATableFieldName||' '||SUBSTR(LViewSelect,INSTR(LViewSelect,'FROM ', -1),LENGTH(LViewSelect));

        LStatement :=
          'CREATE OR REPLACE VIEW ' || AViewName
          || '(' || LViewFields || ',' || AViewFieldName ||') AS '
          || LViewSelect;
      ELSE
        LStatement:='ALTER VIEW '||AViewName||' COMPILE';
      END IF;

      $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView_LStatement_'||' Line:'||$$plsql_line, LStatement); $end null;
      EXECUTE IMMEDIATE LStatement;

     END;

     PROCEDURE GetCoeObjectConfig(ACoeObjectConfigXML OUT CLOB) IS

        LCoeObjectConfigSource     XmlType;
        LCoeObjectConfigTarget     XmlType;
        LXMLCoeObjectConfig        CLOB;

        LDOMDocumentSource         DBMS_XMLDom.DOMDocument;
        LDOMDocumentTarget         DBMS_XMLDom.DOMDocument;

        LDocumentNodeSource        DBMS_XMLDom.DOMNode;
        LDocumentNodeTarget        DBMS_XMLDom.DOMNode;

        LNodeListSource            DBMS_XMLDom.DOMNodelist;
        LNodeListTarget            DBMS_XMLDom.DOMNodelist;

        LNodeSourse                DBMS_XMLDom.DOMNode;
        LNodeTarget                DBMS_XMLDom.DOMNode;
        LNodeTargetParent          DBMS_XMLDom.DOMNode;

        LElemenTarget              DBMS_XMLDom.DomElement;
        LElemenSource              DBMS_XMLDom.DomElement;


        LNodesCount                Number;
        LAttrs                     DBMS_XMLDom.DOMNamedNodeMap;
        LAttr                      DBMS_XMLDom.DOMNode;

        LFieldName                 Varchar2(100);
        LAttrValue                 Varchar2(100);
        LData_Type                 Varchar2(106);
        LData_Length               Number;
        LData_Precision            Number;
        LData_Scale                Number;
        LAux                       Varchar2(100);

        VExists                       Number(1) := 0;
        PROCEDURE AddProperty(APathSource VARCHAR2,APathTarget VARCHAR2,ATableName VARCHAR2,ASection IN VARCHAR2) IS
        BEGIN
            LNodeListSource:=dbms_xslprocessor.selectNodes(LDocumentNodeSource,APathSource);
            LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNodeTarget,APathTarget);

            LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
            LNodeTargetParent:=LNodeTarget;

            FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);

                LNodeSourse := dbms_xmldom.importNode(LDOMDocumentTarget,LNodeSourse,True);
                LElemenSource :=dbms_xmldom.MakeElement(LNodeSourse);

                LNodeTarget:=dbms_xmldom.appendChild(LNodeTargetParent,LNodeSourse);

                LElemenTarget:=dbms_xmldom.MakeElement(LNodeTarget);
                LAttrs := dbms_xmldom.Getattributes(LNodeTarget);

                LAttrValue:=dbms_xmldom.getAttribute(LElemenTarget,'name');

                BEGIN
                    GetDBFieldName(LAttrValue, LAux, LFieldName, ASection, 'DEFINITIVE');

                    SELECT Data_Type,Data_Length,Data_Precision,Data_Scale
                        INTO LData_Type,LData_Length,LData_Precision,LData_Scale
                        FROM User_Tab_Columns
                        WHERE Table_Name=ATableName AND COLUMN_NAME=LFieldName;

                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessFieldsfromDB',' LData_Type:'||LData_Type); $end null;
                    $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessFieldsfromDB',' LData_Length:'||LData_Length); $end null;


                    IF LData_Type='NUMBER' THEN
                        dbms_xmldom.setAttribute(LElemenTarget,'type','NUMBER');
                        IF LData_Precision IS NOT NULL THEN
                            dbms_xmldom.setAttribute(LElemenTarget,'precision',LData_Precision||'.'||NVL(LData_Scale,0));
                        END IF;
                    ELSIF LData_Type='DATE' THEN
                        dbms_xmldom.setAttribute(LElemenTarget,'type','DATE');
                    ELSIF LData_Type='VARCHAR2' THEN
                        IF dbms_xmldom.getAttribute(LElemenSource,'pickListDomainID') IS NULL THEN
                            dbms_xmldom.setAttribute(LElemenTarget,'type','TEXT');
                            dbms_xmldom.setAttribute(LElemenTarget,'precision',LData_Length);
                        ELSE
                            dbms_xmldom.setAttribute(LElemenTarget,'type','PICKLISTDOMAIN');
                        END IF;
                    ELSIF LData_Type='CLOB' THEN
                        dbms_xmldom.setAttribute(LElemenTarget,'type','TEXT');
                        dbms_xmldom.setAttribute(LElemenTarget,'precision','5000');
                    ELSIF LData_Type='NCHAR' AND LData_Length=2 THEN  --nchar(1) is when Data_Length=2
                        dbms_xmldom.setAttribute(LElemenTarget,'type','BOOLEAN');
                    END IF;
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN RAISE_APPLICATION_ERROR(eNoFieldExist,'The field '||LFieldName||' not exist in the table '||ATableName||'.');
                END;

            END LOOP;

        END;

    BEGIN
        SELECT COUNT(1) INTO VExists FROM COEOBJECTCONFIG WHERE ID=2;
        
        IF(VExists > 0) THEN
            SELECT XmlType.CreateXml(XML)
              INTO LCoeObjectConfigSource
              FROM COEOBJECTCONFIG
             WHERE ID=2;
        ELSE
            LCoeObjectConfigSource := GetDefaultMultiCompound;
        END IF;

         LCoeObjectConfigTarget := GetDefaultConfigXml;

        LDOMDocumentSource  := DBMS_XMLDom.NewDOMDocument(LCoeObjectConfigSource);
        LDocumentNodeSource := DBMS_XMLDom.MakeNode(LDOMDocumentSource);

        LDOMDocumentTarget  := DBMS_XMLDom.NewDOMDocument(LCoeObjectConfigTarget);
        LDocumentNodeTarget := DBMS_XMLDom.MakeNode(LDOMDocumentTarget);


        /*Mixture*/
        AddProperty('MultiCompoundRegistryRecord/PropertyList/Property',
                    'ConfigurationRegistryRecord/PropertyList',
                    'MIXTURES','MIXTURE');
        /*Compound*/
        AddProperty('MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList/Property',
                    'ConfigurationRegistryRecord/Compound/PropertyList',
                    'COMPOUND_MOLECULE','COMPOUND');
        /*Structure*/
        AddProperty('MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList/Property',
                    'ConfigurationRegistryRecord/Structure/PropertyList',
                    'STRUCTURES','STRUCTURE');
        /*Batch*/
        AddProperty('MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property',
                    'ConfigurationRegistryRecord/Batch/PropertyList',
                    'BATCHES','BATCH') ;
        /*BatchComponent*/

        AddProperty('MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property',
                    'ConfigurationRegistryRecord/BatchComponent/PropertyList',
                    'BATCHCOMPONENT','BATCHCOMPONENT');

        /*AddIns*/

        LNodeListSource:=dbms_xslprocessor.selectNodes(LDocumentNodeSource,'MultiCompoundRegistryRecord/AddIns');
        LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNodeTarget,'ConfigurationRegistryRecord');

        IF dbms_xmldom.getLength(LNodeListSource) > 0 THEN
            LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);

            LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,0);

            LNodeSourse := dbms_xmldom.importNode(LDOMDocumentTarget,LNodeSourse,TRUE);

            LNodeTarget:=dbms_xmldom.appendChild(LNodeTarget,LNodeSourse);
        END IF;

        ACoeObjectConfigXML:=LCoeObjectConfigTarget.GetClobval();

        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('GetCoeObjectConfig'||' Line:'||$$plsql_line,'ACoeObjectConfigXML='||ACoeObjectConfigXML); $end null;

    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('GetCoeObjectConfig'||' Line:'||$$plsql_line,DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'GetCoeObjectConfig'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

    FUNCTION GetDefaultMultiCompound RETURN XmlType IS
    BEGIN
        RETURN XmlType.CreateXml('<MultiCompoundRegistryRecord>
    <ID></ID>
    <DateCreated></DateCreated>
    <DateLastModified></DateLastModified>
    <PersonCreated></PersonCreated>
    <StructureAggregation></StructureAggregation>
    <Approved></Approved>
    <RegNumber>
            <RegID></RegID>
            <SequenceNumber></SequenceNumber>
            <RegNumber></RegNumber>
            <SequenceID>1</SequenceID>
    </RegNumber>
    <IdentifierList>
    </IdentifierList>
    <ProjectList>
    </ProjectList>
    <PropertyList>
    </PropertyList>
    <ComponentList>
     <Component>
      <ID></ID>
      <ComponentIndex>0</ComponentIndex>
      <Compound>
        <IdentifierList></IdentifierList>
    <CompoundID></CompoundID>
        <DateCreated></DateCreated>
        <PersonCreated></PersonCreated>
        <PersonRegistered></PersonRegistered>
        <DateLastModified></DateLastModified>
          <Tag></Tag>
        <PropertyList>
        </PropertyList>
        <RegNumber>
            <RegID></RegID>
            <SequenceNumber></SequenceNumber>
            <RegNumber></RegNumber>
            <SequenceID>2</SequenceID>
        </RegNumber>
        <BaseFragment>
            <Structure>
                <StructureID></StructureID>
                <StrucureFormat></StrucureFormat>
                <PropertyList>
                </PropertyList>
                <IdentifierList>
                </IdentifierList>
                <Structure molWeight="" formula="">
                    <validationRuleList>
                        <validationRule validationRuleName="requiredField" errorMessage="STRUCTURE required"></validationRule>
                        <validationRule validationRuleName="onlyChemicalContentAllowed"></validationRule>
                    </validationRuleList>
                </Structure>
        <NormalizedStructure></NormalizedStructure>
        <UseNormalization>T</UseNormalization>
            </Structure>
        </BaseFragment>
        <FragmentList>
        </FragmentList>
       </Compound>
      </Component>
     </ComponentList>
     <BatchList>
            <Batch>
                <BatchID></BatchID>
                <BatchNumber></BatchNumber>
        <FullRegNumber></FullRegNumber>
                <DateCreated></DateCreated>
                <PersonCreated></PersonCreated>
                <PersonRegistered></PersonRegistered>
                <DateLastModified></DateLastModified>
        <StatusID></StatusID>
        <ProjectList></ProjectList>
        <IdentifierList></IdentifierList>
                <PropertyList>
                </PropertyList>
                <Notebook></Notebook>
                <BatchComponentList>
                    <BatchComponent>
                        <ID />
                        <BatchID />
                        <CompoundID />
                        <ComponentIndex />
                        <PropertyList>
                      </PropertyList>
                        <BatchComponentFragmentList>
                            <BatchComponentFragment>
                                <ID/>
                                <FragmentID/>
                                <Equivalents/>
                            </BatchComponentFragment>
                        </BatchComponentFragmentList>
                    </BatchComponent>
                </BatchComponentList>
            </Batch>
    </BatchList>
    <AddIns/>
</MultiCompoundRegistryRecord>');
    END;
    
    FUNCTION GetDefaultConfigXml RETURN XmlType IS
    BEGIN
        RETURN XmlType.CreateXml('
            <ConfigurationRegistryRecord>
                <PropertyList>
                </PropertyList>
                <Compound>
                    <PropertyList>
                    </PropertyList>
                </Compound>
                <Structure>
                    <PropertyList>
                    </PropertyList>
                </Structure>
                <Batch>
                    <PropertyList>
                    </PropertyList>
                </Batch>
                <BatchComponent>
                    <PropertyList>
                    </PropertyList>
                </BatchComponent>
            </ConfigurationRegistryRecord>');
    END;
    
END ConfigurationCompoundRegistry;
/
