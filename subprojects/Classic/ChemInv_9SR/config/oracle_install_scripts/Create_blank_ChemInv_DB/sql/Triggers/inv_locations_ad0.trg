-- inv_locations_ad0.trg
-- Audit Trigger After Delete on inv_locations
create or replace trigger TRG_AUDIT_INV_LOCATIONS_AD0
  after delete
  on inv_locations
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_LOCATIONS', :old.rid, 'D');

  deleted_data :=
  to_char(:old.location_id) || '|' ||
  to_char(:old.parent_id) || '|' ||
  :old.description || '|' ||
  to_char(:old.location_type_id_fk) || '|' ||
  :old.location_name || '|' ||
  :old.location_description || '|' ||
  :old.location_barcode || '|' ||
  to_char(:old.address_id_fk) || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp) || '|' ||
  :old.owner_id_fk || '|' ||
  :old.allows_containers;

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;
/

