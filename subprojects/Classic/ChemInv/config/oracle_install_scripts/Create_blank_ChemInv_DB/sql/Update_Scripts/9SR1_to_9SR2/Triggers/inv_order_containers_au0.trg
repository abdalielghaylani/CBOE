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

  audit_trail.check_val(raid, 'ORDER_ID_FK', :new.ORDER_ID_FK, :old.ORDER_ID_FK);
  audit_trail.check_val(raid, 'CONTAINER_ID_FK', :new.CONTAINER_ID_FK, :old.CONTAINER_ID_FK);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);
end;

/
