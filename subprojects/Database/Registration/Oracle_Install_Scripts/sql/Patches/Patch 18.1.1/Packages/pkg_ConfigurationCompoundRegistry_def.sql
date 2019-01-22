prompt 
prompt Starting "pkg_ConfigurationCompoundRegistry_def.sql"...
prompt 

CREATE OR REPLACE PACKAGE ConfigurationCompoundRegistry AS

  -- Author  : Fari
  -- Created : 28/08/2008
  -- Purpose : PropertyList DDL
   
  --  Types
  
  PROCEDURE SetCoeObjectConfig(ACoeObjectConfigXML IN CLOB,AErrorsLog OUT CLOB);
  PROCEDURE GetCoeObjectConfig(ACoeObjectConfigXML OUT CLOB);
  PROCEDURE AddFieldToView(ATableFieldName VARCHAR2, AViewFieldName VARCHAR2, AViewName VARCHAR2, APrimaryTable in VARCHAR2 := NULL);
  PROCEDURE ValidateTables(AErrorsLog OUT CLOB,APropertyListType VARCHAR2,AField VARCHAR2,AValidationType VARCHAR2);
  PROCEDURE UpdateTables(AUpdateLogXML in clob,AErrorsLog OUT varchar2);
  PROCEDURE CreateOrReplaceTypes;
  
  
  Debuging Constant boolean:=False;

  eGenericException Constant Number:=-20000;
  eNoFieldExist     Constant Number:=-20001;
  eFieldNotEmpty    Constant Number:=-20002;

  eFieldNotEmptyLengthBig     Constant Number:=-20003;
  eFieldNotEmptyNotCompatible Constant Number:=-20004;
  eFieldNotEmptyInvalidType   Constant Number:=-20005;

END ConfigurationCompoundRegistry;
/
