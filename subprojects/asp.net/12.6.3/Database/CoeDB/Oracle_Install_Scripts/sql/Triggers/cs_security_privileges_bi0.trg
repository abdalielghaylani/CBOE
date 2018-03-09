REM cs_security_privileges_bi0.trg
REM Generated on 28-JAN-05
REM Script to create BI audit trigger for the CS_SECURITY_PRIVILEGES table.

CREATE OR REPLACE TRIGGER trg_audit_cs_sec_priv_bi0
   BEFORE INSERT
   ON cs_security_privileges
   FOR EACH ROW
   WHEN (NEW.rid IS NULL OR NEW.rid = 0)
BEGIN
   SELECT TRUNC (seq_rid.NEXTVAL)
     INTO :NEW.rid
     FROM DUAL;
END;
/