-- inv_locations_au0.trg
-- Audit Trigger After Update on inv_locations
create or replace trigger TRG_AUDIT_INV_LOCATIONS_AU0
  after update of
  location_id,
  parent_id,
  description,
  location_type_id_fk,
  location_name,
  location_description,
  location_barcode,
  owner_id_fk,
  allows_containers,
  address_id_fk,
  rid
  on inv_locations
  for each row

declare
  raid number(10);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_LOCATIONS', :old.rid, 'U');

  audit_trail.check_val(raid, 'LOCATION_ID', :new.LOCATION_ID, :old.LOCATION_ID);
  audit_trail.check_val(raid, 'PARENT_ID', :new.PARENT_ID, :old.PARENT_ID);
  audit_trail.check_val(raid, 'DESCRIPTION', :new.DESCRIPTION, :old.DESCRIPTION);
  audit_trail.check_val(raid, 'LOCATION_TYPE_ID_FK', :new.LOCATION_TYPE_ID_FK, :old.LOCATION_TYPE_ID_FK);
  audit_trail.check_val(raid, 'LOCATION_NAME', :new.LOCATION_NAME, :old.LOCATION_NAME);
  audit_trail.check_val(raid, 'LOCATION_DESCRIPTION', :new.LOCATION_DESCRIPTION, :old.LOCATION_DESCRIPTION);
  audit_trail.check_val(raid, 'LOCATION_BARCODE', :new.LOCATION_BARCODE, :old.LOCATION_BARCODE);
  audit_trail.check_val(raid, 'OWNER_ID_FK', :new.OWNER_ID_FK, :old.OWNER_ID_FK);
  audit_trail.check_val(raid, 'ALLOWS_CONTAINERS', :new.ALLOWS_CONTAINERS, :old.ALLOWS_CONTAINERS);
  audit_trail.check_val(raid, 'ADDRESS_ID_FK', :new.ADDRESS_ID_FK, :old.ADDRESS_ID_FK);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);
end;
/
