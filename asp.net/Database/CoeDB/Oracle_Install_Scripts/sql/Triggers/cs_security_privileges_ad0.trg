-- cs_security_privileges_ad0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-DELETE audit trigger for the CS_SECURITY_PRIVILEGES table.

CREATE OR REPLACE TRIGGER trg_audit_cs_sec_priv_ad0
   AFTER DELETE
   ON cs_security_privileges
   FOR EACH ROW
DECLARE
   raid           NUMBER (10);
   deleted_data   VARCHAR2 (4000);
BEGIN
   SELECT seq_audit.NEXTVAL INTO raid FROM DUAL;

   audit_trail.record_transaction (raid, 'CS_SECURITY_PRIVILEGES', :OLD.rid, 'D');
   deleted_data := TO_CHAR (:OLD.role_internal_id) || '|'
                || TO_CHAR (:OLD.css_login) || '|'
                || TO_CHAR (:OLD.css_create_user) || '|'
                || TO_CHAR (:OLD.css_edit_user) || '|'
                || TO_CHAR (:OLD.css_delete_user) || '|'
                || TO_CHAR (:OLD.css_change_password) || '|'
                || TO_CHAR (:OLD.css_create_role) || '|'
                || TO_CHAR (:OLD.css_edit_role) || '|'
                || TO_CHAR (:OLD.css_delete_role) || '|'
                || TO_CHAR (:OLD.css_create_workgrp) || '|'
                || TO_CHAR (:OLD.css_edit_workgrp) || '|'
                || TO_CHAR (:OLD.css_delete_workgrp) || '|'
                || TO_CHAR (:OLD.rid) || '|'
                ||          :OLD.creator || '|'
                || TO_CHAR (:OLD.timeStamp);

   INSERT INTO audit_delete (raid, row_data)
        VALUES (raid, deleted_data);
END;
/