CREATE TRIGGER sites_trig
   BEFORE INSERT
   ON sites
   FOR EACH ROW
BEGIN
   SELECT sites_seq.NEXTVAL
     INTO :NEW.site_id
     FROM DUAL;
END;
/
