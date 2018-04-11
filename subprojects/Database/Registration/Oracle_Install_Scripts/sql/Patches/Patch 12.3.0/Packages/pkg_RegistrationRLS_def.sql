prompt 
prompt Starting "pkg_RegistrationRLS_def.sql"...
prompt 

CREATE OR REPLACE PACKAGE REGDB.RegistrationRLS AUTHID CURRENT_USER AS

/******************************************************************************
   NAME:       RegistrationRLS
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   11.0.1      6/13/2008   Fari             1. Created this package.
   11.0.1      4/09/2009   Fari             1. Added the restriction for Batch.
   11.0.3     10/09/2010   Fari             1. Implemetation of Registry Level Projects and Batch Level Projects
   11.0.3     10/15/2010   Fari             1. Implementation of users exempted of RLS.
   11.0.3     11/05/2010   Fari             1. Implementation of the funcionality of editable Registry.



******************************************************************************/

  PROCEDURE ActivateRLS(AState Varchar2);       -- Uses N(Off) or R (Registry Level Projects) or B (Batch Level Projects).
  FUNCTION  GetStateRLS RETURN Boolean;         -- If Config RLS is Active and gEnableRLS is true then return True.
  FUNCTION  GetConfigStateRLS RETURN Varchar2;  -- If Config RLS parameter is Active then return 'T'.
  FUNCTION  GetLevelRLS RETURN Varchar2;        -- If Config RLS parameter is Active then return 'T'.
  PROCEDURE SetEnableRLS(AState in BOOLEAN);    -- Enables and disables temporally the RLS functionality
  FUNCTION  PeopleProject_RLL_Function(p_schema in VARCHAR2, p_object in VARCHAR2) RETURN VARCHAR2;  --This has the restriction for the RLS functionality.
  PROCEDURE SetExemptRLS(AExemptRLS  Varchar2,AClientID  Varchar2); -- Set Yes or No the variable ExemptRLS in the context "RegistrationUsersCtx" for a ClintID
  PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB);


  gEnableRLS Boolean:=True;                     -- Used by SetEnableRLS procedure

  Debuging Constant boolean:=False;             -- Must always be False in PerForce. Developers can change this locally if necessary
  eGenericException Constant Number:=-20000;

END RegistrationRLS;
/