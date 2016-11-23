

PROMPT ===> Starting file import_regcompounds_sdf.sql

-- ChemImp Sd file
PROMPT Importing &&regCompoundsSdfName into stagging table &&sdimptable with ChemImp
PROMPT host &&pathToChemImp &&schemaName/&&schemaPass@&&serverName table=&&sdimptable file=&&sdfile scan_feedback=1000 feedback=500 log=&&tmplog
HOST &&pathToChemImp &&schemaName/&&schemaPass@&&serverName table=&&sdimptable file=&&sdfile scan_feedback=1000 feedback=500 log=&&tmplog append=no 

SPOOL off
HOST type &&tmplog >> &&scriptlog
SPOOL &&scriptlog append

PROMPT ===========================================
PROMPT Creating index on &&sdimptable.(StructureID)
CREATE INDEX ix_t_sdimp_strucid ON &&sdimptable.(StructureID);

PROMPT ===========================================
PROMPT Creating Cartridge index on &&sdimptable.(Structure)
create index lx on &&sdimptable(structure)
indextype is &&cartSchemaName..moleculeindextype;

update &&sdimptable set saltid = null, saltequivalents= null where saltid = 'null' or saltid = 'No Salt';

commit;
