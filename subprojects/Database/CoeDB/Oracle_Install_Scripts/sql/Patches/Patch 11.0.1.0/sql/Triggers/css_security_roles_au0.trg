-- css_security_roles_au0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the SECURITY_ROLES table.

CREATE OR REPLACE TRIGGER trg_audit_css_sec_roles_au0
   AFTER UPDATE OF role_id, privilege_table_int_id, role_name, rid
   ON security_roles FOR EACH ROW
DECLARE
   raid   NUMBER (10);
BEGIN
   SELECT seq_audit.NEXTVAL INTO raid FROM DUAL;

   audit_trail.record_transaction (raid, 'SECURITY_ROLES', :OLD.rid, 'U');

   IF UPDATING ('role_id') THEN
      IF :OLD.role_id != :NEW.role_id THEN
         audit_trail.column_update (raid, 'ROLE_ID', :OLD.role_id, :NEW.role_id);
      END IF;
   END IF;

   IF UPDATING ('privilege_table_int_id') THEN
      IF :OLD.privilege_table_int_id != :NEW.privilege_table_int_id THEN
         audit_trail.column_update (raid, 'PRIVILEGE_TABLE_INT_ID', :OLD.privilege_table_int_id, :NEW.privilege_table_int_id);
      END IF;
   END IF;

   IF UPDATING ('role_name') THEN
      IF :OLD.role_name != :NEW.role_name THEN
         audit_trail.column_update (raid, 'ROLE_NAME', :OLD.role_name, :NEW.role_name);
      END IF;
   END IF;

   IF UPDATING ('rid') THEN
      IF :OLD.rid != :NEW.rid THEN
         audit_trail.column_update (raid, 'RID', :OLD.rid, :NEW.rid);
      END IF;
   END IF;
END;
/