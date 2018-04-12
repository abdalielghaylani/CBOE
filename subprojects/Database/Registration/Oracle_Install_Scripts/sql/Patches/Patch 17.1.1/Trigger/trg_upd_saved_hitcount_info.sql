CREATE OR REPLACE TRIGGER trg_upd_saved_hitcount_info
AFTER DELETE
ON coesavedhitlist
FOR EACH ROW
DECLARE
 linking_id NUMBER(9, 0);
BEGIN
 linking_id := :OLD.hitlistid; 
 UPDATE coesavedhitlistid SET number_hits = (number_hits-1) WHERE id = linking_id;   
END;
/