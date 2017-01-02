-- Add columns to tables:

/* Add a new column LOCKED,ACTIVE,SORTORDER to PICKLISTDOMAIN,PICKLIST*/

alter table &&schemaName..PICKLISTDOMAIN add("LOCKED" NCHAR(1) DEFAULT 'F' );
alter table &&schemaName..PICKLISTDOMAIN add("EXT_SQL_SORTORDER" VARCHAR2(200));
alter table &&schemaName..PICKLIST add(ACTIVE NCHAR(1) DEFAULT 'T' );
alter table &&schemaName..PICKLIST add(SORTORDER NUMBER DEFAULT '1' );
alter table &&schemaName..NOTEBOOKS drop(USER_CODE);
alter table &&schemaName..NOTEBOOKS add(PERSON_ID NUMBER(8,0));
alter table &&schemaName..NOTEBOOKS add(SORTORDER NUMBER DEFAULT '1' );
alter table &&schemaName..SEQUENCE add(SORTORDER NUMBER DEFAULT '1' );


--Add new Views
 
  CREATE OR REPLACE FORCE VIEW  &&schemaName.."VW_SITES" ("ID", "SITECODE", "SITENAME", "ACTIVE") AS 
  SELECT S.SITE_ID, S.SITE_CODE,S.SITE_NAME,DECODE(S.ACTIVE,1,'T',0,'F','F') AS ACTIVE  FROM COEDB.SITES S;

  CREATE OR REPLACE FORCE VIEW &&schemaName.."VW_SITE_PREFIX" ("SEQUENCEID", "PREFIX", "PERSONID", "SITEID", "ACTIVE", "SORTORDER", "TYPE") AS
  SELECT S.SEQUENCE_ID, S.PREFIX,P.PERSON_ID,P.SITE_ID,S.ACTIVE,S.SORTORDER,S.TYPE FROM REGDB."SEQUENCE" S,COEDB.PEOPLE P WHERE P.SITE_ID = S.SITEID AND (P.SITE_ID IS NOT NULL AND   S.SITEID IS NOT NULL)
  UNION
  SELECT S.SEQUENCE_ID, S.PREFIX,null as person_id,S.SITEID,S.ACTIVE,S.SORTORDER,S.TYPE FROM REGDB."SEQUENCE" S WHERE S.SITEID IS NULL 
  UNION
  SELECT S.SEQUENCE_ID,      S.PREFIX,P.PERSON_ID,P.SITE_ID,S.ACTIVE,S.SORTORDER,S.TYPE FROM REGDB."SEQUENCE" S,COEDB.PEOPLE P INNER JOIN REGDB.VW_SITES ST ON (UPPER(ST.SITENAME) = 
  'UNSPECIFIED' or ST.SITECODE= '0')
  WHERE P.SITE_ID = S.SITEID AND P.SITE_ID IS NULL AND S.SITEID IS NULL;
  
 
  CREATE OR REPLACE FORCE VIEW &&schemaName.."VW_PEOPLE" ("PERSONID", "USERID", "SITEID", "ACTIVE") AS 
  SELECT P.PERSON_ID, P.USER_ID,P.SITE_ID,DECODE(P.ACTIVE,1,'T',0,'F','0') AS ACTIVE  FROM COEDB.PEOPLE P;

  CREATE OR REPLACE FORCE VIEW &&schemaName.."VW_NOTEBOOKS" ("NOTEBOOKID", "NAME", "DESCRIPTION", "ACTIVE", "PERSONID","USERCODE","SORTORDER") AS 
  SELECT NOTEBOOK_NUMBER,NOTEBOOK_NAME,DESCRIPTION,N.ACTIVE,N.PERSON_ID,P.USER_CODE,N.SORTORDER FROM NOTEBOOKS N,COEDB.PEOPLE P WHERE  N.PERSON_ID = P.PERSON_ID;

  

-- Add Columns to Views :

begin
  -- Call the procedure
  -- LOCKED
  &&schemaName..configurationcompoundregistry.addfieldtoview('LOCKED', 'LOCKED', 'VW_PICKLISTDOMAIN', 'PICKLISTDOMAIN');

 -- SORTFILTER
  &&schemaName..configurationcompoundregistry.addfieldtoview('EXT_SQL_SORTORDER', 'EXT_SQL_SORTORDER', 'VW_PICKLISTDOMAIN', 'PICKLISTDOMAIN');

  --@ACTIVE
  &&schemaName..configurationcompoundregistry.addfieldtoview('ACTIVE', 'ACTIVE', 'VW_PICKLIST', 'PICKLIST');

  &&schemaName..configurationcompoundregistry.addfieldtoview('SORTORDER', 'SORTORDER', 'VW_PICKLIST', 'PICKLIST');

  &&schemaName..configurationcompoundregistry.addfieldtoview('SORTORDER', 'SORTORDER', 'VW_SEQUENCE', 'SEQUENCE');
 
end;

/ 

declare viewText varchar2(250) ;
  begin
    select TEXT into viewText  from dba_views  where OWNER = 'REGDB'  and VIEW_NAME  = 'VW_UNIT';
    if instr(upper(viewText),'PICKLIST')>0 then
       &&schemaName..configurationcompoundregistry.addfieldtoview('ACTIVE', 'ACTIVE', 'VW_UNIT', 'PICKLIST');
       &&schemaName..configurationcompoundregistry.addfieldtoview('SORTORDER', 'SORTORDER', 'VW_UNIT', 'PICKLIST');
    end if;
end;

/



