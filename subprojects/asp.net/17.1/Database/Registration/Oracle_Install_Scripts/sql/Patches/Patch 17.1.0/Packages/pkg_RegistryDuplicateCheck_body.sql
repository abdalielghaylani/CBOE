prompt 
prompt Starting "Pkg_RegistryDuplicateCheck_body.sql"...
prompt

create or replace PACKAGE BODY RegistryDuplicateCheck IS

  -- Private type declarations
  -- Private constant declarations
  -- Private variable declarations

  /** System settings xml doc for Registration */
  vSystemSettings XMLTYPE;
  /** System setting: true if allow unregistered Compounds in Mixtures */
  vAllowUnregistCompInMix VARCHAR2(200);

  -- Forward declarations: procedures
  PROCEDURE TraceWrite(p_methodName Varchar2, p_lineNumber Integer, p_comment CLOB);

  -- Function and procedure implementations

  /**
  Retrieves the xml metadata describing the Registration system settings.
  -->author fari
  -->since September 2011
  -->return an xml document (of xmltype)
  */
  FUNCTION GetSystemSettings RETURN XMLTYPE IS
  BEGIN
    IF (VSystemSettings IS NULL) THEN
      SELECT c.configurationxml INTO VSystemSettings
      FROM coedb.coeconfiguration c
      WHERE UPPER(c.description) = 'REGISTRATION';
    END IF;
    RETURN VSystemSettings;
  END;

  /**
  Get the value of "AllowUnregisteredComponentsInMixtures" from the "Application Settings" of Registration.
  "AllowUnregisteredComponentsInMixtures" setting enables validation of Unregistered Components In Mixtures
  -->author fari
  -->since September 2011
  -->return an xml document (of xmltype)
  */
  FUNCTION GetAllowUnregistCompInMix RETURN VARCHAR2 IS
    LValue CLOB;
    LSettings XMLTYPE;
  BEGIN
    IF (VAllowUnregistCompInMix is NULL) THEN
      LSettings := GetSystemSettings;
      SELECT Extract(
        LSettings,
        'Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="AllowUnregisteredComponentsInMixtures"]/@value'
      ).GetStringVal() INTO LValue
      FROM DUAl;
      VAllowUnregistCompInMix := to_char(LValue);
    END IF;
    RETURN VAllowUnregistCompInMix;
  END;

  /**
  Selects compound IDs into a cursor based on the intrinsic chemical properties
  of its structure.
  -->author jed
  -->since January 2011
  -->return a sys_refcursor containing a list of matching components IDs
  */
  FUNCTION Compounds_By_Structure(
    p_cartridge_parms varchar2
    , p_structure IN vw_structure.structure%TYPE
  ) RETURN number
  IS
    l_id number;
  BEGIN

    vAllowUnregistCompInMix:=GetAllowUnregistCompInMix;

    -- Cannot perform DML inside a select query
    --INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (p_structure, 0);
     select sq_duplicate_check.nextval into l_id from dual;
    -- create the list of compound IDs
    IF UPPER(vAllowUnregistCompInMix)='TRUE' THEN
        -- get all compound IDs duplicated
        insert into duplicate_check(id, compound_id ,regid ,structureid )
        select  l_id, c.compoundid,c.regid ,c.structureid
          FROM vw_compound c
            INNER JOIN vw_structure s on s.structureid = c.structureid
          WHERE CsCartridge.MoleculeContains(
            s.structure
            , 'select query from cscartridge.tempqueries where id = 0'
            , null
            , p_cartridge_parms
          ) = 1;
    ELSE
        -- get only the compound IDs duplicated of Single registry
        insert into duplicate_check(id, compound_id ,regid ,structureid )
        select distinct l_id, compoundid, regid ,structureid  from (
         select  c.compoundid,c.regid ,c.structureid, count(1) over (partition by MC3.MIXTUREID) as cnt
			      	 FROM VW_MIXTURE_COMPONENT MC
               inner join VW_MIXTURE_COMPONENT MC3 on MC.MIXTUREID = MC3.MIXTUREID
			         INNER JOIN VW_COMPOUND C ON MC.COMPOUNDID = C.COMPOUNDID
			         INNER JOIN VW_STRUCTURE S ON S.STRUCTUREID = C.STRUCTUREID
			         WHERE CSCARTRIDGE.MOLECULECONTAINS
			         (S.STRUCTURE, 'select query from cscartridge.tempqueries where id = 0', NULL, p_cartridge_parms) = 1
			     ) where cnt = 1;
      /*  select  l_id, MC3.compoundid,c.regid ,c.structureid, MC3.COMPOUNDID
			FROM VW_MIXTURE_COMPONENT MC3, Vw_Compound c
			WHERE MC3.MIXTUREID IN
				(SELECT MC2.MIXTUREID
				 FROM VW_MIXTURE_COMPONENT MC2
			     WHERE MC2.MIXTUREID IN
			     	(SELECT MC.MIXTUREID
			      	 FROM VW_MIXTURE_COMPONENT MC
			         INNER JOIN VW_COMPOUND C ON MC.COMPOUNDID = C.COMPOUNDID
			         INNER JOIN VW_STRUCTURE S ON S.STRUCTUREID = C.STRUCTUREID
			         WHERE CSCARTRIDGE.MOLECULECONTAINS
			         (S.STRUCTURE, 'select query from cscartridge.tempqueries where id = 0', NULL, p_cartridge_parms) = 1)
			     HAVING COUNT (1) = 1
			     GROUP BY MC2.MIXTUREID);*/
    END IF;

    RETURN l_id;
  END;

  /**
  Selects compound IDs into a cursor based on an identifier owned by its structure.
  -->author jed
  -->since January 2011
  -->param p_identifier_name the identifier type to filter by
  -->param p_identifier_value the identifier value to filter by
  -->return a sys_refcursor containing a list of matching components IDs
  */
    FUNCTION Compounds_By_StructIdentifier(
    p_identifier_name IN varchar2
    , p_identifier_value IN varchar2
  ) RETURN number
  IS
    l_id number;
    v_sql varchar2(2000) :=
        'insert into duplicate_check(id, compound_id ,regid ,structureid )
        select distinct :l_id, c.compoundid,c.regid ,c.structureid
        from vw_compound c
       inner join vw_structure s on s.structureid = c.structureid and s.drawingtype !=0
       inner join vw_structure_identifier si on si.structureid = s.structureid
       inner join vw_identifiertype it on it.id = si.type
     where it.name = :1 and upper(si.value) = upper(:2)';

  BEGIN
    -- create the list of compound IDs
    select sq_duplicate_check.nextval into l_id from dual;
    execute immediate v_sql USING l_id, p_identifier_name, p_identifier_value;

    RETURN l_id;
  END;

  /**
  Selects compound IDs into a cursor based on a property of its structure.
  -->author jed
  -->since January 2011
  -->param p_property_name the column to filter by
  -->param p_value the value to filter by
  -->return a sys_refcursor containing a list of matching components IDs
  */
  FUNCTION Compounds_By_StructProperty (
    p_property_name IN varchar2
    , p_property_value IN varchar2
  ) RETURN number
  IS
