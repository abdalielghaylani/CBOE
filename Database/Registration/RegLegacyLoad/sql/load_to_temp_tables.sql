PROMPT ====>  Starting file load_to_temp_tables.sql


PROMPT Deleting existing temporary records...
delete from regdb.vw_temporarycompound;
delete from regdb.vw_temporarybatch;

PROMPT
PROMPT ======================================================
PROMPT Transfering first batches from &&sdimptable to VW_TEMPORARYBATCH...
insert into VW_TEMPORARYBATCH (TEMPBATCHID, BATCHNUMBER, STRUCTUREAGGREGATION, BATCH_FORMULA, FORMULA_WEIGHT, PERCENT_ACTIVE, PERSONCREATED, DATECREATED, DATELASTMODIFIED, PROJECTXMLBATCH, IDENTIFIERXMLBATCH, PROJECTXML, IDENTIFIERXML, SEQUENCEID, SCIENTIST_ID, CREATION_DATE, BATCH_COMMENT)
select SEQ_TEMPORARY_BATCH.NextVal,
       0 BATCHNUMBER,
       &&cartSchemaName..convertcdx.cdxtob64(lc.structure) STRUCTUREAGGREGATION,
       ci.formula || nvl2(lc.saltequivalents, chr(149)||trim(lc.saltequivalents)||trim(f.formula),'') BATCH_FORMULA,
	     ci.molweight + nvl2(lc.saltequivalents, (lc.saltequivalents * f.molweight),0) FORMULA_WEIGHT,
	   nvl2(lc.saltequivalents, (100*ci.molweight)/(ci.molweight + lc.saltequivalents * f.molweight),100) percent_active,        
     p.person_id PERSONCREATED,
	   to_date(substr(lc.datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') DATECREATED,
	   to_date(substr(lc.datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') DATELASTMODIFIED,
	   '<ProjectList/>' PROJECTXMLBATCH,
	   '<IdentifierList/>' IDENTIFIERXMLBATCH,
	    '<ProjectList/>' PROJECTXML,
	   '<IdentifierList/>' IDENTIFIERXML,
       1 SEQUENCEID,
       p.person_id SCIENTIST_ID,
       to_date(substr(lc.datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') CREATION_DATE,
       lc.legacyregnumber BATCH_COMMENT
from &&sdimptable lc, &&cartSchemaName..regdb_lx ci, &&securitySchemaName..people p, &&fragmentssdimptable f
where lc.rowid = ci.rid
and lc.saltid= f.fragmentid(+)
and trim(p.user_id) = upper(trim(lc.personcreated))
and to_number(substr(batch,2,3)) =1 ;

PROMPT
PROMPT =========================================================
PROMPT Transfering first batches from &&sdimptable to  VW_TEMPORARYCOMPOUND...
insert into VW_TEMPORARYCOMPOUND (TEMPCOMPOUNDID, TEMPBATCHID, STRUCTUREID, BASE64_CDX, PERSONCREATED, DATECREATED, DATELASTMODIFIED, NORMALIZEDSTRUCTURE, PROJECTXML, IDENTIFIERXML, FRAGMENTXML, BATCHCOMPFRAGMENTXML, SEQUENCEID, USENORMALIZATION, CHEM_NAME_AUTOGEN, CMP_COMMENTS)
select SEQ_TEMPORARY_COMPOUND.NextVal,
	   b.TEMPBATCHID,
	   0 STRUCTUREID,
       &&cartSchemaName..convertcdx.cdxtob64(lc.structure) BASE64_CDX,
	   p.person_id PERSONCREATED,
	   to_date(substr(lc.datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') DATECREATED,
	   to_date(substr(lc.datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') DATELASTMODIFIED,
	   &&cartSchemaName..convertcdx.cdxtob64(lc.structure) NORMALIZEDSTRUCTURE,
	   '<ProjectList/>' PROJECTXML,
	   '<IdentifierList/>' IDENTIFIERXML,	   
     NVL2(lc.saltid ,'<FragmentList><Fragment><CompoundFragmentID>0</CompoundFragmentID><FragmentID>'||TRIM(lc.saltid)||'</FragmentID></Fragment></FragmentList>',
                     '<FragmentList/>') FRAGMENTXML, 
     NVL2(lc.saltid, '<BatchComponentFragmentList><BatchComponentFragment><ID>0</ID><FragmentID>'||TRIM(lc.saltid)||'</FragmentID><Equivalents>'||TRIM(lc.saltequivalents)||'</Equivalents><OrderIndex>1</OrderIndex></BatchComponentFragment></BatchComponentFragmentList>', 
                     '<BatchComponentFragmentList/>') BATCHCOMPFRAGMENTXML,
       2 SEQUENCEID,
       'F' USENORMALIZATION,
       &&cartSchemaName..convertcdx.cdxtoname(lc.structure, '') CHEM_NAME_AUTOGEN,
       lc.legacyregnumber CMP_COMMENTS
from &&sdimptable lc, &&cartSchemaName..regdb_lx ci, &&securitySchemaName..people p, VW_TEMPORARYBATCH b
where lc.rowid = ci.rid
and trim(p.user_id) = upper(trim(lc.personcreated))
and b.batch_comment = lc.legacyRegNumber
and to_number(substr(batch,2,3)) =1 ;

commit;








