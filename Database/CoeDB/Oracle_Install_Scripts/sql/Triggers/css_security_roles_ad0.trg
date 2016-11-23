-- css_security_roles_ad0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-DELETE audit trigger for the SECURITY_ROLES table.

CREATE OR REPLACE TRIGGER trg_audit_css_sec_roles_ad0
   AFTER DELETE
   ON security_roles FOR EACH ROW
DECLARE
   raid           NUMBER (10);
   deleted_data   VARCHAR2 (4000);
BEGIN
   SELECT seq_audit.NEXTVAL INTO raid FROM DUAL;

   audit_trail.record_transaction (raid, 'SECURITY_ROLES', :OLD.rid, 'D');
   deleted_data := TO_CHAR (:OLD.role_id) || '|'
                || TO_CHAR (:OLD.privilege_table_int_id) || '|'
                || TO_CHAR (:OLD.role_name) || '|'
                || TO_CHAR (:OLD.rid) || '|'
                ||          :OLD.creator || '|'
                || TO_CHAR (:OLD.TIMESTAMP);

   INSERT INTO audit_delete (raid, row_data)
        VALUES (raid, deleted_data);
END;
/