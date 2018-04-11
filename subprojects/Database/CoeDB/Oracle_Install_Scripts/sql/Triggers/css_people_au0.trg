-- css_people_au0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the PEOPLE table.

CREATE OR REPLACE TRIGGER trg_audit_css_people_au0
   AFTER UPDATE OF person_id,
                   user_code,
                   user_id,
                   supervisor_internal_id,
                   title,
                   first_name,
                   middle_name,
                   last_name,
                   site_id,
                   department,
                   int_address,
                   telephone,
                   email,
                   active,
                   rid
   ON people FOR EACH ROW
DECLARE
   raid   NUMBER (10);
BEGIN
   SELECT seq_audit.NEXTVAL INTO raid FROM DUAL;

   audit_trail.record_transaction (raid, 'PEOPLE', :OLD.rid, 'U');

   IF UPDATING ('person_id') THEN
      IF :OLD.person_id != :NEW.person_id THEN
         audit_trail.column_update (raid, 'PERSON_ID', :OLD.person_id, :NEW.person_id);
      END IF;
   END IF;

   IF UPDATING ('user_code') THEN
      IF :OLD.user_code != :NEW.user_code THEN
         audit_trail.column_update (raid, 'USER_CODE', :OLD.user_code, :NEW.user_code);
      END IF;
   END IF;

   IF UPDATING ('user_id') THEN
      IF :OLD.user_id != :NEW.user_id THEN
         audit_trail.column_update (raid, 'USER_ID', :OLD.user_id, :NEW.user_id);
      END IF;
   END IF;

   IF UPDATING ('supervisor_internal_id') THEN
      IF :OLD.supervisor_internal_id != :NEW.supervisor_internal_id THEN
         audit_trail.column_update (raid, 'SUPERVIOR_INTERNAL_ID', :OLD.supervisor_internal_id, :NEW.supervisor_internal_id);
      END IF;
   END IF;

   IF UPDATING ('title') THEN
      IF :OLD.title != :NEW.title THEN
         audit_trail.column_update (raid, 'TITLE', :OLD.title, :NEW.title);
      END IF;
   END IF;

   IF UPDATING ('first_name') THEN
      IF :OLD.first_name != :NEW.first_name THEN
         audit_trail.column_update (raid, 'FIRST_NAME', :OLD.first_name, :NEW.first_name);
      END IF;
   END IF;

   IF UPDATING ('middle_name') THEN
      IF :OLD.middle_name != :NEW.middle_name THEN
         audit_trail.column_update (raid, 'MIDDLE_NAME', :OLD.middle_name, :NEW.middle_name);
      END IF;
   END IF;

   IF UPDATING ('last_name') THEN
      IF :OLD.last_name != :NEW.last_name THEN
         audit_trail.column_update (raid, 'LAST_NAME', :OLD.last_name, :NEW.last_name);
      END IF;
   END IF;

   IF UPDATING ('site_id') THEN
      IF :OLD.site_id != :NEW.site_id THEN
         audit_trail.column_update (raid, 'SITE_ID', :OLD.site_id, :NEW.site_id);
      END IF;
   END IF;

   IF UPDATING ('department') THEN
      IF :OLD.department != :NEW.department THEN
         audit_trail.column_update (raid, 'DEPARTMENT', :OLD.department, :NEW.department);
      END IF;
   END IF;

   IF UPDATING ('int_address') THEN
      IF :OLD.int_address != :NEW.int_address THEN
         audit_trail.column_update (raid, 'INT_ADDRESS', :OLD.int_address, :NEW.int_address);
      END IF;
   END IF;

   IF UPDATING ('telephone') THEN
      IF :OLD.telephone != :NEW.telephone THEN
         audit_trail.column_update (raid, 'TELEPHONE', :OLD.telephone, :NEW.telephone);
      END IF;
   END IF;

   IF UPDATING ('email') THEN
      IF :OLD.email != :NEW.email THEN
         audit_trail.column_update (raid, 'EMAIL', :OLD.email, :NEW.email);
      END IF;
   END IF;

   IF UPDATING ('active') THEN
      IF :OLD.active != :NEW.active THEN
         audit_trail.column_update (raid, 'ACTIVE', :OLD.active, :NEW.active);
      END IF;
   END IF;

   IF UPDATING ('rid') THEN
      IF :OLD.rid != :NEW.rid THEN
         audit_trail.column_update (raid, 'RID', :OLD.rid, :NEW.rid);
      END IF;
   END IF;
END;
/