--    v_refcur sys_refcursor;
    l_id number;
    v_sql varchar2(1000);
  BEGIN
    BEGIN
      select sq_duplicate_check.nextval into l_id from dual;
      v_sql :=
        'insert into duplicate_check(id, compound_id ,regid ,structureid )
        select :l_id, c.compoundid,c.regid ,c.structureid       from vw_compound c
         inner join vw_structure s on s.structureid = c.structureid and s.drawingtype !=0
       where upper(s.'
      	|| p_property_name
        || ') = upper(:1)';
    END;
    -- create the list of compound IDs
    execute immediate v_sql USING l_id, p_property_value;

    RETURN l_id;
  END;

  /**
  Selects compound IDs into a cursor based on an identifier owned by the compound.
  -->author jed
  -->since January 2011
  -->param p_identifier_name the identifier type to filter by
  -->param p_identifier_value the identifier value to filter by
  -->return a sys_refcursor containing a list of matching components IDs
  */
  FUNCTION Compounds_By_Identifier(
    p_identifier_name IN varchar2
    , p_identifier_value IN varchar2
  ) RETURN number
  IS
  --  v_refcur sys_refcursor;
  l_id number;
    v_sql varchar2(2000) :=
      'insert into duplicate_check(id,compound_id ,regid ,structureid )
        select :l_id,c.compoundid,c.regid ,c.structureid
       from vw_compound c
         inner join vw_compound_identifier ci on ci.regid = c.regid
         inner join vw_identifiertype it on it.id = ci.type
         inner join vw_structure s on s.structureid = c.structureid and s.drawingtype !=0
       where it.name = :1 and upper(ci.value) = upper(:2)';
  BEGIN
    dbms_output.put_line(p_identifier_name ||' = ' || p_identifier_value);
    select sq_duplicate_check.nextval into l_id from dual;
    -- create the list of compound IDs
    execute immediate v_sql USING l_id, p_identifier_name, p_identifier_value;

    RETURN l_id;
  END;

  /**
  Selects compound IDs into a cursor based on a compound's property.
  -->author jed
  -->since January 2011
  -->param p_property_name the column to filter by
  -->param p_value the value to filter by
  -->return a sys_refcursor containing a list of matching components IDs
  */FUNCTION Compounds_By_Property (
    p_property_name IN varchar2
    , p_property_value IN varchar2
  ) RETURN number
  IS
 --   v_refcur sys_refcursor;
    l_id number;
    v_sql varchar2(1000);
  BEGIN
    BEGIN
      v_sql :=
        'insert into duplicate_check(id, compound_id ,regid ,structureid )
        select :l_id, c.compoundid,c.regid ,c.structureid
         from vw_compound c
           inner join vw_structure s on s.structureid = c.structureid and s.drawingtype !=0
         where upper(c.'
      	|| p_property_name
        || ') = upper(:1)';
    END;
    -- create the list of compound IDs
    select sq_duplicate_check.nextval into l_id from dual;
    execute immediate v_sql USING p_property_value;

    RETURN l_id;
  END;

  /**
  Converts cursor data into a table object and simultaneously pipelines the
  rows back to the caller.
  -->author jed
  -->since January 2011
  -->param p_cursor a sys_refcursor containing the data to be pipelined
  -->return a pipelined table object
  */
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

  /**
  Executes one of several functions that will return a sys_refcursor; the fucntion chosen
  is based on the 'type' parameter.
  -->author jed
  -->since September 2010
  -->param p_type the type of search to perform
  -->param p_params the parameter to use, specific to the type of search to conduct
  -->param p_value the value of the above parameter
  -->return sys_refcursor containing a list of compound IDs, via the executed function
  */
  function Matched_Compounds_Wrapper(
    p_type IN varchar2
    , p_params IN varchar2
    , p_value IN clob
  ) return number
  IS
    v_r number;
  BEGIN
      if (p_type = 'P') then
        v_r := Compounds_By_Property(p_params, to_char(p_value));
      elsif (p_type = 'I') then
        v_r := Compounds_By_Identifier(p_params, to_char(p_value));
      elsif (p_type = 'SP') then
        v_r := Compounds_By_StructProperty(p_params, to_char(p_value));
      elsif (p_type = 'SI') then
        v_r := Compounds_By_StructIdentifier(p_params, to_char(p_value));
      else
        v_r := Compounds_By_Structure(p_params, p_value);
      end if;

   return v_r;
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
    TraceWrite('get_fragmentids_BEGIN', $$plsql_line, 'p_compoundid: ' || p_compoundid);
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
    , p_fragmentListToCompare IN varchar2:='False'
  ) RETURN CLOB IS
    v_message CLOB;
    l_id number;
  BEGIN
    TraceWrite('FindComponentDuplicates_BEGIN', $$plsql_line, 'p_type: ' || p_type);
    TraceWrite('FindComponentDuplicates_BEGIN', $$plsql_line, 'p_params: ' || p_params);
    TraceWrite('FindComponentDuplicates_BEGIN', $$plsql_line, 'p_value: ' || p_value);
    TraceWrite('FindComponentDuplicates_BEGIN', $$plsql_line, 'p_fragmentListToCompare: ' || p_fragmentListToCompare);

    IF (p_type = 'S') THEN
      INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (p_value, 0);
    END IF;
    l_id := REGISTRYDUPLICATECHECK.MATCHED_COMPOUNDS_WRAPPER (p_type, p_params, p_value);
    SELECT /*+1 OPT_ESTIMATE(table, t, scale_rows=0.0001) */
      xmlelement( "RegistryList",
        xmlattributes(
          count(distinct mr.regnumber) as "uniqueRegs"
          , count(distinct mc.compoundid) as "uniqueComps"
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
            xmlattributes( mr.regid as "id", mr.regnumber as "regNum" ),
            xmlagg(
              xmlelement( "Component",
                xmlattributes(mc.compoundid as "id",
                          r.regnumber as "regNum",
                          decode(p_fragmentListToCompare,'False',null,get_fragmentids(mc.compoundid)) as "fragmentlist",
                          decode(p_fragmentListToCompare,'False',null,decode(get_fragmentids(mc.compoundid), p_fragmentListToCompare, 'True', 'False')) as "samefragments"
                          ),
                xmlelement( "Structure",
                  xmlattributes( s.structureid as "id" )
                )
              )
            )
          )
        )
      ).GetClobVal() INTO v_message
	FROM
