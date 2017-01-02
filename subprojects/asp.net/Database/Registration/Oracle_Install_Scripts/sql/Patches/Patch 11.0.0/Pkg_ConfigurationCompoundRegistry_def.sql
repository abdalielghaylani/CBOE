prompt 
prompt Starting "pkg_ConfigurationCompoundRegistry_def.sql"...
prompt 

CREATE OR REPLACE PACKAGE REGDB.ConfigurationCompoundRegistry AS
/******************************************************************************
   NAME:       PropertyList
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        28/08/2008   Fari              1. Created this package body.
   1.1        18/11/2008   Fari              1. Added GetCoeObjectConfig procedure.
******************************************************************************/

  PROCEDURE SetCoeObjectConfig(ACoeObjectConfigXML IN CLOB,AErrorsLog OUT CLOB);
  PROCEDURE GetCoeObjectConfig(ACoeObjectConfigXML OUT CLOB);

  Debuging Constant boolean:=False;

  eGenericException Constant Number:=-20000;
  eNoFieldExist Constant Number:=-20001;
  eFieldNotEmpty Constant Number:=-20002;

END ConfigurationCompoundRegistry;
/