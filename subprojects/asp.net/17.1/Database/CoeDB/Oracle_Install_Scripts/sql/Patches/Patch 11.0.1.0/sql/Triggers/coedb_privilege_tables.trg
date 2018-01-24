CREATE TRIGGER privilege_tables_trig
   BEFORE INSERT
   ON privilege_tables
   FOR EACH ROW
BEGIN
   SELECT privilege_tables_seq.NEXTVAL
     INTO :NEW.privilege_table_id
     FROM DUAL;
END;
/
