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

  audit_trail.check_val(raid, 'REQUEST_ID', :new.REQUEST_ID, :old.REQUEST_ID);
  audit_trail.check_val(raid, 'CONTAINER_ID_FK', :new.CONTAINER_ID_FK, :old.CONTAINER_ID_FK);
  audit_trail.check_val(raid, 'USER_ID_FK', :new.USER_ID_FK, :old.USER_ID_FK);
  audit_trail.check_val(raid, 'DATE_REQUIRED', :new.DATE_REQUIRED, :old.DATE_REQUIRED);
  audit_trail.check_val(raid, 'DATE_DELIVERED', :new.DATE_DELIVERED, :old.DATE_DELIVERED);
  audit_trail.check_val(raid, 'DELIVERED_BY_ID_FK', :new.DELIVERED_BY_ID_FK, :old.DELIVERED_BY_ID_FK);
  audit_trail.check_val(raid, 'QTY_REQUIRED', :new.QTY_REQUIRED, :old.QTY_REQUIRED);
  audit_trail.check_val(raid, 'DELIVERY_LOCATION_ID_FK', :new.DELIVERY_LOCATION_ID_FK, :old.DELIVERY_LOCATION_ID_FK);
  audit_trail.check_val(raid, 'REQUEST_COMMENTS', :new.REQUEST_COMMENTS, :old.REQUEST_COMMENTS);
  audit_trail.check_val(raid, 'REQUEST_TYPE_ID_FK', :new.REQUEST_TYPE_ID_FK, :old.REQUEST_TYPE_ID_FK);
  audit_trail.check_val(raid, 'REQUEST_STATUS_ID_FK', :new.REQUEST_STATUS_ID_FK, :old.REQUEST_STATUS_ID_FK);
  audit_trail.check_val(raid, 'CONTAINER_TYPE_ID_FK', :new.CONTAINER_TYPE_ID_FK, :old.CONTAINER_TYPE_ID_FK);
  audit_trail.check_val(raid, 'NUMBER_CONTAINERS', :new.NUMBER_CONTAINERS, :old.NUMBER_CONTAINERS);
  audit_trail.check_val(raid, 'QUANTITY_LIST', :new.QUANTITY_LIST, :old.QUANTITY_LIST);
  audit_trail.check_val(raid, 'SHIP_TO_NAME', :new.SHIP_TO_NAME, :old.SHIP_TO_NAME);
  audit_trail.check_val(raid, 'DECLINE_REASON', :new.DECLINE_REASON, :old.DECLINE_REASON);
  audit_trail.check_val(raid, 'EXPENSE_CENTER', :new.EXPENSE_CENTER, :old.EXPENSE_CENTER);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);    
end;
/
