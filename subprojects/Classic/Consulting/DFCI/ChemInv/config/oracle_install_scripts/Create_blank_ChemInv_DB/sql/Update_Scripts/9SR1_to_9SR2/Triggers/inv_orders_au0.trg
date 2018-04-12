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
    
  audit_trail.check_val(raid, 'ORDER_ID', :new.ORDER_ID, :old.ORDER_ID);
  audit_trail.check_val(raid, 'DELIVERY_LOCATION_ID_FK', :new.DELIVERY_LOCATION_ID_FK, :old.DELIVERY_LOCATION_ID_FK);
  audit_trail.check_val(raid, 'SHIP_TO_NAME', :new.SHIP_TO_NAME, :old.SHIP_TO_NAME);
  audit_trail.check_val(raid, 'ORDER_STATUS_ID_FK', :new.ORDER_STATUS_ID_FK, :old.ORDER_STATUS_ID_FK);
  audit_trail.check_val(raid, 'SHIPPING_CONDITIONS', :new.SHIPPING_CONDITIONS, :old.SHIPPING_CONDITIONS);
  audit_trail.check_val(raid, 'DATE_CREATED', :new.DATE_CREATED, :old.DATE_CREATED);
  audit_trail.check_val(raid, 'DATE_SHIPPED', :new.DATE_SHIPPED, :old.DATE_SHIPPED);
  audit_trail.check_val(raid, 'DATE_RECEIVED', :new.DATE_RECEIVED, :old.DATE_RECEIVED);
  audit_trail.check_val(raid, 'CANCEL_REASON', :new.CANCEL_REASON, :old.CANCEL_REASON);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);   
end;

/
