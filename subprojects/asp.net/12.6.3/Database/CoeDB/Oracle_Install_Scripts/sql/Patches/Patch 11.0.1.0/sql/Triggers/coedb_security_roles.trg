CREATE TRIGGER security_roles_trig
   BEFORE INSERT
   ON security_roles
   FOR EACH ROW
BEGIN
   SELECT security_roles_seq.NEXTVAL
     INTO :NEW.role_id
     FROM DUAL;
END;
/
