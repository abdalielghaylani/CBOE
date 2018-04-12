-- inv_suppliers_au0.trg
-- Generated on 24-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the INV_SUPPLIERS table.

create or replace trigger TRG_AUDIT_INV_SUPPLIERS_AU0
  after update of
  supplier_id,
  supplier_name,
  supplier_short_name,
  supplier_code,
  supplier_facility_name,
  supplier_address_id_fk,
  is_official_supplier,
  rid
  
  on inv_suppliers
  for each row
  
declare
  raid number(10);

begin

  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_SUPPLIERS', :old.rid, 'U');
    
  audit_trail.check_val(raid, 'SUPPLIER_ID', :new.SUPPLIER_ID, :old.SUPPLIER_ID);
  audit_trail.check_val(raid, 'SUPPLIER_NAME', :new.SUPPLIER_NAME, :old.SUPPLIER_NAME);
  audit_trail.check_val(raid, 'SUPPLIER_SHORT_NAME', :new.SUPPLIER_SHORT_NAME, :old.SUPPLIER_SHORT_NAME);
  audit_trail.check_val(raid, 'SUPPLIER_CODE', :new.SUPPLIER_CODE, :old.SUPPLIER_CODE);
  audit_trail.check_val(raid, 'SUPPLIER_FACILITY_NAME', :new.SUPPLIER_FACILITY_NAME, :old.SUPPLIER_FACILITY_NAME);
  audit_trail.check_val(raid, 'SUPPLIER_ADDRESS_ID_FK', :new.SUPPLIER_ADDRESS_ID_FK, :old.SUPPLIER_ADDRESS_ID_FK);
  audit_trail.check_val(raid, 'IS_OFFICIAL_SUPPLIER', :new.IS_OFFICIAL_SUPPLIER, :old.IS_OFFICIAL_SUPPLIER);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);
end;

/
