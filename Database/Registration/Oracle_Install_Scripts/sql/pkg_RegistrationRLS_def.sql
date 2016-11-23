prompt 
prompt Starting "pkg_RegistrationRLS_def.sql"...
prompt 

CREATE OR REPLACE PACKAGE RegistrationRLS AUTHID CURRENT_USER AS
/******************************************************************************
   NAME:       RegistrationRLS
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        6/13/2008   Fari             1. Created this package.
   2.0        4/09/2009   Fari             1. Added the restriction for Batch.
******************************************************************************/

  PROCEDURE ActivateRLS(AState Varchar2);     -- Uses the True value to activate RLS.
  FUNCTION  GetStateRLS RETURN Boolean;       -- If RLS is Active then return True. 
  PROCEDURE SetEnableRLS(AState in BOOLEAN);  -- Enables and disables temporally the RLS functionality
  FUNCTION  PeopleProject_RLL_Function(p_schema in VARCHAR2, p_object in VARCHAR2) RETURN VARCHAR2;  --Has the restriction for the RLS functionality.  
  PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB);


  gEnableRLS Boolean:=True;         -- Used by SetEnableRLS procedure

  Debuging Constant boolean:=False; -- Must always be False in PerForce. Developers can change this locally if necessary
  eGenericException Constant Number:=-20000;

END RegistrationRLS;
/