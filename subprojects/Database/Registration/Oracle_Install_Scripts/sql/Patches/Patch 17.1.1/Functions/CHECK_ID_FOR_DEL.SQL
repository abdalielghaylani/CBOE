
CREATE OR REPLACE FUNCTION CHECK_ID_FOR_DEL(I_ID NUMBER) RETURN NUMBER
-- function checks, that current ID in table for waiting to delete from temporary registration or not
AS
 CNT NUMBER;
BEGIN
 select COUNT(1) INTO CNT from FOR_DELETE_TEMP_REC d where d.id = I_ID;
 RETURN cnt;
END;
/
show errors
