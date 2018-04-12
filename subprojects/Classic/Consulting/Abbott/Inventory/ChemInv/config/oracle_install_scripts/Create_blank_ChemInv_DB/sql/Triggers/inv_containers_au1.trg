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

  IF UPDATING('location_id_fk') THEN
    IF :old.location_id_fk != :new.location_id_fk THEN
       audit_trail.column_update
         (raid, 'LOCATION_ID_FK',
         :old.location_id_fk, :new.location_id_fk);
    END IF;
  END IF;

  IF UPDATING('container_status_id_fk') THEN
    IF :old.container_status_id_fk != :new.container_status_id_fk THEN
       audit_trail.column_update
         (raid, 'CONTAINER_STATUS_ID_FK',
         :old.container_status_id_fk, :new.container_status_id_fk);
    END IF;
  END IF;

  IF UPDATING('current_user_id_fk') THEN
    IF :old.current_user_id_fk != :new.current_user_id_fk THEN
       audit_trail.column_update
         (raid, 'CURRENT_USER_ID_FK',
         :old.current_user_id_fk, :new.current_user_id_fk);
    END IF;
  END IF;
END;
/


