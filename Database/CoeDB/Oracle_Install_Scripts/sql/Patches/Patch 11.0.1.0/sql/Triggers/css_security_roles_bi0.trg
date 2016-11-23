REM css_security_roles_bi0.trg
REM Generated on 28-JAN-05
REM Script to create BI audit trigger for the SECURITY_ROLES table.

CREATE OR REPLACE TRIGGER trg_audit_css_sec_roles_bi0
   BEFORE INSERT
   ON security_roles
   FOR EACH ROW
   WHEN (NEW.rid IS NULL OR NEW.rid = 0)
BEGIN
   SELECT TRUNC (seq_rid.NEXTVAL)
     INTO :NEW.rid
     FROM DUAL;
END;
/