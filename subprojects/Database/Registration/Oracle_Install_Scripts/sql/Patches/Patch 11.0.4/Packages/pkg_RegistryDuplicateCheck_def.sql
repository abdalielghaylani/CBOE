prompt 
prompt Starting "pkg_RegistryDuplicateCheck_def.sql"...
prompt 

CREATE OR REPLACE PACKAGE REGDB.RegistryDuplicateCheck IS

  -- Author  : JDUGAS
  -- Created : 9/28/2010 8:47:52 PM
  -- Purpose : Stores logic for duplicate-checking

  -- Public type declarations
  TYPE ComponentTable is TABLE OF Component;

  -- Public constant declarations
  -- Public variable declarations

  -- Public function and procedure declarations
  FUNCTION Extract_Compounds (
    p_cursor IN sys_refcursor
  ) RETURN ComponentTable PIPELINED;

  FUNCTION Compounds_By_StructProperty (
    p_property_name IN varchar2
    , p_property_value IN varchar2
  ) RETURN sys_refcursor;

  FUNCTION Compounds_By_StructIdentifier(
    p_identifier_name IN varchar2
    , p_identifier_value IN varchar2
  ) RETURN sys_refcursor;

  FUNCTION Compounds_By_Structure (
    p_cartridge_parms IN varchar2
    , p_structure IN vw_structure.structure%TYPE
  ) RETURN sys_refcursor;

  FUNCTION Compounds_By_Identifier (
    p_identifier_name IN varchar2
    , p_identifier_value IN varchar2
  ) RETURN sys_refcursor;

  FUNCTION Compounds_By_Property (
    p_property_name IN varchar2
    , p_property_value IN varchar2
  ) RETURN sys_refcursor;

  FUNCTION Matched_Compounds_Wrapper (
    p_type IN varchar2
    , p_params IN varchar2
    , p_value IN clob
  ) RETURN sys_refcursor;

  FUNCTION FindComponentDuplicates (
    p_type IN varchar2
    , p_params IN varchar2
    , p_value IN clob
  ) RETURN clob;

END RegistryDuplicateCheck;
/
