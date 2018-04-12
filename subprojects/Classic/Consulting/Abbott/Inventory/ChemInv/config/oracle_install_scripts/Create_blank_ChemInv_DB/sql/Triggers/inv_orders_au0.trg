-- inv_orders_au0.trg
-- Generated on 24-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the INV_ORDERS table.

create or replace trigger TRG_AUDIT_INV_ORDERS_AU0
  after update of
  order_id,
  delivery_location_id_fk,
  ship_to_name,
  order_status_id_fk,
  shipping_conditions,
  date_created,
  date_shipped,
  date_received,
  cancel_reason,
  rid
  
  on inv_orders
  for each row
  
declare
  raid number(10);

begin

  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_ORDERS', :old.rid, 'U');
    
  if updating('order_id') then
    if :old.order_id != :new.order_id then
      audit_trail.column_update
        (raid, 'ORDER_ID',
        :old.order_id, :new.order_id);
    end if;	
  end if; 
  if updating('delivery_location_id_fk') then 
    if :old.delivery_location_id_fk != :new.delivery_location_id_fk then
      audit_trail.column_update
        (raid, 'DELIVERY_LOCATOIN_ID_FK',
        :old.delivery_location_id_fk, :new.delivery_location_id_fk);
    end if;      
  end if; 
  if updating('ship_to_name') then 
    if :old.ship_to_name != :new.ship_to_name then
      audit_trail.column_update
        (raid, 'SHIP_TO_NAME',
        :old.ship_to_name, :new.ship_to_name);
    end if;
  end if; 
  if updating('order_status_id_fk') then 
    if :old.order_status_id_fk != :new.order_status_id_fk then
      audit_trail.column_update
        (raid, 'ORDER_STATUS_ID_FK',
        :old.order_status_id_fk, :new.order_status_id_fk);
    end if;
  end if; 
  if updating('shipping_conditions') then 
    if :old.shipping_conditions != :new.shipping_conditions then
      audit_trail.column_update
        (raid, 'SHIPPING_CONDITIONS',
        :old.shipping_conditions, :new.shipping_conditions);
    end if;
  end if; 
  if updating('date_created') then 
    if nvl(:old.date_created,TO_DATE('1', 'J')) !=
      NVL(:new.date_created,TO_DATE('1', 'J')) then
      audit_trail.column_update
        (raid, 'DATE_CREATED',
        :old.date_created, :new.date_created);
    end if;
  end if; 
  if updating('date_shipped') then 
    if nvl(:old.date_shipped,TO_DATE('1', 'J')) !=
      NVL(:new.date_shipped,TO_DATE('1', 'J')) then
      audit_trail.column_update
        (raid, 'DATE_SHIPPED',
        :old.date_shipped, :new.date_shipped);
    end if;
  end if; 
  if updating('date_received') then 
    if nvl(:old.date_received,TO_DATE('1', 'J')) !=
      NVL(:new.date_received,TO_DATE('1', 'J')) then
      audit_trail.column_update
        (raid, 'DATE_RECEIVED',
        :old.date_received, :new.date_received);
    end if;
  end if; 
  if updating('cancel_reason') then 
    if :old.cancel_reason != :new.cancel_reason then
      audit_trail.column_update
        (raid, 'CANCEL_REASON',
        :old.cancel_reason, :new.cancel_reason);
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
