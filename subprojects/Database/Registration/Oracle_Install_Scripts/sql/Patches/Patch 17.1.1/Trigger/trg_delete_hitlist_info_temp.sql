CREATE OR REPLACE TRIGGER trg_delete_hitlist_info_temp
AFTER DELETE
ON temporary_compound
FOR EACH ROW
DECLARE
 linking_id NUMBER(8, 0);
BEGIN
 linking_id := :OLD.tempcompoundid;
 DELETE FROM &&securitySchemaName..coesavedhitlist WHERE &&securitySchemaName..coesavedhitlist.id = linking_id;
 DELETE FROM &&securitySchemaName..coetemphitlist WHERE &&securitySchemaName..coetemphitlist.id = linking_id;
END;
/