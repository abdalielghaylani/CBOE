-- css_people_ad0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-DELETE audit trigger for the PEOPLE table.

CREATE OR REPLACE TRIGGER trg_audit_css_people_ad0
   AFTER DELETE
   ON people FOR EACH ROW
DECLARE
   raid           NUMBER (10);
   deleted_data   VARCHAR2 (4000);
BEGIN
   SELECT seq_audit.NEXTVAL INTO raid FROM DUAL;

   audit_trail.record_transaction (raid, 'PEOPLE', :OLD.rid, 'D');
   deleted_data := TO_CHAR (:OLD.person_id) || '|'
                ||          :OLD.user_code || '|'
                ||          :OLD.user_id || '|'
                || TO_CHAR (:OLD.supervisor_internal_id) || '|'
                ||          :OLD.title || '|'
                ||          :OLD.first_name || '|'
                ||          :OLD.middle_name || '|'
                ||          :OLD.last_name || '|'
                || TO_CHAR (:OLD.site_id) || '|'
                ||          :OLD.department || '|'
                ||          :OLD.int_address || '|'
                ||          :OLD.telephone || '|'
                ||          :OLD.email || '|'
                || TO_CHAR (:OLD.active) || '|'
                || TO_CHAR (:OLD.rid) || '|'
                ||          :OLD.creator || '|'
                || TO_CHAR (:OLD.timestamp);

   INSERT INTO audit_delete (raid, row_data)
        VALUES (raid, deleted_data);
END;
/