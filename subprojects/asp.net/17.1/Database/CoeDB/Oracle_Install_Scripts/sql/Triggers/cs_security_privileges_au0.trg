-- cs_security_privileges_au0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the CS_SECURITY_PRIVILEGES table.

CREATE OR REPLACE TRIGGER trg_audit_cs_sec_priv_au0
   AFTER UPDATE OF role_internal_id,
                   css_login,
                   css_create_user,
                   css_edit_user,
                   css_delete_user,
                   css_change_password,
                   css_create_role,
                   css_edit_role,
                   css_delete_role,
                   css_create_workgrp,
                   css_edit_workgrp,
                   css_delete_workgrp,
                   rid
   ON cs_security_privileges
   FOR EACH ROW
DECLARE
   raid   NUMBER (10);
BEGIN
   SELECT seq_audit.NEXTVAL INTO raid FROM DUAL;

   audit_trail.record_transaction (raid, 'CS_SECURITY_PRIVILEGES', :OLD.rid, 'U');

   IF UPDATING ('role_internal_id') THEN
      IF :OLD.role_internal_id != :NEW.role_internal_id THEN
         audit_trail.column_update (raid, 'ROLE_INTERNAL_ID', :OLD.role_internal_id, :NEW.role_internal_id);
      END IF;
   END IF;

   IF UPDATING ('css_create_user') THEN
      IF :OLD.css_create_user != :NEW.css_create_user THEN
         audit_trail.column_update (raid, 'CSS_CREATE_USER', :OLD.css_create_user, :NEW.css_create_user);
      END IF;
   END IF;

   IF UPDATING ('css_edit_user') THEN
      IF :OLD.css_edit_user != :NEW.css_edit_user THEN
         audit_trail.column_update (raid, 'CSS_EDIT_USER', :OLD.css_edit_user, :NEW.css_edit_user);
      END IF;
   END IF;

   IF UPDATING ('css_delete_user') THEN
      IF :OLD.css_delete_user != :NEW.css_delete_user THEN
         audit_trail.column_update (raid, 'CSS_DELETE_USER', :OLD.css_delete_user, :NEW.css_delete_user);
      END IF;
   END IF;

   IF UPDATING ('css_change_password') THEN
      IF :OLD.css_change_password != :NEW.css_change_password THEN
         audit_trail.column_update (raid, 'CSS_CHANGE_PASSWORD', :OLD.css_change_password, :NEW.css_change_password);
      END IF;
   END IF;

   IF UPDATING ('css_create_role') THEN
      IF :OLD.css_create_role != :NEW.css_create_role THEN
         audit_trail.column_update (raid, 'CSS_CREATE_ROLE', :OLD.css_create_role, :NEW.css_create_role);
      END IF;
   END IF;

   IF UPDATING ('css_edit_role') THEN
      IF :OLD.css_edit_role != :NEW.css_edit_role THEN
         audit_trail.column_update (raid, 'CSS_EDIT_ROLE', :OLD.css_edit_role, :NEW.css_edit_role);
      END IF;
   END IF;

   IF UPDATING ('css_delete_role') THEN
      IF :OLD.css_delete_role != :NEW.css_delete_role THEN
         audit_trail.column_update (raid, 'CSS_DELETE_ROLE', :OLD.css_delete_role, :NEW.css_delete_role);
      END IF;
   END IF;

   IF UPDATING ('css_create_workgrp') THEN
      IF :OLD.css_create_workgrp != :NEW.css_create_workgrp THEN
         audit_trail.column_update (raid, 'CSS_CREATE_WORKGRP', :OLD.css_create_workgrp, :NEW.css_create_workgrp);
      END IF;
   END IF;

   IF UPDATING ('css_edit_workgrp') THEN
      IF :OLD.css_edit_workgrp != :NEW.css_edit_workgrp THEN
         audit_trail.column_update (raid, 'CSS_EDIT_WORKGRP', :OLD.css_edit_workgrp, :NEW.css_edit_workgrp);
      END IF;
   END IF;

   IF UPDATING ('css_delete_workgrp') THEN
      IF :OLD.css_delete_workgrp != :NEW.css_delete_workgrp THEN
         audit_trail.column_update (raid, 'CSS_DELETE_WORKGRP', :OLD.css_delete_workgrp, :NEW.css_delete_workgrp);
      END IF;
   END IF;

   IF UPDATING ('rid') THEN
      IF :OLD.rid != :NEW.rid THEN
         audit_trail.column_update (raid, 'RID', :OLD.rid, :NEW.rid);
      END IF;
   END IF;
END;
/