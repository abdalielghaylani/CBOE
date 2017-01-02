REM PageControlSettings_coepagecontrol.trg

CREATE OR REPLACE TRIGGER trg_coepagecontrol
   BEFORE INSERT
   ON COEPAGECONTROL    FOR EACH ROW
BEGIN
   SELECT coepagecontrol_seq.NEXTVAL
     INTO :NEW.ID
     FROM DUAL;
END trg_coepagecontrol;
/