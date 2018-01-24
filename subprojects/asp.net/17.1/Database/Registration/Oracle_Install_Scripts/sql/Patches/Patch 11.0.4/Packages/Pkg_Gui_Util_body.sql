prompt 
prompt Starting "pkg_Gui_Util_body.sql"...
prompt 

CREATE OR REPLACE PACKAGE BODY REGDB."GUI_UTIL"
AS


  PROCEDURE GETPROJECTNAMEVALUELIST(O_RS OUT CURSOR_TYPE) AS
  BEGIN
    OPEN O_RS FOR
    Select PROJECTID, NAME from VW_PROJECT;
  END;

  PROCEDURE GETPREFIXNAMEVALUELIST(O_RS OUT CURSOR_TYPE) AS
  BEGIN
    OPEN O_RS FOR
    Select S.SEQUENCEID, NVL( S.PREFIX, 'No Prefix {' || TO_CHAR(S.SEQUENCEID) || '}' ) from VW_SEQUENCE S;
  END;

  PROCEDURE GETACTIVECHEMISTNAMEVALUELIST(O_RS OUT CURSOR_TYPE) AS
  BEGIN
    OPEN O_RS FOR
    Select PERSON_ID, USER_ID from CS_SECURITY.PEOPLE where ACTIVE=1 order by USER_ID;
  END;

  PROCEDURE GETCHEMISTNAMEVALUELIST(O_RS OUT CURSOR_TYPE) AS
  BEGIN
    OPEN O_RS FOR
    Select PERSON_ID, USER_ID from CS_SECURITY.PEOPLE order by USER_ID;
  END;

  PROCEDURE GETFRAGMENTTYPESVALUELIST(O_RS OUT CURSOR_TYPE) AS
  BEGIN
    OPEN O_RS FOR
    SELECT ID,DESCRIPTION from REGDB.VW_FRAGMENTTYPE;
  END;

  PROCEDURE GETPROJECTLIST (O_RS OUT CURSOR_TYPE) AS
  BEGIN
    OPEN O_RS FOR
    SELECT PROJECTID, NAME, ACTIVE, DESCRIPTION, TYPE FROM VW_PROJECT ORDER BY NAME;
  END;

  PROCEDURE GETACTIVEPROJECTLISTBYPERSONID (
    O_RS OUT CURSOR_TYPE
    , APersonID in Number
    , AType IN Char:='A'
  ) AS
  BEGIN
    OPEN O_RS FOR
    SELECT DISTINCT PRO.PROJECTID, PRO.NAME, PRO.ACTIVE, PRO.DESCRIPTION, PRO.TYPE
    FROM REGDB.VW_PROJECT PRO
      LEFT JOIN VW_PEOPLEPROJECT PEO ON  PEO.PROJECTID = PRO.PROJECTID AND PEO.PERSONID = APersonID
    WHERE PRO.ACTIVE = 'T' AND (PRO.TYPE=AType OR AType='A'OR PRO.TYPE='A')
    ORDER BY 2;
  END;

  PROCEDURE GETIDENTIFIERLIST (
    O_RS OUT CURSOR_TYPE
    , AType IN Char:='A'
  ) AS
  BEGIN
    OPEN O_RS FOR
    SELECT ID, NAME, DESCRIPTION, ACTIVE FROM VW_IDENTIFIERTYPE WHERE ACTIVE = 'T' AND (TYPE=AType OR AType='A' OR TYPE='A');
  END;

  PROCEDURE GETFRAGMENTSLIST (O_RS OUT CURSOR_TYPE) AS
  BEGIN
  OPEN O_RS FOR
    SELECT F.FRAGMENTID, F.CODE, F.DESCRIPTION, F.FRAGMENTTYPEID,F.STRUCTURE, F.MOLWEIGHT, F.FORMULA FROM REGDB.VW_FRAGMENT F
    ORDER BY F.DESCRIPTION;
  END;

  PROCEDURE GETFRAGMENTLISTBYIDS(
    AIdList IN FRAGMENT_IDS
    , O_RS OUT CURSOR_TYPE
  ) IS
    v_ids idlist := idlist();
  BEGIN
    v_ids.extend(AIdList.count);
    for i in AIdList.first .. AIdList.last loop
      v_ids(i) := AIdList(i);
    end loop;
  
    open O_RS for
    select
      f.fragmentid, f.code, f.description, f.fragmenttypeid
      , f.structure, f.molweight, f.formula
    from regdb.vw_fragment f
    where f.fragmentid in (
      select column_value from table(v_ids)
    )
    order by f.description;
  END;

  PROCEDURE GETMATCHEDFRAGMENTSLIST (
    AStructure IN CLOB
    , O_RS OUT CURSOR_TYPE
  ) AS
  BEGIN
    open O_RS for
    select
      f.fragmentid, f.code, f.description, f.fragmenttypeid
      , f.structure, f.molweight, f.formula
    from regdb.vw_fragment f
    where CsCARTRIDGE.MoleculeContains(
      f.structure, dbms_lob.substr(AStructure, 32767, 1), null, 'IDENTITY=YES,TAUTOMER=YES'
    ) = 1
    order by f.description;
  END;

  PROCEDURE GETSEQUENCELIST (
    O_RS OUT CURSOR_TYPE
    , ASeqTypeID in Number
  ) AS
  BEGIN
    OPEN O_RS FOR
    SELECT S.SEQUENCEID, S.REGNUMBERLENGTH, S.PREFIX
    FROM VW_SEQUENCE S
    ORDER BY S.PREFIX ASC;
  END;

  --Gets the list of sequences given a personID - Ulises 08/14/09
  PROCEDURE GETSEQUENCELISTBYPERSONID (
    O_RS OUT CURSOR_TYPE
    , ASeqTypeID in Number
    , APersonID in Number
  ) AS
  BEGIN
    OPEN O_RS FOR
    SELECT S.SEQUENCEID, S.REGNUMBERLENGTH, S.PREFIX
    FROM VW_SEQUENCE S
      INNER JOIN PREFIX_USER PF ON S.SEQUENCEID = PF.SEQUENCE_ID
    WHERE PF.PERSON_ID = APersonID ORDER BY S.PREFIX ASC;
  END;

  --Gets the number of perm registries to be shown in the Reg home dashboard
  --Ulises - 02/26/2009
  PROCEDURE GETTEMPREGISTRIESCOUNT (ACount OUT NOCOPY Number) AS
  BEGIN
    SELECT count(*) INTO ACount FROM VW_TEMPORARYBATCH;
  END;

  --Gets the number of perm registries to be shown in the Reg home dashboard
  --Ulises - 02/26/2009
  PROCEDURE GETPERMREGISTRIESCOUNT (ACount out Number) AS
  BEGIN
    SELECT count(*) INTO ACount FROM VW_MIXTURE;
  END;

  PROCEDURE GETAPPROVEDREGISTRIESCOUNT (ACount out Number) AS
  BEGIN
    SELECT count(*) INTO ACount FROM VW_MIXTURE WHERE APPROVED='Approved';
  END;

  --Get the number of compound duplicated
  --Fari - 10/09/2010
  PROCEDURE GETDUPLICATEDCOMPOUNDCOUNT (ACount out Number) AS
  BEGIN
    SELECT count(1) INTO ACount
    FROM (
      SELECT RegNumber FROM VW_Duplicates
      UNION
      SELECT RegNumberDuplicated FROM VW_Duplicates
    );
  END GETDUPLICATEDCOMPOUNDCOUNT;

END "GUI_UTIL";
/
