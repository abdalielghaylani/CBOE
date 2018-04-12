-- &&tabname._au0.trg
-- Generated on 27-JAN-05 by CHEMINVDB2.
-- Script to create AFTER-UPDATE audit trigger for the INV_SYNONYMS table.

create or replace trigger TRG_AUDIT_INV_SYNONYMS_AU0
  after update of

  synonym_id,
  compound_id_fk,
  substance_name,
  rid
  on inv_synonyms
  for each row

declare
  raid number(10);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_SYNONYMS', :old.rid, 'U');

  IF UPDATING('synonym_id') THEN
    IF NVL(:old.synonym_id,0) != NVL(:new.synonym_id,0) THEN
       audit_trail.column_update
         (raid, 'SYNONYM_ID',
         :old.synonym_id, :new.synonym_id);
    END IF;
  END IF;

  IF UPDATING('compound_id_fk') THEN
    IF NVL(:old.compound_id_fk,0) != NVL(:new.compound_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'COMPOUND_ID_FK',
         :old.compound_id_fk, :new.compound_id_fk);
    END IF;
  END IF;
  
  IF UPDATING('substance_name') THEN
    IF :old.substance_name != :new.substance_name THEN
     audit_trail.column_update
       (raid, 'SUBSTANCE_NAME',
       :old.substance_name, :new.substance_name);
    END IF;
  END IF;

  IF UPDATING('rid') THEN
    IF :old.rid != :new.rid THEN
       audit_trail.column_update
         (raid, 'RID',
         :old.rid, :new.rid);
    END IF;
  END IF;

end;
/
