PROMPT ===>  Starting file register_from_temp_tables.sql

PROMPT Updating the Registration Sequences
update regdb.vw_sequence
set prefix ='&&regPrefix', batchdelimiter = '&&regBatchDelimeter',prefixdelimiter = '&&regPrefixDelimeter', EXAMPLE = '&&regPrefix.&&regPrefixDelimeter.000001&&regBatchDelimeter.1' where sequenceid=1;
commit;

PROMPT Truncating bulk import log...
delete from regdb.log_bulkregistration;
delete from regdb.log_bulkregistration_id;

PROMPT Registering first batches...
PROMPT To monitor progress query:  
PROMPT select count(*) RecordsProcessed from regdb.log_bulkregistration 
PROMPT

SET serveroutput on
DECLARE
       currHitList NUMBER;
       idList regdb.compoundregistry.tNumericIdList;
       chunkSize NUMBER:=10;
       counter NUMBER:=0;
       chunkNumber NUMBER;
       v_alogid NUMBER;
       v_amessage CLOB;
       rindex pls_integer := -1;
       slno   pls_integer;
       recordsRegistered NUMBER;
       recordsLeftInTemp NUMBER;
       recordsProcessed NUMBER;
BEGIN     
       FOR r in (select tempbatchid from regdb.temporary_batch order by tempbatchid)
       LOOP
          idList(counter) := r.tempbatchid;
          counter := counter + 1;
        -- Register a block of chunkSize records   
        if remainder(counter,chunkSize) = 0  then
           chunkNumber := chunkNumber + 1;
           compoundregistry.converttempregrecordstoperm(atempids => idList,
                                                        aduplicateaction => 'N',
                                                        adescription => 'Legacy',
                                                        auserid => 'REGDB',
                                                        alogid => v_alogid,
                                                        amessage => v_amessage,
                                                        aregistration => 'Y',
                                                        aregnumgeneration => 'Y',
                                                        aconfigurationid => 1,
                                                        asectionslist => NULL);
           /*
           dbms_application_info.set_session_longops(
                                                       RINDEX      => rindex
                                                      ,SLNO        => slno
                                                      ,OP_NAME     => 'LEGACYLOAD'
                                                      ,SOFAR       => chunkNumber*chunkSize
                                                      ,TOTALWORK   => 100
                                                     );*/

           counter := 0;
           idList.DELETE;                                              
         end if;
        END LOOP;
        -- Register the remainder records
        if counter > 0 then        
           compoundregistry.converttempregrecordstoperm(atempids => idList,
                                                        aduplicateaction => 'N',
                                                        adescription => 'Legacy',
                                                        auserid => 'REGDB',
                                                        alogid => v_alogid,
                                                        amessage => v_amessage,
                                                        aregistration => 'Y',
                                                        aregnumgeneration => 'Y',
                                                        aconfigurationid => 1,
                                                        asectionslist => NULL); 
        end if;
        recordsProcessed := chunkNumber*chunkSize  + counter;
        dbms_output.put_line('Finished loading first batches.'); 
        dbms_output.put_line(recordsProcessed || ' records processed.'); 
        select count(*) into recordsRegistered from log_bulkregistration where reg_number is not null;
        select count(*) into recordsLeftInTemp from log_bulkregistration where temp_id is not null;
        dbms_output.put_line(recordsRegistered || ' Records were registered.');
        dbms_output.put_line(recordsLeftInTemp || ' Records were left in temp for review.');
END;
/

PROMPT
PROMPT Updating the Registry numbers to Legacy numbers....
update regdb.vw_registrynumber r
       set (r.regnumber ,r.sequencenumber)= 
       (select substr(r.regnumber,1,length('&&regPrefix'))||'&&regPrefixDelimeter'||lpad(b.BATCH_COMMENT, 6, '0') regnumber, 
               to_number(lpad(b.BATCH_COMMENT, 6, '0')) sequencenumber 
        from  regdb.vw_batch b
        where b.regid = r.regid
        and b.BATCH_COMMENT is not null
        and b.batchnumber = 1)
where r.regid in  (select b.regid from regdb.vw_registrynumber r, regdb.vw_batch b
                  where r.regid = b.regid
                  and b.BATCH_COMMENT is not null
                  and b.batchnumber=1);
                  
PROMPT
PROMPT Registering additional batches...

insert into regdb.vw_batch (batchid, 
       batchnumber, 
       regid, 
       fullregnumber, 
       datecreated, 
       datelastmodified, 
       personcreated, 
       personregistered, 
       statusid, 
       tempbatchid, 
       scientist_id, 
       creation_date, 
       notebook_text, 
       amount, 
       amount_units, 
       appearance, 
       purity, 
       purity_comments, 
       sampleid, 
       solubility, 
       batch_comment, 
       storage_req_and_warnings, 
       formula_weight, 
       batch_formula, 
       percent_active) 
(select regdb.seq_batches.NextVal batchid, 
       to_number(substr(batch,2,3)) batchnumber, 
       b.regid regid, 
       b.FullRegNumber fullregnumber, 
       to_date(substr(lc.datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') datecreated, 
       to_date(substr(lc.datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') datelastmodified, 
       p.person_id personcreated, 
       p.person_id personregistered, 
       null statusid, 
       0 tempbatchid, 
       p.person_id scientist_id, 
       to_date(substr(lc.datecreated,1,19),'YYYY-MM-DD HH24:MI:SS') creation_date, 
       null notebook_text, 
       null amount, 
       null amount_units, 
       null appearance, 
       null purity, 
       null purity_comments, 
       null sampleid, 
       null solubility, 
       lc.legacyregnumber batch_comment, 
       null storage_req_and_warnings, 
       ci.molweight + nvl2(lc.saltequivalents, (lc.saltequivalents * f.molweight),0) FORMULA_WEIGHT, 
       ci.formula || nvl2(lc.saltequivalents, chr(149)||trim(lc.saltequivalents)||trim(f.formula),'') BATCH_FORMULA,
       nvl2(lc.saltequivalents, (100*ci.molweight)/(ci.molweight + lc.saltequivalents * f.molweight),100) percent_active        
from &&sdimptable lc, &&cartSchemaName..regdb_lx ci, &&securitySchemaName..people p, regdb.vw_batch b, &&fragmentssdimptable f
where lc.rowid = ci.rid
and lc.saltid= f.fragmentid(+)
and b.BATCH_COMMENT = lc.legacyregnumber
and trim(p.user_id) = upper(trim(lc.personcreated))
and to_number(substr(batch,2,3)) >1);


PROMPT
PROMPT Updating the FullRegnumbers to Legacy numbers....
update regdb.vw_batch b
       set b.fullregnumber = substr(b.fullregnumber,1,length('&&regPrefix'))||'&&regPrefixDelimeter'||lpad(b.BATCH_COMMENT, 6, '0')||'&&regBatchDelimeter'||b.batchnumber
where b.BATCH_COMMENT is not null; 

PROMPT
PROMPT Synchronizing the nextinsequence value...
update regdb.vw_sequence
set nextinsequence = (select max(sequencenumber)+1 from regdb.vw_registrynumber where sequenceid=1)
where sequenceid = 1;


          

