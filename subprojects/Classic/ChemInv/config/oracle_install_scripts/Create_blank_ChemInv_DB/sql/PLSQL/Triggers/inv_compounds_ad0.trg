-- inv_compounds_ad0.trg
-- Audit Trigger After Delete on inv_compounds
create or replace trigger TRG_AUDIT_INV_COMPOUNDS_AD0
  after delete
  on inv_compounds
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_COMPOUNDS', :old.rid, 'D');

  deleted_data :=
  to_char(:old.compound_id) || '|' ||
  to_char(:old.mol_id) || '|' ||
  :old.cas || '|' ||
  :old.acx_id || '|' ||
  :old.substance_name || '|' ||
  to_char(:old.molecular_weight) || '|' ||
  to_char(:old.density) || '|' ||
  to_char(:old.clogp) || '|' ||
  to_char(:old.rotatable_bonds) || '|' ||
  to_char(:old.tot_pol_surf_area) || '|' ||
  to_char(:old.hbond_acceptors) || '|' ||
  to_char(:old.hbond_donors) || '|' ||
  :old.alt_id_1 || '|' ||
  :old.alt_id_2 || '|' ||
  :old.alt_id_3 || '|' ||
  :old.alt_id_4 || '|' ||
  :old.alt_id_5 || '|' ||
  :old.conflicting_fields || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
INSERT into Audit_compound (RAID, old_base64_cdx) VALUES (raid, :old.base64_cdx);
end;
/

