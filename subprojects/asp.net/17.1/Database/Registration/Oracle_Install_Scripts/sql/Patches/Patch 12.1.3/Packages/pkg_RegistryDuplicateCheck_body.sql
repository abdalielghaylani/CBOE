prompt 
prompt Starting "pkg_RegistryDuplicateCheck_body.sql"...
prompt 

CREATE OR REPLACE PACKAGE BODY REGDB.RegistryDuplicateCheck IS

    -- Private type declarations
  -- Private constant declarations
  -- Private variable declarations
  -- Function and procedure implementations

  FUNCTION Extract_Compounds(
    p_cursor IN sys_refcursor
  ) RETURN ComponentTable PIPELINED
  IS
    component_in VW_COMPOUND_IDS%ROWTYPE;
  BEGIN
    LOOP FETCH p_cursor INTO component_in;
      EXIT WHEN p_cursor%NOTFOUND;
      PIPE ROW (Component
        (component_in.compoundid)
      );
    END LOOP;
    CLOSE p_cursor;

    RETURN;
  END;

  FUNCTION Compounds_By_StructProperty (
    p_property_name IN varchar2
    , p_property_value IN varchar2
  ) RETURN sys_refcursor
  IS
    v_refcur sys_refcursor;
    v_sql varchar2(1000);
  BEGIN
    BEGIN
      v_sql := 
      'select c.compoundid
       from vw_compound c
         inner join vw_structure s on s.structureid = c.structureid
       where upper(s.'
      	|| p_property_name
        || ') = upper(:1)';
    END;
    -- create the list of compound IDs
    OPEN v_refcur FOR v_sql USING p_property_value;

    RETURN v_refcur;
  END;

  FUNCTION Compounds_By_StructIdentifier(
    p_identifier_name IN varchar2
    , p_identifier_value IN varchar2
  ) RETURN sys_refcursor
  IS
    v_refcur sys_refcursor;
    v_sql varchar2(2000) :=
    'select distinct c.compoundid
     from vw_compound c
       inner join vw_structure s on s.structureid = c.structureid
       inner join vw_structure_identifier si on si.structureid = s.structureid
       inner join vw_identifiertype it on it.id = si.type
     where it.name = :1 and upper(si.value) = upper(:2)';

  BEGIN
    -- create the list of compound IDs
    OPEN v_refcur FOR v_sql USING p_identifier_name, p_identifier_value;

    RETURN v_refcur;
  END;

  FUNCTION Compounds_By_Structure(
    p_cartridge_parms varchar2
    , p_structure IN vw_structure.structure%TYPE
  ) RETURN sys_refcursor
  IS
    v_refcur sys_refcursor;
  BEGIN

    -- Cannot perform DML inside a select query
    --INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (p_structure, 0);

    -- create the list of compound IDs
    OPEN v_refcur FOR
      SELECT c.compoundid
      FROM vw_compound c
        INNER JOIN vw_structure s on s.structureid = c.structureid
      WHERE CsCartridge.MoleculeContains(
        s.structure
        , 'select query from cscartridge.tempqueries where id = 0'
        , null
        , p_cartridge_parms
      ) = 1;

    RETURN v_refcur;
  END;

  FUNCTION Compounds_By_Property (
    p_property_name IN varchar2
    , p_property_value IN varchar2
  ) RETURN sys_refcursor
  IS
    v_refcur sys_refcursor;
    v_sql varchar2(1000);
  BEGIN
    BEGIN
      v_sql := 'select c.compoundid from vw_compound c where upper(c.'
      	|| p_property_name
        || ') = upper(:1)';
    END;
    -- create the list of compound IDs
    OPEN v_refcur FOR v_sql USING p_property_value;

    RETURN v_refcur;
  END;

  FUNCTION Compounds_By_Identifier(
    p_identifier_name IN varchar2
    , p_identifier_value IN varchar2
  ) RETURN sys_refcursor
  IS
    v_refcur sys_refcursor;
    v_sql varchar2(2000) :=
      'select c.compoundid
       from vw_compound c
         inner join vw_compound_identifier ci on ci.regid = c.regid
         inner join vw_identifiertype it on it.id = ci.type
       where it.name = :1 and upper(ci.value) = upper(:2)';
  BEGIN
    dbms_output.put_line(p_identifier_name ||' = ' || p_identifier_value);  

    -- create the list of compound IDs
    OPEN v_refcur FOR v_sql USING p_identifier_name, p_identifier_value;

    RETURN v_refcur;
  END;

  FUNCTION Matched_Compounds_Wrapper(
    p_type IN varchar2
    , p_params IN varchar2
    , p_value IN clob
  ) RETURN sys_refcursor
  IS
    v_refcur sys_refcursor;
  BEGIN
      if (p_type = 'P') then
        v_refcur := Compounds_By_Property(p_params, to_char(p_value));
      elsif (p_type = 'I') then
        v_refcur := Compounds_By_Identifier(p_params, to_char(p_value));
      elsif (p_type = 'SP') then
        v_refcur := Compounds_By_StructProperty(p_params, to_char(p_value));
      elsif (p_type = 'SI') then
        v_refcur := Compounds_By_StructIdentifier(p_params, to_char(p_value));
      else
        v_refcur := Compounds_By_Structure(p_params, p_value);
      end if;

    RETURN v_refcur;
  END;

  /**
  Build a comma delimeted string of fragmentids associated with a compound.
  The list is ordered by fragmentid asc
  This list is used for comparing fragmentlists to compute samefragments flag
  in FindComponentDuplicates
  -->author dgb
  -->since Nov 2011
  -->param p_compoundid the compound identifier
  -->return an ordered list of fragmentids
  */ 
  FUNCTION get_fragmentids (p_compoundid  in  integer)
   RETURN VARCHAR2
  IS
     l_text  VARCHAR2(32767) := NULL;
  BEGIN
    FOR cur_rec IN (SELECT cf.fragmentid 
                     FROM vw_compound_fragment cf 
                      WHERE cf.compoundid  = p_compoundid 
                       order by fragmentid asc) LOOP
      l_text := l_text || ',' || cur_rec.fragmentid;
    END LOOP;
    RETURN LTRIM(l_text, ',');
  END;
  
  
  /**
  Determine if a component 'exists', by virtue of a chemical structure or
  custom component properties, in the permanent registry.
  Added generation of SAMEFRAGMENT flag by comparing to incoming fragment list
  -->author jed
  -->since September 2010
  -->param p_type the type of search to perform
  -->param p_params the parameter to use, specific to the type of search to conduct
  -->param p_value the value of the above parameter
  -->param p_fragmentListDuplicates  a list of fragments to compare against to determine 
  -- SAMEFRAGMENT flag
  -->return an xml CLOB giving a list of matches
  
  */
  FUNCTION FindComponentDuplicates(
    p_type IN varchar2
    , p_params IN varchar2
    , p_value IN clob
    , p_fragmentListToCompare IN varchar2
  ) RETURN CLOB IS
    v_message CLOB;
  BEGIN

    IF (p_type = 'S') THEN
      INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (p_value, 0);
    END IF;

    SELECT
      xmlelement( "RegistryList",
        xmlattributes(
          count(distinct mr.regnumber) as "uniqueRegs"
          , count(distinct c.compoundid) as "uniqueComps"
          , count(distinct s.structureid) as "uniqueStructs"
          , case p_type
              when 'S' then 1
              when 'P' then 2
              when 'I' then 3
              when 'SP' then 4
              when 'SI' then 5
              else 0
            end as "mechanism"
        ),
        xmlagg(
          xmlelement( "Registration",
            xmlattributes( m.regid as "id", mr.regnumber as "regNum" ),
            xmlagg(
              xmlelement( "Component",
                xmlattributes(c.compoundid as "id", 
                              r.regnumber as "regNum",
                              get_fragmentids(c.compoundid) as "fragmentlist",   
                              decode(get_fragmentids(c.compoundid), p_fragmentListToCompare, 'True', 'False') as "samefragments"
                              ),      
                xmlelement( "Structure",
                  xmlattributes( s.structureid as "id" )
                )
              )
            )
          )
        )
      ).GetClobVal() INTO v_message
    FROM vw_compound c
      INNER JOIN vw_structure s on s.structureid = c.structureid
      INNER JOIN vw_mixture_component mc on mc.compoundid = c.compoundid
      INNER JOIN vw_mixture m on m.mixtureid = mc.mixtureid
      INNER JOIN vw_mixture_regnumber mr on mr.mixtureid = m.mixtureid
      INNER JOIN vw_registrynumber r on r.regid = c.regid
      INNER JOIN TABLE (
          Extract_Compounds (
            Matched_Compounds_Wrapper(p_type, p_params, p_value)
          )
        ) t on t.compound_id = c.compoundid
    GROUP BY m.regid, mr.regnumber, c.compoundid, s.structureid
    ORDER BY m.regid, mr.regnumber, c.compoundid, s.structureid;

    -- return the results
    RETURN v_message;

  END;
  
  /**
  Determine if a component 'exists', by virtue of a chemical structure or
  custom component properties, in the permanent registry.
  Calls an overloaded version of the function
  -->author jed
  -->since September 2010
  -->param p_type the type of search to perform
  -->param p_params the parameter to use, specific to the type of search to conduct
  -->param p_value the value of the above parameter
  -->return an xml CLOB giving a list of matches
  */
  FUNCTION FindComponentDuplicates(
    p_type IN varchar2
    , p_params IN varchar2
    , p_value IN clob
  ) RETURN CLOB IS
    v_message CLOB;
  BEGIN
    -- return the results
    RETURN FindComponentDuplicates(p_type 
                                   , p_params
                                   , p_value
                                   , ''   -- pass blank fragment list for backwareds compatibility
                                   );
  END; 

BEGIN
  -- Initialization
  NULL;
END RegistryDuplicateCheck;
/
