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
    
  if updating('supplier_id') then
    if :old.supplier_id != :new.supplier_id then
      audit_trail.column_update
        (raid, 'SUPPLIER_ID',
        :old.supplier_id, :new.supplier_id);
    end if;	
  end if; 
  if updating('supplier_name') then 
    if :old.supplier_name != :new.supplier_name then
      audit_trail.column_update
        (raid, 'SUPPLIER_NAME',
        :old.supplier_name, :new.supplier_name);
    end if;      
  end if; 
  if updating('supplier_short_name') then 
    if :old.supplier_short_name != :new.supplier_short_name then
      audit_trail.column_update
        (raid, 'SUPPLIER_SHORT_NAME',
        :old.supplier_short_name, :new.supplier_short_name);
    end if;
  end if; 
  if updating('supplier_code') then 
    if :old.supplier_code != :new.supplier_code then
      audit_trail.column_update
        (raid, 'SUPPLIER_CODE',
        :old.supplier_code, :new.supplier_code);
    end if;
  end if; 
  if updating('supplier_facility_name') then 
    if :old.supplier_facility_name != :new.supplier_facility_name then
      audit_trail.column_update
        (raid, 'SUPPLIER_FACILITY_NAME',
        :old.supplier_facility_name, :new.supplier_facility_name);
    end if;
  end if; 
  if updating('supplier_address_id_fk') then 
    if :old.supplier_address_id_fk != :new.supplier_address_id_fk then
      audit_trail.column_update
        (raid, 'SUPPLIER_ADDRESS_ID_FK',
        :old.supplier_address_id_fk, :new.supplier_address_id_fk);
    end if;
  end if;
  if updating('is_official_supplier') then 
    if :old.is_official_supplier != :new.is_official_supplier then
      audit_trail.column_update
        (raid, 'IS_OFFICIAL_SUPPLIER',
        :old.is_official_supplier, :new.is_official_supplier);
    end if;
  end if;
  if updating('rid') then 
    if :old.rid != :new.rid then
      audit_trail.column_update
        (raid, 'RID',
        :old.rid, :new.rid);
    end if;     
  end if;

end;

/
