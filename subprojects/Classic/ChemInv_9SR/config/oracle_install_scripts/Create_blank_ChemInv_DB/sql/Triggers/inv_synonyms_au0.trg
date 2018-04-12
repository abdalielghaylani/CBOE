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

  audit_trail.check_val(raid, 'SYNONYM_ID', :new.SYNONYM_ID, :old.SYNONYM_ID);
  audit_trail.check_val(raid, 'COMPOUND_ID_FK', :new.COMPOUND_ID_FK, :old.COMPOUND_ID_FK);
  audit_trail.check_val(raid, 'SUBSTANCE_NAME', :new.SUBSTANCE_NAME, :old.SUBSTANCE_NAME);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);

end;
/
