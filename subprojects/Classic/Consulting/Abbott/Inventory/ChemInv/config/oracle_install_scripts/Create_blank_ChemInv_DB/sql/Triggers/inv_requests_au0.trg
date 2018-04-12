-- inv_requests_au0.trg
-- Generated on 31-AUG-02
-- Script to create AFTER-UPDATE audit trigger for the INV_REQUESTS table.

create or replace trigger TRG_AUDIT_INV_REQUESTS_AU0
  after update of
  request_id,
  container_id_fk,
  user_id_fk,
  date_required,
  date_delivered,
  delivered_by_id_fk,
  qty_required,
  delivery_location_id_fk,
  request_comments,
  request_type_id_fk,
  request_status_id_fk,
  container_type_id_fk,
  number_containers,
  quantity_list,
  ship_to_name,
  decline_reason,
  expense_center,
  rid
  on inv_requests
  for each row
  
declare
  raid number(10);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_REQUESTS', :old.rid, 'U');
 if updating('request_id') then
  if :old.request_id != :new.request_id then
     audit_trail.column_update
       (raid, 'REQUEST_ID',
       :old.request_id, :new.request_id);
  end if;	
 end if; 
 if updating('container_id_fk') then 
   if :old.container_id_fk != :new.container_id_fk then
     audit_trail.column_update
       (raid, 'CONTAINER_ID_FK',
       :old.container_id_fk, :new.container_id_fk);
  end if;      
 end if; 
 if updating('user_id_fk') then 
   if :old.user_id_fk != :new.user_id_fk then
     audit_trail.column_update
       (raid, 'USER_ID_FK',
       :old.user_id_fk, :new.user_id_fk);
  end if;
 end if; 
 if updating('date_required') then 
  if nvl(:old.date_required,TO_DATE('1', 'J')) !=
     NVL(:new.date_required,TO_DATE('1', 'J')) then
     audit_trail.column_update
       (raid, 'DATE_REQUIRED',
       :old.date_required, :new.date_required);
  end if;
 end if; 
 if updating('date_delivered') then 
  if nvl(:old.date_delivered,TO_DATE('1', 'J')) !=
     NVL(:new.date_delivered,TO_DATE('1', 'J')) then
     audit_trail.column_update
       (raid, 'DATE_DELIVERED',
       :old.date_delivered, :new.date_delivered);
  end if;
 end if; 
 if updating('delivered_by_id_fk') then 
  if nvl(:old.delivered_by_id_fk,' ') !=
     NVL(:new.delivered_by_id_fk,' ') then
     audit_trail.column_update
       (raid, 'DELIVERED_BY_ID_FK',
       :old.delivered_by_id_fk, :new.delivered_by_id_fk);
  end if;
 end if; 
 if updating('qty_required') then 
   if :old.qty_required != :new.qty_required then
     audit_trail.column_update
       (raid, 'QTY_REQUIRED',
       :old.qty_required, :new.qty_required);
  end if;
 end if; 
 if updating('delivery_location_id_fk') then 
   if :old.delivery_location_id_fk != :new.delivery_location_id_fk then
     audit_trail.column_update
       (raid, 'DELIVERY_LOCATION_ID_FK',
       :old.delivery_location_id_fk, :new.delivery_location_id_fk);
  end if;
 end if; 
 if updating('request_comments') then 
  if nvl(:old.request_comments,' ') !=
     NVL(:new.request_comments,' ') then
     audit_trail.column_update
       (raid, 'REQUEST_COMMENTS',
       :old.request_comments, :new.request_comments);
  end if;
 end if; 
 if updating('request_type_id_fk') then 
   if :old.request_type_id_fk != :new.request_type_id_fk then
     audit_trail.column_update
       (raid, 'REQUEST_TYPE_ID_FK',
       :old.request_type_id_fk, :new.request_type_id_fk);
  end if;
 end if; 
 if updating('request_status_id_fk') then 
   if :old.request_status_id_fk != :new.request_status_id_fk then
     audit_trail.column_update
       (raid, 'REQUEST_STATUS_ID_FK',
       :old.request_status_id_fk, :new.request_status_id_fk);
  end if;
 end if; 
 if updating('container_type_id_fk') then 
   if :old.container_type_id_fk != :new.container_type_id_fk then
     audit_trail.column_update
       (raid, 'CONTAINER_TYPE_ID_FK',
       :old.container_type_id_fk, :new.container_type_id_fk);
  end if;
 end if; 
 if updating('number_containers') then 
   if :old.number_containers != :new.number_containers then
     audit_trail.column_update
       (raid, 'NUMBER_CONTAINERS',
       :old.number_containers, :new.number_containers);
  end if;
 end if; 
 if updating('quantity_list') then 
   if :old.quantity_list != :new.quantity_list then
     audit_trail.column_update
       (raid, 'QUANTITY_LIST',
       :old.quantity_list, :new.quantity_list);
  end if;
 end if; 
 if updating('ship_to_name') then 
   if :old.ship_to_name != :new.ship_to_name then
     audit_trail.column_update
       (raid, 'SHIP_TO_NAME',
       :old.ship_to_name, :new.ship_to_name);
  end if;
 end if; 
 if updating('decline_reason') then 
  if nvl(:old.decline_reason,' ') !=
     NVL(:new.decline_reason,' ') then
     audit_trail.column_update
       (raid, 'DECLINE_REASON',
       :old.decline_reason, :new.decline_reason);
  end if;
 end if; 
 if updating('expense_center') then 
  if nvl(:old.expense_center,' ') !=
     NVL(:new.expense_center,' ') then
     audit_trail.column_update
       (raid, 'EXPENSE_CENTER',
       :old.expense_center, :new.expense_center);
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
