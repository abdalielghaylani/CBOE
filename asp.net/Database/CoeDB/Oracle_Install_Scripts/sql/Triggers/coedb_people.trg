CREATE TRIGGER &&schemaName..people_trig
   BEFORE INSERT
   ON &&schemaName..people
   FOR EACH ROW
   WHEN (NEW.person_id IS NULL)
BEGIN
   SELECT people_seq.NEXTVAL
     INTO :NEW.person_id
     FROM DUAL;
END;
/