--	 VW_COMPOUND C
--	 INNER JOIN
( SELECT /*+ OPT_ESTIMATE(table, t, rows=3000) */ MC2.MIXTUREID, MC2.COMPOUNDID,t.regid ,t.structureid FROM
	  VW_MIXTURE_COMPONENT MC2 INNER JOIN (select d.* from duplicate_check d where d.id = l_id) t ON
	  T.COMPOUND_ID = MC2.COMPOUNDID)  MC
   INNER JOIN VW_STRUCTURE S ON S.STRUCTUREID = MC.STRUCTUREID
	  INNER
	  JOIN VW_MIXTURE_REGNUMBER MR ON MR.MIXTUREID = MC.MIXTUREID
	  INNER JOIN
	  VW_REGISTRYNUMBER R ON R.REGID = MC.REGID
	  GROUP BY MR.REGID, MR.REGNUMBER, MC.COMPOUNDID, S.STRUCTUREID
	  ORDER BY MR.REGID, MR.REGNUMBER, MC.COMPOUNDID, S.STRUCTUREID;

    -- return the results
    RETURN v_message;

  END;

  /**
  Determine if a component 'exists', by virtue of a chemical structure or
  custom component properties, in the permanent registry. Bypasses Mixtures,
  finds only unique components.
  -->author jed
  -->since July 2011
  -->param p_type the type of search to perform
  -->param p_params the parameter to use, specific to the type of search to conduct
  -->param p_value the value of the above parameter
  -->return an xml CLOB giving a list of matches
  */
  FUNCTION FindComponentMatches (
    p_type IN varchar2
    , p_params IN varchar2
    , p_value IN clob
  )  RETURN CLOB IS
    v_message CLOB;
    l_id number;
  BEGIN

    TraceWrite('FindComponentMatches_BEGIN', $$plsql_line, 'p_type: ' || p_type);
    TraceWrite('FindComponentMatches_BEGIN', $$plsql_line, 'p_params: ' || p_params);
    TraceWrite('FindComponentMatches_BEGIN', $$plsql_line, 'p_value: ' || p_value);

    IF (p_type = 'S') THEN
      INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (p_value, 0);
    END IF;

    vAllowUnregistCompInMix:=GetAllowUnregistCompInMix;
    l_id := Matched_Compounds_Wrapper(p_type, p_params, p_value);
    IF UPPER(vAllowUnregistCompInMix)='TRUE' THEN
        SELECT /*+ OPT_ESTIMATE(table, c, rows=3000) */
          xmlelement( "matches",
            xmlattributes(
              count(distinct c.compoundid) as "uniqueComps"
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
              xmlelement( "match",
                xmlattributes(
                  m.regid as "mixtureRegId"
                  , mrn.regnumber as "mixtureRegNum"
                  , c.compoundid as "compoundRegId"
                  , crn.regnumber as "compoundRegNum"
                  , s.structureid as "structureId"
                )
              )
            )
          ).GetClobVal()
        INTO v_message
        FROM 
        --vw_compound c
        (select /*+ OPT_ESTIMATE(table, d, rows=3000) */ d.compound_id as compoundid, d.regid ,d.structureid from duplicate_check d where d.id = l_id) c
          INNER JOIN vw_structure s on s.structureid = c.structureid
          INNER JOIN vw_mixture_component mc on mc.compoundid = c.compoundid
          INNER JOIN vw_mixture m on m.mixtureid = mc.mixtureid
          INNER JOIN vw_mixture_regnumber mrn on mrn.mixtureid = m.mixtureid
          INNER JOIN vw_registrynumber crn on crn.regid = c.regid
        ;
    ELSE
        SELECT /*+ OPT_ESTIMATE(table, c, rows=3000) */
          xmlelement( "matches",
            xmlattributes(
              count(distinct c.compoundid) as "uniqueComps"
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
              xmlelement( "match",
                xmlattributes(
                  m.regid as "mixtureRegId"
                  , mrn.regnumber as "mixtureRegNum"
                  , c.compoundid as "compoundRegId"
                  , crn.regnumber as "compoundRegNum"
                  , s.structureid as "structureId"
                )
              )
            )
          ).GetClobVal()
        INTO v_message
        FROM 
        --vw_compound c
        (select /*+ OPT_ESTIMATE(table, d, rows=3000) */ d.compound_id as compoundid, d.regid ,d.structureid  from duplicate_check d where d.id = l_id) c
          INNER JOIN vw_structure s on s.structureid = c.structureid
          INNER JOIN vw_mixture_component mc on mc.compoundid = c.compoundid
          INNER JOIN (SELECT mixtureid FROM vw_mixture_component HAVING COUNT(1)=1 GROUP BY mixtureid) mc2 on mc2.mixtureid=mc.mixtureid
          INNER JOIN vw_mixture m on m.mixtureid = mc.mixtureid
          INNER JOIN vw_mixture_regnumber mrn on mrn.mixtureid = m.mixtureid
          INNER JOIN vw_registrynumber crn on crn.regid = c.regid;
    END IF;

    TraceWrite('FindComponentMatches_END_Return', $$plsql_line, v_message);

    RETURN v_message;

  END;
