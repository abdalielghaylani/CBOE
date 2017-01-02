

PROMPT ===> Starting file import_fragments_sdf.sql

-- ChemImp Sd file
PROMPT Importing fragmentssdfile  into stagging table &&fragmentssdimptable  with ChemImp
PROMPT host &&pathToChemImp &&schemaName/&&schemaPass@&&serverName table=&&fragmentssdimptable  file=&&fragmentssdfile  scan_feedback=10 feedback=50 log=&&tmplog
HOST &&pathToChemImp &&schemaName/&&schemaPass@&&serverName table=&&fragmentssdimptable  file=&&fragmentssdfile  scan_feedback=10 feedback=50 log=&&tmplog append=no 

SPOOL off
HOST type &&tmplog >> &&scriptlog
SPOOL &&scriptlog append

PROMPT ===========================================
PROMPT Creating index on &&fragmentssdimptable.(FragmentID)
CREATE INDEX ix_t_fragsdimp_fragid ON &&fragmentssdimptable.(FragmentID);

--PROMPT ===========================================
--PROMPT Creating Cartridge index on &&fragmentssdimptable .(Structure)
--create index fx on &&fragmentssdimptable (structure)
--indextype is &&cartSchemaName..moleculeindextype;


--truncate table regdb.vw_fragment;

PROMPT ===========================================
PROMPT Importing fragments into vw_fragments view....
insert into vw_fragment (fragmentid, code, description, fragmenttypeid, molweight, formula, created, modified, structure, structureformat)
(select fragmentid fragmentid,
       fragmentcode code, 
       description description, 
       fragmenttypeid fragmenttypeid, 
       molweight molweight, 
       formula formula, 
       to_date(substr(datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') created, 
       to_date(substr(datelastmodified,1,19),'YYYY-MM-DD HH24:MI:SS') modified, 
       &&cartSchemaName..convertcdx.cdxtob64(structure) structure, 
       null structureformat 
from       &&fragmentssdimptable);

commit;


              
