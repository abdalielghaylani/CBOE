-- inv_order_containers_au0.trg
-- Generated on 25-AUG-05
-- Script to create AFTER-UPDATE audit trigger for the INV_ORDER_CONTAINERS table.

create or replace trigger TRG_AUDIT_INV_ORD_CNTNRS_AU0
  after update of
  order_id_fk,
  container_id_fk,
  rid
  on inv_order_containers
  for each row
  
declare
  raid number(10);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_ORDER_CONTAINERS', :old.rid, 'U');
 if updating('order_id_fk') then
  if :old.order_id_fk != :new.order_id_fk then
     audit_trail.column_update
       (raid, 'INV_ORDER_CONTAINERS',
       :old.order_id_fk, :new.order_id_fk);
  end if;	
 end if; 
 if updating('container_id_fk') then 
   if :old.container_id_fk != :new.container_id_fk then
     audit_trail.column_update
       (raid, 'INV_ORDER_CONTAINERS',
       :old.container_id_fk, :new.container_id_fk);
  end if;      
 end if; 
 if updating('rid') then 
   if :old.rid != :new.rid then
     audit_trail.column_update
       (raid, 'INV_ORDER_CONTAINERS',
       :old.rid, :new.rid);
  end if;
 end if; 
end;

/
