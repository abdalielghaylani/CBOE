CREATE OR REPLACE TRIGGER trg_delete_hitlist_info_perm
AFTER DELETE
ON mixtures
FOR EACH ROW
BEGIN
   DELETE FROM &&securitySchemaName..coesavedhitlist WHERE id = :OLD.MIX_INTERNAL_ID;
   DELETE FROM &&securitySchemaName..coetemphitlist WHERE id = :OLD.MIX_INTERNAL_ID;
END;
/
