-- inv_locations_au0.trg
-- Audit Trigger After Update on inv_locations
create or replace trigger TRG_AUDIT_INV_LOCATIONS_AU0
  after update of
  location_id,
  parent_id,
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
 if updating('location_id') then
  if :old.location_id != :new.location_id then
     audit_trail.column_update
       (raid, 'LOCATION_ID',
       :old.location_id, :new.location_id);
  end if;
 end if; 
 if updating('parent_id') then 
   if nvl(:old.parent_id,0) !=
     NVL(:new.parent_id,0) then
     audit_trail.column_update
       (raid, 'PARENT_ID',
       :old.parent_id, :new.parent_id);
   end if;
 end if;  
 if updating('location_type_id_fk') then 
  if nvl(:old.location_type_id_fk,0) !=
     NVL(:new.location_type_id_fk,0) then
     audit_trail.column_update
       (raid, 'LOCATION_TYPE_ID_FK',
       :old.location_type_id_fk, :new.location_type_id_fk);
  end if;
 end if; 
 if updating('location_name') then 
    if nvl(:old.location_name,' ') !=
     NVL(:new.location_name,' ') then 
     audit_trail.column_update
       (raid, 'LOCATION_NAME',
       :old.location_name, :new.location_name);
    end if;
 end if; 
 if updating('location_description') then 
  if nvl(:old.location_description,' ') !=
     NVL(:new.location_description,' ') then
     audit_trail.column_update
       (raid, 'LOCATION_DESCRIPTION',
       :old.location_description, :new.location_description);
  end if;
 end if; 
 if updating('location_barcode') then 
  if nvl(:old.location_barcode,' ') !=
     NVL(:new.location_barcode,' ') then
     audit_trail.column_update
       (raid, 'LOCATION_BARCODE',
       :old.location_barcode, :new.location_barcode);
  end if;
 end if; 
 if updating('rid') then 
  if :old.rid != :new.rid then   
     audit_trail.column_update
       (raid, 'RID',
       :old.rid, :new.rid);
  end if;     
 end if; 
 if updating('owner_id_fk') then 
  if nvl(:old.owner_id_fk,' ') !=
     NVL(:new.owner_id_fk,' ') then
     audit_trail.column_update
       (raid, 'OWNER_ID_FK',
       :old.owner_id_fk, :new.owner_id_fk);
  end if;
 end if; 
 if updating('allows_containers') then 
  if nvl(:old.allows_containers,' ') !=
     NVL(:new.allows_containers,' ') then
     audit_trail.column_update
       (raid, 'ALLOWS_CONTAINERS',
       :old.allows_containers, :new.allows_containers);
  end if;
 end if;
 if updating('address_id_fk') then 
  if nvl(:old.address_id_fk,0) !=
     NVL(:new.address_id_fk,0) then
     audit_trail.column_update
       (raid, 'ADDRESS_ID_FK',
       :old.address_id_fk, :new.address_id_fk);
  end if;
 end if;
end;
/
