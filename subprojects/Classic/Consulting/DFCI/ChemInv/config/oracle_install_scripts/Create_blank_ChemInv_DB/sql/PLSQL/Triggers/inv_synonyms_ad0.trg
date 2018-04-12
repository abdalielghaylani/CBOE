-- &&tabname._ad0.trg
-- Generated on 27-JAN-05 by CHEMINVDB2.
-- Script to create AFTER-DELETE audit trigger for the INV_SYNONYMS table.

create or replace trigger TRG_AUDIT_INV_SYNONYMS_AD0
  after delete
  on inv_synonyms
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_SYNONYMS', :old.rid, 'D');

  deleted_data :=

  to_char(:old.synonym_id) || '|' ||
  to_char(:old.compound_id_fk) || '|' ||
  :old.substance_name || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp);
insert into audit_delete
(raid, row_data) values (raid, deleted_data);

end;
/
