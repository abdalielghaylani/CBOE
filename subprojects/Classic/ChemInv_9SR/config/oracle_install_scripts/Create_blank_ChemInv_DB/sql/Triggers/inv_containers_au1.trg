-- inv_containers_au1.trg
-- Script to create AFTER-UPDATE audit trigger for the most frequently
-- changed INV_CONTAINERS fields.

CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_CONTAINERS_AU1
AFTER UPDATE OF 
    location_id_fk, container_status_id_fk, current_user_id_fk ON INV_CONTAINERS FOR EACH ROW 
DECLARE
  raid number(10);
BEGIN
  SELECT seq_audit.NEXTVAL INTO raid FROM dual;

  audit_trail.record_transaction
    (raid, 'INV_CONTAINERS', :old.rid, 'U');

  audit_trail.check_val(raid, 'LOCATION_ID_FK', :new.LOCATION_ID_FK, :old.LOCATION_ID_FK);
  audit_trail.check_val(raid, 'CONTAINER_STATUS_ID_FK', :new.CONTAINER_STATUS_ID_FK, :old.CONTAINER_STATUS_ID_FK);
  audit_trail.check_val(raid, 'CURRENT_USER_ID_FK', :new.CURRENT_USER_ID_FK, :old.CURRENT_USER_ID_FK);
END;
/


