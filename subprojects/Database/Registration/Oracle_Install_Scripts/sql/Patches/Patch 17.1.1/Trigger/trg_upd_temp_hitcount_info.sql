CREATE OR REPLACE TRIGGER trg_upd_temp_hitcount_info
AFTER DELETE
ON coetemphitlist
FOR EACH ROW
DECLARE
 linking_id NUMBER(9, 0);
BEGIN
 linking_id := :OLD.hitlistid; 
 UPDATE coetemphitlistid SET number_hits = (number_hits-1) WHERE id = linking_id;   
END;
/