/*
FUNCTION FindComponentDuplicates(
    p_type IN varchar2
    , p_params IN varchar2
    , p_value IN clob
  ) RETURN CLOB IS
    v_message CLOB;
  BEGIN

    IF (p_type = 'S') THEN
      INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (p_value, 0);
    END IF;

    vAllowUnregistCompInMix:=GetAllowUnregistCompInMix;

    IF UPPER(vAllowUnregistCompInMix)='TRUE' THEN
        SELECT /*+ OPT_ESTIMATE(table, t, scale_rows=0.0001)
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
                    xmlattributes( c.compoundid as "id", r.regnumber as "regNum" ),
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
    ELSE
        SELECT /*+ OPT_ESTIMATE(table, t, scale_rows=0.0001)
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
                    xmlattributes( c.compoundid as "id", r.regnumber as "regNum" ),
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
          INNER JOIN (SELECT mixtureid FROM vw_mixture_component HAVING COUNT(1)=1 GROUP BY mixtureid) mc2 on mc2.mixtureid=mc.mixtureid
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
    END IF;
    -- return the results
    RETURN v_message;

  END;
*/
  /**
  Compares the incoming list of compound IDs to the ID list from all
  existing mixtures. The target list must have 100% overlap in order to be
  considered a 'match'.
  -- test script: replace the component id values as necessary
  declare
    -- Non-scalar parameters require additional processing
    p_componentids idlist;
  begin
    p_componentids := idlist();
    p_componentids.extend(2);
    p_componentids(1) := 22;
    p_componentids(2) := 44;
    -- Call the function
    :result := REGISTRYDUPLICATECHECK.findmixturematches(p_componentids => p_componentids);
  end;
  -->author jed
  -->since July 2011
  -->param p_componentIds the set of component ids to match
  -->return an xml CLOB giving a list of matches
  */
  FUNCTION FindMixtureMatches (
    p_compound_ids IN IDLIST
  )  RETURN CLOB IS
    ids idlist := p_compound_ids;
    mixids idlist := idlist();
    v_loop number := 0;
    v_cnt number;
    v_message CLOB;
    cursor c_mixtures is
      select mc.mixtureid
      from vw_mixture_component mc
        inner join vw_compound c on c.compoundid = mc.compoundid
      having count(mc.mixtureid) > 1
      group by mc.mixtureid;
  BEGIN

    TraceWrite('FindMixtureMatches_BEGIN', $$plsql_line, 'IDLIST');

    FOR mix IN c_mixtures LOOP
      v_loop := v_loop + 1;

      -- What's the ID of the Mixture our sample set is being compared to?
      TraceWrite('FindMixtureMatches_MixtureLoop', $$plsql_line, 'MixtureID: ' || to_char(mix.mixtureid));

      -- What is the count of the *differences* between the two sets?
      select count(*) into v_cnt
      from (
        (
          select c.compoundid from vw_compound c
            where c.compoundid in ( select column_value from table(ids) )
        MINUS
          select mc.compoundid from vw_mixture_component mc
            where mc.mixtureid = mix.mixtureid
        )
        union all
        (
          select mc.compoundid from vw_mixture_component mc
            where mc.mixtureid = mix.mixtureid
        MINUS
          select c.compoundid from vw_compound c
            where c.compoundid in ( select column_value from table(ids) )
        )
      );

      TraceWrite('FindMixtureMatches_MixtureCompoundCount', $$plsql_line, 'Count: ' || to_char(v_cnt));

      --If the lists are EQUAL, queue the mixture ID for ourput
      if (v_cnt = 0) then
        mixids.extend(v_loop);
        mixids(v_loop) := mix.mixtureid;
      end if;

    END LOOP;

    -- Generate the 'match' output
    SELECT
      xmlelement( "matches",
        xmlattributes(
          count(distinct m.mixtureid) as "uniqueMixtures"
          , 6 as "mechanism"
        ),
        xmlagg(
          xmlelement( "match",
            xmlattributes(
              m.regid as "mixtureRegId"
              , rnm.regnumber as "mixtureRegNum"
            ),
            xmlagg(
              xmlelement( "compound",
                xmlattributes(
                  c.compoundid as "compoundRegId"
                  , rnc.regnumber as "compoundRegNum"
                  , s.structureid as "structureId"
                )
              )
            )
          )
        )
      ).GetClobVal()
    INTO v_message
    FROM vw_mixture m
      inner join vw_registrynumber rnm on rnm.regid = m.regid
      inner join vw_mixture_component mc on mc.mixtureid = m.mixtureid
      inner join vw_compound c on c.compoundid = mc.compoundid
      inner join vw_registrynumber rnc on rnc.regid = c.regid
      inner join vw_structure s on s.structureid = c.structureid
    WHERE m.mixtureid in (
      select column_value from TABLE(mixids)
    )
    GROUP BY m.mixtureid, m.regid, rnm.regnumber
    ORDER BY m.mixtureid, m.regid, rnm.regnumber;

    RETURN v_message;
  END;

  FUNCTION FindMixtureMatches (
    p_compound_ids IN Compound_Ids
  ) RETURN CLOB IS
    v_ids idlist := idlist();
  BEGIN
    v_ids.extend(p_compound_ids.count);
    for i in p_compound_ids.first .. p_compound_ids.last loop
      v_ids(i) := p_compound_ids(i);
    end loop;

    RETURN FindMixtureMatches(v_ids);

  END;

  PROCEDURE TraceWrite(p_methodName Varchar2, p_lineNumber Integer, p_comment CLOB )
  IS
    PRAGMA AUTONOMOUS_TRANSACTION;
  BEGIN
    $if (REGISTRYDUPLICATECHECK.TraceEnabled) $then
      --dbms_output.put_line(to_char(p_lineNumber) || ': ' || p_comment);
      insert into log(
        logprocedure,
        logcomment
      )
      values (
        $$plsql_unit || '.' || p_methodName || '.Line -> ' || to_char(p_lineNumber)
        , p_comment
      );
      COMMIT;
    $end NULL;
  END;

BEGIN
-- Package initialization
  NULL;
END RegistryDuplicateCheck;
/
