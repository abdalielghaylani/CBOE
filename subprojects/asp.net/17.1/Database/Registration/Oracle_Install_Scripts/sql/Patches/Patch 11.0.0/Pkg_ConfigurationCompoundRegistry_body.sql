prompt 
prompt Starting "pkg_ConfigurationCompoundRegistry_body.sql"...
prompt 

CREATE OR REPLACE PACKAGE BODY ConfigurationCompoundRegistry AS
/******************************************************************************
   NAME:       PropertyList
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        28/08/2008   Fari              1. Created this package body.
   1.1        18/11/2008   Fari              1. Added GetCoeObjectConfig procedure.
******************************************************************************/
    ErrorsLog Clob;

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
         END CASE;
     END;

     PROCEDURE AddErrorLog(AOperation IN VARCHAR2,AField IN VARCHAR2, AObject IN VARCHAR2, AFieldType IN VARCHAR2,ASection IN VARCHAR2) IS
     BEGIN
        $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField'||' Line:'||$$plsql_line,'Error '||AOperation||' the field "'||AField||'" in "'||ASection||'"-> '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
        ErrorsLog:=ErrorsLog||
        '     <Error>'||CHR(10)||
        '          <Operation>'||AOperation||'</Operation>'||CHR(10)||
        '          <Field>'||AField||'</Field>'||CHR(10)||
        '          <Object>'||AObject||'</Object>'||CHR(10)||
        '          <Fieldtype>'||AFieldType||'</Fieldtype>'||CHR(10)||
        '          <Section>'||ASection||'</Section>'||CHR(10)||
        '          <Message>'||SUBSTR(DBMS_UTILITY.FORMAT_ERROR_STACK,1,LENGTH(DBMS_UTILITY.FORMAT_ERROR_STACK)-1)||'</Message>'||CHR(10)||
        '     </Error>'||CHR(10);
     END;

     PROCEDURE AddField(AField IN VARCHAR2, ASection IN VARCHAR2, AFieldType IN VARCHAR2) IS
         LStatement   Varchar2(4000);
         LViewField   Varchar2(100);
         LTableField  Varchar2(100);
         LObject      Varchar2(100);

         FUNCTION AddFieldToView(AViewName VARCHAR2,APrimaryTable in VARCHAR2:='') RETURN VARCHAR2 IS
             LViewSelect VARCHAR2(10000);
             LViewFields VARCHAR2(10000);
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
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'LViewSelect=1'||LViewSelect); $end null;
             LViewSelect:=SUBSTR(LViewSelect,1,INSTR(LViewSelect,'FROM '||APrimaryTable)-2)||','||LTableField||' '||SUBSTR(LViewSelect,INSTR(LViewSelect,'FROM '||APrimaryTable),LENGTH(LViewSelect));
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'LViewSelect=2'||LViewSelect); $end null;
             LStatement:='CREATE OR REPLACE VIEW '||AViewName||'('||LViewFields||','||LViewField||') AS '||LViewSelect;
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldToView'||' Line:'||$$plsql_line,'LStatement='||LStatement); $end null;
             RETURN LStatement;
         END;

     BEGIN
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddField'||' Line:'||$$plsql_line,'AField='||AField||' ASection='||ASection||' AFieldType='||AFieldType); $end null;

         IF (INSTR(UPPER(AFieldType),'NUMBER')<>0) OR (INSTR(UPPER(AFieldType),'VARCHAR2')<>0) OR (INSTR(UPPER(AFieldType),'DATE')<>0) OR (INSTR(UPPER(AFieldType),'CLOB')<>0) OR (INSTR(UPPER(AFieldType),'NCHAR')<>0) THEN
             CASE UPPER(ASection)
                WHEN 'MIXTURE' THEN
                     BEGIN
                         GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                         LObject:='MIXTURES';
                         LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='VW_MIXTURE';
                         LStatement:=AddFieldToView(LObject);
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='TEMPORARY_BATCH';
                         LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='VW_TEMPORARYBATCH';
                         LStatement:=AddFieldToView(LObject,'TEMPORARY_BATCH');
                         EXECUTE IMMEDIATE LStatement;
                    END;
                 WHEN 'COMPOUND' THEN
                     BEGIN
                         GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                         LObject:='COMPOUND_MOLECULE';
                         LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='VW_COMPOUND';
                         LStatement:=AddFieldToView(LObject);
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='TEMPORARY_COMPOUND';
                         LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='VW_TEMPORARYCOMPOUND';
                         LStatement:=AddFieldToView(LObject);
                         EXECUTE IMMEDIATE LStatement;
                    END;
                 WHEN 'BATCH' THEN
                      BEGIN
                         GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                         LObject:='BATCHES';
                         LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='VW_BATCH';
                         LStatement:=AddFieldToView(LObject);
                         EXECUTE IMMEDIATE LStatement;

                         GetDBFieldName(AField,LViewField,LTableField,ASection,'TEMPORARY');

                         LObject:='TEMPORARY_BATCH';
                         LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='VW_TEMPORARYBATCH';
                         LStatement:=AddFieldToView(LObject,'TEMPORARY_BATCH');
                         EXECUTE IMMEDIATE LStatement;
                    END;
                 WHEN 'BATCHCOMPONENT' THEN
                     BEGIN
                         GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                         LObject:='BATCHCOMPONENT';
                         LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='VW_BATCHCOMPONENT';
                         LStatement:=AddFieldToView(LObject);
                         EXECUTE IMMEDIATE LStatement;

                         GetDBFieldName(AField,LViewField,LTableField,ASection,'TEMPORARY');

                         LObject:='TEMPORARY_COMPOUND';
                         LStatement:='ALTER TABLE '||LObject||' ADD '||UPPER(LTableField)||' '||AFieldType;
                         EXECUTE IMMEDIATE LStatement;

                         LObject:='VW_TEMPORARYCOMPOUND';
                         LStatement:=AddFieldToView(LObject,'TEMPORARY_COMPOUND');
                         EXECUTE IMMEDIATE LStatement;
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
         FUNCTION AddFieldsToView(AViewName VARCHAR2) RETURN VARCHAR2 IS
             LViewSelect VARCHAR2(10000);
             LViewFields VARCHAR2(10000);
             LTable      VARCHAR2(100);
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

             LViewSelect:=REPLACE(' '||SUBSTR(UPPER(LViewSelect),8,LENGTH(LViewSelect)),CHR(10),'');
             LTable:=SUBSTR(LViewSelect,INSTR(LViewSelect,'FROM ',-1),LENGTH(LViewSelect));
             LViewSelect:=REPLACE(LViewSelect,LTable,' ');

             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('AddFieldsToView'||' Line:'||$$plsql_line,'LTableField 1='||LTableField); $end null;

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

         PROCEDURE VerifyEmptyField(AField IN VARCHAR2, aTable IN VARCHAR2) IS
             LCount Number;
         BEGIN
             LStatement:='SELECT COUNT(1) FROM '||aTable||' WHERE ROWNUM<2 AND '||AField||' IS NOT NULL';
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('VerifyEmptyField'||' Line:'||$$plsql_line,'LStatement='||LStatement); $end null;
             BEGIN
                 EXECUTE IMMEDIATE LStatement INTO LCount;
                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'LCount='||LCount); $end null;
                 IF LCount>0 THEN
                     RAISE_APPLICATION_ERROR (eFieldNotEmpty,'Field not empty.');
                 END IF;
             END;
         END;
     BEGIN
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'AField='||AField||' ASection='||ASection); $end null;
         CASE UPPER(ASection)
             WHEN 'MIXTURE' THEN
                 BEGIN

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='MIXTURES';
                     VerifyEmptyField(LTableField,LObject);
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'1 LStatement='||LStatement); $end null;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_MIXTURE';
                     LStatement:=AddFieldsToView(LObject);
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'2 LStatement='||LStatement); $end null;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='TEMPORARY_BATCH';
                     VerifyEmptyField(LTableField,LObject);
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'VerifyEmptyField LTableField='||LTableField||' LObject='||LObject); $end null;
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'3 LStatement='||LStatement); $end null;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYBATCH';
                     LStatement:=AddFieldsToView(LObject);
                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DropField'||' Line:'||$$plsql_line,'4 LStatement='||LStatement); $end null;
                     EXECUTE IMMEDIATE LStatement;

                 END;
             WHEN 'COMPOUND' THEN
                 BEGIN

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='COMPOUND_MOLECULE';
                     VerifyEmptyField(LTableField,LObject);
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_COMPOUND';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='TEMPORARY_COMPOUND';
                     VerifyEmptyField(LTableField,LObject);
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYCOMPOUND';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                 END;
             WHEN 'BATCH' THEN
                 BEGIN

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='BATCHES';
                     VerifyEmptyField(LTableField,LObject);
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_BATCH';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'TEMPORARY');

                     LObject:='TEMPORARY_BATCH';
                     VerifyEmptyField(LTableField,LObject);
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYBATCH';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                 END;
             WHEN 'BATCHCOMPONENT' THEN
                 BEGIN

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'DEFINITIVE');

                     LObject:='BATCHCOMPONENT';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_BATCHCOMPONENT';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

                     GetDBFieldName(AField,LViewField,LTableField,ASection,'TEMPORARY');

                     LObject:='TEMPORARY_COMPOUND';
                     LStatement:='ALTER TABLE '||LObject||' DROP COLUMN '||LTableField;
                     EXECUTE IMMEDIATE LStatement;

                     LObject:='VW_TEMPORARYCOMPOUND';
                     LStatement:=AddFieldsToView(LObject);
                     EXECUTE IMMEDIATE LStatement;

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
         BEGIN
             LNodesCount:=DBMS_XMLDom.GetLength(ANodeList);
             FOR LIndex IN 0..LNodesCount-1 LOOP
                 LNode := DBMS_XMLDom.Item(ANodeList, LIndex);
                 LAttrs := dbms_xmldom.Getattributes(LNode);
                 LAttr := dbms_xmldom.GetNamedItem(LAttrs,'name');
                 LFiled := dbms_xmldom.GetNodeValue(LAttr);

                 LAttrs := dbms_xmldom.Getattributes(LNode);
                 LAttr := dbms_xmldom.GetNamedItem(LAttrs,'type');
                 LAttributeName :=UPPER(dbms_xmldom.GetNodeValue(LAttr));

                 $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessInsert'||' Line:'||$$plsql_line,'LFiled->'||LFiled||' LAttributeName->'||LAttributeName); $end null;

                 IF LAttributeName IN ('TEXT','NUMBER') THEN
                     LAttr := dbms_xmldom.GetNamedItem(LAttrs,'precision');
                     LLong := dbms_xmldom.GetNodeValue(LAttr);

                     IF LAttributeName='TEXT' THEN
                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessInsert'||' Line:'||$$plsql_line,'Text LFiled->'||LFiled||' LLong->'||LLong||' LAttributeName->'||LAttributeName); $end null;
                         IF LLong<=4000 THEN
                             LFiledType:='VARCHAR2('||LLong||')';
                         ELSE
                             LFiledType:='CLOB';
                         END IF;
                     ELSIF LAttributeName='NUMBER' THEN
                         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessInsert'||' Line:'||$$plsql_line,'Number LFiled->'||LFiled||' LLong->'||LLong||' LAttributeName->'||LAttributeName); $end null;
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
                 ELSE
                     LFiledType:=LAttributeName;
                 END IF;

                 AddField(LFiled,ASection,LFiledType);

             END LOOP;
         END;
         PROCEDURE DBProcessUpdate(ANodeList DBMS_XMLDom.DOMNodelist,ASection in VARCHAR2) IS
         BEGIN
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('DBProcessUpdate'||' Line:'||$$plsql_line,'DBMS_XMLDom.GetLength(ANodeList)->'||DBMS_XMLDom.GetLength(ANodeList)); $end null;
             DBProcessDelete(ANodeList,ASection);
             DBProcessInsert(ANodeList,ASection);
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
         END;

         PROCEDURE ProcessAndSaveXMLInDB(ADocumentNode DBMS_XMLDom.DOMNode) IS
             LCoeObjectConfigField XMLType;

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
                                    LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'precision');
                                    LAttr := dbms_xmldom.RemoveNamedItem(LAttrs,'type');
                                    LNodeTargetAux:=dbms_xmldom.appendChild(LNodeTarget,LNodeSourse);
                                 END IF;
                             END IF;
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
             BEGIN
                 CASE UPPER(ASection)
                     WHEN 'DEFAULT' THEN
                     BEGIN
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
                             $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'**ERROR** The tag will not be deleted: '||LxPathTarget); end if; $end null;
                             IF DBMS_XMLDom.GetLength(LNodeListTarget)>0 THEN
                                  $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'Node will be deleted:'||LxPathTarget); $end null;
                                  LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                  LNodeTargetParent:=DBMS_XMLDom.REMOVECHILD(LNodeTargetParent,LNodeTarget);
                             END IF;

                         END LOOP;
                     END;
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
                                 $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'**ERROR** The tag will not be deleted: '||LxPathTarget); end if; $end null;
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
                                 $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'**ERROR** The tag will not be deleted: '||LxPathTarget); end if; $end null;
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
                                 $if ConfigurationCompoundRegistry.Debuging $then if DBMS_XMLDom.GetLength(LNodeListTarget)<=0 then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'**ERROR** The tag will not be deleted: '||LxPathTarget); end if; $end null;
                                 IF DBMS_XMLDom.GetLength(LNodeListTarget)>0 THEN
                                     $if ConfigurationCompoundRegistry.Debuging $then InsertLog('XMLNodeDelete'||' Line:'||$$plsql_line,'Node will be deleted:'||LxPathTarget); $end null;
                                     LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
                                     LNodeTargetParent:=DBMS_XMLDom.RemoveChild(LNodeTargetParent,LNodeTarget);
                                 END IF;

                             END LOOP;
                         END LOOP;

                     END;
                 END CASE;
             END;
        PROCEDURE XMLNodeUpdate(AxPathTarget VARCHAR2,AxPathSource VARCHAR2,ASection IN VARCHAR2) IS
            LNodeListSource       DBMS_XMLDom.DOMNodelist;
            LNodeListTarget       DBMS_XMLDom.DOMNodelist;
            LNodeSourse           DBMS_XMLDom.DOMNode;
            LNodeTarget           DBMS_XMLDom.DOMNode;
            LIndex                Integer;

            LAttrs                DBMS_XMLDom.DOMNamedNodeMap;
            LAttr                 DBMS_XMLDom.DOMNode;
            LValue                VARCHAR2(4000);

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
            END CASE;
        END;

        BEGIN
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Begin'); $end null;
            SELECT XmlType.CreateXml(XML)
                INTO LCoeObjectConfigField
                FROM COEOBJECTCONFIG
                WHERE ID=2;

            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'LCoeObjectConfigField:'||LCoeObjectConfigField.GetClobVal()); $end null;

            LDOMDocument  := DBMS_XMLDom.NewDOMDocument(LCoeObjectConfigField);
            LDocumentNode := DBMS_XMLDom.MakeNode(LDOMDocument);
            
            --Delete
            --  Defaut Sections
            --    Mixture Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Mixture Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/PropertyList/Property','/ConfigurationRegistryRecord/PropertyList/Property[@delete="yes"]','DEFAULT');
            --    Compound Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Compound Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList/Property','/ConfigurationRegistryRecord/Compound/PropertyList/Property[@delete="yes"]','DEFAULT');
            --    Batch Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'Batch Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property','/ConfigurationRegistryRecord/Batch/PropertyList/Property[@delete="yes"]','DEFAULT');
            --  BatchComponent Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'BatchComponent Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property[@delete="yes"]','DEFAULT');
            --    AddIn Delete
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'AddIn Delete'); $end null;
            XMLNodeDelete('/MultiCompoundRegistryRecord/AddIns/AddIn','/ConfigurationRegistryRecord/AddIns/AddIn[@delete="yes"]','DEFAULT');
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
            XMLNodeUpdate('/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList','/ConfigurationRegistryRecord/Batch/PropertyList/Property[@update!=""]','DEFAULT');
            XMLNodeUpdate('/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList','/ConfigurationRegistryRecord/BatchComponent/PropertyList/Property[@update!=""]','DEFAULT');


          $if ConfigurationCompoundRegistry.Debuging $then InsertLog('ProcessAndSaveXMLInDB'||' Line:'||$$plsql_line,'LCoeObjectConfigField.GetClobVal()='||LCoeObjectConfigField.GetClobVal()); $end null;
            UPDATE COEOBJECTCONFIG
               SET XML = LCoeObjectConfigField.GetClobVal()
             WHERE ID = 2;

            COMMIT;
        END;

     BEGIN
         ErrorsLog:='';

         SetSessionParameter;
         
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'Begin ACoeObjectConfigXML:'||ACoeObjectConfigXML); $end null;

         LDOMDocument := DBMS_XMLDom.NewDOMDocument(ACoeObjectConfigXML);
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

         ProcessAndSaveXMLInDB(LDocumentNode);

         SELECT Count(1) INTO LCountInvalid FROM User_Objects WHERE Object_Name='COMPOUNDREGISTRY' AND Status='INVALID';
         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'LCountInvalid'||LCountInvalid); $end null;
         IF LCountInvalid>0 THEN
             EXECUTE IMMEDIATE 'ALTER PACKAGE COMPOUNDREGISTRY COMPILE';
         END IF;
         
         AErrorsLog:='<ErrorList>'||CHR(13)||CHR(10)||ErrorsLog||CHR(13)||CHR(10)||'</ErrorList>';

         $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,'End'); $end null;

     EXCEPTION
         WHEN OTHERS THEN
         BEGIN
             $if ConfigurationCompoundRegistry.Debuging $then InsertLog('SetCoeObjectConfig'||' Line:'||$$plsql_line,DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
             RAISE_APPLICATION_ERROR(eGenericException, 'SetCoeObjectConfig'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
         END;
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

        PROCEDURE AddProperty(APathSource VARCHAR2,APathTarget VARCHAR2,ATableName VARCHAR2,ASection IN VARCHAR2) IS
        BEGIN
            LNodeListSource:=dbms_xslprocessor.selectNodes(LDocumentNodeSource,APathSource);
            LNodeListTarget:=dbms_xslprocessor.selectNodes(LDocumentNodeTarget,APathTarget);

            LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);
            LNodeTargetParent:=LNodeTarget;

            FOR LIndex IN 0..DBMS_XMLDom.GetLength(LNodeListSource)-1 LOOP
                LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,LIndex);

                LNodeSourse := dbms_xmldom.importNode(LDOMDocumentTarget,LNodeSourse,True);

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
                    ELSIF LData_Type='VARCHAR2'THEN
                        dbms_xmldom.setAttribute(LElemenTarget,'type','TEXT');
                        dbms_xmldom.setAttribute(LElemenTarget,'precision',LData_Length);
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
        SELECT XmlType.CreateXml(XML)
          INTO LCoeObjectConfigSource
          FROM COEOBJECTCONFIG
         WHERE ID=2;

         LCoeObjectConfigTarget:=XmlType.CreateXml('
            <ConfigurationRegistryRecord>
                <PropertyList>
                </PropertyList>
                <Compound>
                    <PropertyList>
                    </PropertyList>
                </Compound>
                <Batch>
                    <PropertyList>
                    </PropertyList>
                </Batch>
                <BatchComponent>
                    <PropertyList>
                    </PropertyList>
                </BatchComponent>
            </ConfigurationRegistryRecord>');

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

        LNodeTarget:=DBMS_XMLDom.Item(LNodeListTarget, 0);

        LNodeSourse := DBMS_XMLDom.Item(LNodeListSource,0);

        LNodeSourse := dbms_xmldom.importNode(LDOMDocumentTarget,LNodeSourse,TRUE);

        LNodeTarget:=dbms_xmldom.appendChild(LNodeTarget,LNodeSourse);

        ACoeObjectConfigXML:=LCoeObjectConfigTarget.GetClobval();
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if ConfigurationCompoundRegistry.Debuging $then InsertLog('GetCoeObjectConfig'||' Line:'||$$plsql_line,DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, 'GetCoeObjectConfig'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    END;

END ConfigurationCompoundRegistry;
/